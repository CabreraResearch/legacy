/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";
    var pluginName = 'CswFieldTypeButton';

    var onButtonClick = function (propid, $button, o) {
        var propAttr = Csw.string(propid),
            params;

        $button.CswButton('disable');
        if (Csw.isNullOrEmpty(propAttr)) {
            Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Cannot execute a property\'s button click event without a valid property.', 'Attempted to click a property button with a null or empty propid.'));
            $button.CswButton('enable');
        } else {
            params = {
                NodeTypePropAttr: propAttr
            };

            Csw.ajax.post({
                url: '/NbtWebApp/wsNBT.asmx/onObjectClassButtonClick',
                data: params,
                success: function (data) {
                    $button.CswButton('enable');
                    if (Csw.bool(data.success)) {
                        switch (data.action) {
                            case Csw.enums.onObjectClassClick.reauthenticate:
                                if (Csw.clientChanges.manuallyCheckChanges()) {
                                    /* case 24669 */
                                    Csw.cookie.clearAll();
                                    Csw.ajax.post({
                                        url: '/NbtWebApp/wsNBT.asmx/reauthenticate',
                                        data: { PropId: propAttr },
                                        success: function () {
                                            Csw.clientChanges.unsetChanged();
                                            window.location = "Main.html";
                                        }
                                    });
                                }
                                break;
                            case Csw.enums.onObjectClassClick.refresh:
                                o.onReload();
                                break;
                            default:
                                /* Nada */
                                break;
                        }
                    }
                },
                error: function () {
                    $button.CswButton('enable');
                }
            });
        }
    };

    var methods = {
        init: function (o) { 

            var $Div = $(this);
            $Div.contents().remove();

            var propVals = o.propData.values,
                value = Csw.string(propVals.text, o.propData.name),
                mode = Csw.string(propVals.mode, 'button'),
                $button;

            if (mode === 'button') {
                $button = $Div.CswButton('init', {
                    ID: o.ID,
                    enabledText: value,
                    disabledText: value,
                    disableOnClick: true
                });
            }
            else {
                $button = $Div.CswLink('init', {
                    ID: o.ID,
                    value: value,
                    href: '#'
                });
            }
            $button.click(function () {
                onButtonClick(o.propid, $button, o);
            });

            if (o.Required) {
                $button.addClass('required');
            }
        },
        save: function (o) {
            Csw.preparePropJsonForSave(o.propData);
        }
    };

    $.fn.CswFieldTypeButton = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }
    };
})(jQuery);
