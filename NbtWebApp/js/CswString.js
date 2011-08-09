/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="_Global.js" />
/// <reference path="_CswPrototypeExtensions.js" />

//#region CswString
CswString.inheritsFrom(String);

function CswString(string)
{
    String.call(this);
    
    var value = string;
    this.val = function (string)
    {
        if (arguments.length === 1)
        {
            value = string;
            return this; //for chaining
        }
        else
        {
            return value;
        }
    };
    this.contains = function (findString) { return value.indexOf(findString) !== -1; };
}

CswString.prototype.toString = function () { return this.value; };

//#endregion CswString