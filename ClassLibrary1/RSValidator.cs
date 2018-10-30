using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace RsValidations
{

    public class RSValidator
    {
        public Dictionary<string, string> OutputErrors = new Dictionary<string, string> { };

        private string msgRule;
        private string msgvalue;
        private string dot = ".";

        public enum commonerrormesgs
        { required, regex };

        Dictionary<commonerrormesgs, string> ErrorMessages = new Dictionary<commonerrormesgs, string>  {
         {commonerrormesgs.required,"The {{fieldname}} must be required "},
         {commonerrormesgs.regex,"The {{fieldname}} is not given pattern"},};


        IDictionary<string, string> customizedmessages;

        public static RSValidator RSValidate(IDictionary<string, object> ValidateValue, IDictionary<string, string> ValidationRule, IDictionary<string, string> CustomMessages)
        {

            RSValidator obj = new RSValidator();
            obj.customizedmessages = CustomMessages;

            string[] validatevalueKey = ValidateValue.Keys.ToArray();
            var validatevalueval = ValidateValue.Values.ToArray();


            for (int i = 0; i <= validatevalueKey.Length - 1; i++)
            {

                if (ValidationRule.ContainsKey(validatevalueKey[i]))
                {

                    var value = validatevalueKey[i];
                    string Rules;
                    ValidationRule.TryGetValue(validatevalueKey[i], out Rules);

                    string ruleslwr = Rules.ToLower();
                    string[] Ruleslist = ruleslwr.Split('|'); //split the given string


                    int ruleslength = Ruleslist.Length;  //array length


                    for (int j = 0; j < ruleslength; j++)
                    {
                        string[] lettersnums = Ruleslist[j].Split(':');

                        commonerrormesgs ErrorKey = (commonerrormesgs)Enum.Parse(typeof(commonerrormesgs), lettersnums[0]);

                        switch (ErrorKey)
                        {
                            case commonerrormesgs.required:

                                if (!obj.HasValue(Convert.ToString(validatevalueval[i])))
                                 obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                 else break;
                                 break;

                            case commonerrormesgs.regex:
                                 if (!obj.IsMatchRegex(Convert.ToString(validatevalueval[i]), lettersnums[1]))
                                obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                break;
                        }
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// Has values return true or else return false 
        /// </summary>
        /// <param name="inputmsgValue"></param>
        /// <returns></returns>
        bool HasValue(string inputMsgValue)
        {
            return !string.IsNullOrEmpty(inputMsgValue);
        }
        void CustomMessages(string inputMsgKey, commonerrormesgs errorKey)
        {

            string errorMsg;
            ErrorMessages.TryGetValue(errorKey, out errorMsg);

            msgRule = string.Concat(inputMsgKey, dot, errorKey);

            if (customizedmessages.ContainsKey(msgRule))
            {
                customizedmessages.TryGetValue(msgRule, out msgvalue);

                OutputErrors.Add(inputMsgKey, msgvalue);
            }

            else
            {
                OutputErrors.Add(inputMsgKey, errorMsg.Replace("{{fieldname}}", inputMsgKey));
            }

        }

        /// <summary>
        /// If input match given regex return true or else return false 
        /// </summary>
        /// <param name="inputmsgValue"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        bool IsMatchRegex(string inputMsgValue, string pattern)
        {
            if (!HasValue(inputMsgValue))
                return true;
            Regex regex = new Regex(pattern);
            return regex.IsMatch(inputMsgValue);
        }
    }
}

            