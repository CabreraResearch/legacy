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

                //The render function to be executed as a callback
                var render = function () {
                    'use strict';

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.precision = Csw.number(cswPrivate.propVals.precision, 6);
                    cswPrivate.ceilingVal = '999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision);
                    cswPrivate.selectedNodeId = Csw.string(cswPrivate.propVals.relatednodeid).trim();
                    cswPrivate.selectedName =   Csw.string(cswPrivate.propVals.name).trim();
                    cswPrivate.nodeTypeId = Csw.string(cswPrivate.propVals.nodetypeid).trim();
                    cswPrivate.objectClassId = Csw.string(cswPrivate.propVals.objectclassid).trim();
                    cswPrivate.options = cswPrivate.propVals.options;
                    cswPrivate.relationships = [];
                    cswPrivate.fractional = Csw.bool(cswPrivate.propVals.fractional);
                    cswPrivate.quantityoptional = Csw.bool(cswPrivate.propVals.quantityoptional);
                    cswPrivate.cellCol = 1;                    

                    if (false === cswPrivate.fractional) {
                        cswPrivate.precision = 0;
                    }
                    if (cswPublic.data.isReadOnly()) {
                        cswPublic.control = cswPrivate.parent.span({ text: cswPublic.data.propData.gestalt });
                    } else {

                        if (false === Csw.isNullOrEmpty(cswPublic.data.tabState.relatednodeid) &&
                            Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                            false === cswPublic.data.isMulti() &&
                            (Csw.number(cswPublic.data.tabState.relatednodetypeid) === Csw.number(cswPrivate.nodeTypeId) ||
                              Csw.number(cswPublic.data.tabState.relatedobjectclassid) === Csw.number(cswPrivate.objectClassId))) {
                            cswPrivate.selectedNodeId = cswPublic.data.tabState.relatednodeid;
                            cswPrivate.selectedName = cswPublic.data.tabState.relatednodename;
                        }

                        cswPublic.control = cswPrivate.parent.table();

                        cswPrivate.numberTextBox = cswPublic.control.cell(1, cswPrivate.cellCol).numberTextBox({
                            name: cswPublic.data.name + '_qty',
                            value: Csw.string(cswPrivate.propVals.value).trim(),
                            MinValue: Csw.number(cswPrivate.propVals.minvalue),
                            MaxValue: Csw.number(cswPrivate.propVals.maxvalue),
                            excludeRangeLimits: Csw.bool(cswPrivate.propVals.excludeRangeLimits),
                            ceilingVal: Csw.number(cswPrivate.ceilingVal),
                            Precision: 6, //case 24646 - precision is being handled in the validator below, so we don't want to use the one in numberTextBox.
                            ReadOnly: Csw.bool(cswPublic.data.isReadOnly()),
                            isRequired: Csw.bool(cswPublic.data.isRequired()) && false === cswPrivate.quantityoptional,
                            onChange: function () {
                                var val = cswPrivate.numberTextBox.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ value: val });
                            },
                            isValid: true
                        });
                        cswPrivate.cellCol++;

                        if (false === Csw.isNullOrEmpty(cswPrivate.numberTextBox) && cswPrivate.numberTextBox.length > 0) {
                            cswPrivate.numberTextBox.clickOnEnter(cswPublic.data.saveBtn);
                        }

                        if (false === cswPublic.data.isRequired()) {
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
                        if (false === cswPublic.data.isMulti() && false === cswPrivate.foundSelected && false === Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
                            cswPrivate.relationships.push({ value: cswPrivate.selectedNodeId, display: cswPrivate.selectedName, frac: Csw.bool(cswPrivate.propVals.fractional) });
                        }
                        cswPrivate.selectBox = cswPublic.control.cell(1, cswPrivate.cellCol).select({
                            name: cswPublic.data.name,
                            cssclass: 'selectinput',
                            onChange: function () {
                                var val = cswPrivate.selectBox.val();
                                Csw.eachRecursive(cswPrivate.options, function (relatedObj) {
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
                        if(cswPublic.data.doPropChangeDataBind() && Csw.isNullOrEmpty(cswPrivate.selectedNodeId)) {
                            cswPublic.data.onPropChange({ nodeid: cswPrivate.selectBox.val() });
                        }
                        cswPrivate.cellCol += 1;

                        cswPrivate.selectBox.required(cswPublic.data.isRequired());

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
                            cswPrivate.numberTextBox.required(cswPublic.data.isRequired());

                            $.validator.addMethod('validateQuantityPresent', function (value, element) {
                                return (false === Csw.isNullOrEmpty(cswPrivate.numberTextBox.val()) || Csw.isNullOrEmpty(cswPrivate.selectBox.val()));
                            }, 'Quantity must have a value if Unit is selected.');
                            cswPrivate.selectBox.addClass('validateQuantityPresent');
                        }

                        cswPrivate.selectBox.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectBox.val()); },
                                        function (event) { Csw.nodeHoverOut(event, cswPrivate.selectBox.val()); });
                    }

                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());
        