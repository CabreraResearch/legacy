/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    "use strict";
    Csw.composites.quantity = Csw.composites.quantity ||
        Csw.composites.register('quantity', function (cswParent, options) {

            //#region _preCtor
            var cswPublic = {},
                cswPrivate = {};

            (function _preCtor() {
                Csw.extend(cswPrivate, options);
                //cswPrivate.$parent = cswPrivate.$parent || cswParent.$;
                cswPrivate.name = options.name || '';
                cswPrivate.precision = options.precision || 6;
                cswPrivate.ceilingVal = options.ceilingVal || 999999;

                cswPrivate.selectedNodeId = options.selectedNodeId || '';
                cswPrivate.nodeTypeId = options.nodeTypeId || '';
                cswPrivate.objectClassId = options.objectClassId || '';

                cswPrivate.options = options.options || [];
                cswPrivate.relationships = options.relationships || [];
                cswPrivate.quantity = options.quantity || '';
                cswPrivate.unit = options.unit || '';
                cswPrivate.unitText = options.unitText || '';
                cswPrivate.minvalue = options.minvalue || '';
                cswPrivate.maxvalue = options.maxvalue || '';
                cswPrivate.excludeRangeLimits = options.excludeRangeLimits || false;
                cswPrivate.fractional = options.fractional || false;
                cswPrivate.quantityoptional = options.quantityoptional || false;
                cswPrivate.cellCol = options.cellCol || 1;
                cswPrivate.isMulti = options.isMulti || false;
                cswPrivate.isRequired = options.isRequired || false;
                cswPrivate.isReadOnly = options.isReadOnly || false;
                cswPrivate.isUnitReadOnly = options.isUnitReadOnly || false;

                cswPrivate.onChange = options.onChange || function () { };
                cswPrivate.onPropChange = options.onPropChange || function () { };
                cswPrivate.doPropChangeDataBind = options.doPropChangeDataBind || function () { };
                cswPrivate.onNumberChange = options.onNumberChange || function () { };
                cswPrivate.onQuantityChange = options.onQuantityChange || function () { };

                cswPrivate.validatorMethods = {};

                cswPublic = cswParent.div();
                cswPrivate.table = cswPublic.table();
            }());
            //#endregion _preCtor

            //#region Control Construction

            cswPrivate.makeControl = function () {
                cswPrivate.numberTextBox = cswPrivate.table.cell(1, cswPrivate.cellCol).numberTextBox({
                    name: cswPrivate.name + '_qty',
                    value: cswPrivate.quantity,
                    MinValue: cswPrivate.minvalue,
                    MaxValue: cswPrivate.maxvalue,
                    excludeRangeLimits: cswPrivate.excludeRangeLimits,
                    ceilingVal: cswPrivate.ceilingVal,
                    size: 6,
                    Precision: cswPrivate.precision,
                    ReadOnly: cswPrivate.isReadOnly,
                    isRequired: cswPrivate.isRequired && false === cswPrivate.quantityoptional,
                    onChange: cswPrivate.onNumberChange
                });
                cswPrivate.cellCol++;

                if (false === cswPrivate.isRequired) {
                    cswPrivate.relationships.push({ value: '', display: '', frac: true });
                }

                var foundSelected = false;
                Csw.iterate(cswPrivate.options, function (relatedObj) {
                    if (relatedObj.id === cswPrivate.selectedNodeId) {
                        foundSelected = true;
                        cswPrivate.fractional = Csw.bool(relatedObj.fractional);
                    }
                    cswPrivate.relationships.push({ value: relatedObj.id, display: relatedObj.value, frac: Csw.bool(relatedObj.fractional) });
                });
                if (false === cswPrivate.isMulti && false === foundSelected && false === Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
                    cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.unit, frac: cswPrivate.fractional });
                }

                var unitCell = cswPrivate.table.cell(1, cswPrivate.cellCol);
                cswPrivate.selectBox = unitCell.select({
                    name: cswPrivate.name,
                    cssclass: 'selectinput',
                    onChange: cswPrivate.onQuantityChange,
                    values: cswPrivate.relationships,
                    selected: cswPrivate.selectedNodeId
                });

                if (cswPrivate.doPropChangeDataBind() && Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
                    cswPrivate.onPropChange({ nodeid: cswPrivate.selectBox.val() });
                }
                cswPrivate.cellCol += 1;

                cswPrivate.selectBox.required(cswPrivate.isRequired);

                cswPrivate.addValidators();

                if (cswPrivate.isUnitReadOnly) {
                    cswPrivate.selectBox.hide();
                    unitCell.span({ text: cswPrivate.selectBox.selectedText() }).css('padding-left', '10px');
                }
            };

            /// Add validators
            cswPrivate.addValidators = function () {

                // cswPrivate.validatorMethods.validateInteger
                cswPrivate.validatorMethods.validateInteger = function () {
                    return (cswPrivate.precision != 0 || Csw.validateInteger(cswPrivate.numberTextBox.val()));
                };
                $.validator.addMethod('validateInteger', function (value, element) {
                    return Csw.tryExec(cswPrivate.validatorMethods.validateInteger);
                }, 'Value must be a whole number');
                cswPrivate.numberTextBox.addClass('validateInteger');

                // cswPrivate.validatorMethods.validateIntegerGreaterThanZero
                cswPrivate.validatorMethods.validateIntegerGreaterThanZero = function () {
                    return (Csw.validateIntegerGreaterThanZero(cswPrivate.numberTextBox.val()));
                };
                $.validator.addMethod('validateIntegerGreaterThanZero', function (value, element) {
                    return Csw.tryExec(cswPrivate.validatorMethods.validateIntegerGreaterThanZero);
                }, 'Value must be a non-zero, positive number');
                cswPrivate.numberTextBox.addClass('validateIntegerGreaterThanZero');

                // cswPrivate.validatorMethods.validateUnitPresent
                cswPrivate.validatorMethods.validateUnitPresent = function () {
                    return (false === Csw.isNullOrEmpty(cswPrivate.selectBox.val()) || Csw.isNullOrEmpty(cswPrivate.numberTextBox.val()));
                };
                $.validator.addMethod('validateUnitPresent', function (value, element) {
                    return Csw.tryExec(cswPrivate.validatorMethods.validateUnitPresent);
                }, 'Unit must be selected if Quantity is present.');
                cswPrivate.selectBox.addClass('validateUnitPresent');

                // cswPrivate.validatorMethods.validateQuantityPresent
                if (false === cswPrivate.quantityoptional) {
                    cswPrivate.numberTextBox.required(cswPrivate.isRequired);
                    cswPrivate.validatorMethods.validateQuantityPresent = function () {
                        return (false === Csw.isNullOrEmpty(cswPrivate.numberTextBox.val()) || Csw.isNullOrEmpty(cswPrivate.selectBox.val()));
                    };
                    $.validator.addMethod('validateQuantityPresent', function (value, element) {
                        return Csw.tryExec(cswPrivate.validatorMethods.validateQuantityPresent);
                    }, 'Quantity must have a value if Unit is selected.');
                    cswPrivate.selectBox.addClass('validateQuantityPresent');
                }
            };
            //#endregion Control Construction

            //#region Public

            cswPublic.selectedUnit = function () {
                if (cswPrivate && cswPrivate.selectBox && cswPrivate.selectBox.val) {
                    cswPrivate.unit = cswPrivate.selectBox.val();
                }
                return cswPrivate.unit;
            };

            cswPublic.setUnitVal = function (val) {
                if (cswPrivate && cswPrivate.selectBox && cswPrivate.selectBox.val) {
                    cswPrivate.selectBox.val(val);
                }
            };

            cswPublic.setQtyVal = function (val) {
                if (cswPrivate && cswPrivate.numberTextBox && cswPrivate.numberTextBox.val) {
                    cswPrivate.numberTextBox.val(val);
                }
            };

            cswPublic.value = function () {
                if (cswPrivate && cswPrivate.numberTextBox && cswPrivate.numberTextBox.val &&
                    false == cswPrivate.isReadOnly) {
                    cswPrivate.quantity = cswPrivate.numberTextBox.val();
                }
                return cswPrivate.quantity;
            };

            cswPublic.selectedNodeId = function () {
                return cswPrivate.selectedNodeId;
            };

            cswPublic.remove = function () {
            	/// <summary>
                /// Case 30546: This is a temporary kludge to fix the validation issue.
                /// The 'validateQuantityPresent' validator method was referring to the cached version
                /// of the quantity control. This method makes it such that when it tries to validate that
                /// cached control, we fake that it is valid.
            	/// </summary>
                cswPrivate.validatorMethods.validateQuantityPresent = function() {
                    return true;
                };
            };

            cswPublic.refresh = function (data) {
                cswPrivate.quantity = data.value;
                cswPrivate.nodeid = data.nodeid;
                cswPrivate.name = data.name;
                cswPrivate.isReadOnly = data.qtyReadonly;
                cswPrivate.isRequired = data.isRequired;
                cswPrivate.options = data.options;
                cswPrivate.selectedNodeId = Csw.string(data.relatednodeid).trim();
                cswPrivate.unit = Csw.string(data.unitName).trim();
                cswPrivate.relationships = [];
                cswPrivate.table.empty();
                cswPrivate.makeControl();
                Csw.tryExec(cswPrivate.onChange, cswPublic);
            };
            //#endregion Public

            //#region _postCtor

            (function _quantity() {
                if (cswPrivate.isReadOnly) {
                    cswPrivate.quantityText = cswPrivate.table.cell(1, 1).span({
                        text: cswPrivate.quantity + ' ' + cswPrivate.unit
                    });
                } else {
                    cswPrivate.makeControl();
                }
            }());

            //#endregion _postCtor

            return cswPublic;

        });
}());
