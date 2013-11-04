using System;
using System.Linq;
using System.Xml.Linq;

namespace Procon.Nlp.Tokens.Object {
    using Procon.Nlp.Utils;

    public class PropertyObjectToken : ObjectToken {

        /// <summary>
        /// The name of the property being referred to by this token.
        /// </summary>
        public string PropertyName { get; set; }

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            TokenReflection.SelectDescendants(state.Document, typeof(PropertyObjectToken)).Descendants("Properties").Descendants("Match").Select(element => {
                var text = element.Attribute("text");
                var propertyName = element.Parent != null ? element.Parent.Attribute("name") : null;

                return new PropertyObjectToken() {
                    Text = phrase.Text,
                    Value = text != null ? text.Value : null,
                    PropertyName = propertyName != null ? propertyName.Value : null,
                    Similarity = text != null ? text.Value.StringSimularityRatio(phrase.Text) : 0 
                };
            }).Where(property => property.Similarity >= Token.MinimumSimilarity)
            .ToList()
            .ForEach(phrase.Add);

            return phrase;
        }

        public override string ToString() {
            return String.Format("{0},{1}", base.ToString(), this.PropertyName);
        }
    }
}
