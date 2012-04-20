(function ($) {
    // adapted from http://plugins.jquery.com/project/SafeEnter
    // by teedyay
    $.fn.listenForEnter = function () {
        'use strict';
        var $elements = this;
        return $elements.each(function () {
            var $element = $(this);
            $element.focus(function () {
                $element.data('safeEnter_InAutocomplete', false);
            });
            $element.keypress(function (e) {
                var key = (e.keyCode ? e.keyCode : e.which);
                switch (key) {
                    case 13:
                        // Fire the event if:
                        //   - we're not currently in the browser's Autocomplete, or
                        //   - this isn't a textbox, or
                        //   - this is Opera (which provides its own protection)
                        if (!$element.data('safeEnter_InAutocomplete') || !$element.is('input[type=text]') || $.browser.opera) {
                            $element.trigger('pressedEnter', e);
                        }
                        $element.data('safeEnter_InAutocomplete', false);
                        break;
                    case 40:
                    case 38:
                    case 34:
                    case 33:
                        // down=40,up=38,pgdn=34,pgup=33
                        $element.data('safeEnter_InAutocomplete', true);
                        break;
                    default:
                        $element.data('safeEnter_InAutocomplete', false);
                        break;
                }
            });
        });
    };

    $.fn.clickOnEnter = function ($target) {
        'use strict';
        var $parents = this;
        return $parents.each(function () {
            var $parent = $(this);
            $parent.listenForEnter()
                .bind('pressedEnter', function () {
                    $target.click();
                    $parent.unbind('pressedEnter');
                });
        });
    };

})(jQuery);


