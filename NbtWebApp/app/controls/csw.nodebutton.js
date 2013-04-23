
(function ($) {
    "use strict";

    function onObjectClassButtonClick(opts, tabsAndProps) {
        var actionJson = opts.data.actionData;
        var launchAction = false;
        
        switch (Csw.string(opts.data.action).toLowerCase()) {
            case Csw.enums.nbtButtonAction.refresh:
                //1: Refresh this tab with new prop vals
                tabsAndProps.refresh(opts.data.savedprops.properties);
                break;
            case Csw.enums.nbtButtonAction.refreshall:
                //1: Trigger dialog close
                Csw.publish(Csw.enums.events.afterObjectClassButtonClick, opts.data.action);
                //2: refresh whole context (e.g view)
                Csw.publish(Csw.enums.events.main.refreshSelected, actionJson);
                break;
            case Csw.enums.nbtButtonAction.nothing:
                //1: Do nothing
                break;
            case Csw.enums.nbtButtonAction.creatematerial:
                actionJson.actionname = 'create material';
                actionJson.state.request = actionJson.request;
                launchAction = true;
                break;
            case Csw.enums.nbtButtonAction.move:
                actionJson.actionname = 'MoveContainer';
                launchAction = true;
                break;
            case Csw.enums.nbtButtonAction.dispense:
                actionJson.actionname = 'DispenseContainer';
                launchAction = true;
                break;
            case Csw.enums.nbtButtonAction.editprop:
                $.CswDialog('EditNodeDialog', {
                    currentNodeId: Csw.string(actionJson.nodeid),
                    filterToPropId: Csw.string(actionJson.propidattr),
                    title: Csw.string(actionJson.title),
                    onEditNode: function (nodeid, nodekey, close) {
                        tabsAndProps.refresh(opts.data.savedprops.properties);
                        Csw.tryExec(close);
                    }
                });
                break;

            case Csw.enums.nbtButtonAction.landingPage:
                Csw.publish('refreshLandingPage', actionJson.landingpage);
                break;

            case Csw.enums.nbtButtonAction.loadView:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                Csw.publish(Csw.enums.events.RestoreViewContext, actionJson);
                break;

            case Csw.enums.nbtButtonAction.popup:
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
                        Csw.publish(Csw.enums.events.main.reauthenticate, { username: result.username, customerid: result.customerid });
                        Csw.window.location('Main.html');
                    }
                });

                break;

            case Csw.enums.nbtButtonAction.receive:
                actionJson.actionname = 'Receiving';
                launchAction = true;
                break;

            case Csw.enums.nbtButtonAction.request:
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
                        var nodeGrid = Csw.nbt.nodeGrid(dialogDiv, {
                            nodeid: actionJson.nodeid,
                            readonly: false,
                            reinit: false,
                            viewid: actionJson.viewid,
                            onDeleteNode: function () {
                                nodeGrid.grid.reload();
                            },
                            onEditNode: function () {
                                nodeGrid.grid.reload();
                            },
                            onSuccess: function (cswGrid) {
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
                                        cswGrid.reload();
                                    },
                                    onMultiEdit: function () {
                                        cswGrid.toggleShowCheckboxes();
                                    },
                                    onEditView: function () {
                                        Csw.tryExec(menuDiv.$.dialog('close'));
                                    },
                                    onPrintView: function () {
                                        cswGrid.print();
                                    },
                                    Multi: false
                                };
                                menuDiv.menu(menuOpts);
                            }
                        });
                    }
                });
                break;

            case Csw.enums.nbtButtonAction.assignivglocation:
                actionJson.actionname = 'assign inventory groups';
                launchAction = true;
                break;

            default:
                tabsAndProps.refresh(opts.data.savedprops.properties);
                Csw.debug.error('No event has been defined for button click ' + opts.data.action);
                break;
        }
        if (launchAction) {
            //1: Clear the center divs
            Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
            //2: load th
            Csw.publish(Csw.enums.events.main.handleAction, actionJson);
        }
    }

    Csw.controls.nodeButton = Csw.controls.nodeButton ||
        Csw.controls.register('nodeButton', function (cswParent, options) {

            var cswPublic = {};
            var cswPrivate = {};
            var tabsAndProps;
            
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
                    menuOptions: [],
                    displayName: '',
                    icon: '',
                    nodeId: '',
                    tabId: '',
                    properties: {}
                };

                tabsAndProps = options.tabsAndProps;
                delete options.tabsAndProps;

                Csw.extend(cswPrivate, options, true);
                cswPrivate.div = cswParent.div();
                cswPrivate.div.empty();

                cswPrivate.table = cswPrivate.div.table();
            }());

            cswPrivate.onButtonClick = function() {
                cswPublic.button.disable();
                if (false === tabsAndProps.isFormValid()) {
                    //TODO: make a proper Csw\Ext Dialog class
                    window.Ext.MessageBox.alert('Warning', 'This form contains some invalid values. Please correct them before proceeding.', function () {
                        cswPublic.button.enable();
                        tabsAndProps.validator().focusInvalid();
                    });
                    
                } else {
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
                                    Props: Csw.serialize(tabsAndProps.getPropJson()),
                                    EditMode: tabsAndProps.getEditMode()
                                },
                                success: function(data) {
                                    Csw.clientChanges.unsetChanged();

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
                                        onObjectClassButtonClick(actionData, tabsAndProps);
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
                }
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
                            onClick: cswPrivate.onButtonClick
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
                            }
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

            }());

            return cswPublic;
        });

    
    

})(jQuery);
