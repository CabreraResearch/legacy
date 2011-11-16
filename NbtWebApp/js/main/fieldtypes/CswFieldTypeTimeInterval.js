/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />
/// <reference path="../controls/CswTimeInterval.js" />

(function ($) {

    var pluginName = 'CswFieldTypeTimeInterval';

    var methods = {
        init: function (o) {
            var $Div = $(this);
            o.propVals = o.propData.values;
            o.$parent = $Div;
            var $interval = CswTimeInterval(o);
        },
        save: function (o) {
            try {
                var rateInterval = o.propData.values.Interval.rateintervalvalue;
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
                var rateType = $('[name="' + o.ID + '_type"]:checked').val();
                if (isNullOrEmpty(rateType)) {
                    rateType = $('input[name="' + o.ID + '_type"]').parent().find('[checked="checked"]').val();
                }
                var newInterval = attributes.Interval.rateintervalvalue;
                if (false === o.Multi || $('#' + o.ID + '_textvalue').text() !== CswMultiEditDefaultValue) {
                    switch (rateType) {
                        case 'weekly':
                            if (false === contains(rateInterval, 'weeklyday')) {
                                rateInterval.weeklyday = null;
                            }
                            if (false === contains(rateInterval, 'startingdate')) {
                                rateInterval.startingdate = {
                                    date: null,
                                    dateformat: null
                                };
                            }
                            newInterval.ratetype = 'WeeklyByDay';
                            newInterval.weeklyday = getWeekDayChecked(o.ID + '_weeklyday');
                            newInterval.startingdate = {
                                date: $('#' + o.ID + '_weekly_sd').CswDateTimePicker('value', o.propData.readonly).Date,
                                dateformat: rateInterval.dateformat
                            };
                            break;
                        case 'monthly':
                            if (false === contains(rateInterval, 'monthlyfrequency')) {
                                rateInterval.monthlyfrequency = null;
                            }
                            if (false === contains(rateInterval, 'monthlydate')) {
                                rateInterval.monthlydate = null;
                            }
                            if (false === contains(rateInterval, 'monthlyweek')) {
                                rateInterval.monthlyweek = null;
                            }
                            if (false === contains(rateInterval, 'monthlyday')) {
                                rateInterval.monthlyday = null;
                            }
                            if (false === contains(rateInterval, 'startingmonth')) {
                                rateInterval.startingmonth = null;
                            }
                            if (false === contains(rateInterval, 'startingyear')) {
                                rateInterval.startingyear = null;
                            }
                            var monthlyType = $('[name="' + o.ID + '_monthly"]:checked').val();
                            newInterval.ratetype = monthlyType;
                            newInterval.monthlyfrequency = $('#' + o.ID + '_monthly_rate').val();
                            if (monthlyType === "MonthlyByDate") {
                                newInterval.monthlydate = $('#' + o.ID + '_monthly_date').val();
                            } else // MonthlyByWeekAndDay
                            {
                                newInterval.monthlyweek = $('#' + o.ID + '_monthly_week').val();
                                newInterval.monthlyday = getWeekDayChecked(o.ID + '_monthly_day');
                            }
                            newInterval.startingmonth = $('#' + o.ID + '_monthly_startMonth').val();
                            newInterval.startingyear = $('#' + o.ID + '_monthly_startYear').val();
                            break;
                        case 'yearly':
                            if (false === contains(rateInterval, 'yearlydate')) {
                                rateInterval.yearlydate = {
                                    date: null,
                                    dateformat: null
                                };
                            }
                            var yearDate = $('#' + o.ID + '_yearly_sd').CswDateTimePicker('value', o.propData.readonly).Date;
                            newInterval.ratetype = 'YearlyByDate';
                            newInterval.yearlydate = {
                                date: yearDate,
                                dateformat: rateInterval.dateformat
                            };
                            //attributes.Interval.rateintervalvalue.yearlydate.dateformat = dateFormat;
                            break;
                    } // switch(RateType)
                }
                preparePropJsonForSave(o.Multi, o.propData, attributes);
            } catch (e) {
                if (debugOn()) {
                    log('Error updating propData: ' + e);
                }
            }
        } // save
    };

    function makeWeekDayPicker($parent, id, onchange, useRadio) {
        var $table = $parent.CswTable('init', {
            'ID': id,
            'cellalign': 'center'
        });
        $table.CswTable('cell', 1, 1).append('Su');
        $table.CswTable('cell', 1, 2).append('M');
        $table.CswTable('cell', 1, 3).append('Tu');
        $table.CswTable('cell', 1, 4).append('W');
        $table.CswTable('cell', 1, 5).append('Th');
        $table.CswTable('cell', 1, 6).append('F');
        $table.CswTable('cell', 1, 7).append('Sa');

        for (var i = 1; i <= 7; i++) {
            var type = CswInput_Types.checkbox;
            if (useRadio) type = CswInput_Types.radio;

            var $pickercell = $table.CswTable('cell', 2, i);
            var $pickercheck = $pickercell.CswInput('init', {
                'ID': id + '_' + i,
                'name': id,
                'type': type,
                'onChange': onchange,
                'value': i
            });
        }
    } // makeWeekDayPicker()

    function setWeekDayChecked(id, checkedValues) {
        // set all to false
        $('[name="' + id + '"]').CswAttrDom('checked', '');

        // set ones to true
        var splitvalues = checkedValues.split(',');
        for (var i = 0; i < splitvalues.length; i++) {
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
    } // setWeekyDayChecked()

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


    // Method calling logic
    $.fn.CswFieldTypeTimeInterval = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };
})(jQuery);
