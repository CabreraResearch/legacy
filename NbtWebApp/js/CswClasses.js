///// <reference path="../jquery/jquery-1.6.1-vsdoc.js" />
///// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
///// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
///// <reference path="_Global.js" />

function CswString(string)
{
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
    this.contains = function (string) { return value.indexOf(string) !== -1; };
}

CswString.prototype.toString = function () { return this.value; };