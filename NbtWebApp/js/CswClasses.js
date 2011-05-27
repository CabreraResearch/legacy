///// <reference path="../jquery/jquery-1.6.1-vsdoc.js" />
///// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
///// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
///// <reference path="_Global.js" />

function CswString(string)
{
    this.value = string;
    this.val = function (string)
    {
        if (arguments.length === 1)
        {
            this.value = string;
            return this; //for chaining
        }
        else
        {
            return this.value;
        }
    };
    this.contains = function (string) { return this.value.indexOf(string) !== -1; };
}

