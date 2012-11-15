
/// <reference path="~/app/CswApp-vsdoc.js" />

(function _cswCASNo() {
    'use strict';

    var validateCASNo = function (CASNo) {
        /// <summary> Returns true if the input string matches the CASNo regex </summary>
        /// <param name="CASNo"> the string to check </param>
        /// <returns type="Boolean" />
        var casNoPattern = new RegExp(/^\d{1,7}-\d{2}-\d$/);
        return casNoPattern.test(CASNo);
    }
    Csw.register('validateCASNo', validateCASNo);
    Csw.validateCASNo = Csw.validateCASNo || validateCASNo;

    var checkSumCASNo = function (CASNo) {
        /// <summary> Returns true if the input string has a valid checksum </summary>
        /// <param name="CASNo"> the string to check </param>
        /// <returns type="Boolean" />

        var numbers = CASNo.match(/\d/g);
        var sum = 0;
        var i;
        for (i = 0; i < numbers.length - 1; i++) {
            sum += (numbers.length - 1 - i) * numbers[i];
        }

        return (sum % 10) == numbers[numbers.length - 1];
    }
    Csw.register('checkSumCASNo', checkSumCASNo);
    Csw.checkSumCASNo = Csw.checkSumCASNo || checkSumCASNo;

} ());
