using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Sockets;
using Microsoft.VisualBasic;
using RsValidations;
using Errormessages;
using Validatoins;



namespace AllValidations
{

    public interface IValidator
    {
        IDictionary<string, List<string>> Validate(IDictionary<string, object> validateValue, IDictionary<string, string> validationRule);
    }

    public class BaseIValidator
    {
        
        public void RSValidate(IValidator RSvalidators)
        {


            try
            {
                IDictionary<string, object> InputMessage = new Dictionary<string, object>();
                InputMessage["firstname"] = "";

                IDictionary<string, string> InputRule = new Dictionary<string, string>();
                InputRule["firstname"] = "required|regex:[^a-zA-Z]+$";

                IDictionary<string, string> CustomMessages = new Dictionary<string, string>{
                { "firstname.required" , "Please Enter name."}, };





                var resultObj = RSValidator.RSValidate(InputMessage, InputRule, CustomMessages);
                Console.WriteLine(resultObj.);
                foreach (KeyValuePair<string, string> item in resultObj.errormsgs.OutputErrors)
                {
                   Console.WriteLine(" {0}=>:{1}", item.Key, item.Value);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            Console.ReadLine();
        }
    }
    public class MainClass
    {
        public static void Main(string[] args)
        {
            BaseIValidator baseValidaor = new BaseIValidator();
            baseValidaor.RSValidate(null);


        }
    }

}