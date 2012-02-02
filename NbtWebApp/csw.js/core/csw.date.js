/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    var dateTimeMinValue = new Date('1/1/0001 12:00:00 AM');
    Csw.register('dateTimeMinValue', dateTimeMinValue);
    Csw.dateTimeMinValue = Csw.dateTimeMinValue || dateTimeMinValue;
    
    function date(value) {
        var retDate;
        try {
            retDate = new Date(value);
        } catch(e) {
            retDate = dateTimeMinValue;
        }
        return retDate;
    }
    Csw.register('date', date);
    Csw.date = Csw.date || date;

    function serverDateFormatToJQuery(serverDateFormat) {
        var ret = serverDateFormat;
        ret = ret.replace( /M/g , 'm');
        ret = ret.replace( /mmm/g , 'M');
        ret = ret.replace( /yyyy/g , 'yy');
        return ret;
    }
    Csw.register('serverDateFormatToJQuery', serverDateFormatToJQuery);
    Csw.serverDateFormatToJQuery = Csw.serverDateFormatToJQuery || serverDateFormatToJQuery;
    
    function serverTimeFormatToJQuery(serverTimeFormat) {
        var ret = serverTimeFormat;
        return ret;
    }
    Csw.register('serverTimeFormatToJQuery', serverTimeFormatToJQuery);
    Csw.serverTimeFormatToJQuery = Csw.serverTimeFormatToJQuery || serverTimeFormatToJQuery;

    function isDate(obj) {
        /// <summary> Returns true if the object is a Date</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = (obj instanceof Date);
        return ret;
    }
    Csw.register('isDate', isDate);
    Csw.isDate = Csw.isDate || isDate;

    function validateTime(value) {
        var isValid = true;
        var regex = /^(\d?\d):(\d\d)\s?([APap][Mm])?$/g ;
        var match = regex.exec(value);
        if (match === null) {
            isValid = false;
        } else {
            var hour = Csw.number(match[1]);
            var minute = Csw.number(match[2]);
            if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60) {
                isValid = false;
            }
        }
        return isValid;
    } // validateTime()
    Csw.register('validateTime', validateTime);
    Csw.validateTime = Csw.validateTime || validateTime;

}());