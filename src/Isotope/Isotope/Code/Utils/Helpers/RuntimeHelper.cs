using System;
using System.Collections.Generic;
using System.Reflection;

namespace Isotope.Code.Utils.Helpers
{
    /// <summary>
    /// Helper utilities for configuring the runtime.
    /// </summary>
    public static class RuntimeHelper
    {
        /// <summary>
        /// Instantiates and returns instances of all matching types in all own assemblies.
        /// </summary>
        public static IEnumerable<T> GetAllInstances<T>()
        {
            var targetType = typeof(T);

            foreach (var type in Assembly.GetExecutingAssembly().DefinedTypes)
            {
                if(type.IsGenericTypeDefinition || type.IsAbstract || type.IsInterface)
                    continue;

                if(!targetType.IsAssignableFrom(type))
                    continue;

                yield return (T) Activator.CreateInstance(type);
            }
        }
    }
}
