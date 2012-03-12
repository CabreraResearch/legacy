/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

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
            o.doSave({
                onSuccess: function () {

                    params = {
                        NodeTypePropAttr: propAttr
                    };

                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/onObjectClassButtonClick',
                        data: params,
                        success: function (data) {
                            button.enable();
                            if (Csw.bool(data.success)) {

                                if (false === Csw.isNullOrEmpty(data.message)) {
                                    messagediv.text(data.message);
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
                                                    window.location = "Main.html";
                                                }
                                            });
                                        }
                                        break;

                                    case Csw.enums.nbtButtonAction.refresh:
                                        if (false === Csw.isNullOrEmpty(data.message)) {
                                            var MessageHandler = function (event, newmessagediv) {
                                                $(newmessagediv).text(data.message);
                                                Csw.unsubscribe(o.ID + 'CswFieldTypeButton_MessageHandler', MessageHandler);
                                            };
                                            Csw.subscribe(o.ID + 'CswFieldTypeButton_MessageHandler', MessageHandler);
                                        }
                                        o.onReload();
                                        break;
                                    case Csw.enums.nbtButtonAction.popup:
                                        Csw.openPopup(data.actiondata, 600, 800);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }, // ajax success()
                        error: function () {
                            button.enable();
                        }
                    }); // ajax.post()
                } // doSave.onSuccess()
            }); // doSave()
        }// if-else (Csw.isNullOrEmpty(propAttr)) {
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
                ID: Csw.controls.dom.makeId(o.ID, '', 'tbl')
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
                button = table.cell(1, 1).link({
                    ID: o.ID,
                    value: value,
                    onClick: onClick
                });
            }

            if (Csw.bool(o.ReadOnly)) {
                button.disable();
            }

            messagediv = table.cell(1, 2).div({
                ID: Csw.controls.dom.makeId(o.ID, '', 'msg'),
                cssclass: 'buttonmessage'
            });

            if (o.Required) {
                button.addClass('required');
            }

            Csw.publish(o.ID + 'CswFieldTypeButton_MessageHandler', messagediv.$);

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
