/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    /// <summary> Instance a Csw State object.</summary>
    /// <returns type="Object">Collection of methods to manage state.</returns>

    Csw.clientState.clearCurrent = Csw.clientState.clearCurrent ||
        Csw.clientState.register('clearCurrent', function () {
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
        });

    Csw.clientState.clearLast = Csw.clientState.clearLast ||
        Csw.clientState.register('clearLast', function() {
            /// <summary> Clear all last state cookies  </summary>
            /// <returns type="Boolean">Always true</returns>
            Csw.cookie.clear(Csw.cookie.cookieNames.LastViewId);
            Csw.cookie.clear(Csw.cookie.cookieNames.LastViewMode);
            Csw.cookie.clear(Csw.cookie.cookieNames.LastActionName);
            Csw.cookie.clear(Csw.cookie.cookieNames.LastActionUrl);
            //Csw.cookie.clear(Csw.cookie.cookieNames.LastReportId);
            Csw.cookie.clear(Csw.cookie.cookieNames.LastSearchId);
            return true;
        });

    Csw.clientState.setCurrentView = Csw.clientState.setCurrentView ||
        Csw.clientState.register('setCurrentView', function (viewid, viewmode) {
            /// <summary> Store the current view in a cookie.</summary>
            /// <param name="viewid" type="String">An Nbt ViewId</param>
            /// <param name="viewmode" type="String">An Nbt ViewId</param>
            /// <returns type="Boolean">Always true</returns>
            Csw.clientState.clearCurrent();
            if (false === Csw.isNullOrEmpty(viewid) && false === Csw.isNullOrEmpty(viewmode)) {
                Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewId, viewid);
                Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewMode, viewmode);
            }
            return true;
        });

    Csw.clientState.setCurrentAction = Csw.clientState.setCurrentAction ||
        Csw.clientState.register('setCurrentAction', function (actionname, actionurl) {
            /// <summary> Store the current action in a cookie.</summary>
            /// <param name="actionname" type="String">An Nbt Action name</param>
            /// <param name="actionurl" type="String">An Nbt Action url</param>
            /// <returns type="Boolean">Always true</returns>
            Csw.clientState.clearCurrent();
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionName, actionname);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionUrl, actionurl);
            return true;
        });

    Csw.clientState.setCurrentSearch = Csw.clientState.setCurrentSearch ||
        Csw.clientState.register('setCurrentSearch', function (searchid) {
            /// <summary> Store the current search in a cookie.</summary>
            /// <param name="searchid" type="String">A search id</param>
            /// <returns type="Boolean">Always true</returns>
            Csw.clientState.clearCurrent();
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentSearchId, searchid);
            return true;
        });

    Csw.clientState.getCurrent = Csw.clientState.getCurrent ||
        Csw.clientState.register('getCurrent', function () {
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
        });

    Csw.clientState.getLast = Csw.clientState.getLast ||
        Csw.clientState.register('getLast', function () {
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
        });

    Csw.clientState.setCurrent = Csw.clientState.setCurrent ||
        Csw.clientState.register('setCurrent', function (json) {
            /// <summary> Get all current state data from the cookie.</summary>
            /// <returns type="Boolean">Always true.</returns>
            Csw.clientState.clearCurrent();
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewId, json.viewid);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentViewMode, json.viewmode);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionName, json.actionname);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentActionUrl, json.actionurl);
            //Csw.cookie.set(Csw.cookie.cookieNames.CurrentReportId, json.reportid);
            Csw.cookie.set(Csw.cookie.cookieNames.CurrentSearchId, json.searchid);
            return true;
        });

} ());
