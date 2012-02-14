/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswTimeInterval() {
    'use strict';

    var timeInterval = function (options) {
        var internal = {};
        var external = {};

        var o = {
            ID: '',
            $parent: '',
            values: {},
            Multi: false,
            ReadOnly: false,
            Required: false,
            onChange: null
        };
        if (options) {
            $.extend(true, internal, options);
        }

        var now = new Date(),
            nowString = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear(),
            rateType, divWeekly, divMonthly, divYearly, dateFormat, rateInterval = {}, pickerCell;

        internal.saveRateInterval = function () {
            Csw.clientDb.setItem(internal.ID + '_rateIntervalSave', rateInterval);
        };

        internal.toggleIntervalDiv = function (interval, inpWeeklyRadio, inpMonthlyRadio, inpYearlyRadio) {

            if (window.abandonHope) {
                inpWeeklyRadio.propDom('checked', false);
                inpMonthlyRadio.propDom('checked', false);
                inpYearlyRadio.propDom('checked', false);
            }
            if (false === Csw.isNullOrEmpty(divWeekly, true)) {
                divWeekly.hide();
            }
            if (false === Csw.isNullOrEmpty(divMonthly, true)) {
                divMonthly.hide();
            }
            if (false === Csw.isNullOrEmpty(divYearly, true)) {
                divYearly.hide();
            }
            switch (interval) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    divWeekly.show();
                    if (window.abandonHope) {
                        inpWeeklyRadio.propDom('checked', true);
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    divMonthly.show();
                    if (window.abandonHope) {
                        inpMonthlyRadio.propDom('checked', true);
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    divMonthly.show();
                    if (window.abandonHope) {
                        inpMonthlyRadio.propDom('checked', true);
                    }
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    divYearly.show();
                    if (window.abandonHope) {
                        inpYearlyRadio.propDom('checked', true);
                    }
                    break;
            }
        };

        internal.weekDayDef = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

        internal.makeWeekDayPicker = function (thisRateType) {
            //return (function () {
            var weeklyDayPickerComplete = false,
                ret, weekdays, startingDate,
                isWeekly = (thisRateType === Csw.enums.rateIntervalTypes.WeeklyByDay),
                dayPropName = 'weeklyday';

            if (false === isWeekly) {
                dayPropName = 'monthlyday';
            }

            return function (parent, onchange, useRadio, elemId) {
                var id = elemId || internal.ID + '_weeklyday',
                    pickerTable, i, type, pickerCheck, weeklyStartDate = {}, weeklyTable;

                function isChecked(day) {
                    var thisDay = internal.weekDayDef[day - 1];
                    return false === internal.Multi && Csw.contains(weekdays, thisDay);
                }

                function saveWeekInterval() {
                    if (isWeekly) {
                        Csw.each(rateInterval, function (prop, key) {
                            if (key !== 'dateformat' && key !== 'ratetype' && key !== 'startingdate' && key !== 'weeklyday') {
                                delete rateInterval[key];
                            }
                        });
                        if (startingDate) {
                            rateInterval.startingdate = startingDate.$.CswDateTimePicker('value');
                        } else {
                            rateInterval.startingdate = {};
                        }
                        rateInterval.startingdate.dateformat = dateFormat;
                    }
                    rateInterval.ratetype = thisRateType;
                    rateInterval.dateformat = dateFormat;
                    rateInterval[dayPropName] = weekdays.join(',');
                    internal.saveRateInterval();
                }

                function dayChange() {
                    Csw.tryExec(internal.onChange);

                    var $this = $(this),
                        day = internal.weekDayDef[$this.val() - 1];
                    if ($this.is(':checked')) {
                        if (false === isWeekly) {
                            weekdays = [];
                        }
                        if (false === Csw.contains(weekdays, day)) {
                            weekdays.push(day);
                        }
                    } else {
                        weekdays.splice(weekdays.indexOf(day), 1);
                    }
                    saveWeekInterval();
                }

                if (false === weeklyDayPickerComplete) {
                    ret = parent.div();

                    weeklyTable = ret.table({
                        ID: Csw.controls.dom.makeId(id, 'weeklytbl'),
                        cellalign: 'center',
                        FirstCellRightAlign: true
                    });

                    weekdays = Csw.string(rateInterval[dayPropName]).split(',');

                    weeklyTable.add(1, 1, 'Every:');
                    weeklyTable.add(2, 1, 'Starting On:');
                    startingDate = weeklyTable.cell(2, 2);

                    pickerTable = weeklyTable.cell(1, 2).table({
                        ID: Csw.controls.dom.makeId(id, 'weeklytblpicker'),
                        cellalign: 'center'
                    });

                    pickerTable.add(1, 1, 'Su');
                    pickerTable.add(1, 2, 'M');
                    pickerTable.add(1, 3, 'Tu');
                    pickerTable.add(1, 4, 'W');
                    pickerTable.add(1, 5, 'Th');
                    pickerTable.add(1, 6, 'F');
                    pickerTable.add(1, 7, 'Sa');

                    for (i = 1; i <= 7; i += 1) {
                        type = Csw.enums.inputTypes.checkbox;
                        if (useRadio) {
                            type = Csw.enums.inputTypes.radio;
                        }
                        pickerTable.cell(2, i)
                            .input({
                                ID: id + '_' + i,
                                name: id,
                                type: type,
                                onChange: dayChange(),
                                value: i,
                                checked: isChecked(i)
                            });
                    } //for (i = 1; i <= 7; i += 1)

                    //Starting Date
                    if (isWeekly) {
                        if (false === internal.Multi && Csw.contains(rateInterval, 'startingdate')) {
                            weeklyStartDate = Csw.string(rateInterval.startingdate.date);
                        }
                        if (Csw.isNullOrEmpty(weeklyStartDate)) {
                            rateInterval.startingdate = { date: nowString, dateformat: dateFormat };
                            internal.saveRateInterval();
                        }

                        startingDate.$.CswDateTimePicker('init', {
                            ID: Csw.controls.dom.makeId(internal.ID, 'weekly', 'sd'),
                            Date: weeklyStartDate,
                            DateFormat: dateFormat,
                            DisplayMode: 'Date',
                            ReadOnly: internal.ReadOnly,
                            Required: internal.Required,
                            OnChange: function () {
                                Csw.tryExec(internal.onChange);
                                saveWeekInterval();
                            }
                        });
                    } //if(isWeekly)

                    saveWeekInterval();

                    weeklyDayPickerComplete = true;
                    ret.addClass('CswFieldTypeTimeInterval_Div');
                } // if (false === weeklyDayComplete)

                return ret;
            };
            // } ()); // internal.makeWeekDayPicker()
        };

        internal.weeklyWeekPicker = internal.makeWeekDayPicker(Csw.enums.rateIntervalTypes.WeeklyByDay);
        internal.monthlyWeekPicker = internal.makeWeekDayPicker(Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay);

        internal.makeMonthlyPicker = (function () {
            var monthlyPickerComplete = false,
                ret;

            return function (parent) {
                var monthlyRateSelect, monthlyDateSelect, monthlyWeekSelect, startOnMonth, startOnYear,
                    monthlyRadioId = Csw.controls.dom.makeId(internal.ID, 'monthly'),
                    monthlyDayPickerId = Csw.controls.dom.makeId(internal.ID, 'monthly', 'day');

                function saveMonthInterval() {
                    Csw.each(rateInterval, function (prop, key) {
                        if (key !== 'dateformat' &&
                            key !== 'ratetype' &&
                            key !== 'monthlyday' &&
                            key !== 'monthlydate' &&
                            key !== 'monthlyfrequency' &&
                            key !== 'startingmonth' &&
                            key !== 'startingyear') {
                            delete rateInterval[key];
                        }
                    });
                    if (rateType === Csw.enums.rateIntervalTypes.MonthlyByDate) {
                        delete rateInterval.monthlyday;
                        delete rateInterval.monthlyweek;
                        rateInterval.monthlydate = monthlyDateSelect.find(':selected').val();
                    } else {
                        delete rateInterval.monthlydate;
                        rateInterval.monthlyweek = monthlyWeekSelect.find(':selected').val();
                    }

                    rateInterval.ratetype = rateType;
                    rateInterval.dateformat = dateFormat;
                    rateInterval.monthlyfrequency = monthlyRateSelect.find(':selected').val();
                    rateInterval.startingmonth = startOnMonth.find(':selected').val();
                    rateInterval.startingyear = startOnYear.find(':selected').val();
                    internal.saveRateInterval();
                }

                function makeMonthlyByDateSelect(parent) {
                    var byDate = parent.div(),
                        inpCheck,
                        daysInMonth = ChemSW.makeSequentialArray(1, 31), selectedDay = '';

                    if (Csw.bool(internal.Multi)) {
                        selectedDay = Csw.enums.multiEditDefaultValue;
                        daysInMonth.unshift(Csw.enums.multiEditDefaultValue);
                    } else if (Csw.contains(rateInterval, 'monthlydate')) {
                        selectedDay = Csw.number(rateInterval.monthlydate, 1);
                    }

                    byDate.input({
                        ID: Csw.controls.dom.makeId(internal.ID, 'monthly', 'by_date'),
                        name: monthlyRadioId,
                        type: Csw.enums.inputTypes.radio,
                        onChange: function () {
                            Csw.tryExec(internal.onChange);
                            rateType = ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: Csw.enums.rateIntervalTypes.MonthlyByDate,
                        checked: rateType === Csw.enums.rateIntervalTypes.MonthlyByDate
                    });
                    byDate.append('On Day of Month:&nbsp;');

                    monthlyDateSelect = byDate.select({
                        ID: Csw.controls.dom.makeId(internal.ID, 'monthly', 'date'),
                        onChange: function () {
                            Csw.tryExec(internal.onChange);
                            saveMonthInterval();
                        },
                        values: daysInMonth,
                        selected: selectedDay
                    });

                    return byDate;
                }

                function makeEveryMonthSelect(parent) {
                    var divEvery = parent.div({ text: 'Every' }),
                        frequency = ChemSW.makeSequentialArray(1, 12),
                        selected;

                    if (Csw.bool(internal.Multi)) {
                        frequency.unshift(Csw.enums.multiEditDefaultValue);
                        selected = Csw.enums.multiEditDefaultValue;
                    } else {
                        selected = Csw.number(rateInterval.monthlyfrequency, 1);
                    }

                    monthlyRateSelect = divEvery.select({
                        ID: Csw.controls.dom.makeId(internal.ID, 'monthly', 'rate'),
                        onChange: function () {
                            Csw.tryExec(internal.onChange);
                            saveMonthInterval();
                        },
                        values: frequency,
                        selected: selected
                    });

                    divEvery.append(' Month(s)').br();
                    return divEvery;
                }

                function makeMonthlyByDayOfWeek(parent) {
                    var divByDay = parent.div(),
                        inpCheck,
                        monthlyWeekId = Csw.controls.dom.makeId(internal.ID, 'monthly', 'week'),
                        monthlyByDayId = Csw.controls.dom.makeId(internal.ID, 'monthly', 'by_day'),
                        selected,
                        weeksInMonth = [
                            { value: 1, display: 'First:' },
                            { value: 2, display: 'Second:' },
                            { value: 3, display: 'Third:' },
                            { value: 4, display: 'Fourth:' }
                        ];

                    divByDay.input({
                        ID: monthlyByDayId,
                        name: monthlyRadioId,
                        type: Csw.enums.inputTypes.radio,
                        onChange: function () {
                            Csw.tryExec(internal.onChange);
                            rateType = ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay,
                        checked: rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay
                    });
                    
                    divByDay.append('Every&nbsp;');

                    if (internal.Multi) {
                        weeksInMonth.unshift({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                        selected = Csw.enums.multiEditDefaultValue;
                    } else {
                        selected = Csw.number(rateInterval.monthlyweek, 1);
                    }

                    monthlyWeekSelect = divByDay.select({
                        ID: monthlyWeekId,
                        values: weeksInMonth,
                        selected: selected,
                        onChange: function () {
                            Csw.tryExec(internal.onChange);
                            saveMonthInterval();
                        }
                    });
                    divByDay.br();

                    internal.monthlyWeekPicker(divByDay, internal.onChange, true, monthlyDayPickerId);
                    return divByDay;
                }

                function makeStartOnSelects(parent) {
                    var divStartOn = parent.div(),
                        monthsInYear = ChemSW.makeSequentialArray(1, 12),
                        year = now.getFullYear(),
                        yearsToAllow = ChemSW.makeSequentialArray(year - 10, year + 10),
                        selectedMonth, selectedYear;

                    divStartOn.br();
                    divStartOn.append('Starting On:&nbsp;');

                    if (internal.Multi) {
                        monthsInYear.unshift(Csw.enums.multiEditDefaultValue);
                        yearsToAllow.unshift(Csw.enums.multiEditDefaultValue);
                        selectedMonth = Csw.enums.multiEditDefaultValue;
                        selectedYear = Csw.enums.multiEditDefaultValue;
                    } else {
                        selectedMonth = Csw.number(rateInterval.startingmonth, (now.getMonth() + 1));
                        selectedYear = Csw.number(rateInterval.startingyear, year);
                    }

                    startOnMonth = divStartOn.select({
                        ID: Csw.controls.dom.makeId(internal.ID, 'monthly', 'startMonth'),
                        values: monthsInYear,
                        selected: selectedMonth,
                        onChange: function () {
                            Csw.tryExec(internal.onChange);
                            saveMonthInterval();
                        }
                    });

                    //divStartOn.append('/');

                    startOnYear = divStartOn.select({
                        ID: Csw.controls.dom.makeId(internal.ID, 'monthly', 'startYear'),
                        values: yearsToAllow,
                        selected: selectedYear,
                        onChange: function () {
                            Csw.tryExec(internal.onChange);
                            saveMonthInterval();
                        }
                    });
                    return divStartOn;
                }

                if (false === monthlyPickerComplete) {
                    ret = parent.div();
                    makeEveryMonthSelect(ret);
                    makeMonthlyByDateSelect(ret);
                    ret.br();
                    makeMonthlyByDayOfWeek(ret);
                    makeStartOnSelects(ret);
                    ret.addClass('CswFieldTypeTimeInterval_Div');

                    saveMonthInterval();

                    monthlyPickerComplete = true;
                } // if (false === monthlyPickerComplete)

                return ret;
            };
        } ());

        internal.makeYearlyDatePicker = (function () {
            var yearlyDatePickerComplete = false,
                retDiv, yearlyDate;
            return function (parent) {

                function saveYearInterval() {
                    Csw.each(rateInterval, function (prop, key) {
                        if (key !== 'dateformat' &&
                            key !== 'ratetype' &&
                            key !== 'yearlydate') {
                            delete rateInterval[key];
                        }
                    });

                    rateInterval.ratetype = rateType;
                    rateInterval.dateformat = dateFormat;
                    if (yearlyDate) {
                        rateInterval.yearlydate = yearlyDate.$.CswDateTimePicker('value');
                    } else {
                        rateInterval.yearlydate = {};
                    }
                    rateInterval.yearlydate.dateformat = dateFormat;
                    internal.saveRateInterval();
                }

                if (false === yearlyDatePickerComplete) {
                    retDiv = parent.div();

                    var yearlyStartDate = '';

                    if (Csw.bool(internal.Multi)) {
                        yearlyStartDate = Csw.enums.multiEditDefaultValue;
                    } else if (Csw.contains(rateInterval, 'yearlydate')) {
                        yearlyStartDate = Csw.string(rateInterval.yearlydate.date);
                    }
                    if (Csw.isNullOrEmpty(yearlyStartDate)) {
                        rateInterval.yearlydate = { date: nowString, dateformat: dateFormat };
                    }

                    retDiv.append('Every Year, Starting On:<br/>');

                    yearlyDate = retDiv.div();
                    yearlyDate.$.CswDateTimePicker('init', {
                        ID: Csw.controls.dom.makeId(internal.ID, 'yearly', 'sd'),
                        Date: yearlyStartDate,
                        DateFormat: dateFormat,
                        DisplayMode: 'Date',
                        ReadOnly: internal.ReadOnly,
                        Required: internal.Required,
                        OnChange: function () {
                            Csw.tryExec(internal.onChange);
                            saveYearInterval();
                        }
                    });

                    saveYearInterval();

                    yearlyDatePickerComplete = true;
                } // if (false === yearlyDatePickerComplete)
                return retDiv.addClass('CswFieldTypeTimeInterval_Div');
            };
        } ());

        internal.makeRateType = function (table) {

            var inpWeeklyRadio = table.cell(1, 1).input({
                ID: Csw.controls.dom.makeId(internal.ID, 'type', 'weekly'),
                name: Csw.controls.dom.makeId(internal.ID, 'type'),
                type: Csw.enums.inputTypes.radio,
                value: 'weekly',
                checked: rateType === Csw.enums.rateIntervalTypes.WeeklyByDay
            });

            var inpMonthlyRadio = table.cell(2, 1).input({
                ID: Csw.controls.dom.makeId(internal.ID, 'type', 'monthly'),
                name: Csw.controls.dom.makeId(internal.ID, 'type'),
                type: Csw.enums.inputTypes.radio,
                value: 'monthly',
                checked: rateType === Csw.enums.rateIntervalTypes.MonthlyByDate || rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay
            });

            var inpYearlyRadio = table.cell(3, 1).input({
                ID: Csw.controls.dom.makeId(internal.ID, 'type', 'yearly'),
                name: Csw.controls.dom.makeId(internal.ID, 'type'),
                type: Csw.enums.inputTypes.radio,
                value: 'yearly',
                checked: rateType === Csw.enums.rateIntervalTypes.YearlyByDate
            });

            function onChange() {
                if (Csw.isFunction(internal.onChange)) {
                    internal.onChange();
                }
                internal.toggleIntervalDiv(rateType, inpWeeklyRadio, inpMonthlyRadio, inpYearlyRadio);
                internal.saveRateInterval();
            }

            //Weekly
            table.cell(1, 2).span({ text: '&nbsp;Weekly' });
            inpWeeklyRadio.bind('click', function () {
                rateType = Csw.enums.rateIntervalTypes.WeeklyByDay;
                rateInterval.ratetype = rateType;
                divWeekly = divWeekly || internal.weeklyWeekPicker(pickerCell, internal.onChange, false);
                onChange();
            });

            //Monthly
            table.add(2, 2).span({ text: '&nbsp;Monthly' });
            inpMonthlyRadio.bind('click', function () {
                rateType = Csw.enums.rateIntervalTypes.MonthlyByDate;
                rateInterval.ratetype = rateType;
                divMonthly = divMonthly || internal.makeMonthlyPicker(pickerCell);
                onChange();
            });

            //Yearly
            table.add(3, 2).span({ text: '&nbsp;Yearly' });
            inpYearlyRadio.bind('click', function () {
                rateType = Csw.enums.rateIntervalTypes.YearlyByDate;
                rateInterval.ratetype = rateType;
                divYearly = divYearly || internal.makeYearlyDatePicker(pickerCell);
                onChange();
            });
        };

        external.validateRateInterval = function () {
            var retVal = false, errorString = '';
            switch (rateType) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    if (false === Csw.contains(rateInterval, 'startingdate') ||
                        false === Csw.contains(rateInterval.startingdate, 'date') ||
                            Csw.isNullOrEmpty(rateInterval.startingdate.date)) {
                        errorString += 'Cannot add a Weekly time interval without a starting date. ';
                    }
                    if (false === Csw.contains(rateInterval, 'weeklyday') ||
                        Csw.isNullOrEmpty(rateInterval.weeklyday)) {
                        errorString += 'Cannot add a Weekly time interval without at least one weekday selected. ';
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    if (false === Csw.contains(rateInterval, 'monthlydate') ||
                        Csw.isNullOrEmpty(rateInterval.monthlydate)) {
                        errorString += 'Cannot add a Monthly time interval without an \'On Day of Month\' selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(rateInterval.monthlyfrequency)) {
                        errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(rateInterval.startingmonth)) {
                        errorString += 'Cannot add a Monthly time interval without a Starting Month selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(rateInterval.startingyear)) {
                        errorString += 'Cannot add a Monthly time interval without a Starting Year selected. ';
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    if (false === Csw.contains(rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(rateInterval.monthlyfrequency)) {
                        errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'monthlyday') ||
                        Csw.isNullOrEmpty(rateInterval.monthlyday)) {
                        errorString += 'Cannot add a Monthly time interval without a Weekday selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'monthlyweek') ||
                        Csw.isNullOrEmpty(rateInterval.monthlyweek)) {
                        errorString += 'Cannot add a Monthly time interval without a Weekly frequency selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(rateInterval.startingmonth)) {
                        errorString += 'Cannot add a Monthly time interval without a starting month selected. ';
                    }
                    if (false === Csw.contains(rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(rateInterval.startingyear)) {
                        errorString += 'Cannot add a Monthly time interval without a starting year selected. ';
                    }
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    if (false === Csw.contains(rateInterval, 'yearlydate') ||
                        false === Csw.contains(rateInterval.yearlydate, 'date') ||
                            Csw.isNullOrEmpty(rateInterval.yearlydate.date)) {
                        errorString += 'Cannot addd a Yearly time interval without a starting date. ';
                    }
                    break;
            }
            if (false === Csw.isNullOrEmpty(errorString)) {
                retVal = Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, errorString);
            }
            return retVal;
        };

        (function () {
            var parent = Csw.controls.factory(internal.$parent),
                propVals = internal.propVals,
                textValue,
                table;

            if (internal.Multi) {
                //rateInterval = Csw.enums.multiEditDefaultValue;
                textValue = Csw.enums.multiEditDefaultValue;
                rateType = Csw.enums.rateIntervalTypes.WeeklyByDay;
            } else {
                $.extend(true, rateInterval, propVals.Interval.rateintervalvalue);
                textValue = Csw.string(propVals.Interval.text).trim();
                rateType = rateInterval.ratetype;
            }
            dateFormat = Csw.string(rateInterval.dateformat, 'M/d/yyyy');
            external.interval = parent.div({
                ID: Csw.controls.dom.makeId(internal.ID, 'cswTimeInterval')
            });

            //Page Components
            external.interval.span({
                ID: Csw.controls.dom.makeId(internal.ID, 'textvalue'),
                text: textValue
            });
            table = parent.table({
                ID: Csw.controls.dom.makeId(internal.ID, 'tbl'),
                cellspacing: 5
            });

            internal.makeRateType(table);

            pickerCell = table.cell(1, 3).propDom('rowspan', '3');

            // Set selected values
            switch (rateType) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    divWeekly = internal.weeklyWeekPicker(pickerCell, internal.onChange, false);
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    divMonthly = internal.makeMonthlyPicker(pickerCell);
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    divMonthly = internal.makeMonthlyPicker(pickerCell);
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    divYearly = internal.makeYearlyDatePicker(pickerCell);
                    break;
            } // switch(RateType)

            return external.interval;
        } ());

        external.rateType = function () {
            return rateType;
        };
        external.rateInterval = function () {
            return rateInterval;
        };

        return external;
    };
    Csw.controls.register('timeInterval', timeInterval);
    Csw.controls.timeInterval = Csw.controls.timeInterval || timeInterval;

} ());