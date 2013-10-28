using System;
using System.ServiceProcess;

namespace Procon.Service {

    internal class Program {

        [STAThread, LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        private static void Main(string[] args) {
            ServiceBase.Run(new ServiceBase[] { 
                new ProconService() 
            });
        }
    }
}