/// <reference path="~/js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswTimeInterval() {
    'use strict';

    var timeInterval = function (options) {
        var internal = {
            ID: '',
            $parent: '',
            values: {},
            Multi: false,
            ReadOnly: false,
            Required: false,
            onChange: null,
            rateType: {},
            divWeekly: '',
            divMonthly: '',
            divYearly: '',
            dateFormat: ''
        };
        var external = {
            rateInterval: {}
        };

        if (options) {
            $.extend(true, internal, options);
        }

        internal.now = new Date();
        internal.nowString = (internal.now.getMonth() + 1) + '/' + internal.now.getDate() + '/' + internal.now.getFullYear();

        internal.saveRateInterval = function () {
            Csw.clientDb.setItem(internal.ID + '_rateIntervalSave', external.rateInterval);
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

            return function (onChange, useRadio, elemId, parent) {
                var id = elemId || internal.ID + '_weeklyday',
                    pickerTable, i, type, weeklyStartDate = {}, weeklyTable, weeklyTableCell;

                function isChecked(day) {
                    var thisDay = internal.weekDayDef[day - 1];
                    return false === internal.Multi && Csw.contains(weekdays, thisDay);
                }

                function saveWeekInterval() {
                    if (isWeekly) {
                        Csw.each(external.rateInterval, function (prop, key) {
                            if (key !== 'dateformat' && key !== 'ratetype' && key !== 'startingdate' && key !== 'weeklyday') {
                                delete external.rateInterval[key];
                            }
                        });
                        if (startingDate) {
                            external.rateInterval.startingdate = startingDate.val();
                        } else {
                            external.rateInterval.startingdate = {};
                        }
                        external.rateInterval.startingdate.dateformat = internal.dateFormat;
                    }
                    external.rateInterval.ratetype = thisRateType;
                    external.rateInterval.dateformat = internal.dateFormat;
                    external.rateInterval[dayPropName] = weekdays.join(',');
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
                    ret = parent || internal.pickerCell.div();

                    weeklyTable = ret.table({
                        ID: Csw.controls.dom.makeId(id, 'weeklytbl'),
                        cellalign: 'center',
                        FirstCellRightAlign: true
                    });

                    weekdays = Csw.string(external.rateInterval[dayPropName]).split(',');

                    weeklyTable.cell(1, 1).text('Every:');
                    if (thisRateType === Csw.enums.rateIntervalTypes.WeeklyByDay) {
                        weeklyTable.cell(2, 1).text('Starting On:');
                    }

                    weeklyTableCell = weeklyTable.cell(2, 2);
                    pickerTable = weeklyTable.cell(1, 2).table({
                        ID: Csw.controls.dom.makeId(id, 'weeklytblpicker'),
                        cellalign: 'center'
                    });

                    pickerTable.cell(1, 1).text('Su');
                    pickerTable.cell(1, 2).text('M');
                    pickerTable.cell(1, 3).text('Tu');
                    pickerTable.cell(1, 4).text('W');
                    pickerTable.cell(1, 5).text('Th');
                    pickerTable.cell(1, 6).text('F');
                    pickerTable.cell(1, 7).text('Sa');

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
                                onChange: dayChange,
                                value: i,
                                checked: isChecked(i)
                            });
                    } //for (i = 1; i <= 7; i += 1)

                    //Starting Date
                    if (isWeekly) {
                        if (false === internal.Multi && Csw.contains(external.rateInterval, 'startingdate')) {
                            weeklyStartDate = Csw.string(external.rateInterval.startingdate.date);
                        }
                        if (Csw.isNullOrEmpty(weeklyStartDate)) {
                            weeklyStartDate = internal.nowString;
                            external.rateInterval.startingdate = { date: internal.nowString, dateformat: internal.dateFormat };
                            internal.saveRateInterval();
                        }

                        startingDate = weeklyTableCell.dateTimePicker({
                            ID: Csw.controls.dom.makeId(internal.ID, 'weekly', 'sd'),
                            Date: weeklyStartDate,
                            DateFormat: internal.dateFormat,
                            DisplayMode: 'Date',
                            ReadOnly: internal.ReadOnly,
                            Required: internal.Required,
                            onChange: function () {
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

            return function () {
                var monthlyRateSelect, monthlyDateSelect, monthlyWeekSelect, startOnMonth, startOnYear,
                    monthlyRadioId = Csw.controls.dom.makeId(internal.ID, 'monthly'),
                    monthlyDayPickerId = Csw.controls.dom.makeId(internal.ID, 'monthly', 'day');

                function saveMonthInterval() {
                    Csw.each(external.rateInterval, function (prop, key) {
                        if (key !== 'dateformat' &&
                            key !== 'ratetype' &&
                            key !== 'monthlyday' &&
                            key !== 'monthlydate' &&
                            key !== 'monthlyfrequency' &&
                            key !== 'startingmonth' &&
                            key !== 'startingyear') {
                            delete external.rateInterval[key];
                        }
                    });
                    if (internal.rateType === Csw.enums.rateIntervalTypes.MonthlyByDate) {
                        delete external.rateInterval.monthlyday;
                        delete external.rateInterval.monthlyweek;
                        external.rateInterval.monthlydate = monthlyDateSelect.find(':selected').val();
                    } else {
                        delete external.rateInterval.monthlydate;
                        external.rateInterval.monthlyweek = monthlyWeekSelect.find(':selected').val();
                    }

                    external.rateInterval.ratetype = internal.rateType;
                    external.rateInterval.dateformat = internal.dateFormat;
                    external.rateInterval.monthlyfrequency = monthlyRateSelect.find(':selected').val();
                    external.rateInterval.startingmonth = startOnMonth.find(':selected').val();
                    external.rateInterval.startingyear = startOnYear.find(':selected').val();
                    internal.saveRateInterval();
                }

                function makeMonthlyByDateSelect(monParent) {
                    var byDate = monParent.div(),
                        daysInMonth = ChemSW.makeSequentialArray(1, 31), selectedDay = '';

                    if (Csw.bool(internal.Multi)) {
                        selectedDay = Csw.enums.multiEditDefaultValue;
                        daysInMonth.unshift(Csw.enums.multiEditDefaultValue);
                    } else if (Csw.contains(external.rateInterval, 'monthlydate')) {
                        selectedDay = Csw.number(external.rateInterval.monthlydate, internal.now.getDay());
                    }

                    byDate.input({
                        ID: Csw.controls.dom.makeId(internal.ID, 'monthly', 'by_date'),
                        name: monthlyRadioId,
                        type: Csw.enums.inputTypes.radio,
                        onChange: function () {
                            Csw.tryExec(internal.onChange);
                            internal.rateType = ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: Csw.enums.rateIntervalTypes.MonthlyByDate,
                        checked: internal.rateType === Csw.enums.rateIntervalTypes.MonthlyByDate
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

                function makeEveryMonthSelect(monParent) {
                    var divEvery = monParent.div({ text: 'Every' }),
                        frequency = ChemSW.makeSequentialArray(1, 12),
                        selected;

                    if (Csw.bool(internal.Multi)) {
                        frequency.unshift(Csw.enums.multiEditDefaultValue);
                        selected = Csw.enums.multiEditDefaultValue;
                    } else {
                        selected = Csw.number(external.rateInterval.monthlyfrequency, 1);
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

                function makeMonthlyByDayOfWeek(monParent) {
                    var divByDay = monParent.div(),
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
                            internal.rateType = ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay,
                        checked: internal.rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay
                    });

                    divByDay.append('Every&nbsp;');

                    if (internal.Multi) {
                        weeksInMonth.unshift({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                        selected = Csw.enums.multiEditDefaultValue;
                    } else {
                        selected = Csw.number(external.rateInterval.monthlyweek, 1);
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

                    internal.monthlyWeekPicker(internal.onChange, true, monthlyDayPickerId, divByDay);
                    return divByDay;
                }

                function makeStartOnSelects(monParent) {
                    var divStartOn = monParent.div(),
                        monthsInYear = ChemSW.makeSequentialArray(1, 12),
                        year = internal.now.getFullYear(),
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
                        selectedMonth = Csw.number(external.rateInterval.startingmonth, (internal.now.getMonth() + 1));
                        selectedYear = Csw.number(external.rateInterval.startingyear, year);
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
                    ret = internal.pickerCell.div();
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
            return function () {

                function saveYearInterval() {
                    Csw.each(external.rateInterval, function (prop, key) {
                        if (key !== 'dateformat' &&
                            key !== 'ratetype' &&
                            key !== 'yearlydate') {
                            delete external.rateInterval[key];
                        }
                    });

                    external.rateInterval.ratetype = internal.rateType;
                    external.rateInterval.dateformat = internal.dateFormat;
                    if (yearlyDate) {
                        external.rateInterval.yearlydate = yearlyDate.val();
                    } else {
                        external.rateInterval.yearlydate = {};
                    }
                    external.rateInterval.yearlydate.dateformat = internal.dateFormat;
                    internal.saveRateInterval();
                }

                if (false === yearlyDatePickerComplete) {
                    retDiv = internal.pickerCell.div();

                    var yearlyStartDate = '';

                    if (Csw.bool(internal.Multi)) {
                        yearlyStartDate = Csw.enums.multiEditDefaultValue;
                    }
                    else if (Csw.contains(external.rateInterval, 'yearlydate')) {
                        yearlyStartDate = Csw.string(external.rateInterval.yearlydate.date, internal.now.toLocaleDateString());
                    }
                    if (Csw.isNullOrEmpty(yearlyStartDate)) {
                        yearlyStartDate = internal.nowString;
                        external.rateInterval.yearlydate = { date: internal.nowString, dateformat: internal.dateFormat };
                    }

                    retDiv.append('Every Year, Starting On: ').br();

                    yearlyDate = retDiv.div().dateTimePicker({
                        ID: Csw.controls.dom.makeId(internal.ID, 'yearly', 'sd'),
                        Date: yearlyStartDate,
                        DateFormat: internal.dateFormat,
                        DisplayMode: 'Date',
                        ReadOnly: internal.ReadOnly,
                        Required: internal.Required,
                        onChange: function () {
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

            function onChange(newRateType) {
                Csw.tryExec(internal.onChange);
                internal.rateType = newRateType;
                external.rateInterval.ratetype = internal.rateType;

                if (false === Csw.isNullOrEmpty(internal.divWeekly, true)) {
                    internal.divWeekly.hide();
                }
                if (false === Csw.isNullOrEmpty(internal.divMonthly, true)) {
                    internal.divMonthly.hide();
                }
                if (false === Csw.isNullOrEmpty(internal.divYearly, true)) {
                    internal.divYearly.hide();
                }

                switch (newRateType) {
                    case Csw.enums.rateIntervalTypes.WeeklyByDay:
                        if (internal.divWeekly) {
                            internal.divWeekly.show();
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByDate:
                        if (internal.divMonthly) {
                            internal.divMonthly.show();
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                        if (internal.divMonthly) {
                            internal.divMonthly.show();
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.YearlyByDate:
                        if (internal.divYearly) {
                            internal.divYearly.show();
                        }
                        break;
                }

                internal.saveRateInterval();
            }

            var subTable = table.cell(2, 1).table();

            //Weekly
            subTable.cell(1, 2).span({ text: '&nbsp;Weekly' });
            var inpWeeklyRadio = subTable.cell(1, 1).input({
                ID: Csw.controls.dom.makeId(internal.ID, 'type', 'weekly'),
                name: Csw.controls.dom.makeId(internal.ID, 'type', '', '', false),
                type: Csw.enums.inputTypes.radio,
                value: 'weekly',
                checked: internal.rateType === Csw.enums.rateIntervalTypes.WeeklyByDay,
                onClick: function () {
                    onChange(Csw.enums.rateIntervalTypes.WeeklyByDay);
                    internal.divWeekly = internal.divWeekly || internal.weeklyWeekPicker(internal.onChange, false);
                }
            });

            //Monthly
            subTable.cell(2, 2).span({ text: '&nbsp;Monthly' });
            var inpMonthlyRadio = subTable.cell(2, 1).input({
                ID: Csw.controls.dom.makeId(internal.ID, 'type', 'monthly'),
                name: Csw.controls.dom.makeId(internal.ID, 'type', '', '', false),
                type: Csw.enums.inputTypes.radio,
                value: 'monthly',
                checked: internal.rateType === Csw.enums.rateIntervalTypes.MonthlyByDate || internal.rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay,
                onClick: function () {
                    onChange(Csw.enums.rateIntervalTypes.MonthlyByDate);
                    internal.divMonthly = internal.divMonthly || internal.makeMonthlyPicker();
                }
            });

            //Yearly
            subTable.cell(3, 2).span({ text: '&nbsp;Yearly' });
            var inpYearlyRadio = subTable.cell(3, 1).input({
                ID: Csw.controls.dom.makeId(internal.ID, 'type', 'yearly'),
                name: Csw.controls.dom.makeId(internal.ID, 'type', '', '', false),
                type: Csw.enums.inputTypes.radio,
                value: 'yearly',
                checked: internal.rateType === Csw.enums.rateIntervalTypes.YearlyByDate,
                onClick: function () {
                    onChange(Csw.enums.rateIntervalTypes.YearlyByDate);
                    internal.divYearly = internal.divYearly || internal.makeYearlyDatePicker();
                }
            });
        };

        external.validateRateInterval = function () {
            var retVal = false, errorString = '';
            switch (internal.rateType) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    if (false === Csw.contains(external.rateInterval, 'startingdate') ||
                        false === Csw.contains(external.rateInterval.startingdate, 'date') ||
                            Csw.isNullOrEmpty(external.rateInterval.startingdate.date)) {
                        errorString += 'Cannot add a Weekly time interval without a starting date. ';
                    }
                    if (false === Csw.contains(external.rateInterval, 'weeklyday') ||
                        Csw.isNullOrEmpty(external.rateInterval.weeklyday)) {
                        errorString += 'Cannot add a Weekly time interval without at least one weekday selected. ';
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    if (false === Csw.contains(external.rateInterval, 'monthlydate') ||
                        Csw.isNullOrEmpty(external.rateInterval.monthlydate)) {
                        errorString += 'Cannot add a Monthly time interval without an \'On Day of Month\' selected. ';
                    }
                    if (false === Csw.contains(external.rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(external.rateInterval.monthlyfrequency)) {
                        errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                    }
                    if (false === Csw.contains(external.rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(external.rateInterval.startingmonth)) {
                        errorString += 'Cannot add a Monthly time interval without a Starting Month selected. ';
                    }
                    if (false === Csw.contains(external.rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(external.rateInterval.startingyear)) {
                        errorString += 'Cannot add a Monthly time interval without a Starting Year selected. ';
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    if (false === Csw.contains(external.rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(external.rateInterval.monthlyfrequency)) {
                        errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                    }
                    if (false === Csw.contains(external.rateInterval, 'monthlyday') ||
                        Csw.isNullOrEmpty(external.rateInterval.monthlyday)) {
                        errorString += 'Cannot add a Monthly time interval without a Weekday selected. ';
                    }
                    if (false === Csw.contains(external.rateInterval, 'monthlyweek') ||
                        Csw.isNullOrEmpty(external.rateInterval.monthlyweek)) {
                        errorString += 'Cannot add a Monthly time interval without a Weekly frequency selected. ';
                    }
                    if (false === Csw.contains(external.rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(external.rateInterval.startingmonth)) {
                        errorString += 'Cannot add a Monthly time interval without a starting month selected. ';
                    }
                    if (false === Csw.contains(external.rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(external.rateInterval.startingyear)) {
                        errorString += 'Cannot add a Monthly time interval without a starting year selected. ';
                    }
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    if (false === Csw.contains(external.rateInterval, 'yearlydate') ||
                        false === Csw.contains(external.rateInterval.yearlydate, 'date') ||
                            Csw.isNullOrEmpty(external.rateInterval.yearlydate.date)) {
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
            Csw.controls.factory(internal.$parent, external);
            var propVals = internal.propVals,
                textValue,
                table;

            if (internal.Multi) {
                //external.rateInterval = Csw.enums.multiEditDefaultValue;
                textValue = Csw.enums.multiEditDefaultValue;
                internal.rateType = Csw.enums.rateIntervalTypes.WeeklyByDay;
            } else {
                $.extend(true, external.rateInterval, propVals.Interval.rateintervalvalue);
                textValue = Csw.string(propVals.Interval.text).trim();
                internal.rateType = external.rateInterval.ratetype;
            }
            internal.dateFormat = Csw.string(external.rateInterval.dateformat, 'M/d/yyyy');
            external.interval = external.div({
                ID: Csw.controls.dom.makeId(internal.ID, 'cswTimeInterval')
            });

            //Page Components
            external.interval.span({
                ID: Csw.controls.dom.makeId(internal.ID, 'textvalue'),
                text: textValue
            });
            table = external.table({
                ID: Csw.controls.dom.makeId(internal.ID, 'tbl'),
                cellspacing: 5
            });

            internal.makeRateType(table);

            internal.pickerCell = table.cell(1, 3).propDom('rowspan', '3');

            // Set selected values
            switch (internal.rateType) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    internal.divWeekly = internal.weeklyWeekPicker(internal.onChange, false);
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    internal.divMonthly = internal.makeMonthlyPicker();
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    internal.divMonthly = internal.makeMonthlyPicker();
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    internal.divYearly = internal.makeYearlyDatePicker();
                    break;
            } // switch(RateType)


        } ());

        external.rateType = function () {
            return internal.rateType;
        };
        external.val = function () {
            return external.rateInterval;
        };

        return external;
    };
    Csw.controls.register('timeInterval', timeInterval);
    Csw.controls.timeInterval = Csw.controls.timeInterval || timeInterval;

} ());