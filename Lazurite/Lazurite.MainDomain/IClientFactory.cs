﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazurite.MainDomain
{
    public interface IClientFactory
    {
        IServer GetServer(ConnectionCredentials credentials);
        event Action<IServer, bool> ConnectionStateChanged;
    }
}
