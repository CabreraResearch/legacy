/// <reference path="~/app/CswApp-vsdoc.js" />


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
                    unitName: '',
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
                    unitReadonly: false,
                    excludeRangeLimits: false,
                    isRequired: false,
                    isReadOnly: false
                };
                if (options) Csw.extend(cswPrivate, options);

                cswPublic.control = cswPrivate.parent.table();

                cswPrivate.numberTextBox = cswPublic.control.cell(1, cswPrivate.cellCol).numberTextBox({
                    name: cswPrivate.name + '_qty',
                    value: Csw.string(cswPrivate.propVals.value).trim(),
                    MinValue: Csw.number(cswPrivate.propVals.minvalue),
                    MaxValue: Csw.number(cswPrivate.propVals.maxvalue),
                    excludeRangeLimits: Csw.bool(cswPrivate.propVals.excludeRangeLimits),
                    ceilingVal: Csw.number(cswPrivate.ceilingVal),
                    size: 6,
                    Precision: 6, //case 24646 - precision is being handled in the validator below, so we don't want to use the one in numberTextBox.
                    ReadOnly: Csw.bool(cswPrivate.isReadOnly),
                    isRequired: Csw.bool(cswPrivate.isRequired) && false === cswPrivate.quantityoptional,
                    onChange: function () {
                        var val = cswPrivate.numberTextBox.val();
                        Csw.tryExec(cswPrivate.onChange, val);
                        cswPrivate.onPropChange({ value: val });
                    },
                    isValid: true
                });
                cswPrivate.cellCol++;

                if (false === Csw.isNullOrEmpty(cswPrivate.numberTextBox) && cswPrivate.numberTextBox.length > 0) {
                    cswPrivate.numberTextBox.clickOnEnter(cswPrivate.saveBtn);
                }

                if (false === cswPrivate.isRequired) {
                    cswPrivate.relationships.push({ value: '', display: '', frac: true });
                }

                cswPrivate.foundSelected = false;
                Csw.eachRecursive(cswPrivate.options, function (relatedObj) {
                    if (relatedObj.id === cswPrivate.selectedNodeId) {
                        cswPrivate.foundSelected = true;
                        cswPrivate.fractional = Csw.bool(relatedObj.fractional);
                    }
                    cswPrivate.relationships.push({ value: relatedObj.id, display: relatedObj.value, frac: Csw.bool(relatedObj.fractional) });
                }, false);
                if (false === cswPrivate.isMulti && false === cswPrivate.foundSelected && false === Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
                    cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName, frac: Csw.bool(cswPrivate.propVals.fractional) });
                }
                cswPrivate.selectBox = cswPublic.control.cell(1, cswPrivate.cellCol).select({
                    name: cswPrivate.name,
                    cssclass: 'selectinput',
                    onChange: function () {
                        var val = cswPrivate.selectBox.val();
                        Csw.eachRecursive(cswPrivate.options, function (relatedObj) {
                            if (relatedObj.id === val) {
                                cswPrivate.fractional = Csw.bool(relatedObj.fractional);
                            }
                        }, false);
                        cswPrivate.precision = false === cswPrivate.fractional ? 0 : Csw.number(cswPrivate.propVals.precision, 6);
                        Csw.tryExec(cswPrivate.onChange, val);
                        cswPrivate.onPropChange({ nodeid: val });
                    },
                    values: cswPrivate.relationships,
                    selected: cswPrivate.selectedNodeId
                });
                if (cswPrivate.doPropChangeDataBind() && Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
                    cswPrivate.onPropChange({ nodeid: cswPrivate.selectBox.val() });
                }
                cswPrivate.cellCol += 1;

                cswPrivate.selectBox.required(cswPrivate.isRequired);

                $.validator.addMethod('validateInteger', function (value, element) {
                    return (cswPrivate.precision != 0 || Csw.validateInteger(cswPrivate.numberTextBox.val()));
                }, 'Value must be a whole number');
                cswPrivate.numberTextBox.addClass('validateInteger');

                $.validator.addMethod('validateIntegerGreaterThanZero', function (value, element) {
                    return (Csw.validateIntegerGreaterThanZero(cswPrivate.numberTextBox.val()));
                }, 'Value must be a non-zero, positive number');
                cswPrivate.numberTextBox.addClass('validateIntegerGreaterThanZero');

                $.validator.addMethod('validateUnitPresent', function (value, element) {
                    return (false === Csw.isNullOrEmpty(cswPrivate.selectBox.val()) || Csw.isNullOrEmpty(cswPrivate.numberTextBox.val()));
                }, 'Unit must be selected if Quantity is present.');
                cswPrivate.selectBox.addClass('validateUnitPresent');

                if (false === cswPrivate.quantityoptional) {
                    cswPrivate.numberTextBox.required(cswPrivate.isRequired);

                    $.validator.addMethod('validateQuantityPresent', function (value, element) {
                        return (false === Csw.isNullOrEmpty(cswPrivate.numberTextBox.val()) || Csw.isNullOrEmpty(cswPrivate.selectBox.val()));
                    }, 'Quantity must have a value if Unit is selected.');
                    cswPrivate.selectBox.addClass('validateQuantityPresent');
                }

                cswPrivate.selectBox.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectBox.val()); },
                                        function (event) { Csw.nodeHoverOut(event, cswPrivate.selectBox.val()); });

                cswPublic.refresh = function (data) {
                    cswPrivate.value = data.value;
                    cswPrivate.nodeid = data.nodeid;
                    cswPrivate.name = data.name;
                    cswPrivate.qtyReadonly = data.qtyReadonly;
                    cswPrivate.selectedNodeId = Csw.string(data.relatednodeid).trim();
                    cswPrivate.selectedName = Csw.string(data.unitName).trim();
                    cswPublic.table.empty();
                    buildQuantityTextBox();
                    buildUnitSelect();
                    Csw.tryExec(cswPrivate.onChange, cswPublic);
                };
            });
            return cswPublic;
        });
})(jQuery);
