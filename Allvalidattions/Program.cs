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
        IDictionary<string, List<string>> Validate(IDictionary<string, object> validateValue, IDictionary<string, string> validationRule);
    }

    public class BaseIValidator
    {
        public void RSValidate(IValidator RSvalidators)
        {

            IDictionary<string, List<string>> outputerrors = new Dictionary<string, List<string>> { };

            IDictionary<string, object> inputmsg = new Dictionary<string, object>();
            inputmsg["firstname"] = "rr";
            inputmsg["Lastname"] = "Thangamani";
            inputmsg["email"] = "gowthamrajasr.com";
            inputmsg["mn"] = "9488226229";
            inputmsg["DOB"] = "1991.10.14";
            inputmsg["url"] = "https://www.google.com/";
            inputmsg["ip"]="192.168.1.0";
            inputmsg["digits"] = "23";
            inputmsg["number"] = "566";
            inputmsg["string"] = "true";
            inputmsg["schedulearray"] = new string[] {"int","float"};

                                          

            IDictionary<string, string> inputrule = new Dictionary<string, string>();
            inputrule["firstname"] = "required|max:10";
            inputrule["Lastname"] = "required|max:10|stringcheck|size:35|regex:[^a-zA-Z]+$";
            inputrule["email"] = "required|email";
            inputrule["DOB"] = "required|date|dateformat:yyyy.MM.dd|after:1993.10.12|afterorequal:1991.10.13|before:1991.10.18|beforeorequal:1991.10.13|between:1991.10.14-1992.10.13|dateequals:1991.10.14";
            inputrule["mn"] = "number|max:10|digits";
            inputrule["url"] = "required|url";
            inputrule["ip"]="required|ip";
            inputrule["digits"] = "required|digits|digitbetween:0-1|lessthan:100|greaterthan:150|greaterthanorequal:190";
            inputrule["number"] = "numeric|greaterthan:10|lessthanorequal:0|min:10|integer";
            inputrule["string"] = "boolean|stringcheck|size:65";
            inputrule["schedulearray"] = "array|arraylength:5|arraytype:int[]";



            IDictionary<string, string> customizedmessages = new Dictionary<string, string>{
             { "firstname.required" , "Please Enter name."},
             {"firstname.max", "Name should not exist 10 character."},
             {"Lastname.max", "Name should not exist 10 character."},
             {"Lastname.required","Please Enter name."},
            // {"email.email","incorrect"}
            };
          


            
           var resobj = RSValidator.RSValidate(inputmsg,inputrule,customizedmessages);

           Console.WriteLine(resobj);


          foreach (KeyValuePair<string, List<string>> item in resobj.ErrorkeyValues)
            {
               Console.WriteLine(" {0}=>:{1}", item.Key, string.Join(",", item.Value));
            }
            Console.ReadLine();


        }
    }
    public class RSValidator
    {
      
        bool result = new bool();
        public List<string> values;
        public string KEY = "firstname";
        private Dictionary<string, List<string>> outputerrors = new Dictionary<string, List<string>> { };
        private List<string> oldValue = new List<string>();
        private string msgRule;
        private string msgvalue;
        private string dot = ".";
         
        public enum commonerrormesgs
        {
            required, max, email, number, date, url, ip, digits, dateformat, after,
            afterorequal, before, beforeorequal, between, numeric, digitbetween, dateequals, greaterthan, lessthan,
            lessthanorequal, greaterthanorequal, min, boolean, stringcheck, size, regex, array, arraylength,arraytype,integer
        };


        Dictionary<commonerrormesgs, string> errormessages = new Dictionary<commonerrormesgs, string>  {

          {commonerrormesgs.required,"The {{fieldname}} must be required "},

          {commonerrormesgs.max,"The {{fieldname}}must be given length"},

          {commonerrormesgs.email,"The {{fieldname}} is incorrect"},

          {commonerrormesgs.number,"The {{fieldname}} is incorrect"},

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
          
          {commonerrormesgs.arraytype,"The {{fieldname}} is not given type"},

         {commonerrormesgs.integer,"The {{fieldname}} is not an integer"}


        };







        IDictionary<string, string> customizedmessages;

        public static RSValidator RSValidate(IDictionary<string, object> validateValue, IDictionary<string, string> validationRule, IDictionary<string, string> customizedmessages)
        {
            
            RSValidator obj = new RSValidator();
            
            obj.customizedmessages = customizedmessages;

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

                        commonerrormesgs ErrorKey = (commonerrormesgs)Enum.Parse(typeof(commonerrormesgs), lettersnums[0]);

                        switch (ErrorKey)
                        {
                            case commonerrormesgs.required:

                                if (!obj.HasValue(Convert.ToString(validatevalueval[i])))
                               
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                
                                else break; 
                                break;                         
                             

                            case commonerrormesgs.max:

                                if (obj.Max(Convert.ToString(validatevalueval[i]), lettersnums[1]))
                                

                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                
                                else break; 

                                 break;

                            case commonerrormesgs.email:

                                 if (obj.Email(Convert.ToString(validatevalueval[i])))
                                     obj.CustomMessages(validatevalueKey[i], ErrorKey);

                                 else break;

                                break;

                            case commonerrormesgs.number:
                                if (obj.IsNumber(Convert.ToString(validatevalueval[i])))
                                  obj.CustomMessages(validatevalueKey[i], ErrorKey);

                              else break;

                              break;
                            case commonerrormesgs.date:

                              if (obj.IsDate(Convert.ToString(validatevalueval[i])))
                                  obj.CustomMessages(validatevalueKey[i], ErrorKey);

                              else break;
                               break;

                            case commonerrormesgs.url:

                               if( obj.URL(Convert.ToString(validatevalueval[i])))
                                  obj.CustomMessages(validatevalueKey[i], ErrorKey);
                               else break;
                                break;

                            case commonerrormesgs.ip:
                                if (obj.IPadd(Convert.ToString(validatevalueval[i])))
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                else break;
                                break;

                            case commonerrormesgs.digits:

                                if (obj.Digits(Convert.ToString(validatevalueval[i])))
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                else break;
                                break;

                            case commonerrormesgs.dateformat:

                                if (obj.Dateformat(Convert.ToString(validatevalueval[i]), lettersnums[1]) == true)
                                {
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                }
                                else break;
                                break;

                            case commonerrormesgs.after:
                                if (obj.After(Convert.ToString(validatevalueval[i]), Convert.ToDateTime(lettersnums[1]))==true)
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);

                                else break;
                                break;
                            case commonerrormesgs.afterorequal:

                                if (obj.AfterOrEqual(Convert.ToString(validatevalueval[i]), Convert.ToDateTime(lettersnums[1])))
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);

                                else break;

                                break;

                            case commonerrormesgs.before:

                                if (obj.Before(Convert.ToString(validatevalueval[i]), Convert.ToDateTime(lettersnums[1])))
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                else break;
                                    
                                break;

                            case commonerrormesgs.beforeorequal:

                                if (obj.Beforeorequal(Convert.ToString(validatevalueval[i]), Convert.ToDateTime(lettersnums[1])))
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                else break;
                                break;

                            case commonerrormesgs.between:

                                if (obj.Between(Convert.ToString(validatevalueval[i]), lettersnums[1]))
                                {
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                }
                                else break;
                                break;

                            case commonerrormesgs.numeric:

                                obj.Numeric(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.digitbetween:

                               if( obj.Digitbetween(Convert.ToString(validatevalueval[i]),lettersnums[1]))
                                   obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                break;

                            case commonerrormesgs.dateequals:

                                if (obj.Dateequals(Convert.ToString(validatevalueval[i]), Convert.ToDateTime(lettersnums[1])))
                                   
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                else break;

                                break;

                            case commonerrormesgs.greaterthan:

                               if( obj.Greaterthan(Convert.ToString(validatevalueval[i]), lettersnums[1]))
                                   obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                break;

                            case commonerrormesgs.lessthan:

                                if (obj.Lessthan(Convert.ToString(validatevalueval[i]), lettersnums[1]))
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                else break;
                                break;

                            case commonerrormesgs.lessthanorequal:

                                if (obj.Lessthanorequal(Convert.ToString(validatevalueval[i]), lettersnums[1]))
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                else break;
                                break;

                            case commonerrormesgs.greaterthanorequal:

                                if (obj.Greaterthanorequal(Convert.ToString(validatevalueval[i]), lettersnums[1]))
                                    obj.CustomMessages(validatevalueKey[i], ErrorKey);
                                else break;
                                break;

                            case commonerrormesgs.min:

                                obj.Min(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDecimal(lettersnums[1]));
                                break;

                            case commonerrormesgs.boolean:

                                obj.Boolean(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.stringcheck:

                                obj.String(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;

                            case commonerrormesgs.size:

                                obj.Size(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDecimal(lettersnums[1]));
                                break;

                            case commonerrormesgs.regex:


                                obj.Regex(Convert.ToString(validatevalueval[i]), validatevalueKey[i], lettersnums[1]);
                                break;

                            case commonerrormesgs.array:

                                obj.Array(validatevalueval[i], validatevalueKey[i]);
                                break;


                            case commonerrormesgs.arraylength:

                                obj.Arraylength(Convert.ToString(validatevalueval[i]), validatevalueKey[i], Convert.ToDecimal(lettersnums[1]));
                                break;

                             case commonerrormesgs.arraytype:

                                obj.Arraytype(validatevalueval[i], validatevalueKey[i], lettersnums[1]);
                                break;
                         
                             case commonerrormesgs.integer:

                                obj.Integer(Convert.ToString(validatevalueval[i]), validatevalueKey[i]);
                                break;
                        }
                    }
                }

            }
            return obj;
        }


         public List<string> this[string KEY] 
        {
            get{

              if(  outputerrors.ContainsKey(KEY))
              {
                  
                outputerrors.TryGetValue(KEY, out values);
                return values;
              }
                else
                {
                    return new List<string>();
                }
            }
            
        }


         public bool  HasError
        {
            get
            {
                return outputerrors.Count > 0 ;
            }
        }


         public List<string> ErrorKeys
       {
           get
           {
               return outputerrors.Keys.ToList();
           }

       }


         public List<string>[] ErrorValues
       {
           get
           {
               return outputerrors.Values.ToArray();
           }
       }


         public Dictionary<string ,List<string>> ErrorkeyValues
      {
          get
          {
              return outputerrors;
          }
      }





          bool HasRequired(string inputmsgValue)
         {
             return string.IsNullOrEmpty(inputmsgValue);
         }

        /// <summary>
        /// Has values return true or else return false 
        /// </summary>
        /// <param name="inputmsgValue"></param>
        /// <returns></returns>
          bool HasValue(string inputmsgValue)
          {
              return !string.IsNullOrEmpty(inputmsgValue);
          }
          void CustomMessages(string inputmsgKey, commonerrormesgs errorKey)
         {

             string errormsgs;
                 errormessages.TryGetValue(errorKey,out errormsgs);
              
             msgRule = string.Concat(inputmsgKey, dot, errorKey);

             if(customizedmessages.ContainsKey(msgRule))
             {
                 customizedmessages.TryGetValue(msgRule, out msgvalue);

                 if (outputerrors.TryGetValue(inputmsgKey, out oldValue))
                 {
                    oldValue.Add(msgvalue);
                 }
                 else
                 {
                     outputerrors.Add(inputmsgKey, new List<string> { msgvalue });
                 }

             }
             else if (outputerrors.ContainsKey(inputmsgKey))
             {
                
                 if (outputerrors.TryGetValue(inputmsgKey, out oldValue))
                 {
                     oldValue.Add(errormsgs.Replace("{{fieldname}}", inputmsgKey));
                 }
             }
                 else
                 {
                     outputerrors.Add(inputmsgKey, new List<string> { errormsgs.Replace("{{fieldname}}", inputmsgKey) });
                 }

             }

         
        /// <summary>
        /// if input not below the given number return true or else return false
        /// </summary>
        /// <param name="inputmsgValue"></param>
        /// <param name="inputLength"></param>
        /// <returns></returns>

          bool Max(string inputmsgValue,string inputLength)
         {
             
              if (!HasValue(inputmsgValue))
                return false;

               return !(inputmsgValue.Length >= Convert.ToDecimal(inputLength));

          
         }

        /// <summary>
        /// if email is incorrect return true or else return false
        /// </summary>
        /// <param name="inputmsgValue"></param>
        /// <returns></returns>
          bool Email(string inputmsgValue)
        {
            if (!HasValue(inputmsgValue))
                return false;

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
         
     
               Match match = regex.Match(inputmsgValue);
               return !match.Success;
              
        }

