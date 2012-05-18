/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeButton';

    var onButtonClick = function (propid, button, messagediv, o) {
        var propAttr = Csw.string(propid),
            params;
        button.disable();
        if (Csw.isNullOrEmpty(propAttr)) {
            Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Cannot execute a property\'s button click event without a valid property.', 'Attempted to click a property button with a null or empty propid.'));
            button.enable();
        } else {
            // case 25371 - Save the tab first
            Csw.tryExec(o.doSave, {
                onSuccess: function () {
                    var $btn = $('#' + o.ID).button({ disabled: true });
                    params = {
                        NodeTypePropAttr: propAttr
                    };

                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/onObjectClassButtonClick',
                        data: params,
                        success: function (data) {
                            if (Csw.bool(data.success)) {
                                if (false === Csw.isNullOrEmpty(data.message)) {
                                    // can't use messagediv, since doSave has remade the tab
                                    var $newmessagediv = $('#' + messagediv.getId());
                                    $newmessagediv.text(data.message);
                                }

                                switch (data.action) {
                                    case Csw.enums.nbtButtonAction.reauthenticate:
                                        if (Csw.clientChanges.manuallyCheckChanges()) {
                                            /* case 24669 */
                                            Csw.cookie.clearAll();
                                            Csw.ajax.post({
                                                url: '/NbtWebApp/wsNBT.asmx/reauthenticate',
                                                data: { PropId: propAttr },
                                                success: function () {
                                                    Csw.clientChanges.unsetChanged();
                                                    Csw.window.location("Main.html");
                                                }
                                            });
                                        }
                                        break;

                                    case Csw.enums.nbtButtonAction.refresh: //cases 26201, 26107 
                                        $btn.button({ disabled: false });
                                        Csw.tryExec(o.onReload(
                                            (function (messagedivid) {
                                                return function () {
                                                    if (false === Csw.isNullOrEmpty(data.message)) {
                                                        var $newmessagediv = $('#' + messagedivid);
                                                        $newmessagediv.text(data.message);
                                                    }
                                                };
                                            })(messagediv.getId())
                                        ));
                                        break;
                                    case Csw.enums.nbtButtonAction.popup:
                                        $btn.button({disabled: false});
                                        Csw.openPopup(data.actiondata, 600, 800);
                                        break;
                                    default:
                                        $btn.button({disabled: false});
                                        break;
                                }
                            }
                            Csw.tryExec(o.onAfterButtonClick);
                        }, // ajax success()
                        error: function () {
                            button.enable();
                        }
                    }); // ajax.post()
                } // doSave.onSuccess()
            }); // doSave()
        } // if-else (Csw.isNullOrEmpty(propAttr)) {
    }; // onButtonClick()

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();

            var propVals = o.propData.values,
                value = Csw.string(propVals.text, o.propData.name),
                mode = Csw.string(propVals.mode, 'button'),
                messagediv,
                table,
                button;

            function onClick() {
                onButtonClick(o.propid, button, messagediv, o);
            }

            table = propDiv.table({
                ID: Csw.makeId(o.ID, '', 'tbl')
            });

            if (mode === 'button') {
                button = table.cell(1, 1).button({
                    ID: o.ID,
                    enabledText: value,
                    disabledText: value,
                    disableOnClick: true,
                    onClick: onClick
                });
            }
            else {
                button = table.cell(1, 1).a({
                    ID: o.ID,
                    value: value,
                    onClick: onClick
                });
            }

            if (Csw.bool(o.ReadOnly)) {
                button.disable();
            }

            messagediv = table.cell(1, 2).div({
                ID: Csw.makeId(o.ID, '', 'msg', '', false),
                cssclass: 'buttonmessage'
            });

            if (o.Required) {
                button.addClass('required');
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
