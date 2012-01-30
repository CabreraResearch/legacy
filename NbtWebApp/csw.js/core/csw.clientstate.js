/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientState() {
    'use strict';

    function clientState() {

        function clearCurrent () {
            /// <summary> Clear all current state cookies  </summary>
            /// <returns type="Boolean">Always true</returns>
            $.CswCookie('set', CswCookieName.LastViewId, $.CswCookie('get', CswCookieName.CurrentViewId));
            $.CswCookie('set', CswCookieName.LastViewMode, $.CswCookie('get', CswCookieName.CurrentViewMode));
            $.CswCookie('set', CswCookieName.LastActionName, $.CswCookie('get', CswCookieName.CurrentActionName));
            $.CswCookie('set', CswCookieName.LastActionUrl, $.CswCookie('get', CswCookieName.CurrentActionUrl));
            $.CswCookie('set', CswCookieName.LastReportId, $.CswCookie('get', CswCookieName.CurrentReportId));

            $.CswCookie('clear', CswCookieName.CurrentViewId);
            $.CswCookie('clear', CswCookieName.CurrentViewMode);
            $.CswCookie('clear', CswCookieName.CurrentActionName);
            $.CswCookie('clear', CswCookieName.CurrentActionUrl);
            $.CswCookie('clear', CswCookieName.CurrentReportId);
            return true;
        }
        
        function setCurrentView (viewid, viewmode) {
            /// <summary> Store the current view in a cookie.</summary>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            if (false === Csw.isNullOrEmpty(viewid) && false === Csw.isNullOrEmpty(viewmode)) {
                $.CswCookie('set', CswCookieName.CurrentViewId, viewid);
                $.CswCookie('set', CswCookieName.CurrentViewMode, viewmode);
            }
            return true;
        }

        function setCurrentAction (actionname, actionurl) {
            /// <summary> Store the current action in a cookie.</summary>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            $.CswCookie('set', CswCookieName.CurrentActionName, actionname);
            $.CswCookie('set', CswCookieName.CurrentActionUrl, actionurl);
            return true;
        }

        function setCurrentReport (reportid) {
            /// <summary> Store the current report in a cookie.</summary>
            /// <returns type="Boolean">Always true</returns>
            clearCurrent();
            $.CswCookie('set', CswCookieName.CurrentReportId, reportid);
            return true;
        }

        function getCurrent () {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: $.CswCookie('get', CswCookieName.CurrentViewId),
                viewmode: $.CswCookie('get', CswCookieName.CurrentViewMode),
                actionname: $.CswCookie('get', CswCookieName.CurrentActionName),
                actionurl: $.CswCookie('get', CswCookieName.CurrentActionUrl),
                reportid: $.CswCookie('get', CswCookieName.CurrentReportId)
            };
        }

        function getLast () {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: $.CswCookie('get', CswCookieName.LastViewId),
                viewmode: $.CswCookie('get', CswCookieName.LastViewMode),
                actionname: $.CswCookie('get', CswCookieName.LastActionName),
                actionurl: $.CswCookie('get', CswCookieName.LastActionUrl),
                reportid: $.CswCookie('get', CswCookieName.LastReportId)
            };
        }

        function setCurrent (json) {
            
            clearCurrent();
            $.CswCookie('set', CswCookieName.CurrentViewId, json.viewid);
            $.CswCookie('set', CswCookieName.CurrentViewMode, json.viewmode);
            $.CswCookie('set', CswCookieName.CurrentActionName, json.actionname);
            $.CswCookie('set', CswCookieName.CurrentActionUrl, json.actionurl);
            $.CswCookie('set', CswCookieName.CurrentReportId, json.reportid);
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
    Csw.clientState.clearCurrent = Csw.clientState.clearCurrent || clientState.clearCurrent;
    Csw.clientState.setCurrentView = Csw.clientState.setCurrentView || clientState.setCurrentView;
    Csw.clientState.setCurrentAction = Csw.clientState.setCurrentAction || clientState.setCurrentAction;
    Csw.clientState.setCurrentReport = Csw.clientState.setCurrentReport || clientState.setCurrentReport;
    Csw.clientState.getCurrent = Csw.clientState.getCurrent || clientState.getCurrent;
    Csw.clientState.getLast = Csw.clientState.getLast || clientState.getLast;
    Csw.clientState.setCurrent = Csw.clientState.setCurrent || clientState.setCurrent;
    
}());