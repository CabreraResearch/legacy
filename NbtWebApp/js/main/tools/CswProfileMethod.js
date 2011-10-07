/// <reference path="../js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

//#region CswProfileMethod
function CswProfileMethod(methodName)
{
    this.name = methodName;
    this.started = Date();
    this.ended = Date();
    this.ajaxSuccess = Date();
}
CswProfileMethod.prototype = {
    setAjaxSuccess: function ()
    {
        this.ajaxSuccess = Date();
        return this;
    },
    setEnded: function ()
    {
        this.ended = Date();
        return this;
    },
    toString: function ()
    {
        var $stats = $('<' + this.name + '></' + this.name + '>');
        $stats.append('<started>' + this.started + '</started>');
        $stats.append('<ajaxsuccess>' + this.ajaxSuccess + '</ajaxsuccess>');
        $stats.append('<ended>' + this.ended + '</ended>');
        return xmlToString($stats);
    },
    toHtml: function ()
    {
        var $stats = $('<div>' + this.name + '</div>');
        $stats.append('<p>started: ' + this.started + '</p>');
        $stats.append('<p>ajaxsuccess: ' + this.ajaxSuccess + '</p>');
        $stats.append('<p>ended: ' + this.ended + '</p>');
        return xmlToString($stats);
    }
};
//#endregion CswProfileMethod