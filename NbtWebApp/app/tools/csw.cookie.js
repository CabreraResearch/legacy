/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.cookie.cookieNames = Csw.cookie.cookieNames ||
        Csw.enums.register('cookieNames',
            {
                SessionId: 'CswSessionId',
                CustomerId: 'csw_customerid',
                Username: 'csw_username',
                OriginalUsername: 'csw_orig_username',
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

    Csw.cookie.get = Csw.cookie.get ||
        Csw.register('get', function (cookiename) {
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

    Csw.cookie.set = Csw.cookie.set ||
        Csw.register('set', function (cookiename, value) {
            /// <summary> Get the current value of a cookie by name.</summary>
            /// <param name="cookiename" type="String">A Csw cookie() cookieName</param>
            /// <param name="value" type="String">Value to assign to cookie</param>
            /// <returns type="Object">Cookie value</returns>
            return $.cookie(cookiename, value);
        });

    Csw.cookie.clear = Csw.cookie.clear ||
        Csw.register('clear', function (cookiename) {
            /// <summary> Clear the current value of a cookie by name.</summary>
            /// <param name="cookiename" type="String">A Csw cookie() cookieName</param>
            /// <returns type="Object">Cookie value</returns>
            return $.cookie(cookiename, '');
        });

    Csw.cookie.clearAll = Csw.cookie.clearAll ||
        Csw.register('clearAll', function () {
            /// <summary> Clear the current value of all Csw cookies.</summary>
            /// <returns type="Boolean">Always true.</returns>
            var cookieName;
            for (cookieName in Csw.cookie.cookieNames) {
                if (Csw.contains(Csw.cookie.cookieNames, cookieName) && cookieName !== Csw.cookie.cookieNames.CustomerId) {
                    $.cookie(Csw.cookie.cookieNames[cookieName], null);
                }
            }
            return true;
        });

}());




