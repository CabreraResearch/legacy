/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />

	var PluginName = 'CswNumberTextBox';

	var methods = {
		init: function (options)
		{
			var o = {
				'ID': '',
				'Value': '',
				'MinValue': '',
				'MaxValue': '',
				'Precision': '',
				'ReadOnly': false,
				'Required': false,
				'onchange': function() { },
				'width': ''
			};
			if(options) $.extend(o, options);

			var $Div = $(this);
			//$Div.contents().remove();

			if (o.ReadOnly)
			{
				$Div.append(o.Value);
			}
			else
			{
				var $TextBox = $Div.CswInput('init',{ID: o.ID,
														type: CswInput_Types.text,
														value: o.Value,
														cssclass: 'textinput number',
														onChange: o.onchange,
														width: o.width
													 }); 

				if (o.MinValue !== undefined)
				{
					jQuery.validator.addMethod(o.ID + "_validateFloatMinValue", function (value, element)
					{
						return (this.optional(element) || validateFloatMinValue($(element).val(), o.MinValue));
					}, 'Number must be greater than or equal to ' + o.MinValue);
					$TextBox.addClass(o.ID + "_validateFloatMinValue");
				}
				if (o.MaxValue !== undefined)
				{
					jQuery.validator.addMethod(o.ID + "_validateFloatMaxValue", function (value, element)
					{
						return (this.optional(element) || validateFloatMaxValue($(element).val(), o.MaxValue));
					}, 'Number must be less than or equal to ' + o.MaxValue);
					$TextBox.addClass(o.ID + "_validateFloatMaxValue");
				}
				if (o.Precision === undefined || o.Precision <= 0)
				{
					jQuery.validator.addMethod(o.ID + "_validateInteger", function (value, element)
					{
						return (this.optional(element) || validateInteger($(element).val()));
					}, 'Value must be an integer');
					$TextBox.addClass(o.ID + "_validateInteger");
				} else
				{
					jQuery.validator.addMethod(o.ID + "_validateFloatPrecision", function (value, element)
					{
						return (this.optional(element) || validateFloatPrecision($(element).val(), o.Precision));
					}, 'Value must be numeric');
					$TextBox.addClass(o.ID + "_validateFloatPrecision");
				}

				if (o.Required)
				{
					$TextBox.addClass("required");
				}
				return $TextBox;
			}
		},
		value: function (id)
		{
			var $Div = $(this);
			var $TextBox = $Div.find('input[id="'+id+'"]');
			return $TextBox.val();
		},
		setValue: function (id, newvalue)
		{
			var $Div = $(this);
			var $TextBox = $Div.find('input[id="'+id+'"]');
			if( newvalue !== undefined )
			{
				$TextBox.val( newvalue );
			}
		}
	};

	// Method calling logic
	$.fn.CswNumberTextBox = function (method)
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
