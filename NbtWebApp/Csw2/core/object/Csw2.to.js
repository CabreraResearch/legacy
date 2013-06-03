/*global Csw2:true,window:true,Number:true*/
(function (_$) {

    Csw2.to.lift('bool', function bool(str) {
        var retBool = Csw2.is['true'](str);
        if (retBool === false || retBool !== true) {
            retBool = false;
        }
        return retBool;
    });

    Csw2.to.lift('ES5_ToBool', function (val) {
        return (val !== false && val !== 0 && val !== '' && val !== null && val !== undefined && (typeof val !== 'number' || !isNaN(val)));
    });

    Csw2.to.lift('dateFromTicks', function (tickStr) {
        var ticsDateTime = Csw2.string(tickStr);
        var ret, ticks, offset, localOffset, arr;

        if (false === Csw2.is.nullOrEmpty(ticsDateTime)) {
            ticsDateTime = ticsDateTime.replace('/', '');
            ticsDateTime = ticsDateTime.replace('Date', '');
            ticsDateTime = ticsDateTime.replace('(', '');
            ticsDateTime = ticsDateTime.replace(')', '');
            arr = ticsDateTime.split('-');
            if (arr.length > 1) {
                ticks = Csw2.number(arr[0]);
                offset = Csw2.number(arr[1]);
                localOffset = new Date().getTimezoneOffset();
                ret = new Date((ticks - ((localOffset + (offset / 100 * 60)) * 1000)));
            }
            else if (arr.length === 1) {
                ticks = Csw2.number(arr[0]);
                ret = new Date(ticks);
            }
        }
        return ret;
    });

    Csw2.to.lift('binary', function (obj) {
        var ret = NaN;
        if (obj === 0 || obj === '0' || obj === '' || obj === false || Csw2.to.string(obj).toLowerCase().trim() === 'false') {
            ret = 0;
        }
        else if (obj === 1 || obj === '1' || obj === true || Csw2.to.string(obj).toLowerCase().trim() === 'true') {
            ret = 1;
        }
        return ret;
    });

    /**
     *   Attempts to converts an arbitrary value to a Number.
     *   Loose falsy values are converted to 0.
     *   Loose truthy values are converted to 1.
     *   All other values are parsed as Integers.
     *   Failures return as NaN.
     *
     */
    Csw2.to.lift('number', function (inputNum, defaultNum) {
        'use strict';

        function tryGetNumber(val) {
            var ret = NaN;
            if (Csw2.is.number(val)) {
                ret = val;
            }
            else if (Csw2.is.string(val) || Csw2.is.bool(val)) {

                var tryGet = (function (value) {
                    var num = Csw2.to.binary(value);
                    if (!Csw2.is.number(num) && value) {
                        num = +value;
                    }
                    if (!Csw2.is.number(num)) {
                        num = parseInt(value, 0);
                    }
                    return num;
                }(val));

                if (Csw2.is.number(tryGet)) {
                    ret = tryGet;
                }
            }
            return ret;
        }

        var retVal = tryGetNumber(inputNum);
        if (!Csw2.is.number(retVal)) {
            retVal = tryGetNumber(defaultNum);
            if (!Csw2.is.number(retVal)) {
                retVal = Number.NaN;
            }
        }
        return retVal;
    });

    Csw2.to.lift('string', function (inputStr, defaultStr) {
        function tryGetString(str) {
            var ret;
            if (Csw2.is.string(str)) {
                ret = str;
            }
            else {
                ret = '';
                if (Csw2.is.bool(str) || Csw2.is.number(str) || Csw2.is.date(str)) {
                    ret = str.toString();
                }
            }
            return ret;
        }

        var ret1 = tryGetString(inputStr);
        var ret2 = tryGetString(defaultStr);
        var retVal = '';
        if (ret1.length !== 0) {
            retVal = ret1;
        }
        else if (ret1 === ret2 || ret2.length === 0) {
            retVal = ret1;
        }
        else {
            retVal = ret2;
        }

        return retVal;
    });

    Csw2.to.lift('vendorDomObject', function (id) {
        var ret = null;
        var _$el = _$('#' + id);
        if (_$el) {
            ret = _$el;
        }
        return ret;
    });

    Csw2.to.lift('vendorDomObjFromString', function (html) {
        var ret = null;
        var _$el = _$(html);
        if (_$el) {
            ret = _$el;
        }
        return ret;
    });

}(Csw2['?']));