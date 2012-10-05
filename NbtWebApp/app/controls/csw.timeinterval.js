/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.controls.timeInterval = Csw.controls.timeInterval ||
        Csw.controls.register('timeInterval', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                ID: '',
                rateIntervalValue: {
                    text: '',
                    ratetype: '',
                    startingdate: {
                        date: '',
                        dateformat: ''
                    },
                    startingmonth: '',
                    startingyear: '',
                    hours: '',
                    weeklyday: '',
                    monthlyfrequency: '',
                    monthlydate: '',
                    monthlyweek: '',
                    monthlyday: '',
                    yearlydate: {
                        date: '',
                        dateformat: ''
                    }
                },
                Multi: false,
                ReadOnly: false,
                Required: false,
                onChange: null,

                divHourly: '',
                divWeekly: '',
                divMonthly: '',
                divYearly: '',
                weekDayDef: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']
            };
            if (options) Csw.extend(cswPrivate, options, true);

            // link rateIntervalValue directly (our version of databinding)
            cswPrivate.rateIntervalValue = options.rateIntervalValue;

            var cswPublic = {
                rateInterval: {}
            };



            cswPrivate.makeWeekDayPicker = function (params) {
                var i, type;
                var p = {
                    parent: null,
                    IDPrefix: '',
                    selectedWeekDays: [],
                    useRadio: false,
                    onChange: null
                };
                if (params) Csw.extend(p, params);

                function dayChange(val, checkbox) {
                    var day = cswPrivate.weekDayDef[val - 1];
                    if (p.useRadio) {
                        p.selectedWeekDays = [];
                    }
                    if (checkbox.checked()) {
                        if (false === Csw.contains(p.selectedWeekDays, day)) {
                            p.selectedWeekDays.push(day);
                        }
                    } else {
                        p.selectedWeekDays.splice(p.selectedWeekDays.indexOf(day), 1);
                        if (p.selectedWeekDays.length === 0) {
                            checkbox.click();
                        }
                    }
                    Csw.tryExec(p.onChange, p.selectedWeekDays);
                }

                var pickerTable = p.parent.table({
                    ID: Csw.makeId(p.IDPrefix, 'weekdaypicker'),
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
                    if (p.useRadio) {
                        type = Csw.enums.inputTypes.radio;
                    }
                    pickerTable.cell(2, i)
                        .input({
                            ID: p.IDPrefix + '_' + i,
                            name: p.IDPrefix,
                            type: type,
                            onChange: dayChange,
                            value: i,
                            checked: (false === cswPrivate.Multi && Csw.contains(p.selectedWeekDays, cswPrivate.weekDayDef[i - 1]))
                        });
                } //for (i = 1; i <= 7; i += 1)

                return pickerTable;
            }; // makeWeekDayPicker



            cswPrivate.hideDivs = function () {
                cswPrivate.divHourly.hide();
                cswPrivate.divWeekly.hide();
                cswPrivate.divMonthly.hide();
                cswPrivate.divYearly.hide();
            }; // hideDivs()


            cswPrivate.makeRateTypeDiv = function (parent) {
                var subTable = parent.table();

                //Hourly
                subTable.cell(1, 2).span({ text: '&nbsp;Hourly' });
                subTable.cell(1, 1).input({
                    ID: Csw.makeId(cswPrivate.ID, 'type', 'hourly'),
                    name: Csw.makeId(cswPrivate.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'hourly',
                    checked: cswPublic.rateInterval.ratetype === Csw.enums.rateIntervalTypes.Hourly,
                    onClick: function () {
                        cswPublic.rateInterval.ratetype = Csw.enums.rateIntervalTypes.Hourly;
                        cswPrivate.hideDivs();
                        cswPrivate.divHourly.show();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });

                //Weekly
                subTable.cell(2, 2).span({ text: '&nbsp;Weekly' });
                subTable.cell(2, 1).input({
                    ID: Csw.makeId(cswPrivate.ID, 'type', 'weekly'),
                    name: Csw.makeId(cswPrivate.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'weekly',
                    checked: cswPublic.rateInterval.ratetype === Csw.enums.rateIntervalTypes.WeeklyByDay,
                    onClick: function () {
                        cswPublic.rateInterval.ratetype = Csw.enums.rateIntervalTypes.WeeklyByDay;
                        cswPrivate.hideDivs();
                        cswPrivate.divWeekly.show();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });

                //Monthly
                subTable.cell(3, 2).span({ text: '&nbsp;Monthly' });
                subTable.cell(3, 1).input({
                    ID: Csw.makeId(cswPrivate.ID, 'type', 'monthly'),
                    name: Csw.makeId(cswPrivate.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'monthly',
                    checked: (cswPublic.rateInterval.ratetype === Csw.enums.rateIntervalTypes.MonthlyByDate || cswPublic.rateInterval.ratetype === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay),
                    onClick: function () {
                        cswPublic.rateInterval.ratetype = Csw.enums.rateIntervalTypes.MonthlyByDate;
                        cswPrivate.hideDivs();
                        cswPrivate.divMonthly.show();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });

                //Yearly
                subTable.cell(4, 2).span({ text: '&nbsp;Yearly' });
                subTable.cell(4, 1).input({
                    ID: Csw.makeId(cswPrivate.ID, 'type', 'yearly'),
                    name: Csw.makeId(cswPrivate.ID, 'type', '', '', false),
                    type: Csw.enums.inputTypes.radio,
                    value: 'yearly',
                    checked: cswPublic.rateInterval.ratetype === Csw.enums.rateIntervalTypes.YearlyByDate,
                    onClick: function () {
                        cswPublic.rateInterval.ratetype = Csw.enums.rateIntervalTypes.YearlyByDate;
                        cswPrivate.hideDivs();
                        cswPrivate.divYearly.show();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
            }; // makeRateTypeDiv


            cswPrivate.makeHourlyDiv = function (parent) {
                var hourOptions = ChemSW.makeSequentialArray(1, 24),
                    hoursSelect, startingDatePicker,
                    selectedHours;

                cswPrivate.divHourly = parent.div({
                    ID: Csw.makeId(cswPrivate.ID, 'divhourly'),
                    cssclass: 'CswFieldTypeTimeInterval_Div'
                });

                // Hours
                if (Csw.bool(cswPrivate.Multi)) {
                    frequency.unshift(Csw.enums.multiEditDefaultValue);
                    selectedHours = Csw.enums.multiEditDefaultValue;
                } else {
                    selectedHours = Csw.number(cswPublic.rateInterval.hours, 1);
                }
                cswPrivate.divHourly.append('Every ');
                hoursSelect = cswPrivate.divHourly.select({
                    ID: Csw.makeId(cswPrivate.ID, 'hourly', 'rate'),
                    onChange: function () {
                        cswPublic.rateInterval.hours = hoursSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    },
                    values: hourOptions,
                    selected: selectedHours
                });
                cswPrivate.divHourly.append(' hour(s)');

                cswPrivate.divHourly.br();

                // Starting Date
                cswPrivate.divHourly.append('Starting On:');
                startingDatePicker = cswPrivate.divHourly.dateTimePicker({
                    ID: Csw.makeId(cswPrivate.ID, 'hourly', 'sd'),
                    Date: Csw.string(cswPublic.rateInterval.startingdate.date),
                    DateFormat: cswPublic.rateInterval.startingdate.dateformat,
                    DisplayMode: 'Date',
                    ReadOnly: cswPrivate.ReadOnly,
                    Required: cswPrivate.Required,
                    onChange: function () {
                        cswPublic.rateInterval.startingdate.date = startingDatePicker.val().date;
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
            }; // makeHourlyDiv



            cswPrivate.makeWeeklyDiv = function (parent) {
                var weeklyTable, startingDatePicker;

                cswPrivate.divWeekly = parent.div({
                    ID: Csw.makeId(cswPrivate.ID, 'divweekly'),
                    cssclass: 'CswFieldTypeTimeInterval_Div'
                });

                // Weekday picker
                weeklyTable = cswPrivate.divWeekly.table({
                    ID: Csw.makeId(cswPrivate.ID, 'weeklytbl'),
                    cellalign: 'center',
                    FirstCellRightAlign: true
                });

                weeklyTable.cell(1, 1).text('Every:');
                weeklyTable.cell(2, 1).text('Starting On:');

                cswPrivate.makeWeekDayPicker({
                    parent: weeklyTable.cell(1, 2),
                    IDPrefix: cswPrivate.ID + 'weeklyday',
                    selectedWeekDays: cswPublic.rateInterval.weeklyday.split(','),
                    useRadio: false,
                    onChange: function (selecteddays) {
                        cswPublic.rateInterval.weeklyday = selecteddays.join(',');
                        Csw.tryExec(cswPrivate.onChange);
                    }
                }); // makeWeekDayPicker()

                // Starting Date
                startingDatePicker = weeklyTable.cell(2, 2).dateTimePicker({
                    ID: Csw.makeId(cswPrivate.ID, 'weekly', 'sd'),
                    Date: Csw.string(cswPublic.rateInterval.startingdate.date),
                    DateFormat: cswPublic.rateInterval.startingdate.dateformat,
                    DisplayMode: 'Date',
                    ReadOnly: cswPrivate.ReadOnly,
                    Required: cswPrivate.Required,
                    onChange: function () {
                        cswPublic.rateInterval.startingdate.date = startingDatePicker.val().date;
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
            }; // makeWeeklyDiv


            cswPrivate.makeMonthlyDiv = function (parent) {
                var monthlyRateSelect, monthlyDateSelect, monthlyWeekSelect, startingMonthSelect, startingYearSelect,
                    selectedFrequency, selectedDay, selectedWeek, selectedMonth, selectedYear,
                    everySpan,
                    monthlyRadioId = Csw.makeId(cswPrivate.ID, 'monthly'),
                    monthlyDayPickerId = Csw.makeId(cswPrivate.ID, 'monthly', 'day'),
                    daysInMonth = ChemSW.makeSequentialArray(1, 31),
                    monthsInYear = ChemSW.makeSequentialArray(1, 12),
                    year = cswPrivate.now.getFullYear(),
                    yearsToAllow = ChemSW.makeSequentialArray(year - 10, year + 10),
                    frequency = ChemSW.makeSequentialArray(1, 12),
                    weeksInMonth = [
                        { value: 1, display: 'First:' },
                        { value: 2, display: 'Second:' },
                        { value: 3, display: 'Third:' },
                        { value: 4, display: 'Fourth:' }
                    ];

                cswPrivate.divMonthly = parent.div({
                    ID: Csw.makeId(cswPrivate.ID, 'divmonthly'),
                    cssclass: 'CswFieldTypeTimeInterval_Div'
                });

                // Monthly Frequency
                if (Csw.bool(cswPrivate.Multi)) {
                    frequency.unshift(Csw.enums.multiEditDefaultValue);
                    selectedFrequency = Csw.enums.multiEditDefaultValue;
                } else {
                    selectedFrequency = Csw.number(cswPublic.rateInterval.monthlyfrequency, 1);
                }
                everySpan = cswPrivate.divMonthly.span();
                everySpan.append('Every ');
                monthlyRateSelect = everySpan.select({
                    ID: Csw.makeId(cswPrivate.ID, 'monthly', 'rate'),
                    onChange: function () {
                        cswPublic.rateInterval.monthlyfrequency = monthlyRateSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    },
                    values: frequency,
                    selected: selectedFrequency
                });
                everySpan.append(' Month(s)');

                cswPrivate.divMonthly.br();

                if (Csw.bool(cswPrivate.Multi)) {
                    selectedDay = Csw.enums.multiEditDefaultValue;
                    daysInMonth.unshift(Csw.enums.multiEditDefaultValue);
                } else if (Csw.contains(cswPublic.rateInterval, 'monthlydate')) {
                    selectedDay = Csw.number(cswPublic.rateInterval.monthlydate, cswPrivate.now.getDay());
                }

                // Monthly By Date
                cswPrivate.divMonthly.input({
                    ID: Csw.makeId(cswPrivate.ID, 'monthly', 'by_date'),
                    name: monthlyRadioId,
                    type: Csw.enums.inputTypes.radio,
                    onChange: function () {
                        cswPublic.rateInterval.ratetype = Csw.enums.rateIntervalTypes.MonthlyByDate;
                        Csw.tryExec(cswPrivate.onChange);
                    },
                    value: Csw.enums.rateIntervalTypes.MonthlyByDate,
                    checked: cswPublic.rateInterval.rateType !== Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay    // check this by default unless MonthlyByWeekAndDay is set
                });
                cswPrivate.divMonthly.span({ text: 'On Day of Month:&nbsp;' });
                monthlyDateSelect = cswPrivate.divMonthly.select({
                    ID: Csw.makeId(cswPrivate.ID, 'monthly', 'date'),
                    onChange: function () {
                        cswPublic.rateInterval.monthlydate = monthlyDateSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    },
                    values: daysInMonth,
                    selected: selectedDay
                });

                cswPrivate.divMonthly.br();

                // Monthly by Week and Day
                cswPrivate.divMonthly.input({
                    ID: Csw.makeId(cswPrivate.ID, 'monthly', 'by_day'),
                    name: monthlyRadioId,
                    type: Csw.enums.inputTypes.radio,
                    onChange: function () {
                        cswPublic.rateInterval.ratetype = Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay;
                        Csw.tryExec(cswPrivate.onChange);
                    },
                    value: Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay,
                    checked: cswPublic.rateInterval.rateType === Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay
                });
                cswPrivate.divMonthly.append('Every&nbsp;');
                if (cswPrivate.Multi) {
                    weeksInMonth.unshift({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                    selectedWeek = Csw.enums.multiEditDefaultValue;
                } else {
                    selectedWeek = Csw.number(cswPublic.rateInterval.monthlyweek, 1);
                }
                monthlyWeekSelect = cswPrivate.divMonthly.select({
                    ID: Csw.makeId(cswPrivate.ID, 'monthly', 'week'),
                    values: weeksInMonth,
                    selected: selectedWeek,
                    onChange: function () {
                        cswPublic.rateInterval.monthlyweek = monthlyWeekSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
                cswPrivate.divMonthly.br();
                cswPrivate.makeWeekDayPicker({
                    parent: cswPrivate.divMonthly,
                    IDPrefix: cswPrivate.ID + 'monthlyday',
                    selectedWeekDays: cswPublic.rateInterval.monthlyday.split(','),
                    useRadio: true,
                    onChange: function (selectedDays) {
                        cswPublic.rateInterval.monthlyday = selectedDays.join(',');
                        Csw.tryExec(cswPrivate.onChange);
                    }
                }); // makeWeekDayPicker()
                cswPrivate.divMonthly.br();

                // Starting On
                cswPrivate.divMonthly.append('Starting On:&nbsp;');
                if (cswPrivate.Multi) {
                    monthsInYear.unshift(Csw.enums.multiEditDefaultValue);
                    yearsToAllow.unshift(Csw.enums.multiEditDefaultValue);
                    selectedMonth = Csw.enums.multiEditDefaultValue;
                    selectedYear = Csw.enums.multiEditDefaultValue;
                } else {
                    selectedMonth = Csw.number(cswPublic.rateInterval.startingmonth, (cswPrivate.now.getMonth() + 1));
                    selectedYear = Csw.number(cswPublic.rateInterval.startingyear, year);
                }
                startingMonthSelect = cswPrivate.divMonthly.select({
                    ID: Csw.makeId(cswPrivate.ID, 'monthly', 'startMonth'),
                    values: monthsInYear,
                    selected: selectedMonth,
                    onChange: function () {
                        cswPublic.rateInterval.startingmonth = startingMonthSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
                startingYearSelect = cswPrivate.divMonthly.select({
                    ID: Csw.makeId(cswPrivate.ID, 'monthly', 'startYear'),
                    values: yearsToAllow,
                    selected: selectedYear,
                    onChange: function () {
                        cswPublic.rateInterval.startingyear = startingYearSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
            }; // makeMonthlyDiv

            cswPrivate.makeYearlyDiv = function (parent) {
                var yearlyStartDate = '', yearPicker;

                cswPrivate.divYearly = parent.div({
                    ID: Csw.makeId(cswPrivate.ID, 'divyearly'),
                    cssclass: 'CswFieldTypeTimeInterval_Div'
                });


                if (Csw.bool(cswPrivate.Multi)) {
                    yearlyStartDate = Csw.enums.multiEditDefaultValue;
                } else {
                    yearlyStartDate = Csw.string(cswPublic.rateInterval.yearlydate.date);
                }

                cswPrivate.divYearly.append('Every Year, Starting On: ').br();

                yearPicker = cswPrivate.divYearly.dateTimePicker({
                    ID: Csw.makeId(cswPrivate.ID, 'yearly', 'sd'),
                    Date: yearlyStartDate,
                    DateFormat: cswPublic.rateInterval.yearlydate.dateformat,
                    DisplayMode: 'Date',
                    ReadOnly: cswPrivate.ReadOnly,
                    Required: cswPrivate.Required,
                    onChange: function () {
                        cswPublic.rateInterval.yearlydate.date = yearPicker.val().date;
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
            }; // makeYearlyDiv


            cswPrivate.setDefaults = function() {
                var doOnChange = false;

                if (Csw.isNullOrEmpty(cswPublic.rateInterval.ratetype)) {
                    cswPublic.rateInterval.ratetype = Csw.enums.rateIntervalTypes.WeeklyByDay;
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.startingdate)) {
                    cswPublic.rateInterval.startingdate = {
                        date: cswPrivate.nowString,
                        dateformat: cswPublic.rateInterval.startingdate.dateformat
                    };
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.startingmonth)) {
                    cswPublic.rateInterval.startingmonth = 1;
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.startingyear)) {
                    cswPublic.rateInterval.startingyear = cswPrivate.now.getYear();
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.weeklyday)) {
                    cswPublic.rateInterval.weeklyday = 'Monday';
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.hours)) {
                    cswPublic.rateInterval.hours = 24;
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.monthlyfrequency)) {
                    cswPublic.rateInterval.monthlyfrequency = 1;
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.monthlydate)) {
                    cswPublic.rateInterval.monthlydate = 1;
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.monthlyweek)) {
                    cswPublic.rateInterval.monthlyweek = 1;
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.monthlyday)) {
                    cswPublic.rateInterval.monthlyday = 'Monday';
                    doOnChange = true;
                }
                if (Csw.isNullOrEmpty(cswPublic.rateInterval.yearlydate)) {
                    cswPublic.rateInterval.yearlydate = {
                        date: cswPrivate.nowString,
                        dateformat: cswPublic.rateInterval.startingdate.dateformat
                    };
                    doOnChange = true;
                }
                if (doOnChange) {
                    Csw.tryExec(cswPrivate.onChange);
                }

            }; // cswPrivate.setDefaults

            // constructor
            (function () {
                var textValue;
                //Csw.extend(cswPublic.rateInterval, cswPrivate.rateIntervalValue, true);
                cswPublic.rateInterval = cswPrivate.rateIntervalValue;

                cswPrivate.now = new Date();
                //cswPrivate.nowString = (cswPrivate.now.getMonth() + 1) + '/' + cswPrivate.now.getDate() + '/' + cswPrivate.now.getFullYear();
                cswPrivate.nowString = $.datepicker.formatDate(cswPublic.rateInterval.startingdate.dateformat, cswPrivate.now);

                cswPrivate.setDefaults();

                cswPrivate.div = cswParent.div({ ID: cswPrivate.ID });

                if (cswPrivate.Multi) {
                    textValue = Csw.enums.multiEditDefaultValue;
                } else {
                    textValue = Csw.string(cswPrivate.rateIntervalValue.text).trim();
                }
                if (Csw.isNullOrEmpty(textValue)) {
                    textValue = '[none set]';
                }

                cswPrivate.textValueSpan = cswPrivate.div.span({ text: textValue }).css({ paddingRight: '5px' });

                cswPrivate.editButton = cswPrivate.div.icon({
                    ID: Csw.makeSafeId(cswPrivate.ID, 'editbtn'),
                    iconType: Csw.enums.iconType.pencil,
                    isButton: true,
                    size: 16,
                    onClick: function () {
                        cswPrivate.textValueSpan.hide();
                        cswPrivate.editButton.hide();

                        cswPrivate.table = cswPrivate.div.table({ cellspacing: 5 });
                        cswPrivate.makeRateTypeDiv(cswPrivate.table.cell(1, 1));

                        var divCell = cswPrivate.table.cell(1, 2);
                        cswPrivate.makeHourlyDiv(divCell);
                        cswPrivate.makeWeeklyDiv(divCell);
                        cswPrivate.makeMonthlyDiv(divCell);
                        cswPrivate.makeYearlyDiv(divCell);

                        cswPrivate.hideDivs();
                        switch (cswPublic.rateInterval.ratetype) {
                            case Csw.enums.rateIntervalTypes.Hourly:
                                cswPrivate.divHourly.show();
                                break;
                            case Csw.enums.rateIntervalTypes.WeeklyByDay:
                                cswPrivate.divWeekly.show();
                                break;
                            case Csw.enums.rateIntervalTypes.MonthlyByDate:
                                cswPrivate.divMonthly.show();
                                break;
                            case Csw.enums.rateIntervalTypes.MonthlyByWeekAndDay:
                                cswPrivate.divMonthly.show();
                                break;
                            case Csw.enums.rateIntervalTypes.YearlyByDate:
                                cswPrivate.divYearly.show();
                                break;
                        } // switch
                    } // onClick
                }); // editButton

            } ()); // constructor


            cswPublic.rateType = function () {
                return cswPublic.rateInterval.ratetype;
            };
            cswPublic.val = function () {
                return cswPublic.rateInterval;
            };

            return cswPublic;
        });
} ());