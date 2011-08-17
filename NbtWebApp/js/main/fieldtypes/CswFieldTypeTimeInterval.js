/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeTimeInterval';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey
                var $Div = $(this);
                $Div.contents().remove();

                var textValue = o.propData.Interval.text;

				$Div.append('<span id="' + o.ID + '_textvalue">' + textValue + '</span>');

                if(!o.ReadOnly)
                {
					// Rate Type Selector
					var $table = $Div.CswTable('init', { 'ID': o.ID + '_tbl', cellspacing: 5 });

                    var $weeklyradiocell = $table.CswTable('cell', 1, 1);
					var $weeklyradio = $weeklyradiocell.CswInput('init',{ID: o.ID + '_type_weekly',
                                                                         name: o.ID + '_type',
																		 type: CswInput_Types.radio,
                                                                         value: 'weekly'
                                                                });

					$table.CswTable('cell', 1, 2).append('<span>&nbsp;Weekly</span>');
					$weeklyradio.click(function() { $WeeklyDiv.show(); $MonthlyDiv.hide(); $YearlyDiv.hide(); o.onchange(); });

                    var $monthlyradiocell = $table.CswTable('cell', 2, 1);
					var $monthlyradio = $monthlyradiocell.CswInput('init',{ID: o.ID + '_type_monthly',
                                                                           name: o.ID + '_type',
                                                                           type: CswInput_Types.radio,
                                                                           value: 'monthly'
                                                                    });
					$table.CswTable('cell', 2, 2).append('<span>&nbsp;Monthly</span>');
					$monthlyradio.click(function() { $WeeklyDiv.hide(); $MonthlyDiv.show(); $YearlyDiv.hide(); o.onchange(); });

                    var $yearlyradiocell = $table.CswTable('cell', 3, 1);
					var $yearlyradio = $yearlyradiocell.CswInput('init',{ID: o.ID + '_type_yearly',
                                                                         name: o.ID + '_type',
                                                                         type: CswInput_Types.radio,
                                                                         value: 'yearly'
                                                                });  
					$table.CswTable('cell', 3, 2).append('<span>&nbsp;Yearly</span>');
					$yearlyradio.click(function() { $WeeklyDiv.hide(); $MonthlyDiv.hide(); $YearlyDiv.show(); o.onchange(); });

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
					var $WeeklyStartDate = $WeeklyStartDateCell.CswInput('init', { ID: o.ID + '_weekly_sd',
                                                                                   type: CswInput_Types.text,
                                                                                   cssclass: 'textinput', // date',
                                                                                   onChange: o.onchange
                                                                        });  

					// Monthly
					var $MonthlyDiv = $('<div />')
										.appendTo($topcell)
										.addClass('CswFieldTypeTimeInterval_Div');
	//									.hide();

					$MonthlyDiv.append('Every ');
					var $MonthlyRateSelect = $('<select id="'+ o.ID + '_monthly_rate"/>')
												.appendTo($MonthlyDiv)
												.change(o.onchange);
					for(var i = 1; i <= 12; i++)
					{
						$MonthlyRateSelect.append('<option value="'+ i +'">'+ i +'</option>');
					}
					$MonthlyDiv.append(' Month(s)<br/>');

					var $MonthlyByDateRadio = $MonthlyDiv.CswInput('init',{ID: o.ID +'_monthly_by_date',
                                                                           name: o.ID + '_monthly',
                                                                            type: CswInput_Types.radio,
                                                                            onChange: o.onchange,
                                                                            value: 'MonthlyByDate'
                                                 }); 
					$MonthlyDiv.append('On Day of Month:&nbsp;');
					var $MonthlyDateSelect = $('<select id="'+ o.ID +'_monthly_date" />')
									.appendTo($MonthlyDiv)
									.change(o.onchange);
					for(var i = 1; i <= 31; i++)
					{
						$MonthlyDateSelect.append('<option value="'+ i +'">'+ i +'</option>');
					}
					$MonthlyDiv.append('<br/>');

					var $MonthlyByDayRadio = $MonthlyDiv.CswInput('init',{ID: o.ID +'_monthly_by_day',
                                                                           name: o.ID + '_monthly',
                                                                            type: CswInput_Types.radio,
                                                                            onChange: o.onchange,
                                                                            value: 'MonthlyByWeekAndDay'
                                                                 });  
                    
					$MonthlyDiv.append('Every&nbsp;');
					var $MonthlyWeekSelect = $('<select id="'+ o.ID +'_monthly_week" />')
									.appendTo($MonthlyDiv);
					$MonthlyWeekSelect.append('<option value="1">First:</option>');
					$MonthlyWeekSelect.append('<option value="2">Second:</option>');
					$MonthlyWeekSelect.append('<option value="3">Third:</option>');
					$MonthlyWeekSelect.append('<option value="4">Fourth:</option>');
					$MonthlyDiv.append('<br/>');

					makeWeekDayPicker($MonthlyDiv, o.ID + '_monthly_day', o.onchange, true);

					$MonthlyDiv.append('<br/>Starting On:&nbsp;');
					var $MonthlyStartMonthSelect = $('<select id="'+ o.ID +'_monthly_startMonth" />')
									.appendTo($MonthlyDiv)
									.change(o.onchange);
					for(var i = 1; i <= 12; i++)
					{
						$MonthlyStartMonthSelect.append('<option value="'+ i +'">'+ i +'</option>');
					}
					$MonthlyDiv.append('/');
					var $MonthlyStartYearSelect = $('<select id="'+ o.ID +'_monthly_startYear" />')
									.appendTo($MonthlyDiv)
									.change(o.onchange);
					var year = new Date().getFullYear();
					for(var i = year-10; i <= year+10; i++)
					{
						$MonthlyStartYearSelect.append('<option value="'+ i +'">'+ i +'</option>');
					}
					$MonthlyStartYearSelect.val(year);


					// Yearly
					var $YearlyDiv = $('<div />')
										.appendTo($topcell)
										.addClass('CswFieldTypeTimeInterval_Div')
	//									.hide();
				
					$YearlyDiv.append('Every Year, Starting On:<br/>');
					var $YearlyStartDate = $YearlyDiv.CswInput('init',{ID: o.ID + '_yearly_sd',
                                                                       type: CswInput_Types.text,
                                                                       cssclass: 'textinput', // date',
                                                                       onChange: o.onchange
                                                                }); 
					// Set selected values

					var rateIntervalData = o.propData.Interval.rateintervalvalue;
					var rateType = rateIntervalData.ratetype;
					var dateFormat = ServerDateFormatToJQuery(rateIntervalData.dateformat);

					switch(rateType)
					{
						case "WeeklyByDay":
							$weeklyradio.CswAttrDom('checked', 'true');
							$WeeklyDiv.show(); 
							$MonthlyDiv.hide(); 
							$YearlyDiv.hide();
							setWeekDayChecked( o.ID + '_weeklyday', rateIntervalData.weeklyday);
							$WeeklyStartDate.val(rateIntervalData.startingdate.date);
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
							$YearlyStartDate.val(rateIntervalData.yearlydate.date);
							$MonthlyByDateRadio.CswAttrDom('checked', 'true');     //default (for case 21048)
							break;
					} // switch(RateType)

					$WeeklyStartDate.datepicker({ dateFormat: dateFormat });
                    $YearlyStartDate.datepicker({ dateFormat: dateFormat });
				}
            },
        save: function(o) {
				try {
				    var RateType = $('[name="' + o.ID + '_type"]:checked').val();
				    var RIValue = o.propData.Interval.rateintervalvalue;
					switch (RateType)
				    {
				        case 'weekly':
				            RIValue.ratetype = 'WeeklyByDay';
				            RIValue.weeklyday = getWeekDayChecked(o.ID + '_weeklyday');
				            RIValue.startingdate = {};
							RIValue.startingdate.date = $('#' + o.ID + '_weekly_sd').val();
				            RIValue.startingdate.dateformat = RIValue.dateformat;
				            break;

				        case 'monthly':
				            var monthlyType = $('[name="' + o.ID + '_monthly"]:checked').val();
				            RIValue.ratetype = monthlyType;
				            RIValue.monthlyfrequency = $('#' + o.ID + '_monthly_rate').val();
				            if (monthlyType === "MonthlyByDate")
				            {
				                RIValue.monthlydate = $('#' + o.ID + '_monthly_date').val();
				            }
				            else // MonthlyByWeekAndDay
				            {
				                RIValue.monthlyweek = $('#' + o.ID + '_monthly_week').val();
				                RIValue.monthlyday = getWeekDayChecked(o.ID + '_monthly_day');
				            }
				            RIValue.startingmonth = $('#' + o.ID + '_monthly_startMonth').val();
				            RIValue.startingyear = $('#' + o.ID + '_monthly_startYear').val();
				            break;

				        case 'yearly':
				            RIValue.ratetype = 'YearlyByDate';
				            RIValue.yearlydate = {};
							RIValue.yearlydate.date = $('#' + o.ID + '_yearly_sd').val();
				            RIValue.yearlydate.dateformat = RIValue.dateformat;
				            break;
				    } // switch(RateType)
				} catch(e) {
				    if(debugOn()) {
				        log('Error updating propData: ' + e);
				    }
				}
        } // save
    };
    

	function makeWeekDayPicker($parent, id, onchange, useRadio)
	{
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
