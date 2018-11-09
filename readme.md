# VALIDATION

## Requirement:
     .Net framework 4.
## Installation:
    Add Reference.
## Documentation:
  #### Inputs:
       Input Messages:
             Input messages are dictionary format. Here keys are string and corresponding values are object.
       Input Rules:
             Input rules are also dictionary format. Here keys and values both are string.
       Custom Messages:
           This is in dictionary format. Here keys are input keys and values are having
Custom error messages for corresponding keys.
#### List of Validations:
       Required:
           Here this method checks if input has value return true or else return false.
       Regular Expressions:
             Here this method check whether input is match with given pattern or not.
If it is match return true or else return false.
## Outputs:
This is in dictionary format. Keys and values both are string. Values are having error messages for corresponding keys.
## For Example:
```c#
 //Input messages are given like,
Dictionary<string, object> InputMessage = new Dictionary<string, object>().
    
//Input rules are given like,
Dictionary<string, string> InputRule = new Dictionary<string, string>().

//Custom messages are given like,
Dictionary<string, string> CustomMessages = new Dictionary<string, string>().

//These inputs are passing as parameter like,
var resultObj = RSValidator.RSValidate(InputMessage,InputRule,CustomMessages).

//Get input keys and corresponding  values and validation rules using methods and do the validations.

//Error output is dictionary format
Dictionary<string, string>
//Here key is inputmessage key and value is coresponding error message.

//If output has error return true or else return false.
public bool HasError

//Get errorkeys only
public List<string> ErrorKeys
        
//Get errorvalues only
public string[] ErrorValues

//Get errorkeyvalues 
public Dictionary<string, string> ErrorkeyValues

```