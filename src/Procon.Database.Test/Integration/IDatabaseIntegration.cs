using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Procon.Database.Shared;

namespace Procon.Database.Test.Integration {
    public interface IDatabaseIntegration {
        void TestFindQuery(IEnumerable<IDatabaseObject> setup, IDatabaseObject query, JArray expected);
    }
}
