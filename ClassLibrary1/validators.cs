using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Validatoins
{

    public class Validator
    {

        /// <summary>
        /// this method return true when input has value or else return false 
        /// </summary>
        /// <param name="inputmsgValue"></param>
        /// <returns></returns>
        /// 

        public bool HasValue(string inputMsgValue)
        {
            return !string.IsNullOrEmpty(inputMsgValue);
        }

        /// <summary>
        /// If input match with  given regex return true or else return false 
        /// </summary>
        /// <param name="inputmsgValue"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool IsMatchRegex(string inputMsgValue, string pattern)
        {

            if (!HasValue(inputMsgValue))
                return true;
            Regex regex = new Regex(pattern);
            return regex.IsMatch(inputMsgValue);

        }
    }
}
