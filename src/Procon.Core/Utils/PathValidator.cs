// Copyright 2011 Nick 'MorpheusX(AUT)' Mueller
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace Procon.Core.Utils
{
    public static class PathValidator
    {
        /// <summary>
        /// Makes sure the given path just contains legal characters
        /// </summary>
        /// <param name="validate">string to validate</param>
        /// <returns>valid string</returns>
        public static string Valdiate(string validate)
        {
            Regex pattern = new Regex(@"[^A-Za-z0-9._-]");
            if (pattern.IsMatch(validate))
            {
                MatchEvaluator matchEvaluator = new MatchEvaluator(replaceMethod);
                validate = pattern.Replace(validate, matchEvaluator);
            }
            return validate;
        }

        // I know it's kinda fucked up replacing some chars with themselves,
        // but I failed at writing the appropirate regex-pattern
        private static string replaceMethod(Match m)
        {
            switch (m.Value.ToLower())
            {
                case "ä":
                    return "ae";
                case "ö":
                    return "oe";
                case "ü":
                    return "ue";
                case "ß":
                    return "ss";
                default:
                    return "_";
            }
        }
    }
}
