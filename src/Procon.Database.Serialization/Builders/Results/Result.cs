using System.Linq;
using Newtonsoft.Json.Linq;

namespace Procon.Database.Serialization.Builders.Results {
    public class Result : DatabaseObject {

        /// <summary>
        /// Converting the responses to JArray is much easier to work with
        /// </summary>
        /// <returns></returns>
        public JArray ToJArray() {
            JArray array = new JArray();

            foreach (Document document in this.Where(row => row is Document)) {
                JObject data = new JObject();

                foreach (Assignment assignment in document.Where(statement => statement is Assignment)) {
                    Field field = assignment.FirstOrDefault(statement => statement is Field) as Field;
                    Value value = assignment.FirstOrDefault(statement => statement is Value) as Value;

                    if (field != null && value != null) {
                        data.Add(new JProperty(field.Name, value.ToObject()));
                    }
                }

                array.Add(data);
            }

            return array;
        }
    }
}
