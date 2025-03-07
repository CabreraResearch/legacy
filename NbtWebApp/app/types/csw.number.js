
/// <reference path="~/app/CswApp-vsdoc.js" />

(function _cswNumber() {
    'use strict';

    var int32MinVal = -2147483648;
    Csw.register('int32MinVal', int32MinVal);

    function number(inputNum, defaultNum) {
        /// <summary>
        ///   Returns the inputNum if !NaN, else returns the defaultNum
        /// </summary>
        /// <param name="inputNum" type="Number"> String to parse to number </param>
        /// <param name="defaultNum" type="Number"> Default value if not a number </param>
        /// <returns type="Number" />
        function tryParseNumber() {
            var ret = NaN;

            function makeNumber(value) {
                var num = NaN;
                if (value) {
                    num = +value;
                }
                if (isNaN(num)) {
                    num = parseInt(value, 0);
                }
                return num;
            }
            var tryRet = makeNumber(inputNum);

            if (false === isNaN(tryRet) && tryRet !== int32MinVal) {
                ret = tryRet;
            } else {
                tryRet = makeNumber(defaultNum);
                if (false === isNaN(tryRet) && tryRet !== int32MinVal) {
                    ret = tryRet;
                }
            }
            return ret;
        }

        var retVal = tryParseNumber();

        return retVal;
    }
    Csw.register('number', number);

    function isNumber(obj) {
        /// <summary> Returns true if the object is typeof number</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (typeof obj === 'number');
        return ret;
    }
    Csw.register('isNumber', isNumber);

    var isNumeric = function (obj) {
        /// <summary> Returns true if the input can be parsed as a Number </summary>
        /// <param name="str" type="Object"> String or object to test </param>
        /// <returns type="Boolean" />
        var ret = false;
        if (isNumber(number(obj)) && false === Csw.isNullOrEmpty(obj)) {
            var num = +obj;
            if (false === isNaN(num)) {
                ret = true;
            }
        }
        return ret;
    };
    Csw.register('isNumeric', isNumeric);

    function validateFloatMinValue(value, minvalue, excludeRangeLimits) {
        var nValue = parseFloat(value);
        var nMinValue = parseFloat(minvalue);
        var isValid = true;
        if (excludeRangeLimits === undefined) {
            excludeRangeLimits = false;
        }
        if (nMinValue !== undefined) {
            if (nValue === undefined || excludeRangeLimits ? nValue <= nMinValue : nValue < nMinValue) {
                isValid = false;
            }
        }
        return isValid;
    }
    Csw.register('validateFloatMinValue', validateFloatMinValue);

    function validateFloatMaxValue(value, maxvalue, excludeRangeLimits) {
        var nValue = parseFloat(value);
        var nMaxValue = parseFloat(maxvalue);
        var isValid = true;
        if (excludeRangeLimits === undefined) {
            excludeRangeLimits = false;
        }
        if (nMaxValue !== undefined) {
            if (nValue === undefined || excludeRangeLimits ? nValue >= nMaxValue : nValue > nMaxValue) {
                isValid = false;
            }
        }
        return isValid;
    }
    Csw.register('validateFloatMaxValue', validateFloatMaxValue);

    function validateFloatPrecision(value, precision) {
        var isValid = true;

        var regex;
        if (precision > 0) {
            /* Allow any valid number -- we'll round later */
            regex = /^\-?\d*(\.\d+)?$/g; //Case 28696 - changed regex to NOT accept a '.'
        } else {
            /* Integers Only */
            regex = /^\-?\d*$/g;
        }
        if (isValid && !regex.test(value)) {
            isValid = false;
        }
        return isValid;
    }
    Csw.register('validateFloatPrecision', validateFloatPrecision);

    function validateInteger(value) {
        var regex = /^\-?\d+$/g;
        return (regex.test(value) || value === null);
    }
    Csw.register('validateInteger', validateInteger);

    function validateGreaterThanZero(value) {
        var regex = /^(\d*(\.|)\d*)$/g;
        return ((regex.test(value) && number(value) > 0) || value === null);
    }
    Csw.register('validateGreaterThanZero', validateGreaterThanZero);

    function getMaxValueForPrecision(precision, maxPrecision) {
        var i,
            ret = '',
            precisionMax = maxPrecision || 6;
        if (precision > 0 &&
            precision <= precisionMax) {

            ret += '.';
            for (i = 0; i < precision; i += 1) {
                ret += '9';
            }
        }
        return ret;
    }
    Csw.register('getMaxValueForPrecision', getMaxValueForPrecision);

    //Validates the character length of the string-ified number
    function validateMaxLength(value, maxLength) {
        return (value.length <= maxLength);
    }
    Csw.register('validateMaxLength', validateMaxLength);


}());
