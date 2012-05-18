/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.timeInterval = Csw.controls.timeInterval ||
        Csw.controls.register('timeInterval', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
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
            var cswPublicRet = {
                rateInterval: {}
            };

            if (options) {
                $.extend(true, cswPrivateVar, options);
            }

            cswPrivateVar.now = new Date();
            cswPrivateVar.nowString = (cswPrivateVar.now.getMonth() + 1) + '/' + cswPrivateVar.now.getDate() + '/' + cswPrivateVar.now.getFullYear();

            cswPrivateVar.saveRateInterval = function () {
                Csw.clientDb.setItem(cswPrivateVar.ID + '_rateIntervalSave', cswPublicRet.rateInterval);
            };

            cswPrivateVar.weekDayDef = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

            cswPrivateVar.makeWeekDayPicker = function (thisRateType) {
                //return (function () {
                var weeklyDayPickerComplete = false,
                    ret, weekdays, startingDate,
                    isWeekly = (thisRateType === Csw.enums.rateIntervalTypes.WeeklyByDay),
                    dayPropName = 'weeklyday';

                if (false === isWeekly) {
                    dayPropName = 'monthlyday';
                }

                return function (onChange, useRadio, elemId, parent) {
                    var id = elemId || cswPrivateVar.ID + '_weeklyday',
                        pickerTable, i, type, weeklyStartDate = {}, weeklyTable, weeklyTableCell;

                    function isChecked(day) {
                        var thisDay = cswPrivateVar.weekDayDef[day - 1];
                        return false === cswPrivateVar.Multi && Csw.contains(weekdays, thisDay);
                    }

                    function saveWeekInterval() {
                        if (isWeekly) {
                            Csw.each(cswPublicRet.rateInterval, function (prop, key) {
                                if (key !== 'dateformat' && key !== 'ratetype' && key !== 'startingdate' && key !== 'weeklyday') {
                                    delete cswPublicRet.rateInterval[key];
                                }
                            });
                            if (startingDate) {
                                cswPublicRet.rateInterval.startingdate = startingDate.val();
                            } else {
                                cswPublicRet.rateInterval.startingdate = {};
                            }
                            cswPublicRet.rateInterval.startingdate.dateformat = cswPrivateVar.dateFormat;
                        }
                        cswPublicRet.rateInterval.ratetype = thisRateType;
                        cswPublicRet.rateInterval.dateformat = cswPrivateVar.dateFormat;
                        cswPublicRet.rateInterval[dayPropName] = weekdays.join(',');
                        cswPrivateVar.saveRateInterval();
                    }

                    function dayChange() {
                        Csw.tryExec(cswPrivateVar.onChange);

                        var $this = $(this),
                            day = cswPrivateVar.weekDayDef[$this.val() - 1];
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
                        ret = parent || cswPrivateVar.pickerCell.div();

                        weeklyTable = ret.table({
                            ID: Csw.makeId(id, 'weeklytbl'),
                            cellalign: 'center',
                            FirstCellRightAlign: true
                        });

                        weekdays = Csw.string(cswPublicRet.rateInterval[dayPropName]).split(',');

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
                            if (false === cswPrivateVar.Multi && Csw.contains(cswPublicRet.rateInterval, 'startingdate')) {
                                weeklyStartDate = Csw.string(cswPublicRet.rateInterval.startingdate.date);
                            }
                            if (Csw.isNullOrEmpty(weeklyStartDate)) {
                                weeklyStartDate = cswPrivateVar.nowString;
                                cswPublicRet.rateInterval.startingdate = { date: cswPrivateVar.nowString, dateformat: cswPrivateVar.dateFormat };
                                cswPrivateVar.saveRateInterval();
                            }

                            startingDate = weeklyTableCell.dateTimePicker({
                                ID: Csw.makeId(cswPrivateVar.ID, 'weekly', 'sd'),
                                Date: weeklyStartDate,
                                DateFormat: cswPrivateVar.dateFormat,
                                DisplayMode: 'Date',
                                ReadOnly: cswPrivateVar.ReadOnly,
                                Required: cswPrivateVar.Required,
                                onChange: function () {
                                    Csw.tryExec(cswPrivateVar.onChange);
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
                // } ()); // cswPrivateVar.makeWeekDayPicker()
            };

            cswPrivateVar.weeklyWeekPicker = cswPrivateVar.makeWeekDayPicker(Csw.enums.rateIntervalTypes.WeeklyByDay);
            cswPrivateVar.monthlyWeekPicker = cswPrivateVar.makeWeekDayPicker(Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay);

            cswPrivateVar.makeMonthlyPicker = (function () {
                var monthlyPickerComplete = false,
                    ret;

                return function () {
                    var monthlyRateSelect, monthlyDateSelect, monthlyWeekSelect, startOnMonth, startOnYear,
                        monthlyRadioId = Csw.makeId(cswPrivateVar.ID, 'monthly'),
                        monthlyDayPickerId = Csw.makeId(cswPrivateVar.ID, 'monthly', 'day');

                    function saveMonthInterval() {
                        Csw.each(cswPublicRet.rateInterval, function (prop, key) {
                            if (key !== 'dateformat' &&
                                key !== 'ratetype' &&
                                    key !== 'monthlyday' &&
                                        key !== 'monthlydate' &&
                                            key !== 'monthlyfrequency' &&
                                                key !== 'startingmonth' &&
                                                    key !== 'startingyear') {
                                delete cswPublicRet.rateInterval[key];
                            }
                        });
                        if (cswPrivateVar.rateType === Csw.enums.rateIntervalTypes.MonthlyByDate) {
                            delete cswPublicRet.rateInterval.monthlyday;
                            delete cswPublicRet.rateInterval.monthlyweek;
                            cswPublicRet.rateInterval.monthlydate = monthlyDateSelect.find(':selected').val();
                        } else {
                            delete cswPublicRet.rateInterval.monthlydate;
                            cswPublicRet.rateInterval.monthlyweek = monthlyWeekSelect.find(':selected').val();
                        }

                        cswPublicRet.rateInterval.ratetype = cswPrivateVar.rateType;
                        cswPublicRet.rateInterval.dateformat = cswPrivateVar.dateFormat;
                        cswPublicRet.rateInterval.monthlyfrequency = monthlyRateSelect.find(':selected').val();
                        cswPublicRet.rateInterval.startingmonth = startOnMonth.find(':selected').val();
                        cswPublicRet.rateInterval.startingyear = startOnYear.find(':selected').val();
                        cswPrivateVar.saveRateInterval();
                    }

                    function makeMonthlyByDateSelect(monParent) {
                        var byDate = monParent.div(),
                            daysInMonth = ChemSW.makeSequentialArray(1, 31), selectedDay = '';

                        if (Csw.bool(cswPrivateVar.Multi)) {
                            selectedDay = Csw.enums.multiEditDefaultValue;
                            daysInMonth.unshift(Csw.enums.multiEditDefaultValue);
                        } else if (Csw.contains(cswPublicRet.rateInterval, 'monthlydate')) {
                            selectedDay = Csw.number(cswPublicRet.rateInterval.monthlydate, cswPrivateVar.now.getDay());
                        }

                        byDate.input({
                            ID: Csw.makeId(cswPrivateVar.ID, 'monthly', 'by_date'),
                            name: monthlyRadioId,
                            type: Csw.enums.inputTypes.radio,
                            onChange: function () {
                                Csw.tryExec(cswPrivateVar.onChange);
                                cswPrivateVar.rateType = ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                                saveMonthInterval();
                            },
                            value: Csw.enums.rateIntervalTypes.MonthlyByDate,
                            checked: cswPrivateVar.rateType === Csw.enums.rateIntervalTypes.MonthlyByDate
                        });
                        byDate.append('On Day of Month:&nbsp;');

                        monthlyDateSelect = byDate.select({
                            ID: Csw.makeId(cswPrivateVar.ID, 'monthly', 'date'),
                            onChange: function () {
                                Csw.tryExec(cswPrivateVar.onChange);
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

                        if (Csw.bool(cswPrivateVar.Multi)) {
                            frequency.unshift(Csw.enums.multiEditDefaultValue);
                            selected = Csw.enums.multiEditDefaultValue;
                        } else {
                            selected = Csw.number(cswPublicRet.rateInterval.monthlyfrequency, 1);
                        }

                        monthlyRateSelect = divEvery.select({
                            ID: Csw.makeId(cswPrivateVar.ID, 'monthly', 'rate'),
                            onChange: function () {
                                Csw.tryExec(cswPrivateVar.onChange);
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
                            monthlyWeekId = Csw.makeId(cswPrivateVar.ID, 'monthly', 'week'),
                            monthlyByDayId = Csw.makeId(cswPrivateVar.ID, 'monthly', 'by_day'),
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
                                Csw.tryExec(cswPrivateVar.onChange);
                                cswPrivateVar.rateType = ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                                saveMonthInterval();
                            },
                            value: Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay,
                            checked: cswPrivateVar.rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay
                        });

                        divByDay.append('Every&nbsp;');

                        if (cswPrivateVar.Multi) {
                            weeksInMonth.unshift({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                            selected = Csw.enums.multiEditDefaultValue;
                        } else {
                            selected = Csw.number(cswPublicRet.rateInterval.monthlyweek, 1);
                        }

                        monthlyWeekSelect = divByDay.select({
                            ID: monthlyWeekId,
                            values: weeksInMonth,
                            selected: selected,
                            onChange: function () {
                                Csw.tryExec(cswPrivateVar.onChange);
                                saveMonthInterval();
                            }
                        });
                        divByDay.br();

                        cswPrivateVar.monthlyWeekPicker(cswPrivateVar.onChange, true, monthlyDayPickerId, divByDay);
                        return divByDay;
                    }

                    function makeStartOnSelects(monParent) {
                        var divStartOn = monParent.div(),
                            monthsInYear = ChemSW.makeSequentialArray(1, 12),
                            year = cswPrivateVar.now.getFullYear(),
                            yearsToAllow = ChemSW.makeSequentialArray(year - 10, year + 10),
                            selectedMonth, selectedYear;

                        divStartOn.br();
                        divStartOn.append('Starting On:&nbsp;');

                        if (cswPrivateVar.Multi) {
                            monthsInYear.unshift(Csw.enums.multiEditDefaultValue);
                            yearsToAllow.unshift(Csw.enums.multiEditDefaultValue);
                            selectedMonth = Csw.enums.multiEditDefaultValue;
                            selectedYear = Csw.enums.multiEditDefaultValue;
                        } else {
                            selectedMonth = Csw.number(cswPublicRet.rateInterval.startingmonth, (cswPrivateVar.now.getMonth() + 1));
                            selectedYear = Csw.number(cswPublicRet.rateInterval.startingyear, year);
                        }

                        startOnMonth = divStartOn.select({
                            ID: Csw.makeId(cswPrivateVar.ID, 'monthly', 'startMonth'),
                            values: monthsInYear,
                            selected: selectedMonth,
                            onChange: function () {
                                Csw.tryExec(cswPrivateVar.onChange);
                                saveMonthInterval();
                            }
                        });

                        //divStartOn.append('/');

                        startOnYear = divStartOn.select({
                            ID: Csw.makeId(cswPrivateVar.ID, 'monthly', 'startYear'),
                            values: yearsToAllow,
                            selected: selectedYear,
                            onChange: function () {
                                Csw.tryExec(cswPrivateVar.onChange);
                                saveMonthInterval();
                            }
                        });
                        return divStartOn;
                    }

                    if (false === monthlyPickerComplete) {
                        ret = cswPrivateVar.pickerCell.div();
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

            cswPrivateVar.makeYearlyDatePicker = (function () {
                var yearlyDatePickerComplete = false,
                    retDiv, yearlyDate;
                return function () {

                    function saveYearInterval() {
                        Csw.each(cswPublicRet.rateInterval, function (prop, key) {
                            if (key !== 'dateformat' &&
                                key !== 'ratetype' &&
                                    key !== 'yearlydate') {
                                delete cswPublicRet.rateInterval[key];
                            }
                        });

                        cswPublicRet.rateInterval.ratetype = cswPrivateVar.rateType;
                        cswPublicRet.rateInterval.dateformat = cswPrivateVar.dateFormat;
                        if (yearlyDate) {
                            cswPublicRet.rateInterval.yearlydate = yearlyDate.val();
                        } else {
                            cswPublicRet.rateInterval.yearlydate = {};
                        }
                        cswPublicRet.rateInterval.yearlydate.dateformat = cswPrivateVar.dateFormat;
                        cswPrivateVar.saveRateInterval();
                    }

                    if (false === yearlyDatePickerComplete) {
                        retDiv = cswPrivateVar.pickerCell.div();

                        var yearlyStartDate = '';

                        if (Csw.bool(cswPrivateVar.Multi)) {
                            yearlyStartDate = Csw.enums.multiEditDefaultValue;
                        } else if (Csw.contains(cswPublicRet.rateInterval, 'yearlydate')) {
                            yearlyStartDate = Csw.string(cswPublicRet.rateInterval.yearlydate.date, cswPrivateVar.now.toLocaleDateString());
                        }
                        if (Csw.isNullOrEmpty(yearlyStartDate)) {
                            yearlyStartDate = cswPrivateVar.nowString;
                            cswPublicRet.rateInterval.yearlydate = { date: cswPrivateVar.nowString, dateformat: cswPrivateVar.dateFormat };
                        }

                        retDiv.append('Every Year, Starting On: ').br();

                        yearlyDate = retDiv.div().dateTimePicker({
                            ID: Csw.makeId(cswPrivateVar.ID, 'yearly', 'sd'),
                            Date: yearlyStartDate,
                            DateFormat: cswPrivateVar.dateFormat,
                            DisplayMode: 'Date',
                            ReadOnly: cswPrivateVar.ReadOnly,
                            Required: cswPrivateVar.Required,
                            onChange: function () {
                                Csw.tryExec(cswPrivateVar.onChange);
                                saveYearInterval();
                            }
                        });

                        saveYearInterval();

                        yearlyDatePickerComplete = true;
                    } // if (false === yearlyDatePickerComplete)
                    return retDiv.addClass('CswFieldTypeTimeInterval_Div');
                };
            } ());

            cswPrivateVar.makeRateType = function (table) {

                function onChange(newRateType) {
                    Csw.tryExec(cswPrivateVar.onChange);
                    cswPrivateVar.rateType = newRateType;
                    cswPublicRet.rateInterval.ratetype = cswPrivateVar.rateType;

                    if (false === Csw.isNullOrEmpty(cswPrivateVar.divWeekly, true)) {
                        cswPrivateVar.divWeekly.hide();
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivateVar.divMonthly, true)) {
                        cswPrivateVar.divMonthly.hide();
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivateVar.divYearly, true)) {
                        cswPrivateVar.divYearly.hide();
                    }

                    switch (newRateType) {
                        case Csw.enums.rateIntervalTypes.WeeklyByDay:
                            if (cswPrivateVar.divWeekly) {
                                cswPrivateVar.divWeekly.show();
                            }
                            break;
                        case Csw.enums.rateIntervalTypes.MonthlyByDate:
                            if (cswPrivateVar.divMonthly) {
                                cswPrivateVar.divMonthly.show();
                            }
                            break;
                        case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                            if (cswPrivateVar.divMonthly) {
                                cswPrivateVar.divMonthly.show();
                            }
                            break;
                        case Csw.enums.rateIntervalTypes.YearlyByDate:
                            if (cswPrivateVar.divYearly) {
                                cswPrivateVar.divYearly.show();
                            }
                            break;
                    }

                    cswPrivateVar.saveRateInterval();
                }

                var subTable = table.cell(2, 1).table();

                //Weekly
                subTable.cell(1, 2).span({ text: '&nbsp;Weekly' });
                subTable.cell(1, 1).input({
                    ID: Csw.makeId(cswPrivateVar.ID, 'type', 'weekly'),
                    name: Csw.makeId(cswPrivateVar.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'weekly',
                    checked: cswPrivateVar.rateType === Csw.enums.rateIntervalTypes.WeeklyByDay,
                    onClick: function () {
                        onChange(Csw.enums.rateIntervalTypes.WeeklyByDay);
                        cswPrivateVar.divWeekly = cswPrivateVar.divWeekly || cswPrivateVar.weeklyWeekPicker(cswPrivateVar.onChange, false);
                    }
                });

                //Monthly
                subTable.cell(2, 2).span({ text: '&nbsp;Monthly' });
                subTable.cell(2, 1).input({
                    ID: Csw.makeId(cswPrivateVar.ID, 'type', 'monthly'),
                    name: Csw.makeId(cswPrivateVar.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'monthly',
                    checked: cswPrivateVar.rateType === Csw.enums.rateIntervalTypes.MonthlyByDate || cswPrivateVar.rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay,
                    onClick: function () {
                        onChange(Csw.enums.rateIntervalTypes.MonthlyByDate);
                        cswPrivateVar.divMonthly = cswPrivateVar.divMonthly || cswPrivateVar.makeMonthlyPicker();
                    }
                });

                //Yearly
                subTable.cell(3, 2).span({ text: '&nbsp;Yearly' });
                subTable.cell(3, 1).input({
                    ID: Csw.makeId(cswPrivateVar.ID, 'type', 'yearly'),
                    name: Csw.makeId(cswPrivateVar.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'yearly',
                    checked: cswPrivateVar.rateType === Csw.enums.rateIntervalTypes.YearlyByDate,
                    onClick: function () {
                        onChange(Csw.enums.rateIntervalTypes.YearlyByDate);
                        cswPrivateVar.divYearly = cswPrivateVar.divYearly || cswPrivateVar.makeYearlyDatePicker();
                    }
                });
            };

            (function () {
                cswPrivateVar.interval = cswParent.div({
                    ID: cswPrivateVar.ID
                });
                cswPublicRet = Csw.dom({}, cswPrivateVar.interval);
                //Csw.literals.factory(cswPrivateVar.$parent, cswPublicRet);
                cswPublicRet.rateInterval = {};
                
                var propVals = cswPrivateVar.propVals,
                    textValue,
                    table;

                if (cswPrivateVar.Multi) {
                    //cswPublicRet.rateInterval = Csw.enums.multiEditDefaultValue;
                    textValue = Csw.enums.multiEditDefaultValue;
                    cswPrivateVar.rateType = Csw.enums.rateIntervalTypes.WeeklyByDay;
                } else {
                    $.extend(true, cswPublicRet.rateInterval, propVals.Interval.rateintervalvalue);
                    textValue = Csw.string(propVals.Interval.text).trim();
                    cswPrivateVar.rateType = cswPublicRet.rateInterval.ratetype;
                }
                cswPrivateVar.dateFormat = Csw.string(cswPublicRet.rateInterval.dateformat, 'M/d/yyyy');
                cswPublicRet.interval = cswPrivateVar.interval.div({
                    ID: Csw.makeId(cswPrivateVar.ID, 'cswTimeInterval')
                });

                //Page Components
                cswPublicRet.interval.span({
                    ID: Csw.makeId(cswPrivateVar.ID, 'textvalue'),
                    text: textValue
                });
                table = cswPrivateVar.interval.table({
                    ID: Csw.makeId(cswPrivateVar.ID, 'tbl'),
                    cellspacing: 5
                });

                cswPrivateVar.makeRateType(table);

                cswPrivateVar.pickerCell = table.cell(1, 3).propDom('rowspan', '3');

                // Set selected values
                switch (cswPrivateVar.rateType) {
                    case Csw.enums.rateIntervalTypes.WeeklyByDay:
                        cswPrivateVar.divWeekly = cswPrivateVar.weeklyWeekPicker(cswPrivateVar.onChange, false);
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByDate:
                        cswPrivateVar.divMonthly = cswPrivateVar.makeMonthlyPicker();
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                        cswPrivateVar.divMonthly = cswPrivateVar.makeMonthlyPicker();
                        break;
                    case Csw.enums.rateIntervalTypes.YearlyByDate:
                        cswPrivateVar.divYearly = cswPrivateVar.makeYearlyDatePicker();
                        break;
                } // switch(RateType)


            } ());

            cswPublicRet.validateRateInterval = function () {
                var retVal = false, errorString = '';
                switch (cswPrivateVar.rateType) {
                    case Csw.enums.rateIntervalTypes.WeeklyByDay:
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'startingdate') ||
                        false === Csw.contains(cswPublicRet.rateInterval.startingdate, 'date') ||
                            Csw.isNullOrEmpty(cswPublicRet.rateInterval.startingdate.date)) {
                            errorString += 'Cannot add a Weekly time interval without a starting date. ';
                        }
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'weeklyday') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.weeklyday)) {
                            errorString += 'Cannot add a Weekly time interval without at least one weekday selected. ';
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByDate:
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'monthlydate') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.monthlydate)) {
                            errorString += 'Cannot add a Monthly time interval without an \'On Day of Month\' selected. ';
                        }
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.monthlyfrequency)) {
                            errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                        }
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.startingmonth)) {
                            errorString += 'Cannot add a Monthly time interval without a Starting Month selected. ';
                        }
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.startingyear)) {
                            errorString += 'Cannot add a Monthly time interval without a Starting Year selected. ';
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'monthlyfrequency') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.monthlyfrequency)) {
                            errorString += 'Cannot add a Monthly time interval without a frequency selected. ';
                        }
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'monthlyday') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.monthlyday)) {
                            errorString += 'Cannot add a Monthly time interval without a Weekday selected. ';
                        }
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'monthlyweek') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.monthlyweek)) {
                            errorString += 'Cannot add a Monthly time interval without a Weekly frequency selected. ';
                        }
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'startingmonth') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.startingmonth)) {
                            errorString += 'Cannot add a Monthly time interval without a starting month selected. ';
                        }
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'startingyear') ||
                        Csw.isNullOrEmpty(cswPublicRet.rateInterval.startingyear)) {
                            errorString += 'Cannot add a Monthly time interval without a starting year selected. ';
                        }
                        break;
                    case Csw.enums.rateIntervalTypes.YearlyByDate:
                        if (false === Csw.contains(cswPublicRet.rateInterval, 'yearlydate') ||
                        false === Csw.contains(cswPublicRet.rateInterval.yearlydate, 'date') ||
                            Csw.isNullOrEmpty(cswPublicRet.rateInterval.yearlydate.date)) {
                            errorString += 'Cannot addd a Yearly time interval without a starting date. ';
                        }
                        break;
                }
                if (false === Csw.isNullOrEmpty(errorString)) {
                    retVal = Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, errorString);
                }
                return retVal;
            };

            cswPublicRet.rateType = function () {
                return cswPrivateVar.rateType;
            };
            cswPublicRet.val = function () {
                return cswPublicRet.rateInterval;
            };

            return cswPublicRet;
        });


} ());