/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswClientState() {
    'use strict';

    var clientState = (function clientStateP() {
        /// <summary> Instance a Csw State object.</summary>
        /// <returns type="Object">Collection of methods to manage state.</returns>

        var external = {};

        external.clearCurrent = function () {
            /// <summary> Clear all current state cookies  </summary>
            /// <returns type="Boolean">Always true</returns>
            Csw.cookie.set(Csw.cookie.cookieNames.LastViewId, Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId));
            Csw.cookie.set(Csw.cookie.cookieNames.LastViewMode, Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode));
            Csw.cookie.set(Csw.cookie.cookieNames.LastActionName, Csw.cookie.get(Csw.cookie.cookieNames.CurrentActionName));
            Csw.cookie.set(Csw.cookie.cookieNames.LastActionUrl, Csw.cookie.get(Csw.cookie.cookieNames.CurrentActionUrl));
            //Csw.cookie.set(Csw.cookie.cookieNames.LastReportId, Csw.cookie.get(Csw.cookie.cookieNames.CurrentReportId));
            Csw.cookie.set(Csw.cookie.cookieNames.LastSearchId, Csw.cookie.get(Csw.cookie.cookieNames.CurrentSearchId));

            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentViewId);
            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentViewMode);
            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentActionName);
            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentActionUrl);
            //Csw.cookie.clear(Csw.cookie.cookieNames.CurrentReportId);
            Csw.cookie.clear(Csw.cookie.cookieNames.CurrentSearchId);
            return true;
        };

        external.clearLast = function () {
            /// <summary> Clear all last state cookies  </summary>
            /// <returns type="Boolean">Always true</returns>
            Csw.cookie.clear(Csw.cookie.cookieNames.LastViewId);
            Csw.cookie.clear(Csw.cookie.cookieNames.LastViewMode);
            Csw.cookie.clear(Csw.cookie.cookieNames.LastActionName);
            Csw.cookie.clear(Csw.cookie.cookieNames.LastActionUrl);
            //Csw.cookie.clear(Csw.cookie.cookieNames.LastReportId);
            Csw.cookie.clear(Csw.cookie.cookieNames.LastSearchId);
            return true;
        };

        external.setCurrentView = function (viewid, viewmode) {
            /// <summary> Store the current view in a cookie.</summary>
            /// <param name="viewid" type="String">An Nbt ViewId</param>
            /// <param name="viewmode" type="String">An Nbt ViewId</param>
            /// <returns type="Boolean">Always true</returns>
            external.clearCurrent();
            if (false === Csw.isNullOrEmpty(viewid) && false === Csw.isNullOrEmpty(viewmode)) {
                Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewId, viewid);
                Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewMode, viewmode);
            }
            return true;
        };

        external.setCurrentAction = function (actionname, actionurl) {
            /// <summary> Store the current action in a cookie.</summary>
            /// <param name="actionname" type="String">An Nbt Action name</param>
            /// <param name="actionurl" type="String">An Nbt Action url</param>
            /// <returns type="Boolean">Always true</returns>
            external.clearCurrent();
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionName, actionname);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionUrl, actionurl);
            return true;
        };

//        external.setCurrentReport = function (reportid) {
//            /// <summary> Store the current report in a cookie.</summary>
//            /// <param name="reportid" type="String">An Nbt ReportId</param>
//            /// <returns type="Boolean">Always true</returns>
//            external.clearCurrent();
//            Csw.cookie.set(Csw.cookie.cookieNames.CurrentReportId, reportid);
//            return true;
//        };

        external.setCurrentSearch = function (searchid) {
            /// <summary> Store the current search in a cookie.</summary>
            /// <param name="searchid" type="String">A search id</param>
            /// <returns type="Boolean">Always true</returns>
            external.clearCurrent();
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentSearchId, searchid);
            return true;
        };

        external.getCurrent = function () {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewId),
                viewmode: Csw.cookie.get(Csw.cookie.cookieNames.CurrentViewMode),
                actionname: Csw.cookie.get(Csw.cookie.cookieNames.CurrentActionName),
                actionurl: Csw.cookie.get(Csw.cookie.cookieNames.CurrentActionUrl),
                //reportid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentReportId),
                searchid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentSearchId)
            };
        };

        external.getLast = function () {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Object">Views, actions and reports</returns>
            return {
                viewid: Csw.cookie.get(Csw.cookie.cookieNames.LastViewId),
                viewmode: Csw.cookie.get(Csw.cookie.cookieNames.LastViewMode),
                actionname: Csw.cookie.get(Csw.cookie.cookieNames.LastActionName),
                actionurl: Csw.cookie.get(Csw.cookie.cookieNames.LastActionUrl),
                //reportid: Csw.cookie.get(Csw.cookie.cookieNames.LastReportId),
                searchid: Csw.cookie.get(Csw.cookie.cookieNames.LastSearchId)
            };
        };

        external.setCurrent = function (json) {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Boolean">Always true.</returns>
            external.clearCurrent();
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewId, json.viewid);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewMode, json.viewmode);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionName, json.actionname);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionUrl, json.actionurl);
            //Csw.cookie.set(Csw.cookie.cookieNames.CurrentReportId, json.reportid);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentSearchId, json.searchid);
            return true;
        };

        return external;

    } ());
    Csw.register('clientState', clientState);
    Csw.clientState = Csw.clientState || clientState;

} ());
