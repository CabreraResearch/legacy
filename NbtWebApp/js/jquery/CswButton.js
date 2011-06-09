/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

(function ($) { /// <param name="$" type="jQuery" />

	var PluginName = "CswButton";
    
	var methods = {
		'init': function (options)
		{

			var o = {
				'ID': '',
				'enabledText': '',
				'disabledText': '',
				'hasText': true,
				'disableOnClick': true,
				'inputType': 'button',
				'primaryicon': '',
				'secondaryicon': '',
				'ReadOnly': false,
				//'Required': false,
				'onclick': function () { }
			};
			if (options) $.extend(o, options);

			var $parent = $(this);
			var $button = $('<input />').CswAttrDom('type', o.inputType)
										.CswAttrDom('id', o.ID)
										.CswAttrDom('name', o.ID);
			$button.CswAttrDom('enabledText', o.enabledText);
			$button.CswAttrDom('disabledText', o.disabledText);


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
			$button.button({ label: $button.CswAttrDom('enabledText'), disabled: false });
	}
	function _disable($button)
	{
		if ($button.length > 0)
			$button.button({ label: $button.CswAttrDom('disabledText'), disabled: true });
	}

	// Method calling logic
	$.fn.CswButton = function (method) { /// <param name="$" type="jQuery" />

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
