/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswDialog';

    var methods = {

        //#region Specialized

        ExpireDialog: function (options) {
            var o = {
                onYes: function () { }
            };

            if (options) $.extend(o, options);

            var $div = $('<div></div>');

            $div.append('<p>Your session is about to time out.  Would you like to continue working?</p>');

            $div.CswButton({
                ID: 'renew_btn',
                enabledText: 'Yes',
                onClick: function () { $div.dialog('close'); o.onYes(); }
            });

            openDialog($div, 300, 250, null, 'Expire Warning');

        }, // ExpireDialog
        AddWelcomeItemDialog: function (options) {
            var o = {
                onAdd: function () { }
            };

            if (options) $.extend(o, options);

            var $div = $('<div></div>');

            $div.CswWelcome('getAddItemForm', { 'onAdd': function () {
                $div.dialog('close');
                if (Csw.isFunction(o.onAdd)) {
                    o.onAdd();
                }
            }
            });

            openDialog($div, 400, 400, null, 'New Welcome Item');

        }, // AddWelcomeItemDialog
        AddViewDialog: function (options) {
            var o = {
                ID: 'addviewdialog',
                onAddView: function () { },
                viewid: '',
                viewmode: ''
            };
            if (options) $.extend(o, options);

            var $div = $('<div></div>');
            var table = Csw.controls.table({
                $parent: $div,
                ID: Csw.controls.dom.makeId(o.ID, 'tbl'),
                FirstCellRightAlign: true
            });

            table.cell(1, 1).text('Name:');

            var nameTextCell = table.cell(1, 2);
            var $nametextbox = nameTextCell.$.CswInput('init', { ID: o.ID + '_nametb',
                type: Csw.enums.inputTypes.text,
                cssclass: 'textinput'
            });
            var $displaymodeselect = $('<select id="' + o.ID + '_dmsel" />');
            if (Csw.isNullOrEmpty(o.viewid)) {
                table.cell(2, 1).text('Display Mode:');
                $displaymodeselect.append('<option value="Grid">Grid</option>');
                $displaymodeselect.append('<option value="List" selected>List</option>');
                $displaymodeselect.append('<option value="Table">Table</option>');
                $displaymodeselect.append('<option value="Tree">Tree</option>');
                table.cell(2, 2).text($displaymodeselect);
            }

            var visSelect = Csw.controls.makeViewVisibilitySelect(table, 3, 'Available to:');
            var $savebtn = $div.CswButton({
                ID: o.ID + '_submit',
                enabledText: 'Create View',
                disabledText: 'Creating View',
                onClick: function () {

                    var createData = {};
                    createData.ViewName = $nametextbox.val();
                    createData.ViewId = o.viewid;
                    if (Csw.isNullOrEmpty(o.viewmode)) {
                        createData.ViewMode = $displaymodeselect.val();
                    } else {
                        createData.ViewMode = o.viewmode;
                    }
                    if (!Csw.isNullOrEmpty(visSelect.$visibilityselect)) {
                        createData.Visibility = visSelect.$visibilityselect.val();
                        createData.VisibilityRoleId = visSelect.$visroleselect.val();
                        createData.VisibilityUserId = visSelect.$visuserselect.val();
                    } else {
                        createData.Visibility = "";
                        createData.VisibilityRoleId = "";
                        createData.VisibilityUserId = "";
                    }

                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/createView',
                        data: createData,
                        success: function (data) {
                            $div.dialog('close');
                            o.onAddView(data.newviewid);
                        },
                        error: function () {
                            $savebtn.CswButton('enable');
                        }
                    });
                }
            });
            /* Cancel Button */
            $div.CswButton({
                ID: o.ID + '_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    $div.dialog('close');
                }
            });

            openDialog($div, 400, 200, null, 'New View');
        }, // AddViewDialog
        AddNodeDialog: function (options) {
            var o = {
                text: '',
                nodetypeid: '',
                relatednodeid: '',
                onAddNode: function () { }
            };

            if (options) {
                $.extend(o, options);
            }

            var $div = $('<div></div>'),
                title = 'New ' + o.text;
            $div.CswNodeTabs({
                nodetypeid: o.nodetypeid,
                relatednodeid: o.relatednodeid,
                relatednodetypeid: o.relatednodetypeid,
                EditMode: Csw.enums.editMode.Add,
                onSave: function (nodeid, cswnbtnodekey) {
                    $div.dialog('close');
                    o.onAddNode(nodeid, cswnbtnodekey);
                },
                onInitFinish: function () {
                    openDialog($div, 800, 600, null, title);
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

            var $div = $('<div></div>'),
                $newNode;

            $div.append('New ' + o.nodetypename + ': ');
            $newNode = $div.CswInput('init', { ID: o.ID + '_newNode', type: Csw.enums.inputTypes.text });

            $div.CswButton({
                ID: o.objectClassId + '_add',
                enabledText: 'Add',
                onClick: function () {
                    if (Csw.isFunction(o.onSuccess)) {
                        o.onSuccess($newNode.val());
                    }
                    $div.dialog('close');
                }
            });
            openDialog($div, 300, 200, null, o.title);
        }, // AddNodeClientSideDialog
        AddNodeTypeDialog: function (options) {
            var o = {
                objectClassId: '',
                nodetypename: '',
                maxlength: 50, //DB nodetypename = varchar(50)
                category: '',
                $select: '',
                nodeTypeDescriptor: '',
                onSuccess: null,
                title: ''
            };

            if (options) {
                $.extend(o, options);
            }

            var $div = $('<div></div>'),
                $nodeType, $category, $addBtn,
                category = Csw.string(o.category);

            $div.append('New ' + o.nodeTypeDescriptor + ': ');
            $nodeType = $div.CswInput('init', { ID: o.objectClassId + '_nodeType', type: Csw.enums.inputTypes.text, value: o.nodetypename, maxlength: o.maxlength });
            $div.append('<br />');
            if (Csw.isNullOrEmpty(category)) {
                $div.append('Category Name: ');
                $category = $div.CswInput('init', { ID: o.objectClassId + '_category', type: Csw.enums.inputTypes.text });
                $div.append('<br />');
            }
            $addBtn = $div.CswButton({
                ID: o.objectClassId + '_add',
                enabledText: 'Add',
                onClick: function () {
                    var newNodeTypeName = $nodeType.val();
                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/IsNodeTypeNameUnique',
                        async: false,
                        data: { 'NodeTypeName': newNodeTypeName },
                        success: function () {
                            o.$select.append('<option data-newNodeType="true" value="' + $nodeType.val() + '">' + $nodeType.val() + '</option>');
                            o.$select.val($nodeType.val());
                            if (Csw.isNullOrEmpty(category) && false === Csw.isNullOrEmpty($category)) {
                                category = $category.val();
                            }
                            $div.dialog('close');
                            if (Csw.isFunction(o.onSuccess)) {
                                o.onSuccess({
                                    nodetypename: newNodeTypeName,
                                    category: category
                                });
                            }
                        },
                        error: function () {
                            $addBtn.CswButton('enable');
                        }
                    });
                }
            });
            openDialog($div, 400, 200, null, o.title);
        }, // AddNodeTypeDialog
        EditLayoutDialog: function (cswNodeTabOptions) {
            cswNodeTabOptions.ID = cswNodeTabOptions.ID + '_editlayout';
            cswNodeTabOptions.Config = true;
            cswNodeTabOptions.ShowAsReport = false;
            cswNodeTabOptions.onTabSelect = function (tabid) {
                cswNodeTabOptions.tabid = tabid;
                _configAddOptions();
            };
            cswNodeTabOptions.onPropertyRemove = function () {
                _configAddOptions();
            };

            var $div = $('<div></div>');
            var table = Csw.controls.table({
                $parent: $div,
                ID: 'EditLayoutDialog_table',
                width: '100%'
            });

            var cell11 = table.cell(1, 1).text('Configure:<br/>');
            var cell12 = table.cell(1, 2);

            var $layoutSelect = cell11.$.CswSelect('init', {
                ID: 'EditLayoutDialog_layoutselect',
                selected: 'Edit',
                values: [{ value: 'Add', display: 'Add' },
                         { value: 'Edit', display: 'Edit' },
                         { value: 'Preview', display: 'Preview' },
                         { value: 'Table', display: 'Table' }
                        ],
                onChange: function () {
                    cswNodeTabOptions.EditMode = $('#EditLayoutDialog_layoutselect option:selected').val();
                    _resetLayout();
                }
            });

            cell11.append('<br/><br/>Add:<br/>');
            var $addSelect = cell11.$.CswSelect('init', {
                ID: 'EditLayoutDialog_addselect',
                selected: '',
                values: [],
                onChange: function () {

                    var ajaxdata = {
                        PropId: Csw.string($addSelect.val()),
                        TabId: Csw.string(cswNodeTabOptions.tabid),
                        LayoutType: $layoutSelect.val()
                    };
                    Csw.ajax.post({
                        url: '/NbtWebApp/wsNBT.asmx/addPropertyToLayout',
                        data: ajaxdata,
                        success: function () {
                            _resetLayout();
                        }
                    }); // Csw.ajax
                } // onChange
            }); // CswSelect

            function _resetLayout() {
                cell12.empty();
                cell12.$.CswNodeTabs(cswNodeTabOptions);
                _configAddOptions();
            }

            function _configAddOptions() {
                var ajaxdata = {
                    NodeId: Csw.string(cswNodeTabOptions.nodeids[0]),
                    NodeKey: Csw.string(cswNodeTabOptions.nodekeys[0]),
                    NodeTypeId: Csw.string(cswNodeTabOptions.nodetypeid),
                    TabId: Csw.string(cswNodeTabOptions.tabid),
                    LayoutType: $layoutSelect.val()
                };
                Csw.ajax.post({
                    url: '/NbtWebApp/wsNBT.asmx/getPropertiesForLayoutAdd',
                    data: ajaxdata,
                    success: function (data) {
                        var propOpts = [{ value: '', display: 'Select...'}];
                        for (var p in data) {
                            if (data.hasOwnProperty(p)) {
                                propOpts.push({
                                    value: data[p].propid,
                                    display: data[p].propname
                                });
                            }
                        }
                        $addSelect.CswSelect('setoptions', propOpts, '', true);
                    } // success
                });  // Csw.ajax
            } // _configAddOptions()

            function _onclose() {
                Csw.tryExec(cswNodeTabOptions.Refresh);
            }

            _resetLayout();

            openDialog($div, 900, 600, _onclose, 'Edit Layout');
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
                date: ''     // viewing audit records
            };
            if (options) $.extend(o, options);

            var $div = $('<div></div>');

            var myEditMode = Csw.enums.editMode.EditInPopup;
            var table = Csw.controls.table({
                $parent: $div
            });
            if (false === Csw.isNullOrEmpty(o.date) && false === o.Multi) {
                myEditMode = Csw.enums.editMode.AuditHistoryInPopup;
                table.cell(1, 1).$.CswAuditHistoryGrid({
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
                tabCell.$.CswNodeTabs({
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
                    onEditView: function (viewid) {
                        if (Csw.isFunction(o.onEditView)) {
                            $div.dialog('close');
                            o.onEditView(viewid);
                        }
                    },
                    onSave: function (nodeids, nodekeys, tabcount) {
                        Csw.clientChanges.unsetChanged();
                        if (tabcount === 1 || o.Multi) {
                            $div.dialog('close');
                        }
                        setupTabs(date);
                        if (Csw.isFunction(o.onEditNode)) {
                            o.onEditNode(nodeids, nodekeys);
                        }
                    },
                    onBeforeTabSelect: function () {
                        return Csw.clientChanges.manuallyCheckChanges();
                    },
                    onTabSelect: function (tabid) {
                        Csw.cookie.set(Csw.cookie.cookieNames.CurrentTabId, tabid);
                    },
                    onPropertyChange: function () {
                        Csw.clientChanges.setChanged();
                    }
                });
            } // _setupTabs()
            var title = Csw.string(o.title);
            if (Csw.isNullOrEmpty(title)) {
                title = (false === o.Multi) ? o.nodenames[0] : o.nodenames.join(', ');
            }
            openDialog($div, 900, 600, null, title);
        }, // EditNodeDialog
        CopyNodeDialog: function (options) {
            var o = {
                'nodename': '',
                'nodeid': '',
                'cswnbtnodekey': '',
                'onCopyNode': function () { }
            };

            if (options) {
                $.extend(o, options);
            }

            var $div = $('<div>Copying: ' + o.nodename + '<br/><br/></div>');

            var $copybtn = $div.CswButton({ ID: 'copynode_submit',
                enabledText: 'Copy',
                disabledText: 'Copying',
                onClick: function () {
                    Csw.copyNode({
                        'nodeid': o.nodeid,
                        'nodekey': o.nodekey,
                        'onSuccess': function (nodeid, nodekey) {
                            $div.dialog('close');
                            o.onCopyNode(nodeid, nodekey);
                        },
                        'onError': function () {
                            $copybtn.CswButton('enable');
                        }
                    });
                }
            });
            /* Cancel Button */
            $div.CswButton({ ID: 'copynode_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    $div.dialog('close');
                }
            });

            openDialog($div, 400, 300, null, 'Confirm Copy');
        }, // CopyNodeDialog       
        DeleteNodeDialog: function (options) {
            var o = {
                nodenames: [],
                nodeids: [],
                cswnbtnodekeys: [],
                onDeleteNode: null, //function (nodeid, nodekey) { },
                Multi: false,
                NodeCheckTreeId: '',
                publishDeleteEvent: true
            };

            if (options) {
                $.extend(o, options);
            }

            var $div = $('<div><span>Are you sure you want to delete:&nbsp;</span></div>');

            if (o.Multi) {
                var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
                if (false === Csw.isNullOrEmpty($nodechecks, true) && (o.nodeids.length === 0 || o.cswnbtnodekeys.length === 0)) {
                    var n = 0;
                    $nodechecks.each(function () {
                        var $nodecheck = $(this);
                        o.nodeids[n] = $nodecheck.CswAttrNonDom('nodeid');
                        o.cswnbtnodekeys[n] = $nodecheck.CswAttrNonDom('cswnbtnodekey');
                        $div.append('<br/><span style="padding-left: 10px;">' + $nodecheck.CswAttrNonDom('nodename') + '</span>');
                        n++;
                    });
                } else {
                    for (var i = 0; i < o.nodenames.length; i++) {
                        $div.append('<br/><span style="padding-left: 10px;">' + o.nodenames[i] + '</span>');
                    }
                }
            } else {
                $div.append('<span>' + o.nodenames + '?</span>');
            }
            $div.append('<br/><br/>');

            var $deletebtn = $div.CswButton({ ID: 'deletenode_submit',
                enabledText: 'Delete',
                disabledText: 'Deleting',
                onClick: function () {
                    Csw.deleteNodes({
                        nodeids: o.nodeids,
                        nodekeys: o.cswnbtnodekeys,
                        onSuccess: function (nodeid, nodekey) {
                            $div.dialog('close');
                            if (Csw.isFunction(o.onDeleteNode)) {
                                o.onDeleteNode(nodeid, nodekey);
                            }
                            if (Csw.bool(o.publishDeleteEvent)) {
                                $.publish(Csw.enums.events.CswNodeDelete, { nodeids: o.nodeids, cswnbtnodekeys: o.cswnbtnodekeys });
                            }
                        },
                        onError: function () {
                            $deletebtn.CswButton('enable');
                        }
                    });
                }
            });
            /* Cancel Button */
            $div.CswButton({ ID: 'deletenode_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    $div.dialog('close');
                }
            });
            openDialog($div, 400, 200, null, 'Confirm Delete');
        }, // DeleteNodeDialog
        AboutDialog: function () {
            var $div = $('<div></div>');
            Csw.ajax.post({
                url: '/NbtWebApp/wsNBT.asmx/getAbout',
                data: {},
                success: function (data) {
                    $div.append('NBT Assembly Version: ' + data.assembly + '<br/><br/>');
                    var table = Csw.controls.table({
                        $parent: $div,
                        ID: 'abouttale'
                    });

                    var row = 1;

                    var components = data.components;
                    for (var comp in components) {
                        if (Csw.contains(components, comp)) {
                            var thisComp = components[comp];
                            
                            table.cell(row, 1).text(thisComp.name).css('padding', '2px 5px 2px 5px');
                            table.cell(row, 2).text(thisComp.version).css('padding', '2px 5px 2px 5px');
                            table.cell(row, 3).text(thisComp.copyright).css('padding', '2px 5px 2px 5px');
                            row += 1;
                        }
                    }
                }
            });
            openDialog($div, 600, 400, null, 'About');
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

            var $div = $('<div></div>');

            var $btn = $('<input id="fileupload" type="file" name="fileupload" />')
                            .appendTo($div);

            $btn.fileupload({
                datatype: 'json',
                url: o.url + '?' + $.param(o.params),
                paramName: 'fileupload',
                done: function () {
                    $div.dialog('close');
                    o.onSuccess();
                }
            });

            $div.CswButton({ ID: 'fileupload_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    $div.dialog('close');
                }
            });

            openDialog($div, 400, 300, null, 'Upload');
        },

        'EditMolDialog': function (options) {
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

            var $div = $('<div></div>'),
                $moltxtarea, $txtsave;

            var $btn = $('<input id="fileupload" type="file" name="fileupload" />')
                            .appendTo($div);

            $btn.fileupload({
                datatype: 'json',
                url: o.FileUrl + '?' + $.param({ PropId: o.PropId }),
                paramName: 'fileupload',
                done: function (e, data) {
                    $moltxtarea.text(data.molData);
                    $div.dialog('close');
                    o.onSuccess();
                }
            });

            $div.CswButton({
                ID: 'fileupload_cancel',
                enabledText: 'Cancel',
                disabledText: 'Canceling',
                onClick: function () {
                    $div.dialog('close');
                }
            });

            $div.append('<br/>MOL Text (Paste from Clipboard):<br/>');
            $moltxtarea = $('<textarea id="moltxt" rows="4" cols="40">' + o.molData + '</textarea>')
                                    .appendTo($div);
            $div.append('<br/>');
            $txtsave = $div.CswButton({ ID: 'txt_save',
                enabledText: 'Save MOL Text',
                disabledText: 'Saving MOL...',
                onClick: function () {
                    Csw.ajax.post({
                        url: o.TextUrl,
                        data: {
                            molData: $moltxtarea.val(),
                            PropId: o.PropId
                        },
                        success: function (data) {
                            $div.dialog('close');
                            o.onSuccess();
                        },
                        error: function () {
                            $txtsave.CswButton('enable');
                        }
                    }); // ajax
                } // onClick
            }); // CswButton


            openDialog($div, 400, 300, null, 'Upload');
        }, // FileUploadDialog
        ShowLicenseDialog: function (options) {
            var o = {
                GetLicenseUrl: '/NbtWebApp/wsNBT.asmx/getLicense',
                AcceptLicenseUrl: '/NbtWebApp/wsNBT.asmx/acceptLicense',
                onAccept: function () { },
                onDecline: function () { }
            };
            if (options) $.extend(o, options);
            var $div = $('<div align="center"></div>');
            $div.append('Service Level Agreement<br/>');
            var $licensetextarea = $('<textarea id="license" disabled="true" rows="30" cols="80"></textarea>')
                                    .appendTo($div);
            $div.append('<br/>');

            Csw.ajax.post({
                url: o.GetLicenseUrl,
                success: function (data) {
                    $licensetextarea.text(data.license);
                }
            });

            var $acceptbtn = $div.CswButton({
                ID: 'license_accept',
                enabledText: 'I Accept',
                disabledText: 'Accepting...',
                onClick: function () {
                    Csw.ajax.post({
                        url: o.AcceptLicenseUrl,
                        success: function () {
                            $div.dialog('close');
                            o.onAccept();
                        },
                        error: function () {
                            $acceptbtn.CswButton('enable');
                        }
                    }); // ajax
                } // onClick
            }); // CswButton

            $div.CswButton({ ID: 'license_decline',
                enabledText: 'I Decline',
                disabledText: 'Declining...',
                onClick: function () {
                    $div.dialog('close');
                    o.onDecline();
                }
            });

            openDialog($div, 800, 600, null, 'Terms and Conditions');
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

            var $div = $('<div align="center"></div>');

            $div.append('Select a Label to Print:<br/>');
            var $labelsel_div = $('<div />')
                                .appendTo($div);
            var $labelsel;

            var jData = { PropId: o.propid };
            Csw.ajax.post({
                url: o.GetPrintLabelsUrl,
                data: jData,
                success: function (data) {
                    if (data.labels.length > 0) {
                        $labelsel = $('<select id="' + o.ID + '_labelsel"></select>');
                        for (var i = 0; i < data.labels.length; i++) {
                            var label = data.labels[i];
                            $labelsel.append('<option value="' + label.nodeid + '">' + label.name + '</option>');
                        }
                        $labelsel.appendTo($labelsel_div);
                        $printbtn.CswButton('enable');
                    } else {
                        $printbtn.hide();
                        $labelsel_div.append('<span>No labels have been assigned!</span>');
                    }
                } // success
            }); // ajax

            var $printbtn = $div.CswButton({
                ID: 'print_label_print',
                enabledText: 'Print',
                //disabledText: 'Printing...', 
                disableOnClick: false,
                onClick: function () {
                    var jData2 = { PropId: o.propid, PrintLabelNodeId: $labelsel.val() };
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
            }); // CswButton
            $printbtn.CswButton('disable');

            $div.CswButton({ ID: 'print_label_close',
                enabledText: 'Close',
                disabledText: 'Closing...',
                onClick: function () {
                    $div.dialog('close');
                }
            });

            var $hiddendiv = $('<div style="display: none; border: 1px solid red;"></div>')
                                .appendTo($div);
            $("<OBJECT ID='labelx' Name='labelx' classid='clsid:A8926827-7F19-48A1-A086-B1A5901DB7F0' codebase='CafLabelPrintUtil.cab#version=0,1,6,0' width=500 height=300 align=center hspace=0 vspace=0></OBJECT>")
                                .appendTo($hiddendiv);

            openDialog($div, 400, 300, null, 'Print');
        }, // PrintLabelDialog
        ErrorDialog: function (error) {
            var $div = $('<div />');
            openDialog($div, 400, 300, null, 'Error');
            $div.CswErrorMessage(error);
        },
        AlertDialog: function (message, title) {
            var $div = $('<div>' + message + '</div>');
            openDialog($div, 200, 200, null, title);
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

            var $div = $('<div id="' + o.ID + '"><p>' + o.navigationText + '</p></div>'),
                $select = $div.CswSelect('init', {
                    ID: Csw.controls.dom.makeId({ ID: o.ID, suffix: 'CswNavigationSelectDialog' }),
                    values: o.values
                });

            $div.CswButton({
                ID: Csw.controls.dom.makeId({ ID: o.ID, suffix: 'CswNavigationSelectDialog_OK' }),
                enabledText: 'OK',
                onClick: function () {
                    if (Csw.isFunction(o.onOkClick)) {
                        o.onOkClick($select.find(':selected'));
                    }
                    $div.dialog('close');
                }
            });
            openDialog($div, 600, 150, null, o.title);
        },
        //#endregion Specialized

        //#region Generic

        //		'OpenPopup': function (url) { 
        //							var popup = window.open(url, null, 'height=600, width=600, status=no, resizable=yes, scrollbars=yes, toolbar=yes,location=no, menubar=yes');
        //							popup.focus();
        //							return popup;
        //						},
        OpenDialog: function (id, url) {
            var $dialogdiv = $('<div id="' + id + '"></div>');
            $dialogdiv.load(url,
                            function () {
                                openDialog($dialogdiv, 600, 400);
                            });
        },
        CloseDialog: function (id) {
            $('#' + id)
                .dialog('close')
                .remove();
        }

        //#region Generic
    };


    function openDialog($div, width, height, onClose, title) {
        $('<div id="DialogErrorDiv" style="display: none;"></div>')
            .prependTo($div);

        $div.dialog({
            modal: true,
            width: width,
            height: height,
            title: title,
            close: function () {
                $div.remove();
                if (Csw.isFunction(onClose)) onClose();
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
