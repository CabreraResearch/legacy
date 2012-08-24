/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeQuantity';

    var methods = {
        init: function (o) {

            var isMultiEditValid = function (value) {
                return o.Multi && value === Csw.enums.multiEditDefaultValue;
            }

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values,
                precision = Csw.number(propVals.precision, 6),
                ceilingVal = '999999999' + Csw.getMaxValueForPrecision(precision),
                selectedNodeId = (false === o.Multi) ? Csw.string(propVals.relatednodeid).trim() : Csw.enums.multiEditDefaultValue,
                selectedName = (false === o.Multi) ? Csw.string(propVals.name).trim() : Csw.enums.multiEditDefaultValue,
                nodeTypeId = Csw.string(propVals.nodetypeid).trim(),
                objectClassId = Csw.string(propVals.objectclassid).trim(),
                options = propVals.options,
                relationships = [],
                fractional = Csw.bool(propVals.fractional),
                cellCol = 1;

            if (false === fractional) {
                precision = 0;
            }
            if (Csw.bool(o.propData.readonly)) {
                propDiv.span({ text: o.propData.gestalt });
            } else {

                if (false === Csw.isNullOrEmpty(o.relatednodeid) &&
                    Csw.isNullOrEmpty(selectedNodeId) &&
                    false === o.Multi &&
                    (Csw.number(o.relatednodetypeid) === Csw.number(nodeTypeId) ||
                      Csw.number(o.relatedobjectclassid) === Csw.number(objectClassId))) {
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
                    Precision: 6, //case 24646 - precision is being handled in the validator below, so we don't want to use the one in numberTextBox.
                    ReadOnly: Csw.bool(o.ReadOnly),
                    Required: Csw.bool(o.Required),
                    onChange: o.onChange,
                    isValid: isMultiEditValid
                });
                cellCol++;

                if (false === Csw.isNullOrEmpty(numberTextBox) && numberTextBox.length > 0) {
                    numberTextBox.clickOnEnter(o.saveBtn);
                }

                if (o.Multi) {
                    relationships.push({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                }
                if (false === o.Required) {
                    relationships.push({ value: '', display: '', frac: true });
                }
                var foundSelected = false;
                Csw.crawlObject(options, function (relatedObj) {
                    if (relatedObj.id === selectedNodeId) {
                        foundSelected = true;
                        fractional = Csw.bool(relatedObj.fractional);
                    }
                    relationships.push({ value: relatedObj.id, display: relatedObj.value, frac: Csw.bool(relatedObj.fractional) });
                }, false);
                if (false === o.Multi && false === foundSelected && false === Csw.isNullOrEmpty(selectedNodeId)) {
                    relationships.push({ value: selectedNodeId, display: selectedName, frac: Csw.bool(propVals.fractional) });
                }
                var selectBox = table.cell(1, cellCol).select({
                    ID: o.ID,
                    cssclass: 'selectinput',
                    onChange: function () {
                        Csw.crawlObject(options, function (relatedObj) {
                            if (relatedObj.id === selectBox.val()) {
                                fractional = Csw.bool(relatedObj.fractional);
                            }
                        }, false);
                        precision = false === fractional ? 0 : Csw.number(propVals.precision, 6);
                        o.onChange();
                    },
                    values: relationships,
                    selected: selectedNodeId
                });
                cellCol++;

                if (o.Required) {
                    selectBox.addClass('required');
                    numberTextBox.addClass('required');
                }

                $.validator.addMethod('validateInteger', function (value, element) {
                    return (isMultiEditValid(value) || precision != 0 || Csw.validateInteger(numberTextBox.val()));
                }, 'Value must be a whole number');
                numberTextBox.addClass('validateInteger');

                $.validator.addMethod('validateIntegerGreaterThanZero', function (value, element) {
                    return (isMultiEditValid(value) || Csw.validateIntegerGreaterThanZero(numberTextBox.val()));
                }, 'Value must be a non-zero, positive number');
                numberTextBox.addClass('validateIntegerGreaterThanZero');

                $.validator.addMethod('validateUnitPresent', function (value, element) {
                    return (isMultiEditValid(value) || false === Csw.isNullOrEmpty(selectBox.val()) || Csw.isNullOrEmpty(numberTextBox.val()));
                }, 'Unit must be selected if Quantity is present.');
                selectBox.addClass('validateUnitPresent');

                $.validator.addMethod('validateQuantityPresent', function (value, element) {
                    return (isMultiEditValid(value) || false === Csw.isNullOrEmpty(numberTextBox.val()) || Csw.isNullOrEmpty(selectBox.val()));
                }, 'Quantity must have a value if Unit is selected.');
                selectBox.addClass('validateQuantityPresent');

                propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, selectBox.val()); }, Csw.nodeHoverOut);
            }
        },
        save: function (o) {
            var attributes = {
                value: null,
                nodeid: null
            };
            var compare = {};
            if (false === Csw.bool(o.propData.readonly)) {
                var propDiv = o.propDiv.find('#' + o.ID + '_qty');
                if (false == Csw.isNullOrEmpty(propDiv)) {
                    attributes.value = propDiv.val();
                    compare = attributes;
                }

                var selectBox = o.propDiv.find('select');
                if (false === Csw.isNullOrEmpty(selectBox)) {
                    attributes.nodeid = selectBox.val();
                    compare = attributes;
                }
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
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
