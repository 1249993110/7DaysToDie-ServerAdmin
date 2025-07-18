﻿using LSTY.Sdtd.ServerAdmin.Services.Settings;
using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;

namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    public class SharedState
    {
        public required Guid GameServerId { get; set; }
        public required CommonSettings CommonSettings { get; set; }
        public required IReadOnlyDictionary<Type, IProxy> RpcProxies { get; set; }
        public required IModEventProxy ModEventProxy { get; set; }
        public required IGameManageProxy GameManageProxy { get; set; }
        public required IServiceProvider ServiceProvider { get; set; }
        public required EventForwarder EventForwarder { get; set; }
    }
}
