; (function ($) {
        
    var PluginName = 'CswFieldTypeTimeInterval';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

                var $Div = $(this);
                $Div.contents().remove();

                var TextValue = o.$propxml.children('Interval').attr('text');

				$Div.append('<span id="' + o.ID + '_textvalue">' + TextValue + '</span>');

                if(!o.ReadOnly)
                {
					// Rate Type Selector
					var $table = $Div.CswTable('init', { 'ID': o.ID + '_tbl', cellspacing: 5 });

                    var $weeklyradiocell = $table.CswTable('cell', 1, 1);
					var $weeklyradio = $weeklyradiocell.CswInput('init',{ID: o.ID + '_type_weekly',
                                                                         type: CswInput_Types.radio,
                                                                         value: 'weekly'
                                                                });

					$table.CswTable('cell', 1, 2).append('<span>&nbsp;Weekly</span>');
					$weeklyradio.click(function() { $WeeklyDiv.show(); $MonthlyDiv.hide(); $YearlyDiv.hide(); o.onchange(); });

                    var $monthlyradiocell = $table.CswTable('cell', 2, 1);
					var $monthlyradio = $monthlyradiocell.CswInput('init',{ID: o.ID + '_type_monthly',
                                                                           type: CswInput_Types.radio,
                                                                           value: 'monthly'
                                                                    });
					$table.CswTable('cell', 2, 2).append('<span>&nbsp;Monthly</span>');
					$monthlyradio.click(function() { $WeeklyDiv.hide(); $MonthlyDiv.show(); $YearlyDiv.hide(); o.onchange(); });

                    var $yearlyradiocell = $table.CswTable('cell', 3, 1);
					var $yearlyradio = $yearlyradiocell.CswInput('init',{ID: o.ID + '_type_yearly',
                                                                         type: CswInput_Types.radio,
                                                                         value: 'yearly'
                                                                });  
					$table.CswTable('cell', 3, 2).append('<span>&nbsp;Yearly</span>');
					$yearlyradio.click(function() { $WeeklyDiv.hide(); $MonthlyDiv.hide(); $YearlyDiv.show(); o.onchange(); });

					var $topcell = $table.CswTable('cell', 1, 3);
					$topcell.attr('rowspan', '3');


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
					makeWeekDayPicker($WeeklyTable.CswTable('cell', 1 , 2), o.ID + '_weeklyday', o.onchange);

					$WeeklyTable.CswTable('cell', 2, 1).append('Starting On:');
					var $WeeklyStartDateCell = $WeeklyTable.CswTable('cell', 2, 2);
					var $WeeklyStartDate = $WeeklyStartDateCell.CswInput('init',{ID: o.ID + '_weekly_sd',
                                                                                 type: CswInput_Types.text,
                                                                                 cssclass: 'textinput date',
                                                                                 onChange: o.onchange
                                                                        });  
                    $WeeklyStartDate.datepicker();

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

					makeWeekDayPicker($MonthlyDiv, o.ID + '_monthly_day', o.onchange);

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
                                                                       cssclass: 'textinput date',
                                                                       onChange: o.onchange
                                                                }); 
                    $YearlyStartDate.datepicker();

					// Set selected values

					//<Interval>
					//  <RateIntervalValue>
					//    <RateType>MonthlyByDate</RateType>
					//    <MonthlyFrequency>1</MonthlyFrequency>
					//    <MonthlyDate>1</MonthlyDate>
					//    <StartingMonth>1</StartingMonth>
					//    <StartingYear>2011</StartingYear>
					//  </RateIntervalValue>
					//</Interval>

					var $RateIntervalXml = o.$propxml.children('interval').children('rateintervalvalue');
					var RateType = $RateIntervalXml.children('ratetype').text();
					switch(RateType)
					{
						case "WeeklyByDay":
							$weeklyradio.attr('checked', 'true');
							$WeeklyDiv.show(); 
							$MonthlyDiv.hide(); 
							$YearlyDiv.hide();
							setWeekDayChecked( o.ID + '_weeklyday', $RateIntervalXml.children('weeklyday').text());
							$WeeklyStartDate.val($RateIntervalXml.children('startingdate').text());
							break;
						case "MonthlyByDate":
							$monthlyradio.attr('checked', 'true');
							$WeeklyDiv.hide(); 
							$MonthlyDiv.show(); 
							$YearlyDiv.hide();
							$MonthlyByDateRadio.attr('checked', 'true');
							$MonthlyRateSelect.val($RateIntervalXml.children('monthlyfrequency').text());
							$MonthlyDateSelect.val($RateIntervalXml.children('monthlydate').text());
							$MonthlyStartMonthSelect.val($RateIntervalXml.children('startingmonth').text());
							$MonthlyStartYearSelect.val($RateIntervalXml.children('startingyear').text());
							break;
						case "MonthlyByWeekAndDay":
							$monthlyradio.attr('checked', 'true');
							$WeeklyDiv.hide(); 
							$MonthlyDiv.show(); 
							$YearlyDiv.hide();
							$MonthlyByDayRadio.attr('checked', 'true');
							$MonthlyWeekSelect.val($RateIntervalXml.children('monthlyweek').text());
							setWeekDayChecked( o.ID + '_monthly_day', $RateIntervalXml.children('monthlyday').text());
							$MonthlyStartMonthSelect.val($RateIntervalXml.children('startingmonth').text());
							$MonthlyStartYearSelect.val($RateIntervalXml.children('startingyear').text());
							break;
						case "YearlyByDate":
							$yearlyradio.attr('checked', 'true');
							$WeeklyDiv.hide(); 
							$MonthlyDiv.show(); 
							$YearlyDiv.hide();
							$YearlyStartDate.val($RateIntervalXml.children('yearlydate').text());
							break;
					} // switch(RateType)
				}
            },
        save: function(o) {
				
				var RateType = $('[name="' + o.ID + '_type"]:checked').attr('value');

				o.$propxml.children().remove();
				var $intervalnode = $('<interval />').appendTo(o.$propxml);
				var $rivnode = $('<rateintervalvalue />').appendTo($intervalnode);
				
				switch(RateType)
				{
					case 'weekly': 
						$('<ratetype>WeeklyByDay</ratetype>')
							.appendTo($rivnode);
						$('<weeklyday>' + getWeekDayChecked( o.ID + '_weeklyday' ) + '</weeklyday>')
							.appendTo($rivnode);
						$('<startingdate>'+ $('#' + o.ID + '_weekly_sd').val() +'</startingdate>')
							.appendTo($rivnode);
						break;

					case 'monthly': 
						var MonthlyType = $('[name="'+ o.ID +'_monthly_by"]:checked').attr('value');
						$('<ratetype>'+ MonthlyType +'</ratetype>')
							.appendTo($rivnode);
						$('<monthlyfrequency>'+ $('#' + o.ID + '_monthly_rate').val() +'</monthlyfrequency>')
							.appendTo($rivnode);
						if(MonthlyType == "MonthlyByDate")
						{
							$('<monthlydate>'+ $('#' + o.ID + '_monthly_date').val() +'</monthlydate>')
								.appendTo($rivnode);
						} 
						else // MonthlyByWeekAndDay
						{
							$('<monthlyweek>' + $('#' + o.ID + '_monthly_week' ).val() + '</monthlyweek>')
								.appendTo($rivnode);
							$('<monthlyday>' + getWeekDayChecked( o.ID + '_monthly_day' ) + '</monthlyday>')
								.appendTo($rivnode);
						}
						$('<startingmonth>' + $('#' + o.ID + '_monthly_startMonth' ).val() + '</startingmonth>')
							.appendTo($rivnode);
						$('<startingyear>' + $('#' + o.ID + '_monthly_startYear' ).val() + '</startingyear>')				
							.appendTo($rivnode);
						break;
					
					case 'yearly': 
						$('<ratetype>YearlyByDate</ratetype>')
							.appendTo($rivnode);
						$('<yearlydate>'+ $('#' + o.ID + '_yearly_sd').val() +'</yearlydate>')
							.appendTo($rivnode);
						break;
				} // switch(RateType)

				log($intervalnode.find('*').length);
				$intervalnode.find('*').andSelf().each(function() { $(this).attr('xmlns', ''); });

            } // save
    };
    

	function makeWeekDayPicker($parent, id, onchange)
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
            var $pickercell = $table.CswTable('cell', 2, i);
            var $pickercheck = $pickercell.CswInput('init',{ID: id + '_' + i,
                                                      type: CswInput_Types.checkbox,
                                                      onChange: onchange,
                                                      value: i
                                                 }); 
		}
	} // makeWeekDayPicker()

	function setWeekDayChecked(id, checkedValues)
	{
		// set all to false
		$('[name="' + id + '"]').attr('checked', '');
		
		// set ones to true
		var splitvalues = checkedValues.split(',');		
		for(var i = 0; i < splitvalues.length; i++)
		{
			switch(splitvalues[i])
			{
				case "Sunday": $('#' + id + '_1').attr('checked', 'true'); break;
				case "Monday": $('#' + id + '_2').attr('checked', 'true'); break;
				case "Tuesday": $('#' + id + '_3').attr('checked', 'true'); break;
				case "Wednesday": $('#' + id + '_4').attr('checked', 'true'); break;
				case "Thursday": $('#' + id + '_5').attr('checked', 'true'); break;
				case "Friday": $('#' + id + '_6').attr('checked', 'true'); break;
				case "Saturday": $('#' + id + '_7').attr('checked', 'true'); break;
			}
		} // for(var i = 0; i < splitvalues.length; i++)
	} // setWeekyDayChecked()

	function getWeekDayChecked(id)
	{
		var ret = '';
		$('[name="' + id + '"]').each(function() {
			var $check = $(this);
			if($check.attr('checked') != '')
			{
				if(ret != '') ret += ',';
				switch($check.attr('value'))
				{
					case '1': ret += 'Sunday'; break;
					case '2': ret += 'Monday'; break;
					case '3': ret += 'Tuesday'; break;
					case '4': ret += 'Wednesday'; break;
					case '5': ret += 'Thursday'; break;
					case '6': ret += 'Friday'; break;
					case '7': ret += 'Saturday'; break;
				}
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
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
