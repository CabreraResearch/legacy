/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    var pluginName = "CswButton";

    var methods = {
        'init': function (options) {

            var o = {
                ID: '',
                enabledText: '',
                disabledText: '',
                cssclass: '',
                hasText: true,
                disableOnClick: true,
                inputType: CswInput_Types.button.name,
                primaryicon: '',
                secondaryicon: '',
                ReadOnly: false,
                //'Required': false,
                onclick: null //function () { }
            };
            if (options) $.extend(o, options);

            var $parent = $(this);
            var $button = $('<input />').CswAttrDom({type: o.inputType,
                                                     id: o.ID, 
                                                     name: o.ID,
                                                     enabledText: o.enabledText,
                                                     disabledText: o.disabledText
                                        })
                                        .appendTo($parent);

            if (!isNullOrEmpty(o.cssclass)) {
                $button.addClass(o.cssclass);
            }
            
            var buttonOpt = {
                text: (o.hasText),
                label: o.enabledText,
                disabled: (o.ReadOnly),
                icons: {
                    primary: o.primaryicon,
                    secondary: o.secondaryicon
                }
            };
            if (buttonOpt.disabled) {
                buttonOpt.label = o.disabledText;
            }
            $button.button(buttonOpt);
            
            if (isFunction(o.onclick)) {
                $button.bind('click', function() {
                    if (!ajaxInProgress()) {
                        if (o.disableOnClick) disable($button);
                        o.onclick();
                    } 
                    return false;
                });
            } 

            return $button;
        },

        'enable': function () {
            var $button = $(this);
            enable($button);
            return $button;
        },
        'disable': function () {
            var $button = $(this);
            disable($button);
            return $button;
        },
        'click': function () {
            var $button = $(this);
            $button.click();
            return $button;
        }
    };

    function enable($button) {
        if ($button.length > 0)
            $button.button({ label: $button.CswAttrDom('enabledText'), disabled: false });
    }
    function disable($button) {
        if ($button.length > 0)
            $button.button({ label: $button.CswAttrDom('disabledText'), disabled: true });
    }

    // Method calling logic
    $.fn.CswButton = function (method) { /// <param name="$" type="jQuery" />

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
            return false;
        }
    };

})(jQuery);
