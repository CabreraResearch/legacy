/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />
/// <reference path="../tools/CswClientDb.js" />
/// <reference path="CswDateTimePicker.js" />

var CswTimeInterval = function (options) {
    "use strict";
    var o = {
        ID: '',
        $parent: '',
        values: {},
        Multi: false,
        ReadOnly: false,
        Required: false,
        onchange: null
    };
    if (options) {
        $.extend(true, o, options);
    }

    var now = new Date(),
        clientDb = new CswClientDb(),
        nowString = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear(),
        rateType, $WeeklyDiv, $MonthlyDiv, $YearlyDiv, dateFormat, rateInterval = {}, $pickerCell, $interval;

    var saveRateInterval = function() {
        clientDb.setItem(o.ID + '_rateIntervalSave', rateInterval);
    };
    
    var makeRateType = function ($table) {
        var $weeklyradiocell = $table.CswTable('cell', 1, 1),
            $weeklyradio = $weeklyradiocell.CswInput('init', {
                ID: o.ID + '_type_weekly',
                name: o.ID + '_type',
                type: CswInput_Types.radio,
                value: 'weekly'
            }).CswAttrDom('checked', (rateType === CswRateIntervalTypes.WeeklyByDay)),
            $monthlyradiocell = $table.CswTable('cell', 2, 1),
            $monthlyradio = $monthlyradiocell.CswInput('init', {
                ID: o.ID + '_type_monthly',
                name: o.ID + '_type',
                type: CswInput_Types.radio,
                value: 'monthly'
            }).CswAttrDom('checked', (rateType === CswRateIntervalTypes.MonthlyByDate || rateType === CswRateIntervalTypes.MonthlyByWeekAndDay)),
            $yearlyradiocell = $table.CswTable('cell', 3, 1),
            $yearlyradio = $yearlyradiocell.CswInput('init', {
                ID: o.ID + '_type_yearly',
                name: o.ID + '_type',
                type: CswInput_Types.radio,
                value: 'yearly'
            }).CswAttrDom('checked', (rateType === CswRateIntervalTypes.YearlyByDate));

        function onChange() {
            if (isFunction(o.onchange)) {
                o.onchange();
            }
            toggleIntervalDiv(rateType, $weeklyradio, $monthlyradio, $yearlyradio);
            saveRateInterval();
        }

        //Weekly
        $table.CswTable('cell', 1, 2).append('<span>&nbsp;Weekly</span>');
        $weeklyradio.click(function () {
            rateType = CswRateIntervalTypes.WeeklyByDay;
            rateInterval.ratetype = rateType;
            $('#' + o.ID + '_textvalue').text('Weekly');
            $WeeklyDiv = $WeeklyDiv || weeklyWeekPicker($pickerCell, o.onchange, false);
            onChange();
        });

        //Monthly
        $table.CswTable('cell', 2, 2).append('<span>&nbsp;Monthly</span>');
        $monthlyradio.click(function () {
            rateType = CswRateIntervalTypes.MonthlyByDate;
            rateInterval.ratetype = rateType;
            $('#' + o.ID + '_textvalue').text('Monthly');
            $MonthlyDiv = $MonthlyDiv || makeMonthlyPicker($pickerCell);
            onChange();
        });

        //Yearly
        $table.CswTable('cell', 3, 2).append('<span>&nbsp;Yearly</span>');
        $yearlyradio.click(function () {
            rateType = CswRateIntervalTypes.YearlyByDate;
            rateInterval.ratetype = rateType;
            $('#' + o.ID + '_textvalue').text('Yearly');
            $YearlyDiv = $YearlyDiv || makeYearlyDatePicker($pickerCell);
            onChange();
        });
    };

    var toggleIntervalDiv = function (interval, $weeklyradio, $monthlyradio, $yearlyradio) {

        if (abandonHope) {
            $weeklyradio.attr('checked', false);
            $monthlyradio.attr('checked', false);
            $yearlyradio.attr('checked', false);
        }
        if (false === isNullOrEmpty($WeeklyDiv, true)) {
            $WeeklyDiv.hide();
        }
        if (false === isNullOrEmpty($MonthlyDiv, true)) {
            $MonthlyDiv.hide();
        }
        if (false === isNullOrEmpty($YearlyDiv, true)) {
            $YearlyDiv.hide();
        }
        switch (interval) {
            case CswRateIntervalTypes.WeeklyByDay:
                $WeeklyDiv.show();
                if (abandonHope) {
                    $weeklyradio.attr('checked', true);
                }
                break;
            case CswRateIntervalTypes.MonthlyByDate:
                $MonthlyDiv.show();
                if (abandonHope) {
                    $monthlyradio.attr('checked', true);
                }
                break;
            case CswRateIntervalTypes.MonthlyByWeekAndDay:
                $MonthlyDiv.show();
                if (abandonHope) {
                    $monthlyradio.attr('checked', true);
                }
                break;
            case CswRateIntervalTypes.YearlyByDate:
                $YearlyDiv.show();
                if (abandonHope) {
                    $yearlyradio.attr('checked', true);
                }
                break;
        }
    };

    var weekDayDef = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

    var makeWeekDayPicker = function (thisRateType) {
        return (function () {
            var weeklyDayPickerComplete = false,
                $ret, weekdays, $startingDate,
                isWeekly = (thisRateType === CswRateIntervalTypes.WeeklyByDay),
                dayPropName = 'weeklyday';
            
            if(false === isWeekly) {
                dayPropName = 'monthlyday';
            }

            return function ($parent, onchange, useRadio, elemId) {
                function isChecked(day) {
                    var thisDay = weekDayDef[day - 1];
                    return false === o.Multi && contains(weekdays, thisDay);
                }
                
                function saveWeekInterval() {
                    if(isWeekly) {
                        each(rateInterval, function(prop, key) {
                            if (key !== 'dateformat' && key !== 'ratetype' && key !== 'startingdate' && key !== 'weeklyday') {
                                delete rateInterval[key];
                            }
                        });
                        rateInterval.startingdate = $startingDate.CswDateTimePicker('value');
                        rateInterval.startingdate.dateformat = dateFormat;
                    } 
                    rateInterval.ratetype = thisRateType;
                    rateInterval.dateformat = dateFormat;
                    rateInterval[dayPropName] = weekdays.join(',');
                    saveRateInterval();
                }

                if (false === weeklyDayPickerComplete) {
                    $ret = $('<div />').appendTo($parent);
                    var id = elemId || o.ID + '_weeklyday',
                        $picker, $table, i, type, $pickercell, weeklyStartDate,
                        $WeeklyTable = $ret.CswTable('init', {
                            ID: o.ID + '_weeklytbl',
                            cellalign: 'center',
                            FirstCellRightAlign: true
                        });

                    weekdays = tryParseString(rateInterval[dayPropName]).split(',');
                    
                    $picker = $WeeklyTable.CswTable('cell', 1, 2);
                    $table = $picker.CswTable('init', {
                        ID: id,
                        cellalign: 'center'
                    });

                    $table.CswTable('cell', 1, 1).append('Su');
                    $table.CswTable('cell', 1, 2).append('M');
                    $table.CswTable('cell', 1, 3).append('Tu');
                    $table.CswTable('cell', 1, 4).append('W');
                    $table.CswTable('cell', 1, 5).append('Th');
                    $table.CswTable('cell', 1, 6).append('F');
                    $table.CswTable('cell', 1, 7).append('Sa');
                    
                    for (i = 1; i <= 7; i += 1) {
                        type = CswInput_Types.checkbox;
                        if (useRadio) {
                            type = CswInput_Types.radio;
                        }
                        $pickercell = $table.CswTable('cell', 2, i);
                        $pickercell.CswInput('init', {
                                ID: id + '_' + i,
                                name: id,
                                type: type,
                                onChange: function() {
                                    if (isFunction(o.onchange)) {
                                        o.onchange();
                                    }
                                    var $this = $(this),
                                        day = weekDayDef[$this.val() - 1];
                                    if ($this.is(':checked')) {
                                        if (false === isWeekly) {
                                            weekdays = [];
                                        }
                                        if (false === contains(weekdays, day)) {
                                            weekdays.push(day);
                                        }
                                    } else {
                                        weekdays.slice(cswIndexOf(weekdays, day), 1);
                                    }
                                    saveWeekInterval();
                                },
                                value: i
                            })
                            .CswAttrDom('checked', isChecked(i));

                    } //for (i = 1; i <= 7; i += 1)
                    
                    //Starting Date
                    if(isWeekly) {
                        if (false === o.Multi && contains(rateInterval, 'startingdate')) {
                            weeklyStartDate = tryParseString(rateInterval.startingdate.date);
                        } 
                        if (isNullOrEmpty(weeklyStartDate)) {
                            rateInterval.startingdate = { date: nowString, dateformat: dateFormat };
                            saveRateInterval();
                        }
                        $WeeklyTable.CswTable('cell', 1, 1).append('Every:');
                        
                        $WeeklyTable.CswTable('cell', 2, 1).append('Starting On:');
                        $startingDate = $WeeklyTable.CswTable('cell', 2, 2)
                            .CswDateTimePicker('init', {
                                ID: o.ID + '_weekly_sd',
                                Date: weeklyStartDate,
                                DateFormat: dateFormat,
                                DisplayMode: 'Date',
                                ReadOnly: o.ReadOnly,
                                Required: o.Required,
                                OnChange: function() {
                                    if (isFunction(o.onchange)) {
                                        o.onchange();
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
        } ()); // makeWeekDayPicker()
    };

    var weeklyWeekPicker = makeWeekDayPicker(CswRateIntervalTypes.WeeklyByDay),
        monthlyWeekPicker = makeWeekDayPicker(CswRateIntervalTypes.MonthlyByWeekAndDay);

    var makeMonthlyPicker = (function () {
        var monthlyPickerComplete = false,
            $ret;

        return function ($parent) {
            var $MonthlyRateSelect, $MonthlyDateSelect, $MonthlyWeekSelect, $startOnMonth, $startOnYear,
                monthlyRadioId = makeId({ prefix: o.ID, ID: 'monthly' }),
                monthlyDayPickerId = makeId({ prefix: o.ID, ID: 'monthly', suffix: 'day' });

            function saveMonthInterval() {
                each(rateInterval, function(prop, key) {
                    if (key !== 'dateformat' && key !== 'ratetype' && key !== 'monthlyday' && key !== 'monthlydate' && key !== 'monthlyfrequency' && key !== 'startingmonth' && key !== 'startingyear') {
                        delete rateInterval[key];
                    }
                });
                if(rateType === CswRateIntervalTypes.MonthlyByDate) {
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
                saveRateInterval();
            }
            
            function makeMonthlyByDateSelect() {
                var $byDate = $('<div />'),
                    daysInMonth = ChemSW.makeSequentialArray(1, 31), selectedDay = '';

                if (isTrue(o.Multi)) {
                    selectedDay = CswMultiEditDefaultValue;
                    daysInMonth.unshift(CswMultiEditDefaultValue);
                }
                else if (contains(rateInterval, 'monthlydate')) {
                    selectedDay = tryParseString(rateInterval.monthlydate);
                }

                $byDate.CswInput('init', {
                        ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'by_date' }),
                        name: monthlyRadioId,
                        type: CswInput_Types.radio,
                        onChange: function() {
                            if (isFunction(o.onchange)) {
                                o.onchange();
                            }
                            rateType = $ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: CswRateIntervalTypes.MonthlyByDate
                    })
                    .CswAttrDom('checked', (rateType === CswRateIntervalTypes.MonthlyByDate));

                $byDate.append('On Day of Month:&nbsp;');

                $MonthlyDateSelect = $byDate.CswSelect('init', {
                    ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'date' }),
                    onChange: function() {
                        if (isFunction(o.onchange)) {
                            o.onchange();
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

                if (isTrue(o.Multi)) {
                    frequency.unshift(CswMultiEditDefaultValue);
                    selected = CswMultiEditDefaultValue;
                } else {
                    selected = tryParseNumber(rateInterval.monthlyfrequency, 1);
                }

                $MonthlyRateSelect = $every.CswSelect('init', {
                    ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'rate' }),
                    onChange: function() {
                        if (isFunction(o.onchange)) {
                            o.onchange();
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
                    monthlyWeekId = makeId({ prefix: o.ID, ID: 'monthly', suffix: 'week' }),
                    monthlyByDayId = makeId({ prefix: o.ID, ID: 'monthly', suffix: 'by_day' }),
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
                        type: CswInput_Types.radio,
                        onChange: function() {
                            if (isFunction(o.onchange)) {
                                o.onchange();
                            }
                            rateType = $ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                            saveMonthInterval();
                        },
                        value: CswRateIntervalTypes.MonthlyByWeekAndDay
                    })
                    .CswAttrDom('checked', (rateType === CswRateIntervalTypes.MonthlyByWeekAndDay));

                $byDay.append('Every&nbsp;');

                if (o.Multi) {
                    weeksInMonth.unshift({ value: CswMultiEditDefaultValue, display: CswMultiEditDefaultValue });
                    selected = CswMultiEditDefaultValue;
                } else {
                    selected = tryParseString(rateInterval.monthlyweek);
                }

                $MonthlyWeekSelect = $byDay.CswSelect('init', {
                    ID: monthlyWeekId,
                    values: weeksInMonth,
                    selected: selected,
                    onChange: function() {
                        if (isFunction(o.onchange)) {
                            o.onchange();
                        }
                        saveMonthInterval();
                    }
                });
                $byDay.append('<br/>');

                monthlyWeekPicker($byDay, o.onchange, true, monthlyDayPickerId);
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
                    monthsInYear.unshift(CswMultiEditDefaultValue);
                    yearsToAllow.unshift(CswMultiEditDefaultValue);
                    selectedMonth = CswMultiEditDefaultValue;
                    selectedYear = CswMultiEditDefaultValue;
                } else {
                    selectedMonth = tryParseString(rateInterval.startingmonth, (now.getMonth() + 1));
                    selectedYear = tryParseString(rateInterval.startingyear, year);
                }

                $startOnMonth = $startOn.CswSelect('init', {
                    ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'startMonth' }),
                    values: monthsInYear,
                    selected: selectedMonth,
                    onChange: function() {
                        if (isFunction(o.onchange)) {
                            o.onchange();
                        }
                        saveMonthInterval();
                    }
                });

                $startOn.append('/');

                $startOnYear = $startOn.CswSelect('init', {
                    ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'startYear' }),
                    values: yearsToAllow,
                    selected: selectedYear,
                    onChange: function() {
                        if (isFunction(o.onchange)) {
                            o.onchange();
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

    var makeYearlyDatePicker = (function () {
        var yearlyDatePickerComplete = false,
            $ret, $yearlyDate;
        return function ($parent) {
            function saveYearInterval() {
                each(rateInterval, function(prop, key) {
                    if (key !== 'dateformat' && key !== 'ratetype' && key !== 'yearlydate') {
                        delete rateInterval[key];
                    }
                });
                
                rateInterval.ratetype = rateType;
                rateInterval.dateformat = dateFormat;
                rateInterval.yearlydate = $yearlyDate.CswDateTimePicker('value');
                rateInterval.yearlydate.dateformat = dateFormat;
                saveRateInterval();
            }
            
            if (false === yearlyDatePickerComplete) {
                $ret = $('<div />').appendTo($parent);

                var yearlyStartDate = '';

                if (isTrue(o.Multi)) {
                    yearlyStartDate = CswMultiEditDefaultValue;
                }
                else if (contains(rateInterval, 'yearlydate')) {
                    yearlyStartDate = tryParseString(rateInterval.yearlydate.date);
                }
                if (isNullOrEmpty(yearlyStartDate)) {
                    rateInterval.yearlydate = { date: nowString, dateformat: dateFormat };
                }
                
                $ret.append('Every Year, Starting On:<br/>');

                $yearlyDate = $ret.CswDateTimePicker('init', {
                    ID: makeId({ prefix: o.ID, ID: 'yearly', suffix: 'sd' }),
                    Date: yearlyStartDate,
                    DateFormat: dateFormat,
                    DisplayMode: 'Date',
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    OnChange: function() {
                        if (isFunction(o.onchange)) {
                            o.onchange();
                        }
                        saveYearInterval();
                    }
                });

                $ret.appendTo($parent);
                
                saveYearInterval();
                
                yearlyDatePickerComplete = true;
            } // if (false === yearlyDatePickerComplete)
            return $ret.addClass('CswFieldTypeTimeInterval_Div');
        };
    })();

    if (false === isTrue(o.ReadOnly)) {
        (function () {
            var $Div = o.$parent,
                propVals = o.propVals,
                textValue,
                $table;

            //globals
            if (o.Multi) {
                //rateInterval = CswMultiEditDefaultValue;
                textValue = CswMultiEditDefaultValue;
                rateType = CswRateIntervalTypes.WeeklyByDay;
            } else {
                $.extend(true, rateInterval, propVals.Interval.rateintervalvalue);
                textValue = tryParseString(propVals.Interval.text).trim();
                rateType = rateInterval.ratetype;
            }

            dateFormat = tryParseString(rateInterval.dateformat, 'M/d/yyyy');
            $interval = $('<div id="' + makeId({ ID: o.ID, suffix: '_cswTimeInterval' }) + '"></div>')
                                .appendTo($Div);

            //Page Components
            $interval.append('<span id="' + o.ID + '_textvalue">' + textValue + '</span>');
            $table = $interval.CswTable('init', { 'ID': o.ID + '_tbl', cellspacing: 5 });

            makeRateType($table);

            $pickerCell = $table.CswTable('cell', 1, 3)
                                .CswAttrDom('rowspan', '3');

            // Set selected values
            switch (rateType) {
                case CswRateIntervalTypes.WeeklyByDay:
                    $WeeklyDiv = weeklyWeekPicker($pickerCell, o.onchange, false);
                    break;
                case CswRateIntervalTypes.MonthlyByDate:
                    $MonthlyDiv = makeMonthlyPicker($pickerCell);
                    break;
                case CswRateIntervalTypes.MonthlyByWeekAndDay:
                    $MonthlyDiv = makeMonthlyPicker($pickerCell);
                    break;
                case CswRateIntervalTypes.YearlyByDate:
                    $YearlyDiv = makeYearlyDatePicker($pickerCell);
                    break;
            } // switch(RateType)
            
            return $interval;
        })();
    }

    var ret = {
        $interval: $interval,
        rateType: rateType,
        rateInterval: rateInterval
    };

    return ret;
};



