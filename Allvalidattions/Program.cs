using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net.Sockets;
using Microsoft.VisualBasic;
namespace Validations
{

    public interface IValidator
    {
        IDictionary<string, List<string>> Validate(IDictionary<string, object> validateValue, IDictionary<string, string> validationRule, IDictionary<string, string> customizedmessages);
    }

    public class BaseIValidator
    {
        public void Validate(IValidator validator)
        {
            IDictionary<string, List<string>> outputerrors = new Dictionary<string, List<string>> { };

            IDictionary<string, object> inputmsg = new Dictionary<string, object>();
            inputmsg["firstname"] = "";
            inputmsg["Lastname"] = "Thangamani";
            inputmsg["email"] = "gowtham@rajasr.com";
            inputmsg["mn"] = "9488220623";
            inputmsg["DOB"] = "1991.10.13";
            inputmsg["url"] = "https://www.google.com/";
            inputmsg["ip"]="192.168.1.0";
            inputmsg["digits"] = "1";
            inputmsg["number"] = "1.566";
            inputmsg["string"] = "true";
            inputmsg["schedulearray"] = new string[] {"int","float"};

            
            
            
            
           

            IDictionary<string, string> inputrule = new Dictionary<string, string>();
            inputrule["firstname"] = "max:10|required";
            inputrule["Lastname"] = "required|max:10|stringcheck|size:35|regex:[^a-zA-Z]+$";
            inputrule["email"] = "required|email";
            inputrule["DOB"] = "required|date|dateformat:yyyy-MM-dd|after:1993.10.13|afterorequal:1991.10.14|before:1991.10.12|beforeorequal:1991.10.12|between:1991.10.14-1992.10.13|dateequals:1991.10.14";
            inputrule["mn"] = "mobilenumber|required|max:10|digits";
            inputrule["url"] = "required|url";
            inputrule["ip"]="required|ip";
            inputrule["digits"] = "required|digits|digitbetween:0-1|lessthan:100|greaterthan:150|greaterthanorequal:190";
            inputrule["number"] = "numeric|greaterthan:10|lessthanorequal:0|min:10";
            inputrule["string"] = "boolean|stringcheck|size:65";
            inputrule["schedulearray"] = "array|arraylength:5|arraytype:string";


            IDictionary<string, string> customizedmessages = new Dictionary<string, string>{
           { "firstname.required" , "Please Enter name."},
           {"firstname.max", "Name should not exist 10 character."},
           {"Lastname.max", "Name should not exist 10 character."},
           {"Lastname.required","Please Enter name."},
           //{"email.required","Plese Enter Name"},
           //{"DOB.date","Incorrect"}
           //{"digits.required","please Enter digit."}
            };


            outputerrors = validator.Validate(inputmsg,inputrule,customizedmessages);
            Console.WriteLine("Validation");

            foreach (KeyValuePair<string, List<string>> item in outputerrors)
            {
                Console.WriteLine(" {0}=>:{1}", item.Key, string.Join(",", item.Value));
            }
             Console.ReadLine();


        }
    }
    public class Validator : IValidator
    {


        public enum commonerrormesgs
        {
            required, max, email, mobilenumber, date, url, ip, digits, dateformat, after,
            afterorequal, before, beforeorequal, between, numeric, digitbetween, dateequals, greaterthan, lessthan,
            lessthanorequal, greaterthanorequal, min, boolean, stringcheck, size, regex, array, arraylength,arraytype
        };






