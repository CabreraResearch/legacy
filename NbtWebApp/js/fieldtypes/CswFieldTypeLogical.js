; (function ($) {
		
	var PluginName = 'CswFieldTypeLogical';

	var methods = {
		init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

			$Div = $(this);
			$Div.contents().remove();

			var Checked = o.$propxml.children('checked').text().trim();

			if(o.ReadOnly)
			{
				switch(Checked)
				{
					case "true": $Div.append('Yes'); break;
					case "false": $Div.append('No'); break;
				}
			} 
			else 
			{
				var thisButtonType;
				switch(Checked)
				{
					case "true": thisButtonType = CswImageButton_ButtonType.CheckboxTrue; break;
					case "false": thisButtonType = CswImageButton_ButtonType.CheckboxFalse; break;
					default: thisButtonType = CswImageButton_ButtonType.CheckboxNull; break;
				}

				$Div.CswImageButton({ ButtonType: thisButtonType, 
										AlternateText: Checked,
										onClick: function($ImageDiv) {
													var newvalue = onClick($ImageDiv, o.Required);
													o.onchange(); 
													return newvalue;
												}
									});
			}
		},
		save: function(o) { //$propdiv, $xml
				var $CheckboxImage = o.$propdiv.find('div');
				o.$propxml.children('checked').text($CheckboxImage.attr('title'));
			}
	};
	

	function onClick($ImageDiv, Required)
	{
		var currentValue = $ImageDiv.attr('alt');
		var newValue = CswImageButton_ButtonType.CheckboxNull;
		var newAltText = "null";
		if (currentValue == "null") {
			newValue = CswImageButton_ButtonType.CheckboxTrue;
			newAltText = "true";
		} else if (currentValue == "false") {
			if (Required == "true") {
				newValue = CswImageButton_ButtonType.CheckboxTrue;
				newAltText = "true";
			} else {
				newValue = CswImageButton_ButtonType.CheckboxNull;
				newAltText = "null";
			}
		} else if (currentValue == "true") {
			newValue = CswImageButton_ButtonType.CheckboxFalse;
			newAltText = "false";
		}
		$ImageDiv.attr('alt', newAltText);
		return newValue;
	} // onClick()



	// Method calling logic
	$.fn.CswFieldTypeLogical = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
})(jQuery);





