
(function ($) {
    "use strict";

    Csw.controls.nodeButton = Csw.controls.nodeButton ||
        Csw.controls.register('nodeButton', function (cswParent, options) {

            var cswPublic = {};
            var cswPrivate = {};

            Csw.tryExec(function () {

                (function _pre() {
                    cswPrivate = {
                        name: 'nodebutton',
                        div: {},
                        value: '',
                        mode: 'button',
                        useToolTip: true,
                        messageDiv: {},
                        state: '',
                        confirmmessage: '',
                        table: {},
                        btnCell: {},
                        size: 'small',
                        propId: '',
                        onClickAction: null,
                        disabled: false,
                        menuOptions: [],
                        displayName: '',
                        icon: '',
                        nodeId: '',
                        tabId: ''
                    };
                    Csw.extend(cswPrivate, options, true);
                    cswPrivate.div = cswParent.div();
                    cswPrivate.div.empty();

                    cswPrivate.table = cswPrivate.div.table();
                } ());

                cswPrivate.onButtonClick = function () {
                    cswPublic.button.disable();
                    
                    if (options.saveTheCurrentTab) {
                        options.saveTheCurrentTab(cswPrivate.tabId);
                    }
                    
                    if (Csw.isNullOrEmpty(cswPrivate.propId)) {
                        Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Cannot execute a property\'s button click event without a valid property.', 'Attempted to click a property button with a null or empty propid.'));
                        cswPublic.button.enable();
                    } else {
                        // Case 27263: prompt to save instead

                        var performOnObjectClassButtonClick = function() {
                            Csw.ajax.post({
                                urlMethod: 'onObjectClassButtonClick',
                                data: {
                                    NodeTypePropAttr: cswPrivate.propId,
                                    SelectedText: Csw.string(cswPublic.button.selectedOption, Csw.string(cswPrivate.value)),
                                    TabId: cswPrivate.tabId,
                                    Props: cswPrivate.propertiesForSave || ''
                                },
                                success: function(data) {
                                    cswPublic.button.enable();

                                    var actionData = {
                                        data: data,
                                        propid: cswPrivate.propId,
                                        button: cswPublic.button,
                                        selectedOption: Csw.string(cswPublic.button.selectedOption),
                                        messagediv: cswPrivate.messageDiv,
                                        context: cswPrivate,
                                        onSuccess: cswPrivate.onAfterButtonClick
                                    };

                                    if (false === Csw.isNullOrEmpty(data.message)) {
                                        if (false === cswPrivate.useToolTip) {
                                            cswPublic.messageDiv.text(data.message);
                                        } else {
                                            cswPrivate.btnCell.quickTip({ html: data.message });
                                        }
                                    }
                                    if (Csw.bool(data.success)) {
                                        Csw.publish(Csw.enums.events.objectClassButtonClick, actionData);
                                    }
                                }, // ajax success()
                                error: function() {
                                    cswPublic.button.enable();
                                }
                            }); // ajax.post()
                        }; //performOnObjectClassButtonClick

                        if (false === Csw.isNullOrEmpty(cswPrivate.confirmmessage)) {
                            $.CswDialog('GenericDialog', {
                                name: 'ButtonConfirmationDialog',
                                title: 'Confirm ' + Csw.string(cswPrivate.value),
                                height: 150,
                                width: 400,
                                div: Csw.literals.div({ text: cswPrivate.confirmmessage, align: 'center' }),
                                onOk: function(selectedOption) {
                                    performOnObjectClassButtonClick();
                                },
                                onCancel: function() {
                                    cswPublic.button.enable();
                                },
                                onClose: function() {
                                    cswPublic.button.enable();
                                }
                            });
                        } else {
                            performOnObjectClassButtonClick();
                        }
                    } // if-else (Csw.isNullOrEmpty(propAttr)) {

                    
                        
                        
                    
                }; // onButtonClick()

                (function _post() {
                    cswPrivate.btnCell = cswPrivate.table.cell(1, 1).div();
                    if (cswPrivate.menuOptions && cswPrivate.menuOptions.length > 0) {
                        cswPrivate.mode = 'menu';
                    }
                    switch (cswPrivate.mode) {
                        case 'button':
                            
                            cswPublic.button = cswPrivate.btnCell.buttonExt({
                                size: cswPrivate.size,
                                icon: cswPrivate.icon,
                                enabledText: cswPrivate.displayName,
                                disabledText: cswPrivate.displayName,
                                disableOnClick: true,
                                onClick: cswPrivate.onButtonClick,
                                disabled: cswPrivate.disabled
                            });
                            break;
                        case 'menu':
                            cswPublic.button = cswPrivate.btnCell.menuButton({
                                icon: cswPrivate.icon,
                                selectedText: cswPrivate.selectedText,
                                menuOptions: cswPrivate.menuOptions,
                                size: cswPrivate.size,
                                state: Csw.string(cswPrivate.state, cswPrivate.value),
                                onClick: function (selectedOption) {
                                    cswPrivate.selectedText = selectedOption;
                                    cswPrivate.onButtonClick();
                                },
                                disabled: cswPrivate.disabled
                            });
                            break;
                        case 'landingpage':
                            //landing page handles the button - just execute the onClick event
                            cswPublic.button = cswPrivate.btnCell.a().hide();
                            cswPrivate.onButtonClick();
                            break;
                        default:
                            cswPublic.button = cswPrivate.btnCell.a({
                                value: cswPrivate.value,
                                onClick: cswPrivate.onButtonClick
                            });
                            break;
                    }

                    if (Csw.bool(cswPrivate.disabled)) {
                        cswPublic.button.disable();
                    }

                    cswPublic.messageDiv = cswPrivate.table.cell(1, 2).div({
                        cssclass: 'buttonmessage'
                    });

                } ());
            });
            return cswPublic;
        });

    function onObjectClassButtonClick(eventOj, opts) {
        Csw.debug.assert(false === Csw.isNullOrEmpty(opts.data), 'opts.data is null.');
        var actionJson = opts.data.actionData;
        Csw.publish(Csw.enums.events.afterObjectClassButtonClick, opts.data.action);
        switch (Csw.string(opts.data.action).toLowerCase()) {
            case Csw.enums.nbtButtonAction.refresh:
                Csw.publish(Csw.enums.events.main.refreshSelected, actionJson);
                break;
            case Csw.enums.nbtButtonAction.nothing:
                //Do nothing
                break;
            case Csw.enums.nbtButtonAction.creatematerial:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                actionJson.actionname = 'create material';
                actionJson.state.request = actionJson.request;
                Csw.publish(Csw.enums.events.main.handleAction, actionJson);
                break;
            case Csw.enums.nbtButtonAction.move:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                actionJson.actionname = 'MoveContainer';
                Csw.publish(Csw.enums.events.main.handleAction, actionJson);
                break;
            case Csw.enums.nbtButtonAction.dispense:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                actionJson.actionname = 'DispenseContainer';
                Csw.publish(Csw.enums.events.main.handleAction, actionJson);
                break;
            case Csw.enums.nbtButtonAction.editprop:
                $.CswDialog('EditNodeDialog', {
                    currentNodeId: Csw.string(actionJson.nodeid),
                    filterToPropId: Csw.string(actionJson.propidattr),
                    title: Csw.string(actionJson.title),
                    onEditNode: function (nodeid, nodekey, close) {
                        Csw.tryExec(close);
                    }
                });
                break;

            case Csw.enums.nbtButtonAction.landingPage:
                Csw.publish('refreshLandingPage', actionJson.landingpage);
                break;

            case Csw.enums.nbtButtonAction.loadView:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                Csw.debug.assert(false === Csw.isNullOrEmpty(actionJson), 'actionJson is null.');
                Csw.publish(Csw.enums.events.RestoreViewContext, actionJson);
                break;

            case Csw.enums.nbtButtonAction.popup:
                Csw.debug.assert(false === Csw.isNullOrEmpty(actionJson), 'actionJson is null.');
                Csw.openPopup(actionJson.url);
                break;

            case Csw.enums.nbtButtonAction.reauthenticate:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                /* case 24669 */
                Csw.cookie.clearAll();
                Csw.ajax.post({
                    urlMethod: 'reauthenticate',
                    data: { PropId: Csw.string(opts.propid) },
                    success: function (result) {
                        Csw.clientChanges.unsetChanged();
                        Csw.publish(Csw.enums.events.main.reauthenticate, result.username);
                        Csw.window.location('Main.html');
                    }
                });

                break;

            case Csw.enums.nbtButtonAction.receive:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                actionJson.actionname = 'Receiving';
                Csw.publish(Csw.enums.events.main.handleAction, actionJson);
                break;

            case Csw.enums.nbtButtonAction.request:
                Csw.debug.assert(false === Csw.isNullOrEmpty(actionJson), 'actionJson is null.');
                switch (actionJson.requestaction) {
                    case 'Dispose':
                        Csw.publish(Csw.enums.events.main.refreshHeader);
                        break;
                    default:
                        $.CswDialog('AddNodeDialog', {
                            nodetypeid: actionJson.requestItemNodeTypeId,
                            propertyData: actionJson.requestItemProps,
                            text: actionJson.titleText,
                            onSaveImmediate: function () {
                                Csw.publish(Csw.enums.events.main.refreshHeader);
                            }
                        });
                        break;
                }
                break;

            case Csw.enums.nbtButtonAction.griddialog:
                $.CswDialog('OpenEmptyDialog', {
                    title: actionJson.title,
                    onOpen: function (dialogDiv) {
                        var menuDiv = dialogDiv.div();
                        var grid = Csw.nbt.nodeGrid(dialogDiv, {
                            nodeid: actionJson.nodeid,
                            readonly: false,
                            reinit: false,
                            viewid: actionJson.viewid,
                            onDeleteNode: function () {
                                grid.reload();
                            },
                            onEditNode: function () {
                                grid.reload();
                            },
                            onSuccess: function (grid) {
                                var menuOpts = {
                                    width: 150,
                                    ajax: {
                                        urlMethod: 'getMainMenu',
                                        data: {
                                            ViewId: actionJson.viewid,
                                            SafeNodeKey: '',
                                            NodeId: actionJson.nodeid,
                                            NodeTypeId: actionJson.nodetypeid,
                                            PropIdAttr: '',
                                            LimitMenuTo: '',
                                            ReadOnly: 'false'
                                        }
                                    },
                                    onAlterNode: function () {
                                        grid.reload();
                                    },
                                    onMultiEdit: function () {
                                        grid.toggleShowCheckboxes();
                                    },
                                    onEditView: function () {
                                        Csw.tryExec(menuDiv.$.dialog('close'));
                                    },
                                    onPrintView: function () {
                                        grid.print();
                                    },
                                    Multi: false
                                };
                                menuDiv.menu(menuOpts);
                            }
                        });
                    }
                });
                break;

            default:
                Csw.debug.error('No event has been defined for button click ' + opts.data.action);
                break;
        }
    }
    Csw.subscribe(Csw.enums.events.objectClassButtonClick, onObjectClassButtonClick);

})(jQuery);
