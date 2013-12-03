using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization {
    public abstract class Serializer : ISerializer {

        /// <summary>
        /// Stores the currently working parsed object
        /// </summary>
        /// <remarks></remarks>
        protected IParsedQuery Parsed { get; set; }

        protected Serializer() {
            this.Parsed = new ParsedQuery();
        }

        public abstract ICompiledQuery Compile(IParsedQuery parsed);

        /// <summary>
        /// Compiles a query down to a single managable list of properties
        /// </summary>
        /// <returns></returns>
        public ICompiledQuery Compile() {
            return this.Compile(this.Parsed);
        }

        /// <summary>
        /// Parse a single method, creating all the tokens to then compile the data
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        public abstract ISerializer Parse(Method method, IParsedQuery parsed);

        public ISerializer Parse(Method method) {
            this.Parse(method, this.Parsed);

            return this;
        }

        public ISerializer Parse(IDatabaseObject query) {
            this.Parse(query as Method);

            return this;
        }
    }
}
