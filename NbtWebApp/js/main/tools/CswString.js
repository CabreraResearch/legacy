/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />

//#region CswString
CswString.inheritsFrom(String);

function CswString(string)
{
    String.call(this);
    
    var value = string;
    this.val = function (newString)
    {
        if (arguments.length === 1) {
            value = newString;
            return this; //for chaining
        } else {
            return value;
        }
    };
    this.contains = function (findString) { return contains(value,findString); };
    this.toString = function () { return value; };
}

//#endregion CswString