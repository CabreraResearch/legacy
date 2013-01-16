/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    "use strict";
    Csw.composites.quantity = Csw.composites.quantity ||
        Csw.composites.register('quantity', function (cswParent, cswPrivate) {

            //#region _preCtor
            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.$parent = cswPrivate.$parent || cswParent.$;
                cswPrivate.name = cswPrivate.name || '';
                cswPrivate.precision = cswPrivate.precision || 6;
                cswPrivate.ceilingVal = cswPrivate.ceilingVal || 999999;

                cswPrivate.selectedNodeId = cswPrivate.selectedNodeId || '';
                cswPrivate.nodeTypeId = cswPrivate.nodeTypeId || '';
                cswPrivate.objectClassId = cswPrivate.objectClassId || '';

                cswPrivate.options = cswPrivate.options || [];
                cswPrivate.relationships = cswPrivate.relationships || [];
                cswPrivate.quantity = cswPrivate.quantity || '';
                cswPrivate.unit = cswPrivate.unit || '';
                cswPrivate.unitText = cswPrivate.unitText || '';
                cswPrivate.minvalue = cswPrivate.minvalue || '';
                cswPrivate.maxvalue = cswPrivate.maxvalue || '';
                cswPrivate.excludeRangeLimits = cswPrivate.excludeRangeLimits || false;
                cswPrivate.fractional = cswPrivate.fractional || false;
                cswPrivate.quantityoptional = cswPrivate.quantityoptional || false;
                cswPrivate.cellCol = cswPrivate.cellCol || 1;
                cswPrivate.isMulti = cswPrivate.isMulti || false;
                cswPrivate.isRequired = cswPrivate.isRequired || false;
                cswPrivate.isReadOnly = cswPrivate.isReadOnly || false;

                cswPrivate.onChange = cswPrivate.onChange || function () { };
                cswPrivate.onPropChange = cswPrivate.onPropChange || function () { };
                cswPrivate.doPropChangeDataBind = cswPrivate.doPropChangeDataBind || function () { };
                cswPrivate.onNumberChange = cswPrivate.onNumberChange || function () { };
                cswPrivate.onQuantityChange = cswPrivate.onQuantityChange || function () { };

                cswPublic = cswParent.div();
                cswPrivate.table = cswPublic.table();
            } ());
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
                Csw.eachRecursive(cswPrivate.options, function (relatedObj) {
                    if (relatedObj.id === cswPrivate.selectedNodeId) {
                        foundSelected = true;
                        cswPrivate.fractional = Csw.bool(relatedObj.fractional);
                    }
                    cswPrivate.relationships.push({ value: relatedObj.id, display: relatedObj.value, frac: Csw.bool(relatedObj.fractional) });
                }, false);
                if (false === cswPrivate.isMulti && false === foundSelected && false === Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
                    cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.unit, frac: cswPrivate.fractional });
                }

                cswPrivate.selectBox = cswPrivate.table.cell(1, cswPrivate.cellCol).select({
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
            };

            cswPrivate.addValidators = function () {
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
            };
            //#endregion Control Construction

            //#region Public

            cswPublic.selectedUnit = function () {
                if (cswPrivate && cswPrivate.selectBox && cswPrivate.selectBox.val) {
                    cswPrivate.unit = cswPrivate.selectBox.val();
                }
                return cswPrivate.unit;
            };
            
            cswPublic.selectedUnitText = function () {
                if (cswPrivate && cswPrivate.selectBox && cswPrivate.selectBox.val) {
                    cswPrivate.unitText = cswPrivate.selectBox.selectedText();
                }
                return cswPrivate.unitText;
            };

            cswPublic.value = function () {
                if (cswPrivate && cswPrivate.numberTextBox && cswPrivate.numberTextBox.val) {
                    cswPrivate.quantity = cswPrivate.numberTextBox.val();
                }
                return cswPrivate.quantity;
            };

            cswPublic.selectedNodeId = function () {
                return cswPrivate.selectedNodeId;
            };

            cswPublic.refresh = function (data) {
                cswPrivate.quantity = data.value;
                cswPrivate.nodeid = data.nodeid;
                cswPrivate.name = data.name;
                cswPrivate.qtyReadonly = data.qtyReadonly;
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
            } ());

            //#endregion _postCtor

            return cswPublic;

        });
} ());

//            var cswPrivate = {};

//            Csw.tryExec(function () {
//                cswPrivate = {
//                    relationships: [],
//                    fractional: true,
//                    labelText: '',
//                    maxvalue: NaN,
//                    minvalue: NaN,
//                    name: '',
//                    unitName: '',
//                    nodeid: '',
//                    nodetypeid: '',
//                    options: [],
//                    onNumberChange: null,
//                    onQuantityChange: null,
//                    precision: 6,
//                    relatednodeid: '',
//                    value: '',
//                    cellCol: 1,
//                    quantityText: '',
//                    gestalt: '',
//                    qtyWidth: '',
//                    qtyReadonly: false,
//                    unitReadonly: false,
//                    excludeRangeLimits: false,
//                    isRequired: false,
//                    isReadOnly: false
//                };



//                if (options) Csw.extend(cswPrivate, options);

//                cswPublic = cswParent.div();
//                cswPrivate.table = cswPublic.table();

