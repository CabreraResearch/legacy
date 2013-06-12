/*global nameSpace:true*/
(function (nameSpace) {

    nameSpace.lift('delimitedString', function (string, opts) {
        var nsInternal = {
            newLineToDelimiter: true,
            spaceToDelimiter: true,
            removeDuplicates: true,
            delimiter: ',',
            initString: nameSpace.to.string(string)
        };

        var nsRet = {
            array: [],
            delimited: function () {
                return nsRet.array.join(nsInternal.delimiter);
            },
            string: function (delimiter) {
                delimiter = delimiter || nsInternal.delimiter;
                var ret = '';
                nameSpace.each(nsRet.array, function (val) {
                    if (ret.length > 0) {
                        ret += delimiter;
                    }
                    ret += val;
                });
                return ret;
            },
            toString: function () {
                return nsRet.string();
            },
            add: function (str) {
                nsRet.array.push(nsInternal.parse(str));
                nsInternal.deleteDuplicates();
                return nsRet;
            },
            remove: function (str) {
                var remove = function (array) {
                    return array.filter(function (item) {
                        if (item !== str) {
                            return true;
                        }
                    });
                };
                nsRet.array = remove(nsRet.array);
                return nsRet;
            },
            count: function () {
                return nsRet.array.length;
            },
            contains: function (str, caseSensitive) {
                var isCaseSensitive = nameSpace.to.bool(caseSensitive);
                str = nameSpace.string(str).trim();
                if (false === isCaseSensitive) {
                    str = str.toLowerCase();
                }
                var match = nsRet.array.filter(function (matStr) {
                    return ((isCaseSensitive && nameSpace.to.string(matStr).trim() === str) || nameSpace.to.string(matStr).trim().toLowerCase() === str);
                });
                return match.length > 0;
            },
            each: function (callBack) {
                return nsRet.array.forEach(callBack);
            }
        };

        nsInternal.parse = function (str) {
            var ret = nameSpace.to.string(str);

            if (nsInternal.newLineToDelimiter) {
                while (ret.indexOf('\n') !== -1) {
                    ret = ret.replace(/\n/g, nsInternal.delimiter);
                }
            }
            if (nsInternal.spaceToDelimiter) {
                while (ret.indexOf(' ') !== -1) {
                    ret = ret.replace(/ /g, nsInternal.delimiter);
                }
            }
            while (ret.indexOf(',,') !== -1) {
                ret = ret.replace(/,,/g, nsInternal.delimiter);
            }
            return ret;
        };

        nsInternal.deleteDuplicates = function () {
            if (nsInternal.removeDuplicates) {
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

                    nsRet.array = unique(nsRet.array);
                }());
            }
        };

        (function (a) {
            if (a.length > 1 && false === nameSpace.is.plainObject(opts)) {
                nameSpace.each(a, function (val) {
                    if (false === nameSpace.is.nullOrEmpty(val)) {
                        nsRet.array.push(val);
                    }
                });
            } else if (string && string.length > 0) {
                nameSpace.extend(nsInternal, opts);
                var delimitedString = nsInternal.parse(string);
                nsInternal.initString = delimitedString;
                nsRet.array = delimitedString.split(nsInternal.delimiter);
            }

            nsInternal.deleteDuplicates();
        }(arguments));
        return nsRet;
    });
}(window.$om$));