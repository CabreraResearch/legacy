/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswClientSession() {
    'use strict';

    var cswPrivate = {
        AccessId: '',
        UserName: '',
        Password: '',
        ForMobile: false,
        onAuthenticate: null, // function (UserName) {} 
        onFail: null, // function (errormessage) {} 
        logoutpath: '',
        expiretimeInterval: '',
        expiretime: '',
        expiredInterval: '',
        isAuthenticated: false,
        logglyInput: '75c30bba-4b60-496c-a348-7eb413c01037',
        logglyLevel: 'error',
        server: 'localhost',
        debug: false
    };

    cswPrivate.checkExpired = function () {
        var now = new Date();
        if (Date.parse(cswPrivate.expiretime) - Date.parse(now) < 0) {
            window.clearInterval(cswPrivate.expiredInterval);
            Csw.clientSession.logout();
        }
    };

    cswPrivate.checkExpireTime = function () {
        if (cswPrivate.isAuthenticated) {
            var now = new Date();
            if (Date.parse(cswPrivate.expiretime) - Date.parse(now) < 180000) { // 3 minutes until timeout
                window.clearInterval(cswPrivate.expiretimeInterval);
                $.CswDialog('ExpireDialog', {
                    onYes: function () {
                        Csw.ajax.deprecatedWsNbt({
                            urlMethod: 'RenewSession',
                            success: function () {
                                cswPrivate.isAuthenticated = true;
                            }
                        });
                    }
                });
            }
        }
    };

    cswPrivate.setExpireTimeInterval = function () {
        if (cswPrivate.isAuthenticated) {
            window.clearInterval(cswPrivate.expiretimeInterval);
            window.clearInterval(cswPrivate.expiredInterval);
            cswPrivate.expiretimeInterval = window.setInterval(function () {
                cswPrivate.checkExpireTime();
            }, 60000);
            cswPrivate.expiredInterval = window.setInterval(function () {
                cswPrivate.checkExpired();
            }, 60000);
        }
    };

    Csw.clientSession.currentAccessId = Csw.clientSession.currentAccessId ||
        Csw.clientSession.register('currentAccessId', function () {
            return Csw.string(Csw.cookie.get(Csw.cookie.cookieNames.CustomerId), cswPrivate.AccessId);
        });

    Csw.clientSession.currentServer = Csw.clientSession.currentServer ||
        Csw.clientSession.register('currentServer', function () {
            return Csw.string(cswPrivate.server);
        });

    Csw.clientSession.currentUserName = Csw.clientSession.currentUserName ||
        Csw.clientSession.register('currentUserName', function () {
            return Csw.string(Csw.cookie.get(Csw.cookie.cookieNames.Username), cswPrivate.UserName);
        });

    Csw.clientSession.originalUserName = Csw.clientSession.originalUserName ||
        Csw.clientSession.register('originalUserName', function () {
            return Csw.string(Csw.cookie.get(Csw.cookie.cookieNames.OriginalUsername));
        });

    Csw.clientSession.currentSessionId = Csw.clientSession.currentSessionId ||
        Csw.clientSession.register('currentSessionId', function () {
            return Csw.string(Csw.cookie.get(Csw.cookie.cookieNames.SessionId));
        });

    Csw.clientSession.enableDebug = Csw.clientSession.enableDebug ||
        Csw.clientSession.register('enableDebug', function () {
            cswPrivate.logglyLevel = 'info';
            cswPrivate.debug = true;
        });

    Csw.clientSession.isDebug = Csw.clientSession.isDebug ||
        Csw.clientSession.register('isDebug', function (qs) {
            if (qs && (Csw.bool(qs.debug) || 'dev.html' === Csw.string(qs.pageName).toLowerCase())) {
                cswPrivate.debug = true;
            }
            return cswPrivate.debug;
        });

    Csw.clientSession.setLogglyInput = Csw.clientSession.setLogglyInput ||
        Csw.clientSession.register('setLogglyInput', function (logglyInput, logglyLevel, server) {
            if (false === Csw.isNullOrEmpty(logglyInput) && logglyInput.length == 36) {
                cswPrivate.logglyInput = logglyInput;
            }
            if (false === Csw.isNullOrEmpty(logglyLevel) && Csw.contains(Csw.debug.logLevels(), logglyLevel)) {
                cswPrivate.logglyLevel = logglyLevel;
            }
            if (false === Csw.isNullOrEmpty(server)) {
                cswPrivate.server = server;
            }
        });

    Csw.clientSession.getLogglyInput = Csw.clientSession.getLogglyInput ||
        Csw.clientSession.register('getLogglyInput', function () {
            var ret = '75c30bba-4b60-496c-a348-7eb413c01037';
            if (false === Csw.isNullOrEmpty(cswPrivate.logglyInput) &&
                cswPrivate.logglyInput.length == 36) {
                ret = cswPrivate.logglyInput;
            }
            return ret;
        });

    Csw.clientSession.getLogglyLevel = Csw.clientSession.getLogglyLevel ||
        Csw.clientSession.register('getLogglyLevel', function () {
            var ret = 'error';
            if (Csw.bool(cswPrivate.debug)) {
                ret = 'info';
            }
            else if (false === Csw.isNullOrEmpty(cswPrivate.logglyLevel) &&
                Csw.contains(Csw.debug.logLevels(), cswPrivate.logglyLevel)) {
                ret = cswPrivate.logglyLevel;
            }
            return ret;
        });

    Csw.clientSession.finishLogout = Csw.clientSession.finishLogout ||
        Csw.clientSession.register('finishLogout', function () {
            ///<summary>Complete the logout. Nuke any lingering client-side data.</summary>
            cswPrivate.logoutpath = cswPrivate.logoutpath || Csw.cookie.get(Csw.cookie.cookieNames.LogoutPath);
            Csw.clientDb.clear();
            Csw.cookie.clearAll();
            if (false === Csw.isNullOrEmpty(cswPrivate.logoutpath)) {
                Csw.window.location(cswPrivate.logoutpath);
            } else {
                Csw.window.location(Csw.getGlobalProp('homeUrl'));
            }
        });

    var onLoginSuccess = function (data) {
        //Csw.cookie.set(Csw.cookie.cookieNames.CustomerId, cswPrivate.AccessId);
        //Csw.clientSession.setUsername(cswPrivate.UserName);

        //Case 29617: Once a logout path has been set, do not mutate it.
        if (Csw.isNullOrEmpty(Csw.cookie.get(Csw.cookie.cookieNames.LogoutPath))) {
            Csw.cookie.set(Csw.cookie.cookieNames.LogoutPath, cswPrivate.logoutpath);
        }
        Csw.tryExec(cswPrivate.onAuthenticate, cswPrivate.UserName);
        Csw.cookie.set(Csw.cookie.cookieNames.UserDefaults, JSON.stringify(data));
    };

    Csw.clientSession.login = Csw.clientSession.login ||
        Csw.clientSession.register('login', function (loginopts) {
            ///<summary>Attempt a login.</summary>
            Csw.extend(cswPrivate, loginopts);
            cswPrivate.isAuthenticated = true;
            return Csw.ajaxWcf.post({
                urlMethod: 'Session/Init',
                data: {
                    CustomerId: cswPrivate.AccessId,
                    UserName: cswPrivate.UserName,
                    Password: cswPrivate.Password,
                    IsMobile: cswPrivate.ForMobile
                },
                success: onLoginSuccess,
                onloginfail: function (txt) {
                    cswPrivate.isAuthenticated = false;
                    Csw.tryExec(cswPrivate.onFail, txt);
                },
                error: function () {
                    cswPrivate.isAuthenticated = false;
                    Csw.tryExec(cswPrivate.onFail, 'Webservice Error');
                }
            }); // ajax
        });

    Csw.clientSession.logout = Csw.clientSession.logout ||
        Csw.clientSession.register('logout', function (options) {
            ///<summary>End the current session.</summary>
            Csw.extend(cswPrivate, options);

            cswPrivate.isAuthenticated = false;
            return Csw.ajaxWcf.post({
                urlMethod: 'Session/EndWithAuth',
                data: {},
                complete: function () {
                    Csw.clientSession.finishLogout();
                }
            });
        });

    Csw.clientSession.getExpireTime = Csw.clientSession.getExpireTime ||
        Csw.clientSession.register('getExpireTime', function () {
            ///<summary>Get the expiration time.</summary>
            return cswPrivate.expiretime;
        });

    Csw.clientSession.setExpireTime = Csw.clientSession.setExpireTime ||
        Csw.clientSession.register('setExpireTime', function (value) {
            ///<summary>Define the expiration time.</summary>
            cswPrivate.expiretime = value;
            cswPrivate.setExpireTimeInterval();
        });

    Csw.clientSession.handleAuthenticationStatus = Csw.clientSession.handleAuthenticationStatus ||
        Csw.clientSession.register('handleAuthenticationStatus', function (options) {
            ///<summary>Each web request will authenticate. Depending on the current authentication status, ajax onSuccess methods may need to change.</summary>
            var o = {
                status: '',
                txt: '',
                success: function () {
                },
                failure: function () {
                },
                usernodeid: '',
                usernodekey: '',
                passwordpropid: '',
                ForMobile: false
            };
            Csw.extend(o, options);

            var txt = o.txt;
            if (o.status === 'Authenticated') {
                Csw.tryExec(o.success());
            } else {
                switch (o.status) {
                    case 'ExpiredPassword':
                        $.CswDialog('ChangePasswordDialog', {
                            UserId: o.data.ExpirationReset.UserId,
                            UserKey: o.data.ExpirationReset.UserKey,
                            PasswordId: o.data.ExpirationReset.PasswordId,
                            onSuccess: function () {
                            }
                        });
                        break;
                    case 'ShowLicense':
                        $.CswDialog('ShowLicenseDialog', {});
                        break;
                    case 'AlreadyLoggedIn':
                        $.CswDialog('LogoutExistingSessionsDialog', o.success);
                        break;
                }

                if (!txt) {
                    Csw.tryExec(o.success());
                } else {
                    Csw.tryExec(o.failure, txt, o.status);
                }
            }
        });

    Csw.clientSession.isAdministrator = Csw.clientSession.isAdministrator ||
        Csw.clientSession.register('isAdministrator', function (options) {
            ///<returns type="bool">True if the current session has Administrative priveleges</returns>
            var o = {
                'Yes': null,
                'No': null
            };
            if (options) {
                Csw.extend(o, options);
            }

            return Csw.ajax.deprecatedWsNbt({
                urlMethod: 'isAdministrator',
                success: function (data) {
                    if (Csw.bool(data.Administrator)) {
                        Csw.tryExec(o.Yes);
                    } else {
                        Csw.tryExec(o.No);
                    }
                }
            });
        }); // isAdministrator()


    Csw.clientSession.userDefaults = Csw.clientSession.userDefaults ||
        Csw.clientSession.register('userDefaults', function () {
            var ret = {};
            var userDefaults = Csw.cookie.get(Csw.cookie.cookieNames.UserDefaults);
            if (false === Csw.isNullOrEmpty(userDefaults)) {
                ret = JSON.parse(userDefaults);
            }
            return ret;
        }); // userDefaults()
}());
