using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Validatoins;
using Errormessages;
namespace RsValidations
{   

    public class RSValidator
    {
       public CommonErrors errormsgs = new CommonErrors();
         
         public string KEY = "firstname";
         Validator obj = new Validator();
        
      
        IDictionary<string, string> customizedmessages;

        public static RSValidator RSValidate(IDictionary<string, object> validateValue, IDictionary<string, string> validationRule, IDictionary<string, string> CustomMessages)
        {
           
            RSValidator obj = new RSValidator();
            obj.customizedmessages = CustomMessages;

            string[] validatevalueKey = validateValue.Keys.ToArray();


            for (int i = 0; i <= validatevalueKey.Length - 1; i++)
            {
                obj.InputData(validatevalueKey[i], validateValue, validationRule);
            }
            return obj;
        }

       void InputData(string valiDateValueKey, IDictionary<string, object> valiDateValue, IDictionary<string, string> valiDationRule)
        {
            string currentRule;
            object currentValue;
            string currentKey;

            currentKey = valiDateValueKey;
            currentValue = InputValue(currentKey, valiDateValue);
            currentRule = InputRule(currentKey, valiDationRule);
            Operation(currentRule, currentValue, currentKey,customizedmessages);

        }

        /// <summary>
        /// Get the value associated with the particular key
        /// </summary>
        /// <param name="currentKey"></param>
        /// <param name="validateValue"></param>
        /// <returns></returns>
        object InputValue(string currentKey, IDictionary<string, object> validateValue)
        {
            object rules;
             validateValue.TryGetValue(currentKey, out rules);
             return rules;
        }
        /// <summary>
        /// Get the rule  associated with the particular key 
        /// </summary>
        /// <param name="currentkey"></param>
        /// <param name="validationRule"></param>
        /// <returns></returns>

        string InputRule(string currentkey, IDictionary<string, string> validationRule)
        {
            string inputrules;
            validationRule.TryGetValue(currentkey, out inputrules);
            return inputrules;
        }
        /// <summary>
        /// split the rules and call particular validation method.if validation fail call errormessages method
        /// </summary>
        /// <param name="currentRule"></param>
        /// <param name="currentValue"></param>
        /// <param name="currentKey"></param>
        /// <param name="customizedmessages"></param>
        void Operation(string currentRule, object currentValue, string currentKey, IDictionary<string, string> customizedmessages)
        {


            string rulesLwr = currentRule.ToLower();
            string[] rulesList = rulesLwr.Split('|');
            int rulesLength = rulesList.Length;



            try
            {
                for (int j = 0; j < rulesLength; j++)
                {
                    string[] lettersnums = rulesList[j].Split(':');

                    commonerrormesgs errorKey = (commonerrormesgs)Enum.Parse(typeof(commonerrormesgs), lettersnums[0]);

                    switch (errorKey)
                    {
                        case commonerrormesgs.required:

                            if (!obj.HasValue(Convert.ToString(currentValue)))
                                errormsgs.GetErrorMessages(currentKey, errorKey, customizedmessages);
                            else break;
                            break;

                        case commonerrormesgs.regex:
                            if (lettersnums.Count() == 1 || string.IsNullOrEmpty(lettersnums[1]))
                                throw new Exception("Regexmethod received a null argument!");

                            if (!obj.IsMatchRegex(Convert.ToString(currentValue), lettersnums[1]))
                                errormsgs.GetErrorMessages(currentKey, errorKey, customizedmessages);
                            break;
                    }

                }

            }

            catch (ArgumentException)
            {
                throw new Exception("Invalid argument");

            }
        }


        public string value
        {
            get
            { 

                if (errormsgs.OutputErrors.ContainsKey(KEY))
                {
                    return errormsgs.OutputErrors[KEY];
                }
                else
                {
                    return "null";
                }
            }

        }

        public bool HasError
        {
            get
            {
                return errormsgs.OutputErrors.Count > 0;
            }
        }

        public List<string> ErrorKeys
        {
            get
            {
                return errormsgs.OutputErrors.Keys.ToList();
            }

        }

        public string[] ErrorValues
        {
            get
            {
                return errormsgs.OutputErrors.Values.ToArray();
            }
        }
        public Dictionary<string, string> ErrorkeyValues
        {
            get
            {
                return errormsgs.OutputErrors;
            }
        }


        }
    }