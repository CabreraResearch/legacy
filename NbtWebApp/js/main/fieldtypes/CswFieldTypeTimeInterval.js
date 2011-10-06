/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.4-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeTimeInterval';
    
    var methods = {
        init: function(o) { 
            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var textValue = (false === o.Multi) ? tryParseString(propVals.Interval.text).trim() : CswMultiEditDefaultValue;

            $Div.append('<span id="' + o.ID + '_textvalue">' + textValue + '</span>');

            if(!o.ReadOnly) {
				// Rate Type Selector
				var $table = $Div.CswTable('init', { 'ID': o.ID + '_tbl', cellspacing: 5 });

                var $weeklyradiocell = $table.CswTable('cell', 1, 1);
				var $weeklyradio = $weeklyradiocell.CswInput('init',{ID: o.ID + '_type_weekly',
                                                                        name: o.ID + '_type',
																		type: CswInput_Types.radio,
                                                                        value: 'weekly'
                                                            });

				$table.CswTable('cell', 1, 2).append('<span>&nbsp;Weekly</span>');
				$weeklyradio.click(function() { $('#' + o.ID + '_textvalue').text('Weekly'); $WeeklyDiv.show(); $MonthlyDiv.hide(); $YearlyDiv.hide(); o.onchange(); });

                var $monthlyradiocell = $table.CswTable('cell', 2, 1);
				var $monthlyradio = $monthlyradiocell.CswInput('init',{ID: o.ID + '_type_monthly',
                                                                        name: o.ID + '_type',
                                                                        type: CswInput_Types.radio,
                                                                        value: 'monthly'
                                                                });
				$table.CswTable('cell', 2, 2).append('<span>&nbsp;Monthly</span>');
				$monthlyradio.click(function() { $('#' + o.ID + '_textvalue').text('Monthly'); $WeeklyDiv.hide(); $MonthlyDiv.show(); $YearlyDiv.hide(); o.onchange(); });

                var $yearlyradiocell = $table.CswTable('cell', 3, 1);
				var $yearlyradio = $yearlyradiocell.CswInput('init',{ID: o.ID + '_type_yearly',
                                                                        name: o.ID + '_type',
                                                                        type: CswInput_Types.radio,
                                                                        value: 'yearly'
                                                            });  
				$table.CswTable('cell', 3, 2).append('<span>&nbsp;Yearly</span>');
				$yearlyradio.click(function() { $('#' + o.ID + '_textvalue').text('Yearly'); $WeeklyDiv.hide(); $MonthlyDiv.hide(); $YearlyDiv.show(); o.onchange(); });

				var $topcell = $table.CswTable('cell', 1, 3);
				$topcell.CswAttrDom('rowspan', '3');


				// Weekly
				var $WeeklyDiv = $('<div />')
									.appendTo($topcell)
									.addClass('CswFieldTypeTimeInterval_Div');

				var $WeeklyTable = $WeeklyDiv.CswTable('init', { 
							'ID': o.ID + '_weeklytbl',
							'cellalign': 'center',
							'FirstCellRightAlign': true
						});

				$WeeklyTable.CswTable('cell', 1, 1).append('Every:');
				makeWeekDayPicker($WeeklyTable.CswTable('cell', 1 , 2), o.ID + '_weeklyday', o.onchange, false);

				$WeeklyTable.CswTable('cell', 2, 1).append('Starting On:');
				var $WeeklyStartDateCell = $WeeklyTable.CswTable('cell', 2, 2);

				// Monthly
				var $MonthlyDiv = $('<div />')
									.appendTo($topcell)
									.addClass('CswFieldTypeTimeInterval_Div');
//									.hide();

				$MonthlyDiv.append('Every ');
                var $MonthlyRateSelect = $MonthlyDiv.CswSelect('init', {
                                                        ID: o.ID + '_monthly_rate',
                                                        onChange: o.onchange,
                                                        values: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
                                                    });  
				$MonthlyDiv.append(' Month(s)<br/>');

				var $MonthlyByDateRadio = $MonthlyDiv.CswInput('init',{ID: o.ID +'_monthly_by_date',
                                                                        name: o.ID + '_monthly',
                                                                        type: CswInput_Types.radio,
                                                                        onChange: o.onchange,
                                                                        value: 'MonthlyByDate'
                                                }); 
				$MonthlyDiv.append('On Day of Month:&nbsp;');
                var daysInMonth = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31];
                if (o.Multi) {
                    daysInMonth.push(CswMultiEditDefaultValue);
;                    }
                var $MonthlyDateSelect = $MonthlyDiv.CswSelect('init', {
                                                        ID: o.ID + '_monthly_date',
                                                        onChange: o.onchange,
                                                        values: daysInMonth,
                                                        selected: (false === o.Multi) ? '' : CswMultiEditDefaultValue
                                                    });

                $MonthlyDiv.append('<br/>');

				var $MonthlyByDayRadio = $MonthlyDiv.CswInput('init',{ID: o.ID +'_monthly_by_day',
                                                                        name: o.ID + '_monthly',
                                                                        type: CswInput_Types.radio,
                                                                        onChange: o.onchange,
                                                                        value: 'MonthlyByWeekAndDay'
                                                                });  
                    
				$MonthlyDiv.append('Every&nbsp;');
                var weeksInMonth = [
                    { value: 1, display: 'First:' },
                    { value: 2, display: 'Second:' },
                    { value: 3, display: 'Third:' },
                    { value: 4, display: 'Fourth:' }
                ];
                if (o.Multi) {
                    weeksInMonth.push({value: CswMultiEditDefaultValue, display: CswMultiEditDefaultValue });
                }
                var $MonthlyWeekSelect = $MonthlyDiv.CswSelect('init', {
                                                        ID: o.ID +'_monthly_week',
                                                        values: weeksInMonth,
                                                        selected: (false === o.Multi) ? '' : CswMultiEditDefaultValue
                                                    });
				$MonthlyDiv.append('<br/>');

				makeWeekDayPicker($MonthlyDiv, o.ID + '_monthly_day', o.onchange, true);

				$MonthlyDiv.append('<br/>Starting On:&nbsp;');

                var monthsInYear = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
                if (o.Multi) {
                    monthsInYear.push(CswMultiEditDefaultValue);
                }
                var $MonthlyStartMonthSelect = $MonthlyDiv.CswSelect('init', {
                                                                ID: o.ID +'_monthly_startMonth',
                                                                values: monthsInYear,
                                                                selected: (false === o.Multi) ? '' : CswMultiEditDefaultValue,
                                                                onChange: o.onchange
                                                            });
				$MonthlyDiv.append('/');
					
				var year = new Date().getFullYear();
                var yearsToAllow = [];
                for(var y = year-10; y <= year+10; y++) {
                    yearsToAllow.push(y);
				}
                var selectedYear = year;
                if (o.Multi) {
                    yearsToAllow.push(CswMultiEditDefaultValue);
                    selectedYear = CswMultiEditDefaultValue;
                }
                var $MonthlyStartYearSelect = $MonthlyDiv.CswSelect('init', {
                                                                ID: o.ID +'_monthly_startYear',
                                                                onChange: o.onchange,
                                                                values: yearsToAllow,
                                                                selected: selectedYear
                                                            }); 

				// Yearly
                var $YearlyDiv = $('<div />')
                    .appendTo($topcell)
                    .addClass('CswFieldTypeTimeInterval_Div');
//									.hide();
				
				$YearlyDiv.append('Every Year, Starting On:<br/>');

				// Set selected values

				var rateIntervalData = (false === o.Multi) ? propVals.Interval.rateintervalvalue : CswMultiEditDefaultValue;
				var rateType = (false === o.Multi) ? rateIntervalData.ratetype : 'WeeklyByDay';
				var dateFormat = ServerDateFormatToJQuery(rateIntervalData.dateformat);

				switch(rateType)
				{
					case "WeeklyByDay":
						$weeklyradio.CswAttrDom('checked', 'true');
						$WeeklyDiv.show(); 
						$MonthlyDiv.hide(); 
						$YearlyDiv.hide();
						if (false === o.Multi) {
						    setWeekDayChecked(o.ID + '_weeklyday', rateIntervalData.weeklyday);
						}
					    var WeeklyStartDate = (false === o.Multi) ? rateIntervalData.startingdate.date : '';
						$MonthlyByDateRadio.CswAttrDom('checked', 'true');     //default (for case 21048)
						break;
					case "MonthlyByDate":
						$monthlyradio.CswAttrDom('checked', 'true');
						$WeeklyDiv.hide(); 
						$MonthlyDiv.show(); 
						$YearlyDiv.hide();
						$MonthlyByDateRadio.CswAttrDom('checked', 'true');
						$MonthlyRateSelect.val(rateIntervalData.monthlyfrequency);
						$MonthlyDateSelect.val(rateIntervalData.monthlydate);
						$MonthlyStartMonthSelect.val(rateIntervalData.startingmonth);
						$MonthlyStartYearSelect.val(rateIntervalData.startingyear);
						break;
					case "MonthlyByWeekAndDay":
						$monthlyradio.CswAttrDom('checked', 'true');
						$WeeklyDiv.hide(); 
						$MonthlyDiv.show(); 
						$YearlyDiv.hide();
						$MonthlyByDayRadio.CswAttrDom('checked', 'true');
						$MonthlyWeekSelect.val(rateIntervalData.monthlyweek);
						setWeekDayChecked( o.ID + '_monthly_day', rateIntervalData.monthlyday);
						$MonthlyStartMonthSelect.val(rateIntervalData.startingmonth);
						$MonthlyStartYearSelect.val(rateIntervalData.startingyear);
						break;
					case "YearlyByDate":
						$yearlyradio.CswAttrDom('checked', 'true');
						$WeeklyDiv.hide(); 
						$MonthlyDiv.hide(); 
						$YearlyDiv.show();
						var YearlyStartDate = rateIntervalData.yearlydate.date;
						$MonthlyByDateRadio.CswAttrDom('checked', 'true');     //default (for case 21048)
						break;
				} // switch(RateType)

				$WeeklyStartDateCell.CswDateTimePicker('init', { 
					                    ID: o.ID + '_weekly_sd',
										Date: WeeklyStartDate,
										DateFormat: dateFormat,
										DisplayMode: 'Date',
										ReadOnly: o.ReadOnly,
										Required: o.Required,
										OnChange: o.onchange
									});
				$YearlyDiv.CswDateTimePicker('init', { 
					            ID: o.ID + '_yearly_sd',
								Date: YearlyStartDate,
								DateFormat: dateFormat,
								DisplayMode: 'Date',
								ReadOnly: o.ReadOnly,
								Required: o.Required,
								OnChange: o.onchange
							});
			}
        },
        save: function(o) {
				try {
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
				    var dateFormat = o.propData.values.Interval.rateintervalvalue;
				    if (false === o.Multi || $('#' + o.ID + '_textvalue').text() !== CswMultiEditDefaultValue) {
				        switch (rateType) {
				            case 'weekly':
				                attributes.Interval.rateintervalvalue.ratetype = 'WeeklyByDay';
				                attributes.Interval.rateintervalvalue.weeklyday = getWeekDayChecked(o.ID + '_weeklyday');
				                attributes.Interval.rateintervalvalue.startingdate = { };
				                attributes.Interval.rateintervalvalue.startingdate.date = $('#' + o.ID + '_weekly_sd').CswDateTimePicker('value').Date;
				                attributes.Interval.rateintervalvalue.startingdate.dateformat = dateFormat;
				                break;
				            case 'monthly':
				                var monthlyType = $('[name="' + o.ID + '_monthly"]:checked').val();
				                attributes.Interval.rateintervalvalue.ratetype = monthlyType;
				                attributes.Interval.rateintervalvalue.monthlyfrequency = $('#' + o.ID + '_monthly_rate').val();
				                if (monthlyType === "MonthlyByDate") {
				                    attributes.Interval.rateintervalvalue.monthlydate = $('#' + o.ID + '_monthly_date').val();
				                } else // MonthlyByWeekAndDay
				                {
				                    attributes.Interval.rateintervalvalue.monthlyweek = $('#' + o.ID + '_monthly_week').val();
				                    attributes.Interval.rateintervalvalue.monthlyday = getWeekDayChecked(o.ID + '_monthly_day');
				                }
				                attributes.Interval.rateintervalvalue.startingmonth = $('#' + o.ID + '_monthly_startMonth').val();
				                attributes.Interval.rateintervalvalue.startingyear = $('#' + o.ID + '_monthly_startYear').val();
				                break;
				            case 'yearly':
				                attributes.Interval.rateintervalvalue.ratetype = 'YearlyByDate';
				                attributes.Interval.rateintervalvalue.yearlydate = { };
				                attributes.Interval.rateintervalvalue.yearlydate.date = $('#' + o.ID + '_yearly_sd').CswDateTimePicker('value').Date;
				                attributes.Interval.rateintervalvalue.yearlydate.dateformat = dateFormat;
				                break;
				        } // switch(RateType)
				    } 
				    preparePropJsonForSave(o.Multi, o.propData, attributes);
				} catch(e) {
				    if(debugOn()) {
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

		for(var i = 1; i <= 7; i++)
		{
            var type = CswInput_Types.checkbox;
			if(useRadio) type = CswInput_Types.radio;
			
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

	function setWeekDayChecked(id, checkedValues)
	{
		// set all to false
		$('[name="' + id + '"]').CswAttrDom('checked', '');
		
		// set ones to true
		var splitvalues = checkedValues.split(',');		
		for(var i = 0; i < splitvalues.length; i++)
		{
			switch(splitvalues[i])
			{
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

	function getWeekDayChecked(id)
	{
		var ret = '';
		$('[name="' + id + '"]:checked').each(function() {
			var $check = $(this);
			if(ret !== '') ret += ',';
			switch($check.val())
			{
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
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
