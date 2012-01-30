/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function CswNumber() {
    'use strict';

    var int32MinVal = -2147483648;
    Csw.register('int32MinVal', int32MinVal);
    Csw.int32MinVal = Csw.int32MinVal || int32MinVal;

    function number(inputNum, defaultNum) {
        function tryParseNumber() {
            /// <summary>
            ///   Returns the inputNum if !NaN, else returns the defaultNum
            /// </summary>
            /// <param name="inputNum" type="Number"> String to parse to number </param>
            /// <param name="defaultNum" type="Number"> Default value if not a number </param>
            /// <returns type="Number" />
            var ret = 0;
            var tryRet = +inputNum;

            if (false === isNaN(tryRet) && tryRet !== int32MinVal) {
                ret = tryRet;
            } else {
                tryRet = +defaultNum;
                if (false === isNaN(tryRet) && tryRet !== int32MinVal) {
                    ret = tryRet;
                }
            }
            return ret;
        }

        var retVal = tryParseNumber();

        return retVal;
    };
    Csw.register('number', number);
    Csw.number = Csw.number || number;
    
    function isNumber(obj) {
        /// <summary> Returns true if the object is typeof number</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (typeof obj === 'number');
        return ret;
    };
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

    function validateFloatMinValue(value, minvalue) {
        var nValue = parseFloat(value);
        var nMinValue = parseFloat(minvalue);
        var isValid = true;

        if (nMinValue !== undefined) {
            if (nValue === undefined || nValue < nMinValue) {
                isValid = false;
            }
        }
        return isValid;
    } // validateFloatMinValue()

    function validateFloatMaxValue(value, maxvalue) {
        var nValue = parseFloat(value);
        var nMaxValue = parseFloat(maxvalue);
        var isValid = true;

        if (nMaxValue !== undefined) {
            if (nValue === undefined || nValue > nMaxValue) {
                isValid = false;
            }
        }
        return isValid;
    } // validateFloatMaxValue()

    function validateFloatPrecision(value, precision) {
        var isValid = true;

        var regex;
        if (precision > 0) {
            // Allow any valid number -- we'll round later
            regex = /^\-?\d*\.?\d*$/g ;
        } else {
            // Integers Only
            regex = /^\-?\d*$/g ;
        }
        if (isValid && !regex.test(value)) {
            isValid = false;
        }

        return isValid;
    } // validateFloatPrecision()

    function validateInteger(value) {
        // Integers Only
        var regex = /^\-?\d*$/g ;
        return (regex.test(value));
    } // validateInteger()
    

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

}());