/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswTable.js" />
/// <reference path="../actions/CswAuditHistoryGrid.js" />
/// <reference path="../node/CswNodeTabs.js" />

(function ($) { /// <param name="$" type="jQuery" />
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
                onclick: function () { $div.dialog('close'); o.onYes(); }
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
                if (isFunction(o.onAdd)) {
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
            var $table = $div.CswTable('init', { 'ID': o.ID + '_tbl', 'FirstCellRightAlign': true });

            $table.CswTable('cell', 1, 1).append('Name:');
            var $nametextcell = $table.CswTable('cell', 1, 2);
            var $nametextbox = $nametextcell.CswInput('init', { ID: o.ID + '_nametb',
                type: CswInput_Types.text,
                cssclass: 'textinput'
            });
            var $displaymodeselect = $('<select id="' + o.ID + '_dmsel" />');
            if (isNullOrEmpty(o.viewid)) {
                $table.CswTable('cell', 2, 1).append('Display Mode:');
                $displaymodeselect.append('<option value="Grid">Grid</option>');
                $displaymodeselect.append('<option value="List" selected>List</option>');
                $displaymodeselect.append('<option value="Table">Table</option>');
                $displaymodeselect.append('<option value="Tree">Tree</option>');
                $displaymodeselect.appendTo($table.CswTable('cell', 2, 2));
            }

            var v = makeViewVisibilitySelect($table, 3, 'Available to:');
            var $savebtn = $div.CswButton({
                ID: o.ID + '_submit',
                enabledText: 'Create View',
                disabledText: 'Creating View',
                onclick: function () {

                    var createData = {};
                    createData.ViewName = $nametextbox.val();
                    createData.ViewId = o.viewid;
                    if (isNullOrEmpty(o.viewmode)) {
                        createData.ViewMode = $displaymodeselect.val();
                    } else {
                        createData.ViewMode = o.viewmode;
                    }
                    if (!isNullOrEmpty(v.getvisibilityselect())) {
                        createData.Visibility = v.getvisibilityselect().val();
                        createData.VisibilityRoleId = v.getvisroleselect().val();
                        createData.VisibilityUserId = v.getvisuserselect().val();
                    } else {
                        createData.Visibility = "";
                        createData.VisibilityRoleId = "";
                        createData.VisibilityUserId = "";
                    }

                    CswAjaxJson({
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
                onclick: function () {
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
                EditMode: EditMode.AddInPopup.name,
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
            $newNode = $div.CswInput('init', { ID: o.ID + '_newNode', type: CswInput_Types.text });

            $div.CswButton({
                ID: o.objectClassId + '_add',
                enabledText: 'Add',
                onclick: function () {
                    if (isFunction(o.onSuccess)) {
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
                category = tryParseString(o.category);

            $div.append('New ' + o.nodeTypeDescriptor + ': ');
            $nodeType = $div.CswInput('init', { ID: o.objectClassId + '_nodeType', type: CswInput_Types.text, value: o.nodetypename, maxlength: o.maxlength });
            $div.append('<br />');
            if (isNullOrEmpty(category)) {
                $div.append('Category Name: ');
                $category = $div.CswInput('init', { ID: o.objectClassId + '_category', type: CswInput_Types.text });
                $div.append('<br />');
            }
            $addBtn = $div.CswButton({
                ID: o.objectClassId + '_add',
                enabledText: 'Add',
                onclick: function () {
                    var newNodeTypeName = $nodeType.val();
                    CswAjaxJson({
                        url: '/NbtWebApp/wsNBT.asmx/IsNodeTypeNameUnique',
                        async: false,
                        data: { 'NodeTypeName': newNodeTypeName },
                        success: function () {
                            o.$select.append('<option data-newNodeType="true" value="' + $nodeType.val() + '">' + $nodeType.val() + '</option>');
                            o.$select.val($nodeType.val());
                            if (isNullOrEmpty(category) && false === isNullOrEmpty($category)) {
                                category = $category.val();
                            }
                            $div.dialog('close');
                            if (isFunction(o.onSuccess)) {
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
            var $table = $div.CswTable('init', { ID: 'EditLayoutDialog_table', width: '100%' });
            var $cell11 = $table.CswTable('cell', 1, 1);
            var $cell12 = $table.CswTable('cell', 1, 2);

            $cell11.append("Configure:<br/>");
            var $layoutSelect = $cell11.CswSelect('init', {
                ID: 'EditLayoutDialog_layoutselect',
                selected: 'Edit',
                values: [{ value: 'AddInPopup', display: 'Add' },
                                                { value: 'Edit', display: 'Edit' },
                                                { value: 'Preview', display: 'Preview'}],
                onChange: function () {
                    cswNodeTabOptions.EditMode = $('#EditLayoutDialog_layoutselect option:selected').val();
                    _resetLayout();
                }
            });

            $cell11.append("<br/><br/>Add:<br/>");
            var $addSelect = $cell11.CswSelect('init', {
                ID: 'EditLayoutDialog_addselect',
                selected: '',
                values: [],
                onChange: function () {

                    var ajaxdata = {
                        PropId: $addSelect.val(),
                        TabId: cswNodeTabOptions.tabid,
                        EditMode: $layoutSelect.val()
                    };
                    CswAjaxJson({
                        url: '/NbtWebApp/wsNBT.asmx/addPropertyToLayout',
                        data: ajaxdata,
                        success: function () {
                            _resetLayout();
                        }
                    }); // CswAjaxJson
                } // onChange
            }); // CswSelect

            function _resetLayout() {
                $cell12.contents().remove();
                $cell12.CswNodeTabs(cswNodeTabOptions);
                _configAddOptions();
            }

            function _configAddOptions() {
                var ajaxdata = {
                    NodeId: cswNodeTabOptions.nodeids[0],
                    NodeKey: cswNodeTabOptions.nodekeys[0],
                    NodeTypeId: cswNodeTabOptions.nodetypeid,
                    TabId: cswNodeTabOptions.tabid,
                    EditMode: $layoutSelect.val()
                };
                CswAjaxJson({
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
                });  // CswAjaxJson
            } // _configAddOptions()

            function _onclose() {
                cswNodeTabOptions.Refresh();
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
                filterToPropId: '',
                title: '',
                onEditNode: null, // function (nodeid, nodekey) { },
                onEditView: null, // function (viewid) {}
                date: ''     // viewing audit records
            };
            if (options) $.extend(o, options);

            var $div = $('<div></div>');

            var myEditMode = EditMode.EditInPopup.name;
            var $table = $div.CswTable();
            if (false === isNullOrEmpty(o.date) && false === o.Multi) {
                myEditMode = EditMode.AuditHistoryInPopup.name;
                $table.CswTable('cell', 1, 1).CswAuditHistoryGrid({
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
            var $tabcell = $table.CswTable('cell', 1, 2);

            setupTabs(o.date);

            function setupTabs(date) {
                $tabcell.empty();
                $tabcell.CswNodeTabs({
                    nodeids: o.nodeids,
                    nodekeys: o.nodekeys,
                    nodenames: o.nodenames,
                    filterToPropId: o.filterToPropId,
                    Multi: o.Multi,
                    EditMode: myEditMode,
                    //title: o.title,
                    tabid: $.CswCookie('get', CswCookieName.CurrentTabId),
                    date: date,
                    onEditView: function (viewid) {
                        if (isFunction(o.onEditView)) {
                            $div.dialog('close');
                            o.onEditView(viewid);
                        }
                    },
                    onSave: function (nodeids, nodekeys, tabcount) {
                        unsetChanged();
                        if (tabcount === 1 || o.Multi) {
                            $div.dialog('close');
                        }
                        setupTabs(date);
                        if (isFunction(o.onEditNode)) {
                            o.onEditNode(nodeids, nodekeys);
                        }
                    },
                    onBeforeTabSelect: function () {
                        return manuallyCheckChanges();
                    },
                    onTabSelect: function (tabid) {
                        $.CswCookie('set', CswCookieName.CurrentTabId, tabid);
                    },
                    onPropertyChange: function () {
                        setChanged();
                    }
                });
            } // _setupTabs()
            var title = tryParseString(o.title);
            if (isNullOrEmpty(title)) {
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
                onclick: function () {
                    copyNode({
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
                onclick: function () {
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
                onDeleteNode: null, //function(nodeid, nodekey) { },
                Multi: false,
                NodeCheckTreeId: '',
                publishDeleteEvent: true
            };

            if (options) {
                $.extend(o, options);
            }

            var $div = $('<div><span>Are you sure you want to delete:&nbsp;</span></div>');

            if (o.Multi) {
                if (o.nodeids.length === 0 || o.cswnbtnodekeys.length === 0) {
                var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
                if (false === isNullOrEmpty($nodechecks, true) && 
                    (o.nodeids.length === 0 || o.cswnbtnodekeys.length === 0 )) {
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
                onclick: function () {
                    deleteNodes({
                        nodeids: o.nodeids,
                        nodekeys: o.cswnbtnodekeys,
                        onSuccess: function (nodeid, nodekey) {
                            $div.dialog('close');
                            if (isFunction(o.onDeleteNode)) {
                                o.onDeleteNode(nodeid, nodekey);
                            }
                            if (isTrue(o.publishDeleteEvent)) {
                                $.publish(ChemSW.enums.Events.CswNodeDelete, { nodeids: o.nodeids, cswnbtnodekeys: o.cswnbtnodekeys });
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
                onclick: function () {
                    $div.dialog('close');
                }
            });
            openDialog($div, 400, 200, null, 'Confirm Delete');
        }, // DeleteNodeDialog
        AboutDialog: function () {
            var $div = $('<div></div>');
            CswAjaxJson({
                url: '/NbtWebApp/wsNBT.asmx/getAbout',
                data: {},
                success: function (data) {
                    $div.append('NBT Assembly Version: ' + data.assembly + '<br/><br/>');
                    var $table = $div.CswTable('init', { ID: 'abouttable' });
                    var row = 1;

                    var components = data.components;
                    for (var comp in components) {
                        if (contains(components, comp)) {
                            var thisComp = components[comp];
                            var $namecell = $table.CswTable('cell', row, 1);
                            var $versioncell = $table.CswTable('cell', row, 2);
                            var $copyrightcell = $table.CswTable('cell', row, 3);
                            $namecell.css('padding', '2px 5px 2px 5px');
                            $versioncell.css('padding', '2px 5px 2px 5px');
                            $copyrightcell.css('padding', '2px 5px 2px 5px');
                            $namecell.append(thisComp.name);
                            $versioncell.append(thisComp.version);
                            $copyrightcell.append(thisComp.copyright);
                            row++;
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
                onclick: function () {
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
                onclick: function () {
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
                onclick: function () {
                    CswAjaxJson({
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
                } // onclick
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

            CswAjaxJson({
                url: o.GetLicenseUrl,
                success: function (data) {
                    $licensetextarea.text(data.license);
                }
            });

            var $acceptbtn = $div.CswButton({
                ID: 'license_accept',
                enabledText: 'I Accept',
                disabledText: 'Accepting...',
                onclick: function () {
                    CswAjaxJson({
                        url: o.AcceptLicenseUrl,
                        success: function () {
                            $div.dialog('close');
                            o.onAccept();
                        },
                        error: function () {
                            $acceptbtn.CswButton('enable');
                        }
                    }); // ajax
                } // onclick
            }); // CswButton

            $div.CswButton({ ID: 'license_decline',
                enabledText: 'I Decline',
                disabledText: 'Declining...',
                onclick: function () {
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
            CswAjaxJson({
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
                onclick: function () {
                    var jData2 = { PropId: o.propid, PrintLabelNodeId: $labelsel.val() };
                    CswAjaxJson({
                        url: o.GetEPLTextUrl,
                        data: jData2,
                        success: function (data) {
                            var labelx = $('#labelx').get(0);
                            labelx.EPLScript = data.epl;
                            labelx.Print();
                        } // success
                    }); // ajax
                } // onclick
            }); // CswButton
            $printbtn.CswButton('disable');

            $div.CswButton({ ID: 'print_label_close',
                enabledText: 'Close',
                disabledText: 'Closing...',
                onclick: function () {
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
                buttons: ChemSW.enums.CswDialogButtons["1"],
                values: [],
                onOkClick: null,
                onCancelClick: null
            };

            if (options) {
                $.extend(o, options);
            }

            var $div = $('<div id="' + o.ID + '"><p>' + o.navigationText + '</p></div>'),
                $select = $div.CswSelect('init', {
                    ID: makeId({ ID: o.ID, suffix: 'CswNavigationSelectDialog' }),
                    values: o.values
                });

            $div.CswButton({
                ID: makeId({ ID: o.ID, suffix: 'CswNavigationSelectDialog_OK' }),
                enabledText: 'OK',
                onclick: function () {
                    if (isFunction(o.onOkClick)) {
                        o.onOkClick($select.find(':selected'));
                    }
                    $div.dialog('close');
                }
            });
            openDialog($div, 600, 150, null, o.title);
        },
        //#endregion Specialized

        //#region Generic

        //		'OpenPopup': function(url) { 
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
                if (isFunction(onClose)) onClose();
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
