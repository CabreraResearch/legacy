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

                cswPrivate.relationships = [];
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
                            options.push({ id: obj.NodeId, value: obj.NodeName });
                        });
                        cswPrivate.options = options;
                        cswPrivate.canAdd = Csw.bool(cswPrivate.canAdd) && Csw.bool(data.CanAdd);
                        cswPrivate.useSearch = Csw.bool(data.UseSearch);
                        cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || data.NodeTypeId;
                        cswPrivate.objectClassId = cswPrivate.objectClassId || data.ObjectClassId;
                        cswPrivate.relatedTo.objectClassId = cswPrivate.relatedTo.objectClassId || data.RelatedToObjectClassId

                        cswPrivate.makeControl();
                    }
                });
            };

            cswPrivate.getNodeTypeOptions = function () {
                cswPrivate.blankText = '[Select One]';
                cswPrivate.selectedNodeType = cswPrivate.selectedNodeType ||
                    cswPrivate.table.cell(1, cswPrivate.cellCol)
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
                cswPrivate.cellCol += 1;
            };

            //#endregion AJAX

            //#region Control Construction

            cswPrivate.makeControl = function () {
                if (cswPrivate.useSearch) {
                    cswPrivate.makeSearch();
                } else {
                    cswPrivate.makeSelect();
                }
                cswPrivate.makeAdd();
                Csw.tryExec(cswPrivate.onSuccess, cswPrivate.relationships);
            };

            cswPrivate.bindSelectMethods = function () {
                cswPublic.val = cswPublic.select.val;
                cswPublic.selectedText = cswPublic.select.selectedText;
                cswPublic.selectedVal = cswPublic.select.selectedVal;
                cswPublic.makeOption = cswPublic.select.makeOption;
                cswPublic.makeOptions = cswPublic.select.makeOptions;
                cswPublic.addOption = cswPublic.select.addOption;
                cswPublic.setOptions = cswPublic.select.setOptions;
                cswPublic.removeOption = cswPublic.select.removeOption;
                cswPublic.option = cswPublic.select.option;
            };

            cswPrivate.makeSelect = function () {
                // Select value in a selectbox
                cswPrivate.foundSelected = false;

                Csw.each(cswPrivate.options, function (relatedObj) {
                    if (false === cswPrivate.isMulti && relatedObj.id === cswPrivate.selectedNodeId) {
                        cswPrivate.foundSelected = true;
                    }
                    cswPrivate.relationships.push({ value: relatedObj.id, display: relatedObj.value });
                });
                if (false === cswPrivate.isMulti && false === cswPrivate.foundSelected) {
                    // case 25820 - guarantee selected option appears
                    cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName });
                }

                //Normally, we would assign the select to cswPublic, as it's the primary control; 
                //however, to assign now would be decoupled from the return. The caller would not receive this reference.
                //Instead, we'll assign the select as a member and we'll reassign key methods to cswPublic.
                cswPublic.select = cswPrivate.table.cell(1, cswPrivate.cellCol).select({
                    name: cswPrivate.name,
                    cssclass: 'selectinput',
                    onChange: function () {
                        var val = cswPublic.select.val();
                        Csw.tryExec(cswPrivate.onSelectNode, { nodeid: val });
                    },
                    values: cswPrivate.relationships,
                    selected: cswPrivate.selectedNodeId,
                    width: cswPrivate.width
                });
                cswPrivate.bindSelectMethods();

                cswPublic.select.bind('change', function () {
                    var val = cswPublic.select.val();
                    cswPrivate.selectedNodeId = val;
                    Csw.tryExec(cswPrivate.onChange, cswPublic.select);
                    Csw.tryExec(cswPrivate.onSelect, val);
                });

                cswPrivate.cellCol += 1;
                cswPrivate.nodeLinkText = cswPrivate.table.cell(1, cswPrivate.cellCol);
                cswPrivate.cellCol += 1;
                if (false === cswPrivate.isMulti) {
                    cswPrivate.nodeLinkText = cswPrivate.nodeLinkText.nodeLink({
                        text: cswPrivate.selectedNodeLink
                    });
                }

                cswPrivate.toggleButton = cswPrivate.table.cell(1, cswPrivate.cellCol).icon({
                    iconType: Csw.enums.iconType.pencil,
                    isButton: true,
                    onClick: function () {
                        cswPrivate.toggleOptions(true);
                    }
                });
                cswPrivate.cellCol += 1;

                cswPrivate.toggleOptions(cswPrivate.showSelectOnLoad);

                cswPublic.select.required(cswPrivate.isRequired);

                cswPrivate.nodeLinkText.$.hover(function (event) { Csw.nodeHoverIn(event, cswPublic.select.val()); },
                                function (event) { Csw.nodeHoverOut(event, cswPublic.select.val()); });
            };

            cswPrivate.makeSearch = function () {
                if (cswPrivate.useSearch) {
                    // Find value by using search in a dialog

                    cswPrivate.nameSpan = cswPrivate.table.cell(1, cswPrivate.cellCol).span({
                        name: 'selectedname',
                        text: cswPrivate.selectedName
                    });

                    cswPrivate.hiddenValue = cswPrivate.table.cell(1, cswPrivate.cellCol).input({
                        name: 'hiddenvalue',
                        type: Csw.enums.inputTypes.hidden,
                        value: cswPrivate.selectedNodeId
                    });
                    cswPrivate.cellCol += 1;

                    cswPrivate.table.cell(1, cswPrivate.cellCol).icon({
                        iconType: Csw.enums.iconType.magglass,
                        hovertext: "Search " + cswPrivate.name,
                        size: 16,
                        isButton: true,
                        onClick: function () {
                            $.CswDialog('SearchDialog', {
                                propname: cswPrivate.name,
                                nodetypeid: cswPrivate.nodeTypeId,
                                objectclassid: cswPrivate.objectClassId,
                                onSelectNode: function (nodeObj) {
                                    cswPrivate.nameSpan.text(nodeObj.nodename);
                                    cswPrivate.hiddenValue.val(nodeObj.nodeid);
                                    cswPrivate.selectedNodeId = nodeObj.nodeid;
                                    Csw.tryExec(cswPrivate.onSelectNode, nodeObj);
                                }
                            });
                        }
                    });
                    cswPrivate.cellCol += 1;

                    cswPrivate.nameSpan.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.hiddenValue.val()); },
                        function (event) { Csw.nodeHoverOut(event, cswPrivate.hiddenValue.val()); });
                }
            };

            cswPrivate.toggleOptions = function (on) {
                if (Csw.bool(on)) {
                    cswPublic.select.show();
                    cswPrivate.toggleButton.hide();
                    cswPrivate.nodeLinkText.hide();
                } else {
                    cswPublic.select.hide();
                    cswPrivate.toggleButton.show();
                    cswPrivate.nodeLinkText.show();
                }
            };

            //#endregion Control Construction

            //#region Add

            cswPrivate.onAddNodeFunc = function (nodeid, nodekey, nodename) {
                if (cswPrivate.nameSpan) {
                    cswPrivate.nameSpan.text(nodename);
                }
                if (cswPrivate.hiddenValue) {
                    cswPrivate.hiddenValue.val(nodeid);
                }
                if (cswPublic && cswPublic.select) {
                    cswPublic.select.option({ value: nodeid, display: nodename });
                    cswPublic.select.val(nodeid);
                    cswPrivate.toggleOptions(true);
                    Csw.tryExec(cswPrivate.onSelectNode, { nodeid: nodeid });
                    cswPublic.select.$.valid();
                }
            };

            cswPrivate.openAddNodeDialog = function (nodetypeToAdd) {
                $.CswDialog('AddNodeDialog', {
                    nodetypeid: nodetypeToAdd,
                    onAddNode: cswPrivate.onAddNodeFunc,
                    text: cswPrivate.name
                });
            };

            cswPrivate.makeAddImage = function () {
                cswPrivate.addImage = cswPrivate.table.cell(1, cswPrivate.cellCol).div()
                    .buttonExt({
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                        size: 'small',
                        tooltip: { title: 'Add New ' + cswPrivate.name },
                        onClick: function () {
                            if (Csw.number(cswPrivate.nodeTypeId) > 0) {
                                cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId);
                            }
                            else {
                                cswPrivate.getNodeTypeOptions();
                            }
                        }
                    });
                cswPrivate.cellCol += 1;
            };

            cswPrivate.makeAdd = function () {
                if (cswPrivate.allowAdd) {
                    cswPrivate.makeAddImage();
                } //if (allowAdd)
            };

            //#endregion Add

            //#region Public

            cswPublic.setSelectedNode = function (nodeid, nodename) {
                cswPrivate.selectedNodeId = nodeid;
                cswPrivate.selectedName = nodename;
                if (cswPrivate.useSearch) {
                    cswPrivate.nameSpan.text(nodename);
                    cswPrivate.hiddenValue.val(nodeid);
                } else {
                    cswPublic.select.val(nodeid);
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
                    cswPrivate.nodeTextCell = cswPrivate.table.cell(1, cswPrivate.cellCol);
                    cswPrivate.nodeText = cswPrivate.nodeTextCell.span({
                        text: cswPrivate.selectedName
                    });
                } else if (cswPrivate.isReadOnly) {
                    cswPrivate.nodeLinkTextCell = cswPrivate.table.cell(1, cswPrivate.cellCol);
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

