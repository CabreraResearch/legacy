/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />

//#region CswProfileMethod
//You're probably better off using console.timer
function CswProfileMethod(methodName) {
    var name = methodName,
        started = Date(),
        ended = Date(),
        ajaxSuccess = Date();

    return {
        setAjaxSuccess: function() {
            ajaxSuccess = Date();
            return ajaxSuccess;
        },
        setEnded: function() {
            ended = Date();
            return ended;
        },
        toString: function() {
            var $stats = $('<' + name + '></' + name + '>');
            $stats.append('<started>' + started + '</started>');
            $stats.append('<ajaxsuccess>' + ajaxSuccess + '</ajaxsuccess>');
            $stats.append('<ended>' + ended + '</ended>');
            return $stats.html();
        },
        toHtml: function() {
            var $stats = $('<div>' + name + '</div>');
            $stats.append('<p>started: ' + started + '</p>');
            $stats.append('<p>ajaxsuccess: ' + ajaxSuccess + '</p>');
            $stats.append('<p>ended: ' + ended + '</p>');
            return $stats.html();
        }
    };
}
//#endregion CswProfileMethod