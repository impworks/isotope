using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Isotope.Code.Utils.Helpers;
using Isotope.Data;
using Isotope.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Isotope.Code.Services.Jobs
{
    /// <summary>
    /// Background job runner and manager.
    /// </summary>
    public class BackgroundJobService: IHostedService
    {
        public BackgroundJobService(IServiceScopeFactory scopeFactory, ILogger logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            
            _jobs = new ConcurrentDictionary<int, JobDescriptor>();
        }
        
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;

        private ConcurrentDictionary<int, JobDescriptor> _jobs;

        /// <summary>
        /// Starts the job supervisor service.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var job in await LoadPendingJobsAsync())
                Task.Run(() => ExecuteJobAsync(job));
        }

        /// <summary>
        /// Stops the job supervisor service.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var job in _jobs.Values.ToList())
                job.Cancellation.Cancel();
            
            return Task.CompletedTask;
        }

        #region Public interface

        /// <summary>
        /// Runs a new job.
        /// </summary>
        public async Task RunAsync(JobBuilder jb)
        {
            var def = await CreateDescriptorAsync(jb);
            await ExecuteJobAsync(def);
        }

        /// <summary>
        /// Terminates all jobs related to the specified key.
        /// </summary>
        public void Cancel(string key)
        {
            var jobsToTerminate = _jobs.Values.Where(x => x.ResourceKey == key).ToList();
            foreach (var job in jobsToTerminate)
                job.Cancellation.Cancel();
        }
        
        #endregion

        #region Helpers

        /// <summary>
        /// Loads all incomplete jobs from the database.
        /// </summary>
        private async Task<List<JobDescriptor>> LoadPendingJobsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var di = scope.ServiceProvider;

            var db = di.GetRequiredService<AppDbContext>();
            var states = await db.JobStates
                                 .AsNoTracking()
                                 .Where(x => x.FinishDate == null)
                                 .OrderBy(x => x.StartDate)
                                 .ToListAsync();

            var result = new List<JobDescriptor>();
            foreach (var state in states)
            {
                var jobType = RuntimeHelper.FindType(state.Type);
                var job = (IJob) di.GetRequiredService(jobType);
                var args = GetArguments(state);

                result.Add(new JobDescriptor
                {
                    Job = job,
                    Arguments = args,
                    ResourceKey = job.GetResourceKey(args),
                    Cancellation = new CancellationTokenSource(),
                    JobStateId = state.Id
                });
            }

            return result;

            object GetArguments(JobState state)
            {
                return !string.IsNullOrEmpty(state.Arguments)
                    ? JsonConvert.DeserializeObject(state.Arguments, RuntimeHelper.FindType(state.ArgumentsType))
                    : null;
            }
        }

        /// <summary>
        /// Persists job information in the database.
        /// </summary>
        private async Task<JobDescriptor> CreateDescriptorAsync(JobBuilder jb)
        {
            using var scope = _scopeFactory.CreateScope();
            var di = scope.ServiceProvider;
            
            var state = new JobState
            {
                StartDate = DateTime.Now,
                Type = jb.Type.FullName,
                Arguments = jb.Args != null ? JsonConvert.SerializeObject(jb.Args) : null,
                ArgumentsType = jb.Args?.GetType().FullName,
            };
            var db = di.GetRequiredService<AppDbContext>();
            db.JobStates.Add(state);
            await db.SaveChangesAsync();

            var job = (IJob) di.GetRequiredService(jb.Type);
            return new JobDescriptor
            {
                Job = job,
                Arguments = jb.Args,
                ResourceKey = job.GetResourceKey(jb.Args),
                Cancellation = new CancellationTokenSource(),
                JobStateId = state.Id
            };
        }

        /// <summary>
        /// Runs the specified job.
        /// </summary>
        private async Task ExecuteJobAsync(JobDescriptor def)
        {
            _jobs.TryAdd(def.JobStateId, def);
            
            var success = false;

            try
            {
                await def.Job.ExecuteAsync(def.Arguments, def.Cancellation.Token);
                _logger.Information($"Job {def} has completed successfully.");
                success = true;
            }
            catch (TaskCanceledException)
            {
                _logger.Information($"Job {def} has been cancelled.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Demystify(), $"Job {def} has failed.");
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var state = await db.JobStates.FirstOrDefaultAsync(x => x.Id == def.JobStateId);
                state.Success = success;
                state.FinishDate = DateTime.Now;
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Demystify(), $"Failed to save {def} state.");
            }
            
            _jobs.TryRemove(def.JobStateId, out _);
        }

        #endregion
    }
}