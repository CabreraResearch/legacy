/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {
    'use strict';

    Csw.string = Csw.string ||
        Csw.register('string', function (inputStr, defaultStr) {
            /// <summary> Get a valid string literal based on input</summary>
            /// <param name="inputStr" type="String"> String to parse </param>
            /// <param name="defaultStr" type="String"> Default value if null or empty </param>
            /// <returns type="String">String literal value</returns>

            function tryParseString() {
                /// <summary> Returns the inputStr if !isNullOrEmpty, else returns the defaultStr</summary>
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

            return retObj;

        });

    Csw.delimitedString = Csw.delimitedString ||
        Csw.register('delimitedString', function (string, opts) {
            var cswPrivate = {
                newLineToDelimiter: true,
                spaceToDelimiter: true,
                removeDuplicates: true,
                delimiter: ',',
                string: Csw.string(string)
            };
            if (opts) $.extend(cswPrivate, opts);
            var cswPublic = {
                array: [],
                delimited: '',
                string: {
                    get: function () {
                        return cswPrivate.string;
                    },
                    set: function (str) {
                        cswPrivate.string = str;
                        cswPublic.delimited = cswPrivate.string;
                        if (cswPrivate.newLineToDelimiter) {
                            while (cswPublic.delimited.indexOf('\n') !== -1) {
                                cswPublic.delimited = cswPublic.delimited.replace(/\n/g, cswPrivate.delimiter);
                            }
                        }
                        if (cswPrivate.spaceToDelimiter) {
                            while (cswPublic.delimited.indexOf(' ') !== -1) {
                                cswPublic.delimited = cswPublic.delimited.replace(/ /g, cswPrivate.delimiter);
                            }
                        }
                        while (cswPublic.delimited.indexOf(',,') !== -1) {
                            cswPublic.delimited = cswPublic.delimited.replace(/,,/g, cswPrivate.delimiter);
                        }
                        cswPublic.array = cswPublic.delimited.split(cswPrivate.delimiter);
                        if (cswPrivate.removeDuplicates) {
                            (function () {

                                var unique = function (array) {
                                    var seen = new Set;
                                    return array.filter(function (item) {
                                        if (false === seen.has(item)) {
                                            seen.add(item);
                                            return true;
                                        }
                                    });
                                };

                                cswPublic.array = unique(cswPublic.array);
                            } ());
                        }
                    }
                }
            };
            cswPublic.string.set(cswPrivate.string);
            return cswPublic;
        });

    function isString(obj) {
        /// <summary> Returns true if the object is a String object. </summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = typeof obj === 'string' || Csw.isInstanceOf('string', obj);
        return ret;
    }
    Csw.register('isString', isString);
    Csw.isString = Csw.isString || isString;

    function startsWith(source, search) {
        return (source.substr(0, search.length) === search);
    }
    Csw.register('startsWith', startsWith);
    Csw.startsWith = Csw.startsWith || startsWith;

    function getTimeString(date, timeformat) {
        var militaryTime = false;
        if (false === Csw.isNullOrEmpty(timeformat) && timeformat === 'H:mm:ss') {
            militaryTime = true;
        }

        var ret;
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();

        if (minutes < 10) {
            minutes = '0' + minutes;
        }
        if (seconds < 10) {
            seconds = '0' + seconds;
        }

        if (militaryTime) {
            ret = hours + ':' + minutes + ':' + seconds;
        } else {
            ret = (hours % 12) + ':' + minutes + ':' + seconds + ' ';
            if (hours > 11) {
                ret += 'PM';
            } else {
                ret += 'AM';
            }
        }
        return ret;
    }
    Csw.register('getTimeString', getTimeString);
    Csw.getTimeString = Csw.getTimeString || getTimeString;

} ());
