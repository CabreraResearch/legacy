/*global Csw2:true*/
(function() {

    Csw2.delimitedString = Csw2.delimitedString ||
        Csw2.lift('delimitedString', function (string, opts) {
            var cswInternal = {
                newLineToDelimiter: true,
                spaceToDelimiter: true,
                removeDuplicates: true,
                delimiter: ',',
                initString: Csw2.to.string(string)
            };

            var cswReturn = {
                array: [],
                delimited: function () {
                    return cswReturn.array.join(cswInternal.delimiter);
                },
                string: function (delimiter) {
                    delimiter = delimiter || cswInternal.delimiter;
                    var ret = '';
                    Csw2.each(cswReturn.array, function (val) {
                        if (ret.length > 0) {
                            ret += delimiter;
                        }
                        ret += val;
                    });
                    return ret;
                },
                toString: function () {
                    return cswReturn.string();
                },
                add: function (str) {
                    cswReturn.array.push(cswInternal.parse(str));
                    cswInternal.deleteDuplicates();
                    return cswReturn;
                },
                remove: function (str) {
                    var remove = function (array) {
                        return array.filter(function (item) {
                            if (item !== str) {
                                return true;
                            }
                        });
                    };
                    cswReturn.array = remove(cswReturn.array);
                    return cswReturn;
                },
                count: function() {
                    return cswReturn.array.length;
                },
                contains: function (str, caseSensitive) {
                    var isCaseSensitive = Csw2.to.bool(caseSensitive);
                    str = Csw2.string(str).trim();
                    if (false === isCaseSensitive) {
                        str = str.toLowerCase();
                    }
                    var match = cswReturn.array.filter(function (matStr) {
                        return ((isCaseSensitive && Csw2.to.string(matStr).trim() === str) || Csw2.to.string(matStr).trim().toLowerCase() === str);
                    });
                    return match.length > 0;
                },
                each: function(callBack) {
                    return cswReturn.array.forEach(callBack);
                }
            };

            cswInternal.parse = function (str) {
                var ret = Csw2.to.string(str);

                if (cswInternal.newLineToDelimiter) {
                    while (ret.indexOf('\n') !== -1) {
                        ret = ret.replace(/\n/g, cswInternal.delimiter);
                    }
                }
                if (cswInternal.spaceToDelimiter) {
                    while (ret.indexOf(' ') !== -1) {
                        ret = ret.replace(/ /g, cswInternal.delimiter);
                    }
                }
                while (ret.indexOf(',,') !== -1) {
                    ret = ret.replace(/,,/g, cswInternal.delimiter);
                }
                return ret;
            };

            cswInternal.deleteDuplicates = function () {
                if (cswInternal.removeDuplicates) {
                    (function () {

                        var unique = function (array) {
                            var seen = new Set();
                            return array.filter(function (item) {
                                if (false === seen.has(item)) {
                                    seen.add(item);
                                    return true;
                                }
                            });
                        };

                        cswReturn.array = unique(cswReturn.array);
                    }());
                }
            };

            (function (a) {
                if (a.length > 1 && false === Csw2.is.plainObject(opts)) {
                    Csw2.each(a, function (val) {
                        if (false === Csw2.is.nullOrEmpty(val)) {
                            cswReturn.array.push(val);
                        }
                    });
                } else if(string && string.length > 0) {
                    Csw2.extend(cswInternal, opts);
                    var delimitedString = cswInternal.parse(string);
                    cswInternal.initString = delimitedString;
                    cswReturn.array = delimitedString.split(cswInternal.delimiter);
                }

                cswInternal.deleteDuplicates();
            }(arguments));
            return cswReturn;
        });
}());