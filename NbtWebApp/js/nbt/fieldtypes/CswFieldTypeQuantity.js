/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeQuantity';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values,
                precision = Csw.number(propVals.precision, 6),
                ceilingVal = '999999999' + Csw.getMaxValueForPrecision(precision),
                selectedNodeId = (false === o.Multi) ? Csw.string(propVals.relatednodeid).trim() : Csw.enums.multiEditDefaultValue,
                selectedName = (false === o.Multi) ? Csw.string(propVals.name).trim() : Csw.enums.multiEditDefaultValue,
                nodeTypeId = Csw.string(propVals.nodetypeid).trim(),
                options = propVals.options,
                relationships = [],
                cellCol = 1;

            if (false === Csw.isNullOrEmpty(o.relatednodeid) &&
                    Csw.isNullOrEmpty(selectedNodeId) &&
                    false === o.Multi &&
                    o.relatednodetypeid === nodeTypeId) {
                selectedNodeId = o.relatednodeid;
                selectedName = o.relatednodename;
            }

            var table = propDiv.table({
                ID: Csw.makeId(o.ID, 'tbl')
            });

            var numberTextBox = table.cell(1, cellCol).numberTextBox({
                ID: o.ID + '_qty',
                value: (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue,
                MinValue: Csw.number(propVals.minvalue),
                MaxValue: Csw.number(propVals.maxvalue),
                ceilingVal: Csw.number(ceilingVal),
                Precision: precision,
                ReadOnly: Csw.bool(o.ReadOnly),
                Required: Csw.bool(o.Required),
                onChange: o.onChange
            });
            cellCol++;

            if (false === Csw.isNullOrEmpty(numberTextBox) && numberTextBox.length > 0) {
                numberTextBox.clickOnEnter(o.saveBtn);
            }

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
        },
        save: function (o) {

            var attributes = {
                value: o.propDiv.find('#' + o.ID + '_qty').val(),
                nodeid: null
            };

            var selectBox = o.propDiv.find('select');
            if (false === Csw.isNullOrEmpty(selectBox)) {
                attributes.nodeid = selectBox.val();
            }
            
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeQuantity = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
