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

                    var quantity = {}
                    quantity.precision = Csw.number(cswPrivate.propVals.precision, 6);
                    quantity.ceilingVal = Csw.number('999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision));
                    quantity.selectedNodeId = Csw.string(cswPrivate.propVals.relatednodeid).trim();
                    cswPrivate.selectedName = Csw.string(cswPrivate.propVals.name).trim();
                    quantity.unit = Csw.string(cswPrivate.propVals.name).trim();
                    quantity.nodeTypeId = Csw.string(cswPrivate.propVals.nodetypeid).trim();
                    quantity.objectClassId = Csw.string(cswPrivate.propVals.objectclassid).trim();
                    quantity.options = cswPrivate.propVals.options;
                    quantity.relationships = [];
                    quantity.quantity = Csw.string(cswPrivate.propVals.value).trim();
                    cswPrivate.cellCol = 1;
                    quantity.minvalue = Csw.number(cswPrivate.propVals.minvalue);
                    quantity.maxvalue = Csw.number(cswPrivate.propVals.maxvalue);
                    quantity.excludeRangeLimits = Csw.bool(cswPrivate.propVals.excludeRangeLimits);
                    quantity.fractional = Csw.bool(cswPrivate.propVals.fractional);
                    quantity.quantityoptional = Csw.bool(cswPrivate.propVals.quantityoptional);
                    quantity.cellCol = 1;
                    quantity.name = cswPublic.data.name;
                    quantity.onChange = cswPublic.data.onChange;
                    quantity.isMulti = cswPublic.data.isMulti();
                    quantity.isRequired = cswPublic.data.isRequired();
                    quantity.isReadOnly = cswPublic.data.isReadOnly();
                    quantity.onPropChange = cswPublic.data.onPropChange;
                    quantity.doPropChangeDataBind = cswPublic.data.doPropChangeDataBind;
                    quantity.propVals = cswPrivate.propVals;
                    quantity.onNumberChange = function () {
                        var val = cswPrivate.quntCtrl.value();
                        Csw.tryExec(cswPublic.data.onChange, val);
                        cswPublic.data.onPropChange({ value: val });
                    };
                    quantity.onQuantityChange = function () {
                        var val = cswPrivate.quntCtrl.selectedUnit();
                        Csw.eachRecursive(quantity.options, function (relatedObj) {
                            if (relatedObj.id === val) {
                                quantity.fractional = Csw.bool(relatedObj.fractional);
                            }
                        }, false);
                        quantity.precision = false === cswPrivate.fractional ? 0 : Csw.number(cswPrivate.propVals.precision, 6);
                        Csw.tryExec(quantity.onChange, val);
                        quantity.onPropChange({ nodeid: val });
                    };

                    if (false === cswPrivate.fractional) {
                        quantity.precision = 0;
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
                        cswPrivate.quntCtrl = cswPublic.control.quantity(quantity);
                    }

                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());
        