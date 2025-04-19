using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.ServerAdmin.Data.Notifications
{
    public class GameServerDisconnected : INotification
    {
        public required string GameServerId { get; set; }
    }
}
