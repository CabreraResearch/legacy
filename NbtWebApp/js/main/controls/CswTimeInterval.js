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
            $.extend(true, o, options);
        }

        var now = new Date(),
            nowString = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear(),
            rateType, $WeeklyDiv, $MonthlyDiv, $YearlyDiv, dateFormat, rateInterval = {}, pickerCell;

        internal.saveRateInterval = function () {
            Csw.clientDb.setItem(o.ID + '_rateIntervalSave', rateInterval);
        };

        internal.toggleIntervalDiv = function (interval, inpWeeklyRadio, inpMonthlyRadio, inpYearlyRadio) {

            if (window.abandonHope) {
                inpWeeklyRadio.propDom('checked', false);
                inpMonthlyRadio.propDom('checked', false);
                inpYearlyRadio.propDom('checked', false);
            }
            if (false === Csw.isNullOrEmpty($WeeklyDiv, true)) {
                $WeeklyDiv.hide();
            }
            if (false === Csw.isNullOrEmpty($MonthlyDiv, true)) {
                $MonthlyDiv.hide();
            }
            if (false === Csw.isNullOrEmpty($YearlyDiv, true)) {
                $YearlyDiv.hide();
            }
            switch (interval) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    $WeeklyDiv.show();
                    if (window.abandonHope) {
                        inpWeeklyRadio.propDom('checked', true);
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    $MonthlyDiv.show();
                    if (window.abandonHope) {
                        inpMonthlyRadio.propDom('checked', true);
                    }
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    $MonthlyDiv.show();
                    if (window.abandonHope) {
                        inpMonthlyRadio.propDom('checked', true);
                    }
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    $YearlyDiv.show();
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
                $ret, weekdays, startingDate,
                isWeekly = (thisRateType === Csw.enums.rateIntervalTypes.WeeklyByDay),
                dayPropName = 'weeklyday';

            if (false === isWeekly) {
                dayPropName = 'monthlyday';
            }

            return function ($parent, onChange, useRadio, elemId) {
                var id = elemId || o.ID + '_weeklyday',
                    picker, pickerTable, i, type, pickerCell, weeklyStartDate, weeklyTable;

                function isChecked(day) {
                    var thisDay = internal.weekDayDef[day - 1];
                    return false === o.Multi && Csw.contains(weekdays, thisDay);
                }

                function saveWeekInterval() {
                    if (isWeekly) {
                        Csw.each(rateInterval, function (prop, key) {
                            if (key !== 'dateformat' && key !== 'ratetype' && key !== 'startingdate' && key !== 'weeklyday') {
                                delete rateInterval[key];
                            }
                        });
                        rateInterval.startingdate = startingDate.CswDateTimePicker('value');
                        rateInterval.startingdate.dateformat = dateFormat;
                    }
                    rateInterval.ratetype = thisRateType;
                    rateInterval.dateformat = dateFormat;
                    rateInterval[dayPropName] = weekdays.join(',');
                    internal.saveRateInterval();
                }

                function dayChange() {
                    if (Csw.isFunction(o.onChange)) {
                        o.onChange();
                    }
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
                    $ret = $('<div />').appendTo($parent);

                    weeklyTable = Csw.controls.table({
                        $parent: $ret,
                        ID: Csw.controls.dom.makeId(id, 'weeklytbl'),
                        cellalign: 'center',
                        FirstCellRightAlign: true
                    });

                    weekdays = Csw.string(rateInterval[dayPropName]).split(',');

                    picker = weeklyTable.cell(1, 2);
                    pickerTable = Csw.controls.table({
                        $parent: picker.$,
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
                        pickerCell = pickerTable.cell(2, i);
                        pickerCell.input({
                            ID: id + '_' + i,
                            name: id,
                            type: type,
                            onChange: dayChange,
                            value: i
                        }).propDom('checked', isChecked(i));

                    } //for (i = 1; i <= 7; i += 1)

                    //Starting Date
                    if (isWeekly) {
                        if (false === o.Multi && Csw.contains(rateInterval, 'startingdate')) {
                            weeklyStartDate = Csw.string(rateInterval.startingdate.date);
                        }
                        if (Csw.isNullOrEmpty(weeklyStartDate)) {
                            rateInterval.startingdate = { date: nowString, dateformat: dateFormat };
                            internal.saveRateInterval();
                        }
                        weeklyTable.add(1, 1, 'Every:');
                        weeklyTable.add(2, 1, 'Starting On:');
                        startingDate = weeklyTable.cell(2, 2);
                        startingDate.$.CswDateTimePicker('init', {
                            ID: startingDate.makeId(o.ID, 'weekly_sd'),
                            Date: weeklyStartDate,
                            DateFormat: dateFormat,
                            DisplayMode: 'Date',
                            ReadOnly: o.ReadOnly,
                            Required: o.Required,
                            onChange: function () {
                                if (Csw.isFunction(o.onChange)) {
                                    o.onChange();
                                }
                                saveWeekInterval();
                            }
                        });
                    } //if(isWeekly)

                    saveWeekInterval();

                    weeklyDayPickerComplete = true;
                    $ret.addClass('CswFieldTypeTimeInterval_Div');
                } // if (false === weeklyDayComplete)

                return $ret;
            };
            // } ()); // internal.makeWeekDayPicker()
        };

        internal.weeklyWeekPicker = internal.makeWeekDayPicker(Csw.enums.rateIntervalTypes.WeeklyByDay);
        internal.monthlyWeekPicker = internal.makeWeekDayPicker(Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay);

        internal.makeMonthlyPicker = (function () {
            var monthlyPickerComplete = false,
                $ret;

            return function ($parent) {
                var $MonthlyRateSelect, $MonthlyDateSelect, $MonthlyWeekSelect, $startOnMonth, $startOnYear,
                    monthlyRadioId = Csw.controls.dom.makeId({ prefix: o.ID, ID: 'monthly' }),
                    monthlyDayPickerId = Csw.controls.dom.makeId({ prefix: o.ID, ID: 'monthly', suffix: 'day' });

                function saveMonthInterval() {
                    Csw.each(rateInterval, function (prop, key) {
                        if (key !== 'dateformat' && key !== 'ratetype' && key !== 'monthlyday' && key !== 'monthlydate' && key !== 'monthlyfrequency' && key !== 'startingmonth' && key !== 'startingyear') {
                            delete rateInterval[key];
                        }
                    });
                    if (rateType === Csw.enums.rateIntervalTypes.MonthlyByDate) {
                        delete rateInterval.monthlyday;
                        delete rateInterval.monthlyweek;
                        rateInterval.monthlydate = $MonthlyDateSelect.find(':selected').val();
                    } else {
                        delete rateInterval.monthlydate;
                        rateInterval.monthlyweek = $MonthlyWeekSelect.find(':selected').val();
                    }

                    rateInterval.ratetype = rateType;
                    rateInterval.dateformat = dateFormat;
                    rateInterval.monthlyfrequency = $MonthlyRateSelect.find(':selected').val();
                    rateInterval.startingmonth = $startOnMonth.find(':selected').val();
                    rateInterval.startingyear = $startOnYear.find(':selected').val();
                    internal.saveRateInterval();
                }

                function makeMonthlyByDateSelect() {
                    var $byDate = $('<div />'),
                        daysInMonth = ChemSW.makeSequentialArray(1, 31), selectedDay = '';

                    if (Csw.bool(o.Multi)) {
                        selectedDay = Csw.enums.multiEditDefaultValue;
                        daysInMonth.unshift(Csw.enums.multiEditDefaultValue);
                    } else if (Csw.contains(rateInterval, 'monthlydate')) {
                        selectedDay = Csw.number(rateInterval.monthlydate, 1);
                    }

                    $byDate.CswInput('init', {
                        ID: Csw.controls.dom.makeId({ prefix: o.ID, ID: 'monthly', suffix: 'by_date' }),
                        name: monthlyRadioId,
                        type: Csw.enums.inputTypes.radio,
                        onChange: function () {
                            if (Csw.isFunction(o.onChange)) {
                                o.onChange();
                            }
                            rateType = $ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: Csw.enums.rateIntervalTypes.MonthlyByDate
                    })
                        .CswAttrDom('checked', (rateType === Csw.enums.rateIntervalTypes.MonthlyByDate));

                    $byDate.append('On Day of Month:&nbsp;');

                    $MonthlyDateSelect = $byDate.CswSelect('init', {
                        ID: Csw.controls.dom.makeId({ prefix: o.ID, ID: 'monthly', suffix: 'date' }),
                        onChange: function () {
                            if (Csw.isFunction(o.onChange)) {
                                o.onChange();
                            }
                            saveMonthInterval();
                        },
                        values: daysInMonth,
                        selected: selectedDay
                    });

                    return $byDate;
                }

                function makeEveryMonthSelect() {
                    var $every = $('<div>Every </div>'),
                        frequency = ChemSW.makeSequentialArray(1, 12),
                        selected;

                    if (Csw.bool(o.Multi)) {
                        frequency.unshift(Csw.enums.multiEditDefaultValue);
                        selected = Csw.enums.multiEditDefaultValue;
                    } else {
                        selected = Csw.number(rateInterval.monthlyfrequency, 1);
                    }

                    $MonthlyRateSelect = $every.CswSelect('init', {
                        ID: Csw.controls.dom.makeId({ prefix: o.ID, ID: 'monthly', suffix: 'rate' }),
                        onChange: function () {
                            if (Csw.isFunction(o.onChange)) {
                                o.onChange();
                            }
                            saveMonthInterval();
                        },
                        values: frequency,
                        selected: selected
                    });

                    $every.append(' Month(s)<br/>');
                    return $every;
                }

                function makeMonthlyByDayOfWeek() {
                    var $byDay = $('<div />'),
                        monthlyWeekId = Csw.controls.dom.makeId({ prefix: o.ID, ID: 'monthly', suffix: 'week' }),
                        monthlyByDayId = Csw.controls.dom.makeId({ prefix: o.ID, ID: 'monthly', suffix: 'by_day' }),
                        selected,
                        weeksInMonth = [
                            { value: 1, display: 'First:' },
                            { value: 2, display: 'Second:' },
                            { value: 3, display: 'Third:' },
                            { value: 4, display: 'Fourth:' }
                        ];

                    $byDay.CswInput('init', {
                        ID: monthlyByDayId,
                        name: monthlyRadioId,
                        type: Csw.enums.inputTypes.radio,
                        onChange: function () {
                            if (Csw.isFunction(o.onChange)) {
                                o.onChange();
                            }
                            rateType = $ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay
                    })
                        .CswAttrDom('checked', (rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay));

                    $byDay.append('Every&nbsp;');

                    if (o.Multi) {
                        weeksInMonth.unshift({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                        selected = Csw.enums.multiEditDefaultValue;
                    } else {
                        selected = Csw.number(rateInterval.monthlyweek, 1);
                    }

                    $MonthlyWeekSelect = $byDay.CswSelect('init', {
                        ID: monthlyWeekId,
                        values: weeksInMonth,
                        selected: selected,
                        onChange: function () {
                            if (Csw.isFunction(o.onChange)) {
                                o.onChange();
                            }
                            saveMonthInterval();
                        }
                    });
                    $byDay.append('<br/>');

                    internal.monthlyWeekPicker($byDay, o.onChange, true, monthlyDayPickerId);
                    return $byDay;
                }

                function makeStartOnSelects() {
                    var $startOn = $('<div />'),
                        monthsInYear = ChemSW.makeSequentialArray(1, 12),
                        year = now.getFullYear(),
                        yearsToAllow = ChemSW.makeSequentialArray(year - 10, year + 10),
                        selectedMonth, selectedYear;

                    $startOn.append('<br/>Starting On:&nbsp;');

                    if (o.Multi) {
                        monthsInYear.unshift(Csw.enums.multiEditDefaultValue);
                        yearsToAllow.unshift(Csw.enums.multiEditDefaultValue);
                        selectedMonth = Csw.enums.multiEditDefaultValue;
                        selectedYear = Csw.enums.multiEditDefaultValue;
                    } else {
                        selectedMonth = Csw.number(rateInterval.startingmonth, (now.getMonth() + 1));
                        selectedYear = Csw.number(rateInterval.startingyear, year);
                    }

                    $startOnMonth = $startOn.CswSelect('init', {
                        ID: Csw.controls.dom.makeId({ prefix: o.ID, ID: 'monthly', suffix: 'startMonth' }),
                        values: monthsInYear,
                        selected: selectedMonth,
                        onChange: function () {
                            if (Csw.isFunction(o.onChange)) {
                                o.onChange();
                            }
                            saveMonthInterval();
                        }
                    });

                    $startOn.append('/');

                    $startOnYear = $startOn.CswSelect('init', {
                        ID: Csw.controls.dom.makeId({ prefix: o.ID, ID: 'monthly', suffix: 'startYear' }),
                        values: yearsToAllow,
                        selected: selectedYear,
                        onChange: function () {
                            if (Csw.isFunction(o.onChange)) {
                                o.onChange();
                            }
                            saveMonthInterval();
                        }
                    });
                    return $startOn;
                }

                if (false === monthlyPickerComplete) {
                    $ret = $('<div />').appendTo($parent);
                    $ret.append(makeEveryMonthSelect());
                    $ret.append(makeMonthlyByDateSelect());
                    $ret.append('<br/>');
                    $ret.append(makeMonthlyByDayOfWeek());
                    $ret.append(makeStartOnSelects());
                    $ret.addClass('CswFieldTypeTimeInterval_Div');

                    saveMonthInterval();

                    monthlyPickerComplete = true;
                } // if (false === monthlyPickerComplete)

                return $ret;
            };
        } ());

        internal.makeYearlyDatePicker = (function () {
            var yearlyDatePickerComplete = false,
                ret, yearlyDate;
            return function (parent) {

                function saveYearInterval() {
                    Csw.each(rateInterval, function (prop, key) {
                        if (key !== 'dateformat' && key !== 'ratetype' && key !== 'yearlydate') {
                            delete rateInterval[key];
                        }
                    });

                    rateInterval.ratetype = rateType;
                    rateInterval.dateformat = dateFormat;
                    rateInterval.yearlydate = yearlyDate.val();
                    rateInterval.yearlydate.dateformat = dateFormat;
                    internal.saveRateInterval();
                }

                if (false === yearlyDatePickerComplete) {
                    ret = parent.div();

                    var yearlyStartDate = '';

                    if (Csw.bool(o.Multi)) {
                        yearlyStartDate = Csw.enums.multiEditDefaultValue;
                    } else if (Csw.contains(rateInterval, 'yearlydate')) {
                        yearlyStartDate = Csw.string(rateInterval.yearlydate.date);
                    }
                    if (Csw.isNullOrEmpty(yearlyStartDate)) {
                        rateInterval.yearlydate = { date: nowString, dateformat: dateFormat };
                    }

                    ret.append('Every Year, Starting On').br();

                    yearlyDate = ret.dateTimePicker({
                        ID: Csw.controls.dom.makeId(o.ID, 'yearly', 'sd'),
                        Date: yearlyStartDate,
                        DateFormat: dateFormat,
                        DisplayMode: 'Date',
                        ReadOnly: o.ReadOnly,
                        Required: o.Required,
                        onChange: function () {
                            Csw.tryExec(o.onChange);
                            saveYearInterval();
                        }
                    });

                    ret.appendTo(parent);

                    saveYearInterval();

                    yearlyDatePickerComplete = true;
                } // if (false === yearlyDatePickerComplete)
                return ret.addClass('CswFieldTypeTimeInterval_Div');
            };
        } ());

        internal.makeRateType = function (table) {

            var inpWeeklyRadio = table.cell(1, 1).input({
                ID: o.ID + '_type_weekly',
                name: o.ID + '_type',
                type: Csw.enums.inputTypes.radio,
                value: 'weekly'
            }).propDom('checked', (rateType === Csw.enums.rateIntervalTypes.WeeklyByDay));

            var inpMonthlyRadio = table.cell(2, 1).input({
                ID: o.ID + '_type_monthly',
                name: o.ID + '_type',
                type: Csw.enums.inputTypes.radio,
                value: 'monthly'
            }).propDom('checked', (rateType === Csw.enums.rateIntervalTypes.MonthlyByDate || rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay));

            var inpYearlyRadio = table.cell(3, 1).input({
                ID: o.ID + '_type_yearly',
                name: o.ID + '_type',
                type: Csw.enums.inputTypes.radio,
                value: 'yearly'
            }).propDom('checked', (rateType === Csw.enums.rateIntervalTypes.YearlyByDate));

            function onChange() {
                if (Csw.isFunction(o.onChange)) {
                    o.onChange();
                }
                internal.toggleIntervalDiv(rateType, inpWeeklyRadio, inpMonthlyRadio, inpYearlyRadio);
                internal.saveRateInterval();
            }

            //Weekly
            table.add(1, 2, '<span>&nbsp;Weekly</span>');
            inpWeeklyRadio.bind('click', function () {
                rateType = Csw.enums.rateIntervalTypes.WeeklyByDay;
                rateInterval.ratetype = rateType;
                $WeeklyDiv = $WeeklyDiv || internal.weeklyWeekPicker(pickerCell, o.onChange, false);
                onChange();
            });

            //Monthly
            table.add(2, 2, '<span>&nbsp;Monthly</span>');
            inpMonthlyRadio.bind('click', function () {
                rateType = Csw.enums.rateIntervalTypes.MonthlyByDate;
                rateInterval.ratetype = rateType;
                $MonthlyDiv = $MonthlyDiv || internal.makeMonthlyPicker(pickerCell);
                onChange();
            });

            //Yearly
            table.add(3, 2, '<span>&nbsp;Yearly</span>');
            inpYearlyRadio.bind('click', function () {
                rateType = Csw.enums.rateIntervalTypes.YearlyByDate;
                rateInterval.ratetype = rateType;
                $YearlyDiv = $YearlyDiv || internal.makeYearlyDatePicker(pickerCell);
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
            var $Div = o.$parent,
                propVals = o.propVals,
                textValue,
                table;

            //globals
            if (o.Multi) {
                //rateInterval = Csw.enums.multiEditDefaultValue;
                textValue = Csw.enums.multiEditDefaultValue;
                rateType = Csw.enums.rateIntervalTypes.WeeklyByDay;
            } else {
                $.extend(true, rateInterval, propVals.Interval.rateintervalvalue);
                textValue = Csw.string(propVals.Interval.text).trim();
                rateType = rateInterval.ratetype;
            }
            dateFormat = Csw.string(rateInterval.dateformat, 'M/d/yyyy');
            external.$interval = $('<div id="' + Csw.controls.dom.makeId(o.ID, 'cswTimeInterval') + '"></div>')
                .appendTo($Div);

            //Page Components
            external.$interval.append('<span id="' + o.ID + '_textvalue">' + textValue + '</span>');
            table = Csw.controls.table({
                $parent: external.$interval,
                ID: Csw.controls.dom.makeId(o.ID, 'tbl'),
                cellspacing: 5
            });

            internal.makeRateType(table);

            pickerCell = table.cell(1, 3).propDom('rowspan', '3');

            // Set selected values
            switch (rateType) {
                case Csw.enums.rateIntervalTypes.WeeklyByDay:
                    $WeeklyDiv = internal.weeklyWeekPicker(pickerCell, o.onChange, false);
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByDate:
                    $MonthlyDiv = internal.makeMonthlyPicker(pickerCell);
                    break;
                case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                    $MonthlyDiv = internal.makeMonthlyPicker(pickerCell);
                    break;
                case Csw.enums.rateIntervalTypes.YearlyByDate:
                    $YearlyDiv = internal.makeYearlyDatePicker(pickerCell);
                    break;
            } // switch(RateType)

            return external.$interval;
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