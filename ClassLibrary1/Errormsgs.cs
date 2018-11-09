﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Errormessages
{
    public enum commonerrormesgs
    { required, regex };

    public class CommonErrors
    {
       



        public Dictionary<string, string> OutputErrors = new Dictionary<string, string> { };

        Dictionary<commonerrormesgs, string> ErrorMessages = new Dictionary<commonerrormesgs, string>  {
         {commonerrormesgs.required,"The {{fieldname}} must be required "},
         {commonerrormesgs.regex,"The {{fieldname}} is not given pattern"},};

        public void GetErrorMessages(string inputMsgKey, commonerrormesgs errorKey, IDictionary<string, string> customizedMessages)
        {
              string msgRule;
             string msgValue;
             string dot = ".";
            string errorMsg;
            ErrorMessages.TryGetValue(errorKey, out errorMsg);

            msgRule = string.Concat(inputMsgKey, dot, errorKey);

            if (customizedMessages.ContainsKey(msgRule))
            {
                customizedMessages.TryGetValue(msgRule, out msgValue);

                OutputErrors.Add(inputMsgKey, msgValue);
            }

            else
            {
                OutputErrors.Add(inputMsgKey, errorMsg.Replace("{{fieldname}}", inputMsgKey));
            }

        }


    }
}
