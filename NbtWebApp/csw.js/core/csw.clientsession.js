/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswClientSession() {
    'use strict';
    
    var expiretime = '';
    var expiretimeInterval;
    var expiredInterval;

    function getExpireTime() {
        return expiretime;
    }
    Csw.register('getExpireTime', getExpireTime);
    Csw.getExpireTime = Csw.getExpireTime || getExpireTime;
    
    function setExpireTime(value) {
        expiretime = value;
        setExpireTimeInterval ();
    }
    Csw.register('setExpireTime', setExpireTime);
    Csw.setExpireTime = Csw.setExpireTime || setExpireTime;

    function setExpireTimeInterval() {
        clearInterval (expiretimeInterval);
        clearInterval (expiredInterval);
        expiretimeInterval = setInterval (function () {
            checkExpireTime ();
        }, 60000);
        expiredInterval = setInterval (function () {
            checkExpired ();
        }, 60000);
    }
    Csw.register('setExpireTimeInterval', setExpireTimeInterval);
    Csw.setExpireTimeInterval = Csw.setExpireTimeInterval || setExpireTimeInterval;

    function checkExpired() {
        var now = new Date ();
        if (Date.parse (expiretime) - Date.parse (now) < 0) {
            clearInterval (expiredInterval);
            logout ();
        }
    }
    Csw.register('setExpireTimeInterval', setExpireTimeInterval);
    Csw.setExpireTimeInterval = Csw.setExpireTimeInterval || setExpireTimeInterval;

    function checkExpireTime() {
        var now = new Date ();
        if (Date.parse (expiretime) - Date.parse (now) < 180000)     	// 3 minutes until timeout
        {
            clearInterval (expiretimeInterval);
            $.CswDialog ('ExpireDialog', {
                'onYes': function () {
                    Csw.ajax ({
                        'url': '/NbtWebApp/wsNBT.asmx/RenewSession',
                        'success': function () {
                        }
                    });
                }
            });
        }
    }
    Csw.register('checkExpireTime', checkExpireTime);
    Csw.checkExpireTime = Csw.checkExpireTime || checkExpireTime;

    function handleAuthenticationStatus(options) {
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
    } // _handleAuthenticationStatus()
    Csw.register('handleAuthenticationStatus', handleAuthenticationStatus);
    Csw.handleAuthenticationStatus = Csw.handleAuthenticationStatus || handleAuthenticationStatus;
    
    function logout(options) {
        var o = {
            DeauthenticateUrl: '/NbtWebApp/wsNBT.asmx/deauthenticate',
            onDeauthenticate: function () {
            }
        };

        if (options) {
            $.extend(o, options);
        }

        Csw.ajax({
            url: o.DeauthenticateUrl,
            data: {},
            success: function () { //data
                finishLogout();
                o.onDeauthenticate();
            } // success{}
        });
    } // logout
    Csw.register('logout', logout, true);
    Csw.logout = Csw.logout || logout;

    function finishLogout() {
        var logoutpath = $.CswCookie('get', CswCookieName.LogoutPath);
        $.CswCookie('clearAll');
        if (false === Csw.isNullOrEmpty(logoutpath)) {
            window.location = logoutpath;
        } else {
            window.location = Csw.getGlobalProp('homeUrl');
        }
    }
    Csw.register('finishLogout', finishLogout, true);
    Csw.finishLogout = Csw.finishLogout || finishLogout;

    function isAdministrator(options) {
        var o = {
            'Yes': function () {
            },
            'No': function () {
            }
        };
        if (options) {
            $.extend(o, options);
        }

        Csw.ajax({
            url: '/NbtWebApp/wsNBT.asmx/isAdministrator',
            success: function (data) {
                if (data.Administrator === "true") {
                    o.Yes();
                } else {
                    o.No();
                }
            }
        });
    } // IsAdministrator()
    Csw.register('isAdministrator', isAdministrator);
    Csw.isAdministrator = Csw.isAdministrator || isAdministrator;
    
}());