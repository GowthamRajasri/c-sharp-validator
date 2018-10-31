                                                                  VALIDATIONS
Requirement:
     .Net framework 4.
Installation:
    Add Reference.
Documentation:
   Inputs:
       Input Messages:
             Input messages are dictionary format. Here keys are string and        corresponding values are object.
      Input Rules:
             Input rules are also dictionary format. Here keys and values both are string.
    Custom Messages:
           This is in dictionary format. Here keys are input keys and values are having
Custom error messages for corresponding keys.
List of Validations:
      Required:
           Here this method checks if input has value return true or else return false.
      Regular Expressions:
             Here this method check whether input is match with given pattern or not.
If it is match return true or else return false.
Outputs:
This is in dictionary format. Keys and values both are string. Values are having error messages for corresponding keys.
For Example:
Input messages are given like IDictionary<string, object> InputMessage        = new Dictionary<string, object>();
    
Input rules are given like  IDictionary<string, string> InputRule = new Dictionary<string, string>();
   
Custom messages are given like IDictionary<string, string> CustomMessages = new Dictionary<string, string>

These inputs are passing as parameter like   var resultObj = RSValidator.RSValidate(InputMessage, InputRule, CustomMessages);
 
And get input keys and corresponding  values and also get validation rules using split function(string[] lettersnums = Ruleslist[j].Split(':');)
Required Validation:
        
       return !string.IsNullOrEmpty(inputMsgValue);

  

