
/// <reference path="~app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    Csw.dateTimeMinValue = Csw.dateTimeMinValue ||
        Csw.register('dateTimeMinValue', new Date('1/1/0001 12:00:00 AM'));

    Csw.date = Csw.date ||
        Csw.register('date', function (value) {
            //        var retDate;
            //        try {
            //            retDate = new Date(value);
            //        } catch (e) {
            //            retDate = dateTimeMinValue;
            //        }
            //        return retDate;

            var cswPrivate = {
                dateFormat: 'mm/dd/yyyy',   // need to derive from current user eventually
                timeFormat: 'hh:MM:ss',   // need to derive from current user eventually
                secondms: 1000,
                minutems: (1000 * 60),
                hourms: (1000 * 60 * 60),
                dayms: (1000 * 60 * 60 * 24),
                yearms: (1000 * 60 * 60 * 24 * 365)   // leap year will throw this off by a day
            };

            var cswPublic = {};

            cswPublic.value = Csw.number(value); // converts a Date() to milliseconds
            if (isNaN(cswPublic.value) || cswPublic.value <= 0) {
                cswPublic.value = Csw.number(new Date()); //default to today
            }

            cswPublic.addSeconds = function (s) {
                cswPublic.value += s * cswPrivate.secondms;
                return cswPublic; // chaining
            };
            cswPublic.addMinutes = function (m) {
                cswPublic.value += m * cswPrivate.minutems;
                return cswPublic; // chaining
            };
            cswPublic.addHours = function (h) {
                cswPublic.value += h * cswPrivate.hourms;
                return cswPublic; // chaining
            };
            cswPublic.addDays = function (d) {
                cswPublic.value += d * cswPrivate.dayms;
                return cswPublic; // chaining
            };
            cswPublic.addYears = function (y) {
                cswPublic.value += y * cswPrivate.yearms;
                return cswPublic; // chaining
            };
            cswPublic.toDate = function () {
                return new Date(cswPublic.value);
            };


            // ***********************************************************************************
            // BEGIN EXTERNAL CODE

            /*
            * Date Format 1.2.3
            * (c) 2007-2009 Steven Levithan <stevenlevithan.com>
            * MIT license
            *
            * Includes enhancements by Scott Trenda <scott.trenda.net>
            * and Kris Kowal <cixar.com/~kris.kowal/>
            *
            * Accepts a date, a mask, or a date and a mask.
            * Returns a formatted version of the given date.
            * The date defaults to the current date/time.
            * The mask defaults to dateFormat.masks.default.
            */


            var dateFormat = (function () {
                var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
                    timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
                    timezoneClip = /[^-+\dA-Z]/g,
                    pad = function (val, len) {
                        val = String(val);
                        len = len || 2;
                        while (val.length < len) val = "0" + val;
                        return val;
                    };

                // Regexes and supporting functions are cached through closure
                return function (date, mask, utc) {
                    var dF = dateFormat;

                    // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
                    if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
                        mask = date;
                        date = undefined;
                    }

                    // Passing date through Date applies Date.parse, if necessary
                    date = date ? new Date(date) : new Date;
                    if (isNaN(date)) throw SyntaxError("invalid date");

                    mask = String(dF.masks[mask] || mask || dF.masks["default"]);

                    // Allow setting the utc argument via the mask
                    if (mask.slice(0, 4) == "UTC:") {
                        mask = mask.slice(4);
                        utc = true;
                    }

                    var _ = utc ? "getUTC" : "get",
                        d = date[_ + "Date"](),
                        D = date[_ + "Day"](),
                        m = date[_ + "Month"](),
                        y = date[_ + "FullYear"](),
                        H = date[_ + "Hours"](),
                        M = date[_ + "Minutes"](),
                        s = date[_ + "Seconds"](),
                        L = date[_ + "Milliseconds"](),
                        o = utc ? 0 : date.getTimezoneOffset(),
                        flags = {
                            d: d,
                            dd: pad(d),
                            ddd: dF.i18n.dayNames[D],
                            dddd: dF.i18n.dayNames[D + 7],
                            m: m + 1,
                            mm: pad(m + 1),
                            mmm: dF.i18n.monthNames[m],
                            mmmm: dF.i18n.monthNames[m + 12],
                            yy: String(y).slice(2),
                            yyyy: y,
                            h: H % 12 || 12,
                            hh: pad(H % 12 || 12),
                            H: H,
                            HH: pad(H),
                            M: M,
                            MM: pad(M),
                            s: s,
                            ss: pad(s),
                            l: pad(L, 3),
                            L: pad(L > 99 ? Math.round(L / 10) : L),
                            t: H < 12 ? "a" : "p",
                            tt: H < 12 ? "am" : "pm",
                            T: H < 12 ? "A" : "P",
                            TT: H < 12 ? "AM" : "PM",
                            Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                            o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
                            S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
                        };

                    return mask.replace(token, function ($0) {
                        return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
                    });
                };
            } ());

            // Some common format strings
            dateFormat.masks = {
                "default": "ddd mmm dd yyyy HH:MM:ss",
                shortDate: "m/d/yy",
                mediumDate: "mmm d, yyyy",
                longDate: "mmmm d, yyyy",
                fullDate: "dddd, mmmm d, yyyy",
                shortTime: "h:MM TT",
                mediumTime: "h:MM:ss TT",
                longTime: "h:MM:ss TT Z",
                isoDate: "yyyy-mm-dd",
                isoTime: "HH:MM:ss",
                isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
                isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
            };

            // Internationalization strings
            dateFormat.i18n = {
                dayNames: [
                    "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
                    "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
                ],
                monthNames: [
                    "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
                    "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
                ]
            };

            //        // For convenience...
            //        Date.prototype.format = function (mask, utc) {
            //            return dateFormat(this, mask, utc);
            //        };

            // END EXTERNAL CODE
            // ***********************************************************************************

            cswPublic.format = function (mask, utc) {
                return dateFormat(new Date(cswPublic.value), mask, utc);
            };

            cswPublic.toString = function () {
                return cswPublic.format(cswPrivate.dateFormat + ' ' + cswPrivate.timeFormat);
            };
            return cswPublic;
        });


    Csw.serverDateFormatToJQuery = Csw.serverDateFormatToJQuery ||
        Csw.register('serverDateFormatToJQuery', function (serverDateFormat) {
            var ret = serverDateFormat;
            ret = ret.replace(/M/g, 'm');
            ret = ret.replace(/mmm/g, 'M');
            ret = ret.replace(/yyyy/g, 'yy');
            return ret;
        });

    Csw.serverTimeFormatToJQuery = Csw.serverTimeFormatToJQuery ||
        Csw.register('serverTimeFormatToJQuery', function (serverTimeFormat) {
            var ret = serverTimeFormat;
            return ret;
        });

    Csw.isDate = Csw.isDate ||
        Csw.register('isDate', function (obj) {
            /// <summary> Returns true if the object is a Date</summary>
            /// <param name="obj" type="Object"> Object to test</param>
            /// <returns type="Boolean" />
            var ret = (obj instanceof Date);
            return ret;
        });

    Csw.validateTime = Csw.validateTime ||
        Csw.register('validateTime', function (value) {
            var isValid = true;
            var regex = /^(\d?\d):(\d\d)\s?([APap][Mm])?$/g;
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
        });

        Csw.todayAsString = Csw.todayAsString ||
        Csw.register('todayAsString', function () {
            var today = new Date();
            return (today.getMonth() + 1) + '/' + today.getDate() + '/' + today.getFullYear();
        });

    Csw.getDateFromDnJson = Csw.getDateFromDnJson ||
        Csw.register('getDateFromDnJson', function (dnDate) {
            /// <summary> Transforms a .NET JSON date into a JavaScript date.</summary>
            /// <param name="obj" type="Object"> Object to test</param>
            /// <returns type="Boolean" />
            /*
            var milli = Csw.number(DnDate.replace(/\/Date\((\d+)\-?(\d+)\)\//, '$1'));
            var offset = Csw.number(DnDate.replace(/\/Date\(\d+([\+\-]?\d+)\)\//, '$1'));
            var localOffset = new Date().getTimezoneOffset();
            return new Date((milli - ((localOffset + (offset / 100 * 60)) * 1000)));
            */
            /* Dn Date will look like /Date(1335758400000-0400)/  */
            var dnDateStr = Csw.string(dnDate);
            var ret, ticks, offset, localOffset, arr;
            ret = Csw.dateTimeMinValue;
            if (false === Csw.isNullOrEmpty(dnDateStr)) {
                dnDateStr = dnDateStr.replace('/', '');
                dnDateStr = dnDateStr.replace('Date', '');
                dnDateStr = dnDateStr.replace('(', '');
                dnDateStr = dnDateStr.replace(')', '');
                arr = dnDateStr.split('-');
                if (arr.length > 1) {
                    ticks = Csw.number(arr[0]);
                    offset = Csw.number(arr[1]);
                    localOffset = new Date().getTimezoneOffset();
                    ret = new Date((ticks - ((localOffset + (offset / 100 * 60)) * 1000)));
                } else if (arr.length === 1) {
                    ticks = Csw.number(arr[0]);
                    ret = new Date(ticks);
                }
            }
            return ret;
        });


} ());