        Dictionary<commonerrormesgs, string> errormessages = new Dictionary<commonerrormesgs, string>  {

          {commonerrormesgs.required,"The {{fieldname}} must be required "},

          {commonerrormesgs.max,"The {{fieldname}}must be given length"},

          {commonerrormesgs.email,"The {{fieldname}} is incorrect"},

          {commonerrormesgs.mobilenumber,"The {{fieldname}} is incorrect"},

          {commonerrormesgs.date,"The {{fieldname}} is incorrect"},

          {commonerrormesgs.url,"The {{fieldname}} is incorrect" } ,

          {commonerrormesgs.ip,"The {{fieldname}} is incorrect" },

          {commonerrormesgs.digits,"The {{fieldname}} is not digit"},

          {commonerrormesgs.dateformat,"The {{fieldname}} is Incorrect format"},

          {commonerrormesgs.after,"The {{fieldname}} is not after the date"},

          {commonerrormesgs.afterorequal,"The {{fieldname}} is not afterorequal the date"},

          {commonerrormesgs.before,"The {{fieldname}} is not before the date"},

          {commonerrormesgs.beforeorequal,"The {{fieldname}} is not beforeorequal the date"},

          {commonerrormesgs.between,"The {{fieldname}} is not between the date"},

          {commonerrormesgs.numeric,"The {{fieldname}} is not a numeric"},

          {commonerrormesgs.digitbetween,"The {{fieldname}} is not betweendigit"},

          {commonerrormesgs.dateequals,"The {{fieldname}} is not dateequal"},

          {commonerrormesgs.greaterthan,"The {{fieldname}} is not greaterthan"},

          {commonerrormesgs.lessthan,"The {{fieldname}} is not lessthan"},

          {commonerrormesgs.lessthanorequal,"The {{fieldname}} is not lessthanorequal"},

          {commonerrormesgs.greaterthanorequal,"The {{fieldname}} is not greaterthanorequal"},

          {commonerrormesgs.min,"The {{fieldname}} is not min"},

          {commonerrormesgs.boolean,"The {{fieldname}} is not boolean"},
          
          {commonerrormesgs.stringcheck,"The {{fieldname}} is not string"},
          
          {commonerrormesgs.size,"The {{fieldname}} is not given size"},
          
          {commonerrormesgs.regex,"The {{fieldname}} is not given pattern"},

          {commonerrormesgs.array,"The {{fieldname}} is not an array"},

          {commonerrormesgs.arraylength,"The {{fieldname}} is not given length"},
          
          {commonerrormesgs.arraytype,"The {{fieldname}} is not given type"}


             

        };



        Dictionary<string, List<string>> outputerrors = new Dictionary<string, List<string>> { };
        List<string> oldValue = new List<string>();
        string msgRule;
        string msgvalue;
        string dot = ".";

        IDictionary<string, string> customizedmessages;

