using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DOTSRTS.Utilities.DOTS.Editor
{
    public static class ProjectTools
    {
        public static List<Type> GetAllTypesInProject()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Create a list to store all the types
            var allTypes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                try
                {
                    // Get all types from the current assembly
                    var types = assembly.GetTypes();
                    allTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    allTypes.AddRange(ex.Types.Where(type => type != null));
                }
            }


            return allTypes;
        }
    }
}