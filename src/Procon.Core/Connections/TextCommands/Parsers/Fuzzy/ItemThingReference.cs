using System.Collections.Generic;
using System.Linq;
using Procon.Fuzzy.Tokens.Object;
using Procon.Net.Shared.Models;

namespace Procon.Core.Connections.TextCommands.Parsers.Fuzzy {
    /// <summary>
    /// A reference for Procon.Fuzzy to use for item information (weapons etc)
    /// </summary>
    public class ItemThingReference : IThingReference {
        /// <summary>
        /// The items attached to this thing.
        /// </summary>
        public List<Item> Items { get; set; }

        public bool CompatibleWith(IThingReference other) {
            return other is ItemThingReference || other is PlayerThingReference;
        }

        public IThingReference Union(IThingReference other) {
            ItemThingReference itemThingReference = other as ItemThingReference;

            if (itemThingReference != null) {
                this.Items.AddRange(itemThingReference.Items.Where(item => this.Items.Contains(item) == false));
            }

            return this;
        }

        public IThingReference Complement(IThingReference other) {
            ItemThingReference itemThingReference = other as ItemThingReference;

            if (itemThingReference != null) {
                this.Items.RemoveAll(item => itemThingReference.Items.Contains(item));
            }

            return this;
        }
    }
}
