using System;
using System.IO;

namespace Procon.Core {
    using Procon.Core.Localization;
    using Procon.Core.Utils;

    public abstract class ExecutableBase : MarshalByRefObject {
        public static Config             MasterConfig    = new Config().LoadDirectory(new DirectoryInfo(Defines.CONFIGS_DIRECTORY));
        public static LanguageController MasterLanguages = new LanguageController().Execute();
    }
}
