/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.relationship = Csw.properties.relationship ||
        Csw.properties.register('relationship',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption || Csw.nbt.propertyOption(propertyOption)
                };

                var render = function () {
                    'use strict';

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.selectedNodeId = (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.relatednodeid).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.selectedName = (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.name).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.nodeTypeId = Csw.string(cswPrivate.propVals.nodetypeid).trim();
                    cswPrivate.objectClassId = Csw.string(cswPrivate.propVals.objectclassid).trim();
                    cswPrivate.allowAdd = Csw.bool(cswPrivate.propVals.allowadd);
                    cswPrivate.options = cswPrivate.propVals.options;
                    cswPrivate.useSearch = Csw.bool(cswPrivate.propVals.usesearch);
                    cswPrivate.relationships = [];
                    cswPrivate.cellCol = 1;
                    cswPrivate.selectedNodeType = { };
                    cswPrivate.addImage = { };
                    cswPrivate.onAddNodeFunc = { };

                    // Default to selected node as relationship value for new nodes being added
                    if (false === Csw.isNullOrEmpty(cswPublic.data.relatednodeid) &&
                        Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                        false === cswPublic.data.Multi &&
                        (Csw.number(cswPublic.data.relatednodetypeid) === Csw.number(cswPrivate.nodeTypeId) ||
                          Csw.number(cswPublic.data.relatedobjectclassid) === Csw.number(cswPrivate.objectClassId))) {

                        cswPrivate.selectedNodeId = cswPublic.data.relatednodeid;
                        cswPrivate.selectedName = cswPublic.data.relatednodename;
                    }

                    if (cswPublic.data.ReadOnly) {
                        cswPublic.control = cswPrivate.parent.append(cswPrivate.selectedName);
                        cswPrivate.parent.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectedNodeId); },
                                        function (event) { Csw.nodeHoverOut(event, cswPrivate.selectedNodeId); });
                    } else {

                        cswPublic.control = cswPrivate.parent.table({
                            ID: Csw.makeId(cswPublic.data.ID, window.Ext.id())
                        });


                        if (cswPrivate.useSearch) {
                            // Find value by using search in a dialog

                            cswPrivate.nameSpan = cswPublic.control.cell(1, cswPrivate.cellCol).span({
                                ID: Csw.makeId(cswPublic.data.ID, '', 'selectedname'),
                                text: cswPrivate.selectedName
                            });

                            cswPrivate.hiddenValue = cswPublic.control.cell(1, cswPrivate.cellCol).input({
                                ID: Csw.makeId(cswPublic.data.ID, '', 'hiddenvalue'),
                                type: Csw.enums.inputTypes.hidden,
                                value: cswPrivate.selectedNodeId
                            });
                            cswPrivate.cellCol += 1;

                            cswPublic.control.cell(1, cswPrivate.cellCol).icon({
                                ID: Csw.makeId(cswPublic.data.ID,window.Ext.id()),
                                iconType: Csw.enums.iconType.magglass,
                                hovertext: "Search " + cswPublic.data.propData.name,
                                size: 16,
                                isButton: true,
                                onClick: function () {
                                    $.CswDialog('SearchDialog', {
                                        propname: cswPublic.data.propData.name,
                                        nodetypeid: cswPrivate.nodeTypeId,
                                        objectclassid: cswPrivate.objectClassId,
                                        onSelectNode: function (nodeObj) {
                                            cswPrivate.nameSpan.text(nodeObj.nodename);
                                            cswPrivate.hiddenValue.val(nodeObj.nodeid);
                                            Csw.tryExec(cswPublic.data.onChange, nodeObj.nodeid);
                                            cswPublic.data.onPropChange({ nodeid: nodeObj.nodeid });
                                        }
                                    });
                                }
                            });
                            cswPrivate.cellCol += 1;

                            cswPrivate.parent.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.hiddenValue.val()); },
                                            function (event) { Csw.nodeHoverOut(event, cswPrivate.hiddenValue.val()); });

                            cswPrivate.onAddNodeFunc = function (nodeid, nodekey, nodename) {
                                cswPrivate.nameSpan.text(nodename);
                                cswPrivate.hiddenValue.val(nodeid);
                            };

                        } else {
                            // Select value in a selectbox

                            if (cswPublic.data.Multi) {
                                cswPrivate.relationships.push({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                            }
                            cswPrivate.foundSelected = false;
                            Csw.crawlObject(cswPrivate.options, function (relatedObj) {
                                if (relatedObj.id === cswPrivate.selectedNodeId) {
                                    cswPrivate.foundSelected = true;
                                }
                                cswPrivate.relationships.push({ value: relatedObj.id, display: relatedObj.value });
                            }, false);
                            if (false === cswPublic.data.Multi && false === cswPrivate.foundSelected) {
                                // case 25820 - guarantee selected option appears
                                cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName });
                            }

                            cswPrivate.selectBox = cswPublic.control.cell(1, cswPrivate.cellCol).select({
                                ID: Csw.makeId(cswPublic.data.ID,window.Ext.id()),
                                name: cswPublic.data.ID,
                                cssclass: 'selectinput',
                                onChange: function () {
                                    var val = cswPrivate.selectBox.val();
                                    Csw.tryExec(cswPublic.data.onChange, val);
                                    cswPublic.data.onPropChange({ nodeid: val });
                                },
                                values: cswPrivate.relationships,
                                selected: cswPrivate.selectedNodeId
                            });
                            
                            cswPrivate.cellCol += 1;

                            cswPrivate.nodeLinkText = '';
                            Csw.ajax.post({
                                urlMethod: 'GetNodeRef',
                                async: false,
                                data: { nodeId: cswPrivate.selectedNodeId },
                                success: function (data) {
                                    cswPrivate.nodeLinkText = cswPublic.control.cell(1, cswPrivate.cellCol).nodeLink({
                                        text: data.noderef
                                    });
                                }
                            });
                            cswPrivate.cellCol += 1;

                            cswPrivate.toggleButton = cswPublic.control.cell(1, cswPrivate.cellCol).icon({
                                iconType: Csw.enums.iconType.pencil,
                                isButton: true,
                                ID: Csw.makeId(cswPublic.data.ID,window.Ext.id()),
                                onClick: function () {
                                    cswPrivate.toggleOptions(true);
                                }
                            });
                            cswPrivate.cellCol += 1;

                            cswPrivate.toggleOptions = function(on) {
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

                            cswPrivate.toggleOptions(cswPublic.data.EditMode === Csw.enums.editMode.Add || (cswPublic.data.Required && Csw.isNullOrEmpty(cswPrivate.selectedNodeId)));
                                                                                                                                                                                      
                            if (cswPublic.data.Required) {
                                cswPrivate.selectBox.addClass("required");
                            }

                            cswPrivate.onAddNodeFunc = function (nodeid, nodekey, nodename) {
                                cswPrivate.selectBox.option({ value: nodeid, display: nodename });
                                cswPrivate.selectBox.val(nodeid);
                                cswPrivate.toggleOptions(true);
                                cswPublic.data.onPropChange({ nodeid: nodeid });
                                cswPrivate.selectBox.$.valid();
                            };

                            cswPrivate.parent.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectBox.val()); },
                                            function (event) { Csw.nodeHoverOut(event, cswPrivate.selectBox.val()); });
                        } //if-else(useSearch)
                        if (cswPrivate.allowAdd) {

                            cswPrivate.openAddNodeDialog = function (nodetypeToAdd) {
                                $.CswDialog('AddNodeDialog', {
                                    nodetypeid: nodetypeToAdd,
                                    onAddNode: cswPrivate.onAddNodeFunc,
                                    text: cswPublic.data.propData.name
                                });
                            };

                            cswPrivate.getNodeTypeOptions = function () {
                                cswPrivate.blankText = '[Select One]';
                                cswPrivate.selectedNodeType = cswPublic.control.cell(1, cswPrivate.cellCol)
                                    .nodeTypeSelect({
                                        objectClassId: cswPrivate.objectClassId,
                                        onSelect: function (data, nodeTypeCount) {
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

                            cswPrivate.makeAddImage = function (nodeTypeCount, lastNodeTypeId) {
                                cswPrivate.addImage = cswPublic.control.cell(1, cswPrivate.cellCol).div()
                                    .buttonExt({
                                        ID: Csw.makeId(cswPublic.data.ID,window.Ext.id()),
                                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                                        size: 'small',
                                        tooltip: { title: 'Add New ' + cswPublic.data.propData.name },
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
                    } // if-else (o.ReadOnly) {
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());        