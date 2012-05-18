/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswDialog';

    var methods = {

        //#region Specialized

        ExpireDialog: function (options) {
            var o = {
                onYes: null
            };

            if (options) {
                $.extend(o, options);
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
            
            openDialog(div, 300, 150, Csw.clientSession.logout, 'Expire Warning');

        }, // ExpireDialog
        AddWelcomeItemDialog: function (options) {
            var o = {
                onAdd: function () { }
            };

            if (options) $.extend(o, options);

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
            if (options) $.extend(o, options);

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
            var o = {
                text: '',
                nodetypeid: '',
                relatednodeid: '',
                relatednodename: '',
                relatednodetypeid: '',
                relatedobjectclassid: '',
                onAddNode: function () { }
            };

            if (options) {
                $.extend(o, options);
            }

            var div = Csw.literals.div(),
                title = 'New ' + o.text;

            //            div.$.CswNodeTabs({
            Csw.layouts.tabsAndProps(div, {
                nodetypeid: o.nodetypeid,
                relatednodeid: o.relatednodeid,
                relatednodename: o.relatednodename,
                relatednodetypeid: o.relatednodetypeid,
                relatedobjectclassid: o.relatedobjectclassid,
                EditMode: Csw.enums.editMode.Add,
                ReloadTabOnSave: false,
                onSave: function (nodeid, cswnbtnodekey, tabcount, nodename) {
                    div.$.dialog('close');
                    Csw.tryExec(o.onAddNode, nodeid, cswnbtnodekey, nodename);
                },
                onInitFinish: function () {
                    openDialog(div, 800, 600, null, title);
                },
                ShowAsReport: false
            });

        }, // AddNodeDialog
        AddNodeClientSideDialog: function (options) {
            var o = {
                ID: '',
                nodetypename: '',
                title: '',
                onSuccess: null
            };

            if (options) {
                $.extend(o, options);
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
                $.extend(o, options);
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
                            if(Csw.bool(p.hidden)) {
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
            var o = {
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
                onAfterButtonClick: null,
                date: ''     // viewing audit records
            };
            if (options) $.extend(o, options);

            var div = Csw.literals.div();

            var myEditMode = Csw.enums.editMode.EditInPopup;
            var table = div.table();
            if (false === Csw.isNullOrEmpty(o.date) && false === o.Multi) {
                myEditMode = Csw.enums.editMode.AuditHistoryInPopup;
                Csw.actions.auditHistory(table.cell(1, 1), {
                    ID: o.nodeids[0] + '_history',
                    nodeid: o.nodeids[0],
                    cswnbtnodekey: o.nodekeys[0],
                    onEditNode: o.onEditNode,
                    JustDateColumn: true,
                    selectedDate: o.date,
                    onSelectRow: function (date) { setupTabs(date); },
                    allowEditRow: false
                });
            }
            var tabCell = table.cell(1, 2);

            setupTabs(o.date);

            function setupTabs(date) {
                tabCell.empty();
                //tabCell.$.CswNodeTabs({
                Csw.layouts.tabsAndProps(tabCell, {
                    nodeids: o.nodeids,
                    nodekeys: o.nodekeys,
                    nodenames: o.nodenames,
                    filterToPropId: o.filterToPropId,
                    Multi: o.Multi,
                    ReadOnly: o.ReadOnly,
                    EditMode: myEditMode,
                    //title: o.title,
                    tabid: Csw.cookie.get(Csw.cookie.cookieNames.CurrentTabId),
                    date: date,
                    ReloadTabOnSave: false,
                    onEditView: function (viewid) {
                        div.$.dialog('close');
                        Csw.tryExec(o.onEditView, viewid);
                    },
                    onSave: function (nodeids, nodekeys, tabcount) {
                        Csw.clientChanges.unsetChanged();
                        if (tabcount === 1 || o.Multi) {
                            div.$.dialog('close');
                        }
                        //setupTabs(date);//case 26107
                        Csw.tryExec(o.onEditNode, nodeids, nodekeys);
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
                    onAfterButtonClick: o.onAfterButtonClick
                });
            } // _setupTabs()
            var title = Csw.string(o.title);
            if (Csw.isNullOrEmpty(title)) {
                title = (false === o.Multi) ? o.nodenames[0] : o.nodenames.join(', ');
            }
            openDialog(div, 900, 600, null, title);
        }, // EditNodeDialog
        CopyNodeDialog: function (options) {
            var o = {
                'nodename': '',
                'nodeid': '',
                'nodetypeid': '',
                'cswnbtnodekey': '',
                'onCopyNode': function () { }
            };

            if (options) {
                $.extend(o, options);
            }

            // Prevent copy if quota is reached
            var div = Csw.literals.div({ ID: 'CopyNodeDialogDiv' });
            var tbl = div.table({ ID: 'CopyNodeDialogDiv_table' });
            var cell11 = tbl.cell(1, 1).propDom('colspan', '2');
            var cell21 = tbl.cell(2, 1);
            var cell22 = tbl.cell(2, 2);

            Csw.ajax.post({
                urlMethod: 'checkQuota',
                data: { NodeTypeId: o.nodetypeid },
                success: function (data) {
                    if (Csw.bool(data.result)) {

                        cell11.append('Copying: ' + o.nodename);
                        cell11.br({number: 2});

                        var copyBtn = cell21.button({ ID: 'copynode_submit',
                            enabledText: 'Copy',
                            disabledText: 'Copying',
                            onClick: function () {
                                Csw.copyNode({
                                    'nodeid': o.nodeid,
                                    'nodekey': o.nodekey,
                                    'onSuccess': function (nodeid, nodekey) {
                                        div.$.dialog('close');
                                        o.onCopyNode(nodeid, nodekey);
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
                    div.$.dialog('close');
                }
            });

            openDialog(div, 400, 300, null, 'Confirm Copy');
        }, // CopyNodeDialog       
        DeleteNodeDialog: function (options) {
            var o = {
                nodenames: [],
                nodeids: [],
                cswnbtnodekeys: [],
                onDeleteNode: null, //function (nodeid, nodekey) { },
                Multi: false,
                nodeTreeCheck: null,
                publishDeleteEvent: true
            };

            if (options) {
                $.extend(o, options);
            }

            var div = Csw.literals.div();
            div.span({ text: 'Are you sure you want to delete' });


            if (o.Multi) {
                //var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
                //var nodechecked = $('#' + o.NodeCheckTreeId).CswNodeTree('checkedNodes');
                div.span({ text: '&nbsp;the following?' }).br();
                var nodechecks = null;
                if (false == Csw.isNullOrEmpty(o.nodeTreeCheck)) {
                    nodechecks = o.nodeTreeCheck.checkedNodes();
                }
                if (false === Csw.isNullOrEmpty(nodechecks, true) && (o.nodeids.length === 0 || o.cswnbtnodekeys.length === 0)) {
                    var n = 0;
                    //$nodechecks.each(function () {
                    Csw.each(nodechecks, function (thisObj) {
                        o.nodeids[n] = thisObj.nodeid;
                        o.cswnbtnodekeys[n] = thisObj.cswnbtnodekey;
                        div.span({ text: thisObj.nodename }).css({ 'padding-left': '10px' }).br();
                        n += 1;
                    });
                } else {
                    for (var i = 0; i < o.nodenames.length; i++) {
                        div.span({ text: o.nodenames[i] }).css({ 'padding-left': '10px' }).br();
                    }
                }
            } else {
                div.span({ text: ':&nbsp;' + o.nodenames + '?' });
            }
            div.br({number: 2});

            var deleteBtn = div.button({ ID: 'deletenode_submit',
                enabledText: 'Delete',
                disabledText: 'Deleting',
                onClick: function () {
                    Csw.deleteNodes({
                        nodeids: o.nodeids,
                        nodekeys: o.cswnbtnodekeys,
                        onSuccess: function (nodeid, nodekey) {
                            div.$.dialog('close');
                            Csw.tryExec(o.onDeleteNode, nodeid, nodekey);
                            if (Csw.bool(o.publishDeleteEvent)) {
                                Csw.publish(Csw.enums.events.CswNodeDelete, { nodeids: o.nodeids, cswnbtnodekeys: o.cswnbtnodekeys });
                            }
                        },
                        onError: deleteBtn.enable
                    });
                }
            });
            /* Cancel Button */
            div.button({ ID: 'deletenode_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    div.$.dialog('close');
                }
            });
            openDialog(div, 400, 200, null, 'Confirm Delete');
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
                $.extend(o, options);
            }

            var div = Csw.literals.div();

            var uploadBtn = div.input({ ID: 'fileupload', type: Csw.enums.inputTypes.file });

            uploadBtn.$.fileupload({
                datatype: 'json',
                url: o.url + '?' + $.param(o.params),
                paramName: 'fileupload',
                done: function () {
                    div.$.dialog('close');
                    Csw.tryExec(o.onSuccess);
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
                $.extend(o, options);
            }
            var div = Csw.literals.div(),
                molTxtArea, saveBtn;

            var uploadBtn = div.input({ ID: 'fileupload', type: Csw.enums.inputTypes.file });

            uploadBtn.$.fileupload({
                datatype: 'json',
                url: o.FileUrl + '?' + $.param({ PropId: o.PropId }),
                paramName: 'fileupload',
                done: function (e, data) {
                    molTxtArea.text(data.molData);
                    div.dialog('close');
                    o.onSuccess();
                }
            });

            div.button({
                ID: 'fileupload_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    div.$.dialog('close');
                }
            });

            div.span({ text: 'MOL Text (Paste from Clipboard):' }).br();

            molTxtArea = div.textArea({ ID: '', rows: 4, cols: 40 });
            div.br();
            saveBtn = div.button({ ID: 'txt_save',
                enabledText: 'Save MOL Text',
                disabledText: 'Saving MOL...',
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

            openDialog(div, 400, 300, null, 'Upload');
        }, // FileUploadDialog
        ShowLicenseDialog: function (options) {
            var o = {
                GetLicenseUrl: '/NbtWebApp/wsNBT.asmx/getLicense',
                AcceptLicenseUrl: '/NbtWebApp/wsNBT.asmx/acceptLicense',
                onAccept: function () { },
                onDecline: function () { }
            };
            if (options) {
                $.extend(o, options);
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

            var o = {
                ID: 'print_label',
                GetPrintLabelsUrl: '/NbtWebApp/wsNBT.asmx/getLabels',
                GetEPLTextUrl: '/NbtWebApp/wsNBT.asmx/getEPLText',
                nodeid: '',
                propid: ''
            };
            if (options) $.extend(o, options);

            var div = Csw.literals.div({ align: 'center', text: 'Select a Label to Print:' });
            div.br();
            var labelSelDiv = div.div();
            var labelSel = labelSelDiv.select({ ID: o.ID + '_labelsel' });

            var jData = { PropId: o.propid };
            Csw.ajax.post({
                url: o.GetPrintLabelsUrl,
                data: jData,
                success: function (data) {
                    if (data.labels.length > 0) {
                        for (var i = 0; i < data.labels.length; i++) {
                            var label = data.labels[i];
                            labelSel.option({ value: label.nodeid, display: label.name });
                        }
                        printBtn.enable();
                    } else {
                        printBtn.hide();
                        labelSelDiv.span({ text: 'No labels have been assigned!' });
                    }
                } // success
            }); // ajax

            var printBtn = div.button({
                ID: 'print_label_print',
                enabledText: 'Print',
                //disabledText: 'Printing...', 
                disableOnClick: false,
                onClick: function () {
                    var jData2 = { PropId: o.propid, PrintLabelNodeId: labelSel.val() };
                    Csw.ajax.post({
                        url: o.GetEPLTextUrl,
                        data: jData2,
                        success: function (data) {
                            var labelx = $('#labelx').get(0);
                            labelx.EPLScript = data.epl;
                            labelx.Print();
                        } // success
                    }); // ajax
                } // onClick
            }); // 
            printBtn.disable();

            div.button({ ID: 'print_label_close',
                enabledText: 'Close',
                disabledText: 'Closing...',
                onClick: function () {
                    div.$.dialog('close');
                }
            });

            var hiddenDiv = div.div().css({ display: 'none', border: '1px solid red' });

            hiddenDiv.append('<OBJECT ID="labelx" Name="labelx" classid="clsid:A8926827-7F19-48A1-A086-B1A5901DB7F0" codebase="CafLabelPrintUtil.cab#version=0,1,6,0" width=500 height=300 align=center hspace=0 vspace=0></OBJECT>');

            openDialog(div, 400, 300, null, 'Print');
        }, // PrintLabelDialog

        ImpersonateDialog: function (options) {
            var o = {
                onImpersonate: null
            };
            if (options) $.extend(o, options);

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
            var o = {
                propname: '',
                nodetypeid: '',
                objectclassid: '',
                onSelectNode: null
            };
            if (options) $.extend(o, options);

            var div = Csw.literals.div({ ID: 'searchdialog_div' });
            var table = div.table({ ID: 'searchdialog_table', cellpadding: '2px' });

            var cell11 = table.cell(1, 1);
            var cell21 = table.cell(2, 1);
            var cell22 = table.cell(2, 2);

            cell11.propDom('colspan', 2);

            var searchdiv = cell11.div({ ID: 'searchdialog_searchdiv' });
            var resultsdiv = cell22.div({ ID: 'searchdialog_resultsdiv' });
            var filtersdiv = cell21.div({ ID: 'searchdialog_filtersdiv' });

            var universalsearch = Csw.composites.universalSearch({}, {
                $searchbox_parent: searchdiv.$,
                $searchresults_parent: resultsdiv.$,
                $searchfilters_parent: filtersdiv.$,
                nodetypeid: o.nodetypeid,
                objectclassid: o.objectclassid,
                onBeforeSearch: function () { },
                onAfterSearch: function () { },
                onAfterNewSearch: function (searchid) { },
                onAddView: function (viewid, viewmode) { },
                onLoadView: function (viewid, viewmode) { },
                showSaveAsView: false,
                allowEdit: false,
                allowDelete: false,
                extraAction: 'Select',
                onExtraAction: function (nodeObj) {
                    div.$.dialog('close');
                    Csw.tryExec(o.onSelectNode, nodeObj);
                }
            });

            openDialog(div, 800, 600, null, 'Search ' + o.propname);
        }, // SearchDialog

        GenericDialog: function(options) {
            var o = {
                div: null, 
                title: '', 
                onOk: null, 
                onCancel: null,
                onClose: null,
                height: 400,
                width: 600
            };
            if(options) $.extend(o, options);

            o.div.button({
                enabledText: 'OK',
                onClick: function () {
                    Csw.tryExec(o.onOk);
                    o.div.$.dialog('close');
                }
            });

            o.div.button({
                enabledText: 'Cancel',
                onClick: function () {
                    Csw.tryExec(o.onCancel);
                    o.div.$.dialog('close');
                }
            });

            openDialog(o.div, o.width, o.height, o.onClose, o.title);

        }, // GenericDialog


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
                $.extend(o, options);
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
                $.extend(o, options);
            }
            var div = Csw.literals.div(o.ID);
            Csw.tryExec(o.onOpen, div);
            openDialog(div, o.width, o.height, o.onClose, o.title);
        },
        CloseDialog: function (id) {
            $('#' + id)
                .dialog('close')
                .remove();
        }

        //#region Generic
    };


    function openDialog(div, width, height, onClose, title) {
        $('<div id="DialogErrorDiv" style="display: none;"></div>')
            .prependTo(div.$);

        div.$.dialog({
            modal: true,
            width: width,
            height: height,
            title: title,
            close: function () {
                div.remove();
                Csw.tryExec(onClose);
            }
        });
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
