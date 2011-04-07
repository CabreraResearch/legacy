; (function ($) {
	var PluginName = "CswWizard";

	var methods = {
		'init': function(options) 
			{
				var o = {
					ID: '',
					Title: 'A Wizard',
					StepCount: 1,
					Steps: { 1: 'Default' },
					SelectedStep: 1,
					FinishText: 'Finish',
					onStepChange: function(newstepno) {},
					onFinish: function() {},
					onCancel: function() {}
				};
				if(options) $.extend(o, options);
				
				var $parent = $(this);
				
				var $table = $parent.CswTable({ ID: o.ID, TableCssClass: 'CswWizard_WizardTable' });
				$table.attr("stepcount", o.StepCount);
				
				var $titlecell = $table.CswTable('cell', 1, 1)
									.addClass('CswWizard_TitleCell')
									.attr('colspan', 2)
									.append(o.Title);

				var $indexcell = $table.CswTable('cell', 2, 1)
									.attr('rowspan', 2)
									.addClass('CswWizard_IndexCell');
				
				var $stepscell = $table.CswTable('cell', 2, 2)
									.addClass('CswWizard_StepsCell');

				var $buttonscell = $table.CswTable('cell', 3, 1)
									.addClass('CswWizard_ButtonsCell');

				var steplinks = [];
				var stepdivs = [];
				for(var s = 1; s <= o.StepCount; s++)
				{
					var title = o.Steps[s];

					steplinks[s] = $( '<div class="CswWizard_StepLinkDiv" stepno="' + s + '"><a href="#">' + s + '.&nbsp;' + title + '</a></div>')
										.appendTo($indexcell)
										.children('a')
										.click( function(stepno) { return function() { _selectStep($table, stepno, o.onStepChange); return false; }; }(s) );

					stepdivs[s] = $('<div class="CswWizard_StepDiv" id="' + o.ID + '_' + s + '" stepno="' + s + '" ></div>')
										.appendTo($stepscell);
				}

				var $prevbtn = $('<input type="button" id="' + o.ID + '_prev" value="&lt;&nbsp;Previous" />')
									.click(function() { 
												var currentStepNo = _getCurrentStepNo($table);
												_selectStep($table, currentStepNo - 1, o.onStepChange);
											});
				var $nextbtn = $('<input type="button" id="' + o.ID + '_next" value="Next&nbsp;&gt;" />')
									.click(function() { 
												var currentStepNo = _getCurrentStepNo($table);
												_selectStep($table, currentStepNo + 1, o.onStepChange);
											});
				var $finishbtn = $('<input type="button" id="' + o.ID + '_finish" value="'+ o.FinishText +'" />')
									.click(o.onFinish);
				var $cancelbtn = $('<input type="button" id="' + o.ID + '_cancel" value="Cancel" />')
									.click(o.onCancel);

				$buttontbl = $buttonscell.CswTable({ ID: o.ID + '_btntbl', width: '100%' });
				$buttontbl.CswTable('cell', 1, 1)
							.append($prevbtn)
							.append($nextbtn)
							.append($finishbtn)
							.attr('align', 'right')
							.attr('width', '65%');
				$buttontbl.CswTable('cell', 1, 2)
							.append($cancelbtn)
							.attr('align', 'right')
							.attr('width', '35%');

				_selectStep($table, o.SelectedStep, o.onStepChange);

				return $table;
			}, // init()

		'div': function(stepno) {
				var $table = $(this);
				return $table.find('.CswWizard_StepDiv[stepno=' + stepno + ']');
			}
	};


	function _getCurrentStepNo($table)
	{
		return parseInt($table.find('.CswWizard_StepLinkDivSelected').attr('stepno'));
	}
	
	function _selectStep($table, stepno, onStepChange)
	{
		var stepcount = $table.attr("stepcount");
		if(stepno > 0 && stepno <= stepcount)
		{
			$table.find('.CswWizard_StepLinkDiv').removeClass('CswWizard_StepLinkDivSelected');
			$table.find('.CswWizard_StepLinkDiv[stepno='+ stepno + ']').addClass('CswWizard_StepLinkDivSelected');

			$table.find('.CswWizard_StepDiv').hide();
			$table.find('.CswWizard_StepDiv[stepno=' + stepno + ']').show();

			var $prevbtn = $('#' + $table.attr('id') + '_prev');
			if(stepno == 1) 
				$prevbtn.css('visibility', 'hidden');
			else
				$prevbtn.css('visibility', '');

			var $nextbtn = $('#' + $table.attr('id') + '_next');
			if(stepno >= stepcount)
				$nextbtn.css('visibility', 'hidden');
			else
				$nextbtn.css('visibility', '');

			onStepChange(stepno);
		} // if(stepno <= stepcount)
	} // _selectStep()
	
	// Method calling logic
	$.fn.CswWizard = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

}) (jQuery);

