/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />

var CswTimeInterval = function (options) {

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
        rateType, $WeeklyDiv, $MonthlyDiv, $YearlyDiv, dateFormat, rateInterval = {}, $pickerCell, $interval;

    var intervalTypes = {
        WeeklyByDay: 'WeeklyByDay',
        MonthlyByDate: 'MonthlyByDate',
        MonthlyByWeekAndDay: 'MonthlyByWeekAndDay',
        YearlyByDate: 'YearlyByDate'
    };

    var makeRateType = function ($table) {
        var $weeklyradiocell = $table.CswTable('cell', 1, 1),
            $weeklyradio = $weeklyradiocell.CswInput('init', {
                ID: o.ID + '_type_weekly',
                name: o.ID + '_type',
                type: CswInput_Types.radio,
                value: 'weekly'
            }),
            $monthlyradiocell = $table.CswTable('cell', 2, 1),
            $monthlyradio = $monthlyradiocell.CswInput('init', {
                ID: o.ID + '_type_monthly',
                name: o.ID + '_type',
                type: CswInput_Types.radio,
                value: 'monthly'
            }),
            $yearlyradiocell = $table.CswTable('cell', 3, 1),
            $yearlyradio = $yearlyradiocell.CswInput('init', {
                ID: o.ID + '_type_yearly',
                name: o.ID + '_type',
                type: CswInput_Types.radio,
                value: 'yearly'
            });

        //Weekly
        $table.CswTable('cell', 1, 2).append('<span>&nbsp;Weekly</span>');
        $weeklyradio.click(function () {
            rateType = 'Weekly';
            $('#' + o.ID + '_textvalue').text('Weekly');
            $WeeklyDiv = makeWeekDayPicker($pickerCell, o.onchange, false);
            o.onchange();
            if (abandonHope) {
                $weeklyradio.attr('checked', true);
                $monthlyradio.attr('checked', false);
                $yearlyradio.attr('checked', false);
            }
        });

        //Monthly
        $table.CswTable('cell', 2, 2).append('<span>&nbsp;Monthly</span>');
        $monthlyradio.click(function () {
            rateType = 'Monthly';
            $('#' + o.ID + '_textvalue').text('Monthly');
            $MonthlyDiv = makeMonthlyPicker($pickerCell);
            o.onchange();
            if (abandonHope) {
                $weeklyradio.attr('checked', false);
                $monthlyradio.attr('checked', true);
                $yearlyradio.attr('checked', false);
            }
        });

        //Yearly
        $table.CswTable('cell', 3, 2).append('<span>&nbsp;Yearly</span>');
        $yearlyradio.click(function () {
            rateType = 'Yearly';
            $('#' + o.ID + '_textvalue').text('Yearly');
            $YearlyDiv = makeYearlyDatePicker($pickerCell);
            o.onchange();
            if (abandonHope) {
                $weeklyradio.attr('checked', false);
                $monthlyradio.attr('checked', false);
                $yearlyradio.attr('checked', true);
            }
        });
    };

    function getWeekDayChecked(id) {
        var ret = '';
        $('[name="' + id + '"]:checked').each(function () {
            var $check = $(this);
            if (ret !== '') ret += ',';
            switch ($check.val()) {
                case '1': ret += 'Sunday'; break;
                case '2': ret += 'Monday'; break;
                case '3': ret += 'Tuesday'; break;
                case '4': ret += 'Wednesday'; break;
                case '5': ret += 'Thursday'; break;
                case '6': ret += 'Friday'; break;
                case '7': ret += 'Saturday'; break;
            }
        });
        return ret;
    } // getWeekDayChecked()

    var setWeekDayChecked = function (id, checkedValues) {
        var i,
            splitvalues = checkedValues.split(',');

        // set all to false
        $('[name="' + id + '"]').CswAttrDom('checked', '');

        // set ones to true
        for (i = 0; i < splitvalues.length; i++) {
            switch (splitvalues[i]) {
                case "Sunday": $('#' + id + '_1').CswAttrDom('checked', 'true'); break;
                case "Monday": $('#' + id + '_2').CswAttrDom('checked', 'true'); break;
                case "Tuesday": $('#' + id + '_3').CswAttrDom('checked', 'true'); break;
                case "Wednesday": $('#' + id + '_4').CswAttrDom('checked', 'true'); break;
                case "Thursday": $('#' + id + '_5').CswAttrDom('checked', 'true'); break;
                case "Friday": $('#' + id + '_6').CswAttrDom('checked', 'true'); break;
                case "Saturday": $('#' + id + '_7').CswAttrDom('checked', 'true'); break;
            }
        } // for(var i = 0; i < splitvalues.length; i++)
    }; // setWeekyDayChecked()

    var makeWeekDayPicker = (function () {
        var weeklyDayPickerComplete = false,
            $ret;

        return function ($parent, onchange, useRadio, elemId) {
            if (false === weeklyDayPickerComplete) {
                $ret = $('<div />');
                var id = elemId || o.ID + '_weeklyday',
                    $picker, $table, i, type, $pickercell, $WeeklyStartDateCell,
                    weeklyStartDate = (false === o.Multi) ? rateInterval.startingdate.date : '',
                    $WeeklyTable = $ret.CswTable('init', {
                        ID: o.ID + '_weeklytbl',
                        cellalign: 'center',
                        FirstCellRightAlign: true
                    });


                $WeeklyTable.CswTable('cell', 1, 1).append('Every:');
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

                for (i = 1; i <= 7; i++) {
                    type = CswInput_Types.checkbox;
                    if (useRadio) {
                        type = CswInput_Types.radio;
                    }
                    $pickercell = $table.CswTable('cell', 2, i);
                    $pickercell.CswInput('init', {
                        ID: id + '_' + i,
                        name: id,
                        type: type,
                        onChange: onchange,
                        value: i
                    })
                        .bind('change', function () {
                            rateInterval.weeklyday = getWeekDayChecked(o.ID + '_weekly_sd');
                        });
                }

                //Starting Date
                $WeeklyTable.CswTable('cell', 2, 1).append('Starting On:');
                $WeeklyStartDateCell = $WeeklyTable.CswTable('cell', 2, 2)
                                                   .CswDateTimePicker('init', {
                                                       ID: o.ID + '_weekly_sd',
                                                       Date: weeklyStartDate,
                                                       DateFormat: dateFormat,
                                                       DisplayMode: 'Date',
                                                       ReadOnly: o.ReadOnly,
                                                       Required: o.Required,
                                                       OnChange: o.onchange
                                                   })
                                                    .bind('change', function () {
                                                        rateInterval.startingdate.date = $(this).val();
                                                    });

                if (false === o.Multi) {
                    setWeekDayChecked(o.ID + '_weeklyday', rateInterval.weeklyday);
                }

                weeklyDayPickerComplete = true;
                $ret.appendTo($parent).addClass('CswFieldTypeTimeInterval_Div');
            } // if (false === weeklyDayComplete)

            if (false === isNullOrEmpty($WeeklyDiv, true)) {
                $WeeklyDiv.show();
            }
            if (false === isNullOrEmpty($MonthlyDiv, true)) {
                $MonthlyDiv.hide();
            }
            if (false === isNullOrEmpty($YearlyDiv, true)) {
                $YearlyDiv.hide();
            }

            rateInterval.ratetype = intervalTypes.WeeklyByDay;
            rateInterval.weeklyday = getWeekDayChecked(o.ID + '_weeklyday');
            rateInterval.startingdate = {
                date: $('#' + o.ID + '_weekly_sd').CswDateTimePicker('value', o.ReadOnly).Date,
                dateformat: rateInterval.dateformat
            };

            return $ret;
        };
    } ()); // makeWeekDayPicker()

    var makeMonthlyPicker = (function () {
        var monthlyPickerComplete = false,
            $ret;

        return function ($parent) {
            var $MonthlyRateSelect, $MonthlyDateSelect, $MonthlyWeekSelect, $startOnMonth, $startOnYear,
                monthlyRadioId = makeId({ prefix: o.ID, ID: 'monthly' }),
                monthlyDayPickerId = makeId({ prefix: o.ID, ID: 'monthly', suffix: 'day' });

            if (false === monthlyPickerComplete) {
                $ret = $('<div />');

                function makeEveryMonthSelect() {
                    var $every = $('<div>Every </div>');

                    $MonthlyRateSelect = $every.CswSelect('init', {
                        ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'rate' }),
                        onChange: o.onchange,
                        values: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
                    })
                        .bind('change', function () {
                            rateInterval.monthlyfrequency = $MonthlyRateSelect.find(':selected').val();
                        });
                    $every.append(' Month(s)<br/>');
                    return $every;
                }
                $ret.append(makeEveryMonthSelect());

                function makeMonthlyByDateSelect() {
                    var $byDate = $('<div />'),
                        daysInMonth = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31];

                    var $MonthlyByDateRadio = $byDate.CswInput('init', {
                        ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'by_date' }),
                        name: monthlyRadioId,
                        type: CswInput_Types.radio,
                        onChange: o.onchange,
                        value: intervalTypes.MonthlyByDate
                    })
                        .bind('change', function () {
                            rateInterval.ratetype = $ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                        });

                    if (rateType === intervalTypes.MonthlyByDate) {
                        $MonthlyByDateRadio.CswAttrDom('checked', true);
                    }

                    $byDate.append('On Day of Month:&nbsp;');
                    if (o.Multi) {
                        daysInMonth.push(CswMultiEditDefaultValue);
                    }

                    $MonthlyDateSelect = $byDate.CswSelect('init', {
                        ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'date' }),
                        onChange: o.onchange,
                        values: daysInMonth,
                        selected: (false === o.Multi) ? '' : CswMultiEditDefaultValue
                    })
                        .bind('change', function () {
                            rateInterval.monthlydate = $MonthlyDateSelect.find(':selected').val();
                        });

                    return $byDate;
                }
                $ret.append(makeMonthlyByDateSelect());

                $ret.append('<br/>');

                function makeMonthlyByDay() {
                    var $byDay = $('<div />'),
                        weeklyDayPicker = makeWeekDayPicker,
                        monthlyWeekId = makeId({ prefix: o.ID, ID: 'monthly', suffix: 'week' }),
                        monthlyByDayId = makeId({ prefix: o.ID, ID: 'monthly', suffix: 'by_day' }),
                        weeksInMonth = [
                            { value: 1, display: 'First:' },
                            { value: 2, display: 'Second:' },
                            { value: 3, display: 'Third:' },
                            { value: 4, display: 'Fourth:' }
                        ];

                    var $MonthlyByDayRadio = $byDay.CswInput('init', {
                        ID: monthlyByDayId,
                        name: monthlyRadioId,
                        type: CswInput_Types.radio,
                        onChange: o.onchange,
                        value: intervalTypes.MonthlyByWeekAndDay
                    })
                        .bind('change', function () {
                            rateInterval.ratetype = $ret.find('[name="' + monthlyRadioId + '"]:checked').val();
                        });

                    if (rateType === intervalTypes.MonthlyByWeekAndDay) {
                        $MonthlyByDayRadio.CswAttrDom('checked', true);
                    }

                    $byDay.append('Every&nbsp;');

                    if (o.Multi) {
                        weeksInMonth.push({ value: CswMultiEditDefaultValue, display: CswMultiEditDefaultValue });
                    }
                    $MonthlyWeekSelect = $byDay.CswSelect('init', {
                        ID: monthlyWeekId,
                        values: weeksInMonth,
                        selected: (false === o.Multi) ? '' : CswMultiEditDefaultValue
                    })
                        .bind('change', function () {
                            rateInterval.monthlyweek = $MonthlyWeekSelect.find(':selected').val();
                        });
                    $byDay.append('<br/>');

                    weeklyDayPicker($byDay, o.onchange, true, monthlyDayPickerId);
                    setWeekDayChecked(monthlyDayPickerId, rateInterval.monthlyday);
                }
                $ret.append(makeMonthlyByDay());

                function makeStartOnYear() {
                    var $startOn = $('<div />'),
                        monthsInYear = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
                        selectedMonth, year, y, yearsToAllow, selectedYear;

                    $startOn.append('<br/>Starting On:&nbsp;');
                    if (o.Multi) {
                        monthsInYear.push(CswMultiEditDefaultValue);
                    }
                    selectedMonth = tryParseString(rateInterval.startingmonth, now.getMonth());

                    $startOnMonth = $startOn.CswSelect('init', {
                        ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'startMonth' }),
                        values: monthsInYear,
                        selected: (false === o.Multi) ? selectedMonth : CswMultiEditDefaultValue,
                        onChange: o.onchange
                    })
                        .bind('change', function () {
                            rateInterval.startingmonth = $startOnMonth.find(':selected').val();
                        });

                    $startOn.append('/');

                    year = now.getFullYear();
                    yearsToAllow = [];
                    for (y = year - 10; y <= year + 10; y++) {
                        yearsToAllow.push(y);
                    }
                    selectedYear = tryParseString(rateInterval.startingyear, year);
                    if (o.Multi) {
                        yearsToAllow.push(CswMultiEditDefaultValue);
                        selectedYear = CswMultiEditDefaultValue;
                    }
                    $startOnYear = $startOn.CswSelect('init', {
                        ID: makeId({ prefix: o.ID, ID: 'monthly', suffix: 'startYear' }),
                        onChange: o.onchange,
                        values: yearsToAllow,
                        selected: selectedYear
                    })
                        .bind('change', function () {
                            rateInterval.startingyear = $startOnYear.find(':selected').val();
                        });
                    return $startOn;
                }
                $ret.append(makeStartOnYear());


                $ret.appendTo($parent).addClass('CswFieldTypeTimeInterval_Div');
            } // if (false === monthlyPickerComplete)

            if (false === isNullOrEmpty($MonthlyDiv, true)) {
                $MonthlyDiv.show();
            }
            if (false === isNullOrEmpty($WeeklyDiv, true)) {
                $WeeklyDiv.hide();
            }
            if (false === isNullOrEmpty($YearlyDiv, true)) {
                $YearlyDiv.hide();
            }
            rateInterval.ratetype = $ret.find('[name="' + monthlyRadioId + '"]:checked').val();
            rateInterval.monthlyfrequency = $MonthlyRateSelect.find(':selected').val();
            rateInterval.startingmonth = $startOnMonth.find(':selected').val();
            rateInterval.startingyear = $startOnYear.find(':selected').val();

            switch (rateInterval.monthlyfrequency) {
                case intervalTypes.MonthlyByDate:
                    rateInterval.monthlydate = $MonthlyDateSelect.find(':selected').val();
                    break;

                case intervalTypes.MonthlyByWeekAndDay:
                    rateInterval.monthlyweek = $MonthlyWeekSelect.find(':selected').val();
                    rateInterval.monthlyday = getWeekDayChecked(monthlyDayPickerId);
                    break;
            }

            return $ret;
        };
    } ());

    var makeYearlyDatePicker = (function () {
        var yearlyDatePickerComplete = false,
            $ret, $yearlyDate;
        return function ($parent) {
            if (false === yearlyDatePickerComplete) {
                $ret = $('<div />').addClass('CswFieldTypeTimeInterval_Div');

                var yearlyStartDate = rateInterval.yearlydate.date;
                $ret.append('Every Year, Starting On:<br/>');

                $yearlyDate = $ret.CswDateTimePicker('init', {
                    ID: makeId({ prefix: o.ID, ID: 'yearly', suffix: 'sd' }),
                    Date: yearlyStartDate,
                    DateFormat: dateFormat,
                    DisplayMode: 'Date',
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    OnChange: o.onchange
                });

                $ret.appendTo($parent);
            } // if (false === yearlyDatePickerComplete)

            if (false === isNullOrEmpty($YearlyDiv, true)) {
                $YearlyDiv.show();
            }
            if (false === isNullOrEmpty($WeeklyDiv, true)) {
                $WeeklyDiv.hide();
            }
            if (false === isNullOrEmpty($MonthlyDiv, true)) {
                $MonthlyDiv.hide();
            }

            var yearDate = $('#' + o.ID + '_yearly_sd').CswDateTimePicker('value', o.propData.readonly).Date;
            rateInterval.ratetype = intervalTypes.YearlyByDate;
            rateInterval.yearlydate = {
                date: yearDate
            };
            return $ret;
        };
    })();

    if (false === isTrue(o.ReadOnly)) {
        (function () {
            var $Div = o.$parent,
                propVals = o.propVals,
                textValue = (isTrue(o.Multi)) ? CswMultiEditDefaultValue : tryParseString(propVals.Interval.text).trim(),
                $table,
                weekDayPicker = makeWeekDayPicker;

            //globals
            if (o.Multi) {
                rateInterval = CswMultiEditDefaultValue;
            } else {
                $.extend(true, rateInterval, propVals.Interval.rateintervalvalue);
            }
            rateType = (false === o.Multi) ? rateInterval.ratetype : 'WeeklyByDay';
            dateFormat = ServerDateFormatToJQuery(rateInterval.dateformat);
            $interval = $('<div id="' + makeId({ ID: o.ID, suffix: '_cswTimeInterval' }) + '"></div>');

            //Page Components
            $interval.append('<span id="' + o.ID + '_textvalue">' + textValue + '</span>');
            $table = $interval.CswTable('init', { 'ID': o.ID + '_tbl', cellspacing: 5 });
            makeRateType($table);
            $pickerCell = $table.CswTable('cell', 1, 3).CswAttrDom('rowspan', '3');

            // Set selected values
            switch (rateType) {
                case intervalTypes.WeeklyByDay:
                    $WeeklyDiv = weekDayPicker($pickerCell, o.onchange, false);
                    break;
                case intervalTypes.MonthlyByDate:
                    $MonthlyDiv = makeMonthlyPicker($pickerCell);
                    break;
                case intervalTypes.MonthlyByWeekAndDay:
                    $MonthlyDiv = makeMonthlyPicker($pickerCell);
                    break;
                case intervalTypes.YearlyByDate:
                    $YearlyDiv = makeYearlyDatePicker($pickerCell);
                    break;
            } // switch(RateType)

            return $interval.appendTo($Div);
        })();
    }

    var ret = {
        $interval: $interval,
        rateType: rateType,
        rateInterval: rateInterval
    };

    return ret;
};



