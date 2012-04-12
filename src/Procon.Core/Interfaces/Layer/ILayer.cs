// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.ComponentModel;

namespace Procon.Core.Interfaces.Layer {
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Net;

    public interface ILayer : INotifyPropertyChanged
    {
        // Public Properties
        String Hostname { get; set; }
        UInt16 Port     { get; set; }
        Boolean IsEncrypted  { get; set; }
        Boolean IsCompressed { get; set; }
        ConnectionState ConnectionState { get; }

        // Events
        event LayerGame.ProcessLayerEventHandler ProcessLayerEvent;

        // Methods
        void Begin();
        void Shutdown();
        void Request(Context context, CommandName command, EventName @event, params Object[] arguments);
        Context ServerContext(String hostname, UInt16 port);
    }
}
