/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    $.fn.CswFieldTypeRelationship = function (method) {

        var pluginName = 'CswFieldTypeRelationship';

        var methods = {
            init: function (o) {

                var propDiv = o.propDiv;
                propDiv.empty();
                var propVals = o.propData.values,
                    selectedNodeId = (false === o.Multi) ? Csw.string(propVals.relatednodeid).trim() : Csw.enums.multiEditDefaultValue,
                    selectedName = (false === o.Multi) ? Csw.string(propVals.name).trim() : Csw.enums.multiEditDefaultValue,
                    nodeTypeId = Csw.string(propVals.nodetypeid).trim(),
                    objectClassId = Csw.string(propVals.objectclassid).trim(),
                    allowAdd = Csw.bool(propVals.allowadd),
                    options = propVals.options,
                    useSearch = Csw.bool(propVals.usesearch),
                    relationships = [],
                    cellCol = 1,
                    selectedNodeType = {},
                    addImage = {},
                    onAddNodeFunc = {};

                // Default to selected node as relationship value for new nodes being added
                if (false === Csw.isNullOrEmpty(o.relatednodeid) &&
                    Csw.isNullOrEmpty(selectedNodeId) &&
                    false === o.Multi &&
                    (Csw.number(o.relatednodetypeid) === Csw.number(nodeTypeId) ||
                      Csw.number(o.relatedobjectclassid) === Csw.number(objectClassId))) {

                    selectedNodeId = o.relatednodeid;
                    selectedName = o.relatednodename;
                }

                if (o.ReadOnly) {
                    propDiv.append(selectedName);
                    propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, selectedNodeId); },
                                    function (event) { Csw.nodeHoverOut(event, selectedNodeId); });
                } else {

                    var table = propDiv.table({
                        ID: Csw.makeId(o.ID, 'tbl')
                    });


                    if (useSearch) {
                        // Find value by using search in a dialog

                        var nameSpan = table.cell(1, cellCol).span({
                            ID: Csw.makeId(o.ID, '', 'selectedname'),
                            text: selectedName
                        });

                        var hiddenValue = table.cell(1, cellCol).input({
                            ID: Csw.makeId(o.ID, '', 'hiddenvalue'),
                            type: Csw.enums.inputTypes.hidden,
                            value: selectedNodeId
                        });
                        cellCol++;

                        //                        var dialogLink = table.cell(1, cellCol).a({
                        //                            ID: Csw.makeId(o.ID, '', 'searchlink'),
                        //                            text: 'Find',
                        table.cell(1, cellCol).icon({
                            iconType: Csw.enums.iconType.magglass,
                            hovertext: "Search " + o.propData.name,
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                $.CswDialog('SearchDialog', {
                                    propname: o.propData.name,
                                    nodetypeid: nodeTypeId,
                                    objectclassid: objectClassId,
                                    onSelectNode: function (nodeObj) {
                                        nameSpan.text(nodeObj.nodename);
                                        hiddenValue.val(nodeObj.nodeid);
                                    }
                                });
                            }
                        });
                        cellCol++;

                        propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, hiddenValue.val()); },
                                        function (event) { Csw.nodeHoverOut(event, hiddenValue.val()); });

                        onAddNodeFunc = function (nodeid, nodekey, nodename) {
                            nameSpan.text(nodename);
                            hiddenValue.val(nodeid);
                        };

                    } else {
                        // Select value in a selectbox

                        if (o.Multi) {
                            relationships.push({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                        }
                        var foundSelected = false;
                        Csw.crawlObject(options, function (relatedObj) {
                            if (relatedObj.id === selectedNodeId) {
                                foundSelected = true;
                            }
                            relationships.push({ value: relatedObj.id, display: relatedObj.value });
                        }, false);
                        if (false === o.Multi && false === foundSelected) {
                            // case 25820 - guarantee selected option appears
                            relationships.push({ value: selectedNodeId, display: selectedName });
                        }

                        var selectBox = table.cell(1, cellCol).select({
                            ID: o.ID,
                            name: o.ID,
                            cssclass: 'selectinput',
                            onChange: o.onChange,
                            values: relationships,
                            selected: selectedNodeId
                        });
                        cellCol++;

                        var nodeLinkText;
                        Csw.ajax.post({
                            urlMethod: 'GetNodeRef',
                            async: false,
                            data: { nodeId: selectedNodeId },
                            success: function (data) {
                                nodeLinkText = table.cell(1, cellCol).nodeLink({
                                    text: data.noderef
                                });
                            }
                        });
                        cellCol++;

                        var toggleButton = table.cell(1, cellCol).imageButton({
                            ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                            AlternateText: 'Edit',
                            ID: Csw.makeId(o.ID, 'toggle'),
                            onClick: function () {
                                selectBox.show();
                                toggleButton.hide();
                                nodeLinkText.hide();
                            }
                        });
                        cellCol++;

                        if (o.EditMode === Csw.enums.editMode.Add) {
                            selectBox.show();
                            toggleButton.hide();
                            nodeLinkText.hide();
                        } else {
                            selectBox.hide();
                        }

                        if (o.Required) {
                            selectBox.addClass("required");
                        }

                        onAddNodeFunc = function (nodeid, nodekey, nodename) {
                            selectBox.option({ value: nodeid, display: nodename });
                            selectBox.val(nodeid);
                        };

                        propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, selectBox.val()); },
                                        function (event) { Csw.nodeHoverOut(event, selectBox.val()); });
                    } //if-else(useSearch)
                    if (allowAdd) {

                        var openAddNodeDialog = function (nodetypeToAdd) {
                            $.CswDialog('AddNodeDialog', {
                                nodetypeid: nodetypeToAdd,
                                onAddNode: onAddNodeFunc,
                                text: o.propData.name
                            });
                        };

                        var getNodeTypeOptions = function () {
                            var blankText = '[Select One]';
                            selectedNodeType = table.cell(1, cellCol)
                                .nodeTypeSelect({
                                    objectClassId: objectClassId,
                                    onSelect: function (data, nodeTypeCount) {
                                        if (blankText !== selectedNodeType.val()) {
                                            openAddNodeDialog(selectedNodeType.val());
                                        }
                                    },
                                    onSuccess: function (data, nodeTypeCount, lastNodeTypeId) {
                                        if (Csw.number(nodeTypeCount) > 1) {
                                            selectedNodeType.show();
                                            addImage.hide();
                                        }
                                        if (nodeTypeCount === 1 && false === Csw.isNullOrEmpty(selectedNodeType)) {
                                            openAddNodeDialog(lastNodeTypeId);
                                        }
                                    },
                                    blankOptionText: blankText,
                                    filterToPermission: 'Create'
                                })
                                .hide();
                            cellCol++;
                        };

                        var makeAddImage = function (nodeTypeCount, lastNodeTypeId) {
                            addImage = table.cell(1, cellCol).div()
                                .buttonExt({
                                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                                    size: 'small',
                                    tooltip: { title: 'Add New ' + o.propData.name },
                                    onClick: function () {
                                        if (false === Csw.isNullOrEmpty(nodeTypeId)) {
                                            openAddNodeDialog(nodeTypeId);
                                        }
                                        else {
                                            getNodeTypeOptions();
                                        }
                                    }
                                });
                            cellCol++;
                        };

                        makeAddImage();

                    } //if (allowAdd)
                } // if-else (o.ReadOnly) {
            }, // init
            save: function (o) {
                var attributes = {
                    nodeid: null
                };
                var compare = {};
                var propVals = o.propData.values;
                var useSearch = Csw.bool(propVals.usesearch);

                if (useSearch) {
                    var hiddenValue = o.propDiv.find('input');
                    if (false === Csw.isNullOrEmpty(hiddenValue)) {
                        attributes.nodeid = hiddenValue.val();
                        compare = attributes;
                    }
                } else {
                    var selectBox = o.propDiv.find('select');
                    if (false === Csw.isNullOrEmpty(selectBox)) {
                        attributes.nodeid = selectBox.val();
                        compare = attributes;
                    }
                }
                Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
            } // save
        }; // methods

        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
