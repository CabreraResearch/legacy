/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
        authenticateUrl: '/NbtWebApp/wsNBT.asmx/authenticate',
        DeauthenticateUrl: '/NbtWebApp/wsNBT.asmx/deauthenticate',
        expiretimeInterval: '',
        expiretime: '',
        expiredInterval: '',
        isAuthenticated: false
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
                        Csw.ajax.post({
                            'url': '/NbtWebApp/wsNBT.asmx/RenewSession',
                            'success': function () {
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

    Csw.clientSession = Csw.clientSession ||
        Csw.register('clientSession', Csw.makeNameSpace());

    Csw.currentAccessId = Csw.currentAccessId ||
        Csw.clientSession.register('currentAccessId', function () {
            return Csw.cookie.get(Csw.cookie.cookieNames.CustomerId);
        });

    Csw.clientSession.finishLogout = Csw.clientSession.finishLogout ||
        Csw.clientSession.register('finishLogout', function () {
            ///<summary>Complete the logout. Nuke any lingering client-side data.</summary>
            cswPrivate.logoutpath = Csw.cookie.get(Csw.cookie.cookieNames.LogoutPath);
            Csw.clientDb.clear();
            Csw.cookie.clearAll();
            if (false === Csw.isNullOrEmpty(cswPrivate.logoutpath)) {
                Csw.window.location(cswPrivate.logoutpath);
            } else {
                Csw.window.location(Csw.getGlobalProp('homeUrl'));
            }
        });

    Csw.clientSession.login = Csw.clientSession.login ||
        Csw.clientSession.register('login', function (loginopts) {
            ///<summary>Attempt a login.</summary>
            if (loginopts) {
                $.extend(cswPrivate, loginopts);
            }
            cswPrivate.isAuthenticated = true;
            Csw.ajax.post({
                url: cswPrivate.authenticateUrl,
                data: {
                    AccessId: cswPrivate.AccessId,
                    UserName: cswPrivate.UserName,
                    Password: cswPrivate.Password,
                    ForMobile: cswPrivate.ForMobile
                },
                success: function () {
                    Csw.cookie.set(Csw.cookie.cookieNames.CustomerId, cswPrivate.AccessId);
                    Csw.cookie.set(Csw.cookie.cookieNames.Username, cswPrivate.UserName);
                    Csw.cookie.set(Csw.cookie.cookieNames.LogoutPath, cswPrivate.logoutpath);
                    Csw.tryExec(cswPrivate.onAuthenticate, cswPrivate.UserName);
                },
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
            if (options) {
                $.extend(cswPrivate, options);
            }
            cswPrivate.isAuthenticated = false;
            Csw.ajax.post({
                url: cswPrivate.DeauthenticateUrl,
                data: {},
                success: function () {
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
                success: function () {
                },
                failure: function () {
                },
                usernodeid: '',
                usernodekey: '',
                passwordpropid: '',
                ForMobile: false
            };
            if (options) {
                $.extend(o, options);
            }

            var txt = '';
            var goodEnoughForMobile = false; //Ignore password expirery and license accept for Mobile for now
            switch (o.status) {
                case 'Authenticated':
                    o.success();
                    break;
                case 'Archived':
                    txt = 'Your account is archived. Please see your account administrator.';
                    break;
                case 'Deauthenticated':
                    o.success(); // yes, o.success() is intentional here.
                    break;
                case 'Failed':
                    txt = 'Invalid login.';
                    break;
                case 'Locked':
                    txt = 'Your account is locked.  Please see your account administrator.';
                    break;
                case 'Deactivated':
                    txt = 'Your account is deactivated.  Please see your account administrator.';
                    break;
                case 'ModuleNotEnabled':
                    txt = 'This feature is not enabled.  Please see your account administrator.';
                    break;
                case 'TooManyUsers':
                    txt = 'Too many users are currently connected.  Try again later.';
                    break;
                case 'NonExistentAccessId':
                    txt = 'Invalid login.';
                    break;
                case 'NonExistentSession':
                    txt = 'Your session has timed out.  Please login again.';
                    break;
                case 'Unknown':
                    txt = 'An Unknown Error Occurred';
                    break;
                case 'TimedOut':
                    goodEnoughForMobile = true;
                    txt = 'Your session has timed out.  Please login again.';
                    break;
                case 'ExpiredPassword':
                    goodEnoughForMobile = true;
                    if (!o.ForMobile) {
                        $.CswDialog('EditNodeDialog', {
                            nodeids: [o.usernodeid],
                            nodekeys: [o.usernodekey],
                            filterToPropId: o.passwordpropid,
                            title: 'Your password has expired.  Please change it now:',
                            onEditNode: function () {
                                o.success();
                            }
                        });
                    }
                    break;
                case 'ShowLicense':
                    goodEnoughForMobile = true;
                    if (!o.ForMobile) {
                        $.CswDialog('ShowLicenseDialog', {
                            'onAccept': function () {
                                o.success();
                            },
                            'onDecline': function () {
                                o.failure('You must accept the license agreement to use this application');
                            }
                        });
                    }
                    break;
                case 'Ignore':
                    o.success();
                    break;
            }

            if (o.ForMobile &&
                (o.status !== 'Authenticated' && goodEnoughForMobile)) {
                o.success();
            } else if (false === Csw.isNullOrEmpty(txt) && o.status !== 'Authenticated') {
                o.failure(txt, o.status);
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
                $.extend(o, options);
            }

            Csw.ajax.post({
                url: '/NbtWebApp/wsNBT.asmx/isAdministrator',
                success: function (data) {
                    if (Csw.bool(data.Administrator)) {
                        Csw.tryExec(o.Yes);
                    } else {
                        Csw.tryExec(o.No);
                    }
                }
            });
        });

} ());
