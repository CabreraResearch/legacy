; (function ($)
{

	var PluginName = 'CswTristateCheckBox';

	var methods = {
		init: function (options)
		{
			var o = {
				'ID': '',
                'prefix': '',
				'Checked': '',
				'ReadOnly': false,
				'Required': false,
				'onchange': function() { }
			};
			if(options) $.extend(o, options);

			var $parent = $(this);
            $parent.empty();
            var elementId = o.prefix + '_' + o.ID;
			if(o.ReadOnly)
			{
				switch(o.Checked)
				{
					case "true": $parent.append('Yes'); break;
					case "false": $parent.append('No'); break;
				}
			} 
			else 
			{
				var thisButtonType;
				switch(o.Checked)
				{
					case "true": thisButtonType = CswImageButton_ButtonType.CheckboxTrue; break;
					case "false": thisButtonType = CswImageButton_ButtonType.CheckboxFalse; break;
					default: thisButtonType = CswImageButton_ButtonType.CheckboxNull; break;
				}

				$parent.CswImageButton({ ID: elementId,  
                                        ButtonType: thisButtonType, 
										AlternateText: o.Checked,
										onClick: function($ImageDiv) {
													var newvalue = onClick($ImageDiv, o.Required);
													o.onchange(); 
													return newvalue;
												}
									});
			}
		},
		value: function ()
		{
			var $CheckboxImage = $(this);
			var Checked = $CheckboxImage.attr('alt');
            return Checked;
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
	$.fn.CswTristateCheckBox = function (method)
	{

		if (methods[method])
		{
			return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
		} else if (typeof method === 'object' || !method)
		{
			return methods.init.apply(this, arguments);
		} else
		{
			$.error('Method ' + method + ' does not exist on ' + PluginName);
		}

	};
})(jQuery);
