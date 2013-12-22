using System.Collections.Generic;
using System.Linq;
using Procon.Database.Shared.Builders;
using Procon.Database.Shared.Builders.Modifiers;
using Procon.Database.Shared.Builders.Values;

namespace Procon.Database.Shared {
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
        protected List<IParsedQuery> ParseChildren(IMethod method) {
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
        public abstract ISerializer Parse(IMethod method, IParsedQuery parsed);

        /// <summary>
        /// Converts a query into the required query (String by default), however
        /// the object may also be populated with additional requirements 
        /// </summary>
        /// <param name="method">The method object</param>
        /// <returns></returns>
        public ISerializer Parse(IMethod method) {
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

        /// <summary>
        /// Fetches the numeric value from a skip object
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual long? ParseSkip(IDatabaseObject query) {
            return query.Where(skip => skip is Skip && skip.Any(value => value is NumericValue)).Select(skip => ((NumericValue)skip.First(value => value is NumericValue)).Long).FirstOrDefault();
        }

        /// <summary>
        /// Fetches the numeric value from a limit object
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        protected virtual long? ParseLimit(IDatabaseObject query) {
            return query.Where(limit => limit is Limit && limit.Any(value => value is NumericValue)).Select(limit => ((NumericValue)limit.First(value => value is NumericValue)).Long).FirstOrDefault();
        }

    }
}
