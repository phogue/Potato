using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Procon.Database.Serialization;

namespace Procon.Database.Test.Integration {
    public interface IDatabaseIntegration {
        void TestFindQuery(List<IDatabaseObject> setup, IDatabaseObject query, JArray expected);
    }
}
