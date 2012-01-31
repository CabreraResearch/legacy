/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientState() {
    'use strict';
    
    function clientState() {
        /// <summary> Instance a Csw State object.</summary>
        /// <returns type="Object">Collection of methods to manage state.</returns>

        var cswCookie = Csw.cookie();

        function clearCurrent() {
            /// <summary> Clear all current state cookies  </summary>
            /// <returns type="Boolean">Always true</returns>
            cswCookie.set(cswCookie.cookieNames.LastViewId, cswCookie.get(cswCookie.cookieNames.CurrentViewId));
            cswCookie.set(cswCookie.cookieNames.LastViewMode, cswCookie.get(cswCookie.cookieNames.CurrentViewMode));
            cswCookie.set(cswCookie.cookieNames.LastActionName, cswCookie.get(cswCookie.cookieNames.CurrentActionName));
            cswCookie.set(cswCookie.cookieNames.LastActionUrl, cswCookie.get(cswCookie.cookieNames.CurrentActionUrl));
            cswCookie.set(cswCookie.cookieNames.LastReportId, cswCookie.get(cswCookie.cookieNames.CurrentReportId));

            cswCookie.clear(cswCookie.cookieNames.CurrentViewId);
            cswCookie.clear(cswCookie.cookieNames.CurrentViewMode);
            cswCookie.clear(cswCookie.cookieNames.CurrentActionName);
            cswCookie.clear(cswCookie.cookieNames.CurrentActionUrl);
            cswCookie.clear(cswCookie.cookieNames.CurrentReportId);
            return true;
        }
        
        function setCurrentView(viewid, viewmode) {
            /// <summary> Store the current view in a cookie.</summary>
            /// <param name="viewid" type="String">An Nbt ViewId</param>
            /// <param name="viewmode" type="String">An Nbt ViewId</param>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            if (false === Csw.isNullOrEmpty(viewid) && false === Csw.isNullOrEmpty(viewmode)) {
                cswCookie.set(cswCookie.cookieNames.CurrentViewId, viewid);
                cswCookie.set(cswCookie.cookieNames.CurrentViewMode, viewmode);
            }
            return true;
        }

        function setCurrentAction(actionname, actionurl) {
            /// <summary> Store the current action in a cookie.</summary>
            /// <param name="actionname" type="String">An Nbt Action name</param>
            /// <param name="actionurl" type="String">An Nbt Action url</param>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            cswCookie.set(cswCookie.cookieNames.CurrentActionName, actionname);
            cswCookie.set(cswCookie.cookieNames.CurrentActionUrl, actionurl);
            return true;
        }

        function setCurrentReport(reportid) {
            /// <summary> Store the current report in a cookie.</summary>
            /// <param name="reportid" type="String">An Nbt ReportId</param>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            cswCookie.set(cswCookie.cookieNames.CurrentReportId, reportid);
            return true;
        }

        function getCurrent() {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: cswCookie.get(cswCookie.cookieNames.CurrentViewId),
                viewmode: cswCookie.get(cswCookie.cookieNames.CurrentViewMode),
                actionname: cswCookie.get(cswCookie.cookieNames.CurrentActionName),
                actionurl: cswCookie.get(cswCookie.cookieNames.CurrentActionUrl),
                reportid: cswCookie.get(cswCookie.cookieNames.CurrentReportId)
            };
        }

        function getLast() {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: cswCookie.get(cswCookie.cookieNames.LastViewId),
                viewmode: cswCookie.get(cswCookie.cookieNames.LastViewMode),
                actionname: cswCookie.get(cswCookie.cookieNames.LastActionName),
                actionurl: cswCookie.get(cswCookie.cookieNames.LastActionUrl),
                reportid: cswCookie.get(cswCookie.cookieNames.LastReportId)
            };
        }
        
        function setCurrent(json) {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Boolean">Always true.</returns>
            clearCurrent();
            cswCookie.set(cswCookie.cookieNames.CurrentViewId, json.viewid);
            cswCookie.set(cswCookie.cookieNames.CurrentViewMode, json.viewmode);
            cswCookie.set(cswCookie.cookieNames.CurrentActionName, json.actionname);
            cswCookie.set(cswCookie.cookieNames.CurrentActionUrl, json.actionurl);
            cswCookie.set(cswCookie.cookieNames.CurrentReportId, json.reportid);
            return true;
        }
        
        return {
            clearCurrent: clearCurrent,
            setCurrentView: setCurrentView,
            setCurrentAction: setCurrentAction,
            setCurrentReport: setCurrentReport,
            getCurrent: getCurrent,
            getLast: getLast,
            setCurrent: setCurrent
        };

    }
    Csw.register('clientState', clientState);
    Csw.clientState = Csw.clientState || clientState;
    
}());