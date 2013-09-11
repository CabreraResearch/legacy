/// <reference path="CswApp-vsdoc.js" />

(function _mainTearDown() {

    Csw.main.onReady.then(function() {

        Csw.subscribe(Csw.enums.events.main.refreshHeader, function (eventObj, opts) {
            Csw.main.refreshHeaderMenu(opts);
        });

        Csw.main.register('refreshHeaderMenu', function (onSuccess) {
            var u = Csw.cookie.get(Csw.cookie.cookieNames.Username);
            Csw.main.headerMenu.empty();

            return Csw.main.headerMenu.menu({
                width: '100%',
                ajax: {
                    urlMethod: 'getHeaderMenu',
                    data: {}
                },
                
                useCache: true,

                onLogout: function () {
                    return Csw.clientSession.logout();
                },
                onQuotas: function () {
                    return Csw.main.handleAction({ 'actionname': 'Quotas' });
                },
                onModules: function () {
                    return Csw.main.handleAction({ 'actionname': 'Modules' });
                },
                onSubmitRequest: function () {
                    return Csw.main.handleAction({ 'actionname': 'Submit_Request' });
                },
                onSessions: function () {
                    return Csw.main.handleAction({ 'actionname': 'Sessions' });
                },
                onSubscriptions: function () {
                    return Csw.main.handleAction({ 'actionname': 'Subscriptions' });
                },
                onLoginData: function () {
                    return Csw.main.handleAction({ 'actionname': 'Login_Data' });
                },
                onImpersonate: function (userid, username) {
                    return Csw.main.handleImpersonation(userid, username, function () {
                        Csw.clientState.clearCurrent();
                        Csw.window.location(Csw.getGlobalProp('homeUrl'));
                    });
                },
                onEndImpersonation: function () {
                    return Csw.ajax.deprecatedWsNbt({
                        urlMethod: 'endImpersonation',
                        success: function (data) {
                            if (Csw.bool(data.result)) {
                                Csw.ajax.abortAll();
                                Csw.clientState.clearCurrent();
                                Csw.window.location(Csw.getGlobalProp('homeUrl'));
                            }
                        } // success
                    }); // ajax
                }, // onEndImpersonation
                onReturnToNbtManager: function () {
                    Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                    var sessionid = Csw.cookie.get(Csw.cookie.cookieNames.SessionId);
                    /* case 24669 */
                    Csw.cookie.clearAll();
                    return Csw.ajax.deprecatedWsNbt({
                        urlMethod: 'nbtManagerReauthenticate',
                        success: function (result) {
                            Csw.ajax.abortAll();
                            Csw.clientChanges.unsetChanged();
                            Csw.publish(Csw.enums.events.main.reauthenticate, { username: result.username, customerid: result.customerid });
                            Csw.window.location('Main.html');
                        }
                    });
                },
                onSuccess: onSuccess
            }); // CswMenuHeader
        });
        

    });
}());