; (function ($)
{

	var PluginName = 'CswButton';

	var methods = {
		init: function (options)
		{
			var o = {
                'ID': '',
				'enabledText': '',
                'disabledText': '',
                'hasText': true,
                'disableOnClick': true,
                'primaryicon': '',
                'secondaryicon': '',
				'ReadOnly': false,
				'Required': false,
				'onclick': function() { }
			};
			if(options) $.extend(o, options);

			var $parent = $(this);

            var buttonOpt = {
                text: (o.hasText),
                label: o.enabledText,
                disabled: (o.ReadOnly),
                icons: {
                            primary: o.primaryicon,
                            secondary: o.secondaryicon
                        }
            };
            if( buttonOpt.disabled )
            {
                buttonOpt.label = o.disabledText;
            }
            $parent.button(buttonOpt)
                   .click(function() {
                        $thisButton = $(this);
                        if(o.disableOnClick) $thisButton.button({label: o.disabledText, disabled: true});
                        o.onclick();
                        if(o.disableOnClick) $thisButton.button({label: o.enabledText, disabled: false});
                   })
                   .attr('id',o.ID)
                   .attr('name',o.ID);

		}
	};

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
