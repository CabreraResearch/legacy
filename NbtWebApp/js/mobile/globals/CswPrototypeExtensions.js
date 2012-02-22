/// <reference path="CswEnums.js" />
/// <reference path="CswGlobalTools.js" />
/// <reference path="../../Scripts/modernizr-2.0.6-development-only.js" />


//#region String

// for IE 8
String.prototype.trim = String.prototype.trim || function() {
    return this.replace( /^\s+|\s+$/g , '');
};

String.prototype.toUpperCaseFirstChar = String.prototype.toUpperCaseFirstChar || function() {
    return this.substr(0, 1).toUpperCase() + this.substr(1);
};

String.prototype.toLowerCaseFirstChar = String.prototype.toLowerCaseFirstChar || function() {
    return this.substr(0, 1).toLowerCase() + this.substr(1);
};

String.prototype.toUpperCaseEachWord = String.prototype.toUpperCaseEachWord || function(delim) {
    delim = delim ? delim : ' ';
    return this.split(delim).map(function(v) { return v.toUpperCaseFirstChar(); }).join(delim);
};

String.prototype.toLowerCaseEachWord = String.prototype.toLowerCaseEachWord || function(delim) {
    delim = delim ? delim : ' ';
    return this.split(delim).map(function(v) { return v.toLowerCaseFirstChar(); }).join(delim);
};

//#endregion String

//#region Array

// Production steps of ECMA-262, Edition 5, 15.4.4.18
// Reference: http://es5.github.com/#x15.4.4.18
if (!Array.prototype.forEach) {
    Array.prototype.forEach = function (callback, thisArg) {
        var T, k;
        if (this == null) {
            throw new TypeError(" this is null or not defined");
        }
        // 1. Let O be the result of calling ToObject passing the |this| value as the argument.
        var O = Object(this);
        // 2. Let lenValue be the result of calling the Get internal method of O with the argument "length".
        // 3. Let len be ToUint32(lenValue).
        var len = O.length >>> 0; // Hack to convert O.length to a UInt32
        // 4. If IsCallable(callback) is false, throw a TypeError exception.
        // See: http://es5.github.com/#x9.11
        if ({}.toString.call(callback) != "[object Function]") {
            throw new TypeError(callback + " is not a function");
        }
        // 5. If thisArg was supplied, let T be thisArg; else let T be undefined.
        if (thisArg) {
            T = thisArg;
        }
        // 6. Let k be 0
        k = 0;
        // 7. Repeat, while k < len
        while (k < len) {
            var kValue;
            // a. Let Pk be ToString(k).
            //   This is implicit for LHS operands of the in operator
            // b. Let kPresent be the result of calling the HasProperty internal method of O with argument Pk.
            //   This step can be combined with c
            // c. If kPresent is true, then
            if (k in O) {
                // i. Let kValue be the result of calling the Get internal method of O with argument Pk.
                kValue = O[k];
                // ii. Call the Call internal method of callback with T as the this value and
                // argument list containing kValue, k, and O.
                callback.call(T, kValue, k, O);
            }
            // d. Increase k by 1.
            k++;
        }
        // 8. return undefined
    };
}

if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function(searchElement /*, fromIndex */) {
        "use strict";
        if (this === void 0 || this === null) {
            throw new TypeError();
        }
        var t = Object(this);
        var len = t.length >>> 0;
        if (len === 0) {
            return -1;
        }
        var n = 0;
        if (arguments.length > 0) {
            n = Number(arguments[1]);
            if (n !== n) { // shortcut for verifying if it's NaN
                n = 0;
            } else if (n !== 0 && n !== Infinity && n !== -Infinity) {
                n = (n > 0 || -1) * Math.floor(Math.abs(n));
            }
        }
        if (n >= len) {
            return -1;
        }
        var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
        for (; k < len; k++) {
            if (k in t && t[k] === searchElement) {
                return k;
            }
        }
        return -1;
    };
}

