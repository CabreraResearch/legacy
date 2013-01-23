/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.nodeSelect = Csw.controls.nodeSelect ||
        Csw.controls.register('nodeSelect', function (cswParent, cswPrivate) {
            'use strict';

            //#region _preCtor

            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.$parent = cswPrivate.$parent || cswParent.$;
                cswPrivate.name = cswPrivate.name || '';
                cswPrivate.async = cswPrivate.async; // || true;
                cswPrivate.nodesUrlMethod = cswPrivate.nodesUrlMethod || 'Nodes/get';

                cswPrivate.labelText = cswPrivate.labelText || null;
                cswPrivate.excludeNodeTypeIds = cswPrivate.excludeNodeTypeIds || '';
                cswPrivate.selectedNodeId = cswPrivate.selectedNodeId || '';
                cswPrivate.selectedName = cswPrivate.selectedName || '';
                cswPrivate.viewid = cswPrivate.viewid || '';

                cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || '';
                cswPrivate.objectClassId = cswPrivate.objectClassId || '';
                cswPrivate.objectClassName = cswPrivate.objectClassName || '';
                cswPrivate.addNodeDialogTitle = cswPrivate.addNodeDialogTitle || '';

                cswPrivate.relatedTo = cswPrivate.relatedTo || {};
                cswPrivate.relatedTo.relatednodeid = cswPrivate.relatedTo.relatednodeid || '';
                cswPrivate.relatedTo.relatednodename = cswPrivate.relatedTo.relatednodename || '';
                cswPrivate.relatedTo.relatednodetypeid = cswPrivate.relatedTo.relatednodetypeid || '';
                cswPrivate.relatedTo.relatedobjectclassid = cswPrivate.relatedTo.relatedobjectclassid || '';

                cswPrivate.cellCol = cswPrivate.cellCol || 1;
                cswPrivate.width = cswPrivate.width || '200px';

                cswPrivate.onSelectNode = cswPrivate.onSelectNode || function () { };
                cswPrivate.onSuccess = cswPrivate.onSuccess || function () { };

                cswPrivate.addNewOption = cswPrivate.addNewOption; // || false;
                cswPrivate.allowAdd = cswPrivate.allowAdd; // || false;
                cswPrivate.isRequired = cswPrivate.isRequired; // || false;
                cswPrivate.isMulti = cswPrivate.isMulti; // || false;
                cswPrivate.isReadOnly = cswPrivate.isReadOnly; // || false;
                cswPrivate.showSelectOnLoad = cswPrivate.showSelectOnLoad; // || true;
                cswPrivate.isClickable = cswPrivate.isClickable; // ||true;
                cswPrivate.useSearch = cswPrivate.useSearch;
                cswPrivate.options = cswPrivate.options || [];

                cswPublic = cswParent.div();
                cswPrivate.table = cswPublic.table();

                // Default to selected node as relationship value for new nodes being added
                if (false === Csw.isNullOrEmpty(cswPrivate.relatedTo.relatednodeid) &&
                    Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                    false === cswPrivate.isMulti &&
                    (Csw.number(cswPrivate.relatedTo.relatednodetypeid) === Csw.number(cswPrivate.nodeTypeId) ||
                      Csw.number(cswPrivate.relatedTo.relatedobjectclassid) === Csw.number(cswPrivate.objectClassId))) {

                    cswPrivate.selectedNodeId = cswPrivate.relatedTo.relatednodeid;
                    cswPrivate.selectedName = cswPrivate.relatedTo.relatednodename;
                }

                cswPrivate.selectCellCol = cswPrivate.cellCol + 0;
                cswPrivate.textCellCol = cswPrivate.cellCol + 1;
                cswPrivate.editCellCol = cswPrivate.cellCol + 2;
                cswPrivate.nodeTypeCellCol = cswPrivate.cellCol + 3;
                cswPrivate.searchCellCol = cswPrivate.cellCol + 0;
                cswPrivate.searchButtonCellCol = cswPrivate.cellCol + 1;
                cswPrivate.addCellCol = cswPrivate.cellCol + 4;
                cswPrivate.tipCellCol = cswPrivate.cellCol + 5;

            } ());

            //#endregion _preCtor

            //#region AJAX

            cswPrivate.getNodes = function () {
                Csw.ajaxWcf.post({
                    urlMethod: cswPrivate.nodesUrlMethod,
                    async: Csw.bool(cswPrivate.async),
                    data: cswPrivate.ajaxData || {
                        NodeTypeId: Csw.number(cswPrivate.nodeTypeId, 0),
                        ObjectClassId: Csw.number(cswPrivate.objectClassId, 0),
                        ObjectClass: Csw.string(cswPrivate.objectClassName),
                        RelatedToObjectClass: Csw.string(cswPrivate.relatedTo.objectClassName),
                        RelatedToNodeId: Csw.string(cswPrivate.relatedTo.nodeId),
                        ViewId: Csw.string(cswPrivate.viewid)
                    },
                    success: function (data) {
                        //cswPrivate.options = JSON.parse(data.options);
                        var options = [];
                        data.Nodes.forEach(function (obj) {
                            options.push({ id: obj.NodeId, value: obj.NodeName, nodelink: obj.NodeLink });
                        });
                        cswPrivate.options = options;
                        cswPrivate.canAdd = Csw.bool(cswPrivate.canAdd) && Csw.bool(data.CanAdd);
                        cswPrivate.useSearch = Csw.bool(data.UseSearch);
                        cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || data.NodeTypeId;
                        cswPrivate.objectClassId = cswPrivate.objectClassId || data.ObjectClassId;
                        cswPrivate.relatedTo.objectClassId = cswPrivate.relatedTo.objectClassId || data.RelatedToObjectClassId;

                        cswPrivate.makeControl();
                    }
                });
            };

            cswPrivate.getNodeTypeOptions = function () {
                cswPrivate.blankText = '[Select One]';
                cswPrivate.selectedNodeType = cswPrivate.selectedNodeType ||
                    cswPrivate.table.cell(1, cswPrivate.nodeTypeCellCol)
                             .nodeTypeSelect({
                                 objectClassName: cswPrivate.objectClassName,
                                 objectClassId: cswPrivate.objectClassId,
                                 onSelect: function () {
                                     if (cswPrivate.blankText !== cswPrivate.selectedNodeType.val()) {
                                         cswPrivate.nodeTypeId = cswPrivate.selectedNodeType.val();
                                         cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId);
                                     }
                                 },
                                 onSuccess: function (data, nodeTypeCount, lastNodeTypeId) {
                                     if (Csw.number(nodeTypeCount) > 1) {
                                         cswPrivate.selectedNodeType.show();
                                         cswPrivate.addImage.hide();
                                     } else {
                                         cswPrivate.nodeTypeId = lastNodeTypeId;
                                         cswPrivate.selectedNodeType.hide();
                                         cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId);
                                     }
                                 },
                                 blankOptionText: cswPrivate.blankText,
                                 filterToPermission: 'Create'
                             }).hide();
            };

            //#endregion AJAX

            //#region Control Construction

            cswPrivate.makeControl = function () {
                cswPrivate.makeAddImage();
                if (cswPrivate.useSearch) {
                    cswPrivate.makeSearch();
                } else {
                    cswPrivate.makeSelect();
                }
                Csw.tryExec(cswPrivate.onSuccess, cswPrivate.options);
            };

            cswPrivate.bindSelectMethods = function () {
                cswPublic.val = cswPrivate.select.val;
                cswPublic.selectedText = cswPrivate.select.selectedText;
                cswPublic.selectedVal = cswPrivate.select.selectedVal;
                cswPublic.makeOption = cswPrivate.select.makeOption;
                cswPublic.makeOptions = cswPrivate.select.makeOptions;
                cswPublic.addOption = cswPrivate.select.addOption;
                cswPublic.setOptions = cswPrivate.select.setOptions;
                cswPublic.removeOption = cswPrivate.select.removeOption;
                cswPublic.option = cswPrivate.select.option;
            };

            cswPrivate.makeSelect = function () {
                
                //Normally, we would assign the select to cswPublic, as it's the primary control; 
                //however, to assign now would be decoupled from the return. The caller would not receive this reference.
                //Instead, we'll assign the select as a member and we'll reassign key methods to cswPublic.
                cswPrivate.select = cswPrivate.table.cell(1, cswPrivate.selectCellCol).select({
                    name: cswPrivate.name,
                    cssclass: 'selectinput',
                    onChange: function () {
                        var val = cswPrivate.select.selectedVal();
                        var name = cswPrivate.select.selectedText();
                        var link = cswPrivate.select.selectedData('link');
                        Csw.tryExec(cswPrivate.onSelectNode, { nodeid: val, name: name, selectedNodeId: val, relatednodelink: link });
                    },
                    width: cswPrivate.width
                });
                // Select value in a selectbox
                cswPrivate.foundSelected = false;

                Csw.each(cswPrivate.options, function (relatedObj) {
                    if (false === cswPrivate.isMulti && relatedObj.id === cswPrivate.selectedNodeId) {
                        cswPrivate.foundSelected = true;
                        cswPrivate.select.option({ value: relatedObj.id, display: relatedObj.value, selected: true }).data({ link: relatedObj.nodelink });
                    } else {
                        cswPrivate.select.option({ value: relatedObj.id, display: relatedObj.value }).data({ link: relatedObj.nodelink });
                    }
                });
                if (false === cswPrivate.isMulti && false === cswPrivate.foundSelected) {
                    // case 25820 - guarantee selected option appears
                    cswPrivate.select.option({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName }).data({ link: cswPrivate.selectedNodeLink });
                }

                cswPrivate.bindSelectMethods();

                cswPrivate.select.bind('change', function () {
                    var val = cswPrivate.select.val();
                    cswPrivate.selectedNodeId = val;
                    cswPrivate.selectedName = cswPrivate.select.selectedText();
                    Csw.tryExec(cswPrivate.onChange, cswPrivate.select);
                    Csw.tryExec(cswPrivate.onSelect, val);
                });

                cswPrivate.nodeLinkText = cswPrivate.table.cell(1, cswPrivate.textCellCol);

                if (false === cswPrivate.isMulti) {
                    cswPrivate.nodeLinkText = cswPrivate.nodeLinkText.nodeLink({
                        text: cswPrivate.selectedNodeLink
                    });
                }

                cswPrivate.toggleButton = cswPrivate.table.cell(1, cswPrivate.editCellCol).buttonExt({
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                    size: 'small',
                    enabledText: 'Edit',
                    onClick: function () {
                        cswPrivate.toggleOptions(true);
                    }
                });

                cswPrivate.toggleOptions(cswPrivate.showSelectOnLoad);

                cswPrivate.select.required(cswPrivate.isRequired);

                cswPrivate.nodeLinkText.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.select.val()); },
                                function (event) { Csw.nodeHoverOut(event, cswPrivate.select.val()); });

                // case 28427 - default private values to currently selected
                cswPrivate.selectedNodeId = cswPrivate.select.val();
                cswPrivate.selectedName = cswPrivate.select.selectedText();
            };

            cswPrivate.makeSearch = function () {
                if (cswPrivate.useSearch) {
                    // Find value by using search in a dialog

                    cswPrivate.nameSpan = cswPrivate.table.cell(1, cswPrivate.searchCellCol).nodeLink({
                        text: Csw.string(cswPrivate.selectedNodeLink) + '&nbsp;'
                    });

                    cswPrivate.hiddenValue = cswPrivate.table.cell(1, cswPrivate.searchCellCol).input({
                        name: 'hiddenvalue',
                        type: Csw.enums.inputTypes.hidden,
                        value: cswPrivate.selectedNodeId
                    });

                    cswPrivate.table.cell(1, cswPrivate.searchButtonCellCol).buttonExt({
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.magglass),
                        size: 'small',
                        enabledText: "Search",
                        tooltip: { title: "Search " + cswPrivate.name, },
                        onClick: function () {
                            $.CswDialog('SearchDialog', {
                                propname: cswPrivate.name,
                                nodetypeid: cswPrivate.nodeTypeId,
                                objectclassid: cswPrivate.objectClassId,
                                onSelectNode: function (nodeObj) {
                                    cswPrivate.nameSpan.empty();
                                    cswPrivate.nameSpan.nodeLink({
                                        text: nodeObj.nodelink + '&nbsp;'
                                    });
                                    cswPrivate.hiddenValue.val(nodeObj.nodeid);
                                    cswPrivate.selectedNodeId = nodeObj.nodeid;
                                    cswPrivate.selectedNodeLink = nodeObj.nodelink;
                                    Csw.tryExec(cswPrivate.onSelectNode, nodeObj);
                                }
                            });
                        }
                    });

                    cswPrivate.nameSpan.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.hiddenValue.val()); },
                        function (event) { Csw.nodeHoverOut(event, cswPrivate.hiddenValue.val()); });
                }
            };

            cswPrivate.toggleOptions = function (on) {
                if (Csw.bool(on)) {
                    cswPrivate.select.show();
                    if (cswPrivate.addImage) {
                        cswPrivate.addImage.show();
                    }
                    cswPrivate.toggleButton.hide();
                    cswPrivate.nodeLinkText.hide();
                } else {
                    cswPrivate.select.hide();
                    if (cswPrivate.addImage) {
                        cswPrivate.addImage.hide();
                    }
                    cswPrivate.toggleButton.show();
                    cswPrivate.nodeLinkText.show();
                }
            };

            //#endregion Control Construction

            //#region Add

            cswPrivate.onAddNodeFunc = function (nodeid, nodekey, nodename, nodelink) {
                if (cswPrivate.nameSpan) {
                    cswPrivate.nameSpan.empty();
                    cswPrivate.selectedNodeLink = nodelink;
                    cswPrivate.nameSpan.nodeLink({
                        text: nodelink
                    });
                }
                if (cswPrivate.hiddenValue) {
                    cswPrivate.hiddenValue.val(nodeid);
                }
                if (cswPrivate.select) {
                    Csw.ajaxWcf.post({
                        urlMethod: 'Nodes/get',
                        async: false,
                        data: cswPrivate.ajaxData || {
                            RelatedToNodeTypeId: Csw.number(cswPrivate.relatedTo.relatednodetypeid, 0),
                            ViewId: Csw.string(cswPrivate.viewid),
                            NodeTypeId: Csw.number(cswPrivate.nodeTypeId, 0),
                            ObjectClassId: Csw.number(cswPrivate.objectClassId, 0),
                            ObjectClass: Csw.string(cswPrivate.objectClassName),
                            RelatedToObjectClass: Csw.string(cswPrivate.relatedTo.objectClassName),
                            RelatedToNodeId: Csw.string(cswPrivate.relatedTo.nodeId)
                        },
                        success: function (data) {
                            if (data.Nodes.length > 0) {
                                var found = false;
                                
                                data.Nodes.forEach(function (obj) {
                                    if (obj.NodeId === nodeid) {
                                        found = true;
                                        options.push({ id: obj.NodeId, value: obj.NodeName, isSelected: obj.NodeId === nodeid });
                                        cswPrivate.select.option({ value: obj.NodeId, display: obj.NodeName, selected: true }).data({ link: obj.NodeLink });
                                        cswPrivate.select.val(obj.NodeId);
                                        cswPrivate.selectedNodeId = obj.NodeId;
                                    }
                                });
                                if (false === found) {
                                    cswPrivate.select.val(cswPrivate.selectedNodeId);
                                    cswPrivate.table.cell(1, cswPrivate.tipCellCol).quickTip({ html: nodename + ' has been added.  However, it is not an available option for ' + cswPrivate.name + '.' });
                                }
                            } else {
                                cswPrivate.select.option({ value: nodeid, display: nodename });
                            }
                        }
                    });
                    cswPrivate.toggleOptions(true);

                    cswPrivate.select.$.valid();
                }
            };

            cswPrivate.openAddNodeDialog = function (nodetypeToAdd) {
                $.CswDialog('AddNodeDialog', {
                    nodetypeid: nodetypeToAdd,
                    onAddNode: cswPrivate.onAddNodeFunc,
                    text: 'Add New ' + cswPrivate.name,
                    relatednodeid: cswPrivate.relatedTo.relatednodeid,
                    relatednodename: cswPrivate.relatedTo.relatednodename,
                    relatednodetypeid: cswPrivate.relatedTo.relatednodetypeid,
                    relatedobjectclassid: cswPrivate.relatedTo.relatedobjectclassid
                });
            };

            cswPrivate.makeAddImage = function () {
                if (cswPrivate.allowAdd) {
                    cswPrivate.addImage = cswPrivate.table.cell(1, cswPrivate.addCellCol).div()
                        .buttonExt({
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                            size: 'small',
                            enabledText: 'New',
                            tooltip: { title: 'Add New ' + cswPrivate.name },
                            onClick: function() {
                                if (Csw.number(cswPrivate.nodeTypeId) > 0) {
                                    cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId);
                                } else {
                                    cswPrivate.getNodeTypeOptions();
                                }
                            }
                        });
                }
            };

            //#endregion Add

            //#region Public

            cswPublic.setSelectedNode = function (nodeid, nodename) {
                cswPrivate.selectedNodeId = nodeid;
                cswPrivate.selectedName = nodename;
                if (cswPrivate.useSearch && cswPrivate.nameSpan && cswPrivate.hiddenValue) {
                    cswPrivate.nameSpan.text(nodename);
                    cswPrivate.hiddenValue.val(nodeid);
                } else if (cswPrivate.select) {
                    cswPrivate.select.val(nodeid);
                }
            }; // setSelectedNode

            cswPublic.selectedNodeId = function () {
                return cswPrivate.selectedNodeId;
            }; // selectedNodeId
            cswPublic.selectedName = function () {
                return cswPrivate.selectedName;
            }; // selectedName

            //#endregion Public

            //#region _postCtor

            (function _relationship() {
                if (false === cswPrivate.isClickable) { //case 28180 - relationships not clickable from audit history grid
                    cswPrivate.nodeTextCell = cswPrivate.table.cell(1, cswPrivate.textCellCol);
                    cswPrivate.nodeText = cswPrivate.nodeTextCell.span({
                        text: cswPrivate.selectedName
                    });
                } else if (cswPrivate.isReadOnly) {
                    cswPrivate.nodeLinkTextCell = cswPrivate.table.cell(1, cswPrivate.textCellCol);
                    cswPrivate.nodeLinkText = cswPrivate.nodeLinkTextCell.nodeLink({
                        text: cswPrivate.selectedNodeLink
                    });
                    cswPublic.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectedNodeId); },
                                    function (event) { Csw.nodeHoverOut(event, cswPrivate.selectedNodeId); });
                } else {
                    if (cswPrivate.options.length > 0 || false === cswPrivate.doGetNodes) {
                        cswPrivate.makeControl();
                    } else {
                        cswPrivate.getNodes();
                    }
                } // if-else (o.ReadOnly) {
            } ());

            return cswPublic;

            //#endregion _postCtor
        });
} ());

