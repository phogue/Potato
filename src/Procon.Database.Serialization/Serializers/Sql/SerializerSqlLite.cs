using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization.Serializers.Sql {
    /// <summary>
    /// Serializer for SqlLite support.
    /// </summary>
    public class SerializerSqlLite : SerializerSql {

        public override ICompiledQuery Compile(IParsedQuery parsed) {
           
            return null;
        }

        public override ISerializer Parse(Method method, IParsedQuery parsed) {

            return this;
        }
    }
}
