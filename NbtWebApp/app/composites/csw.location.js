
/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function () {
    'use strict';

    Csw.composites.location = Csw.composites.location ||
        Csw.composites.register('location', function (cswParent, options) {
            //#region variables
            var cswPrivate = {
                name: 'csw_location_tree',
                locationobjectclassid: '',
                locationnodetypeids: [],
                relatedmatch: false,
                relatedobjectclassid: '',
                nodeid: '',
                viewid: '',
                selectedName: '',
                path: '',
                nodeKey: '',
                selectednodelink: '',
                Multi: false,
                ReadOnly: false,
                isRequired: false,
                onChange: null,
                overrideSelectedLocation: true,
                EditMode: Csw.enums.editMode.Edit,
                value: ''
            };

            var cswPublic = {};
            //#endregion variables

            //#region safety net
            Csw.tryExec(function () {

                //#region init ctor
                (function _pre() {
                    Csw.extend(cswPrivate, options, true);
                    if (Csw.isNullOrEmpty(cswPrivate.viewid)) {
                        Csw.ajax.post({
                            urlMethod: 'getLocationView',
                            async: false,
                            data: {
                                NodeId: Csw.string(cswPrivate.nodeid)
                            },
                            success: function (data) {
                                cswPrivate.viewid = data.viewid;
                                cswPrivate.nodeid = data.nodeid;
                                cswPrivate.path = data.path;
                            }
                        });
                    }
                    cswParent.empty();

                    cswPrivate.relatedmatch = (cswPrivate.relatedobjectclassid === cswPrivate.locationobjectclassid);
                    Csw.each(cswPrivate.locationnodetypeids, function (thisNTid) {
                        cswPrivate.relatedmatch = (cswPrivate.relatedmatch || thisNTid === cswPrivate.relatednodetypeid);
                    });

                    if (cswPrivate.relatedmatch) {
                        cswPrivate.nodeid = Csw.string(cswPrivate.relatednodeid, cswPrivate.nodeid).trim();
                        cswPrivate.name = Csw.string(cswPrivate.relatednodename, cswPrivate.name).trim();
                        cswPrivate.path = Csw.string(cswPrivate.relatednodename, cswPrivate.path).trim();
                    } else {
                        cswPrivate.nodeid = Csw.string(cswPrivate.nodeid).trim();
                        cswPrivate.name = Csw.string(cswPrivate.name).trim();
                        cswPrivate.path = Csw.string(cswPrivate.path).trim();
                    }
                    cswPrivate.viewid = Csw.string(cswPrivate.viewid).trim();
                    cswPrivate.value = cswPrivate.nodeid;
                    cswPublic.table = cswParent.table({ TableCssClass: 'cswInline' });

                    cswPrivate.pathCell = cswPublic.table.cell(1, 1);
                    cswPrivate.selectCell = cswPublic.table.cell(1, 2);
                    cswPrivate.editCell = cswPublic.table.cell(1, 3);
                    cswPrivate.previewCell = cswPublic.table.cell(1, 4);
                    cswPrivate.validateCell = cswPublic.table.cell(1, 5);

                    if (cswPrivate.isRequired) {
                        cswPrivate.checkBox = cswPrivate.validateCell.div().input().hide();
                        cswPrivate.checkBox.required(true);
                        cswPrivate.checkBox.addClass('validateLocation');
                        if (false === Csw.isNullOrEmpty(cswPrivate.nodeid)) {
                            cswPrivate.checkBox.val(true);
                        } else {
                            cswPrivate.checkBox.val(false);
                        }
                        $.validator.addMethod('validateLocation', function () {
                            return Csw.bool(cswPrivate.checkBox.val());
                        }, 'Location is required.');

                    }

                    cswPrivate.pathCell.text(cswPrivate.path);

                    cswPrivate.selectDiv = cswPrivate.selectCell.div({
                        cssclass: 'locationselect',
                        value: cswPrivate.nodeid,
                        onChange: function () {
                            cswPrivate.selectDiv.val();
                        }
                    });
                    cswPrivate.selectCell.hide();
                } ());
                //#endregion init ctor

                //#region cswPrivate/cswPublic methods and props

                cswPublic.val = function () {
                    return cswPrivate.value;
                };

                cswPublic.selectedName = function () {
                    return cswPrivate.selectedName;
                };

                cswPrivate.onTreeSelect = function (optSelect) {
                    optSelect = optSelect || { nodeid: '', nodename: '', iconurl: '' };
                    if (optSelect.nodeid === 'root' || optSelect.nodeid === undefined) {
                        optSelect.nodeid = ''; // case 21046   
                    }
                    cswPublic.comboBox.topContent(optSelect.nodename, optSelect.nodeid);
                    if (cswPublic.comboBox.val() !== optSelect.nodeid) {
                        cswPublic.comboBox.val(optSelect.nodeid);
                    }
                    Csw.tryExec(cswPrivate.onChange, optSelect.nodeid, optSelect.nodename);
                    cswPrivate.value = optSelect.nodeid;
                    cswPrivate.name = optSelect.nodename;
                    Csw.defer(function () { cswPublic.comboBox.close(); }, 100);
                };

                cswPrivate.makeLocationCombo = function () {
                    cswPrivate.pathCell.hide();
                    cswPrivate.editCell.hide();
                    cswPrivate.selectCell.show();

                    cswPublic.comboBox = cswPrivate.selectDiv.comboBox({
                        name: cswPrivate.name + '_combo',
                        topContent: cswPrivate.selectedName,
                        selectContent: '',
                        width: '290px',
                        onClick: (function () {
                            var first = true;
                            return function () {
                                if (first) {      // only do this once
                                    cswPublic.locationTree.nodeTree.expandAll();
                                    first = false;
                                }
                                if (cswPrivate.isRequired) {
                                    cswPrivate.checkBox.$.valid();
                                }
                                cswPublic.comboBox.open(); // ensure we're open on click
                                return false;    // but only close when onTreeSelect fires, below
                            };
                        })()
                    });
                    
                    cswPublic.locationTree = Csw.nbt.nodeTreeExt(cswPublic.comboBox.pickList, {
                        name: cswPrivate.name,
                        onBeforeSelectNode: function(treeNode) {
                            var ret = true;
                            if (cswPrivate.isRequired) {
                                if (!treeNode.raw || Csw.isNullOrEmpty(treeNode.raw.nodeid)) {
                                    ret = false;
                                    //cswPrivate.checkBox.val(false); //no need to set the checkbox false--we just prevented selection
                                } else {
                                    cswPrivate.checkBox.val(true);
                                }
                                cswPrivate.checkBox.$.valid();
                            }
                            return ret;
                        },
                        onSelectNode: function(node) {
                            Csw.tryExec(cswPrivate.onTreeSelect, node);
                        },
                        showToggleLink: false,
                        useScrollbars: false,
                        rootVisible: true,
                        requireViewPermissions: false,
                        forceSelected: cswPrivate.overrideSelectedLocation,
                        useHover: (cswPrivate.EditMode !== Csw.enums.editMode.Add), // case 28849
                        state: {
                            viewId: cswPrivate.viewid,
                            nodeId: cswPrivate.nodeid,
                            nodeKey: cswPrivate.nodeKey,
                            includeInQuickLaunch: false,
                            defaultSelect: Csw.enums.nodeTree_DefaultSelect.root.name
                        }
                    });
                }; // makeLocationCombo()

                //#endregion cswPrivate/cswPublic methods and props

                //#region final ctor
                (function _post() {
                    if (false === cswPrivate.ReadOnly) {
                        if (cswPrivate.EditMode === Csw.enums.editMode.Add) {
                            cswPrivate.makeLocationCombo();
                        } else {
                            cswPrivate.editCell.icon({
                                name: cswPrivate.name + '_toggle',
                                iconType: Csw.enums.iconType.pencil,
                                hovertext: 'Edit',
                                size: 16,
                                isButton: true,
                                onClick: cswPrivate.makeLocationCombo
                            }); // imageButton
                        }

                        cswPrivate.previewCell.css({ width: '24px' });
                        cswParent.$.hover(function (event) {
                            Csw.nodeHoverIn(event, {
                                nodeid: cswPrivate.value,
                                nodename: cswPrivate.selectedName,
                                parentDiv: cswPrivate.previewCell,
                                useAbsolutePosition: false,
                                rightpad: 0
                            });
                        },
                        function (event) { Csw.nodeHoverOut(event, cswPrivate.value); });
                    }
                } ());
                //#region final ctor

            });
            //#endregion safety net

            return cswPublic;
        });

} ());


