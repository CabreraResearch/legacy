/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientSession() {
    'use strict';
    var cswCookie = Csw.cookie();

    var _expiretime = '';
    var _expiretimeInterval;
    var _expiredInterval;

    function clientSession() {

        function _setExpireTimeInterval () {
            clearInterval(_expiretimeInterval);
            clearInterval(_expiredInterval);
            _expiretimeInterval = setInterval(function () {
                _checkExpireTime();
            }, 60000);
            _expiredInterval = setInterval(function () {
                _checkExpired();
            }, 60000);
        }

        function _checkExpired () {
            var now = new Date();
            if (Date.parse(_expiretime) - Date.parse(now) < 0) {
                clearInterval(_expiredInterval);
                logout();
            }
        }

        function _checkExpireTime () {
            var now = new Date();
            if (Date.parse(_expiretime) - Date.parse(now) < 180000)     	// 3 minutes until timeout
            {
                clearInterval(_expiretimeInterval);
                $.CswDialog('ExpireDialog', {
                    'onYes': function () {
                        Csw.ajax.post({
                            'url': '/NbtWebApp/wsNBT.asmx/RenewSession',
                            'success': function () {
                            }
                        });
                    }
                });
            }
        }

        function getExpireTime () {
            return _expiretime;
        }
        
        function setExpireTime (value) {
            _expiretime = value;
            _setExpireTimeInterval();
        }
        
        function handleAuthenticationStatus (options) {
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
                case 'Deauthenticated':
                    o.success(); // yes, o.success() is intentional here.
                    break;
                case 'Failed':
                    txt = "Invalid login.";
                    break;
                case 'Locked':
                    txt = "Your account is locked.  Please see your account administrator.";
                    break;
                case 'Deactivated':
                    txt = "Your account is deactivated.  Please see your account administrator.";
                    break;
                case 'ModuleNotEnabled':
                    txt = "This feature is not enabled.  Please see your account administrator.";
                    break;
                case 'TooManyUsers':
                    txt = "Too many users are currently connected.  Try again later.";
                    break;
                case 'NonExistentAccessId':
                    txt = "Invalid login.";
                    break;
                case 'NonExistentSession':
                    txt = "Your session has timed out.  Please login again.";
                    break;
                case 'Unknown':
                    txt = "An Unknown Error Occurred";
                    break;
                case 'TimedOut':
                    goodEnoughForMobile = true;
                    txt = "Your session has timed out.  Please login again.";
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
            }

            if (o.ForMobile &&
                (o.status !== 'Authenticated' && goodEnoughForMobile)) {
                o.success();
            } else if (false === Csw.isNullOrEmpty(txt) && o.status !== 'Authenticated') {
                o.failure(txt, o.status);
            }
        }

        function logout (options) {
            var o = {
                DeauthenticateUrl: '/NbtWebApp/wsNBT.asmx/deauthenticate',
                onDeauthenticate: function () {
                }
            };

            if (options) {
                $.extend(o, options);
            }

            Csw.ajax.post({
                url: o.DeauthenticateUrl,
                data: {},
                success: function () { 
                    finishLogout();
                    o.onDeauthenticate();
                } 
            });
        }

        function finishLogout () {
            var logoutpath = cswCookie.get(cswCookie.cookieNames.LogoutPath);
            cswCookie.clearAll();
            if (false === Csw.isNullOrEmpty(logoutpath)) {
                window.location = logoutpath;
            } else {
                window.location = Csw.getGlobalProp('homeUrl');
            }
        }

        function isAdministrator (options) {
            var o = {
                'Yes': function () {
                },
                'No': function () {
                }
            };
            if (options) {
                $.extend(o, options);
            }

            cswAjax.post({
                url: '/NbtWebApp/wsNBT.asmx/isAdministrator',
                success: function (data) {
                    if (data.Administrator === "true") {
                        o.Yes();
                    } else {
                        o.No();
                    }
                }
            });
        }

        return {
            isAdministrator: isAdministrator,
            finishLogout: finishLogout,
            logout: logout,
            handleAuthenticationStatus: handleAuthenticationStatus,
            getExpireTime: getExpireTime,
            setExpireTime: setExpireTime
        };

    }
    Csw.register('clientSession', clientSession);
    Csw.clientSession = Csw.clientSession || clientSession;

}());