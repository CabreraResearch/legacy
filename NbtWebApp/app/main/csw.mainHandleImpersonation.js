/// <reference path="CswApp-vsdoc.js" />

(function _impersonation() {

    Csw.main.onReady.then(function() {
        
        Csw.subscribe(Csw.enums.events.RestoreViewContext, Csw.main.loadImpersonation);

        Csw.main.register('loadImpersonation', function (eventObj, actionData) {
            if (false === Csw.isNullOrEmpty(actionData.userid)) {
                return Csw.main.handleImpersonation(actionData.userid, actionData.username, function () {
                    Csw.main.initAll(function () {
                        Csw.main.handleItemSelect({
                            itemid: Csw.string(actionData.actionid, actionData.viewid),
                            nodeid: actionData.selectedNodeId,
                            mode: actionData.viewmode,
                            name: actionData.actionname,
                            url: actionData.actionurl,
                            type: actionData.type
                        });
                    });
                });
            } else {
                return Csw.main.handleItemSelect({
                    itemid: Csw.string(actionData.actionid, actionData.viewid),
                    nodeid: actionData.selectedNodeId,
                    mode: actionData.viewmode,
                    name: actionData.actionname,
                    url: actionData.actionurl,
                    type: actionData.type
                });
            }
        });
        

        Csw.main.register('handleImpersonation', function (userid, username, onSuccess) {
            //var u = Csw.cookie.get(Csw.cookie.cookieNames.Username);
            return Csw.ajax.deprecatedWsNbt({
                urlMethod: 'impersonate',
                data: { UserId: userid },
                success: function (data) {
                    if (Csw.bool(data.result)) {
                        Csw.tryExec(onSuccess);
                    }
                } // success
            }); // ajax
        });

    });
}());