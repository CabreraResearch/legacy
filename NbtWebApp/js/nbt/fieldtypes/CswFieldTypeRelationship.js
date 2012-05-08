/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
                    cellCol = 1;

                // Default to selected node as relationship value for new nodes being added
                if (false === Csw.isNullOrEmpty(o.relatednodeid) &&
                    Csw.isNullOrEmpty(selectedNodeId) &&
                    false === o.Multi &&
                    (o.relatednodetypeid === nodeTypeId ||
                      o.relatedobjectclassid === objectClassId)) {

                    selectedNodeId = o.relatednodeid;
                    selectedName = o.relatednodename;
                }

                if (o.ReadOnly) {
                    propDiv.append(selectedName);
                    propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, selectedNodeId); }, Csw.nodeHoverOut);
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
                        table.cell(1, cellCol).imageButton({
                            ButtonType: Csw.enums.imageButton_ButtonType.View,
                            AlternateText: "Search " + o.propData.name,
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

                        propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, hiddenValue.val()); }, Csw.nodeHoverOut);

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
                            cssclass: 'selectinput',
                            onChange: o.onChange,
                            values: relationships,
                            selected: selectedNodeId
                        });
                        cellCol++;

                        if (o.Required) {
                            selectBox.addClass("required");
                        }

                        propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, selectBox.val()); }, Csw.nodeHoverOut);
                    }
                    if (allowAdd) {
                        if (false === Csw.isNullOrEmpty(nodeTypeId)) {
                            // Add new value
                            table.cell(1, cellCol)
                            .div()
                            .imageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Add,
                                AlternateText: "Add New " + o.propData.name,
                                onClick: function () {
                                    $.CswDialog('AddNodeDialog', {
                                        'nodetypeid': nodeTypeId,
                                        'onAddNode': o.onReload
                                    });
                                }
                            });
                            cellCol++;
                        }
                        else if (false === Csw.isNullOrEmpty(objectClassId)) {
                            //case 25721
                            var selectedNodeType = table.cell(1, cellCol)
                                .nodeTypeSelect({ objectClassId: objectClassId });
                            cellCol++;
                            table.cell(1, cellCol)
                            .div()
                            .imageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Add,
                                AlternateText: "Add New " + o.propData.name,
                                onClick: function () {
                                    $.CswDialog('AddNodeDialog', {
                                        'nodetypeid': selectedNodeType.val(),
                                        'onAddNode': o.onReload
                                    });
                                }
                            });
                        }
                    }//if (allowAdd)
                } // if-else (o.ReadOnly) {
            }, // init
            save: function (o) {
                var attributes = {
                    nodeid: null
                };
                var propVals = o.propData.values;
                var useSearch = Csw.bool(propVals.usesearch);

                if (useSearch) {
                    var hiddenValue = o.propDiv.find('input');
                    if (false === Csw.isNullOrEmpty(hiddenValue)) {
                        attributes.nodeid = hiddenValue.val();
                    }
                } else {
                    var selectBox = o.propDiv.find('select');
                    if (false === Csw.isNullOrEmpty(selectBox)) {
                        attributes.nodeid = selectBox.val();
                    }
                }
                Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
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
