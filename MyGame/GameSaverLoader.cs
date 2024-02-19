using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Project2.MyGame
{
    internal static class GameSaverLoader
    {
        public struct Save
        {
            public long Time;
            public int Points;
            public uint LevelData;
        }

        public unsafe static void SaveGame(Save save)
        {
            string path = Assembly.GetExecutingAssembly().Location;
            path = Path.Combine(Path.GetDirectoryName(path), "saves");
            using FileStream fs = File.Open(path, FileMode.Append);

            int size = Marshal.SizeOf<Save>();
            byte[] buffer = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(save, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
            Marshal.FreeHGlobal(ptr);
            
            fs.Write(buffer);
            fs.Flush();
        }

        public unsafe static Save[] GetSaves()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            path = Path.Combine(Path.GetDirectoryName(path), "saves");
            Console.WriteLine($"Reading saves from '{path}'");
            using FileStream fs = File.Open(path, FileMode.OpenOrCreate);

            int stride = Marshal.SizeOf<Save>();
            byte[] buffer = new byte[stride];

            List<Save> saves = new List<Save>();
            while (fs.Position < fs.Length - stride)
            {
                fs.Read(buffer, 0, stride);
                IntPtr ptr = Marshal.AllocHGlobal(stride);
                Marshal.Copy(buffer, 0, ptr, stride);
                saves.Add((Save)Marshal.PtrToStructure(ptr, typeof(Save)));
                Marshal.FreeHGlobal(ptr);
            }

            return saves.ToArray();
        }

    }
}
