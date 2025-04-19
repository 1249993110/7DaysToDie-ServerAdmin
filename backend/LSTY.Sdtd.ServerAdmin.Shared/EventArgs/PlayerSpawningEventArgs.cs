using LSTY.Sdtd.ServerAdmin.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.ServerAdmin.Shared.EventArgs
{
    public class PlayerSpawningEventArgs : System.EventArgs
    {
        public required PlayerInfo PlayerInfo { get; set; }

        public required int ChunkViewDim { get; set; }

        public required PlayerProfile PlayerProfile { get; set; }

        public required DateTime Timestamp { get; set; }
    }
}
