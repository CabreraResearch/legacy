
(function ($) {
    "use strict";

    function onObjectClassButtonClick(opts, tabsAndProps, onRefresh) {
        var actionJson = opts.data.actionData;
        var launchAction = false;

        switch (Csw.string(opts.data.action).toLowerCase()) {
            case Csw.enums.nbtButtonAction.batchop:
                if (tabsAndProps) {
                    tabsAndProps.refresh(opts.data.savedprops.properties);
                    Csw.tryExec(onRefresh);
                }
                if (false === Csw.isNullOrEmpty(actionJson.batch)) {
                    $.CswDialog('BatchOpDialog', {
                        opname: 'multi-edit',
                        batch: actionJson.batch
                    });
                }
                break;
            case Csw.enums.nbtButtonAction.refresh:
                //1: Refresh this tab with new prop vals
                if (tabsAndProps) {
                    tabsAndProps.refresh(opts.data.savedprops.properties);
                    Csw.tryExec(onRefresh);
                } else {
                    Csw.publish(Csw.enums.events.main.refreshSelected, actionJson);
                    Csw.tryExec(onRefresh);
                }
                break;
            case Csw.enums.nbtButtonAction.refreshonadd:
                if (tabsAndProps) {
                    tabsAndProps.refreshOnAdd(opts.data.savedprops.node);
                    tabsAndProps.tearDown();
                    Csw.tryExec(onRefresh);
                } else {
                    Csw.publish(Csw.enums.events.main.refreshSelected, actionJson);
                    Csw.tryExec(onRefresh);
                }
                break;
            case Csw.enums.nbtButtonAction.nothing:
                //1: Do nothing _except_ reenable the button
                Csw.publish('onAnyNodeButtonClickFinish', true);
                break;
            case Csw.enums.nbtButtonAction.creatematerial:
                actionJson.actionname = 'create material';
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
                        if (tabsAndProps) {
                            tabsAndProps.refresh(opts.data.savedprops.properties);
                        }
                        Csw.tryExec(onRefresh);
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
                Csw.publish('onAnyNodeButtonClickFinish', true);
                break;

            case Csw.enums.nbtButtonAction.reauthenticate:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                /* case 24669 */
                Csw.cookie.clearAll([Csw.cookie.cookieNames.LogoutPath]);
                Csw.ajax.deprecatedWsNbt({
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
                        Csw.publish('onAnyNodeButtonClickFinish', true);
                        Csw.publish(Csw.enums.events.main.refreshHeader);
                        break;
                    default:
                        Csw.dialogs.addnode({
                            nodetypeid: actionJson.requestItemNodeTypeId,
                            relatednodeid: actionJson.relatednodeid,
                            propertyData: actionJson.requestItemProps,
                            title: actionJson.titleText,
                            onSaveImmediate: function () {
                                Csw.publish('onAnyNodeButtonClickFinish', true);
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
                                nodeGrid.grid.reload(true);
                                Csw.tryExec(onRefresh);
                            },
                            onEditNode: function () {
                                nodeGrid.grid.reload();
                                Csw.tryExec(onRefresh);
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
                                        cswGrid.grid.reload(true);
                                    },
                                    onMultiEdit: function () {
                                        cswGrid.grid.toggleShowCheckboxes();
                                    },
                                    onEditView: function () {
                                        Csw.tryExec(menuDiv.$.dialog('close'));
                                    },
                                    onPrintView: function () {
                                        cswGrid.grid.print();
                                    },
                                    Multi: false
                                };
                                menuDiv.menu(menuOpts);
                            }
                        });
                    }
                });
                break;

            case Csw.enums.nbtButtonAction.managelocations.toLowerCase():
                actionJson.actionname = 'Manage Locations';
                launchAction = true;
                break;

            default:
                if (tabsAndProps) {
                    tabsAndProps.refresh(opts.data.savedprops.properties);
                }
                Csw.debug.warn('No event has been defined for button click ' + opts.data.action);
                break;
        }
        if (launchAction) {
            //1: Clear the center divs
            Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
            //2: load th
            Csw.publish(Csw.enums.events.main.handleAction, actionJson);
        }
    }


    Csw.composites.register('nodeButton', function (cswParent, options) {

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
                identityTabId: '',
                properties: {},
                onRefresh: function () { }
            };

            tabsAndProps = options.tabsAndProps;
            delete options.tabsAndProps;

            Csw.extend(cswPrivate, options, true);
            cswPrivate.div = cswParent.div();
            cswPrivate.div.empty();

            cswPrivate.table = cswPrivate.div.table();
        }());

        var onAnyNodeButtonClick = function () {
            cswPublic.button.disable();
            var onAnyNodeButtonClickFinish = function (eventObj, reEnable) {
                Csw.unsubscribe('onAnyNodeButtonClickFinish', onAnyNodeButtonClickFinish);
                if (reEnable) {
                    cswPublic.button.enable();
                }
            };
            Csw.subscribe('onAnyNodeButtonClickFinish', onAnyNodeButtonClickFinish);
        };
        Csw.subscribe('onAnyNodeButtonClick', onAnyNodeButtonClick);

        cswPrivate.onButtonClick = function () {
            Csw.publish('onAnyNodeButtonClick');
            Csw.unsubscribe('triggerSave', cswPrivate.onButtonClick);

            if (tabsAndProps && false === tabsAndProps.isFormValid()) {
                var warningDialog = Csw.layouts.dialog({
                    title: 'Warning',
                    width: 500,
                    height: 130,
                    onOpen: function () {
                        warningDialog.div.span({ text: 'This form contains some invalid values. Please correct them before proceeding.' });
                        warningDialog.div.br({ number: 3 });
                        warningDialog.div.buttonExt({
                            enabledText: 'Ok',
                            onClick: function() {
                                Csw.publish('onAnyNodeButtonClickFinish', true);
                                tabsAndProps.validator().focusInvalid();
                                warningDialog.close();
                            }
                        });
                    }
                });
                warningDialog.open();

            } else {
                if (Csw.isNullOrEmpty(cswPrivate.propId)) {
                    Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Cannot execute a property\'s button click event without a valid property.', 'Attempted to click a property button with a null or empty propid.'));
                    Csw.publish('onAnyNodeButtonClickFinish', true);
                } else {
                    // Case 27263: prompt to save instead

                    var propJson = '';
                    var editMode = Csw.enums.editMode.Table;
                    var nodeIds = '';
                    var propIds = '';
                    var tabIds = '';
                    if (tabsAndProps) {
                        propJson = Csw.serialize(tabsAndProps.getPropJson());
                        editMode = tabsAndProps.getEditMode();
                        nodeIds = tabsAndProps.getSelectedNodes();
                        propIds = tabsAndProps.getSelectedProps();
                        tabIds = tabsAndProps.getTabIds();
                    }

                    var performOnObjectClassButtonClick = function () {
                        Csw.unsubscribe('triggerSave', cswPrivate.onButtonClick);
                        Csw.ajax.deprecatedWsNbt({
                            urlMethod: 'onObjectClassButtonClick',
                            data: {
                                NodeTypePropAttr: cswPrivate.propId,
                                SelectedText: Csw.string(cswPublic.button.selectedOption, Csw.string(cswPrivate.value)),
                                TabIds: tabIds,  //cswPrivate.identityTabId + "," + cswPrivate.tabId,
                                Props: propJson,
                                NodeIds: nodeIds,
                                PropIds: propIds,
                                EditMode: editMode
                            },
                            success: function (data) {
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
                                    onObjectClassButtonClick(actionData, tabsAndProps, cswPrivate.onRefresh);
                                }
                            }, // ajax success()
                            error: function () {
                                Csw.publish('onAnyNodeButtonClickFinish', true);
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
                            onOk: function (selectedOption) {
                                performOnObjectClassButtonClick();
                            },
                            onCancel: function () {
                                Csw.publish('onAnyNodeButtonClickFinish', true);
                            },
                            onClose: function () {
                                Csw.publish('onAnyNodeButtonClickFinish', true);
                            }
                        });
                    } else {
                        performOnObjectClassButtonClick();
                    }
                } // if-else (Csw.isNullOrEmpty(propAttr)) {
            }
        }; // onButtonClick()
        Csw.subscribe('triggerSave', cswPrivate.onButtonClick);

        (function _post() {
            cswPrivate.btnCell = cswPrivate.table.cell(1, 1).div();

            var makeButton = function () {

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
                            selectedText: cswPrivate.displayName,
                            menuOptions: cswPrivate.menuOptions,
                            size: cswPrivate.size,
                            state: Csw.string(cswPrivate.state, cswPrivate.value),
                            propId: cswPrivate.propId,
                            onClick: function (selectedOption) {
                                cswPrivate.displayName = selectedOption;
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
            };

            if (cswPrivate.menuOptions && cswPrivate.menuOptions.length > 0) {
                cswPrivate.mode = 'menu';
                makeButton();
            } else {
                makeButton();
            }

        }());

        return cswPublic;
    });




})(jQuery);
