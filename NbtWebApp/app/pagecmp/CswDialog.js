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
    }());
    var cswPrivate = cswPrivateInit();
    cswPrivate.div = Csw.literals.div();

    var posX = cswPrivate.origXAccessor();
    var posY = cswPrivate.origYAccessor();
    var incrPosBy = 30;

    // case 21337 - Prevent showing multiples of these dialogs...
    var ExistingChangePasswordDialog = false;
    var ExistingShowLicenseDialog = false;
    var ExistingMultisessionDialog = false;

    var methods = {

        //#region Specialized

        ExpireDialog: function (options) {
            'use strict';
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
            'use strict';
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
            'use strict';
            var o = {
                name: 'addviewdialog',
                onAddView: function () { },
                viewid: '',
                viewmode: '',
                category: ''
            };
            if (options) Csw.extend(o, options);

            var div = Csw.literals.div();
            var form = div.form();
            var table = form.table({
                name: 'tbl',
                FirstCellRightAlign: true
            });

            var row = 1;
            table.cell(row, 1).setLabelText("Name", true, false);
            var nameTextCell = table.cell(row, 2);
            var nameTextBox = nameTextCell.input({
                name: 'nametb',
                type: Csw.enums.inputTypes.text,
                cssclass: 'textinput',
            }).required(true, false);
            row += 1;

            table.cell(row, 1).setLabelText('Category', false, false);
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
                table.cell(row, 1).setLabelText('Display Mode', false, false);
                displayModeSelect = table.cell(row, 2).select({ name: o.name + '_dmsel' });
                displayModeSelect.option({ value: 'Grid' });
                displayModeSelect.option({ value: 'List' });
                displayModeSelect.option({ value: 'Table' });
                displayModeSelect.option({ value: 'Tree', isSelected: true });
                row += 1;
            }

            var visSelect = Csw.composites.makeViewVisibilitySelect(table, row, 'Available to', {
                onRenderFinish: function () {
                    row += 1;
                    var saveBtn = form.button({
                        name: o.name + '_submit',
                        enabledText: 'Create View',
                        disabledText: 'Creating View',
                        onClick: function () {
                            if (form.$.valid()) {
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

                                Csw.ajax.deprecatedWsNbt({
                                    urlMethod: 'createView',
                                    data: createData,
                                    success: function (data) {
                                        div.$.dialog('close');
                                        Csw.tryExec(o.onAddView, data.newviewid, createData.ViewMode);
                                    },
                                    error: saveBtn.enable
                                });
                            } else {
                                saveBtn.enable();
                            }
                        }
                    });

                    /* Cancel Button */
                    form.button({
                        name: o.name + '_cancel',
                        enabledText: 'Cancel',
                        disabledText: 'Canceling',
                        onClick: function () {
                            div.$.dialog('close');
                        }
                    });
                }
            });

            openDialog(div, 425, 210, null, 'New View');

        }, // AddViewDialog
        AddFeedbackDialog: function (options) {
            'use strict';
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
                    cswPublic.tabsAndProps.refresh(null, null);
                    cswPublic.tabsAndProps.tearDown();
                },
                title: 'New ' + cswDlgPrivate.text
            };

            cswDlgPrivate.onOpen = function () {

                var state = Csw.clientState.getCurrent();
                Csw.ajax.deprecatedWsNbt({
                    urlMethod: "getFeedbackNode",
                    data: {
                        nodetypeid: cswDlgPrivate.nodetypeid,
                        actionname: state.actionname,
                        viewid: state.viewid,
                        viewmode: state.viewmode,
                        selectednodeid: Csw.cookie.get('csw_currentnodeid'),
                        author: Csw.cookie.get(Csw.cookie.cookieNames.Username)
                    },
                    success: function (data) {
                        cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(cswPublic.div, {
                            tabState: {
                                propertyData: data.propdata,
                                ShowAsReport: false,
                                nodeid: data.nodeid,
                                nodetypeid: cswDlgPrivate.nodetypeid,
                                EditMode: Csw.enums.editMode.Add,
                                relatednodeid: data.nodeid
                            },
                            ReloadTabOnSave: false,
                            onSave: function (nodeid, nodekey, tabcount, nodename) {
                                Csw.ajax.deprecatedWsNbt({
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
        AddNodeTypeDialog: function (options) {
            'use strict';
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
                    Csw.ajax.deprecatedWsNbt({
                        urlMethod: 'IsNodeTypeNameUnique',
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
                        error: function () { addBtn.enable(); }
                    });
                }
            });
            openDialog(div, 400, 200, null, o.title);
        }, // AddNodeTypeDialog
        EditLayoutDialog: function (options) {
            'use strict';
            var cswDlgPrivate = {
                name: 'editlayout',
                tabState: {
                    nodeid: '',
                    nodekey: '',
                    nodetypeid: '',
                    tabid: '',
                    tabNo: 0,
                    EditMode: 'Edit'
                },
                Refresh: null
            };
            Csw.extend(cswDlgPrivate, options);

            var div = Csw.literals.div();

            cswDlgPrivate.onOpen = function () {
                cswDlgPrivate.tabState.ShowAsReport = false;
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
                            Csw.ajax.deprecatedWsNbt({
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
                        NodeId: Csw.string(cswDlgPrivate.tabState.nodeid),
                        NodeKey: Csw.string(cswDlgPrivate.tabState.nodekey),
                        NodeTypeId: Csw.string(cswDlgPrivate.tabState.nodetypeid),
                        TabId: Csw.string(cswDlgPrivate.tabState.tabid),
                        LayoutType: layoutSelect.val()
                    };
                    Csw.ajax.deprecatedWsNbt({
                        urlMethod: 'getPropertiesForLayoutAdd',
                        data: ajaxdata,
                        success: function (data) {
                            var propOpts = [{ value: '', display: 'Select...' }];
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
        DeleteNodeDialog: function (options) {
            'use strict';
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
            if (false === Csw.isNullOrEmpty(cswDlgPrivate.nodes)) {
                var n = 0;
                Csw.iterate(cswDlgPrivate.nodes, function (nodeObj) {
                    cswDlgPrivate.nodeids[n] = nodeObj.nodeid;
                    cswDlgPrivate.cswnbtnodekeys[n] = nodeObj.nodekey;
                    cswPublic.div.span({ text: nodeObj.nodename }).css({ 'padding-left': '10px' }).br();
                    n += 1;
                });
            } else {
                cswPublic.div.span({ text: cswDlgPrivate.nodenames[0] }).css({ 'padding-left': '10px' }).br();
            }

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

        ChangePasswordDialog: function (options) {
            'use strict';
            var allowClose = false;
            if (false === ExistingChangePasswordDialog) {
                ExistingChangePasswordDialog = true;
                var cswDlgPrivate = {
                    UserId: '',
                    UserKey: '',
                    PasswordId: '',
                    title: 'Your password has expired.  Please change it now:',
                    onSuccess: function () {
                    }
                };
                Csw.extend(cswDlgPrivate, options);

                var doRefresh = true;
                var cswPublic = {
                    closed: false,
                    div: Csw.literals.div({ ID: window.Ext.id() }), //Case 28799 - we have to differentiate dialog div Ids from each other
                    close: function () {
                        if (false === cswPublic.closed && doRefresh) {
                            cswPublic.closed = true;
                            cswPublic.tabsAndProps.tearDown();
                            Csw.tryExec(cswDlgPrivate.onClose);
                            ExistingChangePasswordDialog = false;
                        }
                    }
                };

                cswPublic.title = Csw.string(cswDlgPrivate.title);

                cswDlgPrivate.onOpen = function () {
                    var table = cswPublic.div.table({ width: '100%' });
                    var tabCell = table.cell(1, 2);

                    cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(tabCell, {
                        forceReadOnly: cswDlgPrivate.ReadOnly,
                        Multi: cswDlgPrivate.Multi,
                        tabState: {
                            filterToPropId: cswDlgPrivate.PasswordId,
                            nodeid: cswDlgPrivate.UserId,
                            nodekey: cswDlgPrivate.UserKey,
                            isChangePasswordDialog: true,     // kludgetastic!  case 29841
                            EditMode: Csw.enums.editMode.Edit
                        },
                        onSave: function (nodeids, nodekeys, tabcount) {
                            Csw.clientChanges.unsetChanged();
                            if (false === cswPublic.closed) {
                                allowClose = true;
                                cswPublic.close();
                                cswPublic.div.$.dialog('close');
                            }
                            Csw.tryExec(cswDlgPrivate.onSuccess, nodeids, nodekeys, cswPublic.close);
                        },
                        onBeforeTabSelect: function () {
                            return Csw.clientChanges.manuallyCheckChanges();
                        },
                        onPropertyChange: function () {
                            Csw.clientChanges.setChanged();
                        }
                    }); // tabsandprops
                }; // onOpen()

                openDialog(cswPublic.div, 900, 600, cswPublic.close, cswPublic.title, cswDlgPrivate.onOpen, function () { return allowClose; });
            } // if (false === ExistingChangePasswordDialog)
            return cswPublic;
        }, // ChangePasswordDialog


        LogoutExistingSessionsDialog: function (onSuccess) {
            'use strict';

            if (false === ExistingMultisessionDialog) {
                ExistingMultisessionDialog = true;
                var logoutOnClose = true;

                var onClose = function () {
                    if (logoutOnClose) {
                        Csw.clientSession.logout();
                    }
                };

                var div = Csw.literals.div().css({});
                var resetLoginTable = div.table({ width: '80%', align: 'center' });
                resetLoginTable.cell(1, 1).propDom('colspan', 2).br({ number: 4 });
                resetLoginTable.cell(1, 1).append('You are already logged in at another location. Would you like to end your previous session and log in from this computer?');
                resetLoginTable.cell(1, 1).br({ number: 4 });

                resetLoginTable.cell(2, 1).css('text-align', 'center').button({
                    enabledText: 'Yes, Logout Other Sessions',
                    onClick: function () {
                        Csw.ajaxWcf.post({
                            urlMethod: 'Session/endCurrentUserSessions',
                            complete: function (data) {
                                logoutOnClose = false;
                                div.$.dialog('close');
                                Csw.tryExec(onSuccess);
                                ExistingMultisessionDialog = false;
                            }//success
                        }); //Csw.ajaxWcf.post
                    }//onClick
                }); //resetLoginTable.cell(2, 2).button

                resetLoginTable.cell(2, 2).css('text-align', 'center').button({
                    enabledText: 'No, Log Me Out',
                    onClick: function () {
                        div.$.dialog('close');
                    }
                });


                openDialog(div, 600, 400, onClose, 'Logout Existing Sessions');
            }
        }, //LogoutExistingSessionsDialog

        AboutDialog: function () {
            'use strict';
            var div = Csw.literals.div();
            Csw.ajax.deprecatedWsNbt({
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

                    // Chemcat Datasource dates (only displayed if c3 is enabled)
                    if (data.dsDates) {
                        table.cell(row, 1).css({ padding: '2px 5px 2px 5px' }).append('ChemCatCentral Data Sources');
                        row += 1;
                        table.cell(row, 1).css({ padding: '2px 5px 2px 5px' }).append('---------------------------');
                        row += 1;

                        for (var ds in data.dsDates) {
                            var thisDS = data.dsDates[ds];
                            table.cell(row, 1).css({ padding: '2px 5px 2px 5px' }).append(thisDS.componentName);
                            table.cell(row, 2).css({ padding: '2px 5px 2px 5px' }).append(thisDS.value);
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
        StructureSearchDialog: function (options) {
            'use strict';
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

            table.cell(3, 1).span().setLabelText('MOL Text (Paste from Clipboard):', false, false);
            var molText = table.cell(4, 1).textArea({
                name: '',
                rows: 12,
                cols: 50,
                onChange: function () {
                    Csw.getMolImgFromText('', molText.val(), displayMolThumbnail);
                }
            });

            var displayMolThumbnail = function (data) {
                table.cell(4, 2).empty();
                if (data.molImgAsBase64String) {
                    molText.val(data.molString);
                    table.cell(4, 2).img({
                        labelText: "Query Image",
                        src: "data:image/jpeg;base64," + data.molImgAsBase64String
                    });
                }
            };

            var currentNodeId = Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId);
            Csw.getMolImgFromText(currentNodeId, '', displayMolThumbnail);


            var fileTbl = table.cell(2, 1).table({ cellpadding: '2px', align: 'left' });
            cswPrivate.cell11 = fileTbl.cell(1, 1).div().setLabelText('Selected MOL File: ', false, false);
            cswPrivate.cell12 = fileTbl.cell(1, 2).div().text('(No File Chosen)');
            cswPrivate.cell13 = fileTbl.cell(1, 3).div().icon({
                name: 'uploadmolSS',
                iconType: Csw.enums.iconType.pencil,
                hovertext: 'Upload a Mol file',
                size: 16,
                isButton: true,
                onClick: function () {
                    $.CswDialog('FileUploadDialog', {
                        url: 'Services/BlobData/getText',
                        forceIFrameTransport: true,
                        dataType: 'iframe',
                        onSuccess: function (data) {

                            var fileName = Csw.getPropFromIFrame(data, 'filename');
                            var fileText = Csw.getPropFromIFrame(data, 'filetext');

                            cswPrivate.cell12.text(fileName);
                            molText.val(fileText);
                            Csw.getMolImgFromText('', molText.val(), displayMolThumbnail);

                        }
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
            'use strict';
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
                molTxtArea, saveBtn, table, fileTbl;

            table = div.table();
            fileTbl = table.cell(2, 1).table({ cellpadding: '2px', align: 'left' });
            cswPrivate.cell11 = fileTbl.cell(1, 1).div().setLabelText('MOL File: ', false, false);
            cswPrivate.cell12 = fileTbl.cell(1, 2).div().text('(No File Chosen)');
            cswPrivate.cell13 = fileTbl.cell(1, 3).div().icon({
                name: 'uploadmolEditMol',
                iconType: Csw.enums.iconType.pencil,
                hovertext: 'Upload a Mol file',
                size: 16,
                isButton: true,
                onClick: function () {
                    $.CswDialog('FileUploadDialog', {
                        url: 'Services/BlobData/getText',
                        forceIframeTransport: true, //because IE9 doesn't work
                        dataType: 'iframe', //response will be in an iframe obj
                        onSuccess: function (data) {
                            var fileText = Csw.getPropFromIFrame(data, 'filetext');
                            var fileName = Csw.getPropFromIFrame(data, 'filename', true);

                            molTxtArea.val(fileText);
                            cswPrivate.cell12.text(fileName);
                        }
                    });
                }
            });

            div.br({ number: 1 });

            div.span({ text: 'MOL Text (Paste from Clipboard):' }).br();

            molTxtArea = div.textArea({
                name: '',
                rows: 15,
                cols: 50
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

            openDialog(div, 525, 450, null, 'Change MOL Data');
        }, // FileUploadDialog
        ShowLicenseDialog: function (options) {
            'use strict';
            if (false === ExistingShowLicenseDialog) {
                ExistingShowLicenseDialog = true;
                var allowClose = false;
                var o = {
                    GetLicenseUrl: 'getLicense',
                    AcceptLicenseUrl: 'acceptLicense',
                    onAccept: function () {
                    },
                    onDecline: function () {
                    }
                };
                if (options) {
                    Csw.extend(o, options);
                }

                var div = Csw.literals.div({ align: 'center' });
                div.append('Service Level Agreement').br();
                var licenseTextArea = div.textArea({ name: 'license', rows: 30, cols: 80 }).propDom({ disabled: true });
                div.br();

                Csw.ajax.deprecatedWsNbt({
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
                        Csw.ajax.deprecatedWsNbt({
                            urlMethod: o.AcceptLicenseUrl,
                            success: function () {
                                allowClose = true;
                                div.$.dialog('close');
                                Csw.tryExec(o.onAccept);
                            },
                            error: acceptBtn.enable
                        }); // ajax
                    } // onClick
                }); // 
                var declineBtn = div.button({
                    name: 'license_decline',
                    enabledText: 'I Decline',
                    disabledText: 'Declining...',
                    onClick: function () {
                        Csw.tryExec(o.onDecline);
                    } // onClick
                }); // 
                openDialog(div, 800, 600, function () { ExistingShowLicenseDialog = false; }, 'Terms and Conditions', null, function () { return allowClose; });
            }
        }, // ShowLicenseDialog
        PrintLabelDialog: function (options) {
            'use strict';
            ///<summary>Creates an Print Label dialog and returns an object represent that dialog.</summary>
            var cswDlgPrivate = {
                name: 'print_label',
                nodes: [],
                nodeids: [],
                nodetypeid: '',
                selectedLabel: '',
            };
            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Print Label Dialog without options.', '', 'CswDialog.js', 893));
            }
            Csw.extend(cswDlgPrivate, options);
            var cswPublic = Csw.object();

            if (!cswDlgPrivate.nodes || Object.keys(cswDlgPrivate.nodes).length < 1) {
                Csw.dialogs.alert({
                    title: 'Empty selection',
                    message: 'Nothing has been selected to print. <br>Go back and select an item to print.'
                }).open();
            } else {

                cswPublic = {
                    div: Csw.literals.div(),
                    close: function () {
                        cswPublic.div.$.dialog('close');
                    }
                };

                Csw.composites.printLabels(cswPublic.div, {
                    nodeids: cswDlgPrivate.nodeids,
                    nodes: cswDlgPrivate.nodes,
                    selectedLabel: cswDlgPrivate.selectedLabel
                }, Csw.number(cswDlgPrivate.nodetypeid, 0), cswDlgPrivate.nodeids[0]);

                openDialog(cswPublic.div, 400, 300, null, 'Print');
            }
            return cswPublic;

        }, // PrintLabelDialog

        ImpersonateDialog: function (options) {
            'use strict';
            var o = {
                onImpersonate: null
            };
            if (options) Csw.extend(o, options);

            var div = Csw.literals.div().empty(),
                form = div.form(),
                table = form.table();

            function onOpen(tbl) {
                Csw.ajaxWcf.post({
                    urlMethod: 'Menus/initImpersonate',
                    success: function (data) {
                        var viewid = data.ImpersonateViewId;

                        // Case 31086 - Use NodeSelect instead of Select
                        var usersel = tbl.cell(1, 1).nodeSelect({
                            name: 'ImperonsateSelect',
                            objectClassName: 'UserClass',
                            allowAdd: false,
                            isRequired: true,
                            showSelectOnLoad: true,
                            isMulti: false,
                            selectedNodeId: '',
                            viewid: viewid,
                            excludeNodeIds: data.ExcludeNodeIds,
                            onSelectNode: function (nodeObject) {
                                if (nodeObject) {
                                    var name = nodeObject.name || nodeObject.nodename;
                                    var nodeid = nodeObject.nodeid;
                                    if (false === Csw.isNullOrEmpty(name) || false === Csw.isNullOrEmpty(nodeid)) {
                                        impersonateBtn.enable();
                                    } else {
                                        impersonateBtn.disable();
                                    }
                                }//if (nodeObject)
                            }//onSelectNode
                        });

                        var impersonateBtn = tbl.cell(1, 2).button({
                            name: 'ImpersonateButton',
                            enabledText: 'Impersonate',
                            isEnabled: false,
                            onClick: function () {
                                var val = usersel.val() || usersel.selectedNodeId();
                                var text = getUserSelText(usersel);
                                Csw.tryExec(o.onImpersonate, val, text);
                                div.$.dialog('close');
                            }
                        });

                        tbl.cell(1, 3).button({
                            name: 'CancelButton',
                            enabledText: 'Cancel',
                            onClick: function () {
                                div.$.dialog('close');
                            }
                        });

                    } // success
                }); // ajax
            }//onOpen()

            function getUserSelText(sel) {
                var text = '';
                if (sel.selectedText) {
                    text = sel.selectedText();
                } else if (sel.selectedName) {
                    text = sel.selectedName();
                }
                return text;
            }

            openDialog(div, 450, 300, null, 'Impersonate', onOpen(table));
        }, // ImpersonateDialog

        SearchDialog: function (options) {
            'use strict';
            var cswDlgPrivate = {
                name: 'searchdialog',
                propname: '',
                title: '',
                nodetypeid: '',
                objectclassid: '',
                propertysetid: '',
                onSelectNode: null,
                onClose: function () { }
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

            openDialog(cswPublic.div, 800, 600, null, 'Search ' + cswDlgPrivate.propname, cswDlgPrivate.onClose);

            cswPublic.search = Csw.composites.universalSearch(cswPublic.div, {
                name: cswDlgPrivate.name,
                nodetypeid: cswDlgPrivate.nodetypeid,
                objectclassid: cswDlgPrivate.objectclassid,
                propertysetid: cswDlgPrivate.propertysetid,
                onBeforeSearch: function () { },
                onAfterSearch: function () { },
                onAfterNewSearch: function (searchid) { },
                onAddView: function (viewid, viewmode) { },
                onLoadView: function (viewid, viewmode) { },
                showSave: false,
                allowEdit: false,
                allowDelete: false,
                allowNodeTypeChange: false,
                compactResults: true,
                extraAction: 'Select',
                extraActionIcon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.check),
                universalSearchOnly: true, //No C3 or Structure Search here
                showC3SrchPromptText: false, // Don't prompt users to search C3
                onExtraAction: function (nodeObj) {
                    cswPublic.close();
                    Csw.tryExec(cswDlgPrivate.onSelectNode, nodeObj);
                },
                excludeNodeIds: cswDlgPrivate.excludeNodeIds,
                includeInRecent: false,
            });
            return cswPublic;
        }, // SearchDialog

        SaveSearchDialog: function (options) {
            'use strict';
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
            'use strict';
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
                    if (Csw.tryExec(o.onOk) !== false)
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
            'use strict';
            var o = {
                opname: 'operation',
                onClose: null,
                onViewBatchOperation: null
            };
            if (options) Csw.extend(o, options);

            var div = Csw.literals.div({ name: 'searchdialog_div' });

            div.append('This ' + o.opname + ' will be performed as a batch operation');

            div.nodeLink({ text: o.batch, onClick: function () { div.$.dialog('close'); } });

            div.button({
                enabledText: 'Close',
                onClick: function () {
                    div.$.dialog('close');
                }
            });

            openDialog(div, 400, 300, o.onClose, 'Batch Operation');
        }, // BatchOpDialog


        RelatedToDemoNodesDialog: function (options) {
            'use strict';
            var cswPrivate = {
                title: "Related Data",
                relatedNodesGridRequest: options.relatedNodesGridRequest,
                relatedNodeName: options.relatedNodeName || ' Current Node',
                onCloseDialog: options.onCloseDialog || null
                //searchresults: null
            };

            if (Csw.isNullOrEmpty(options)) {
                Csw.error.throwException(Csw.error.exception('Cannot create an Delete Dialog without options.', '', 'CswDialog.js', 641));
            }
            Csw.extend(cswPrivate, options);

            var div = Csw.literals.div(),
               newNode;

            var getRelatedNodesGrid = function () {

                var mainGrid;
                var gridId = 'relatedDemoDataNodesGrid';

                function post() {

                    Csw.ajaxWcf.post({
                        urlMethod: 'DemoData/getDemoDataNodesAsGrid',
                        data: cswPrivate.relatedNodesGridRequest,
                        success: function (result) {

                            //see case 29437: Massage row structure
                            result.Grid.data.items.forEach(function (element, index, array) {
                                Csw.extend(element, element.Row);
                            }); //foreach on grid rows                            

                            if (mainGrid) {
                                mainGrid.empty();
                            }
                            mainGrid = div.grid({
                                name: gridId,
                                storeId: gridId,
                                data: result.Grid,
                                stateId: gridId,
                                height: 375,
                                width: '950px',
                                forceFit: true,
                                title: 'Data Related To ' + cswPrivate.relatedNodeName,
                                usePaging: false,
                                showActionColumn: true,
                                onEdit: function (rows) {
                                    // this works for both Multi-edit and regular
                                    var nodekeys = Csw.delimitedString(),
                                        nodeids = Csw.delimitedString(),
                                        nodenames = [],
                                        firstNodeId, firstNodeKey;

                                    Csw.iterate(rows, function (row) {
                                        firstNodeId = firstNodeId || row.nodeid;
                                        firstNodeKey = firstNodeKey || row.nodekey;
                                        nodekeys.add(row.nodekey);
                                        nodeids.add(row.nodeid);
                                        nodenames.push(row.nodename);
                                    });

                                    Csw.dialogs.editnode({
                                        currentNodeId: firstNodeId,
                                        currentNodeKey: firstNodeKey,
                                        selectedNodeIds: nodeids,
                                        selectedNodeKeys: nodekeys,
                                        nodenames: nodenames,
                                        Multi: false,
                                        ReadOnly: true
                                    });
                                }, // onEdit
                                onDelete: function (rows) {
                                    // this works for both Multi-edit and regular
                                    var node_data = Csw.deserialize(rows[0].menuoptions);
                                    var nodes = [];
                                    nodes.push(node_data);


                                    $.CswDialog('DeleteNodeDialog', {
                                        nodes: nodes,
                                        Multi: (nodes.length > 1),
                                        publishDeleteEvent: false,
                                        onDeleteNode: function () {
                                            post();
                                        }//onDeleteNode() 
                                    });
                                }, // onDelete
                                onPreview: function (o, nodeObj, event) {
                                    var preview = Csw.nbt.nodePreview(Csw.main.body, {
                                        nodeid: nodeObj.nodeid,
                                        nodekey: nodeObj.nodekey,
                                        nodename: nodeObj.nodename,
                                        event: event
                                    });
                                    preview.open();
                                },
                                canSelectRow: false,
                                selModel: {
                                    selType: 'cellmodel'
                                }
                            }); //grid()
                        }//success() 
                    }); //post to get grid
                }//function wrapper of poset

                post();
            }; //getRelatedNodesGrid()

            var onOpen = function () {
                getRelatedNodesGrid();
            };

            openDialog(div, 1000, 500, cswPrivate.onCloseDialog, cswPrivate.title, onOpen);

        }, // RelatedToDemoNodesDialog
        NavigationSelectDialog: function (options) {
            'use strict';
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

        OpenDialog: function (id, url) {
            'use strict';
            var div = Csw.literals.div({ name: id });
            div.$.load(url,
                function () {
                    openDialog(div, 600, 400);
                });
        },
        OpenEmptyDialog: function (options) {
            'use strict';
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
            'use strict';
            posX -= incrPosBy;
            posY -= incrPosBy;
            Csw.dialogsCount(-1);
            $('#' + id)
                .dialog('close')
                .remove();
        }

        //#region Generic
    };


    function openDialog(div, width, height, onClose, title, onOpen, beforeClose) {
        'use strict';
        $('<div id="DialogErrorDiv" style="display: none;"></div>')
            .prependTo(div.$);

        Csw.tryExec(div.$.dialog, 'close');
        if (Csw.dialogsCount() === 0) { //as per discussion - dialogs should be centered
            posX = (cswPrivate.windowWidth() / 2) - (width / 2) + posX;
            posY = (cswPrivate.windowHeight() / 2) - (height / 2) + posY;
        }

        Csw.subscribe(Csw.enums.events.main.clear, function _close() {
            Csw.tryExec(div.remove);
            Csw.tryExec(onClose);
            unbindEvents();
            Csw.unsubscribe(Csw.enums.events.main.clear, _close);
        });

        div.$.dialog({
            modal: true,
            width: width,
            height: height,
            title: title,
            position: [posX, posY],
            beforeClose: function () {
                var ret = Csw.clientChanges.manuallyCheckChanges();
                if (Csw.isFunction(beforeClose)) {
                    ret = ret && beforeClose();
                }
                return ret;
            },
            close: function () {
                posX -= incrPosBy;
                posY -= incrPosBy;
                Csw.dialogsCount(-1);
                Csw.tryExec(onClose);

                unbindEvents();
                if (Csw.dialogsCount() === 0) {
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
                Csw.dialogsCount(1);
                Csw.tryExec(onOpen, div);
                div.$.parent().find(' :button').blur();
            }
        });
        posX += incrPosBy;
        posY += incrPosBy;

        var doClose = function (func) {
            if (!func || true === func()) {
                Csw.tryExec(onClose);
                div.$.dialog('close');
                unbindEvents();
            }
        };

        var unbindEvents = function () {
            Csw.publish('onAnyNodeButtonClickFinish', true);
            Csw.unsubscribe(Csw.enums.events.afterObjectClassButtonClick, doClose);
            Csw.unsubscribe('initGlobalEventTeardown', doClose);
        };
        Csw.subscribe(Csw.enums.events.afterObjectClassButtonClick, doClose);
        Csw.subscribe('initGlobalEventTeardown', doClose);
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
