using System.Collections.Generic;
using System.Linq;
using Procon.Database.Serialization.Builders;

namespace Procon.Database.Serialization {
    /// <summary>
    /// Base class for serializing a collection of database objects
    /// to a usable compiled query
    /// </summary>
    public abstract class Serializer : ISerializer {

        /// <summary>
        /// Stores the currently working parsed object
        /// </summary>
        /// <remarks></remarks>
        private IParsedQuery Parsed { get; set; }

        protected Serializer() {
            this.Parsed = new ParsedQuery();
        }

        /// <summary>
        /// Compile a parsed query
        /// </summary>
        /// <param name="parsed"></param>
        /// <returns></returns>
        public abstract ICompiledQuery Compile(IParsedQuery parsed);

        /// <summary>
        /// Compiles a query down to a single managable list of properties
        /// </summary>
        /// <returns></returns>
        public ICompiledQuery Compile() {
            return this.Compile(this.Parsed);
        }

        /// <summary>
        /// Parses all children of the method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        protected List<IParsedQuery> ParseChildren(Method method) {
            List<IParsedQuery> children = new List<IParsedQuery>();

            foreach (Method child in method.Where(child => child is Method)) {
                IParsedQuery parsedChild = new ParsedQuery();

                this.Parse(child, parsedChild);

                children.Add(parsedChild);
            }

            return children;
        }

        /// <summary>
        /// Parse a single method, creating all the tokens to then compile the data
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        public abstract ISerializer Parse(Method method, IParsedQuery parsed);

        /// <summary>
        /// Converts a query into the required query (String by default), however
        /// the object may also be populated with additional requirements 
        /// </summary>
        /// <param name="method">The method object</param>
        /// <returns></returns>
        public ISerializer Parse(Method method) {
            this.Parse(method, this.Parsed);

            return this;
        }

        /// <summary>
        /// Alias for Parse(Method method) without requirement of caller to convert type.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public ISerializer Parse(IDatabaseObject query) {
            this.Parse(query as Method);

            return this;
        }
    }
}
