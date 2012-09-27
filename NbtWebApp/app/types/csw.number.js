
/// <reference path="~/app/CswApp-vsdoc.js" />

(function _cswNumber() {
    'use strict';

    var int32MinVal = -2147483648;
    Csw.register('int32MinVal', int32MinVal);
    Csw.int32MinVal = Csw.int32MinVal || int32MinVal;

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
    Csw.number = Csw.number || number;

    function isNumber(obj) {
        /// <summary> Returns true if the object is typeof number</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (typeof obj === 'number');
        return ret;
    }
    Csw.register('isNumber', isNumber);
    Csw.isNumber = Csw.isNumber || isNumber;

    var isNumeric = function (obj) {
        /// <summary> Returns true if the input can be parsed as a Number </summary>
        /// <param name="str" type="Object"> String or object to test </param>
        /// <returns type="Boolean" />
        var ret = false;
        if (isNumber(obj) && false === Csw.isNullOrEmpty(obj)) {
            var num = +obj;
            if (false === isNaN(num)) {
                ret = true;
            }
        }
        return ret;
    };
    Csw.register('isNumeric', isNumeric);
    Csw.isNumeric = Csw.isNumeric || isNumeric;

    function validateFloatMinValue(value, minvalue, isOpenSet) {
        var nValue = parseFloat(value);
        var nMinValue = parseFloat(minvalue);
        var isValid = true;
        if (isOpenSet === undefined) {
            isOpenSet = false;
        }
        if (nMinValue !== undefined) {
            if (nValue === undefined || isOpenSet ? nValue <= nMinValue : nValue < nMinValue) {
                isValid = false;
            }
        }
        return isValid;
    }
    Csw.register('validateFloatMinValue', validateFloatMinValue);
    Csw.validateFloatMinValue = Csw.validateFloatMinValue || validateFloatMinValue;

    function validateFloatMaxValue(value, maxvalue, isOpenSet) {
        var nValue = parseFloat(value);
        var nMaxValue = parseFloat(maxvalue);
        var isValid = true;
        if (isOpenSet === undefined) {
            isOpenSet = false;
        }
        if (nMaxValue !== undefined) {
            if (nValue === undefined || isOpenSet ? nValue >= nMaxValue : nValue > nMaxValue) {
                isValid = false;
            }
        }
        return isValid;
    }
    Csw.register('validateFloatMaxValue', validateFloatMaxValue);
    Csw.validateFloatMaxValue = Csw.validateFloatMaxValue || validateFloatMaxValue;

    function validateFloatPrecision(value, precision) {
        var isValid = true;

        var regex;
        if (precision > 0) {
            /* Allow any valid number -- we'll round later */
            regex = /^\-?\d*\.?\d*$/g;
        } else {
            /* Integers Only */
            regex = /^\-?\d*$/g;
        }
        if (isValid && !regex.test(value)) {
            isValid = false;
        }
        if (value === Csw.enums.multiEditDefaultValue) {
            isValid = true;
        }
        return isValid;
    }
    Csw.register('validateFloatPrecision', validateFloatPrecision);
    Csw.validateFloatPrecision = Csw.validateFloatPrecision || validateFloatPrecision;

    function validateInteger(value) {
        var regex = /^\-?\d*$/g;
        return (regex.test(value));
    }
    Csw.register('validateInteger', validateInteger);
    Csw.validateInteger = Csw.validateInteger || validateInteger;

    function validateIntegerGreaterThanZero(value) {
        var regex = /^(\d*(\.|)\d*)$/g;
        return (regex.test(value));
    }
    Csw.register('validateIntegerGreaterThanZero', validateIntegerGreaterThanZero);
    Csw.validateIntegerGreaterThanZero = Csw.validateIntegerGreaterThanZero || validateIntegerGreaterThanZero;

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
    Csw.getMaxValueForPrecision = Csw.getMaxValueForPrecision || getMaxValueForPrecision;

    function validateMaxLength(value, maxLength) {
        return (value.length <= maxLength);
    }
    Csw.register('validateMaxLength', validateMaxLength);
    Csw.validateMaxLength = Csw.validateMaxLength || validateMaxLength;

} ());
