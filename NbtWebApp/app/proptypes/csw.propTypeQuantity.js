/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.quantity = Csw.properties.quantity ||
        Csw.properties.register('quantity',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption || Csw.nbt.propertyOption(propertyOption)
                };

                var render = function () {
                    'use strict';

                    cswPrivate.isMultiEditValid = function (value) {
                        return cswPublic.data.Multi && value === Csw.enums.multiEditDefaultValue;
                    };

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.precision = Csw.number(cswPrivate.propVals.precision, 6);
                    cswPrivate.ceilingVal = '999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision);
                    cswPrivate.selectedNodeId = (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.relatednodeid).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.selectedName = (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.name).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.nodeTypeId = Csw.string(cswPrivate.propVals.nodetypeid).trim();
                    cswPrivate.objectClassId = Csw.string(cswPrivate.propVals.objectclassid).trim();
                    cswPrivate.options = cswPrivate.propVals.options;
                    cswPrivate.relationships = [];
                    cswPrivate.fractional = Csw.bool(cswPrivate.propVals.fractional);
                    cswPrivate.cellCol = 1;

                    if (false === cswPrivate.fractional) {
                        cswPrivate.precision = 0;
                    }
                    if (Csw.bool(cswPublic.data.propData.readonly)) {
                        cswPublic.control = cswPrivate.parent.span({ text: cswPublic.data.propData.gestalt });
                    } else {

                        if (false === Csw.isNullOrEmpty(cswPublic.data.relatednodeid) &&
                            Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                            false === cswPublic.data.Multi &&
                            (Csw.number(cswPublic.data.relatednodetypeid) === Csw.number(cswPrivate.nodeTypeId) ||
                              Csw.number(cswPublic.data.relatedobjectclassid) === Csw.number(cswPrivate.objectClassId))) {
                            cswPrivate.selectedNodeId = cswPublic.data.relatednodeid;
                            cswPrivate.selectedName = cswPublic.data.relatednodename;
                        }

                        cswPublic.control = cswPrivate.parent.table({
                            ID: Csw.makeId(cswPublic.data.ID, 'tbl')
                        });

                        cswPrivate.numberTextBox = cswPublic.control.cell(1, cswPrivate.cellCol).numberTextBox({
                            ID: cswPublic.data.ID + '_qty',
                            value: (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.value).trim() : Csw.enums.multiEditDefaultValue,
                            MinValue: Csw.number(cswPrivate.propVals.minvalue),
                            MaxValue: Csw.number(cswPrivate.propVals.maxvalue),
                            ceilingVal: Csw.number(cswPrivate.ceilingVal),
                            Precision: 6, //case 24646 - precision is being handled in the validator below, so we don't want to use the one in numberTextBox.
                            ReadOnly: Csw.bool(cswPublic.data.ReadOnly),
                            Required: Csw.bool(cswPublic.data.Required),
                            onChange: function () {
                                var val = cswPrivate.numberTextBox.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ value: val });
                            },
                            isValid: cswPrivate.isMultiEditValid
                        });
                        cswPrivate.cellCol++;

                        if (false === Csw.isNullOrEmpty(cswPrivate.numberTextBox) && cswPrivate.numberTextBox.length > 0) {
                            cswPrivate.numberTextBox.clickOnEnter(cswPublic.data.saveBtn);
                        }

                        if (cswPublic.data.Multi) {
                            cswPrivate.relationships.push({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue });
                        }
                        if (false === cswPublic.data.Required) {
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
                        if (false === cswPublic.data.Multi && false === cswPrivate.foundSelected && false === Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
                            cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName, frac: Csw.bool(cswPrivate.propVals.fractional) });
                        }
                        cswPrivate.selectBox = cswPublic.control.cell(1, cswPrivate.cellCol).select({
                            ID: cswPublic.data.ID,
                            cssclass: 'selectinput',
                            onChange: function () {
                                var val = cswPrivate.selectBox.val();
                                Csw.crawlObject(cswPrivate.options, function (relatedObj) {
                                    if (relatedObj.id === val) {
                                        cswPrivate.fractional = Csw.bool(relatedObj.fractional);
                                    }
                                }, false);
                                cswPrivate.precision = false === cswPrivate.fractional ? 0 : Csw.number(cswPrivate.propVals.precision, 6);
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ nodeid: val });
                            },
                            values: cswPrivate.relationships,
                            selected: cswPrivate.selectedNodeId
                        });
                        cswPrivate.cellCol += 1;

                        cswPrivate.selectBox.required(cswPublic.data.Required);
                        cswPrivate.numberTextBox.required(cswPublic.data.Required);

                        $.validator.addMethod('validateInteger', function (value, element) {
                            return (cswPrivate.isMultiEditValid(value) || cswPrivate.precision != 0 || Csw.validateInteger(cswPrivate.numberTextBox.val()));
                        }, 'Value must be a whole number');
                        cswPrivate.numberTextBox.addClass('validateInteger');

                        $.validator.addMethod('validateIntegerGreaterThanZero', function (value, element) {
                            return (cswPrivate.isMultiEditValid(value) || Csw.validateIntegerGreaterThanZero(cswPrivate.numberTextBox.val()));
                        }, 'Value must be a non-zero, positive number');
                        cswPrivate.numberTextBox.addClass('validateIntegerGreaterThanZero');

                        $.validator.addMethod('validateUnitPresent', function (value, element) {
                            return (cswPrivate.isMultiEditValid(value) || false === Csw.isNullOrEmpty(cswPrivate.selectBox.val()) || Csw.isNullOrEmpty(cswPrivate.numberTextBox.val()));
                        }, 'Unit must be selected if Quantity is present.');
                        cswPrivate.selectBox.addClass('validateUnitPresent');

                        $.validator.addMethod('validateQuantityPresent', function (value, element) {
                            return (cswPrivate.isMultiEditValid(value) || false === Csw.isNullOrEmpty(cswPrivate.numberTextBox.val()) || Csw.isNullOrEmpty(cswPrivate.selectBox.val()));
                        }, 'Quantity must have a value if Unit is selected.');
                        cswPrivate.selectBox.addClass('validateQuantityPresent');

                        cswPrivate.parent.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectBox.val()); },
                                        function (event) { Csw.nodeHoverOut(event, cswPrivate.selectBox.val()); });
                    }

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
        