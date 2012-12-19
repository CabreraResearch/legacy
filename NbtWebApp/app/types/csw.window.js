
/// <reference path="~/app/CswApp-vsdoc.js" />

(function CswWindow() {
    'use strict';
    //#region Window

    //Case 24114: IE7 doesn't support web storage
    var newLocalStorage = (function storageClosure() {
        var storage = {};
        var keys = [];
        var length = 0;
        return {
            getItem: function (sKey) {
                var ret = null;
                if (sKey && storage.hasOwnProperty(sKey)) {
                    ret = storage[sKey];
                }
                return ret;
            },
            key: function (nKeyId) {
                var ret = null;
                if (keys.hasOwnProperty(nKeyId)) {
                    ret = keys[nKeyId];
                }
                return ret;
            },
            setItem: function (sKey, sValue) {
                var ret = null;
                if (sKey) {
                    if (false === storage.hasOwnProperty(sKey)) {
                        keys.push(sKey);
                        length += 1;
                    }
                    storage[sKey] = sValue;
                }
                return ret;
            },
            length: length,
            removeItem: function (sKey) {
                var ret = false;
                if (sKey && storage.hasOwnProperty(sKey)) {
                    keys.splice(sKey, 1);
                    length -= 1;
                    delete storage[sKey];
                    ret = true;
                }
                return ret;
            },
            clear: function () {
                storage = {};
                keys = [];
                length = 0;
                return true;
            },
            hasOwnProperty: function (sKey) {
                return storage.hasOwnProperty(sKey);
            }
        };
    } ());

    if (window.Modernizr && window.hasOwnProperty('Modernizr') ) {
        if(window.Modernizr.hasOwnProperty('localstorage') && false === window.Modernizr.localstorage) {
            window.localStorage = newLocalStorage;
        }
        if (window.Modernizr.hasOwnProperty('sessionstorage') && false === window.Modernizr.sessionstorage) {
            window.sessionStorage = newLocalStorage;
        }    
    }
    
    Csw.window.location = Csw.window.location ||
        Csw.window.register('location', function (path) {
            if (false === Csw.isNullOrEmpty(path)) {
                window.location = path;
            }
            return window.location;
        });

    Csw.window.getPath = Csw.window.getPath ||
        Csw.window.register('getPath', function() {
            var path = window.location.href;
            return path.substring(0, path.lastIndexOf("/")) + "/";
        });

    //#endregion Window


} ());

