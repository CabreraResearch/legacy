/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';
    /// <summary>
    ///   Client db class to encapsulate get/set/update and delete methods against the localStorage object.
    /// </summary>
    /// <returns type="CswClientDb">Instance of itself. Must instance with 'new' keyword.</returns>

    var cswPrivateVar = {
        keys: [],
        deserialize: $.parseJSON,
        serializer: JSON,
        hasLocalStorage: (window.Modernizr.localstorage),
        hasSessionStorage: (window.Modernizr.sessionstorage)
    };
    cswPrivateVar.serialize = cswPrivateVar.serializer.stringify;

    cswPrivateVar.closureStorage = (function () {
        var storage = {},
            keys = [],
            length = 0;
        var clientDbP = {};
        clientDbP.getItem = function (sKey) {
            var ret = null;
            if (sKey && storage.hasOwnProperty(sKey)) {
                ret = storage[sKey];
            }
            return ret;
        };
        clientDbP.key = function (nKeyId) {
            var ret = null;
            if (keys.hasOwnProperty(nKeyId)) {
                ret = keys[nKeyId];
            }
            return ret;
        };
        clientDbP.setItem = function (sKey, sValue) {
            var ret = null;
            if (sKey) {
                if (false === storage.hasOwnProperty(sKey)) {
                    keys.push(sKey);
                    length += 1;
                }
                storage[sKey] = sValue;
            }
            return ret;
        };
        clientDbP.length = length;
        clientDbP.removeItem = function (sKey) {
            var ret = false;
            if (sKey && storage.hasOwnProperty(sKey)) {
                keys.splice(sKey, 1);
                length -= 1;
                delete storage[sKey];
                ret = true;
            }
            return ret;
        };
        clientDbP.clear = function () {
            storage = {};
            keys = [];
            length = 0;
            return true;
        };
        clientDbP.hasOwnProperty = function (sKey) {
            return storage.hasOwnProperty(sKey);
        };

        return clientDbP;
    } ());

    Csw.hasWebStorage = Csw.hasWebStorage ||
        Csw.register('hasWebStorage', function () {
            var ret = (window.Modernizr.localstorage || window.Modernizr.sessionstorage);
            return ret;
        });

    Csw.clientDb = Csw.clientDb ||
        Csw.register('clientDb', Csw.makeNameSpace());


    Csw.clientDb.clear = Csw.clientDb.clear ||
        Csw.clientDb.register('clear', function () {
            //nuke the entire storage collection
            if (cswPrivateVar.hasLocalStorage) {
                window.localStorage.clear();
            }
            if (cswPrivateVar.hasSessionStorage) {
                window.sessionStorage.clear();
            }
            cswPrivateVar.closureStorage.clear();
            return this;
        });

    Csw.clientDb.getItem = Csw.clientDb.getItem ||
        Csw.clientDb.register('getItem', function (key) {
            var ret = '';
            if (false === Csw.isNullOrEmpty(key)) {
                var value = Csw.string(window.localStorage.getItem(key));
                if (Csw.isNullOrEmpty(value) || value === 'undefined') {
                    value = Csw.string(window.sessionStorage.getItem(key));
                }
                if (Csw.isNullOrEmpty(value) || value === 'undefined') {
                    value = Csw.string(cswPrivateVar.closureStorage.getItem(key));
                }
                if (!Csw.isNullOrEmpty(value) && value !== 'undefined') {
                    try {
                        ret = cswPrivateVar.deserialize(value);
                    } catch (e) {
                        ret = value;
                    }
                }
            }
            return ret;
        });

    Csw.clientDb.getKeys = Csw.clientDb.getKeys ||
        Csw.clientDb.register('getKeys', function () {
            var locKey, sesKey, memKey;
            if (Csw.isNullOrEmpty(cswPrivateVar.keys) && window.localStorage.length > 0) {
                for (locKey in window.localStorage) {
                    cswPrivateVar.keys.push(locKey);
                }
                if (window.sessionStorage.length > 0) {
                    for (sesKey in window.sessionStorage) {
                        cswPrivateVar.keys.push(sesKey);
                    }
                }
                if (cswPrivateVar.closureStorage.length > 0) {
                    for (memKey in cswPrivateVar.closureStorage.keys) {
                        cswPrivateVar.keys.push(memKey);
                    }
                }
            }
            return cswPrivateVar.keys;
        });

    Csw.clientDb.hasKey = Csw.clientDb.hasKey ||
        Csw.clientDb.register('hasKey', function (key) {
            var ret = Csw.contains(this.getKeys(), key);
            return ret;
        });

    Csw.clientDb.removeItem = Csw.clientDb.removeItem ||
        Csw.clientDb.register('removeItem', function (key) {
            window.localStorage.removeItem(key);
            window.sessionStorage.removeItem(key);
            cswPrivateVar.closureStorage.removeItem(key);
            delete cswPrivateVar.keys[key];
        });

    Csw.clientDb.setItem = Csw.clientDb.setItem ||
        Csw.clientDb.register('setItem', function (key, value) {
            /// <summary>
            ///   Stores a key/value pair in localStorage. 
            ///   If localStorage is full, use sessionStorage. 
            ///   if sessionStorage is full, store in memory.
            /// </summary>
            /// <param name="key" type="String">The property name to store.</param>
            /// <param name="value" type="String">The property value to store. If not a string, serializer will be called.</param>
            /// <returns type="Boolean">True if successful</returns>
            var ret = true;
            if (false === Csw.isNullOrEmpty(key)) {
                if (false === this.hasKey(key)) {
                    cswPrivateVar.keys.push(key);
                }
                var val = (typeof value === 'object') ? cswPrivateVar.serialize(value) : value;

                // if localStorage is full, we should fail gracefully into sessionStorage, then memory
                try {
                    window.localStorage.setItem(key, val);
                } catch (locErr) {
                    try {
                        window.localStorage.removeItem(key);
                        window.sessionStorage.setItem(key, val);
                    } catch (ssnErr) {
                        try {
                            window.sessionStorage.removeItem(key);
                            cswPrivateVar.closureStorage.setItem(key, value);
                        } catch (memErr) {
                            ret = false;
                        }
                    }
                }
            }
            return ret;
        });


} ());
