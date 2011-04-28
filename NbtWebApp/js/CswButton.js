(function ($)
{
	var PluginName = "CswButton";

	var methods = {
		'init': function (options)
		{

			var o = {
				'ID': '',
				'prefix': '',
				'enabledText': '',
				'disabledText': '',
				'hasText': true,
				'disableOnClick': true,
				'enableDelay': 5000,
				'inputType': 'button',
				'primaryicon': '',
				'secondaryicon': '',
				'ReadOnly': false,
				//'Required': false,
				'onclick': function () { }
			};
			if (options) $.extend(o, options);

			var $parent = $(this);
			var elementId = makeId({ prefix: o.prefix, ID: o.ID });
			var $button = $('<input />').attr('type', o.inputType)
										.attr('id', elementId)
										.attr('name', elementId);
			$button.attr('enabledText', o.enabledText);
			$button.attr('disabledText', o.disabledText);


			var buttonOpt = {
				text: (o.hasText),
				label: o.enabledText,
				disabled: (o.ReadOnly),
				icons: {
					primary: o.primaryicon,
					secondary: o.secondaryicon
				}
			};
			if (buttonOpt.disabled)
			{
				buttonOpt.label = o.disabledText;
			}
			$button.button(buttonOpt)
					.click(function ()
					{
						if (o.disableOnClick) _disable($button);
						o.onclick();
						// case 21369 - enable after time delay
						setTimeout(function () { _enable($button); }, o.enableDelay);
					});
			$parent.append($button);
			return $button;
		},

		'enable': function ()
		{
			var $button = $(this);
			_enable($button);
		},
		'disable': function ()
		{
			var $button = $(this);
			_disable($button);
		}
	};

	function _enable($button)
	{
		if ($button.length > 0)
			$button.button({ label: $button.attr('enabledText'), disabled: false });
	}
	function _disable($button)
	{
		if ($button.length > 0)
			$button.button({ label: $button.attr('disabledText'), disabled: true });
	}

	// Method calling logic
	$.fn.CswButton = function (method)
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
