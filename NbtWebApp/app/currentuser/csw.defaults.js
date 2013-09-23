/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswClientSession() {
    'use strict';
    
    var defaults = function () {
        var ret = {};
        var userDefaults = Csw.cookie.get(Csw.cookie.cookieNames.UserDefaults);
        if (false === Csw.isNullOrEmpty(userDefaults)) {
            ret = JSON.parse(userDefaults);
        }
        return ret;
    };

    Csw.currentUser.register('defaults', defaults);

    Csw.currentUser.register('dateFormat', function () {
        var dflt = defaults();
        var ret = Csw.string(dflt['JS Date Format'], 'n/j/Y');
        return ret;
    });

    Csw.currentUser.register('timeFormat', function () {
        var dflt = defaults();
        var ret = Csw.string(dflt['JS Time Format'], 'g:i:s a');
        return ret;
    });

}());
