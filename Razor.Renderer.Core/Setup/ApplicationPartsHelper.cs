using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Razor.Renderer.Core.Setup
{
    /// <summary>
    /// Helper to fetch all the resources in the project
    /// </summary>
    internal static class ApplicationPartsHelper
    {
        /// <summary>
        /// Get all the resources that are available in the projects
        /// </summary>
        /// <returns></returns>
        public static List<ApplicationPart> GetApplicationParts()
        {
            // Look over all assemblies to check if they're Razor Class Library assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            // Add all assemblies from /bin folder
            var assembliesFromBin = GetAllAssembliesFromBin();
            if (assembliesFromBin?.Any() ?? false)
            {
                assemblies.AddRange(assembliesFromBin);
            }

            var applicationParts = new List<ApplicationPart>();
            var uniqueAssemblies = new HashSet<Assembly>();

            foreach (var assembly in assemblies)
            {
                // only continue the loop for unique assemblies
                if (!uniqueAssemblies.Add(assembly))
                    continue;

                // fetch the related assemblies
                var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, false);
                foreach (var relatedAssembly in relatedAssemblies)
                {
                    // This assembly is auto generated pre compiled Razor Class Library
                    if (relatedAssembly.FullName?.Contains(".Views,") ?? false)
                    {
                        AddApplicationParts(ref applicationParts, relatedAssembly);
                    }
                }

                if (relatedAssemblies.Any())
                {
                    // Add application part of the main Razor Class Library assembly for View Component Features, ...
                    AddApplicationParts(ref applicationParts, assembly);
                }
            }

            return applicationParts;
        }

        private static void AddApplicationParts(ref List<ApplicationPart> applicationParts, Assembly assembly)
        {
            var applicationPartFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
            var applicationPartsAssembly = applicationPartFactory.GetApplicationParts(assembly);

            foreach (var applicationPartAssembly in applicationPartsAssembly)
            {
                if (!applicationParts.Any(a => a.Name == applicationPartAssembly.Name))
                    applicationParts.Add(applicationPartAssembly);
            }
        }

        /// <summary>
        /// Gets all the assemblies from /bin
        /// </summary>
        /// <returns>List of the assemblies in /bin</returns>
        private static IEnumerable<Assembly> GetAllAssembliesFromBin()
        {
            var executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;

            var assemblies = new List<Assembly>();

            if (!string.IsNullOrEmpty(executingAssemblyLocation))
            {
                var path = Path.GetDirectoryName(executingAssemblyLocation);
                var dlls = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);

                foreach (var dll in dlls ?? new string[] { })
                {
                    try
                    {
                        Assembly loadedAssembly = Assembly.Load(dll);
                        assemblies.Add(loadedAssembly);
                    }
                    catch (Exception) { }
                }
            }

            return assemblies;
        }
    }
}
