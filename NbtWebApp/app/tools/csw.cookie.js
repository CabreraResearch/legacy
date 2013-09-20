/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';


    Csw.cookie.register('cookieNames',
        {
            // Server set
            SessionId: 'CswSessionId',
            CustomerId: 'CswAccessId',
            Username: 'CswUsername',
            OriginalUsername: 'CswOriginalUsername',

            LogoutPath: 'csw_logoutpath',

            UserDefaults: 'csw_userdefaults',

            CurrentNodeId: 'csw_currentnodeid',
            CurrentNodeKey: 'csw_currentnodekey',
            CurrentTabId: 'csw_currenttabid',
            CurrentActionName: 'csw_currentactionname',
            CurrentActionUrl: 'csw_currentactionurl',
            CurrentViewId: 'csw_currentviewid',
            CurrentViewMode: 'csw_currentviewmode',
            //CurrentReportId: 'csw_currentreportid',
            CurrentSearchId: 'csw_currentsearchid',

            LastActionName: 'csw_lastactionname',
            LastActionUrl: 'csw_lastactionurl',
            LastViewId: 'csw_lastviewid',
            LastViewMode: 'csw_lastviewmode',
            //LastReportId: 'csw_lastreportid',
            LastSearchId: 'csw_lastsearchid'
        }
    );

    Csw.cookie.register('get', function (cookiename) {
        /// <summary> Get the current value of a cookie by name.</summary>
        /// <param name="cookiename" type="String">A Csw cookie() cookieName</param>
        /// <returns type="Object">Cookie value</returns>
        var cookie = $.cookie(cookiename);
        var ret = '';
        if (cookie !== "[object Object]") {
            ret = Csw.string(cookie);
        }
        return ret;
    });

    Csw.cookie.register('set', function (cookiename, value) {
        /// <summary> Get the current value of a cookie by name.</summary>
        /// <param name="cookiename" type="String">A Csw cookie() cookieName</param>
        /// <param name="value" type="String">Value to assign to cookie</param>
        /// <returns type="Object">Cookie value</returns>
        return $.cookie(cookiename, value);
    });

    Csw.cookie.register('clear', function (cookiename) {
        /// <summary> Clear the current value of a cookie by name.</summary>
        /// <param name="cookiename" type="String">A Csw cookie() cookieName</param>
        /// <returns type="Object">Cookie value</returns>
        return $.cookie(cookiename, '');
    });

    Csw.cookie.register('clearAll', function (exceptions) {
        /// <summary> Clear the current value of all Csw cookies.</summary>
        /// <returns type="Boolean">Always true.</returns>
        var preserveThese = [];
        exceptions = exceptions || [];
        Csw.iterate(exceptions, function (cookieName) {
            var val = Csw.cookie.get(cookieName);
            if (false === Csw.isNullOrEmpty(val)) {
                preserveThese.push({ name: cookieName, val: val });
            }
        });

        Csw.iterate(Csw.cookie.cookieNames, function (name) {
            $.cookie(name, null);
        });
        Csw.iterate($.cookie(), function (name) {
            $.cookie(name, null);
        });

        Csw.iterate(preserveThese, function (cook) {
            Csw.cookie.set(cook.name, cook.val);
        });

        return true;
    });

}());




