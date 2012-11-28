/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.controls.nodeSelect = Csw.controls.nodeSelect ||
        Csw.controls.register('nodeSelect', function (cswParent, cswPrivate) {
            'use strict';
            
            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.$parent = cswPrivate.$parent || cswParent.$;
                cswPrivate.name = cswPrivate.name || '';
                cswPrivate.async = cswPrivate.async; // || true;
                cswPrivate.nodesUrlMethod = cswPrivate.nodesUrlMethod || 'Nodes/get';

                cswPrivate.labelText = cswPrivate.labelText || null;
                cswPrivate.excludeNodeTypeIds = cswPrivate.excludeNodeTypeIds || '';
                cswPrivate.selectedNodeId = cswPrivate.selectedNodeId || '';
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
                
                cswPrivate.onSelectNode = cswPrivate.onSelectNode || function() {};
                cswPrivate.onSuccess = cswPrivate.onSuccess || function () { };
                
                cswPrivate.addNewOption = cswPrivate.addNewOption; // || false;
                cswPrivate.allowAdd = cswPrivate.allowAdd; // || false;
                cswPrivate.isRequired = cswPrivate.isRequired; // || false;
                cswPrivate.isMulti = cswPrivate.isMulti; // || false;
                cswPrivate.isReadOnly = cswPrivate.isReadOnly; // || false;
                cswPrivate.showSelectOnLoad = cswPrivate.showSelectOnLoad; // || true;
                
                cswPrivate.relationships = cswPrivate.relationships || [];
                
                cswPublic = cswParent.table();

                // Default to selected node as relationship value for new nodes being added
                if (false === Csw.isNullOrEmpty(cswPrivate.relatedTo.relatednodeid) &&
                    Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                    false === cswPrivate.isMulti &&
                    (Csw.number(cswPrivate.relatedTo.relatednodetypeid) === Csw.number(cswPrivate.nodeTypeId) ||
                      Csw.number(cswPrivate.relatedTo.relatedobjectclassid) === Csw.number(cswPrivate.objectClassId))) {

                    cswPrivate.selectedNodeId = cswPrivate.relatedTo.relatednodeid;
                    cswPrivate.selectedName = cswPrivate.relatedTo.relatednodename;
                }


            }());
            
            cswPrivate.getNodes = function() {
                cswPublic.bind('change', function () {
                    cswPrivate.selectedNodeId = cswPublic.val();
                    Csw.tryExec(cswPrivate.onChange, cswPublic);
                    Csw.tryExec(cswPrivate.onSelect, cswPublic.val());
                });

                cswPublic.selectedNodeId = function () {
                    return cswPrivate.selectedNodeId || cswPublic.val();
                };

                Csw.ajaxWcf.post({
                    urlMethod: cswPrivate.nodesUrlMethod,
                    async: Csw.bool(cswPrivate.async),
                    data: {
                        NodeTypeId: Csw.number(cswPrivate.nodeTypeId, 0),
                        ObjectClassId: Csw.number(cswPrivate.objectClassId, 0),
                        ObjectClass: Csw.string(cswPrivate.objectClassName),
                        RelatedToObjectClass: Csw.string(cswPrivate.relatedTo.objectClassName),
                        RelatedToNodeId: Csw.string(cswPrivate.relatedTo.nodeId),
                        ViewId: Csw.string(cswPrivate.viewid)
                    },
                    success: function (data) {
                        cswPrivate.relationships = data.Nodes;
                        cswPrivate.canAdd = Csw.bool(cswPrivate.canAdd) && Csw.bool(data.CanAdd);
                        cswPrivate.useSearch = Csw.bool(data.UseSearch);
                        cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || data.NodeTypeId;
                        cswPrivate.objectClassId = cswPrivate.objectClassId || data.ObjectClassId;
                        cswPrivate.relatedTo.objectClassId = cswPrivate.relatedTo.objectClassId || data.RelatedToObjectClassId;

                        Csw.tryExec(cswPrivate.onSuccess, data);
                        cswPublic.css('width', Csw.string(cswPrivate.width));

                        cswPrivate.makeControl();
                    }
                });
            };

            cswPrivate.makeControl = function() {
                if (cswPrivate.useSearch) {
                    cswPrivate.makeSearch();
                } else {
                    cswPrivate.makeSelect();
                }
                cswPrivate.makeAdd();
            };

            cswPrivate.makeSelect = function() {
                // Select value in a selectbox
                cswPrivate.foundSelected = false;

                Csw.eachRecursive(cswPrivate.options, function (relatedObj) {
                    if (false === cswPrivate.isMulti && relatedObj.id === cswPrivate.selectedNodeId) {
                        cswPrivate.foundSelected = true;
                    }
                    cswPrivate.relationships.push({ value: relatedObj.id, display: relatedObj.value });
                }, false);
                if (false === cswPrivate.isMulti && false === cswPrivate.foundSelected) {
                    // case 25820 - guarantee selected option appears
                    cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName });
                }

                cswPrivate.selectBox = cswPublic.cell(1, cswPrivate.cellCol).select({
                    name: cswPrivate.name,
                    cssclass: 'selectinput',
                    onChange: function () {
                        var val = cswPrivate.selectBox.val();
                        Csw.tryExec(cswPrivate.onSelectNode, { nodeid: val });
                    },
                    values: cswPrivate.relationships,
                    selected: cswPrivate.selectedNodeId
                });

                cswPrivate.cellCol += 1;
                cswPrivate.nodeLinkText = cswPublic.cell(1, cswPrivate.cellCol);
                cswPrivate.cellCol += 1;
                if (false === cswPrivate.isMulti) {
                    cswPrivate.nodeLinkText = cswPrivate.nodeLinkText.nodeLink({
                        text: cswPrivate.selectedNodeLink
                    });
                }

                cswPrivate.toggleButton = cswPublic.cell(1, cswPrivate.cellCol).icon({
                    iconType: Csw.enums.iconType.pencil,
                    isButton: true,
                    onClick: function () {
                        cswPrivate.toggleOptions(true);
                    }
                });
                cswPrivate.cellCol += 1;

                cswPrivate.toggleOptions = function (on) {
                    if (Csw.bool(on)) {
                        cswPrivate.selectBox.show();
                        cswPrivate.toggleButton.hide();
                        cswPrivate.nodeLinkText.hide();
                    } else {
                        cswPrivate.selectBox.hide();
                        cswPrivate.toggleButton.show();
                        cswPrivate.nodeLinkText.show();
                    }
                };

                cswPrivate.toggleOptions(cswPrivate.showSelectOnLoad);

                cswPrivate.selectBox.required(cswPrivate.isRequired);

                cswPrivate.onAddNodeFunc = function (nodeid, nodekey, nodename) {
                    cswPrivate.selectBox.option({ value: nodeid, display: nodename });
                    cswPrivate.selectBox.val(nodeid);
                    cswPrivate.toggleOptions(true);
                    Csw.tryExec(cswPrivate.onSelectNode, { nodeid: nodeid });
                    cswPrivate.selectBox.$.valid();
                };

                cswPrivate.nodeLinkText.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectBox.val()); },
                                function (event) { Csw.nodeHoverOut(event, cswPrivate.selectBox.val()); });
            };

            cswPrivate.makeSearch = function() {
                if (cswPrivate.useSearch) {
                    // Find value by using search in a dialog

                    cswPrivate.nameSpan = cswPublic.cell(1, cswPrivate.cellCol).span({
                        name: 'selectedname',
                        text: cswPrivate.selectedName
                    });

                    cswPrivate.hiddenValue = cswPublic.cell(1, cswPrivate.cellCol).input({
                        name: 'hiddenvalue',
                        type: Csw.enums.inputTypes.hidden,
                        value: cswPrivate.selectedNodeId
                    });
                    cswPrivate.cellCol += 1;

                    cswPublic.cell(1, cswPrivate.cellCol).icon({
                        iconType: Csw.enums.iconType.magglass,
                        hovertext: "Search " + cswPrivate.name,
                        size: 16,
                        isButton: true,
                        onClick: function() {
                            $.CswDialog('SearchDialog', {
                                propname: cswPrivate.name,
                                nodetypeid: cswPrivate.nodeTypeId,
                                objectclassid: cswPrivate.objectClassId,
                                onSelectNode: function(nodeObj) {
                                    cswPrivate.nameSpan.text(nodeObj.nodename);
                                    cswPrivate.hiddenValue.val(nodeObj.nodeid);
                                    Csw.tryExec(cswPrivate.onSelectNode);
                                }
                            });
                        }
                    });
                    cswPrivate.cellCol += 1;

                    cswPrivate.nameSpan.$.hover(function(event) { Csw.nodeHoverIn(event, cswPrivate.hiddenValue.val()); },
                        function(event) { Csw.nodeHoverOut(event, cswPrivate.hiddenValue.val()); });

                    cswPrivate.onAddNodeFunc = function(nodeid, nodekey, nodename) {
                        cswPrivate.nameSpan.text(nodename);
                        cswPrivate.hiddenValue.val(nodeid);
                    };

                }
            };

            cswPrivate.makeAdd = function() {
                if (cswPrivate.allowAdd) {

                    cswPrivate.openAddNodeDialog = function (nodetypeToAdd) {
                        $.CswDialog('AddNodeDialog', {
                            nodetypeid: nodetypeToAdd,
                            onAddNode: cswPrivate.onAddNodeFunc,
                            text: cswPrivate.name
                        });
                    };

                    cswPrivate.getNodeTypeOptions = function () {
                        cswPrivate.blankText = '[Select One]';
                        cswPrivate.selectedNodeType = cswPublic.cell(1, cswPrivate.cellCol)
                            .nodeTypeSelect({
                                objectClassId: cswPrivate.objectClassId,
                                onSelect: function () {
                                    if (cswPrivate.blankText !== cswPrivate.selectedNodeType.val()) {
                                        cswPrivate.openAddNodeDialog(cswPrivate.selectedNodeType.val());
                                    }
                                },
                                onSuccess: function (data, nodeTypeCount, lastNodeTypeId) {
                                    if (Csw.number(nodeTypeCount) > 1) {
                                        cswPrivate.selectedNodeType.show();
                                        cswPrivate.addImage.hide();
                                    }
                                    if (nodeTypeCount === 1 && false === Csw.isNullOrEmpty(cswPrivate.selectedNodeType)) {
                                        cswPrivate.openAddNodeDialog(lastNodeTypeId);
                                    }
                                },
                                blankOptionText: cswPrivate.blankText,
                                filterToPermission: 'Create'
                            })
                            .hide();
                        cswPrivate.cellCol += 1;
                    };

                    cswPrivate.makeAddImage = function () {
                        cswPrivate.addImage = cswPublic.cell(1, cswPrivate.cellCol).div()
                            .buttonExt({
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                                size: 'small',
                                tooltip: { title: 'Add New ' + cswPrivate.name },
                                onClick: function () {
                                    if (false === Csw.isNullOrEmpty(cswPrivate.nodeTypeId)) {
                                        cswPrivate.openAddNodeDialog(cswPrivate.nodeTypeId);
                                    }
                                    else {
                                        cswPrivate.getNodeTypeOptions();
                                    }
                                }
                            });
                        cswPrivate.cellCol += 1;
                    };

                    cswPrivate.makeAddImage();

                } //if (allowAdd)
            };

            (function _relationship() {
                if (cswPrivate.isReadOnly) {
                    cswPrivate.nodeLinkTextCell = cswPublic.cell(1, cswPrivate.cellCol);
                    cswPrivate.nodeLinkText = cswPrivate.nodeLinkTextCell.nodeLink({
                        text: cswPrivate.selectedNodeLink
                    });
                    cswPrivate.parent.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectedNodeId); },
                                    function (event) { Csw.nodeHoverOut(event, cswPrivate.selectedNodeId); });
                } else {
                    if (cswPrivate.relationships.length > 0) {
                        cswPrivate.makeControl();
                    } else {
                        cswPrivate.getNodes();
                    }
                } // if-else (o.ReadOnly) {
            }());

            return cswPublic;
        });
} ());