//                cswPrivate.numberTextBox = cswPrivate.table.cell(1, cswPrivate.cellCol).numberTextBox({
//                    name: cswPrivate.name + '_qty',
//                    value: cswPrivate.value,
//                    MinValue: cswPrivate.minvalue,
//                    MaxValue: cswPrivate.maxvalue,
//                    excludeRangeLimits: cswPrivate.excludeRangeLimits,
//                    ceilingVal: cswPrivate.ceilingVal,
//                    size: 6,
//                    Precision: 6, //case 24646 - precision is being handled in the validator below, so we don't want to use the one in numberTextBox.
//                    ReadOnly: Csw.bool(cswPrivate.isReadOnly),
//                    isRequired: Csw.bool(cswPrivate.isRequired) && false === cswPrivate.quantityoptional,
//                    onChange: cswPrivate.onNumberChange,
//                    //                    function () {
//                    //                        var val = cswPrivate.numberTextBox.val();
//                    //                        Csw.tryExec(cswPrivate.onChange, val);
//                    //                        cswPrivate.onPropChange({ value: val });
//                    //                    },
//                    isValid: true
//                });
//                cswPrivate.cellCol++;

//                if (false === Csw.isNullOrEmpty(cswPrivate.numberTextBox) && cswPrivate.numberTextBox.length > 0) {
//                    cswPrivate.numberTextBox.clickOnEnter(cswPrivate.saveBtn);
//                }

//                if (false === cswPrivate.isRequired) {
//                    cswPrivate.relationships.push({ value: '', display: '', frac: true });
//                }

//                cswPrivate.foundSelected = false;
//                Csw.eachRecursive(cswPrivate.options, function (relatedObj) {
//                    if (relatedObj.id === cswPrivate.selectedNodeId) {
//                        cswPrivate.foundSelected = true;
//                        cswPrivate.fractional = Csw.bool(relatedObj.fractional);
//                    }
//                    cswPrivate.relationships.push({ value: relatedObj.id, display: relatedObj.value, frac: Csw.bool(relatedObj.fractional) });
//                }, false);
//                if (false === cswPrivate.isMulti && false === cswPrivate.foundSelected && false === Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
//                    cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName, frac: cswPrivate.fractional });
//                }
//                cswPrivate.selectBox = cswPrivate.table.cell(1, cswPrivate.cellCol).select({
//                    name: cswPrivate.name,
//                    cssclass: 'selectinput',
//                    onChange: cswPrivate.onQuantityChange,
//                    //function () {
//                    //                        var val = cswPrivate.selectBox.val();
//                    //                        Csw.eachRecursive(cswPrivate.options, function (relatedObj) {
//                    //                            if (relatedObj.id === val) {
//                    //                                cswPrivate.fractional = Csw.bool(relatedObj.fractional);
//                    //                            }
//                    //                        }, false);
//                    //                        cswPrivate.precision = false === cswPrivate.fractional ? 0 : Csw.number(cswPrivate.propVals.precision, 6);
//                    //                        Csw.tryExec(cswPrivate.onChange, val);
//                    //                        cswPrivate.onPropChange({ nodeid: val });
//                    //                    },
//                    values: cswPrivate.relationships,
//                    selected: cswPrivate.selectedNodeId
//                });
//                if (cswPrivate.doPropChangeDataBind() && Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
//                    cswPrivate.onPropChange({ nodeid: cswPrivate.selectBox.val() });
//                }
//                cswPrivate.cellCol += 1;

//                cswPrivate.selectBox.required(cswPrivate.isRequired);

//                $.validator.addMethod('validateInteger', function (value, element) {
//                    return (cswPrivate.precision != 0 || Csw.validateInteger(cswPrivate.numberTextBox.val()));
//                }, 'Value must be a whole number');
//                cswPrivate.numberTextBox.addClass('validateInteger');

//                $.validator.addMethod('validateIntegerGreaterThanZero', function (value, element) {
//                    return (Csw.validateIntegerGreaterThanZero(cswPrivate.numberTextBox.val()));
//                }, 'Value must be a non-zero, positive number');
//                cswPrivate.numberTextBox.addClass('validateIntegerGreaterThanZero');

//                $.validator.addMethod('validateUnitPresent', function (value, element) {
//                    return (false === Csw.isNullOrEmpty(cswPrivate.selectBox.val()) || Csw.isNullOrEmpty(cswPrivate.numberTextBox.val()));
//                }, 'Unit must be selected if Quantity is present.');
//                cswPrivate.selectBox.addClass('validateUnitPresent');

//                if (false === cswPrivate.quantityoptional) {
//                    cswPrivate.numberTextBox.required(cswPrivate.isRequired);

//                    $.validator.addMethod('validateQuantityPresent', function (value, element) {
//                        return (false === Csw.isNullOrEmpty(cswPrivate.numberTextBox.val()) || Csw.isNullOrEmpty(cswPrivate.selectBox.val()));
//                    }, 'Quantity must have a value if Unit is selected.');
//                    cswPrivate.selectBox.addClass('validateQuantityPresent');
//                }

//                cswPrivate.selectBox.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectBox.val()); },
//                                        function (event) { Csw.nodeHoverOut(event, cswPrivate.selectBox.val()); });

//                cswPublic.refresh = function (data) {
//                    cswPrivate.value = data.value;
//                    cswPrivate.nodeid = data.nodeid;
//                    cswPrivate.name = data.name;
//                    cswPrivate.qtyReadonly = data.qtyReadonly;
//                    cswPrivate.selectedNodeId = Csw.string(data.relatednodeid).trim();
//                    cswPrivate.selectedName = Csw.string(data.unitName).trim();
//                    cswPublic.table.empty();
//                    buildQuantityTextBox();
//                    buildUnitSelect();
//                    Csw.tryExec(cswPrivate.onChange, cswPublic);
//                };
//            });
//            return cswPublic;
//        });
//})(jQuery);
