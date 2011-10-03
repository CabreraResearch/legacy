/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />

//#region CswArray
function CswArray()
{
    var ctor = [];
    ctor.push.apply(arr, arguments);
    ctor.__proto__ = CswArray.prototype;
    return ctor;
}
CswArray.prototype = new Array;
CswArray.prototype = {
    last: function () {
        return this[this.length - 1];
    },
    contains: function (key) {
        return contains(this,key);
    }
};

//#endregion CswArray