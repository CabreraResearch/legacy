/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />
/// <reference path="../controls/CswTimeInterval.js" />
/// <reference path="../tools/CswClientDb.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeTimeInterval';

    var methods = {
        init: function (o) {
            var $Div = $(this);
            o.propVals = o.propData.values;
            o.$parent = $Div;
            var $interval = CswTimeInterval(o);
        },
        save: function (o) {
            var clientDb = new CswClientDb();
            //Case 20939: if your prop isn't saving, check for duplicate IDs
            var intervalData = clientDb.getItem(o.ID + '_rateIntervalSave');
            var $this = $(this);
            
            try {
                var attributes = {
                    Interval: {
                        rateintervalvalue: {
                            ratetype: null,
                            weeklyday: null,
                            startingdate: {
                                date: null,
                                dateformat: null
                            },
                            monthlyfrequency: null,
                            monthlydate: null,
                            monthlyweek: null,
                            startingmonth: null,
                            startingyear: null,
                            yearlydate: {
                                date: null,
                                dateformat: null
                            }
                        }
                    }
                };
                
                var newInterval = attributes.Interval.rateintervalvalue;
                if (false === o.Multi || $this.find('#' + o.ID + '_textvalue').text() !== CswMultiEditDefaultValue) {
                    $.extend(true, newInterval, intervalData);
                }
                var oldInterval = o.propData.values.Interval.rateintervalvalue;
                switch (attributes.Interval.rateintervalvalue.ratetype) {
                    case CswRateIntervalTypes.WeeklyByDay:
                        if(false === contains(oldInterval, 'startingdate')) {
                            oldInterval.startingdate = { date: '', dateformat: '' };
                        }
                        if(false === contains(oldInterval, 'weeklyday')) {
                            oldInterval.weeklyday = '';
                        }
                        break;
                    case CswRateIntervalTypes.MonthlyByDate:
                        if(false === contains(oldInterval, 'monthlydate')) {
                            oldInterval.monthlydate = { date: '', dateformat: '' };
                        }
                        if(false === contains(oldInterval, 'monthlyfrequency')) {
                            oldInterval.monthlyfrequency = '';
                        }
                        if(false === contains(oldInterval,  'startingmonth')) {
                            oldInterval.startingmonth = '';
                        }
                        if(false === contains(oldInterval, 'startingyear')) {
                            oldInterval.startingyear = '';
                        }
                        break;
                    case CswRateIntervalTypes.MonthlyByWeekAndDay:
                        if(false === contains(oldInterval, 'monthlyweek')) {
                            oldInterval.monthlyweek = '';
                        }
                        if(false === contains(oldInterval, 'monthlyday')) {
                            oldInterval.monthlyday = '';
                        }
                        if(false === contains(oldInterval, 'monthlyfrequency')) {
                            oldInterval.monthlyfrequency = '';
                        }
                        if(false === contains(oldInterval,  'startingmonth')) {
                            oldInterval.startingmonth = '';
                        }
                        if(false === contains(oldInterval, 'startingyear')) {
                            oldInterval.startingyear = '';
                        }
                        break;
                    case CswRateIntervalTypes.YearlyByDate:
                        if(false === contains(oldInterval, 'yearlydate')) {
                            oldInterval.yearlydate = { date: '', dateformat: ''};
                        }
                        break;
                }
                
                preparePropJsonForSave(o.Multi, o.propData, attributes);
            } catch (e) {
                if (debugOn()) {
                    log('Error updating propData: ' + e);
                }
            }
        } // save
    };

    // Method calling logic
    $.fn.CswFieldTypeTimeInterval = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };
})(jQuery);
