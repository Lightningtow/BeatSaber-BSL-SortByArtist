using BetterSongList.Interfaces;
using BetterSongList.SortModels;
using BetterSongList.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IPA.Utilities;

namespace ArtistSort
{
    public class ArtistSort : ISorterPrimitive, ISorterWithLegend, ITransformerPlugin
    {
        // static PlayerDataModel playerDataModel;
        public bool isReady => true;
        public string name => "Song Artist";
        public bool visible => true;

        public IEnumerable<KeyValuePair<string, int>> BuildLegend(BeatmapLevel[] levels)
            => SongListLegendBuilder.BuildFor(levels, level => CalculateGroupNameForArtist(level.songAuthorName));

        private static string CalculateGroupNameForArtist(string artist)
        {
            artist = artist.Trim();
            if (string.IsNullOrEmpty(artist)) return "-";
            char c = artist[0];
            // try { c = artist.Trim()[0]; } catch (Exception e) { return "#"; } 
            if (Char.IsNumber(c)) return "#";
            if (Char.IsLetter(c)) return c.ToString().ToUpper();
            Plugin.Log.Info("Artist char not handled: >" + c.ToString() + "< in " + artist);
            return "#";
        }

        public float? GetValueFor(BeatmapLevel song) => ConvertStringToFloat(song.songAuthorName); // this is the line that determines the actual order of the songs
        
        private static float ConvertStringToFloat(string input)
        {
            input = input.Trim();
            // 340282300000000000000000000000000000000 largest value storable in a float. changed the last 3 to a 2 to give it some wiggle room, protect against overflows
            if (string.IsNullOrEmpty(input)) return 340282200000000000000000000000000000000f; // put at bottom of list
            if (!char.IsLetterOrDigit(input[0])) return 0.0f; // put at top of list

            double result = 0.0;
            double power = 1.0;
            int baseN = 26; 

            string normalizedInput = input.ToLowerInvariant(); // make input string lowercase

            foreach (char c in normalizedInput) // turn the artist as a string into a number
            {
                int charValue;
                if (c >= 'a' && c <= 'z')
                {
                    charValue = (int)c - (int)'a' + 1;     // a == 1, b == 2, z == 26
                  //  charValue = baseN - (standardValue - 1);      // reverse a == 26, z == 1
                }
                else { charValue = 0; } // non letter characters become 0
                power *= baseN; 
                result += charValue / power; 
            }

            result *= 1000000000000000; // to prevent losing precision when converting to float
            float newresult = Convert.ToSingle(result); // convert to float
            return newresult;
        }        

        public Task Prepare(CancellationToken cancelToken) => Task.CompletedTask;
        public void ContextSwitch(SelectLevelCategoryViewController.LevelCategory levelCategory, BeatmapLevelPack playlist) { }
    }
}
