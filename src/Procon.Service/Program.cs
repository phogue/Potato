using System;
using System.ServiceProcess;

namespace Procon.Service {
    internal class Program {
        [STAThread, LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        private static void Main() {
            ServiceBase.Run(new ServiceBase[] { 
                new ProconService() 
            });
        }
    }
}