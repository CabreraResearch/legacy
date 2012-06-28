/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    Csw.composites.thinGrid = Csw.composites.thinGrid ||
        Csw.composites.register('thinGrid', function (cswParent, options) {
            var cswPrivate = {
                propDiv: options.propDiv,
                propVals: options.propData.values,
                relationships: [],
                cellCol: 1
            };

            var cswPublic = {};

            cswPrivate.propDiv.empty();
            cswPrivate.precision = Csw.number(cswPrivate.propVals.precision, 6);
            cswPrivate.ceilingVal = '999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision);
            cswPrivate.selectedNodeId = (false === options.Multi) ? Csw.string(cswPrivate.propVals.relatednodeid).trim() : Csw.enums.multiEditDefaultValue;
            cswPrivate.selectedName = (false === options.Multi) ? Csw.string(cswPrivate.propVals.name).trim() : Csw.enums.multiEditDefaultValue;
            cswPrivate.nodeTypeId = Csw.string(cswPrivate.propVals.nodetypeid).trim();
            cswPrivate.options = cswPrivate.propVals.options;
            cswPrivate.fractional = Csw.bool(cswPrivate.propVals.fractional);

            if (false === cswPrivate.fractional) {
                cswPrivate.precision = 0;
            }
            if (Csw.bool(options.propData.readonly)) {
                cswPrivate.propDiv.span({ text: options.propData.gestalt });
            } else {

                if (false === Csw.isNullOrEmpty(options.relatednodeid) &&
                    Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                        false === options.Multi &&
                            options.relatednodetypeid === cswPrivate.nodeTypeId) {
                    cswPrivate.selectedNodeId = options.relatednodeid;
                    cswPrivate.selectedName = options.relatednodename;
                }

                cswPublic.table = cswPrivate.propDiv.table({
                    ID: Csw.makeId(options.ID, 'tbl')
                });

                cswPublic.quantityTextBox = cswPublic.table.cell(1, cswPrivate.cellCol).numberTextBox({
                    ID: options.ID + '_qty',
                    value: (false === options.Multi) ? Csw.string(cswPrivate.propVals.value).trim() : Csw.enums.multiEditDefaultValue,
                    MinValue: Csw.number(cswPrivate.propVals.minvalue),
                    MaxValue: Csw.number(cswPrivate.propVals.maxvalue),
                    ceilingVal: Csw.number(cswPrivate.ceilingVal),
                    Precision: 6, //case 24646 - precision is being handled in the validator below, so we don't want to use the one in numberTextBox.
                    ReadOnly: Csw.bool(options.ReadOnly),
                    Required: Csw.bool(options.Required),
                    onChange: function () {
                        cswPrivate.propVals.value = cswPublic.quantityTextBox.val();
                        options.propData.value = cswPrivate.propVals.value;
                        Csw.tryExec(options.onChange);
                    }
                });
                cswPrivate.cellCol += 1;

                if (false === Csw.isNullOrEmpty(cswPublic.quantityTextBox) && cswPublic.quantityTextBox.length > 0) {
                    cswPublic.quantityTextBox.clickOnEnter(options.saveBtn);
                }

                if (options.Multi) {
                    cswPrivate.relationships.push({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                }
                if (false === options.Required && false === Csw.isNullOrEmpty(cswPrivate.selectedName)) {
                    cswPrivate.relationships.push({ value: '', display: '', frac: true });
                }
                cswPrivate.foundSelected = false;
                Csw.crawlObject(cswPrivate.options, function (relatedObj) {
                    if (relatedObj.id === cswPrivate.selectedNodeId) {
                        cswPrivate.foundSelected = true;
                        cswPrivate.fractional = Csw.bool(relatedObj.fractional);
                    }
                    cswPrivate.relationships.push({ value: relatedObj.id, display: relatedObj.value, frac: Csw.bool(relatedObj.fractional) });
                }, false);
                if (false === options.Multi && false === cswPrivate.foundSelected) {
                    cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName, frac: Csw.bool(cswPrivate.propVals.fractional) });
                }
                cswPublic.unitSelect = cswPublic.table.cell(1, cswPrivate.cellCol).select({
                    ID: options.ID,
                    cssclass: 'selectinput',
                    onChange: function () {
                        Csw.crawlObject(cswPrivate.options, function (relatedObj) {
                            if (relatedObj.id === cswPublic.unitSelect.val()) {
                                cswPrivate.fractional = Csw.bool(relatedObj.fractional);
                            }
                        }, false);
                        cswPrivate.precision = false === cswPrivate.fractional ? 0 : Csw.number(cswPrivate.propVals.precision, 6);
                        options.onChange();
                        cswPrivate.selectedNodeId = cswPublic.unitSelect.val();
                        options.propData.nodeid = cswPrivate.selectedNodeId;
                    },
                    values: cswPrivate.relationships,
                    selected: cswPrivate.selectedNodeId
                });
                cswPrivate.cellCol += 1;

                if (options.Required) {
                    cswPublic.unitSelect.addClass('required');
                    cswPublic.quantityTextBox.addClass('required');
                }

                $.validator.addMethod('validateInteger', function (value, element) {
                    return (cswPrivate.precision != 0 || Csw.validateInteger(cswPublic.quantityTextBox.val()));
                }, 'Value must be an integer');
                cswPublic.quantityTextBox.addClass('validateInteger');

                $.validator.addMethod('validateUnitPresent', function (value, element) {
                    return (false === Csw.isNullOrEmpty(cswPublic.unitSelect.val()) || Csw.isNullOrEmpty(cswPublic.quantityTextBox.val()));
                }, 'Unit must be selected if Quantity is present.');
                cswPublic.unitSelect.addClass('validateUnitPresent');

                $.validator.addMethod('validateQuantityPresent', function (value, element) {
                    return (false === Csw.isNullOrEmpty(cswPublic.quantityTextBox.val()) || Csw.isNullOrEmpty(cswPublic.unitSelect.val()));
                }, 'Quantity must have a value if Unit is selected.');
                cswPublic.unitSelect.addClass('validateQuantityPresent');

                cswPrivate.propDiv.$.hover(function (event) { Csw.nodeHoverIn(event, cswPublic.unitSelect.val()); }, Csw.nodeHoverOut);
            }
            return cswPublic;
        });
})(jQuery);
