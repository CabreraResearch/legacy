/// <reference path="~app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.timeInterval = Csw.controls.timeInterval ||
        Csw.controls.register('timeInterval', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                ID: '',
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
            var cswPublic = {
                rateInterval: {}
            };

            if (options) {
                Csw.extend(cswPrivate, options, true);
            }

            cswPrivate.now = new Date();
            cswPrivate.nowString = (cswPrivate.now.getMonth() + 1) + '/' + cswPrivate.now.getDate() + '/' + cswPrivate.now.getFullYear();

            cswPrivate.saveRateInterval = function () {
                Csw.clientDb.setItem(cswPrivate.ID + '_rateIntervalSave', cswPublic.rateInterval);
            };

            cswPrivate.weekDayDef = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

            cswPrivate.makeWeekDayPicker = function (thisRateType) {
                //return (function () {
                var weeklyDayPickerComplete = false,
                    ret, weekdays, startingDate,
                    isWeekly = (thisRateType === Csw.enums.rateIntervalTypes.WeeklyByDay),
                    dayPropName = 'weeklyday';

                if (false === isWeekly) {
                    dayPropName = 'monthlyday';
                }

                return function (onChange, useRadio, elemId, parent) {
                    var id = elemId || cswPrivate.ID + '_weeklyday',
                        pickerTable, i, type, weeklyStartDate = {}, weeklyTable, weeklyTableCell;

                    function isChecked(day) {
                        var thisDay = cswPrivate.weekDayDef[day - 1];
                        return false === cswPrivate.Multi && Csw.contains(weekdays, thisDay);
                    }

                    function saveWeekInterval() {
                        if (isWeekly) {
                            Csw.each(cswPublic.rateInterval, function (prop, key) {
                                if (key !== 'dateformat' && key !== 'ratetype' && key !== 'startingdate' && key !== 'weeklyday') {
                                    delete cswPublic.rateInterval[key];
                                }
                            });
                            if (startingDate) {
                                cswPublic.rateInterval.startingdate = startingDate.val();
                            } else {
                                cswPublic.rateInterval.startingdate = {};
                            }
                            cswPublic.rateInterval.startingdate.dateformat = cswPrivate.dateFormat;
                        }
                        cswPublic.rateInterval.ratetype = thisRateType;
                        cswPublic.rateInterval.dateformat = cswPrivate.dateFormat;
                        cswPublic.rateInterval[dayPropName] = weekdays.join(',');
                        cswPrivate.saveRateInterval();
                    }

                    function dayChange(val, checkbox) {
                        Csw.tryExec(cswPrivate.onChange);

                        var day = cswPrivate.weekDayDef[val - 1];
                        if (checkbox.checked()) {
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
                        ret = parent || cswPrivate.pickerCell.div();

                        weeklyTable = ret.table({
                            ID: Csw.makeId(id, 'weeklytbl'),
                            cellalign: 'center',
                            FirstCellRightAlign: true
                        });

                        weekdays = Csw.string(cswPublic.rateInterval[dayPropName]).split(',');

                        weeklyTable.cell(1, 1).text('Every:');
                        if (thisRateType === Csw.enums.rateIntervalTypes.WeeklyByDay) {
                            weeklyTable.cell(2, 1).text('Starting On:');
                        }

                        weeklyTableCell = weeklyTable.cell(2, 2);
                        pickerTable = weeklyTable.cell(1, 2).table({
                            ID: Csw.makeId(id, 'weeklytblpicker'),
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
                            if (false === cswPrivate.Multi && Csw.contains(cswPublic.rateInterval, 'startingdate')) {
                                weeklyStartDate = Csw.string(cswPublic.rateInterval.startingdate.date);
                            }
                            if (Csw.isNullOrEmpty(weeklyStartDate)) {
                                weeklyStartDate = cswPrivate.nowString;
                                cswPublic.rateInterval.startingdate = { date: cswPrivate.nowString, dateformat: cswPrivate.dateFormat };
                                cswPrivate.saveRateInterval();
                            }

                            startingDate = weeklyTableCell.dateTimePicker({
                                ID: Csw.makeId(cswPrivate.ID, 'weekly', 'sd'),
                                Date: weeklyStartDate,
                                DateFormat: cswPrivate.dateFormat,
                                DisplayMode: 'Date',
                                ReadOnly: cswPrivate.ReadOnly,
                                Required: cswPrivate.Required,
                                onChange: function () {
                                    Csw.tryExec(cswPrivate.onChange);
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
                // } ()); // cswPrivate.makeWeekDayPicker()
            };

            cswPrivate.weeklyWeekPicker = cswPrivate.makeWeekDayPicker(Csw.enums.rateIntervalTypes.WeeklyByDay);
            cswPrivate.monthlyWeekPicker = cswPrivate.makeWeekDayPicker(Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay);

            cswPrivate.makeMonthlyPicker = (function () {
                var monthlyPickerComplete = false,
                    ret;

                return function () {
                    var monthlyRateSelect, monthlyDateSelect, monthlyWeekSelect, startOnMonth, startOnYear,
                        monthlyRadioId = Csw.makeId(cswPrivate.ID, 'monthly'),
                        monthlyDayPickerId = Csw.makeId(cswPrivate.ID, 'monthly', 'day');

                    function saveMonthInterval() {
                        Csw.each(cswPublic.rateInterval, function (prop, key) {
                            if (key !== 'dateformat' &&
                                key !== 'ratetype' &&
                                    key !== 'monthlyday' &&
                                        key !== 'monthlydate' &&
                                            key !== 'monthlyfrequency' &&
                                                key !== 'startingmonth' &&
                                                    key !== 'startingyear') {
                                delete cswPublic.rateInterval[key];
                            }
                        });
                        if (cswPrivate.rateType === Csw.enums.rateIntervalTypes.MonthlyByDate) {
                            delete cswPublic.rateInterval.monthlyday;
                            delete cswPublic.rateInterval.monthlyweek;
                            cswPublic.rateInterval.monthlydate = monthlyDateSelect.find(':selected').val();
                        } else {
                            delete cswPublic.rateInterval.monthlydate;
                            cswPublic.rateInterval.monthlyweek = monthlyWeekSelect.find(':selected').val();
                        }

                        cswPublic.rateInterval.ratetype = cswPrivate.rateType;
                        cswPublic.rateInterval.dateformat = cswPrivate.dateFormat;
                        cswPublic.rateInterval.monthlyfrequency = monthlyRateSelect.find(':selected').val();
                        cswPublic.rateInterval.startingmonth = startOnMonth.find(':selected').val();
                        cswPublic.rateInterval.startingyear = startOnYear.find(':selected').val();
                        cswPrivate.saveRateInterval();
                    }

                    function makeMonthlyByDateSelect(monParent) {
                        var byDate = monParent.div(),
                            daysInMonth = ChemSW.makeSequentialArray(1, 31), selectedDay = '';

                        if (Csw.bool(cswPrivate.Multi)) {
                            selectedDay = Csw.enums.multiEditDefaultValue;
                            daysInMonth.unshift(Csw.enums.multiEditDefaultValue);
                        } else if (Csw.contains(cswPublic.rateInterval, 'monthlydate')) {
                            selectedDay = Csw.number(cswPublic.rateInterval.monthlydate, cswPrivate.now.getDay());
                        }

                        byDate.input({
                            ID: Csw.makeId(cswPrivate.ID, 'monthly', 'by_date'),
                            name: monthlyRadioId,
                            type: Csw.enums.inputTypes.radio,
                            onChange: function () {
                                Csw.tryExec(cswPrivate.onChange);
                                cswPrivate.rateType = ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                                saveMonthInterval();
                            },
                            value: Csw.enums.rateIntervalTypes.MonthlyByDate,
                            checked: cswPrivate.rateType === Csw.enums.rateIntervalTypes.MonthlyByDate
                        });
                        byDate.append('On Day of Month:&nbsp;');

                        monthlyDateSelect = byDate.select({
                            ID: Csw.makeId(cswPrivate.ID, 'monthly', 'date'),
                            onChange: function () {
                                Csw.tryExec(cswPrivate.onChange);
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

                        if (Csw.bool(cswPrivate.Multi)) {
                            frequency.unshift(Csw.enums.multiEditDefaultValue);
                            selected = Csw.enums.multiEditDefaultValue;
                        } else {
                            selected = Csw.number(cswPublic.rateInterval.monthlyfrequency, 1);
                        }

                        monthlyRateSelect = divEvery.select({
                            ID: Csw.makeId(cswPrivate.ID, 'monthly', 'rate'),
                            onChange: function () {
                                Csw.tryExec(cswPrivate.onChange);
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
                            monthlyWeekId = Csw.makeId(cswPrivate.ID, 'monthly', 'week'),
                            monthlyByDayId = Csw.makeId(cswPrivate.ID, 'monthly', 'by_day'),
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
                                Csw.tryExec(cswPrivate.onChange);
                                cswPrivate.rateType = ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                                saveMonthInterval();
                            },
                            value: Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay,
                            checked: cswPrivate.rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay
                        });

                        divByDay.append('Every&nbsp;');

                        if (cswPrivate.Multi) {
                            weeksInMonth.unshift({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                            selected = Csw.enums.multiEditDefaultValue;
                        } else {
                            selected = Csw.number(cswPublic.rateInterval.monthlyweek, 1);
                        }

                        monthlyWeekSelect = divByDay.select({
                            ID: monthlyWeekId,
                            values: weeksInMonth,
                            selected: selected,
                            onChange: function () {
                                Csw.tryExec(cswPrivate.onChange);
                                saveMonthInterval();
                            }
                        });
                        divByDay.br();

                        cswPrivate.monthlyWeekPicker(cswPrivate.onChange, true, monthlyDayPickerId, divByDay);
                        return divByDay;
                    }

                    function makeStartOnSelects(monParent) {
                        var divStartOn = monParent.div(),
                            monthsInYear = ChemSW.makeSequentialArray(1, 12),
                            year = cswPrivate.now.getFullYear(),
                            yearsToAllow = ChemSW.makeSequentialArray(year - 10, year + 10),
                            selectedMonth, selectedYear;

                        divStartOn.br();
                        divStartOn.append('Starting On:&nbsp;');

                        if (cswPrivate.Multi) {
                            monthsInYear.unshift(Csw.enums.multiEditDefaultValue);
                            yearsToAllow.unshift(Csw.enums.multiEditDefaultValue);
                            selectedMonth = Csw.enums.multiEditDefaultValue;
                            selectedYear = Csw.enums.multiEditDefaultValue;
                        } else {
                            selectedMonth = Csw.number(cswPublic.rateInterval.startingmonth, (cswPrivate.now.getMonth() + 1));
                            selectedYear = Csw.number(cswPublic.rateInterval.startingyear, year);
                        }

                        startOnMonth = divStartOn.select({
                            ID: Csw.makeId(cswPrivate.ID, 'monthly', 'startMonth'),
                            values: monthsInYear,
                            selected: selectedMonth,
                            onChange: function () {
                                Csw.tryExec(cswPrivate.onChange);
                                saveMonthInterval();
                            }
                        });

                        //divStartOn.append('/');

                        startOnYear = divStartOn.select({
                            ID: Csw.makeId(cswPrivate.ID, 'monthly', 'startYear'),
                            values: yearsToAllow,
                            selected: selectedYear,
                            onChange: function () {
                                Csw.tryExec(cswPrivate.onChange);
                                saveMonthInterval();
                            }
                        });
                        return divStartOn;
                    }

                    if (false === monthlyPickerComplete) {
                        ret = cswPrivate.pickerCell.div();
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

            cswPrivate.makeYearlyDatePicker = (function () {
                var yearlyDatePickerComplete = false,
                    retDiv, yearlyDate;
                return function () {

                    function saveYearInterval() {
                        Csw.each(cswPublic.rateInterval, function (prop, key) {
                            if (key !== 'dateformat' &&
                                key !== 'ratetype' &&
                                    key !== 'yearlydate') {
                                delete cswPublic.rateInterval[key];
                            }
                        });

                        cswPublic.rateInterval.ratetype = cswPrivate.rateType;
                        cswPublic.rateInterval.dateformat = cswPrivate.dateFormat;
                        if (yearlyDate) {
                            cswPublic.rateInterval.yearlydate = yearlyDate.val();
                        } else {
                            cswPublic.rateInterval.yearlydate = {};
                        }
                        cswPublic.rateInterval.yearlydate.dateformat = cswPrivate.dateFormat;
                        cswPrivate.saveRateInterval();
                    }

                    if (false === yearlyDatePickerComplete) {
                        retDiv = cswPrivate.pickerCell.div();

                        var yearlyStartDate = '';

                        if (Csw.bool(cswPrivate.Multi)) {
                            yearlyStartDate = Csw.enums.multiEditDefaultValue;
                        } else if (Csw.contains(cswPublic.rateInterval, 'yearlydate')) {
                            yearlyStartDate = Csw.string(cswPublic.rateInterval.yearlydate.date, cswPrivate.now.toLocaleDateString());
                        }
                        if (Csw.isNullOrEmpty(yearlyStartDate)) {
                            yearlyStartDate = cswPrivate.nowString;
                            cswPublic.rateInterval.yearlydate = { date: cswPrivate.nowString, dateformat: cswPrivate.dateFormat };
                        }

                        retDiv.append('Every Year, Starting On: ').br();

                        yearlyDate = retDiv.div().dateTimePicker({
                            ID: Csw.makeId(cswPrivate.ID, 'yearly', 'sd'),
                            Date: yearlyStartDate,
                            DateFormat: cswPrivate.dateFormat,
                            DisplayMode: 'Date',
                            ReadOnly: cswPrivate.ReadOnly,
                            Required: cswPrivate.Required,
                            onChange: function () {
                                Csw.tryExec(cswPrivate.onChange);
                                saveYearInterval();
                            }
                        });

                        saveYearInterval();

                        yearlyDatePickerComplete = true;
                    } // if (false === yearlyDatePickerComplete)
                    return retDiv.addClass('CswFieldTypeTimeInterval_Div');
                };
            } ());

            cswPrivate.makeRateType = function (table) {

                function onChange(newRateType) {
                    Csw.tryExec(cswPrivate.onChange);
                    cswPrivate.rateType = newRateType;
                    cswPublic.rateInterval.ratetype = cswPrivate.rateType;

                    if (false === Csw.isNullOrEmpty(cswPrivate.divWeekly, true)) {
                        cswPrivate.divWeekly.hide();
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivate.divMonthly, true)) {
                        cswPrivate.divMonthly.hide();
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivate.divYearly, true)) {
                        cswPrivate.divYearly.hide();
                    }

                    switch (newRateType) {
                        case Csw.enums.rateIntervalTypes.WeeklyByDay:
                            if (cswPrivate.divWeekly) {
                                cswPrivate.divWeekly.show();
                            }
                            break;
                        case Csw.enums.rateIntervalTypes.MonthlyByDate:
                            if (cswPrivate.divMonthly) {
                                cswPrivate.divMonthly.show();
                            }
                            break;
                        case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                            if (cswPrivate.divMonthly) {
                                cswPrivate.divMonthly.show();
                            }
                            break;
                        case Csw.enums.rateIntervalTypes.YearlyByDate:
                            if (cswPrivate.divYearly) {
                                cswPrivate.divYearly.show();
                            }
                            break;
                    }

                    cswPrivate.saveRateInterval();
                }

                var subTable = table.cell(2, 1).table();

                //Weekly
                subTable.cell(1, 2).span({ text: '&nbsp;Weekly' });
                subTable.cell(1, 1).input({
                    ID: Csw.makeId(cswPrivate.ID, 'type', 'weekly'),
                    name: Csw.makeId(cswPrivate.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'weekly',
                    checked: cswPrivate.rateType === Csw.enums.rateIntervalTypes.WeeklyByDay,
                    onClick: function () {
                        onChange(Csw.enums.rateIntervalTypes.WeeklyByDay);
                        cswPrivate.divWeekly = cswPrivate.divWeekly || cswPrivate.weeklyWeekPicker(cswPrivate.onChange, false);
                    }
                });

                //Monthly
                subTable.cell(2, 2).span({ text: '&nbsp;Monthly' });
                subTable.cell(2, 1).input({
                    ID: Csw.makeId(cswPrivate.ID, 'type', 'monthly'),
                    name: Csw.makeId(cswPrivate.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'monthly',
                    checked: cswPrivate.rateType === Csw.enums.rateIntervalTypes.MonthlyByDate || cswPrivate.rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay,
                    onClick: function () {
                        onChange(Csw.enums.rateIntervalTypes.MonthlyByDate);
                        cswPrivate.divMonthly = cswPrivate.divMonthly || cswPrivate.makeMonthlyPicker();
                    }
                });

                //Yearly
                subTable.cell(3, 2).span({ text: '&nbsp;Yearly' });
                subTable.cell(3, 1).input({
                    ID: Csw.makeId(cswPrivate.ID, 'type', 'yearly'),
                    name: Csw.makeId(cswPrivate.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'yearly',
                    checked: cswPrivate.rateType === Csw.enums.rateIntervalTypes.YearlyByDate,
                    onClick: function () {
                        onChange(Csw.enums.rateIntervalTypes.YearlyByDate);
                        cswPrivate.divYearly = cswPrivate.divYearly || cswPrivate.makeYearlyDatePicker();
                    }
                });
            };

            (function () {
                cswPrivate.interval = cswParent.div({
                    ID: cswPrivate.ID
                });
                cswPublic = Csw.dom({}, cswPrivate.interval);
                //Csw.literals.factory(cswPrivate.$parent, cswPublic);
                cswPublic.rateInterval = {};
                
                var propVals = cswPrivate.propVals,
                    textValue,
                    table;

                if (cswPrivate.Multi) {
                    //cswPublic.rateInterval = Csw.enums.multiEditDefaultValue;
                    textValue = Csw.enums.multiEditDefaultValue;
                    cswPrivate.rateType = Csw.enums.rateIntervalTypes.WeeklyByDay;
                } else {
                    Csw.extend(cswPublic.rateInterval, propVals.Interval.rateintervalvalue, true);
                    textValue = Csw.string(propVals.Interval.text).trim();
                    cswPrivate.rateType = cswPublic.rateInterval.ratetype;
                }
                cswPrivate.dateFormat = Csw.string(cswPublic.rateInterval.dateformat, 'M/d/yyyy');
                cswPublic.interval = cswPrivate.interval.div({
                    ID: Csw.makeId(cswPrivate.ID, 'cswTimeInterval')
                });

                //Page Components
                cswPublic.interval.span({
                    ID: Csw.makeId(cswPrivate.ID, 'textvalue'),
                    text: textValue
                });
                table = cswPrivate.interval.table({
                    ID: Csw.makeId(cswPrivate.ID, 'tbl'),
                    cellspacing: 5
                });

                cswPrivate.makeRateType(table);

                cswPrivate.pickerCell = table.cell(1, 3).propDom('rowspan', '3');

                // Set selected values
                switch (cswPrivate.rateType) {
                    case Csw.enums.rateIntervalTypes.WeeklyByDay:
                        cswPrivate.divWeekly = cswPrivate.weeklyWeekPicker(cswPrivate.onChange, false);
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByDate:
                        cswPrivate.divMonthly = cswPrivate.makeMonthlyPicker();
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                        cswPrivate.divMonthly = cswPrivate.makeMonthlyPicker();
                        break;
                    case Csw.enums.rateIntervalTypes.YearlyByDate:
                        cswPrivate.divYearly = cswPrivate.makeYearlyDatePicker();
                        break;
                } // switch(RateType)


            } ());

            cswPublic.validateRateInterval = function () {
                var retVal = false, errorString = '';
                switch (cswPrivate.rateType) {
                    case Csw.enums.rateIntervalTypes.WeeklyByDay:
                        if (false === Csw.contains(cswPublic.rateInterval, 'startingdate') ||
                        false === Csw.contains(cswPublic.rateInterval.startingdate, 'date') ||
                            Csw.isNullOrEmpty(cswPublic.rateInterval.startingdate.date)) {
                            errorString += 'Cannot add a Weekly time interval without a starting date. ';
                        }
                        if (false === Csw.contains(cswPublic.rateInterval, 'weeklyday') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.weeklyday)) {
                            errorString += 'Cannot add a Weekly time interval without at least one weekday selected. ';
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByDate:
                        if (false === Csw.contains(cswPublic.rateInterval, 'monthlydate') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.monthlydate)) {
                            errorString += 'Cannot add a Monthly time interval without an \'On Day of Month\' selected. ';
                        }
                        if (false === Csw.contains(cswPublic.rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.monthlyfrequency)) {
                            errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                        }
                        if (false === Csw.contains(cswPublic.rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.startingmonth)) {
                            errorString += 'Cannot add a Monthly time interval without a Starting Month selected. ';
                        }
                        if (false === Csw.contains(cswPublic.rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.startingyear)) {
                            errorString += 'Cannot add a Monthly time interval without a Starting Year selected. ';
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                        if (false === Csw.contains(cswPublic.rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.monthlyfrequency)) {
                            errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                        }
                        if (false === Csw.contains(cswPublic.rateInterval, 'monthlyday') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.monthlyday)) {
                            errorString += 'Cannot add a Monthly time interval without a Weekday selected. ';
                        }
                        if (false === Csw.contains(cswPublic.rateInterval, 'monthlyweek') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.monthlyweek)) {
                            errorString += 'Cannot add a Monthly time interval without a Weekly frequency selected. ';
                        }
                        if (false === Csw.contains(cswPublic.rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.startingmonth)) {
                            errorString += 'Cannot add a Monthly time interval without a starting month selected. ';
                        }
                        if (false === Csw.contains(cswPublic.rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(cswPublic.rateInterval.startingyear)) {
                            errorString += 'Cannot add a Monthly time interval without a starting year selected. ';
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.YearlyByDate:
                        if (false === Csw.contains(cswPublic.rateInterval, 'yearlydate') ||
                        false === Csw.contains(cswPublic.rateInterval.yearlydate, 'date') ||
                            Csw.isNullOrEmpty(cswPublic.rateInterval.yearlydate.date)) {
                            errorString += 'Cannot addd a Yearly time interval without a starting date. ';
                        }
                        break;
                }
                if (false === Csw.isNullOrEmpty(errorString)) {
                    retVal = Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, errorString);
                }
                return retVal;
            };

            cswPublic.rateType = function () {
                return cswPrivate.rateType;
            };
            cswPublic.val = function () {
                return cswPublic.rateInterval;
            };

            return cswPublic;
        });


} ());