using LSTY.Sdtd.ServerAdmin.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.ServerAdmin.Services.Settings
{
    public class GeneralSettings : ISettings
    {
        public required bool IsEnabled { get; set; }
    }
}
