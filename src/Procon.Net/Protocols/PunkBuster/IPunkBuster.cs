using System.Text.RegularExpressions;

namespace Procon.Net.Protocols.PunkBuster {
    public interface IPunkBuster {

        /// <summary>
        /// Deserialize a regular expression match object into the the object.
        /// </summary>
        /// <param name="data"></param>
        void Deserialize(Match data);
    }
}
