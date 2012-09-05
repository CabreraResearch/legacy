///// <reference path="~app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswFieldTypeTimeInterval';

//    var methods = {
//        init: function (o) {
//            var propDiv = o.propDiv;
//            propDiv.empty();
//            o.propVals = o.propData.values;

//            if (false === Csw.bool(o.ReadOnly)) {
//                propDiv.timeInterval(o);
//            } else {
//                propDiv.append(o.propData.gestalt);
//            }
//        },
//        save: function (o) {
//            //Case 20939: if your prop isn't saving, check for duplicate IDs
//            var intervalData = Csw.clientDb.getItem(o.ID + '_rateIntervalSave');
//            var $parent = $(this);
//            var parent = Csw.literals.factory($parent);
//            var compare = {};
//            try {
//                var attributes = {
//                    Interval: {
//                        rateintervalvalue: {
//                            ratetype: null,
//                            weeklyday: null,
//                            startingdate: {
//                                date: null,
//                                dateformat: null
//                            },
//                            monthlyfrequency: null,
//                            monthlydate: null,
//                            monthlyweek: null,
//                            startingmonth: null,
//                            startingyear: null,
//                            yearlydate: {
//                                date: null,
//                                dateformat: null
//                            }
//                        }
//                    }
//                };

//                var newInterval = attributes.Interval.rateintervalvalue;
//                if (false === o.Multi || parent.find('#' + o.ID + '_textvalue').text() !== Csw.enums.multiEditDefaultValue) {
//                    Csw.extend(newInterval, intervalData, true);
//                    compare = attributes;
//                }
//                var oldInterval = o.propData.values.Interval.rateintervalvalue;
//                switch (attributes.Interval.rateintervalvalue.ratetype) {
//                    case Csw.enums.rateIntervalTypes.WeeklyByDay:
//                        if (false === Csw.contains(oldInterval, 'startingdate')) {
//                            oldInterval.startingdate = { date: '', dateformat: '' };
//                        }
//                        if (false === Csw.contains(oldInterval, 'weeklyday')) {
//                            oldInterval.weeklyday = '';
//                        }
//                        break;
//                    case Csw.enums.rateIntervalTypes.MonthlyByDate:
//                        if (false === Csw.contains(oldInterval, 'monthlydate')) {
//                            oldInterval.monthlydate = { date: '', dateformat: '' };
//                        }
//                        if (false === Csw.contains(oldInterval, 'monthlyfrequency')) {
//                            oldInterval.monthlyfrequency = '';
//                        }
//                        if (false === Csw.contains(oldInterval, 'startingmonth')) {
//                            oldInterval.startingmonth = '';
//                        }
//                        if (false === Csw.contains(oldInterval, 'startingyear')) {
//                            oldInterval.startingyear = '';
//                        }
//                        break;
//                    case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
//                        if (false === Csw.contains(oldInterval, 'monthlyweek')) {
//                            oldInterval.monthlyweek = '';
//                        }
//                        if (false === Csw.contains(oldInterval, 'monthlyday')) {
//                            oldInterval.monthlyday = '';
//                        }
//                        if (false === Csw.contains(oldInterval, 'monthlyfrequency')) {
//                            oldInterval.monthlyfrequency = '';
//                        }
//                        if (false === Csw.contains(oldInterval, 'startingmonth')) {
//                            oldInterval.startingmonth = '';
//                        }
//                        if (false === Csw.contains(oldInterval, 'startingyear')) {
//                            oldInterval.startingyear = '';
//                        }
//                        break;
//                    case Csw.enums.rateIntervalTypes.YearlyByDate:
//                        if (false === Csw.contains(oldInterval, 'yearlydate')) {
//                            oldInterval.yearlydate = { date: '', dateformat: '' };
//                        }
//                        break;
//                }

//                Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
//            } catch (e) {
//                if (Csw.debug) {
//                    Csw.debug.log('Error updating propData: ' + e);
//                }
//            }
//        } // save
//    };

//    // Method calling logic
//    $.fn.CswFieldTypeTimeInterval = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);
