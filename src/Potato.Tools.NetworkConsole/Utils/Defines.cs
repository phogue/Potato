#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.IO;

namespace Potato.Tools.NetworkConsole.Utils {
    public static class Defines {
        public static readonly string Potato_NET_CONSOLE_EXE = "Potato.Tools.NetworkConsole.exe";
        public static readonly string Potato_NET_CONSOLE_XML = "Potato.Tools.NetworkConsole.xml";

        public static readonly string CONFIGS_DIRECTORY = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");

        public static readonly string CONFIGS_DIRECTORY_Potato_NET_CONSOLE_XML = Path.Combine(CONFIGS_DIRECTORY, Potato_NET_CONSOLE_XML);
    }
}
