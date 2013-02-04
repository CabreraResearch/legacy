/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswDialog';

    var cswPrivateInit = (function () { //create this to prevent anyone from modifying the orginal position of the dialog positions
        var origX = 0; //150
        var origY = 0; //30

        return function () {
            return {
                origXAccessor: function () {
                    return origX;
                },
                origYAccessor: function () {
                    return origY;
                },
                windowWidth: function () {
                    return document.documentElement.clientWidth;
                },
                windowHeight: function () {
                    return document.documentElement.clientHeight;
                }
            };
        };
    } ());
    var cswPrivate = cswPrivateInit();
    cswPrivate.div = Csw.literals.div();

    var posX = cswPrivate.origXAccessor();
    var posY = cswPrivate.origYAccessor();
    var incrPosBy = 30;
    var dialogsCount = 0;

    var afterObjectClassButtonClick = function (action, dialog) {
        switch (Csw.string(action).toLowerCase()) {
            case Csw.enums.nbtButtonAction.dispense:
                Csw.tryExec(dialog.close);
                break;
            case Csw.enums.nbtButtonAction.reauthenticate:
                Csw.tryExec(dialog.close);
                break;
            case Csw.enums.nbtButtonAction.receive:
                Csw.tryExec(dialog.close);
                break;
            case Csw.enums.nbtButtonAction.refresh:
                Csw.tryExec(dialog.close);
                break;
            case Csw.enums.nbtButtonAction.popup:
                break;
            case Csw.enums.nbtButtonAction.request:
                break;
            case Csw.enums.nbtButtonAction.loadView:
                Csw.tryExec(dialog.close);
                break;
            case Csw.enums.nbtButtonAction.editprop:
                break;
        }
    };
    var methods = {

        //#region Specialized

        ExpireDialog: function (options) {
            var o = {
                onYes: null
            };

            if (options) {
                Csw.extend(o, options);
            }

            var div = Csw.literals.div();

            var tbl = div.table();

            tbl.cell(1, 1).span({ text: 'Your session is about to time out.' });
            tbl.cell(2, 1).span({ text: 'Would you like to continue working?' });

            var btnTbl = tbl.cell(2, 1).table();

            btnTbl.cell(3, 1).button({
                name: 'renew_btn',
                enabledText: 'Yes',
                bindOnEnter: div,
                onClick: function () {
                    div.$.dialog('close');
                    Csw.tryExec(o.onYes);
                }
            });

            openDialog(div, 300, 150, null, 'Expire Warning');
        }, // ExpireDialog
        AddLandingPageItemDialog: function (options) {
            var o = {
                form: function () { },
                onAdd: function () { }
            };
            if (options) Csw.extend(o, options);

            var div = Csw.literals.div();

            o.form(div, {
                onAdd: function () {
                    div.$.dialog('close');
                    Csw.tryExec(o.onAdd);
                }
            });

            openDialog(div, 400, 400, null, 'New Landing Page Item'); //should this be page-specific?
        }, // AddLandingPageItemDialog
        AddViewDialog: function (options) {
            var o = {
                name: 'addviewdialog',
                onAddView: function () { },
                viewid: '',
                viewmode: '',
                category: ''
            };
            if (options) Csw.extend(o, options);

            var div = Csw.literals.div();
            var table = div.table({
                name: 'tbl',
                FirstCellRightAlign: true
            });

            var row = 1;
            table.cell(row, 1).text('Name:');
            var nameTextCell = table.cell(row, 2);
            var nameTextBox = nameTextCell.input({
                name: 'nametb',
                type: Csw.enums.inputTypes.text,
                cssclass: 'textinput'
            });
            row += 1;

            table.cell(row, 1).text('Category:');
            var categoryTextCell = table.cell(row, 2);
            var categoryTextBox = categoryTextCell.input({
                name: 'cattb',
                type: Csw.enums.inputTypes.text,
                value: o.category,
                cssclass: 'textinput'
            });
            row += 1;

            var displayModeSelect;
            if (Csw.isNullOrEmpty(o.viewmode)) {
                table.cell(row, 1).text('Display Mode:');
                displayModeSelect = table.cell(row, 2).select({ name: o.name + '_dmsel' });
                displayModeSelect.option({ value: 'Grid' });
                displayModeSelect.option({ value: 'List' });
                displayModeSelect.option({ value: 'Table' });
                displayModeSelect.option({ value: 'Tree', isSelected: true });
                row += 1;
            }

            var visSelect = Csw.controls.makeViewVisibilitySelect(table, row, 'Available to:');
            row += 1;
            var saveBtn = div.button({
                name: o.name + '_submit',
                enabledText: 'Create View',
                disabledText: 'Creating View',
                onClick: function () {

                    var createData = {
                        Visibility: '',
                        VisibilityRoleId: '',
                        VisibilityUserId: ''
                    };
                    createData.ViewName = nameTextBox.val();
                    createData.Category = categoryTextBox.val();
                    createData.ViewId = o.viewid;
                    if (Csw.isNullOrEmpty(o.viewmode)) {
                        createData.ViewMode = displayModeSelect.val();
                    } else {
                        createData.ViewMode = o.viewmode;
                    }

                    var visValue = visSelect.getSelected();
                    createData.Visibility = visValue.visibility;
                    createData.VisibilityRoleId = visValue.roleid;
                    createData.VisibilityUserId = visValue.userid;

                    Csw.ajax.post({
                        urlMethod: 'createView',
                        data: createData,
                        success: function (data) {
                            div.$.dialog('close');
                            Csw.tryExec(o.onAddView, data.newviewid, createData.ViewMode);
                        },
                        error: saveBtn.enable
                    });
                }
            });

            /* Cancel Button */
            div.button({
                name: o.name + '_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    div.$.dialog('close');
                }
            });

            openDialog(div, 400, 200, null, 'New View');
        }, // AddViewDialog
        AddNodeDialog: function (options) {
            ///<summary>Creates an Add Node dialog and returns an object represent that dialog.</summary>
            var cswDlgPrivate = {
                text: '',
                nodeid: '',
                nodetypeid: '',
                relatednodeid: '',
                relatednodename: '',
                relatednodetypeid: '',
                relatedobjectclassid: '',
                onAddNode: function () { },
                onSaveImmediate: function () { },
                propertyData: null
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Add Dialog without options.', '', 'CswDialog.js', 177));
            }
            Csw.extend(cswDlgPrivate, options);
            cswDlgPrivate.name = cswDlgPrivate.text;
            var cswPublic = {
                isOpen: true,
                div: cswPrivate.div.div({ name: cswDlgPrivate.name }),
                close: function () {
                    if (cswPublic.isOpen) {
                        cswPublic.isOpen = false;
                        cswPublic.tabsAndProps.tearDown();
                    }
                },
                title: cswDlgPrivate.text
            };
            cswDlgPrivate.onOpen = function () {
                if (false === Csw.isNullOrEmpty(cswDlgPrivate.propertyData) && false === Csw.isNullOrEmpty(cswDlgPrivate.propertyData.nodeid)) {
                    cswDlgPrivate.nodeid = Csw.string(cswDlgPrivate.nodeid, cswDlgPrivate.propertyData.nodeid);
                }
                cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(cswPublic.div, {
                    name: 'tabsAndProps',
                    globalState: {
                        propertyData: cswDlgPrivate.propertyData,
                        ShowAsReport: false,
                        currentNodeId: cswDlgPrivate.nodeid
                    },
                    tabState: {
                        nodetypeid: cswDlgPrivate.nodetypeid,
                        relatednodeid: cswDlgPrivate.relatednodeid,
                        relatednodename: cswDlgPrivate.relatednodename,
                        relatednodetypeid: cswDlgPrivate.relatednodetypeid,
                        relatedobjectclassid: cswDlgPrivate.relatedobjectclassid,
                        EditMode: Csw.enums.editMode.Add
                    },
                    ReloadTabOnSave: false,
                    onSave: function (nodeid, nodekey, tabcount, nodename, nodelink) {
                        cswPublic.close();
                        cswPublic.div.$.dialog('close');
                        Csw.tryExec(cswDlgPrivate.onAddNode, nodeid, nodekey, nodename, nodelink);
                        Csw.tryExec(cswDlgPrivate.onSaveImmediate);
                    },
                    onInitFinish: function () {
                        //openDialog(cswPublic.div, 800, 600, null, cswPublic.title);
                    }
                });
            };
            openDialog(cswPublic.div, 800, 600, cswPublic.close, cswPublic.title, cswDlgPrivate.onOpen);
            return cswPublic;
        },
        AddFeedbackDialog: function (options) {
            ///<summary>Creates an Add Feedback dialog and returns an object represent that dialog.</summary>
            var cswDlgPrivate = {
                text: '',
                nodetypeid: '',
                onAddNode: function () { }
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Add Feedback without options.', '', 'CswDialog.js', 215));
            }
            Csw.extend(cswDlgPrivate, options);
            var cswPublic = {
                div: cswPrivate.div.div(),
                close: function () {
                    cswPublic.tabsAndProps.tearDown();
                },
                title: 'New ' + cswDlgPrivate.text
            };

            cswDlgPrivate.onOpen = function () {

                var state = Csw.clientState.getCurrent();
                Csw.ajax.post({
                    urlMethod: "getFeedbackNode",
                    data: {
                        nodetypeid: cswDlgPrivate.nodetypeid,
                        actionname: state.actionname,
                        viewid: state.viewid,
                        viewmode: state.viewmode,
                        selectednodeid: Csw.cookie.get('csw_currentnodeid'),
                        author: Csw.cookie.get('csw_username')
                    },
                    success: function (data) {
                        cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(cswPublic.div, {
                            globalState: {
                                ShowAsReport: false,
                                currentNodeId: data.nodeid,
                                propertyData: data.propdata
                            },
                            tabState: {
                                nodetypeid: cswDlgPrivate.nodetypeid,
                                EditMode: Csw.enums.editMode.Add,
                                relatednodeid: data.nodeid
                            },
                            ReloadTabOnSave: false,
                            onSave: function (nodeid, nodekey, tabcount, nodename) {
                                Csw.ajax.post({
                                    urlMethod: 'GetFeedbackCaseNumber',
                                    data: { nodeId: nodeid },
                                    success: function (result) {

                                        var closeDialog = function () { cswPublic.div.$.dialog('close'); };

                                        cswPublic.div.empty();
                                        //div.text('Your feedback has been submitted. Your case number is ' + result.casenumber + '.');
                                        cswPublic.div.nodeLink({
                                            text: 'Your feedback has been submitted. Your case number is ' + result.noderef + '.',
                                            onClick: closeDialog
                                        });

                                        cswPublic.div.br();
                                        cswPublic.div.button({
                                            name: '_feedbackOk',
                                            enabledText: 'OK',
                                            onClick: closeDialog
                                        });
                                        Csw.tryExec(cswDlgPrivate.onAddNode, nodeid, nodekey, nodename);
                                    }
                                });
                            },
                            onInitFinish: function () {

                            }
                        });
                    }
                });
            };
            openDialog(cswPublic.div, 800, 600, null, cswPublic.title, cswDlgPrivate.onOpen);
            return cswPublic;
        }, // AddFeedbackDialog
        AddNodeClientSideDialog: function (options) {
            var o = {
                name: '',
                nodetypename: '',
                title: '',
                onSuccess: null
            };

            if (options) {
                Csw.extend(o, options);
            }

            var div = Csw.literals.div(),
                newNode;

            div.append('New ' + o.nodetypename + ': ');
            newNode = div.input({ name: o.name + '_newNode', type: Csw.enums.inputTypes.text });

            div.button({
                name: o.objectClassId + '_add',
                enabledText: 'Add',
                onClick: function () {
                    Csw.tryExec(o.onSuccess, newNode.val());
                    div.$.dialog('close');
                }
            });
            openDialog(div, 300, 200, null, o.title);
        }, // AddNodeClientSideDialog
        AddNodeTypeDialog: function (options) {
            var o = {
                objectClassId: '',
                nodetypename: '',
                maxlength: 50, //DB nodetypename = varchar(50)
                category: '',
                select: '',
                nodeTypeDescriptor: '',
                onSuccess: null,
                title: ''
            };

            if (options) {
                Csw.extend(o, options);
            }

            var div = Csw.literals.div(),
                nodeTypeInp, categoryInp, addBtn,
                category = Csw.string(o.category);

            div.append('New ' + o.nodeTypeDescriptor + ': ');
            nodeTypeInp = div.input({ name: o.objectClassId + '_nodeType', type: Csw.enums.inputTypes.text, value: o.nodetypename, maxlength: o.maxlength });
            div.br();
            if (Csw.isNullOrEmpty(category)) {
                div.append('Category Name: ');
                categoryInp = div.input({ name: o.objectClassId + '_category', type: Csw.enums.inputTypes.text });
                div.br();
            }
            addBtn = div.button({
                name: o.objectClassId + '_add',
                enabledText: 'Add',
                onClick: function () {
                    var newNodeTypeName = nodeTypeInp.val();
                    Csw.ajax.post({
                        urlMethod: 'IsNodeTypeNameUnique',
                        async: false,
                        data: { 'NodeTypeName': newNodeTypeName },
                        success: function () {
                            o.select.option({ value: nodeTypeInp.val() }).propNonDom({ 'data-newNodeType': true });
                            o.select.val(nodeTypeInp.val());
                            if (Csw.isNullOrEmpty(category) && false === Csw.isNullOrEmpty(categoryInp)) {
                                category = categoryInp.val();
                            }
                            div.$.dialog('close');
                            Csw.tryExec(o.onSuccess, {
                                nodetypename: newNodeTypeName,
                                category: category
                            });
                        },
                        error: addBtn.enable
                    });
                }
            });
            openDialog(div, 400, 200, null, o.title);
        }, // AddNodeTypeDialog
        EditLayoutDialog: function (options) {
            var cswDlgPrivate = {
                name: 'editlayout',
                globalState: {
                    currentNodeId: '',
                    currentNodeKey: ''
                },
                tabState: {
                    tabid: '',
                    tabNo: 0,
                    EditMode: 'Edit'
                },
                Refresh: null
            };
            Csw.extend(cswDlgPrivate, options);

            var div = Csw.literals.div();

            cswDlgPrivate.onOpen = function () {
                cswDlgPrivate.ShowAsReport = false;
                cswDlgPrivate.tabState.Config = true;
                cswDlgPrivate.onTabSelect = function (tabid) {
                    if (cswDlgPrivate.tabState.tabid !== tabid) {
                        cswDlgPrivate.tabState.tabid = tabid;
                        _resetLayout();
                    }
                };
                cswDlgPrivate.onPropertyRemove = function () {
                    _configAddOptions();
                };

                var table = div.table({
                    name: 'EditLayoutDialog_table',
                    width: '100%'
                });

                /* Keep the add content in the same space */
                var table2 = table.cell(1, 1).table();
                var cell11 = table2.cell(1, 1);
                //cell11.append('Configure:');

                var cell12 = table.cell(1, 2);

                var layoutSelect = cell11.select({
                    name: 'EditLayoutDialog_layoutselect',
                    labelText: 'Configure: ',
                    selected: 'Edit',
                    values: ['Add', 'Edit', 'Preview', 'Table'],
                    onChange: function () {
                        cswDlgPrivate.tabState.EditMode = layoutSelect.val();
                        _resetLayout();
                    }
                });

                var cell21 = table2.cell(2, 1);

                function _resetLayout() {
                    cell12.empty();
                    Csw.layouts.tabsAndProps(cell12, cswDlgPrivate);
                    _configAddOptions();
                }

                function _configAddOptions() {
                    cell21.empty();
                    cell21.br({ number: 2 });

                    var addSelect = cell21.select({
                        name: 'EditLayoutDialog_addselect',
                        labelText: 'Add: ',
                        selected: '',
                        values: [],
                        onChange: function () {
                            Csw.ajax.post({
                                urlMethod: 'addPropertyToLayout',
                                data: {
                                    PropId: Csw.string(addSelect.val()),
                                    TabId: Csw.string(cswDlgPrivate.tabState.tabid),
                                    LayoutType: layoutSelect.val()
                                },
                                success: function () {
                                    _resetLayout();
                                }
                            }); // Csw.ajax
                        } // onChange
                    }); // 
                    var ajaxdata = {
                        NodeId: Csw.string(cswDlgPrivate.globalState.currentNodeId),
                        NodeKey: Csw.string(cswDlgPrivate.globalState.currentNodeKey),
                        NodeTypeId: Csw.string(cswDlgPrivate.globalState.nodetypeid),
                        TabId: Csw.string(cswDlgPrivate.tabState.tabid),
                        LayoutType: layoutSelect.val()
                    };
                    Csw.ajax.post({
                        urlMethod: 'getPropertiesForLayoutAdd',
                        data: ajaxdata,
                        success: function (data) {
                            var propOpts = [{ value: '', display: 'Select...'}];
                            Csw.each(data.add, function (p) {
                                var display = p.propname;
                                if (Csw.bool(p.hidden)) {
                                    display += ' (hidden)';
                                }
                                propOpts.push({
                                    value: p.propid,
                                    display: display
                                });
                            });
                            addSelect.setOptions(propOpts, '', true);
                        } // success
                    }); // Csw.ajax
                }

                // _configAddOptions()

                _resetLayout();
            };

            function _onclose() {
                Csw.publish('initPropertyTearDown');
                Csw.tryExec(cswDlgPrivate.Refresh);
            }

            openDialog(div, 900, 600, _onclose, 'Configure Layouts', cswDlgPrivate.onOpen);
        }, // EditLayoutDialog
        EditNodeDialog: function (options) {
            var cswDlgPrivate = {
                selectedNodeIds: Csw.delimitedString(),
                selectedNodeKeys: Csw.delimitedString(),
                currentNodeId: '',
                currentNodeKey: '',
                nodenames: [],
                Multi: false,
                ReadOnly: false,
                filterToPropId: '',
                title: 'Edit',
                onEditNode: null, // function (nodeid, nodekey) { },
                onEditView: null, // function (viewid) {}
                onRefresh: null,
                onClose: null,
                onAfterButtonClick: null,
                date: '', // viewing audit records
                editMode: Csw.enums.editMode.EditInPopup
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Add Dialog without options.', '', 'CswDialog.js', 177));
            }
            Csw.extend(cswDlgPrivate, options);
            var cswPublic = {
                div: Csw.literals.div(),
                close: function () {
                    cswPublic.tabsAndProps.tearDown();
                    Csw.tryExec(cswDlgPrivate.onClose);
                }
            };

            var title = Csw.string(cswDlgPrivate.title);
            if (cswDlgPrivate.nodenames.length > 1) {
                title += ': ' + cswDlgPrivate.nodenames.join(', ');
            }
            cswPublic.title = title;

            cswDlgPrivate.onOpen = function () {
                var table = cswPublic.div.table({ width: '100%' });
                var tabCell = table.cell(1, 2);

                setupTabs(cswDlgPrivate.date);

                function setupTabs(date) {
                    tabCell.empty();

                    cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(tabCell, {
                        Multi: cswDlgPrivate.Multi,
                        globalState: {
                            date: date,
                            selectedNodeIds: cswDlgPrivate.selectedNodeIds,
                            selectedNodeKeys: cswDlgPrivate.selectedNodeKeys,
                            currentNodeId: cswDlgPrivate.currentNodeId,
                            currentNodeKey: cswDlgPrivate.currentNodeKey,
                            nodenames: cswDlgPrivate.nodenames,
                            filterToPropId: cswDlgPrivate.filterToPropId
                        },
                        tabState: {
                            ReadOnly: cswDlgPrivate.ReadOnly,
                            EditMode: cswDlgPrivate.editMode,
                            tabid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentTabId)
                        },

                        ReloadTabOnSave: true,
                        Refresh: cswDlgPrivate.onRefresh,
                        onEditView: function (viewid) {
                            cswPublic.close();
                            Csw.tryExec(cswDlgPrivate.onEditView, viewid);
                        },
                        onSave: function (nodeids, nodekeys, tabcount) {
                            Csw.clientChanges.unsetChanged();
                            if (tabcount <= 2 || cswDlgPrivate.Multi) { /* Ignore history tab */
                                cswPublic.close();
                                cswPublic.div.$.dialog('close');
                            }
                            Csw.tryExec(cswDlgPrivate.onEditNode, nodeids, nodekeys, cswPublic.close);
                        },
                        onBeforeTabSelect: function () {
                            return Csw.clientChanges.manuallyCheckChanges();
                        },
                        onTabSelect: function (tabid) {
                            Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, tabid);
                        },
                        onPropertyChange: function () {
                            Csw.clientChanges.setChanged();
                        },
                        onAfterButtonClick: cswDlgPrivate.onAfterButtonClick
                    });
                }

                // _setupTabs()
            };
            openDialog(cswPublic.div, 900, 600, cswPublic.close, title, cswDlgPrivate.onOpen);
            return cswPublic;
        }, // EditNodeDialog
        CopyNodeDialog: function (options) {
            var cswDlgPrivate = {
                'nodename': '',
                'nodeid': '',
                'nodetypeid': '',
                'nodekey': '',
                'onCopyNode': function () { }
            };

            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Copy Dialog without options.', '', 'CswDialog.js', 177));
            }
            Csw.extend(cswDlgPrivate, options);
            var cswPublic = {
                div: Csw.literals.div({
                    name: 'CopyNodeDialogDiv'
                }),
                close: function () {
                    cswPublic.div.$.dialog('close');
                }
            };

            // Prevent copy if quota is reached
            var tbl = cswPublic.div.table({
                name: 'CopyNodeDialogDiv_table'
            });
            var cell11 = tbl.cell(1, 1).propDom('colspan', '2');
            var cell21 = tbl.cell(2, 1);
            var cell22 = tbl.cell(2, 2);

            Csw.ajax.post({
                urlMethod: 'checkQuota',
                data: {
                    NodeTypeId: Csw.string(cswDlgPrivate.nodetypeid),
                    NodeKey: Csw.string(cswDlgPrivate.nodekey)
                },
                success: function (data) {
                    if (Csw.bool(data.result)) {

                        cell11.append('Copying: ' + cswDlgPrivate.nodename);
                        cell11.br({ number: 2 });

                        var copyBtn = cell21.button({
                            name: 'copynode_submit',
                            enabledText: 'Copy',
                            disabledText: 'Copying',
                            onClick: function () {
                                Csw.copyNode({
                                    'nodeid': cswDlgPrivate.nodeid,
                                    'nodekey': Csw.string(cswDlgPrivate.nodekey, cswDlgPrivate.nodekey[0]),
                                    'onSuccess': function (nodeid, nodekey) {
                                        cswPublic.close();
                                        cswDlgPrivate.onCopyNode(nodeid, nodekey);
                                    },
                                    'onError': function () {
                                        copyBtn.enable();
                                    }
                                });
                            }
                        });

                    } else {
                        cell11.append('You have used all of your purchased quota, and must purchase additional quota space in order to add more.');
                    } // if-else (Csw.bool(data.result)) {
                } // success()
            }); // ajax

            /* Cancel Button */
            cell22.button({
                name: 'copynode_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    cswPublic.close();
                }
            });
            openDialog(cswPublic.div, 400, 300, null, 'Confirm Copy');
            return cswPublic;
        }, // CopyNodeDialog       
        DeleteNodeDialog: function (options) {
            var cswDlgPrivate = {
                nodes: {},
                nodenames: [],
                nodeids: [],
                cswnbtnodekeys: [],
                onDeleteNode: null, //function (nodeid, nodekey) { },
                Multi: false,
                nodeTreeCheck: null,
                publishDeleteEvent: true
            };

            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Delete Dialog without options.', '', 'CswDialog.js', 641));
            }
            Csw.extend(cswDlgPrivate, options);
            var cswPublic = {
                div: Csw.literals.div(),
                close: function () {
                    cswPublic.div.$.dialog('close');
                }
            };

            cswPublic.div.span({ text: 'Are you sure you want to delete the following?' }).br();
            var n = 0;
            Csw.each(cswDlgPrivate.nodes, function (nodeObj) {
                cswDlgPrivate.nodeids[n] = nodeObj.nodeid;
                cswDlgPrivate.cswnbtnodekeys[n] = nodeObj.nodekey;
                cswPublic.div.span({ text: nodeObj.nodename }).css({ 'padding-left': '10px' }).br();
                n += 1;
            });

            cswPublic.div.br({ number: 2 });

            var deleteBtn = cswPublic.div.button({
                name: 'deletenode_submit',
                enabledText: 'Delete',
                disabledText: 'Deleting',
                onClick: function () {
                    Csw.deleteNodes({
                        nodeids: cswDlgPrivate.nodeids,
                        nodekeys: cswDlgPrivate.cswnbtnodekeys,
                        onSuccess: function (nodeid, nodekey) {
                            cswPublic.close();
                            Csw.tryExec(cswDlgPrivate.onDeleteNode, nodeid, nodekey);
                            if (Csw.bool(cswDlgPrivate.publishDeleteEvent)) {
                                Csw.publish(Csw.enums.events.CswNodeDelete, { nodeids: cswDlgPrivate.nodeids, cswnbtnodekeys: cswDlgPrivate.cswnbtnodekeys });
                            }
                        },
                        onError: deleteBtn.enable
                    });
                }
            });
            /* Cancel Button */
            cswPublic.div.button({
                name: 'deletenode_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    cswPublic.close();
                }
            });
            openDialog(cswPublic.div, 400, 200, null, 'Confirm Delete');
            return cswPublic;
        }, // DeleteNodeDialog
        AboutDialog: function () {

            var div = Csw.literals.div();
            Csw.ajax.post({
                urlMethod: 'getAbout',
                data: {},
                success: function (data) {
                    div.append('NBT Assembly Version: ' + data.assembly + '<br/><br/>');
                    var table = div.table({
                        name: 'abouttale'
                    });

                    var row = 1;
                    var components = data.components;
                    for (var comp in components) {
                        if (Csw.contains(components, comp)) {
                            var thisComp = components[comp];

                            table.cell(row, 1).css({ padding: '2px 5px 2px 5px' }).append(thisComp.name);
                            table.cell(row, 2).css({ padding: '2px 5px 2px 5px' }).append(thisComp.version);
                            table.cell(row, 3).css({ padding: '2px 5px 2px 5px' }).append(thisComp.copyright);
                            row += 1;
                        }
                    }
                    table.cell(row, 1).css({ padding: '15px 1px 1px 1px' }).append('');
                    row += 1;
                    table.cell(row, 1).css({ padding: '2px 5px 2px 5px' }).append('Session Info');
                    row += 1;
                    table.cell(row, 1).css({ padding: '2px 5px 2px 5px' }).append('---------------------------');
                    row += 1;
                    for (var userComp in data.userProps) {
                        var thisProp = data.userProps[userComp];
                        table.cell(row, 1).css({ padding: '2px 5px 2px 5px' }).append(thisProp.componentName);
                        table.cell(row, 2).css({ padding: '2px 5px 2px 5px' }).append(thisProp.value);
                        row += 1;
                    }
                }
            });
            openDialog(div, 600, 400, null, 'About');
        }, // AboutDialog
        FileUploadDialog: function (options) {
            var o = {
                urlMethod: 'fileForProp',
                params: {},
                onSuccess: function () { }
            };
            if (options) {
                Csw.extend(o, options);
            }

            var div = Csw.literals.div();

            div.fileUpload({
                uploadUrl: o.urlMethod,
                params: o.params,
                onSuccess: function (data) {
                    div.$.dialog('close');
                    Csw.tryExec(o.onSuccess, data);
                }
            });

            div.button({
                name: 'fileupload_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    div.$.dialog('close');
                }
            });

            openDialog(div, 400, 300, null, 'Upload');
        }, // FileUploadDialog
        //        ImportC3RecordDialog: function (options) {
        //            var cswDlgPrivate = {
        //                nodes: {},
        //                nodenames: [],
        //                nodeids: [],
        //                cswnbtnodekeys: [],
        //                onDeleteNode: null, //function (nodeid, nodekey) { },
        //                Multi: false,
        //                nodeTreeCheck: null,
        //                publishDeleteEvent: true
        //            };

        //            if (Csw.isNullOrEmpty(options)) {
        //                Csw.error.throwException(Csw.error.exception('Cannot create an Delete Dialog without options.', '', 'CswDialog.js', 641));
        //            }
        //            Csw.extend(cswDlgPrivate, options);
        //            var cswPublic = {
        //                div: Csw.literals.div(),
        //                close: function () {
        //                    cswPublic.div.$.dialog('close');
        //                }
        //            };

        //            cswPublic.div.span({ text: 'To do: Dummy dialog for the time being.' }).br();

        //            openDialog(cswPublic.div, 400, 200, null, 'Import Record');

        //        }, // ImportC3RecordDialog
        C3DetailsDialog: function (options) {

            var cswPrivate = {
                title: "Product Details",
                node: options.nodeObj,
                searchresults: null
            };

            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Delete Dialog without options.', '', 'CswDialog.js', 641));
            }
            Csw.extend(cswPrivate, options);
            var cswPublic = {
                div: Csw.literals.div(),
                close: function () {
                    cswPublic.div.$.dialog('close');
                }
            };

            //is this necessary or can i use the declaration above?
            var div = Csw.literals.div(),
                newNode;

            var getProductDetails = function () {

                var CswC3SearchParams = {
                    Field: 'ProductId',
                    Query: cswPrivate.node.c3productid,
                    SearchOperator: '',
                    SourceName: '',
                    MaxRows: 10
                };

                Csw.ajaxWcf.post({
                    urlMethod: 'ChemCatCentral/GetProductDetails',
                    data: CswC3SearchParams,
                    success: function (data) {

                        //Create the table
                        var table1 = div.table({ cellspacing: '5px', align: 'left', width: '100%' });

                        table1.cell(1, 1).div({
                            text: cswPrivate.node.nodename
                        }).css({ 'font-size': '18px', 'font-weight': 'bold' });

                        table1.cell(2, 1).div({
                            text: 'Supplier: ' + data.ProductDetails.SupplierName
                        });

                        table1.cell(3, 1).div({
                            text: 'Catalog#: ' + data.ProductDetails.CatalogNo
                        });

                        var cell4_hidden = 'hidden';
                        var producturl = data.ProductDetails.ProductUrl;
                        if (null != producturl) {
                            cell4_hidden = 'visible';
                        }
                        table1.cell(4, 1).div({
                            text: '<a href=' + producturl + ' target="_blank">Product Website</a>',
                            styles: { 'visibility': cell4_hidden }
                        });

                        var cell5_hidden = 'hidden';
                        var msdsurl = data.ProductDetails.MsdsUrl;
                        if (null != msdsurl) {
                            cell5_hidden = 'visible';
                        }
                        table1.cell(5, 1).div({
                            text: '<a href=' + msdsurl + ' target="_blank">MSDS</a>',
                            styles: { 'visibility': cell5_hidden }
                        });

                        table1.cell(6, 1).grid({
                            name: 'c3detailsgrid_size',
                            title: 'Sizes',
                            height: 100,
                            width: 300,
                            fields: [{ name: 'pkg_qty', type: 'string' }, { name: 'pkg_qty_uom', type: 'string' }, { name: 'case_qty', type: 'string'}],
                            columns: [{ header: 'Package Quantity', dataIndex: 'pkg_qty' }, { header: 'UOM', dataIndex: 'pkg_qty_uom' }, { header: 'Case Quantity', dataIndex: 'case_qty'}],
                            data: {
                                items: data.ProductDetails.ProductSize,
                                buttons: []
                            },
                            usePaging: false,
                            showActionColumn: false
                        });

                        table1.cell(7, 1).grid({
                            name: 'c3detailsgrid_extradata',
                            title: 'Extra Attributes',
                            height: 150,
                            width: 300,
                            fields: [{ name: 'attribute', type: 'string' }, { name: 'value', type: 'string'}],
                            columns: [{ header: 'Attribute', dataIndex: 'attribute' }, { header: 'Value', dataIndex: 'value'}],
                            data: {
                                items: data.ProductDetails.TemplateSelectedExtensionData,
                                buttons: []
                            },
                            usePaging: false,
                            showActionColumn: false
                        });

                    }
                });
            };

            var onOpen = function () {
                getProductDetails();
            };

            openDialog(div, 500, 500, null, cswPrivate.title, onOpen);

        }, // C3DetailsDialog
        C3SearchDialog: function (options) {

            var cswPrivate = {
                title: "ChemCatCentral Search",
                c3searchterm: options.c3searchterm,
                c3handleresults: options.c3handleresults,
                clearview: options.clearview,
                loadView: null //function () { }
            };

            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var div = Csw.literals.div(),
                newNode;

            var tableOuter = div.table({ cellpadding: '2px', align: 'left', width: '700px' });

            tableOuter.cell(1, 1).p({ text: '' });

            var tableInner = div.table({ cellpadding: '2px' });

            //DataSources Picklist
            var sourceSelect = tableInner.cell(1, 1).select({
                name: 'C3Search_sourceSelect',
                selected: 'All Sources'
            });

            function getAvailableDataSources() {

                Csw.ajaxWcf.post({
                    async: false,
                    urlMethod: 'ChemCatCentral/GetAvailableDataSources',
                    success: function (data) {
                        sourceSelect.setOptions(sourceSelect.makeOptions(data.AvailableDataSources));
                    }
                });
            };

            getAvailableDataSources(); //call function

            //SearchTypes Picklist
            var searchTypeSelect = tableInner.cell(1, 2).select({
                name: 'C3Search_searchTypeSelect'
            });

            function getSearchTypes() {

                Csw.ajaxWcf.post({
                    async: false,
                    urlMethod: 'ChemCatCentral/GetSearchTypes',
                    success: function (data) {
                        searchTypeSelect.setOptions(searchTypeSelect.makeOptions(data.SearchTypes));
                    }
                });
            }

            getSearchTypes(); //call function

            var searchOperatorSelect = tableInner.cell(1, 3).select({
                name: 'C3Search_searchOperatorSelect'
            });
            searchOperatorSelect.option({ display: 'Begins', value: 'begins' });
            searchOperatorSelect.option({ display: 'Contains', value: 'contains' });
            searchOperatorSelect.option({ display: 'Exact', value: 'exact' });

            var searchTermField = tableInner.cell(1, 4).input({
                value: cswPrivate.c3searchterm
            });

            tableInner.cell(1, 5).button({
                name: 'c3SearchBtn',
                enabledText: 'Search',
                disabledText: "Searching...",
                onClick: function () {

                    var CswC3SearchParams = {
                        Field: searchTypeSelect.selectedVal(),
                        Query: searchTermField.val(),
                        SearchOperator: searchOperatorSelect.selectedVal(),
                        SourceName: sourceSelect.selectedVal()
                    };

                    Csw.ajaxWcf.post({
                        urlMethod: 'ChemCatCentral/Search',
                        data: CswC3SearchParams,
                        success: function (data) {
                            //Convert to object from string
                            var obj = JSON.parse(data.SearchResults);
                            Csw.tryExec(cswPrivate.clearview);
                            Csw.tryExec(cswPrivate.c3handleresults(obj));
                            div.$.dialog('close');
                        }
                    });
                }
            });


            tableOuter.cell(2, 1).div(tableInner);

            openDialog(div, 750, 300, null, cswPrivate.title);
        }, // C3SearchDialog
        StructureSearchDialog: function (options) {
            var cswPrivate = {
                title: "Structure Search",
                loadView: null //function () { }
            };

            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var div = Csw.literals.div(),
                newNode;

            var table = div.table({ cellpadding: '2px', align: 'left' });

            table.cell(3, 1).span({ text: 'MOL Text (Paste from Clipboard):' });
            var molText = table.cell(4, 1).textArea({
                name: '',
                rows: 12,
                cols: 50,
                onChange: function () {
                    getMolImgFromText(molText.val(), '');
                }
            });

            var getMolImgFromText = function (molTxt, nodeId) {
                var ret = '';

                Csw.ajaxWcf.post({
                    async: false,
                    urlMethod: 'Mol/getImg',
                    data: {
                        molString: molTxt,
                        nodeId: nodeId,
                        molImgAsBase64String: ''
                    },
                    success: function (data) {
                        table.cell(4, 2).empty();
                        if (data.molImgAsBase64String) {
                            molText.val(data.molString);
                            table.cell(4, 2).img({
                                labelText: "Query Image",
                                src: "data:image/jpeg;base64," + data.molImgAsBase64String
                            });
                        }
                    }
                });
                return ret;
            };

            var currentNodeId = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
            getMolImgFromText('', currentNodeId);

            var uploadBtn = table.cell(2, 1).input({
                name: 'fileupload',
                labelText: "Mol File",
                type: Csw.enums.inputTypes.file
            }).css('float', 'left');
            uploadBtn.$.fileupload({
                datatype: 'json',
                url: 'Services/Mol/getImgFromFile',
                paramName: 'filename',
                done: function (e, data) {
                    molText.val(data.result.Data.molString);
                    table.cell(4, 2).empty();
                    table.cell(4, 2).img({
                        labelText: "Query Image",
                        src: "data:image/jpeg;base64," + data.result.Data.molImgAsBase64String
                    });
                }
            });

            var exactSearchChkBox = table.cell(5, 1).input({
                labelText: "Exact Search",
                useWide: true,
                type: Csw.enums.inputTypes.checkbox
            });

            table.cell(6, 1).button({
                name: 'ssSearchBtn',
                enabledText: 'Search',
                disabledText: "Searching...",
                onClick: function () {
                    Csw.ajaxWcf.post({
                        urlMethod: 'Mol/runStructureSearch',
                        data: {
                            viewId: '',
                            viewMode: '',
                            molString: molText.val(),
                            exact: exactSearchChkBox.checked()
                        },
                        success: function (data) {
                            var viewId = data.viewId;
                            var viewMode = data.viewMode;
                            Csw.tryExec(cswPrivate.loadView, viewId, viewMode);
                            div.$.dialog('close');
                        }
                    });
                }
            });

            table.cell(9, 1).span({
                text: "please note that larger molecules can extend the search time"
            });

            openDialog(div, 750, 500, null, cswPrivate.title);
        },
        EditMolDialog: function (options) {
            var o = {
                TextUrl: '',
                FileUrl: '',
                PropId: '',
                molData: '',
                onSuccess: function () { }
            };
            if (options) {
                Csw.extend(o, options);
            }
            var div = Csw.literals.div(),
                molTxtArea, saveBtn;

            div.label({
                text: 'Upload a MOL File:',
                cssclass: 'changeMolDataDialogLabel'
            });

            div.br({ number: 2 });

            var uploadBtn = div.input({
                name: 'fileupload',
                type: Csw.enums.inputTypes.file
            });
            uploadBtn.$.fileupload({
                datatype: 'json',
                url: 'Services/Mol/saveMolPropFile?' + $.param({ PropId: o.PropId }),
                paramName: 'fileupload',
                done: function (e, data) {
                    div.$.dialog('close');
                    o.onSuccess(data.result.Data);
                }
            });

            div.br({ number: 2 });

            div.span({ text: 'MOL Text (Paste from Clipboard):' }).br();

            molTxtArea = div.textArea({
                name: '',
                rows: 6,
                cols: 40
            });
            molTxtArea.text(o.molData);
            div.br();

            var buttonsDiv = div.div({ align: 'right' });

            saveBtn = buttonsDiv.button({
                name: 'txt_save',
                enabledText: 'Save',
                disabledText: 'Saving...',
                onClick: function () {
                    Csw.ajaxWcf.post({
                        urlMethod: 'Mol/saveMolPropText',
                        data: {
                            molString: molTxtArea.val(),
                            propId: o.PropId
                        },
                        success: function (data) {
                            div.$.dialog('close');
                            Csw.tryExec(o.onSuccess, data);
                        },
                        error: saveBtn.enable
                    }); // ajax
                } // onClick
            }); // 

            buttonsDiv.button({
                name: 'fileupload_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    div.$.dialog('close');
                }
            });

            openDialog(div, 400, 300, null, 'Change MOL Data');
        }, // FileUploadDialog
        ShowLicenseDialog: function (options) {
            var o = {
                GetLicenseUrl: 'getLicense',
                AcceptLicenseUrl: 'acceptLicense',
                onAccept: function () { },
                onDecline: function () { }
            };
            if (options) {
                Csw.extend(o, options);
            }

            var div = Csw.literals.div({ align: 'center' });
            div.append('Service Level Agreement').br();
            var licenseTextArea = div.textArea({ name: 'license', rows: 30, cols: 80 }).propDom({ disabled: true });
            div.br();

            Csw.ajax.post({
                urlMethod: o.GetLicenseUrl,
                success: function (data) {
                    licenseTextArea.text(data.license);
                }
            });

            var acceptBtn = div.button({
                name: 'license_accept',
                enabledText: 'I Accept',
                disabledText: 'Accepting...',
                onClick: function () {
                    Csw.ajax.post({
                        urlMethod: o.AcceptLicenseUrl,
                        success: function () {
                            div.$.dialog('close');
                            Csw.tryExec(o.onAccept);
                        },
                        error: acceptBtn.enable
                    }); // ajax
                } // onClick
            }); // 

            div.button({
                name: 'license_decline',
                enabledText: 'I Decline',
                disabledText: 'Declining...',
                onClick: function () {
                    div.$.dialog('close');
                    Csw.tryExec(o.onDecline);
                }
            });

            openDialog(div, 800, 600, null, 'Terms and Conditions');
        }, // ShowLicenseDialog
        PrintLabelDialog: function (options) {
            ///<summary>Creates an Print Label dialog and returns an object represent that dialog.</summary>
            var cswDlgPrivate = {
                name: 'print_label',
                nodes: {},
                nodeids: [],
                nodetypeid: ''
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Print Label Dialog without options.', '', 'CswDialog.js', 893));
            }
            Csw.extend(cswDlgPrivate, options);


            var cswPublic = {
                div: Csw.literals.div({ text: 'Print labels for the following: ' }),
                close: function () {
                    cswPublic.div.$.dialog('close');
                }
            };

            cswPublic.div.br();
            Csw.each(cswDlgPrivate.nodes, function (nodeObj, nodeId) {
                cswDlgPrivate.nodeids.push(nodeId);
                cswPublic.div.span({ text: nodeObj.nodename }).css({ 'padding-left': '10px' }).br();
            });

            var handlePrint = function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Labels/newPrintJob',
                    data: {
                        LabelId: labelSel.val(),
                        PrinterId: printerSel.selectedNodeId(),
                        TargetIds: cswDlgPrivate.nodeids.join(',')
                    },
                    success: function (data) {
                        cswPublic.div.empty();
                        cswPublic.div.nodeLink({ text: 'Label(s) will be printed in Job: ' + data.JobLink });
                    } // success
                }); // ajax
            }; // handlePrint()

            cswPublic.div.br();
            cswPublic.div.div({ text: 'Select a label to Print:' });
            var labelSelDiv = cswPublic.div.div();
            var labelSel = labelSelDiv.select({
                name: cswDlgPrivate.name + '_labelsel'
            });

            Csw.ajaxWcf.post({
                urlMethod: 'Labels/list',
                data: {
                    TargetTypeId: cswDlgPrivate.nodetypeid,
                    TargetId: cswDlgPrivate.nodeids[0]
                },
                success: function (data) {
                    if (data.Labels && data.Labels.length > 0) {
                        for (var i = 0; i < data.Labels.length; i += 1) {
                            var label = data.Labels[i];
                            var isSelected = Csw.bool(label.Id === data.SelectedLabelId);
                            labelSel.option({ value: label.Id, display: label.Name, isSelected: isSelected });
                        }
                    } else {
                        labelSelDiv.span({ text: 'No labels have been assigned!' });
                    }
                } // success
            }); // ajax

            labelSelDiv.br();
            labelSelDiv.br();
            labelSelDiv.div({ text: 'Select a Printer:' });

            var userDefaults = JSON.parse(Csw.cookie.get(Csw.cookie.cookieNames.UserDefaults));

            var printerSel = labelSelDiv.nodeSelect({
                name: cswDlgPrivate.name + '_printersel',
                objectClassName: 'PrinterClass',
                allowAdd: false,
                isRequired: true,
                showSelectOnLoad: true,
                isMulti: false,
                selectedNodeId: userDefaults.DefaultPrinterId
            });

            cswPublic.div.button({
                name: 'print_label_close',
                enabledText: 'Close',
                disabledText: 'Closing...',
                onClick: function () {
                    cswPublic.close();
                }
            });

            cswPublic.div.button({
                name: 'print_label_print',
                enabledText: 'Print',
                //disabledText: 'Printing...', 
                disableOnClick: false,
                onClick: handlePrint //getEplContext
            });
            //printBtn.hide();

            openDialog(cswPublic.div, 400, 300, null, 'Print');
            return cswPublic;
        }, // PrintLabelDialog

        ImpersonateDialog: function (options) {
            var o = {
                onImpersonate: null
            };
            if (options) Csw.extend(o, options);

            var div = Csw.literals.div();

            Csw.ajax.post({
                urlMethod: 'getUsers',
                success: function (data) {
                    if (Csw.bool(data.result)) {
                        var usersel = div.select({
                            name: 'ImpersonateSelect',
                            selected: ''
                        });

                        Csw.each(data.users, function (thisUser) {
                            usersel.addOption({ value: thisUser.userid, display: thisUser.username }, false);
                        });

                        div.button({
                            name: 'ImpersonateButton',
                            enabledText: 'Impersonate',
                            onClick: function () {
                                Csw.tryExec(o.onImpersonate, usersel.val(), usersel.selectedText());
                                div.$.dialog('close');
                            }
                        });

                        div.button({
                            name: 'CancelButton',
                            enabledText: 'Cancel',
                            onClick: function () {
                                div.$.dialog('close');
                            }
                        });
                    } // if(Csw.bool(data.result))
                } // success
            }); // ajax

            openDialog(div, 400, 300, null, 'Impersonate');
        }, // ImpersonateDialog

        SearchDialog: function (options) {
            var cswDlgPrivate = {
                name: 'searchdialog',
                propname: '',
                title: '',
                nodetypeid: '',
                objectclassid: '',
                onSelectNode: null
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Search Dialog without options.', '', 'CswDialog.js', 1013));
            }
            Csw.extend(cswDlgPrivate, options);
            var cswPublic = {
                div: Csw.literals.div({ name: 'searchdialog_div' }),
                close: function () {
                    cswPublic.div.$.dialog('close');
                },
                title: Csw.string(cswDlgPrivate.title, 'Search ' + cswDlgPrivate.propname)
            };

            openDialog(cswPublic.div, 800, 600, null, 'Search ' + cswDlgPrivate.propname);

            cswPublic.search = Csw.composites.universalSearch(cswPublic.div, {
                name: cswDlgPrivate.name,
                nodetypeid: cswDlgPrivate.nodetypeid,
                objectclassid: cswDlgPrivate.objectclassid,
                onBeforeSearch: function () { },
                onAfterSearch: function () { },
                onAfterNewSearch: function (searchid) { },
                onAddView: function (viewid, viewmode) { },
                onLoadView: function (viewid, viewmode) { },
                showSave: false,
                allowEdit: false,
                allowDelete: false,
                compactResults: true,
                extraAction: 'Select',
                extraActionIcon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.check),
                onExtraAction: function (nodeObj) {
                    cswPublic.close();
                    Csw.tryExec(cswDlgPrivate.onSelectNode, nodeObj);
                }
            });
            return cswPublic;
        }, // SearchDialog

        SaveSearchDialog: function (options) {
            var o = {
                div: Csw.literals.div(),
                title: 'Save Search',
                onOk: null,
                onClose: null,
                height: 400,
                width: 600,
                name: '',
                category: ''
            };
            Csw.extend(o, options);

            var nameInput = o.div.input({
                labelText: 'Name:&nbsp;',
                value: o.name
            });
            o.div.br();

            var categoryInput = o.div.input({
                labelText: 'Category:&nbsp;',
                value: o.category
            });
            o.div.br();

            o.div.button({
                enabledText: 'Save',
                onClick: function () {
                    Csw.tryExec(o.onOk, nameInput.val(), categoryInput.val());
                    o.div.$.dialog('close');
                }
            });

            o.div.button({
                enabledText: 'Cancel',
                onClick: function () {
                    o.div.$.dialog('close');
                }
            });

            openDialog(o.div, o.width, o.height, o.onClose, o.title);
        }, // SearchDialog
        GenericDialog: function (options) {
            var o = {
                div: Csw.literals.div(),
                title: '',
                onOk: null,
                onCancel: null,
                onClose: null,
                height: 400,
                width: 600,
                okText: 'Ok',
                cancelText: 'Cancel'
            };
            if (options) Csw.extend(o, options);

            o.div.br({ number: 2 });

            o.div.button({
                enabledText: o.okText,
                onClick: function () {
                    Csw.tryExec(o.onOk);
                    o.div.$.dialog('close');
                }
            });

            o.div.button({
                enabledText: o.cancelText,
                onClick: function () {
                    Csw.tryExec(o.onCancel);
                    o.div.$.dialog('close');
                }
            });

            openDialog(o.div, o.width, o.height, o.onClose, o.title);
        }, // GenericDialog

        BatchOpDialog: function (options) {
            var o = {
                opname: 'operation',
                onClose: null,
                onViewBatchOperation: null
            };
            if (options) Csw.extend(o, options);

            var div = Csw.literals.div({ name: 'searchdialog_div' });

            div.append('This ' + o.opname + ' will be performed as a batch operation');

            div.button({
                enabledText: 'Close',
                onClick: function () {
                    div.$.dialog('close');
                }
            });

            div.button({
                enabledText: 'View Batch Operation',
                onClick: function () {
                    Csw.tryExec(o.onViewBatchOperation);
                    div.$.dialog('close');
                }
            });

            openDialog(div, 400, 300, o.onClose, 'Batch Operation');
        }, // BatchOpDialog

        ErrorDialog: function (error) {
            var div = Csw.literals.div();
            openDialog(div, 400, 300, null, 'Error');
            div.$.CswErrorMessage(error);
        },

        AlertDialog: function (message, title, onClose, height, width, onOpen) {

            var div = Csw.literals.div({
                name: Csw.string(title, 'an alert dialog').replace(' ', '_'),
                text: message,
                align: 'center'
            });

            div.br();

            var divBody = div.div();

            div.button({
                enabledText: 'OK',
                onClick: function () {
                    div.$.dialog('close');
                    Csw.tryExec(onClose);
                }
            });

            Csw.tryExec(onOpen, divBody);

            openDialog(div, Csw.number(width, 400), Csw.number(height, 200), null, title);
        },
        ConfirmDialog: function (message, title, okFunc, cancelFunc) {

            var div = Csw.literals.div({
                name: Csw.string(title, 'an alert dialog').replace(' ', '_'),
                text: message,
                align: 'center'
            });

            div.br();

            div.button({
                enabledText: 'OK',
                onClick: function () {
                    Csw.tryExec(okFunc);
                    div.$.dialog('close');
                }
            });

            div.button({
                enabledText: 'Cancel',
                onClick: function () {
                    Csw.tryExec(cancelFunc);
                    div.$.dialog('close');
                }
            });

            openDialog(div, 400, 150, null, title);
        },
        NavigationSelectDialog: function (options) {
            var o = {
                name: '',
                title: 'Select from the following options',
                navigationText: 'Click OK to continue',
                buttons: Csw.enums.dialogButtons["1"],
                values: [],
                onOkClick: null,
                onCancelClick: null
            };

            if (options) {
                Csw.extend(o, options);
            }

            var div = Csw.literals.div({
                name: o.name
            });
            div.p({ text: o.navigationText });
            var select = div.select({
                name: 'CswNavigationSelectDialog',
                values: o.values
            });

            div.button({
                name: 'CswNavigationSelectDialog_OK',
                enabledText: 'OK',
                onClick: function () {
                    Csw.tryExec(o.onOkClick, select.find(':selected'));
                    div.$.dialog('close');
                }
            });
            openDialog(div, 600, 150, null, o.title);
        },
        //#endregion Specialized

        //#region Generic

        //		'OpenPopup': function (url) { 
        //							var popup = window.open(url, null, 'height=600, width=600, status=no, resizable=yes, scrollbars=yes, toolbar=yes,location=no, menubar=yes');
        //							popup.focus();
        //							return popup;
        //						},
        OpenDialog: function (id, url) {
            var div = Csw.literals.div({ name: id });
            div.$.load(url,
                function () {
                    openDialog(div, 600, 400);
                });
        },
        OpenEmptyDialog: function (options) {
            var o = {
                name: '',
                title: '',
                width: 900,
                height: 600,
                onOpen: null,
                onClose: null
            };
            if (options) {
                Csw.extend(o, options);
            }
            var div = Csw.literals.div(o.name);
            openDialog(div, o.width, o.height, o.onClose, o.title);
            Csw.tryExec(o.onOpen, div);
        },
        CloseDialog: function (id) {
            posX -= incrPosBy;
            posY -= incrPosBy;
            dialogsCount--;
            $('#' + id)
                .dialog('close')
                .remove();
        }

        //#region Generic
    };


    function openDialog(div, width, height, onClose, title, onOpen) {

        $('<div id="DialogErrorDiv" style="display: none;"></div>')
            .prependTo(div.$);

        Csw.tryExec(div.$.dialog, 'close');
        if (dialogsCount === 0) { //as per discussion - dialogs should be centered
            posX = (cswPrivate.windowWidth() / 2) - (width / 2) + posX;
            posY = (cswPrivate.windowHeight() / 2) - (height / 2) + posY;
        }

        div.$.dialog({
            modal: true,
            width: width,
            height: height,
            title: title,
            position: [posX, posY],
            beforeClose: function () {
                return Csw.clientChanges.manuallyCheckChanges();
            },
            close: function () {
                posX -= incrPosBy;
                posY -= incrPosBy;
                dialogsCount--;
                Csw.tryExec(onClose);

                unbindEvents();
                if (dialogsCount === 0) {
                    posX = cswPrivate.origXAccessor();
                    posY = cswPrivate.origYAccessor();
                }
                div.remove(); //case 27566
            },
            dragStop: function () {
                var newPos = div.$.dialog("option", "position");
                posX = newPos[0] + incrPosBy;
                posY = newPos[1] + incrPosBy;
            },
            open: function () {
                dialogsCount++;
                Csw.tryExec(onOpen);
            }
        });
        posX += incrPosBy;
        posY += incrPosBy;

        var doClose = function () {
            if (Csw.clientChanges.manuallyCheckChanges()) {
                Csw.tryExec(onClose);
                div.$.dialog('close');
                unbindEvents();
            }
        };
        var closeMe = function (eventObj, action) {
            afterObjectClassButtonClick(action, {
                close: function () {
                    doClose();
                }
            });
        };
        var unbindEvents = function () {
            Csw.subscribe(Csw.enums.events.afterObjectClassButtonClick, closeMe);
            Csw.subscribe('initGlobalEventTeardown', doClose);
        };

    }

    // Method calling logic
    $.CswDialog = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
