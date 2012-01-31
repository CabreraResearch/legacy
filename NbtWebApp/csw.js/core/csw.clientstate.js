/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientState() {
    'use strict';
    
    function clientState() {
        /// <summary> Instance a Csw State object.</summary>
        /// <returns type="Object">Collection of methods to manage state.</returns>

        function clearCurrent() {
            /// <summary> Clear all current state cookies  </summary>
            /// <returns type="Boolean">Always true</returns>
            Csw.cookie.set(Csw.cookie.cookieNames.LastViewId, Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId));
            Csw.cookie.set(Csw.cookie.cookieNames.LastViewMode, Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode));
            Csw.cookie.set(Csw.cookie.cookieNames.LastActionName, Csw.cookie.get(Csw.cookie.cookieNames.CurrentActionName));
            Csw.cookie.set(Csw.cookie.cookieNames.LastActionUrl, Csw.cookie.get(Csw.cookie.cookieNames.CurrentActionUrl));
            Csw.cookie.set(Csw.cookie.cookieNames.LastReportId, Csw.cookie.get(Csw.cookie.cookieNames.CurrentReportId));

            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentViewId);
            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentViewMode);
            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentActionName);
            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentActionUrl);
            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentReportId);
            return true;
        }
        
        function setCurrentView(viewid, viewmode) {
            /// <summary> Store the current view in a cookie.</summary>
            /// <param name="viewid" type="String">An Nbt ViewId</param>
            /// <param name="viewmode" type="String">An Nbt ViewId</param>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            if (false === Csw.isNullOrEmpty(viewid) && false === Csw.isNullOrEmpty(viewmode)) {
                Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewId, viewid);
                Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewMode, viewmode);
            }
            return true;
        }

        function setCurrentAction(actionname, actionurl) {
            /// <summary> Store the current action in a cookie.</summary>
            /// <param name="actionname" type="String">An Nbt Action name</param>
            /// <param name="actionurl" type="String">An Nbt Action url</param>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionName, actionname);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionUrl, actionurl);
            return true;
        }

        function setCurrentReport(reportid) {
            /// <summary> Store the current report in a cookie.</summary>
            /// <param name="reportid" type="String">An Nbt ReportId</param>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentReportId, reportid);
            return true;
        }

        function getCurrent() {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId),
                viewmode: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode),
                actionname: Csw.cookie.get(Csw.cookie.cookieNames.CurrentActionName),
                actionurl: Csw.cookie.get(Csw.cookie.cookieNames.CurrentActionUrl),
                reportid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentReportId)
            };
        }

        function getLast() {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: Csw.cookie.get(Csw.cookie.cookieNames.LastViewId),
                viewmode: Csw.cookie.get(Csw.cookie.cookieNames.LastViewMode),
                actionname: Csw.cookie.get(Csw.cookie.cookieNames.LastActionName),
                actionurl: Csw.cookie.get(Csw.cookie.cookieNames.LastActionUrl),
                reportid: Csw.cookie.get(Csw.cookie.cookieNames.LastReportId)
            };
        }
        
        function setCurrent(json) {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Boolean">Always true.</returns>
            clearCurrent();
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewId, json.viewid);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewMode, json.viewmode);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionName, json.actionname);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionUrl, json.actionurl);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentReportId, json.reportid);
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