        public IDictionary<string, List<string>> Validate(IDictionary<string, object> validateValue, IDictionary<string, string> validationRule, IDictionary<string, string> customizedmessages)
        {
            this.customizedmessages = customizedmessages;

            string[] validatevalueKey = validateValue.Keys.ToArray();
            var validatevalueval = validateValue.Values.ToArray();


            for (int i = 0; i <= validatevalueKey.Length - 1; i++)
            {

                if (validationRule.ContainsKey(validatevalueKey[i]))
                {

                    var value = validatevalueKey[i];
                    string Rules;
                    validationRule.TryGetValue(validatevalueKey[i], out Rules);

                    string ruleslwr = Rules.ToLower();
                    string[] Ruleslist = ruleslwr.Split('|'); //split the given string


                    int ruleslength = Ruleslist.Length;  //array length


                    for (int j = 0; j < ruleslength; j++)
                    {


                        string[] lettersnums = Ruleslist[j].Split(':');
                        commonerrormesgs errorkey = (commonerrormesgs)Enum.Parse(typeof(commonerrormesgs), lettersnums[0]);

                        switch (errorkey)
                        {
                            case commonerrormesgs.required:

                                Required(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.max:

                                Max(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.email:

                                Email(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.mobilenumber:

                                mobilenumber(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.date:

                                Date(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.url:

                                URL(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.ip:
                                IPadd(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.digits:

                                Digits(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.dateformat:

                                Dateformat(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.after:
                                After(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDateTime(lettersnums[1]));
                                break;
                            case commonerrormesgs.afterorequal:

                                Afterorequal(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDateTime(lettersnums[1]));
                                break;

                            case commonerrormesgs.before:

                                Before(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDateTime(lettersnums[1]));
                                break;

                            case commonerrormesgs.beforeorequal:

                                Beforeorequal(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDateTime(lettersnums[1]));
                                break;

                            case commonerrormesgs.between:

                                Between(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.numeric:

                                Numeric(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.digitbetween:

                                Digitbetween(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.dateequals:

                                Dateequals(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDateTime(lettersnums[1]));
                                break;

                            case commonerrormesgs.greaterthan:

                                Greaterthan(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.lessthan:

                                Lessthan(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.lessthanorequal:

                                Lessthanorequal(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.greaterthanorequal:

                                Greaterthanorequal(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.min:

                                Min(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDecimal(lettersnums[1]));
                                break;

                            case commonerrormesgs.boolean:

                                Boolean(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.stringcheck:

                                String(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.size:

                                Size(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDecimal(lettersnums[1]));
                                break;

                            case commonerrormesgs.regex:


                                Regex(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.array:

                                Array(validatevalueval[i], validatevalueKey[i]);
                                break;


                            case commonerrormesgs.arraylength:

                                Arraylength(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDecimal(lettersnums[1]));
                                break;

                            case commonerrormesgs.arraytype:

                                Arraytype(validatevalueval[i], validatevalueKey[i], lettersnums[1]);
                                break;







                        }
                    }
                }

            }
            return outputerrors;
        }





        public void Required(string inputmsgvalue, string inputmsgkey)
        {
            string requirederrormgs;
            errormessages.TryGetValue(commonerrormesgs.required, out requirederrormgs);

            if (string.IsNullOrEmpty(inputmsgvalue))
            {

                msgRule = string.Concat(inputmsgkey, dot, "required");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);

                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }


                else if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                {


                    oldValue.Add(requirederrormgs.Replace("{{fieldname}}", inputmsgkey));

                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { requirederrormgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void Max(string inputmsgvalue, string inputmsgkey, string inputlength)
        {
            string maxerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.max, out maxerrormsgs);

            msgRule = string.Concat(inputmsgkey, dot, "max");

            int valuelength = inputmsgvalue.Length;
            decimal num = Convert.ToDecimal(inputlength);

            if (valuelength < num)
            {
                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);

                    if (outputerrors.ContainsKey(inputmsgkey))
                    {

                        List<string> oldValue = new List<string>();

                        if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                        {

                            oldValue.Add(msgvalue);
                        }
                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }


                else
                {

                    if (outputerrors.ContainsKey(inputmsgkey))
                    {

                        List<string> oldValue = new List<string>();

                        if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                        {


                            oldValue.Add(maxerrormsgs.Replace("{{fieldname}}", inputmsgkey));


                        }
                    }
                    else
                    {

                        outputerrors.Add(inputmsgkey, new List<string> { maxerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                    }
                }
            }


        }



        public void Email(string inputmsgvalue, string inputmsgkey)
        {
            string emailerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.email, out emailerrormsgs);

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(inputmsgvalue);

            if (match.Success)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "email");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }
                else if (outputerrors.ContainsKey(inputmsgkey))
                {


                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(emailerrormsgs.Replace("{{fieldname}}", inputmsgkey));

                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { emailerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void mobilenumber(string inputmsgvalue, string inputmsgkey)
        {
            string mnerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.mobilenumber, out mnerrormsgs);

            var expr = new Regex(@"^((\+){0,1}91(\s){0,1}(\-){0,1}(\s){0,1}){0,1}9[0-9](\s){0,1}(\-){0,1}(\s){0,1}[1-9]{1}[0-9]{7}$");

            if (expr.IsMatch(inputmsgvalue))
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "numeric");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }
                else if (outputerrors.ContainsKey(inputmsgkey))
                {

                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(mnerrormsgs.Replace("{{fieldname}}", inputmsgkey));

                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { mnerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void Date(string inputmsgvalue, string inputmsgkey)
        {
            string dateerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.date, out dateerrormsgs);

            var format = new[] { "yyyy.MM.dd", "yyyy-MM-dd", "yyyy/MM/dd" };
            DateTime dt;

            if (DateTime.TryParseExact((String)inputmsgvalue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            {

            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "date");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }
                else if (outputerrors.ContainsKey(inputmsgkey))
                {

                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {

                        oldValue.Add(dateerrormsgs.Replace("{{fieldname}}", inputmsgkey));

                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { dateerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void URL(string inputmsgvalue, string inputmsgkey)
        {
            string urlerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.url, out urlerrormsgs);

            string pattern = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (reg.IsMatch(inputmsgvalue))
            {
            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "url");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgvalue, out oldValue))
                    {
                        oldValue.Add(urlerrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { urlerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }




        public void IPadd(string inputmsgvalue, string inputmsgkey)
        {
            string ipaddresserrormsgs;
            errormessages.TryGetValue(commonerrormesgs.ip, out ipaddresserrormsgs);

            System.Net.IPAddress ipAddress = null;
            if (System.Net.IPAddress.TryParse(inputmsgvalue, out ipAddress))
            {

            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "ip");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgvalue, out oldValue))
                    {
                        oldValue.Add(ipaddresserrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { ipaddresserrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }




        public void Digits(string inputmsgvalue, string inputmsgkey)
        {
            string digitserrormsgs;
            errormessages.TryGetValue(commonerrormesgs.digits, out digitserrormsgs);

            if (inputmsgvalue.All(char.IsDigit))
            {
            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "digits");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(digitserrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { digitserrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void Dateformat(string inputmsgvalue, string inputmsgkey, string format)
        {
            string dateformaterrormsgs;
            errormessages.TryGetValue(commonerrormesgs.dateformat, out dateformaterrormsgs);

            DateTime dDate;
            if (DateTime.TryParseExact(inputmsgvalue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dDate))
            {
                string.Format(format, dDate);
            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "dateformat");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(dateformaterrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { dateformaterrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }
        }



        public void After(string inputmsgvalue, string inputmsgkey, DateTime date)
        {
            string afterdate;
            errormessages.TryGetValue(commonerrormesgs.after, out afterdate);

            DateTime Temp;
            if (DateTime.TryParse(inputmsgvalue, out Temp) == true && Temp.Date > date)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "after");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(afterdate.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { afterdate.Replace("{{fieldname}}", inputmsgkey) });
                }
            }


        }



        public void Afterorequal(string inputmsgvalue, string inputmsgkey, DateTime date)
        {
            string afterorequal;
            errormessages.TryGetValue(commonerrormesgs.afterorequal, out afterorequal);

            DateTime Temp;
            if (DateTime.TryParse(inputmsgvalue, out Temp) == true && Temp.Date >= date)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "afterorequal");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(afterorequal.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { afterorequal.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void Before(string inputmsgvalue, string inputmsgkey, DateTime date)
        {
            string before;
            errormessages.TryGetValue(commonerrormesgs.before, out before);

            DateTime Temp;
            if (DateTime.TryParse(inputmsgvalue, out Temp) == true && Temp.Date < date)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "before");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(before.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { before.Replace("{{fieldname}}", inputmsgkey) });
                }
            }
        }



        public void Beforeorequal(string inputmsgvalue, string inputmsgkey, DateTime date)
        {
            string beforeorequal;
            errormessages.TryGetValue(commonerrormesgs.beforeorequal, out beforeorequal);

            DateTime Temp;

            if (DateTime.TryParse(inputmsgvalue, out Temp) == true && Temp.Date <= date)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "beforeorequal");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(beforeorequal.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { beforeorequal.Replace("{{fieldname}}", inputmsgkey) });
                }
            }
        }




        public void Between(string inputmsgvalue, string inputmsgkey, string date)
        {
            string[] dates = date.Split('-');
            DateTime startdate = Convert.ToDateTime(dates[0]);
            DateTime enddate = Convert.ToDateTime(dates[1]);

            string between;
            errormessages.TryGetValue(commonerrormesgs.between, out between);

            DateTime Temp;
            if (DateTime.TryParse(inputmsgvalue, out Temp) == true && Temp.Date >= startdate && Temp.Date <= enddate)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "beforeorequal");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(between.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { between.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void Numeric(string inputmsgvalue, string inputmsgkey)
        {
            string numericerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.numeric, out numericerrormsgs);
            double myNum = 0;

            if (Double.TryParse(inputmsgvalue, out myNum))
            {
            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "beforeorequal");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(numericerrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { numericerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }


        }




        public void Digitbetween(string inputmsgvalue, string inputmsgkey, string digits)
        {
            string Digitbetweenerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.digitbetween, out Digitbetweenerrormsgs);

            string[] Digits = digits.Split('-');
            char startdigit = Convert.ToChar(Digits[0]);
            char enddigit = Convert.ToChar(Digits[1]);

            foreach (char c in inputmsgvalue)
            {

                if (c >= startdigit && c <= enddigit)
                {

                    continue;
                }

                else
                {
                    msgRule = string.Concat(inputmsgkey, dot, "digitbetween");

                    if (customizedmessages.ContainsKey(msgRule))
                    {
                        customizedmessages.TryGetValue(msgRule, out msgvalue);
                        if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                        {
                            oldValue.Add(msgvalue);

                        }
                        else
                        {
                            outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                        }
                    }

                    else if (outputerrors.ContainsKey(inputmsgkey))
                    {
                        if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                        {
                            oldValue.Add(Digitbetweenerrormsgs.Replace("{{fieldname}}", inputmsgkey));
                        }
                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { Digitbetweenerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                    }
                }
            }
        }




        public void Dateequals(string inputmsgvalue, string inputmsgkey, DateTime date)
        {
            string Dateequals;
            errormessages.TryGetValue(commonerrormesgs.dateequals, out Dateequals);

            DateTime Temp;

            if (DateTime.TryParse(inputmsgvalue, out Temp) == true && Temp.Date == date)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "beforeorequal");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Dateequals.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Dateequals.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void Lessthan(string inputmsgvalue, string inputmsgkey, string number)
        {
            string Lessthan;
            errormessages.TryGetValue(commonerrormesgs.lessthan, out Lessthan);
            decimal temp = decimal.Parse(inputmsgvalue);
            decimal num = decimal.Parse(number);
            if (temp < num)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "lessthan");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Lessthan.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Lessthan.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void Greaterthan(string inputmsgvalue, string inputmsgkey, string number)
        {
            string Greaterthan;
            errormessages.TryGetValue(commonerrormesgs.greaterthan, out Greaterthan);
            decimal temp = decimal.Parse(inputmsgvalue);
            decimal num = decimal.Parse(number);
            if (temp > num)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "greaterthan");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Greaterthan.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Greaterthan.Replace("{{fieldname}}", inputmsgkey) });
                }
            }
        }




        public void Lessthanorequal(string inputmsgvalue, string inputmsgkey, string number)
        {
            string Lessthanorequal;
            errormessages.TryGetValue(commonerrormesgs.lessthanorequal, out Lessthanorequal);
            decimal temp = decimal.Parse(inputmsgvalue);
            decimal num = decimal.Parse(number);
            if (temp <= num)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "lessthanorequal");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Lessthanorequal.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Lessthanorequal.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void Greaterthanorequal(string inputmsgvalue, string inputmsgkey, string number)
        {
            string Greaterthanorequal;
            errormessages.TryGetValue(commonerrormesgs.greaterthanorequal, out Greaterthanorequal);
            decimal temp = decimal.Parse(inputmsgvalue);
            decimal num = decimal.Parse(number);
            if (temp >= num)
            { }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "greaterthanorequal");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Greaterthanorequal.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Greaterthanorequal.Replace("{{fieldname}}", inputmsgkey) });
                }
            }
        }



        public void Min(string inputmsgvalue, string inputmsgkey, decimal min)
        {

            string Minerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.min, out Minerrormsgs);
            int valuelength = inputmsgvalue.Length;
            decimal num = Convert.ToDecimal(valuelength);
            decimal inputvalue = decimal.Parse(inputmsgvalue);
            if (num >= min)
            {
            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "min");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Minerrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Minerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }
        }



        public void Boolean(string inputmsgvalue, string inputmsgkey)
        {

            string Booleanerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.boolean, out Booleanerrormsgs);
            bool value;


            if (bool.TryParse(inputmsgvalue, out value) || inputmsgvalue == "0" || inputmsgvalue == "1")
            {

            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "boolean");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Booleanerrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Booleanerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }


        }



        public void String(string inputmsgvalue, string inputmsgkey)
        {
            string stringerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.stringcheck, out stringerrormsgs);


            Regex r = new Regex("^[a-zA-Z]*$");
            if (!string.IsNullOrEmpty(inputmsgvalue) && r.IsMatch(inputmsgvalue))
            {
            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "string");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(stringerrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { stringerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }

        }



        public void Size(string inputmsgvalue, string inputmsgkey, decimal number)
        {
            string Sizeerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.size, out Sizeerrormsgs);

            if (!string.IsNullOrEmpty(inputmsgvalue) && inputmsgvalue.Length > number)
            {
            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "size");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Sizeerrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Sizeerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }



        }



        public void Regex(string inputmsgvalue, string inputmsgkey, string pattern)
        {
            string regexerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.regex, out regexerrormsgs);

            Regex regex = new Regex(pattern);
            if (regex.IsMatch(inputmsgvalue))
            {
            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "regex");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {


                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(regexerrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { regexerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }



        }



        public void Array(object inputmsgvalue, string inputmsgkey)
        {
            string Arrayerrormsgs;
            errormessages.TryGetValue(commonerrormesgs.array, out Arrayerrormsgs);
            Type type = inputmsgvalue.GetType();

            if (type.IsArray)
            {
            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "array");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {

                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Arrayerrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Arrayerrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }




        }



        public void Arraylength(string inputmsgvalue, string inputmsgkey, decimal number)
        {

            string Arraylenghtherrormsgs;
            errormessages.TryGetValue(commonerrormesgs.arraylength, out Arraylenghtherrormsgs);

            if (inputmsgvalue.Length <= number)
            {

            }
            else
            {
                msgRule = string.Concat(inputmsgkey, dot, "arraylength");

                if (customizedmessages.ContainsKey(msgRule))
                {
                    customizedmessages.TryGetValue(msgRule, out msgvalue);
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {

                        oldValue.Add(msgvalue);

                    }
                    else
                    {
                        outputerrors.Add(inputmsgkey, new List<string> { msgvalue });
                    }
                }

                else if (outputerrors.ContainsKey(inputmsgkey))
                {
                    if (outputerrors.TryGetValue(inputmsgkey, out oldValue))
                    {
                        oldValue.Add(Arraylenghtherrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Arraylenghtherrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }
        }



        public void Arraytype(object inputmsgvalue, string inputmsgkey, string arraytype)
        {
         
            string str = inputmsgvalue as string;
            
            object objstr = (object)arraytype;

            Type valueType = objstr.GetType();
            

            var expectedType = typeof(valueType);

            if (inputmsgvalue is expectedType)


                if (inputmsgvalue.GetType().IsArrayOf(expectedType)) 
            {
            }
        }








    }





   

   public  class MainClass
    {
        public static void Main(string[] args)
        {
            BaseIValidator baseValidaor = new BaseIValidator();
            baseValidaor.Validate(new Validator());


        }
    }
}


