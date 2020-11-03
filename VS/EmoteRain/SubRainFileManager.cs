using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EmoteRain.Logger;
using UnityEngine;

namespace EmoteRain
{
    class SubRainFileManager
    {
        public static Dictionary<string, Texture2D> SubRainTextures = new Dictionary<string, Texture2D>();

        public static void load()
        {
            DirectoryInfo dir = Directory.CreateDirectory("CustomSubRain");
            FileInfo[] allFiles = dir.GetFiles();
            foreach(FileInfo e in allFiles)
            {
                if (e.Name.EndsWith(".png"))
                {
                    Texture2D tempTex = new Texture2D(2,2);
                    ImageConversion.LoadImage(tempTex, File.ReadAllBytes(e.FullName));
                    SubRainTextures.Add(e.Name.Remove(e.Name.Length - 4), tempTex);
                    Log("Cached " + e.Name.Remove(e.Name.Length - 4));
                }
            }
        }

        public static void reload()
        {
            SubRainTextures = new Dictionary<string, Texture2D>();
            load();
        }
    }
}
