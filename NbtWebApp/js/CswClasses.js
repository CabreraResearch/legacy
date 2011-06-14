///// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
///// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
///// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
///// <reference path="_Global.js" />


//#region CswString
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

//#endregion CswString

//#region CswArray
function CswArray()
{
    var ctor = [];
    ctor.push.apply(arr, arguments);
    ctor.__proto__ = CswArray.prototype;
    return ctor;
}
CswArray.prototype = new Array;
CswArray.prototype.last = function ()
{
    return this[this.length - 1];
};

//#endregion CswArray

//#region CswLocalStorage
//function CswLocalStorage()
//{
//    localStorage.clear();
//    var ctor = localStorage;
//    ctor.__proto__ = CswLocalStorage.prototype;
//    return ctor;
//}

//CswLocalStorage.prototype.parseArray = function (name, arr)
//{
//    log(name, true);
//    log(arr, true);
//    if (arguments.length === 2)
//    {
//        this[name] = JSON.stringify(arr);
//    }

//    return JSON.parse(this[name]);
//};
//#endregion CswLocalStorage