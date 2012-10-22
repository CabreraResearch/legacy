
/// <reference path="~/app/CswApp-vsdoc.js" />

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
            /// <summary>
            /// Parses a string into an array using the specified delimiter.
            /// </summary>
            /// <param name="string"></param>
            /// <param name="opts"></param>
            /// <returns type="delimitedString">A delimitedString object</returns>
            var cswPrivate = {
                newLineToDelimiter: true,
                spaceToDelimiter: true,
                removeDuplicates: true,
                delimiter: ',',
                initString: Csw.string(string)
            };

            var cswPublic = {
                array: [],
                delimited: function () {
                    /// <summary>
                    /// returns the delimited string as an Array
                    /// </summary>
                    return cswPublic.array.join(cswPrivate.delimiter);
                },
                string: function (delimiter) {
                    /// <summary>
                    /// returns the delimited string as a String
                    /// </summary>
                    delimiter = delimiter || cswPrivate.delimiter;
                    var ret = '';
                    Csw.each(cswPublic.array, function (val) {
                        if (ret.length > 0) {
                            ret += delimiter;
                        }
                        ret += val;
                    });
                    return ret;
                },
                toString: function () {
                    return cswPublic.string();
                },
                add: function (str) {
                    /// <summary>
                    /// Adds an item to the delimited string and returns the delimitedString object
                    /// </summary>
                    cswPublic.array.push(cswPrivate.parse(str));
                    cswPrivate.deleteDuplicates();
                    return cswPublic;
                },
                contains: function (str, caseSensitive) {
                    /// <summary>
                    /// True if the delimited string contains the provided string
                    /// </summary>
                    var isCaseSensitive = Csw.bool(caseSensitive);
                    str = Csw.string(str).trim();
                    if (false === isCaseSensitive) {
                        str = str.toLowerCase();
                    }
                    var match = cswPublic.array.filter(function (matStr) {
                        return ((isCaseSensitive && Csw.string(matStr).trim() === str) || Csw.string(matStr).trim().toLowerCase() === str);
                    });
                    return match.length > 0;
                }
            };

            cswPrivate.parse = function (str) {
                var ret = Csw.string(str);

                if (cswPrivate.newLineToDelimiter) {
                    while (ret.indexOf('\n') !== -1) {
                        ret = ret.replace(/\n/g, cswPrivate.delimiter);
                    }
                }
                if (cswPrivate.spaceToDelimiter) {
                    while (ret.indexOf(' ') !== -1) {
                        ret = ret.replace(/ /g, cswPrivate.delimiter);
                    }
                }
                while (ret.indexOf(',,') !== -1) {
                    ret = ret.replace(/,,/g, cswPrivate.delimiter);
                }
                return ret;
            };

            cswPrivate.deleteDuplicates = function () {
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
                    }());
                }
            };

            (function (a) { //ctor
                if (a.length > 1 && false === Csw.isPlainObject(opts)) {
                    Csw.each(a, function (val) {
                        if (false === Csw.isNullOrEmpty(val)) {
                            cswPublic.array.push(val);
                        }
                    });
                }
                else if (opts) {
                    Csw.extend(cswPrivate, opts);
                    var delimitedString = cswPrivate.parse(string);
                    cswPrivate.initString = delimitedString;
                    cswPublic.array = delimitedString.split(cswPrivate.delimiter);
                }

                cswPrivate.deleteDuplicates();
            }(arguments));
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

}());
