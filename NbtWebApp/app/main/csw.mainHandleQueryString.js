/// <reference path="CswApp-vsdoc.js" />

(function _queryString() {

    Csw.main.onReady.then(function() {
        
        Csw.main.register('handleQueryString', function () {
            var ret = false;
            var qs = Csw.queryString();

            var homeUrl = Csw.cookie.get(Csw.cookie.cookieNames.LogoutPath) || 'Main.html';
            if (Csw.clientSession.isDebug(qs)) {
                Csw.clientSession.enableDebug();
                if (window.location.pathname.endsWith('Dev.html')) {
                    homeUrl = 'Dev.html';
                    if (Csw.isNullOrEmpty(Csw.cookie.get(Csw.cookie.cookieNames.LogoutPath))) {
                        Csw.cookie.set(Csw.cookie.cookieNames.LogoutPath, homeUrl);
                    }
                }
            }
            Csw.clientDb.setItem('homeUrl', homeUrl);

            if (false == Csw.isNullOrEmpty(qs.action)) {
                var actopts = {};
                Csw.extend(actopts, qs);
                Csw.main.handleAction({ actionname: qs.action, ActionOptions: actopts });

            } else if (false == Csw.isNullOrEmpty(qs.viewid)) {
                var setView = function (viewid, viewmode) {
                    Csw.main.handleItemSelect({
                        type: 'view',
                        itemid: viewid,
                        mode: viewmode
                    });
                };
                if (Csw.isNullOrEmpty(qs.viewmode)) {
                    Csw.ajax.deprecatedWsNbt({
                        url: Csw.enums.ajaxUrlPrefix + 'getViewMode',
                        data: { ViewId: qs.viewid },
                        success: function (data) {
                            setView(qs.viewid, Csw.string(data.viewmode, 'tree'));
                        }
                    });
                } else {
                    setView(qs.viewid, Csw.string(qs.viewmode));
                }

            } else if (false == Csw.isNullOrEmpty(qs.nodeid)) {
                Csw.main.handleItemSelect({
                    type: 'view',
                    mode: 'tree',
                    linktype: 'link',
                    nodeid: qs.nodeid
                });

            } else if (false == Csw.isNullOrEmpty(qs.reportid)) {
                Csw.main.handleReport(qs.reportid);
                ret = true; // load the current context (probably the welcome landing page) below the report

            } else if (false == Csw.isNullOrEmpty(qs.clear)) {
                Csw.clientState.clearCurrent();
                ret = true;

            } else {
                ret = true;
            }

            return ret;
        });

    });
}());