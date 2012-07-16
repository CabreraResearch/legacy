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
                        NodeTypePropAttr: propAttr,
                        SelectedText: Csw.string(button.selectedOption, Csw.string(o.propData.values.text, o.propData.name))
                    };

                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/onObjectClassButtonClick',
                        data: params,
                        success: function (data) {
                            $btn.button({ disabled: false });

                            var actionData = {
                                data: data,
                                propid: propid,
                                button: button,
                                selectedOption: Csw.string(button.selectedOption),
                                messagediv: messagediv,
                                context: o,
                                onSuccess: o.onAfterButtonClick
                            };

                            if (false === Csw.isNullOrEmpty(data.message)) {
                                // can't use messagediv, since doSave has remade the tab
                                var $newmessagediv = $('#' + messagediv.getId());
                                $newmessagediv.text(data.message);
                            }

                            if (Csw.bool(data.success)) {
                                if (data.action == Csw.enums.nbtButtonAction.refresh) { //cases 26201, 26107 
                                    Csw.tryExec(o.onReload(
                                        (function(messagedivid) {
                                            return function() {
                                                if (false === Csw.isNullOrEmpty(data.message)) {
                                                    var $newmessagediv = $('#' + messagedivid);
                                                    $newmessagediv.text(data.message);
                                                }
                                            };
                                        })(messagediv.getId())
                                    ));
                                } else {
                                    Csw.publish(Csw.enums.events.objectClassButtonClick, actionData);
                                }
                            }
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
                table, btnCell,
                button, menuoptions, state, text;

            menuoptions = propVals.menuoptions.split(',');
            state = propVals.state;
            text = propVals.text;
            
            function onClick() {
                onButtonClick(o.propid, button, messagediv, o);
            }

            table = propDiv.table({
                ID: Csw.makeId(o.ID, 'tbl')
            });
            btnCell = table.cell(1, 1);
            switch (mode) {
                case 'button':
                    button = btnCell.button({
                        ID: o.ID,
                        enabledText: value,
                        disabledText: value,
                        disableOnClick: true,
                        onClick: onClick
                    });
                    break;
                case 'menu':
                    button = btnCell.menuButton({
                        ID: Csw.makeId(o.ID, 'menuBtn'),
                        selectedText: text,
                        menuOptions: menuoptions,
                        size: o.size,
                        state: state,
                        onClick: function(selectedOption) {
                            Csw.tryExec(onClick, selectedOption);
                        }
                    });
                    break;
                case 'link': //this is a fallthrough case
                default:
                    button = btnCell.a({
                        ID: o.ID,
                        value: value,
                        onClick: onClick
                    });
                    break;
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
