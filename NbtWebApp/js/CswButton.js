﻿(function ($)
{
    $.fn.CswButton = function (options)
    {

        var o = {
            'ID': '',
            'prefix': '',
            'enabledText': '',
            'disabledText': '',
            'hasText': true,
            'disableOnClick': true,
            'enableAfterClick': false,
            'inputType': 'button',
            'primaryicon': '',
            'secondaryicon': '',
            'ReadOnly': false,
            //'Required': false,
            'onclick': function () { }
        };
        if (options) $.extend(o, options);

        var $parent = $(this);
        var elementId = makeId({prefix: o.prefix, ID: o.ID});
        var $button = $('<input />').attr('type', o.inputType)
                                    .attr('id', elementId)
                                    .attr('name', elementId);

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
                    $thisButton = $(this);
                    if (o.disableOnClick) $thisButton.button({ label: o.disabledText, disabled: true });
                    o.onclick();
                    if (o.enableAfterClick) $thisButton.button({ label: o.enabledText, disabled: false });
                });
        $parent.append($button);
        return $button;
    };
})(jQuery);
