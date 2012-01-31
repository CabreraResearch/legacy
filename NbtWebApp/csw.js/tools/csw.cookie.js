/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswCookie() { 
    'use strict';

    var cookie = (function () {

        var cookieNames = {
            SessionId: 'CswSessionId',
            Username: 'csw_username',
            LogoutPath: 'csw_logoutpath',
            CurrentNodeId: 'csw_currentnodeid',
            CurrentNodeKey: 'csw_currentnodekey',
            CurrentTabId: 'csw_currenttabid',
            CurrentActionName: 'csw_currentactionname',
            CurrentActionUrl: 'csw_currentactionurl',
            CurrentViewId: 'csw_currentviewid',
            CurrentViewMode: 'csw_currentviewmode',
            CurrentReportId: 'csw_currentreportid',
            LastActionName: 'csw_lastactionname',
            LastActionUrl: 'csw_lastactionurl',
            LastViewId: 'csw_lastviewid',
            LastViewMode: 'csw_lastviewmode',
            LastReportId: 'csw_lastreportid'
        };

        function get (cookiename) {
            /// <summary> Get the current value of a cookie by name.</summary>
            /// <param name="cookiename" type="String">A Csw cookie() cookieName</param>
            /// <returns type="Object">Cookie value</returns>
            var ret = Csw.string($.cookie(cookiename));
            return ret;
        }

        function set (cookiename, value) {
            /// <summary> Get the current value of a cookie by name.</summary>
            /// <param name="cookiename" type="String">A Csw cookie() cookieName</param>
            /// <param name="value" type="String">Value to assign to cookie</param>
            /// <returns type="Object">Cookie value</returns>
            return $.cookie(cookiename, value);
        }

        function clear (cookiename) {
            /// <summary> Clear the current value of a cookie by name.</summary>
            /// <param name="cookiename" type="String">A Csw cookie() cookieName</param>
            /// <returns type="Object">Cookie value</returns>
            return $.cookie(cookiename, '');
        }

        function clearAll () {
            /// <summary> Clear the current value of all Csw cookies.</summary>
            /// <returns type="Boolean">Always true.</returns>
            var cookieName;
            for (cookieName in cookieNames) {
                if (Csw.contains(cookieNames, cookieName)) {
                    $.cookie(cookieNames[cookieName], null);
                }
            }
            return true;
        }

        return {
            cookieNames: cookieNames,
            get: get,
            set: set,
            clear: clear,
            clearAll: clearAll
        };
    }());
    Csw.register('cookie', cookie);
    Csw.cookie = Csw.cookie || cookie;
    
})();




