using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Isotope.Code.Utils
{
    /// <summary>
    /// Helper utilities for configuring the runtime.
    /// </summary>
    public static class RuntimeHelper
    {
        /// <summary>
        /// Enumerates all assemblies related to current one.
        /// </summary>
        public static IEnumerable<Assembly> GetOwnAssemblies()
        {
            var rootAsm = Assembly.GetEntryAssembly();
            var namePrefix = rootAsm.FullName.Split(new[] {", "}, StringSplitOptions.None)[0];
            ForceLoadReferences(rootAsm, namePrefix);
            return AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName?.StartsWith(namePrefix) == true);
        }

        /// <summary>
        /// Instantiates and returns instances of all matching types in all own assemblies.
        /// </summary>
        public static IEnumerable<T> GetAllInstances<T>()
        {
            var targetType = typeof(T);
            foreach (var asm in GetOwnAssemblies())
            {
                var types = asm.GetTypes().Where(x => x.IsConcrete() && targetType.IsAssignableFrom(x));
                foreach (var type in types)
                {
                    yield return (T) Activator.CreateInstance(type);
                }
            }
        }

        /// <summary>
        /// Returns true if the type is an implementation of the specified generic.
        /// </summary>
        public static bool ImplementsGeneric(this Type type, Type other)
        {
            return type == other
                || (type.IsGenericType && type.GetGenericTypeDefinition() == other);
        }

        /// <summary>
        /// Checks if the type can be instantiated via CreateInstance.
        /// </summary>
        public static bool IsConcrete(this Type type)
        {
            return !type.IsAbstract
                && !type.IsInterface
                && !type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// Ensures that all recursive references are loaded.
        /// </summary>
        private static void ForceLoadReferences(Assembly asm, string namePrefix)
        {
            var loaded = AppDomain.CurrentDomain.GetAssemblies().ToDictionary(x => x.FullName, x => x);
            LoadRecursively(asm);

            void LoadRecursively(Assembly currAsm)
            {
                var refs = currAsm.GetReferencedAssemblies();
                foreach (var r in refs)
                {
                    if (r.FullName?.StartsWith(namePrefix) != true)
                        continue;

                    if(!loaded.ContainsKey(r.FullName))
                        loaded[r.FullName] = Assembly.Load(r);

                    LoadRecursively(loaded[r.FullName]);
                }
            }
        }
    }
}
