/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />

//#region CswString
var CswString = function(string) {
    "use strict";
    var value = string;
    return {
        val: function (newString) {
            if (arguments.length === 1) {
                value = newString;
                return this; //for chaining
            } else {
                return value;
            }
        },
        contains: function (findString) { return value.indexOf(findString) !== -1; },
        toString: function () { return value; }
    };
}

//#endregion CswString