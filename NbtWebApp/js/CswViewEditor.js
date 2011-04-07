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

				var wizopts = 
				{ 
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
				};
				var $wizard = $div.CswWizard('init', wizopts); 

				
				

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

