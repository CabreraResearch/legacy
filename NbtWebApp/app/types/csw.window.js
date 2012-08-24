
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

    if (false === window.Modernizr.localstorage) {
        window.localStorage = newLocalStorage;
    }
    if (false === window.Modernizr.sessionstorage) {
        window.sessionStorage = newLocalStorage;
    }

    Csw.window = Csw.window || Csw.register('window', Csw.makeNameSpace());

    Csw.window.location = Csw.window.location ||
        Csw.window.register('location', function (path) {
            if (false === Csw.isNullOrEmpty(path)) {
                window.location = path;
            }
            return window.location;
        });

    //#endregion Window


} ());

