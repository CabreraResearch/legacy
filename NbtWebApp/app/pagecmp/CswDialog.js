/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswDialog';

    var cswPrivateInit = function () { //create this to prevent anyone from modifying the orginal position of the dialog positions
        var origX = 150;
        var origY = 30;

        this.origXAccessor = function () {
            return origX;
        }
        this.origYAccessor = function () {
            return origY;
        }
    };
    var cswPrivate = new cswPrivateInit();

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
                ID: 'renew_btn',
                enabledText: 'Yes',
                bindOnEnter: div,
                onClick: function () {
                    div.$.dialog('close');
                    Csw.tryExec(o.onYes);
                }
            });

            openDialog(div, 300, 150, null, 'Expire Warning');
        }, // ExpireDialog
        AddWelcomeItemDialog: function (options) {
            var o = {
                onAdd: function () { }
            };

            if (options) Csw.extend(o, options);

            var div = Csw.literals.div();

            div.$.CswWelcome('getAddItemForm', {
                'onAdd': function () {
                    div.$.dialog('close');
                    Csw.tryExec(o.onAdd);
                }
            });

            openDialog(div, 400, 400, null, 'New Welcome Item');
        }, // AddWelcomeItemDialog
        AddViewDialog: function (options) {
            var o = {
                ID: 'addviewdialog',
                onAddView: function () { },
                viewid: '',
                viewmode: '',
                category: ''
            };
            if (options) Csw.extend(o, options);

            var div = Csw.literals.div();
            var table = div.table({
                ID: Csw.makeId(o.ID, 'tbl'),
                FirstCellRightAlign: true
            });

            var row = 1;
            table.cell(row, 1).text('Name:');
            var nameTextCell = table.cell(row, 2);
            var nameTextBox = nameTextCell.input({
                ID: o.ID + '_nametb',
                type: Csw.enums.inputTypes.text,
                cssclass: 'textinput'
            });
            row += 1;

            table.cell(row, 1).text('Category:');
            var categoryTextCell = table.cell(row, 2);
            var categoryTextBox = categoryTextCell.input({
                ID: o.ID + '_cattb',
                type: Csw.enums.inputTypes.text,
                value: o.category,
                cssclass: 'textinput'
            });
            row += 1;

            var displayModeSelect;
            if (Csw.isNullOrEmpty(o.viewmode)) {
                table.cell(row, 1).text('Display Mode:');
                displayModeSelect = table.cell(row, 2).select({ ID: o.ID + '_dmsel' });
                displayModeSelect.option({ value: 'Grid' });
                displayModeSelect.option({ value: 'List' });
                displayModeSelect.option({ value: 'Table' });
                displayModeSelect.option({ value: 'Tree', isSelected: true });
                row += 1;
            }

            var visSelect = Csw.controls.makeViewVisibilitySelect(table, row, 'Available to:');
            row += 1;
            var saveBtn = div.button({
                ID: o.ID + '_submit',
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

                    if (false === Csw.isNullOrEmpty(visSelect.$visibilityselect)) {
                        createData.Visibility = visSelect.$visibilityselect.val();
                        createData.VisibilityRoleId = visSelect.$visroleselect.val();
                        createData.VisibilityUserId = visSelect.$visuserselect.val();
                    }

                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/createView',
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
                ID: o.ID + '_cancel',
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
            var cswPrivate = {
                text: '',
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
            Csw.extend(cswPrivate, options);
            cswPrivate.ID = Csw.makeSafeId(cswPrivate.text, Math.floor(Math.random() * 99999));
            var cswPublic = {
                div: Csw.literals.div({ ID: cswPrivate.ID }),
                close: function () {
                    cswPublic.div.$.dialog('close');
                },
                title: 'New ' + cswPrivate.text
            };

            openDialog(cswPublic.div, 800, 600, null, cswPublic.title);

            cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(cswPublic.div, {
                ID: Csw.makeId(cswPrivate.ID, 'tabsAndProps'),
                nodetypeid: cswPrivate.nodetypeid,
                relatednodeid: cswPrivate.relatednodeid,
                relatednodename: cswPrivate.relatednodename,
                relatednodetypeid: cswPrivate.relatednodetypeid,
                relatedobjectclassid: cswPrivate.relatedobjectclassid,
                propertyData: cswPrivate.propertyData,
                EditMode: Csw.enums.editMode.Add,
                ReloadTabOnSave: false,
                onSave: function (nodeid, cswnbtnodekey, tabcount, nodename) {
                    cswPublic.div.$.dialog('close');
                    Csw.tryExec(cswPrivate.onAddNode, nodeid, cswnbtnodekey, nodename);
                    Csw.tryExec(cswPrivate.onSaveImmediate);
                },
                onInitFinish: function () {
                    //openDialog(cswPublic.div, 800, 600, null, cswPublic.title);
                },
                ShowAsReport: false
            });
            return cswPublic;
        },
        AddFeedbackDialog: function (options) {
            ///<summary>Creates an Add Feedback dialog and returns an object represent that dialog.</summary>
            var cswPrivate = {
                text: '',
                nodetypeid: '',
                onAddNode: function () { }
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Add Feedback without options.', '', 'CswDialog.js', 215));
            }
            Csw.extend(cswPrivate, options);
            var cswPublic = {
                div: Csw.literals.div(),
                close: function () {
                    cswPublic.div.$.dialog('close');
                },
                title: 'New ' + cswPrivate.text
            };

            cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(cswPublic.div, {
                nodetypeid: cswPrivate.nodetypeid,
                EditMode: Csw.enums.editMode.Add,
                ReloadTabOnSave: false,
                onSave: function (nodeid, cswnbtnodekey, tabcount, nodename) {
                    Csw.ajax.post({
                        urlMethod: 'GetFeedbackCaseNumber',
                        data: { nodeId: nodeid },
                        success: function (result) {

                            var closeDialog = function () { cswPublic.div.$.dialog('close'); };

                            cswPublic.div.$.empty();
                            //div.text('Your feedback has been submitted. Your case number is ' + result.casenumber + '.');
                            cswPublic.div.nodeLink({
                                text: 'Your feedback has been submitted. Your case number is ' + result.noderef + '.',
                                onClick: closeDialog
                            });

                            cswPublic.div.br();
                            cswPublic.div.button({
                                ID: '_feedbackOk',
                                enabledText: 'OK',
                                onClick: closeDialog
                            });
                            Csw.tryExec(cswPrivate.onAddNode, nodeid, cswnbtnodekey, nodename);
                        }
                    });
                },
                onInitFinish: function () {
                    openDialog(cswPublic.div, 800, 600, null, cswPublic.title);
                },
                ShowAsReport: false
            });
            return cswPublic;
        }, // AddFeedbackDialog
        AddNodeClientSideDialog: function (options) {
            var o = {
                ID: '',
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
            newNode = div.input({ ID: o.ID + '_newNode', type: Csw.enums.inputTypes.text });

            div.button({
                ID: o.objectClassId + '_add',
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
            nodeTypeInp = div.input({ ID: o.objectClassId + '_nodeType', type: Csw.enums.inputTypes.text, value: o.nodetypename, maxlength: o.maxlength });
            div.br();
            if (Csw.isNullOrEmpty(category)) {
                div.append('Category Name: ');
                categoryInp = div.input({ ID: o.objectClassId + '_category', type: Csw.enums.inputTypes.text });
                div.br();
            }
            addBtn = div.button({
                ID: o.objectClassId + '_add',
                enabledText: 'Add',
                onClick: function () {
                    var newNodeTypeName = nodeTypeInp.val();
                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/IsNodeTypeNameUnique',
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
        EditLayoutDialog: function (cswNodeTabOptions) {
            cswNodeTabOptions.ID = cswNodeTabOptions.ID + '_editlayout';
            cswNodeTabOptions.Config = true;
            cswNodeTabOptions.ShowAsReport = false;
            cswNodeTabOptions.EditMode = 'Edit';
            cswNodeTabOptions.onTabSelect = function (tabid) {
                cswNodeTabOptions.tabid = tabid;
                _configAddOptions();
            };
            cswNodeTabOptions.onPropertyRemove = function () {
                _configAddOptions();
            };

            var div = Csw.literals.div();
            var table = div.table({
                ID: 'EditLayoutDialog_table',
                width: '100%'
            });

            /* Keep the add content in the same space */
            var table2 = table.cell(1, 1).table();
            var cell11 = table2.cell(1, 1);
            //cell11.append('Configure:');

            var cell12 = table.cell(1, 2);

            var layoutSelect = cell11.select({
                ID: 'EditLayoutDialog_layoutselect',
                labelText: 'Configure: ',
                selected: 'Edit',
                values: ['Add', 'Edit', 'Preview', 'Table'],
                onChange: function () {
                    cswNodeTabOptions.EditMode = $('#EditLayoutDialog_layoutselect option:selected').val();
                    _resetLayout();
                }
            });

            var cell21 = table2.cell(2, 1);

            function _resetLayout() {
                cell12.empty();
                //cell12.$.CswNodeTabs(cswNodeTabOptions);
                Csw.layouts.tabsAndProps(cell12, cswNodeTabOptions);
                _configAddOptions();
            }

            function _configAddOptions() {
                cell21.empty();
                cell21.br({ number: 2 });

                var addSelect = cell21.select({
                    ID: 'EditLayoutDialog_addselect',
                    labelText: 'Add: ',
                    selected: '',
                    values: [],
                    onChange: function () {
                        Csw.ajax.post({
                            url: '/NbtWebApp/wsNBT.asmx/addPropertyToLayout',
                            data: {
                                PropId: Csw.string(addSelect.val()),
                                TabId: Csw.string(cswNodeTabOptions.tabid),
                                LayoutType: layoutSelect.val()
                            },
                            success: function () {
                                _resetLayout();
                            }
                        }); // Csw.ajax
                    } // onChange
                }); // 
                var ajaxdata = {
                    NodeId: Csw.string(cswNodeTabOptions.nodeids[0]),
                    NodeKey: Csw.string(cswNodeTabOptions.nodekeys[0]),
                    NodeTypeId: Csw.string(cswNodeTabOptions.nodetypeid),
                    TabId: Csw.string(cswNodeTabOptions.tabid),
                    LayoutType: layoutSelect.val()
                };
                Csw.ajax.post({
                    url: '/NbtWebApp/wsNBT.asmx/getPropertiesForLayoutAdd',
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
                });  // Csw.ajax
            } // _configAddOptions()

            function _onclose() {
                Csw.tryExec(cswNodeTabOptions.Refresh);
            }

            _resetLayout();

            openDialog(div, 900, 600, _onclose, 'Edit Layout');
        }, // EditLayoutDialog
        EditNodeDialog: function (options) {
            var cswPrivate = {
                nodeids: [],
                nodepks: [],
                nodekeys: [],
                nodenames: [],
                Multi: false,
                ReadOnly: false,
                filterToPropId: '',
                title: '',
                onEditNode: null, // function (nodeid, nodekey) { },
                onEditView: null, // function (viewid) {}
                onRefresh: null,
                onClose: null,
                onAfterButtonClick: null,
                date: ''     // viewing audit records
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Add Dialog without options.', '', 'CswDialog.js', 177));
            }
            Csw.extend(cswPrivate, options);
            var cswPublic = {
                div: Csw.literals.div(),
                close: function () {
                    cswPublic.div.$.dialog('close');
                }
            };

            var myEditMode = Csw.enums.editMode.EditInPopup;
            var table = cswPublic.div.table();
            if (false === Csw.isNullOrEmpty(cswPrivate.date) && false === cswPrivate.Multi) {
                myEditMode = Csw.enums.editMode.AuditHistoryInPopup;
                Csw.actions.auditHistory(table.cell(1, 1), {
                    ID: cswPrivate.nodeids[0] + '_history',
                    nodeid: cswPrivate.nodeids[0],
                    cswnbtnodekey: cswPrivate.nodekeys[0],
                    onEditNode: cswPrivate.onEditNode,
                    JustDateColumn: true,
                    selectedDate: cswPrivate.date,
                    onSelectRow: function (date) { setupTabs(date); },
                    allowEditRow: false
                });
            }
            var tabCell = table.cell(1, 2);

            setupTabs(cswPrivate.date);

            function setupTabs(date) {
                tabCell.empty();
                //tabCell.$.CswNodeTabs({

                cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(tabCell, {
                    nodeids: cswPrivate.nodeids,
                    nodekeys: cswPrivate.nodekeys,
                    nodenames: cswPrivate.nodenames,
                    filterToPropId: cswPrivate.filterToPropId,
                    Multi: cswPrivate.Multi,
                    ReadOnly: cswPrivate.ReadOnly,
                    EditMode: myEditMode,
                    //title: o.title,
                    tabid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentTabId),
                    date: date,
                    ReloadTabOnSave: true,
                    Refresh: cswPrivate.onRefresh,
                    onEditView: function (viewid) {
                        cswPublic.close();
                        Csw.tryExec(cswPrivate.onEditView, viewid);
                    },
                    onSave: function (nodeids, nodekeys, tabcount) {
                        Csw.clientChanges.unsetChanged();
                        if (tabcount === 2 || cswPrivate.Multi) { /* Ignore history tab */
                            cswPublic.close();
                        }
                        //setupTabs(date);//case 26107
                        Csw.tryExec(cswPrivate.onEditNode, nodeids, nodekeys, cswPublic.close);
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
                    onAfterButtonClick: cswPrivate.onAfterButtonClick
                });
            } // _setupTabs()
            var title = Csw.string(cswPrivate.title);
            if (Csw.isNullOrEmpty(title)) {
                title = (false === cswPrivate.Multi) ? cswPrivate.nodenames[0] : cswPrivate.nodenames.join(', ');
            }
            cswPublic.title = title;
            openDialog(cswPublic.div, 900, 600, cswPrivate.onClose, title);
            return cswPublic;
        }, // EditNodeDialog
        CopyNodeDialog: function (options) {
            var cswPrivate = {
                'nodename': '',
                'nodeid': '',
                'nodetypeid': '',
                'cswnbtnodekey': '',
                'onCopyNode': function () { }
            };

            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Copy Dialog without options.', '', 'CswDialog.js', 177));
            }
            Csw.extend(cswPrivate, options);
            var cswPublic = {
                div: Csw.literals.div({ ID: 'CopyNodeDialogDiv' }),
                close: function () {
                    cswPublic.div.$.dialog('close');
                }
            };

            // Prevent copy if quota is reached
            var tbl = cswPublic.div.table({ ID: 'CopyNodeDialogDiv_table' });
            var cell11 = tbl.cell(1, 1).propDom('colspan', '2');
            var cell21 = tbl.cell(2, 1);
            var cell22 = tbl.cell(2, 2);

            Csw.ajax.post({
                urlMethod: 'checkQuota',
                data: {
                    NodeTypeId: Csw.string(cswPrivate.nodetypeid),
                    NodeKey: Csw.string(cswPrivate.cswnbtnodekey)
                },
                success: function (data) {
                    if (Csw.bool(data.result)) {

                        cell11.append('Copying: ' + cswPrivate.nodename);
                        cell11.br({ number: 2 });

                        var copyBtn = cell21.button({ ID: 'copynode_submit',
                            enabledText: 'Copy',
                            disabledText: 'Copying',
                            onClick: function () {
                                Csw.copyNode({
                                    'nodeid': cswPrivate.nodeid,
                                    'nodekey': Csw.string(cswPrivate.nodekey, cswPrivate.cswnbtnodekey[0]),
                                    'onSuccess': function (nodeid, nodekey) {
                                        cswPublic.close();
                                        cswPrivate.onCopyNode(nodeid, nodekey);
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
            cell22.button({ ID: 'copynode_cancel',
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
            var cswPrivate = {
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
            Csw.extend(cswPrivate, options);
            var cswPublic = {
                div: Csw.literals.div(),
                close: function () {
                    cswPublic.div.$.dialog('close');
                }
            };

            cswPublic.div.span({ text: 'Are you sure you want to delete' });

            if (cswPrivate.Multi) {
                //var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
                //var nodechecked = $('#' + o.NodeCheckTreeId).CswNodeTree('checkedNodes');
                cswPublic.div.span({ text: '&nbsp;the following?' }).br();
                var nodechecks = null;
                if (false == Csw.isNullOrEmpty(cswPrivate.nodeTreeCheck)) {
                    nodechecks = cswPrivate.nodeTreeCheck.checkedNodes();
                }
                if (false === Csw.isNullOrEmpty(nodechecks, true) && (cswPrivate.nodeids.length === 0 || cswPrivate.cswnbtnodekeys.length === 0)) {
                    var n = 0;
                    //$nodechecks.each(function () {
                    Csw.each(nodechecks, function (thisObj) {
                        cswPrivate.nodeids[n] = thisObj.nodeid;
                        cswPrivate.cswnbtnodekeys[n] = thisObj.cswnbtnodekey;
                        cswPublic.div.span({ text: thisObj.nodename }).css({ 'padding-left': '10px' }).br();
                        n += 1;
                    });
                } else {
                    for (var i = 0; i < cswPrivate.nodenames.length; i++) {
                        cswPublic.div.span({ text: cswPrivate.nodenames[i] }).css({ 'padding-left': '10px' }).br();
                    }
                }
            } else {
                cswPublic.div.span({ text: ':&nbsp;' + cswPrivate.nodenames + '?' });
            }
            cswPublic.div.br({ number: 2 });

            var deleteBtn = cswPublic.div.button({ ID: 'deletenode_submit',
                enabledText: 'Delete',
                disabledText: 'Deleting',
                onClick: function () {
                    Csw.deleteNodes({
                        nodeids: cswPrivate.nodeids,
                        nodekeys: cswPrivate.cswnbtnodekeys,
                        onSuccess: function (nodeid, nodekey) {
                            cswPublic.close();
                            Csw.tryExec(cswPrivate.onDeleteNode, nodeid, nodekey);
                            if (Csw.bool(cswPrivate.publishDeleteEvent)) {
                                Csw.publish(Csw.enums.events.CswNodeDelete, { nodeids: cswPrivate.nodeids, cswnbtnodekeys: cswPrivate.cswnbtnodekeys });
                            }
                        },
                        onError: deleteBtn.enable
                    });
                }
            });
            /* Cancel Button */
            cswPublic.div.button({ ID: 'deletenode_cancel',
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
                url: '/NbtWebApp/wsNBT.asmx/getAbout',
                data: {},
                success: function (data) {
                    div.append('NBT Assembly Version: ' + data.assembly + '<br/><br/>');
                    var table = div.table({
                        ID: 'abouttale'
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
                url: '',
                params: {},
                onSuccess: function () { }
            };
            if (options) {
                Csw.extend(o, options);
            }

            var div = Csw.literals.div();

            var uploadBtn = div.input({ ID: 'fileupload', type: Csw.enums.inputTypes.file });

            uploadBtn.$.fileupload({
                datatype: 'json',
                url: o.url + '?' + $.param(o.params),
                paramName: 'fileupload',
                success: function (result, textStatus, jqXHR) {
                    div.$.dialog('close');
                    Csw.tryExec(o.onSuccess, result.data);
                }
            });

            div.button({ ID: 'fileupload_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    div.$.dialog('close');
                }
            });

            openDialog(div, 400, 300, null, 'Upload');
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

            var uploadBtn = div.input({ ID: 'fileupload', type: Csw.enums.inputTypes.file });

            uploadBtn.$.fileupload({
                datatype: 'json',
                url: o.FileUrl + '?' + $.param({ PropId: o.PropId }),
                paramName: 'fileupload',
                done: function (e, data) {
                    div.$.dialog('close');
                    o.onSuccess();
                }
            });

            div.br({ number: 2 });

            div.span({ text: 'MOL Text (Paste from Clipboard):' }).br();

            molTxtArea = div.textArea({ ID: '', rows: 6, cols: 40 });
            molTxtArea.text(o.molData);
            div.br();

            var buttonsDiv = div.div({ align: 'right' });

            saveBtn = buttonsDiv.button({ ID: 'txt_save',
                enabledText: 'Save',
                disabledText: 'Saving...',
                onClick: function () {
                    Csw.ajax.post({
                        url: o.TextUrl,
                        data: {
                            molData: molTxtArea.val(),
                            PropId: o.PropId
                        },
                        success: function (data) {
                            div.$.dialog('close');
                            Csw.tryExec(o.onSuccess);
                        },
                        error: saveBtn.enable
                    }); // ajax
                } // onClick
            }); // 

            buttonsDiv.button({
                ID: 'fileupload_cancel',
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
                GetLicenseUrl: '/NbtWebApp/wsNBT.asmx/getLicense',
                AcceptLicenseUrl: '/NbtWebApp/wsNBT.asmx/acceptLicense',
                onAccept: function () { },
                onDecline: function () { }
            };
            if (options) {
                Csw.extend(o, options);
            }

            var div = Csw.literals.div({ align: 'center' });
            div.append('Service Level Agreement').br();
            var licenseTextArea = div.textArea({ ID: 'license', rows: 30, cols: 80 }).propDom({ disabled: true });
            div.br();

            Csw.ajax.post({
                url: o.GetLicenseUrl,
                success: function (data) {
                    licenseTextArea.text(data.license);
                }
            });

            var acceptBtn = div.button({
                ID: 'license_accept',
                enabledText: 'I Accept',
                disabledText: 'Accepting...',
                onClick: function () {
                    Csw.ajax.post({
                        url: o.AcceptLicenseUrl,
                        success: function () {
                            div.$.dialog('close');
                            Csw.tryExec(o.onAccept);
                        },
                        error: acceptBtn.enable
                    }); // ajax
                } // onClick
            }); // 

            div.button({ ID: 'license_decline',
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
            var cswPrivate = {
                ID: 'print_label',
                GetPrintLabelsUrl: '/NbtWebApp/wsNBT.asmx/getLabels',
                GetEPLTextUrl: '/NbtWebApp/wsNBT.asmx/getEPLText',
                nodeid: '',
                propid: ''
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Print Label Dialog without options.', '', 'CswDialog.js', 893));
            }
            Csw.extend(cswPrivate, options);
            var cswPublic = {
                div: Csw.literals.div({ text: 'Select a Label to Print:' }),
                close: function () {
                    cswPublic.div.$.dialog('close');
                }
            };

            var getEplContext = function() {
                Csw.openPopup('Print.html?PropId=' + cswPrivate.propid + '&PrintLabelNodeId=' + labelSel.val(), 'Print ' + labelSel.selectedText(), {
                    width: 400,
                    height: 200,
                    location: 'no',
                    toolbar: 'no',
                    status: 'no',
                    menubar: 'no',
                    chrome: 'yes',
                    centerscreen: 'yes'
                });
                cswPublic.close();
            };
            
            cswPublic.div.br();
            var labelSelDiv = cswPublic.div.div();
            var labelSel = labelSelDiv.select({
                ID: cswPrivate.ID + '_labelsel'
            });
            
            var jData = { PropId: cswPrivate.propid };
            Csw.ajax.post({
                url: cswPrivate.GetPrintLabelsUrl,
                data: jData,
                success: function (data) {
                    if (data.labels.length > 0) {
                        for (var i = 0; i < data.labels.length; i++) {
                            var label = data.labels[i];
                            labelSel.option({ value: label.nodeid, display: label.name });
                        }
                    } else {
                        
                        labelSelDiv.span({ text: 'No labels have been assigned!' });
                    }
                } // success
            }); // ajax
            
            cswPublic.div.button({ ID: 'print_label_close',
                enabledText: 'Close',
                disabledText: 'Closing...',
                onClick: function () {
                    cswPublic.close();
                }
            });

            cswPublic.div.button({
                ID: 'print_label_print',
                enabledText: 'Print',
                //disabledText: 'Printing...', 
                disableOnClick: false,
                onClick: getEplContext
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
                url: '/NbtWebApp/wsNBT.asmx/getUsers',
                success: function (data) {
                    if (Csw.bool(data.result)) {
                        var usersel = div.select({
                            ID: 'ImpersonateSelect',
                            selected: ''
                        });

                        Csw.each(data.users, function (thisUser) {
                            usersel.addOption({ value: thisUser.userid, display: thisUser.username }, false);
                        });

                        div.button({
                            ID: 'ImpersonateButton',
                            enabledText: 'Impersonate',
                            onClick: function () {
                                Csw.tryExec(o.onImpersonate, usersel.val(), usersel.selectedText());
                                div.$.dialog('close');
                            }
                        });

                        div.button({
                            ID: 'CancelButton',
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
            var cswPrivate = {
                ID: 'searchdialog',
                propname: '',
                title: '',
                nodetypeid: '',
                objectclassid: '',
                onSelectNode: null
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Search Dialog without options.', '', 'CswDialog.js', 1013));
            }
            Csw.extend(cswPrivate, options);
            var cswPublic = {
                div: Csw.literals.div({ ID: 'searchdialog_div' }),
                close: function () {
                    cswPublic.div.$.dialog('close');
                },
                title: Csw.string(cswPrivate.title, 'Search ' + cswPrivate.propname)
            };

            cswPublic.search = Csw.composites.universalSearch(cswPublic.div, {
                ID: cswPrivate.ID,
                nodetypeid: cswPrivate.nodetypeid,
                objectclassid: cswPrivate.objectclassid,
                onBeforeSearch: function () { },
                onAfterSearch: function () { },
                onAfterNewSearch: function (searchid) { },
                onAddView: function (viewid, viewmode) { },
                onLoadView: function (viewid, viewmode) { },
                showSaveAsView: false,
                allowEdit: false,
                allowDelete: false,
                extraAction: 'Select',
                extraActionIcon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.check),
                onExtraAction: function (nodeObj) {
                    cswPublic.close();
                    Csw.tryExec(cswPrivate.onSelectNode, nodeObj);
                }
            });
            openDialog(cswPublic.div, 800, 600, null, 'Search ' + cswPrivate.propname);
            return cswPublic;
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

            var div = Csw.literals.div({ ID: 'searchdialog_div' });

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

        AlertDialog: function (message, title) {

            var div = Csw.literals.div({
                ID: Csw.string(title, 'an alert dialog').replace(' ', '_'),
                text: message,
                align: 'center'
            });

            div.br();

            div.button({
                enabledText: 'OK',
                onClick: function () {
                    div.$.dialog('close');
                }
            });

            openDialog(div, 400, 200, null, title);
        },
        ConfirmDialog: function (message, title, okFunc, cancelFunc) {
            var width = Csw.number((message.length * 7), 200);
            var div = Csw.literals.div({
                ID: Csw.string(title, 'an alert dialog').replace(' ', '_'),
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

            openDialog(div, width, 200, null, title);
        },
        NavigationSelectDialog: function (options) {
            var o = {
                ID: '',
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
                ID: o.ID
            });
            div.p({ text: o.navigationText });
            var select = div.select({
                ID: Csw.makeId({ ID: o.ID, suffix: 'CswNavigationSelectDialog' }),
                values: o.values
            });

            div.button({
                ID: Csw.makeId({ ID: o.ID, suffix: 'CswNavigationSelectDialog_OK' }),
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
            var div = Csw.literals.div({ ID: id });
            div.$.load(url,
                function () {
                    openDialog(div, 600, 400);
                });
        },
        OpenEmptyDialog: function (options) {
            var o = {
                ID: '',
                title: '',
                width: 900,
                height: 600,
                onOpen: null,
                onClose: null
            };
            if (options) {
                Csw.extend(o, options);
            }
            var div = Csw.literals.div(o.ID);
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


    function openDialog(div, width, height, onClose, title) {
        $('<div id="DialogErrorDiv" style="display: none;"></div>')
            .prependTo(div.$);

        Csw.tryExec(div.$.dialog, 'close');

        div.$.dialog({
            modal: true,
            width: width,
            height: height,
            title: title,
            position: [posX, posY],
            close: function () {
                posX -= incrPosBy;
                posY -= incrPosBy;
                dialogsCount--;
                if (Csw.isFunction(onClose)) {
                    Csw.tryExec(onClose);
                }
                
                Csw.unsubscribe(Csw.enums.events.afterObjectClassButtonClick, closeMe);
                if (dialogsCount === 0) {
                    posX = cswPrivate.origXAccessor();
                    posY = cswPrivate.origYAccessor();
                }
            },
            dragStop: function () {
                var newPos = div.$.dialog("option", "position");
                posX = newPos[0] + incrPosBy;
                posY = newPos[1] + incrPosBy;
            },
            open: function () {
                dialogsCount++;
            }
        });
        posX += incrPosBy;
        posY += incrPosBy;
        function closeMe(eventObj, action) {
            afterObjectClassButtonClick(action, {
                close: function () {
                    div.$.dialog('close');
                    Csw.unsubscribe(Csw.enums.events.afterObjectClassButtonClick, closeMe);
                }
            });
            posX -= incrPosBy;
            posY -= incrPosBy;
            dialogsCount--;
        }
        Csw.subscribe(Csw.enums.events.afterObjectClassButtonClick, closeMe);
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
