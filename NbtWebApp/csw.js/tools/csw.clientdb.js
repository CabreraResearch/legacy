/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function cswClientDb () {
    'use strict';
    var clientDb = (function () {


        /// <summary>
        ///   Client db class to encapsulate get/set/update and delete methods against the localStorage object.
        /// </summary>
        /// <returns type="CswClientDb">Instance of itself. Must instance with 'new' keyword.</returns>
    
        //private
        var memoryStorage = (function storageClosure() {
            var storage = { },
                keys = [],
                length = 0;
            var externalP = {};
            externalP.getItem = function (sKey) {
                var ret = null;
                if (sKey && storage.hasOwnProperty(sKey)) {
                    ret = storage[sKey];
                }
                return ret;
            };
            externalP.key = function (nKeyId) {
                var ret = null;
                if (keys.hasOwnProperty(nKeyId)) {
                    ret = keys[nKeyId];
                }
                return ret;
            };
            externalP.setItem = function (sKey, sValue) {
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
            externalP.length = length;
            externalP.removeItem = function (sKey) {
                var ret = false;
                if (sKey && storage.hasOwnProperty(sKey)) {
                    keys.splice(sKey, 1);
                    length -= 1;
                    delete storage[sKey];
                    ret = true;
                }
                return ret;
            };
            externalP.clear = function () {
                storage = {};
                keys = [];
                length = 0;
                return true;
            };
            externalP.hasOwnProperty = function (sKey) {
                return storage.hasOwnProperty(sKey);
            };
            
            return externalP;
        }());

        var keys = [],
            serializer = JSON,
            serialize = serializer.stringify,
            deserialize = $.parseJSON,
            hasLocalStorage = (window.Modernizr.localstorage && false === Csw.isNullOrEmpty(window.localStorage)),
            hasSessionStorage = (window.Modernizr.sessionstorage && false === Csw.isNullOrEmpty(window.sessionStorage));

        var external = {};
        external.clear = function () {
            //nuke the entire storage collection
            if (hasLocalStorage) {
                window.localStorage.clear();
            }
            if (hasSessionStorage) {
                window.sessionStorage.clear();
            }
            memoryStorage.clear();
            return this;
        };
        external.getItem = function (key) {
            var ret = '';
            if (false === Csw.isNullOrEmpty(key)) {
                var value = Csw.string(window.localStorage.getItem(key));
                if (Csw.isNullOrEmpty(value) || value === 'undefined') {
                    value = Csw.string(window.sessionStorage.getItem(key));
                }
                if (Csw.isNullOrEmpty(value) || value === 'undefined') {
                    value = Csw.string(memoryStorage.getItem(key));
                }
                if (!Csw.isNullOrEmpty(value) && value !== 'undefined') {
                    try {
                        ret = deserialize(value);
                    } catch (e) {
                        ret = value;
                    }
                }
            }
            return ret;
        };
        external.getKeys = function () {
            var locKey, sesKey, memKey;
            if (Csw.isNullOrEmpty(keys) && localStorage.length > 0) {
                for (locKey in window.localStorage) {
                    keys.push(locKey);
                }
                if (sessionStorage.length > 0) {
                    for (sesKey in window.sessionStorage) {
                        keys.push(sesKey);
                    }
                }
                if (memoryStorage.length > 0) {
                    for (memKey in memoryStorage.keys) {
                        keys.push(memKey);
                    }
                }
            }
            return keys;
        };
        external.hasKey = function (key) {
            var ret = Csw.contains(this.getKeys(), key);
            return ret;
        };
        external.removeItem = function (key) {
            window.localStorage.removeItem(key);
            window.sessionStorage.removeItem(key);
            memoryStorage.removeItem(key);
            delete keys[key];
        };
        external.setItem = function (key, value) {
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
                    keys.push(key);
                }
                var val = (typeof value === 'object') ? serialize(value) : value;

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
                            memoryStorage.setItem(key, value);
                        } catch (memErr) {
                            ret = false;
                        }
                    }
                }
            }
            return ret;
        };

        return external;

    }());
    Csw.register('clientDb', clientDb);
    
}());
