using System;
using System.ComponentModel;

namespace Procon.Core.Shared {
    [Serializable]
    public class CoreModel : ICloneable {
        /// <summary>
        /// Event for whenever a property is modified on this executable object
        /// </summary>
        /// <remarks>I think this is only used for variables, which I would like to move specifically to
        /// the variables controlle. There is no need for other variables to use this functionality.</remarks>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="property"></param>
        protected void OnPropertyChanged(Object sender, String property) {
            var handler = this.PropertyChanged;
            if (handler != null) {
                handler(sender, new PropertyChangedEventArgs(property));
            }
        }

        public object Clone() {
            return this.MemberwiseClone();
        }
    }
}
