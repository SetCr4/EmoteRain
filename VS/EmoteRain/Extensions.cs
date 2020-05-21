using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace EmoteRain {
    public static class Extensions {
        /// <summary>
        /// Returns the full path of a GameObject in the scene hierarchy.
        /// </summary>
        /// <param name="gameObject">The instance of a GameObject to generate a path for.</param>
        /// <returns></returns>
        internal static string GetFullPath(this GameObject gameObject) {
            StringBuilder path = new StringBuilder();
            while(true) {
                path.Insert(0, "/" + gameObject.name);
                if(gameObject.transform.parent == null) {
                    path.Insert(0, gameObject.scene.name);
                    break;
                }
                gameObject = gameObject.transform.parent.gameObject;
            }
            return path.ToString();
        }
        /// <summary>
        /// Returns the full path of a Component in the scene hierarchy.
        /// </summary>
        /// <param name="component">The instance of a Component to generate a path for.</param>
        /// <returns></returns>
        internal static string GetFullPath(this Component component) {
            StringBuilder path = new StringBuilder(component.gameObject.GetFullPath());
            path.Append("/" + component.GetType().Name);
            return path.ToString();
        }

        internal static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            objects.Sort();
            return objects;
        }
    }
}
