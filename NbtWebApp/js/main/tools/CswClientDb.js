/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
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
    var storedInMemory = {};
    var keys = [];
    
    var serializer = JSON;
    var serialize = serializer.stringify;
    var deserialize = $.parseJSON;
    var hasLocalStorage = (Modernizr.localstorage && false === isNullOrEmpty(window.localStorage));
    var hasSessionStorage = (Modernizr.sessionstorage && false === isNullOrEmpty(window.sessionStorage));
    
    //priveleged, public
    this.clear = function () {
        //nuke the entire storage collection
        if (hasLocalStorage) {
            localStorage.clear();
        }
        if (hasSessionStorage) {
            sessionStorage.clear();
        }
        storedInMemory = {};
        return this;
    };

    this.getItem = function(key) {
        var ret = '';
        if (false === isNullOrEmpty(key)) {
            var value = tryParseString(localStorage.getItem(key), '');
            if (isNullOrEmpty(value) || value === 'undefined') {
                value = tryParseString(sessionStorage.getItem(key), '');
            }
            if (isNullOrEmpty(value) || value === 'undefined') {
                value = tryParseString( storedInMemory[key], '');
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
    };

    this.getKeys = function () {
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
            if (storedInMemory.length > 0) {
                for (memKey in storedInMemory) {
                    keys.push(memKey);
                }
            }
        }
        return keys;
    };

    this.hasKey = function(key) {
        var ret = contains(this.getKeys(), key);
        return ret;
    };

    this.removeItem = function(key) {
        localStorage.removeItem(key);
        sessionStorage.removeItem(key);
        delete storedInMemory[key];
        delete keys[key];
    };

    this.setItem = function (key, value) {
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
            }
            catch (locErr) {
                try {
//                    if (debugOn()) {
//                        log('localStorage failed:' + locErr);
//                    }
                    localStorage.removeItem(key);
                    sessionStorage.setItem(key, val);
                } catch (ssnErr) {
//                    if (debugOn()) {
//                        log('sessionStorage failed:' + ssnErr);
//                    }
                    try {
                        sessionStorage.removeItem(key);
                        storedInMemory[key] = value;
                    }
                    catch (memErr) {
//                        if (debugOn()) {
//                            log('memory storage failed:' + memErr);
//                        }
                        ret = false;
                    }
                }
            }
        }
        return ret;
    };
}

//#endregion CswClientDb
