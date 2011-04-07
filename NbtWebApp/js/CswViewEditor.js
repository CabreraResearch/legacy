; (function ($) {
	var PluginName = "CswViewEditor";

	var methods = {
		'init': function(options) 
			{
				var o = {
					ID: 'vieweditor'
				};
				if(options) $.extend(o, options);

				var $parent = $(this);
				var $div = $('<div></div>')
							.appendTo($parent);

				var $wizard = $div.CswWizard('init', { 
						'ID': o.ID + '_wizard',
						'Title': 'Edit View',
						'StepCount': 5,
						'Steps': 
						{ 
							1: 'Choose a View',
							2: 'Edit View Attributes',
							3: 'Add Relationships',
							4: 'Select Properties',
							5: 'Set Filters'
						},
						'FinishText': 'Save and Finish'
					});

				// Step 1 - Choose a View
				var $div1 = $wizard.CswWizard('div', 1);
				var instructions = "A <em>View</em> controls the arrangement of information you see in a tree or grid.  "+
								   "Views are useful for defining a user's workflow or for creating elaborate search criteria. "+
								   "This wizard will take you step by step through the process of creating a new View or "+
								   "editing an existing View.<br/><br/>";
				$div1.append(instructions);
				$div1.append('Select a View to Edit:');
				

				return $div;
			}
	};
	
	// Method calling logic
	$.fn.CswViewEditor = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

}) (jQuery);

