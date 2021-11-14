using SdlEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineKart
{
    // TODO: Should this be a "system"?
    // TODO: It could have the spawning / pruning code from the generator as well

    // This data is pulled out of the generator for easy access by other code
    // Mostly used to ensure render position stays aligned with the track
    public class TrackCollection
    {
        public int FirstActiveObjectIndex { get; set; } = -1;
        public int LastActiveObjectIndex { get; set; } = -1;

        public Dictionary<int, GameObject> TrackSegments { get; set; } = new Dictionary<int, GameObject>();
    }
}