//#endregion Array

//#region Function

Function.prototype.inheritsFrom = function(parentClassOrObject) {
    if (parentClassOrObject.constructor === Function) {
        //Normal Inheritance 
        this.prototype = new parentClassOrObject;
        this.prototype.constructor = this;
        this.prototype.parent = parentClassOrObject.prototype;
    } else {
        //Pure Virtual Inheritance 
        this.prototype = parentClassOrObject;
        this.prototype.constructor = this;
        this.prototype.parent = parentClassOrObject;
    }
    return this;
};

//#endregion Function

//#region Object

//#endregion Object

//#region Window

//Case 24114: IE7 doesn't support web storage
if (false === Modernizr.localstorage) {
    window.localStorage = (function () {
        var storage = {};
        var keys = [];
        var length = 0;
        return {
            getItem: function (sKey) {
                var ret = null;
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
                    ret = storage[sKey];
                }
                return ret;
            },
            key: function (nKeyId) {
                var ret = null;
                if (contains(keys, nKeyId)) {
                    ret = keys[nKeyId];
                }
                return ret;
            },
            setItem: function (sKey, sValue) {
                var ret = null;
                if (false === isNullOrEmpty(sKey)) {
                    if (false === contains(storage, sKey)) {
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
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
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
                return contains(storage, sKey);
            }
        };
    } ());
}
if (false === Modernizr.sessionstorage) {
    //https://developer.mozilla.org/en/DOM/Storage
//    window.sessionStorage = {
//        getItem: function (sKey) {
//            if (isNullOrEmpty(sKey) || false === contains(this, sKey)) { return null; }
//            return unescape(document.cookie.replace(new RegExp("(?:^|.*;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=\\s*((?:[^;](?!;))*[^;]?).*"), "$1"));
//        },
//        key: function (nKeyId) { return unescape(document.cookie.replace(/\s*\=(?:.(?!;))*$/, "").split(/\s*\=(?:[^;](?!;))*[^;]?;\s*/)[nKeyId]); },
//        setItem: function (sKey, sValue) {
//            if (isNullOrEmpty(sKey)) { return; }
//            var now = new Date();
//            var soon = now.setTime(now.getTime() + 90000);
//            var expires = "; expires=" + soon.toGMTString();
//            document.cookie = escape(sKey) + "=" + escape(sValue) + expires + "; path=/";
//            this.length = document.cookie.match(/\=/g).length;
//        },
//        length: 0,
//        removeItem: function (sKey) {
//            if (isNullOrEmpty(sKey) || false === contains(this, sKey)) { return; }
//            var sExpDate = new Date();
//            sExpDate.setDate(sExpDate.getDate() - 1);
//            document.cookie = escape(sKey) + "=; expires=" + sExpDate.toGMTString() + "; path=/";
//            this.length--;
//        },
//        clear: function () { },
//        hasOwnProperty: function (sKey) { return (new RegExp("(?:^|;\\s*)" + escape(sKey).replace(/[\-\.\+\*]/g, "\\$&") + "\\s*\\=")).test(document.cookie); }
//    };
    //    window.sessionStorage.length = (document.cookie.match(/\=/g) || window.sessionStorage).length;
    window.sessionStorage = (function () {
        var storage = {};
        var keys = [];
        var length = 0;
        return {
            getItem: function (sKey) {
                var ret = null;
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
                    ret = storage[sKey];
                }
                return ret;
            },
            key: function (nKeyId) {
                var ret = null;
                if (contains(keys, nKeyId)) {
                    ret = keys[nKeyId];
                }
                return ret;
            },
            setItem: function (sKey, sValue) {
                var ret = null;
                if (false === isNullOrEmpty(sKey)) {
                    if (false === contains(storage, sKey)) {
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
                if (false === isNullOrEmpty(sKey) && contains(storage, sKey)) {
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
                return contains(storage, sKey);
            }
        };
    } ());
}

//#endregion Window