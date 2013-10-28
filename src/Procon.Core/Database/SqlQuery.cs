using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Database {

    /// <summary>
    /// Implements a modified Active Record pattern, but more so butchered from the CodeIgniter
    /// implementation. I just did this because it's handy to build sql queries.
    /// 
    /// It's heavily based on CodeIgniter's implementation because I use that a lot. It's neat, 
    /// not bloated and does not add the overhead that a full ORM would add.
    /// 
    /// It's also serializable across the AppDomain which is handy for plugins to you know, function.
    /// 
    /// @todo modify this statement as it's so butchered that refering to is as an Active Record implementation is funny.
    /// </summary>
    public class SqlQuery : Query {

        protected List<String> SelectList { get; set; } 

        public SqlQuery() {
            this.SelectList = new List<String>();
        }

        public virtual SqlQuery Select(params String[] names) {

            // Nothing was passed in, select everything.
            if (names.Length == 0) {
                this.SelectList.Add("*");
            }

            foreach (String name in names) {
                this.SelectList.Add(name.Trim());
            }

            return this;
        }


    }
}
