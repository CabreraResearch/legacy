/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    Csw.composites.quantity = Csw.composites.quantity ||
        Csw.composites.register('quantity', function (cswParent, options) {
            var cswPublic = {};
            var cswPrivate = {};

            Csw.tryExec(function () {
                cswPrivate = {
                    relationships: [],
                    fractional: true,
                    labelText: '',
                    maxvalue: NaN,
                    minvalue: NaN,
                    name: '',
                    nodeid: '',
                    nodetypeid: '',
                    options: [],
                    onChange: null,
                    precision: 6,
                    relatednodeid: '',
                    value: '',
                    cellCol: 1,
                    quantityText: '',
                    gestalt: '',
                    qtyWidth: '',
                    qtyReadonly: false,
                    unitReadonly: false
                };
                if (options) Csw.extend(cswPrivate, options);

                cswParent.empty();
                cswPrivate.precision = Csw.number(cswPrivate.precision, 6);
                cswPrivate.ceilingVal = '999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision);
                cswPrivate.selectedNodeId = Csw.string(cswPrivate.relatednodeid).trim();
                cswPrivate.selectedName = Csw.string(cswPrivate.name).trim();
                cswPrivate.nodeTypeId = Csw.string(cswPrivate.nodetypeid).trim();
                cswPrivate.fractional = Csw.bool(cswPrivate.fractional);
                cswPrivate.gestalt = cswPrivate.value + ' ' + cswPrivate.selectedName;

                if (false === cswPrivate.fractional) {
                    cswPrivate.precision = 0;
                }

                if (false === Csw.isNullOrEmpty(cswPrivate.relatednodeid) &&
                    Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                        false === cswPrivate.Multi &&
                            cswPrivate.relatednodetypeid === cswPrivate.nodeTypeId) {
                    cswPrivate.selectedNodeId = cswPrivate.relatednodeid;
                    cswPrivate.selectedName = cswPrivate.relatednodename;
                }

                cswPublic.table = cswParent.table({
                    ID: Csw.makeId(cswPrivate.ID, 'tbl')
                });

                var buildQuantityTextBox = function () {
                    cswPublic.quantityTextBox = cswPublic.table.cell(1, cswPrivate.cellCol).numberTextBox({
                        ID: cswPrivate.ID + '_qty',
                        labelText: cswPrivate.labelText,
                        useWide: cswPrivate.useWide,
                        width: cswPrivate.qtyWidth,
                        value: Csw.string(cswPrivate.value).trim(),
                        MinValue: Csw.number(cswPrivate.minvalue),
                        MaxValue: Csw.number(cswPrivate.maxvalue),
                        ceilingVal: Csw.number(cswPrivate.ceilingVal),
                        Precision: 6, //case 24646 - precision is being handled in the validator below, so we don't want to use the one in numberTextBox.
                        ReadOnly: Csw.bool(cswPrivate.qtyReadonly),
                        Required: Csw.bool(cswPrivate.Required),
                        onChange: function () {
                            var val = cswPublic.quantityTextBox.val();
                            cswPrivate.value = val;
                            cswPublic.quantityValue = val;
                            Csw.tryExec(cswPrivate.onChange, val);
                        }
                    });
                    cswPublic.quantityValue = Csw.bool(cswPrivate.qtyReadonly) ? cswPrivate.value : cswPublic.quantityTextBox.val();

                    cswPrivate.cellCol += 1;
                }

                var buildUnitSelect = function () {
                    if (Csw.bool(cswPrivate.unitReadonly)) {
                        cswPublic.unitVal = cswPrivate.selectedNodeId;
                        cswPublic.unitText = cswPrivate.selectedName;
                        cswPublic.unitSelectReaDOnly = cswPublic.table.cell(1, cswPrivate.cellCol).span({ text: cswPrivate.selectedName });
                    } else {
                        if (false === cswPrivate.Required && false === Csw.isNullOrEmpty(cswPrivate.selectedName)) {
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
                        if (false === cswPrivate.foundSelected) {
                            cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName, frac: cswPrivate.fractional });
                        }
                        cswPublic.unitSelect = cswPublic.table.cell(1, cswPrivate.cellCol).select({
                            ID: cswPrivate.ID,
                            cssclass: 'selectinput',
                            onChange: function () {
                                var val = cswPublic.unitSelect.val();
                                Csw.crawlObject(cswPrivate.options, function (relatedObj) {
                                    if (relatedObj.id === cswPublic.unitSelect.val()) {
                                        cswPrivate.fractional = Csw.bool(relatedObj.fractional);
                                    }
                                }, false);
                                cswPrivate.precision = false === cswPrivate.fractional ? 0 : Csw.number(cswPrivate.precision, 6);
                                cswPrivate.selectedNodeId = val;
                                cswPrivate.nodeid = val;
                                cswPublic.unitVal = val;
                                cswPublic.unitText = cswPublic.unitSelect.find(':selected').text();
                                Csw.tryExec(cswPrivate.onChange, val);
                            },
                            values: cswPrivate.relationships,
                            selected: cswPrivate.selectedNodeId
                        });
                        cswPublic.unitVal = cswPublic.unitSelect.val();
                        cswPublic.unitText = cswPublic.unitSelect.find(':selected').text();

                        cswPrivate.cellCol += 1;

                        if (cswPrivate.Required) {
                            cswPublic.unitSelect.addClass('required');
                            cswPublic.quantityTextBox.addClass('required');
                        }

                        $.validator.addMethod('validateInteger', function () {
                            return (cswPrivate.precision != 0 || Csw.validateInteger(cswPublic.quantityTextBox.val()));
                        }, 'Value must be an integer');
                        cswPublic.quantityTextBox.addClass('validateInteger');

                        if (false === Csw.bool(cswPrivate.unitReadonly)) {
                            $.validator.addMethod('validateUnitPresent', function () {
                                return (false === Csw.isNullOrEmpty(cswPublic.unitSelect.val()) || Csw.isNullOrEmpty(cswPublic.quantityTextBox.val()));
                            }, 'Unit must be selected if Quantity is present.');
                            cswPublic.unitSelect.addClass('validateUnitPresent');

                            $.validator.addMethod('validateQuantityPresent', function () {
                                return (false === Csw.isNullOrEmpty(cswPublic.quantityTextBox.val()) || Csw.isNullOrEmpty(cswPublic.unitSelect.val()));
                            }, 'Quantity must have a value if Unit is selected.');
                            cswPublic.unitSelect.addClass('validateQuantityPresent');
                        }

                        cswParent.$.hover(function (event) { Csw.nodeHoverIn(event, cswPublic.unitSelect.val()); }, Csw.nodeHoverOut);
                    }
                }

                buildQuantityTextBox();
                buildUnitSelect();

                cswPublic.refresh = function (data) {
                    cswPrivate.value = data.value;
                    cswPrivate.nodeid = data.nodeid;
                    cswPrivate.name = data.name;
                    cswPrivate.qtyReadonly = data.qtyReadonly;
                    cswPrivate.selectedNodeId = Csw.string(data.relatednodeid).trim();
                    cswPrivate.selectedName = Csw.string(data.name).trim();
                    cswPublic.table.empty();
                    buildQuantityTextBox();
                    buildUnitSelect();
                    Csw.tryExec(cswPrivate.onChange, cswPublic);
                };
            });
            return cswPublic;
        });
})(jQuery);
