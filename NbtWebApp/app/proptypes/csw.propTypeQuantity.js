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
                    cswPrivate.selectedName = Csw.string(cswPrivate.propVals.name).trim();
                    cswPrivate.nodeTypeId = Csw.string(cswPrivate.propVals.nodetypeid).trim();
                    cswPrivate.objectClassId = Csw.string(cswPrivate.propVals.objectclassid).trim();
                    cswPrivate.options = cswPrivate.propVals.options;
                    cswPrivate.relationships = [];
                    cswPrivate.fractional = Csw.bool(cswPrivate.propVals.fractional);
                    cswPrivate.quantityoptional = Csw.bool(cswPrivate.propVals.quantityoptional);
                    cswPrivate.cellCol = 1;
                    cswPrivate.name = cswPublic.data.name;
                    cswPrivate.onChange = cswPublic.data.onChange;
                    cswPrivate.isMulti = cswPublic.data.isMulti();
                    cswPrivate.isRequired = cswPublic.data.isRequired();
                    cswPrivate.isReadOnly = cswPublic.data.isReadOnly();
                    cswPrivate.onPropChange = cswPublic.data.onPropChange;
                    cswPrivate.doPropChangeDataBind = cswPublic.data.doPropChangeDataBind;

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
                        cswPrivate.quntCtrl = cswPublic.control.quantity(cswPrivate);
                    }

                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());
        