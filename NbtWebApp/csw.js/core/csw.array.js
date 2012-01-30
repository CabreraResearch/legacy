/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    //#region Prototype Extension

        // Production steps of ECMA-262, Edition 5, 15.4.4.18
        // Reference: http://es5.github.com/#x15.4.4.18
        if (!Array.prototype.forEach) {
            Array.prototype.forEach = function (callback, thisArg) {
                var t = '', k;
                if (this == null) {
                    throw new TypeError(" this is null or not defined");
                }
                // 1. Let O be the result of calling ToObject passing the |this| value as the argument.
                var o = Object(this);
                // 2. Let lenValue be the result of calling the Get internal method of O with the argument "length".
                // 3. Let len be ToUint32(lenValue).
                var len = o.length >>> 0; // Hack to convert O.length to a UInt32
                // 4. If IsCallable(callback) is false, throw a TypeError exception.
                // See: http://es5.github.com/#x9.11
                if ({ }.toString.call(callback) != "[object Function]") {
                    throw new TypeError(callback + " is not a function");
                }
                // 5. If thisArg was supplied, let T be thisArg; else let T be undefined.
                if (thisArg) {
                    t = thisArg;
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
                    if (k in o) {
                        // i. Let kValue be the result of calling the Get internal method of O with argument Pk.
                        kValue = o[k];
                        // ii. Call the Call internal method of callback with T as the this value and
                        // argument list containing kValue, k, and O.
                        callback.call(t, kValue, k, o);
                    }
                    // d. Increase k by 1.
                    k++;
                }
                // 8. return undefined
            };
        }

        if (!Array.prototype.indexOf) {
            Array.prototype.indexOf = function (searchElement /*, fromIndex */) {
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

    //#endregion Prototype Extension

    function isArray(obj) {
        /// <summary> Returns true if the object is an array</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = ($.isArray(obj));
        return ret;
    };
    Csw.register('isArray', isArray);
    Csw.isArray = Csw.isArray || isArray;

    function array() {
        var retArray = []; 
        if(arguments.length > 0) {
            retArray = Array.prototype.slice.call(arguments, 0);
        }
        
        retArray.contains = retArray.contains || function (value) {
            return retArray.indexOf(value) != -1;
        };
        
        return retArray;
    };
    Csw.register('array', array);
    Csw.array = Csw.array || array;
    
    function makeSequentialArray(start, end) {
        var ret = array(),
            i = +start;
        end = +end;
        if (Csw.isNumber(start) &&
            Csw.isNumber(end)) {
            for ( ; i <= end; i += 1) {
                ret.push(i);
            }
        }
        return ret;
    };    
    Csw.register('makeSequentialArray', makeSequentialArray);
    Csw.makeSequentialArray = Csw.makeSequentialArray || makeSequentialArray;

}());