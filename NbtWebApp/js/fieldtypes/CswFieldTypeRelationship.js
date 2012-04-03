/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    $.fn.CswFieldTypeRelationship = function (method) {

        var pluginName = 'CswFieldTypeRelationship';

        var methods = {
            init: function (o) {

                var propDiv = o.propDiv;
                propDiv.empty();
                var propVals = o.propData.values,
                    selectedNodeId = (false === o.Multi) ? Csw.string(propVals.nodeid).trim() : Csw.enums.multiEditDefaultValue,
                    selectedName = (false === o.Multi) ? Csw.string(propVals.name).trim() : Csw.enums.multiEditDefaultValue,
                    nodeTypeId = Csw.string(propVals.nodetypeid).trim(),
                    allowAdd = Csw.bool(propVals.allowadd),
                    options = propVals.options,
                    relationships = [];

                if (false === Csw.isNullOrEmpty(o.relatednodeid) && Csw.isNullOrEmpty(selectedNodeId) && false === o.Multi) {
                    selectedNodeId = o.relatednodeid;
                }

                if (o.Multi) {
                    relationships.push({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                }
                Csw.crawlObject(options, function (relatedObj) {
                    relationships.push({ value: relatedObj.id, display: relatedObj.value });
                }, false);

                if (o.ReadOnly) {
                    propDiv.append(selectedName);
                    propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, selectedNodeId); }, Csw.nodeHoverOut);
                } else {
                    var table = propDiv.table({
                        ID: Csw.controls.dom.makeId(o.ID, 'tbl')
                    });

                    var selectBox = table.cell(1, 1).select({
                        ID: o.ID,
                        cssclass: 'selectinput',
                        onChange: o.onChange,
                        values: relationships,
                        selected: selectedNodeId
                    });

                    if (false === Csw.isNullOrEmpty(nodeTypeId) && allowAdd) {

                        table.cell(1, 2)
                            .div()
                            .imageButton({
                                ButtonType: Csw.enums.imageButton_ButtonType.Add,
                                AlternateText: "Add New",
                                onClick: function () {
                                    $.CswDialog('AddNodeDialog', {
                                        'nodetypeid': nodeTypeId,
                                        'onAddNode': function () {
                                            o.onReload();
                                        }
                                    });
                                }
                            });
                    }

                    if (o.Required) {
                        selectBox.addClass("required");
                    }

                    propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, selectBox.val()); }, Csw.nodeHoverOut);
                }
            },
            save: function (o) {
                var attributes = {
                    nodeid: null
                };
                var nodeId = o.propDiv.find('select');
                if (false === Csw.isNullOrEmpty(nodeId)) {
                    attributes.nodeid = nodeId.val();
                }
                Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
            }
        };

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
