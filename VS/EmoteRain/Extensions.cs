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
    }
}
