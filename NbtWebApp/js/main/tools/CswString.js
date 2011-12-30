/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />

//#region CswString
CswString.inheritsFrom(String);

function CswString(string) {
    "use strict";
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
    this.contains = function (findString) { return value.indexOf(findString) !== -1; };
    this.toString = function () { return value; };
}

//#endregion CswString