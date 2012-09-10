/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.timeInterval = Csw.properties.timeInterval ||
        Csw.properties.register('timeInterval',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    if (false === Csw.bool(cswPublic.data.ReadOnly)) {

                        cswPublic.control = cswPrivate.parent.timeInterval({
                            propVals: cswPrivate.propVals,
                            Multi: cswPublic.data.Multi,
                            ReadOnly: cswPublic.data.ReadOnly,
                            Required: cswPublic.data.Required,
                            rateType: cswPublic.data.ratetype,
                            dateFormat: cswPublic.data.dateformat,
                            onChange: function () {
                                if(cswPublic.control) {
                                var val = cswPublic.control.val();
                                var compare = {};

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
                                if (false === cswPublic.data.Multi || parent.find('#' + cswPublic.data.ID + '_textvalue').text() !== Csw.enums.multiEditDefaultValue) {
                                    Csw.extend(newInterval, val, true);
                                    compare = attributes;
                                }
                                var oldInterval = cswPublic.data.propData.values.Interval.rateintervalvalue;
                                switch (attributes.Interval.rateintervalvalue.ratetype) {
                                    case Csw.enums.rateIntervalTypes.WeeklyByDay:
                                        if (false === Csw.contains(oldInterval, 'startingdate')) {
                                            oldInterval.startingdate = { date: '', dateformat: '' };
                                        }
                                        if (false === Csw.contains(oldInterval, 'weeklyday')) {
                                            oldInterval.weeklyday = '';
                                        }
                                        break;
                                    case Csw.enums.rateIntervalTypes.MonthlyByDate:
                                        if (false === Csw.contains(oldInterval, 'monthlydate')) {
                                            oldInterval.monthlydate = { date: '', dateformat: '' };
                                        }
                                        if (false === Csw.contains(oldInterval, 'monthlyfrequency')) {
                                            oldInterval.monthlyfrequency = '';
                                        }
                                        if (false === Csw.contains(oldInterval, 'startingmonth')) {
                                            oldInterval.startingmonth = '';
                                        }
                                        if (false === Csw.contains(oldInterval, 'startingyear')) {
                                            oldInterval.startingyear = '';
                                        }
                                        break;
                                    case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                                        if (false === Csw.contains(oldInterval, 'monthlyweek')) {
                                            oldInterval.monthlyweek = '';
                                        }
                                        if (false === Csw.contains(oldInterval, 'monthlyday')) {
                                            oldInterval.monthlyday = '';
                                        }
                                        if (false === Csw.contains(oldInterval, 'monthlyfrequency')) {
                                            oldInterval.monthlyfrequency = '';
                                        }
                                        if (false === Csw.contains(oldInterval, 'startingmonth')) {
                                            oldInterval.startingmonth = '';
                                        }
                                        if (false === Csw.contains(oldInterval, 'startingyear')) {
                                            oldInterval.startingyear = '';
                                        }
                                        break;
                                    case Csw.enums.rateIntervalTypes.YearlyByDate:
                                        if (false === Csw.contains(oldInterval, 'yearlydate')) {
                                            oldInterval.yearlydate = { date: '', dateformat: '' };
                                        }
                                        break;
                                }
                                cswPublic.data.onPropChange(compare);
                                }
                            }
                        });
                    } else {
                        cswPublic.control = cswPrivate.parent.append(cswPublic.data.propData.gestalt);
                    }
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());

/*

/Case 20939: if your prop isn't saving, check for duplicate IDs
            var intervalData = Csw.clientDb.getItem(o.ID + '_rateIntervalSave');
            var $parent = $(this);
            var parent = Csw.literals.factory($parent);
            var compare = {};
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
                if (false === o.Multi || parent.find('#' + o.ID + '_textvalue').text() !== Csw.enums.multiEditDefaultValue) {
                    Csw.extend(newInterval, intervalData, true);
                    compare = attributes;
                }
                var oldInterval = o.propData.values.Interval.rateintervalvalue;
                switch (attributes.Interval.rateintervalvalue.ratetype) {
                    case Csw.enums.rateIntervalTypes.WeeklyByDay:
                        if (false === Csw.contains(oldInterval, 'startingdate')) {
                            oldInterval.startingdate = { date: '', dateformat: '' };
                        }
                        if (false === Csw.contains(oldInterval, 'weeklyday')) {
                            oldInterval.weeklyday = '';
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByDate:
                        if (false === Csw.contains(oldInterval, 'monthlydate')) {
                            oldInterval.monthlydate = { date: '', dateformat: '' };
                        }
                        if (false === Csw.contains(oldInterval, 'monthlyfrequency')) {
                            oldInterval.monthlyfrequency = '';
                        }
                        if (false === Csw.contains(oldInterval, 'startingmonth')) {
                            oldInterval.startingmonth = '';
                        }
                        if (false === Csw.contains(oldInterval, 'startingyear')) {
                            oldInterval.startingyear = '';
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                        if (false === Csw.contains(oldInterval, 'monthlyweek')) {
                            oldInterval.monthlyweek = '';
                        }
                        if (false === Csw.contains(oldInterval, 'monthlyday')) {
                            oldInterval.monthlyday = '';
                        }
                        if (false === Csw.contains(oldInterval, 'monthlyfrequency')) {
                            oldInterval.monthlyfrequency = '';
                        }
                        if (false === Csw.contains(oldInterval, 'startingmonth')) {
                            oldInterval.startingmonth = '';
                        }
                        if (false === Csw.contains(oldInterval, 'startingyear')) {
                            oldInterval.startingyear = '';
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.YearlyByDate:
                        if (false === Csw.contains(oldInterval, 'yearlydate')) {
                            oldInterval.yearlydate = { date: '', dateformat: '' };
                        }
                        break;
                }

                Csw.preparePropJsonForSave(o.Multi, o.propData, compare);

*/