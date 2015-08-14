#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Net;

namespace Potato.Core.Test.Remote {
    public class TestRemote {
        protected void SetUp() {

            // We could actually validate the certificate in /Certificates directory
            // but for unit testing I find this acceptable enough.

            // If you're reading this, never put this into production (anywhere.)
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }
    }
}