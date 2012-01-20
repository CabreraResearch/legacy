/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswPrototypeExtensions.js" />

//#region CswClientDb
function CswClientDb() {
    
    /// <summary>
    ///   Client db class to encapsulate get/set/update and delete methods against the localStorage object.
    /// </summary>
    /// <returns type="CswClientDb">Instance of itself. Must instance with 'new' keyword.</returns>
    
    //private
    var memoryStorage = (function() {
        var storage = { };
        var memKeys = [];
        var length = 0;
        return {
            getItem: function(sKey) {
                var ret = null;
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
                    ret = storage[sKey];
                }
                return ret;
            },
            key: function(nKeyId) {
                var ret = null;
                if (contains(memKeys, nKeyId)) {
                    ret = memKeys[nKeyId];
                }
                return ret;
            },
            setItem: function(sKey, sValue) {
                var ret = null;
                if (false === isNullOrEmpty(sKey)) {
                    if (false === contains(storage, sKey)) {
                        memKeys.push(sKey);
                        length += 1;
                    }
                    storage[sKey] = sValue;
                }
                return ret;
            },
            length: length,
            removeItem: function(sKey) {
                var ret = false;
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
                    memKeys.splice(sKey, 1);
                    length -= 1;
                    delete storage[sKey];
                    ret = true;
                }
                return ret;
            },
            clear: function() {
                storage = { };
                memKeys = [];
                length = 0;
                return true;
            },
            keys: memKeys,
            valueOf: memKeys,
            hasOwnProperty: function(sKey) {
                return contains(storage, sKey);
            }
        };
    }());

    var keys = [];
    var serializer = JSON;
    var serialize = serializer.stringify;
    var deserialize = $.parseJSON;
    var hasLocalStorage = (window.Modernizr.localstorage && false === isNullOrEmpty(window.localStorage));
    var hasSessionStorage = (window.Modernizr.sessionstorage && false === isNullOrEmpty(window.sessionStorage));
    
    //priveleged, public

    return {
        clear: function() {
            //nuke the entire storage collection
            if (hasLocalStorage) {
                localStorage.clear();
            }
            if (hasSessionStorage) {
                sessionStorage.clear();
            }
            memoryStorage.clear();
            return this;
        },

        getItem: function(key) {
            var ret = '';
            if (false === isNullOrEmpty(key)) {
                var value = tryParseString(localStorage.getItem(key));
                if (isNullOrEmpty(value) || value === 'undefined') {
                    value = tryParseString(sessionStorage.getItem(key));
                }
                if (isNullOrEmpty(value) || value === 'undefined') {
                    value = tryParseString(memoryStorage.getItem(key));
                }
                if (!isNullOrEmpty(value) && value !== 'undefined') {
                    try {
                        ret = deserialize(value);
                    } catch(e) {
                        ret = value;
                    }
                }
            }
            return ret;
        },

        getKeys: function() {
            var locKey, sesKey, memKey;
            if (isNullOrEmpty(keys) && localStorage.length > 0) {
                for (locKey in localStorage) {
                    keys.push(locKey);
                }
                if (sessionStorage.length > 0) {
                    for (sesKey in sessionStorage) {
                        keys.push(sesKey);
                    }
                }
                if (memoryStorage.length > 0) {
                    for (memKey in memoryStorage.keys) keys.push(memKey);
                }
            }
            return keys;
        },

        hasKey: function(key) {
            var ret = contains(this.getKeys(), key);
            return ret;
        },

        removeItem: function(key) {
            localStorage.removeItem(key);
            sessionStorage.removeItem(key);
            memoryStorage.removeItem(key);
            delete keys[key];
        },

        setItem: function(key, value) {
            /// <summary>
            ///   Stores a key/value pair in localStorage. 
            ///   If localStorage is full, use sessionStorage. 
            ///   if sessionStorage is full, store in memory.
            /// </summary>
            /// <param name="key" type="String">The property name to store.</param>
            /// <param name="value" type="String">The property value to store. If not a string, serializer will be called.</param>
            /// <returns type="Boolean">True if successful</returns>
            var ret = true;
            if (false === isNullOrEmpty(key)) {
                if (false === this.hasKey(key)) {
                    keys.push(key);
                }
                var val = (typeof value === 'object') ? serialize(value) : value;

                // if localStorage is full, we should fail gracefully into sessionStorage, then memory
                try {
                    localStorage.setItem(key, val);
                } catch(locErr) {
                    try {
                        localStorage.removeItem(key);
                        sessionStorage.setItem(key, val);
                    } catch(ssnErr) {
                        try {
                            sessionStorage.removeItem(key);
                            memoryStorage.setItem(key, value);
                        } catch(memErr) {
                            ret = false;
                        }
                    }
                }
            }
            return ret;
        }
    };
}

//#endregion CswClientDb
