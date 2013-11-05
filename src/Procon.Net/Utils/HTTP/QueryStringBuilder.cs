using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Collections.Specialized;

namespace Procon.Net.Utils.HTTP {
    /// <summary><![CDATA[
    /// Used to build complex query strings to be used in
    /// GET and POST HTTP operations.
    /// 
    /// Complex being query strings where the keys have 
    /// indexed arrays or associative.. so key[0]=value&key[1]=value 
    /// as well as sub keys, key[0][subkeyname1]=value&key[1][subkeyname2]=value 
    /// 
    /// Some developers love using these things, like the crazy developers on metabans
    /// and such.
    /// 
    /// This class is not built with optimization in mind, but flexability.
    /// If you are thinking of using this more than a few times a second you may
    /// want to use something more specialized or look into making this class better :)
    /// 
    /// ]]></summary>
    [Serializable]
    public class QueryStringBuilder : NameObjectCollectionBase {

        public void Add(String name, String value) {
            this.BaseAdd(name, value);
        }
        
        public void Add(String name, List<Object> value) {
            this.BaseAdd(name, value);
        }

        public void Add(String name, Dictionary<String, Object> value) {
            this.BaseAdd(name, value);
        }

        /// <summary>
        /// Removes all elements in the Query.
        /// </summary>
        public void Clear() {
            this.BaseClear();
        }

        /// <summary>
        /// Removes a specific key from the query
        /// </summary>
        /// <param name="name">key</param>
        public void Remove(String name) {
            this.BaseRemove(name);
        }

        public Object this[string key] {
            get {
                return this.BaseGet(key);
            }
        }

        /// <summary>
        /// Converts a stack of keys into a indexed key in the query string
        /// keyStack = { "hello" } returns "hello"
        /// keyStack = { "hello", "there", "0" } returns "hello[there][0]"
        /// </summary>
        /// <param name="keyStack">Array of keys</param>
        /// <returns>Indexed key of the query string</returns>
        private static String KeyStackToString(ICollection<String> keyStack) {
            String key = keyStack.FirstOrDefault();

            if (keyStack.Count > 1 && key != null) {
                return Uri.EscapeDataString(key) + "[" + String.Join("][", keyStack.Skip(1).Select(Uri.EscapeDataString).ToArray()) + "]";
            }
            if (keyStack.Count == 1 && key != null) {
                return Uri.EscapeDataString(key);
            }
            
            // the hell is the keystack == 0 for?
            return String.Empty;
        }

        /// <summary>
        /// Recursively appends keyvaluepairs to vars. If the value is a dictionary or a list then it will
        /// append the key from the dictionary, or the index of the list, to the keyStack then
        /// append this to the vars.
        /// 
        /// vars = {};
        /// keyStack = "word"
        /// value = { "Zero", "One }
        /// 
        /// Will eventually make vars = { { "word[0]", "Zero" }, { "word[1]", "One" } }
        /// 
        /// </summary>
        /// <param name="vars">Dictionary of values to append the key/value pairs to</param>
        /// <param name="keyStack">Where the indexed key is at</param>
        /// <param name="value">Object of string, List or Dictionary. Other values are ignored.</param>
        private void AppendKeysValues(IDictionary<String, String> vars, Stack<String> keyStack, Object value) {

            if (value is String) {
                vars.Add(KeyStackToString(keyStack.Reverse().ToArray()), Uri.EscapeDataString((String)value));
            }
            else {
                List<object> list = value as List<object>;
                if (list != null) {
                    List<Object> valueList = list;

                    for (int i = 0; i < valueList.Count; i++) {
                        keyStack.Push(i.ToString(CultureInfo.InvariantCulture));

                        this.AppendKeysValues(vars, keyStack, valueList[i]);

                        keyStack.Pop();
                    }
                }
                else if (value is Dictionary<String, Object>) {
                    Dictionary<String, Object> valueDictionary = ((Dictionary<String, Object>)value);

                    foreach (KeyValuePair<String, Object> kvp in valueDictionary) {
                        keyStack.Push(kvp.Key);

                        this.AppendKeysValues(vars, keyStack, kvp.Value);

                        keyStack.Pop();
                    }
                }
            }
        }

        public override String ToString() {
            Dictionary<String, String> vars = new Dictionary<String, String>();
            Stack<String> keyStack = new Stack<String>();

            foreach (String key in this.Keys) {
                keyStack.Push(key);

                this.AppendKeysValues(vars, keyStack, this[key]);

                keyStack.Pop();
            }

            return String.Join("&", vars.Select(x => String.Format("{0}={1}", x.Key, x.Value)).ToArray());
        }
    }
}