/// <summary>
/// Given number not satisfied given condition return true or else return false
/// </summary>
/// <param name="inputmsgValue"></param>
/// <returns></returns>
        bool IsNumber(string inputmsgValue)
        {           
            if (!HasValue(inputmsgValue))
                return false;

            Regex regex = new Regex(@"(?<!\d)\d{10}(?!\d)");
            return !regex.IsMatch(inputmsgValue);
            
        }


        /// <summary>
        ///If Given input is not a date format return true or else return false
        /// </summary>
        /// <param name="inputmsgValue"></param>
        /// <returns></returns>
        bool IsDate(string inputmsgValue)
        {
            if (!HasValue(inputmsgValue))
                return false;


             var format = new[] { "yyyy.MM.dd", "yyyy-MM-dd", "yyyy/MM/dd" };
             DateTime dt;
            return  !(DateTime.TryParseExact((String)inputmsgValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt));

               
        }



         bool URL(string inputmsgValue)
        {
            

            string pattern = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (HasRequired(inputmsgValue) == false)
            {
            result=reg.IsMatch(inputmsgValue);
            
            }
             return result;

        }




         bool IPadd(string inputmsgValue)
        {
            System.Net.IPAddress ipAddress = null;
            if (HasRequired(inputmsgValue) == false)
            {
                result = System.Net.IPAddress.TryParse(inputmsgValue, out ipAddress);
            }
            return result;
        }




         bool Digits(string inputmsgValue)
         {
             if (HasRequired(inputmsgValue))
             {
                 return true;
             }
             return !inputmsgValue.All(char.IsDigit);
         }



         bool Dateformat(string inputmsgValue, string format)
         {
             bool result = false;
             DateTime dDate;
             if (HasRequired(inputmsgValue) == false)
             {
                 if (IsDate(inputmsgValue) == false)
                 {

                     if (DateTime.TryParseExact(inputmsgValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dDate))
                     {

                         //return System.Convert.ToBoolean(string.Format(format, dDate));
                         

                     }

                 }
                 

             }
             return result;
         }

         bool After(string inputmsgValue, DateTime date)
         {
             bool result = false;
             DateTime Temp;
             if (HasRequired(inputmsgValue) == false)
             {
                 if (IsDate(inputmsgValue) == false)
                 {
                     if (!(DateTime.TryParse(inputmsgValue, out Temp) == true && Temp.Date > date == true))
                     {
                         result= true;
                     }
                     
                 }
                 
             }
            return result;
         }


         bool AfterOrEqual(string inputmsgValue,DateTime date)
        {

            bool result = false;
            DateTime Temp;
            if (HasRequired(inputmsgValue) == false)
            {
                if (IsDate(inputmsgValue) == false)
                {
                    if (!(DateTime.TryParse(inputmsgValue, out Temp) == true && Temp.Date >= date == true))
                    {
                        result = true;
                    }

                }

            }
            return result;
        }


        bool Before(string inputmsgValue, DateTime date)
        {
            bool result = false;
            DateTime Temp;
            if (HasRequired(inputmsgValue) == false)
            {
                if (IsDate(inputmsgValue) == false)
                {
                    if (!(DateTime.TryParse(inputmsgValue, out Temp) == true && Temp.Date < date == true))
                    {
                        result = true;
                    }

                }

            }
            return result;
        }




        private bool Beforeorequal(string inputmsgValue, DateTime date)
        {
            bool result = false;
            DateTime Temp;
            if (HasRequired(inputmsgValue) == false)
            {
                if (IsDate(inputmsgValue) == false)
                {
                    if (!(DateTime.TryParse(inputmsgValue, out Temp) == true && Temp.Date <= date == true))
                    {
                        result = true;
                    }

                }

            }
            return result;
        }




        private bool Between(string inputmsgValue,string date)
        {

            bool result = false;
            string[] dates = date.Split('-');
            DateTime startdate = Convert.ToDateTime(dates[0]);
            DateTime enddate = Convert.ToDateTime(dates[1]);


           
            DateTime Temp;
            if (HasRequired(inputmsgValue) == false)
            {
                if (IsDate(inputmsgValue) == false)
                {
                    if(! (DateTime.TryParse(inputmsgValue, out Temp) == true && Temp.Date >= startdate && Temp.Date <= enddate))
                    { 
                        result = true; 
                    }
                }
            }
            return result;
           
        }



        private void Numeric(string inputmsgvalue, string inputmsgkey)
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




        private bool Digitbetween(string inputmsgvalue, string digits)
        {
            bool result = false;

            string[] digit = digits.Split('-');
            char startdigit = Convert.ToChar(digit[0]);
            char enddigit = Convert.ToChar(digit[1]);
            if (HasRequired(inputmsgvalue) == false)
            {
                if (Digits(inputmsgvalue) == false)
                {
                    foreach (char c in inputmsgvalue)
                    {
                        if (c >= startdigit && c <= enddigit)
                        {
                            continue;
                        }

                        else
                        {
                            result = true;
                        }

                    }
                }
            }

            return result;
        }



        private bool Dateequals(string inputmsgValue, DateTime date)
        {
            bool result = false;
            DateTime Temp;
            if (HasRequired(inputmsgValue) == false)
            {
                if (IsDate(inputmsgValue) == false)
                {
                    if (!(DateTime.TryParse(inputmsgValue, out Temp) == true && Temp.Date == date))
                    { result = true; }

                }
            }
            return result;
        }

        private bool Lessthan(string inputmsgvalue,string number)
        {
            bool result = false;
            decimal temp = decimal.Parse(inputmsgvalue);
            decimal num = decimal.Parse(number);
            if (HasRequired(inputmsgvalue) == false)
            {
                if (Digits(inputmsgvalue) == false)
                {
                    if (temp < num)
                    { }
                    else
                    {

                        result = true;
                    }
                }
            }
            return result;
        }



        private bool Greaterthan(string inputmsgvalue,string number)
        {
            bool result = false;
            decimal temp = decimal.Parse(inputmsgvalue);
            decimal num = decimal.Parse(number);
            if (HasRequired(inputmsgvalue) == false)
            {
                
                    if (temp > num)
                    { }
                    else
                    {
                        result = true;
                    }
                
            }
            return result;      
        }




        private bool Lessthanorequal(string inputmsgvalue, string number)
        {
            bool result = false;
            decimal temp = decimal.Parse(inputmsgvalue);
            decimal num = decimal.Parse(number);
            if (HasRequired(inputmsgvalue) == false)
            {
                if (temp <= num)
                 { }
                  else
                   {
                        result = true;
                    }
                
            }
            return result;

        }



        private bool Greaterthanorequal(string inputmsgvalue, string number)
        {
            
            decimal temp = decimal.Parse(inputmsgvalue);
            decimal num = decimal.Parse(number);

            if (HasRequired(inputmsgvalue) == false)
            {
                if (temp >= num)
                    { }
                    else
                    {
                        result = true;
                    }
                
            }
            return result;
        }



        private bool Min(string inputmsgValue, string inputmsgkey, decimal min)
        {

           
            int valuelength = inputmsgValue.Length;
            decimal num = Convert.ToDecimal(valuelength);
            decimal inputvalue = decimal.Parse(inputmsgValue);
            if (HasRequired(inputmsgValue) == false)
            {

                if (num >= min)
                { }

                else { result = true; }
            }
            return true;
        }
            
       
        



        private void Boolean(string inputmsgvalue, string inputmsgkey)
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



        private void String(string inputmsgvalue, string inputmsgkey)
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



        private void Size(string inputmsgvalue, string inputmsgkey, decimal number)
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



        private void Regex(string inputmsgvalue, string inputmsgkey, string pattern)
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



        private void Array(object inputmsgvalue, string inputmsgkey)
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



        private void Arraylength(string inputmsgvalue, string inputmsgkey, decimal number)
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



        private void Arraytype(object  inputmsgvalue, string inputmsgkey, string type)
        {

            string Arraytyperrormsgs;
            errormessages.TryGetValue(commonerrormesgs.arraytype, out Arraytyperrormsgs);

            object aa = (object)type.ToArray();
        
            Type ExpectedType = aa.GetType();

            Type inputType = inputmsgvalue.GetType();
            if (inputType.Equals(aa))   

            if (inputType.GetType().IsAssignableFrom(ExpectedType))
            {
                      
            }
            else{
                msgRule = string.Concat(inputmsgkey, dot, "arraytype");

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
                        oldValue.Add(Arraytyperrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Arraytyperrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }
        }


        private void Integer(string inputmsgvalue, string inputmsgkey)
        {
            string Integerrrormsgs;
            errormessages.TryGetValue(commonerrormesgs.integer, out Integerrrormsgs);


            int value;
            if (int.TryParse(inputmsgvalue, out value))
            {
            }
            else{
                msgRule = string.Concat(inputmsgkey, dot, "integer");

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
                        oldValue.Add(Integerrrormsgs.Replace("{{fieldname}}", inputmsgkey));
                    }
                }
                else
                {
                    outputerrors.Add(inputmsgkey, new List<string> { Integerrrormsgs.Replace("{{fieldname}}", inputmsgkey) });
                }
            }







        }

       




    }


    class RSValaditionExcetpiotn : Exception
    {

    }




   

   public  class MainClass
    {
        public static void Main(string[] args)
        {
            BaseIValidator baseValidaor = new BaseIValidator();
            baseValidaor.RSValidate(null);


        }
    }
}


