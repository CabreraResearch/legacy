/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.controls.timeInterval = Csw.controls.timeInterval ||
        Csw.controls.register('timeInterval', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: '',
                rateIntervalValue: {
                    text: '',
                    ratetype: '',
                    startingdate: {
                        date: '',
                        time: '',
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
                isRequired: false,
                onChange: null,
                useEditButton: true,

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
                    name: 'weekdaypicker',
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
                            name: p.IDPrefix,
                            type: type,
                            onChange: dayChange,
                            value: i,
                            checked: Csw.contains(p.selectedWeekDays, cswPrivate.weekDayDef[i - 1])
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
                    name: cswPrivate.name + '_type',
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
                    name: cswPrivate.name + '_type',
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
                    name: cswPrivate.name + '_type',
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
                    name: cswPrivate.name + '_type',
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
                    hoursSelect, startingDatePicker;

                cswPrivate.divHourly = parent.div({
                    name: cswPrivate.name + '_divhourly',
                    cssclass: 'CswFieldTypeTimeInterval_Div'
                });

                // Hours
                cswPrivate.divHourly.append('Every ');
                hoursSelect = cswPrivate.divHourly.select({
                    name: cswPrivate.name + '_hourly_rate',
                    onChange: function () {
                        cswPublic.rateInterval.hours = hoursSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    },
                    values: hourOptions,
                    selected: Csw.number(cswPublic.rateInterval.hours)
                });
                cswPrivate.divHourly.append(' hour(s)');

                cswPrivate.divHourly.br();

                // Starting Date
                cswPrivate.divHourly.append('Starting On:');
                startingDatePicker = cswPrivate.divHourly.dateTimePicker({
                    name: cswPrivate.name + '_hourly_sd',
                    Date: Csw.string(cswPublic.rateInterval.startingdate.date),
                    Time: Csw.string(cswPublic.rateInterval.startingdate.time),
                    DateFormat: cswPublic.rateInterval.startingdate.dateformat,
                    DisplayMode: 'DateTime',
                    ReadOnly: cswPrivate.ReadOnly,
                    isRequired: cswPrivate.isRequired,
                    onChange: function () {
                        cswPublic.rateInterval.startingdate.date = startingDatePicker.val().date;
                        cswPublic.rateInterval.startingdate.time = startingDatePicker.val().time;
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
            }; // makeHourlyDiv



            cswPrivate.makeWeeklyDiv = function (parent) {
                var weeklyTable, startingDatePicker;

                cswPrivate.divWeekly = parent.div({
                    name: cswPrivate.name + '_divweekly',
                    cssclass: 'CswFieldTypeTimeInterval_Div'
                });

                // Weekday picker
                weeklyTable = cswPrivate.divWeekly.table({
                    name: cswPrivate.name + '_weeklytbl',
                    cellalign: 'center',
                    FirstCellRightAlign: true
                });

                weeklyTable.cell(1, 1).text('Every:');
                weeklyTable.cell(2, 1).text('Starting On:');

                cswPrivate.makeWeekDayPicker({
                    parent: weeklyTable.cell(1, 2),
                    IDPrefix: cswPrivate.name + 'weeklyday',
                    selectedWeekDays: cswPublic.rateInterval.weeklyday.split(','),
                    useRadio: false,
                    onChange: function (selecteddays) {
                        cswPublic.rateInterval.weeklyday = selecteddays.join(',');
                        Csw.tryExec(cswPrivate.onChange);
                    }
                }); // makeWeekDayPicker()

                // Starting Date
                startingDatePicker = weeklyTable.cell(2, 2).dateTimePicker({
                    name: cswPrivate.name + '_weekly_sd',
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
                    everySpan,
                    monthlyRadioId = cswPrivate.name + '_monthly', 
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
                    name: cswPrivate.name + '_divmonthly',
                    cssclass: 'CswFieldTypeTimeInterval_Div'
                });

                // Monthly Frequency
                everySpan = cswPrivate.divMonthly.span();
                everySpan.append('Every ');
                monthlyRateSelect = everySpan.select({
                    name: cswPrivate.name + '_monthly_rate',
                    onChange: function () {
                        cswPublic.rateInterval.monthlyfrequency = monthlyRateSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    },
                    values: frequency,
                    selected: Csw.number(cswPublic.rateInterval.monthlyfrequency)
                });
                everySpan.append(' Month(s)');

                cswPrivate.divMonthly.br();

                // Monthly By Date
                cswPrivate.divMonthly.input({
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
                    name: cswPrivate.name + '_monthly_date',
                    onChange: function () {
                        cswPublic.rateInterval.monthlydate = monthlyDateSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    },
                    values: daysInMonth,
                    selected: Csw.number(cswPublic.rateInterval.monthlydate)
                });

                cswPrivate.divMonthly.br();

                // Monthly by Week and Day
                cswPrivate.divMonthly.input({
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
                monthlyWeekSelect = cswPrivate.divMonthly.select({
                    name: cswPrivate.name + '_monthly_week',
                    values: weeksInMonth,
                    selected: Csw.number(cswPublic.rateInterval.monthlyweek),
                    onChange: function () {
                        cswPublic.rateInterval.monthlyweek = monthlyWeekSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
                cswPrivate.divMonthly.br();
                cswPrivate.makeWeekDayPicker({
                    parent: cswPrivate.divMonthly,
                    IDPrefix: cswPrivate.name + 'monthlyday',
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
                startingMonthSelect = cswPrivate.divMonthly.select({
                    name: cswPrivate.name + '_monthly_startMonth',
                    values: monthsInYear,
                    selected: Csw.number(cswPublic.rateInterval.startingmonth),
                    onChange: function () {
                        cswPublic.rateInterval.startingmonth = startingMonthSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
                startingYearSelect = cswPrivate.divMonthly.select({
                    name: cswPrivate.name + '_monthly_startYear',
                    values: yearsToAllow,
                    selected: Csw.number(cswPublic.rateInterval.startingyear),
                    onChange: function () {
                        cswPublic.rateInterval.startingyear = startingYearSelect.val();
                        Csw.tryExec(cswPrivate.onChange);
                    }
                });
            }; // makeMonthlyDiv

            cswPrivate.makeYearlyDiv = function (parent) {
                var yearPicker;

                cswPrivate.divYearly = parent.div({
                    name: cswPrivate.name + '_divyearly',
                    cssclass: 'CswFieldTypeTimeInterval_Div'
                });

                cswPrivate.divYearly.append('Every Year, Starting On: ').br();

                yearPicker = cswPrivate.divYearly.dateTimePicker({
                    name: cswPrivate.name + '_yearly_sd',
                    Date: cswPublic.rateInterval.yearlydate.date,
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


            cswPrivate.setDefaults = function () {
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


            cswPrivate.make = function () {
                cswPrivate.textValueSpan.hide();

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
            }; // make()

            // constructor
            (function () {
                var textValue;
                cswPublic.rateInterval = cswPrivate.rateIntervalValue;

                cswPrivate.now = new Date();

                cswPrivate.nowString = $.datepicker.formatDate(cswPublic.rateInterval.startingdate.dateformat, cswPrivate.now);

                textValue = Csw.string(cswPrivate.rateIntervalValue.text).trim();
                if (Csw.isNullOrEmpty(textValue)) {
                    textValue = '[none set]';
                }

                cswPrivate.setDefaults();

                cswPrivate.div = cswParent.div({ name: cswPrivate.name });

                cswPrivate.textValueSpan = cswPrivate.div.span({ text: textValue }).css({ paddingRight: '5px' });

                if (false === cswPrivate.ReadOnly) {
                    if (cswPrivate.useEditButton) {
                        cswPrivate.editButton = cswPrivate.div.icon({
                        name: 'editbtn',
                            iconType: Csw.enums.iconType.pencil,
                            isButton: true,
                            size: 16,
                            onClick: function () {
                                cswPrivate.editButton.hide();
                                cswPrivate.make();
                            } // onClick
                        }); // editButton
                    } // if(cswPrivate.useEditButton)
                    else {
                        cswPrivate.make();
                    }
                } // if(false === cswPrivate.ReadOnly)

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