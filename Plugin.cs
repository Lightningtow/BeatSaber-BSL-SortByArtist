using BetterSongList;
using IPA;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using IPALogger = IPA.Logging.Logger;

[assembly: AssemblyTitle("BetterSongList-ArtistSort")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyCopyright("MIT License")]

//[assembly: Guid("f1378c34-e815-4e04-9471-8be443b18e96")]

// A huge thank you to SteffanDonal,
// author of https://github.com/SteffanDonal/BeatSaber-BetterSongList-PlayCount,
// which I used as the basis for this mod.


namespace ArtistSort
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        static ArtistSort ArtistSorter => new ArtistSort();

        internal static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        public static readonly string Name = Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        public static readonly string Version = Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        public static IPALogger Log { get; internal set; }


        static bool _isInitialized;

        [Init]
        public void Init(object _, IPALogger log)
        {
            Log = log;

            SortMethods.RegisterPrimitiveSorter(ArtistSorter);
        }

        [OnStart]
        public void OnStart()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"Plugin had {nameof(OnStart)} called more than once! Critical failure.");

            _isInitialized = true;

            Log.Info($"v{Version} loaded!");
        }
    }
}
