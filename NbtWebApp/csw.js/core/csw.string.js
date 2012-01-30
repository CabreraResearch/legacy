/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    //#region Prototype Extension

    // for IE 8
    String.prototype.trim = String.prototype.trim || function () {
        return this.replace( /^\s+|\s+$/g , '');
    };

    String.prototype.toUpperCaseFirstChar = String.prototype.toUpperCaseFirstChar || function () {
        return this.substr(0, 1).toUpperCase() + this.substr(1);
    };

    String.prototype.toLowerCaseFirstChar = String.prototype.toLowerCaseFirstChar || function () {
        return this.substr(0, 1).toLowerCase() + this.substr(1);
    };

    String.prototype.toUpperCaseEachWord = String.prototype.toUpperCaseEachWord || function (delim) {
        delim = delim || ' ';
        return this.split(delim).map(function (v) { return v.toUpperCaseFirstChar(); }).join(delim);
    };

    String.prototype.toLowerCaseEachWord = String.prototype.toLowerCaseEachWord || function (delim) {
        delim = delim || ' ';
        return this.split(delim).map(function (v) { return v.toLowerCaseFirstChar(); }).join(delim);
    };

    //#endregion Prototype Extension

    function string(inputStr, defaultStr) {

        function tryParseString() {
            /// <summary>
            ///   Returns the inputStr if !isNullOrEmpty, else returns the defaultStr
            /// </summary>
            /// <param name="inputStr" type="String"> String to parse </param>
            /// <param name="defaultStr" type="String"> Default value if null or empty </param>
            /// <returns type="String" />
            var ret = '';
            if (false === Csw.isPlainObject(inputStr) &&
                false === Csw.isFunction(inputStr) &&
                    false === Csw.isNullOrEmpty(inputStr)) {
                ret = inputStr.toString();
            } else if (false === Csw.isPlainObject(defaultStr) &&
                false === Csw.isFunction(defaultStr) &&
                    false === Csw.isNullOrEmpty(defaultStr)) {
                ret = defaultStr.toString();
            }
            return ret;
        }

        var retObj = tryParseString();

        retObj.val = function () {
            return inputStr;
        };

        retObj.trim = function () {
            inputStr = $.trim(inputStr);
            return inputStr;
        };

        retObj.contains = function (valTest) {
            return inputStr.indexOf(valTest) !== -1;
        };
        
        return retObj;

    }
    Csw.register('string', string);
    Csw.string = Csw.string || string;
    
    function isString(obj) {
        /// <summary> Returns true if the object is a String object. </summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = typeof obj === 'string' || Csw.isInstanceOf('string', obj);
        return ret;
    }
    Csw.register('isString', isString);
    Csw.isString = Csw.isString || isString;
    
    function trim(str) {
        /// <summary>Returns a string without left and right whitespace</summary>
        /// <param name="str" type="String"> String to parse </param>
        /// <returns type="String">Parsed string</returns>
        return $.trim(str);
    }
    Csw.register('trim', trim);
    Csw.trim = window.Csw.trim || trim;
    
    function startsWith(source, search) {
        return (source.substr(0, search.length) === search);
    }
    Csw.register('startsWith', startsWith);
    Csw.startsWith = Csw.startsWith || startsWith;
    
    function getTimeString(date, timeformat) {
        var militaryTime = false;
        if (false === Csw.isNullOrEmpty(timeformat) && timeformat === "H:mm:ss") {
            militaryTime = true;
        }

        var ret;
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();

        if (minutes < 10) {
            minutes = "0" + minutes;
        }
        if (seconds < 10) {
            seconds = "0" + seconds;
        }

        if (militaryTime) {
            ret = hours + ":" + minutes + ":" + seconds;
        } else {
            ret = (hours % 12) + ":" + minutes + ":" + seconds + " ";
            if (hours > 11) {
                ret += "PM";
            } else {
                ret += "AM";
            }
        }
        return ret;
    };
    Csw.register('getTimeString', getTimeString);
    Csw.getTimeString = Csw.getTimeString || getTimeString;

}());