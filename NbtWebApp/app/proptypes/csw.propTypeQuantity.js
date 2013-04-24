/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.quantity = Csw.properties.register('quantity',
        function(nodeProperty) {
            'use strict';
            
            var cswPublic = {
                
            };

            //The render function to be executed as a callback
            var render = function() {
                'use strict';
                var cswPrivate = Csw.object();

                cswPrivate.selectedName = nodeProperty.propData.values.name;
                cswPrivate.cellCol = 1;
                cswPrivate.nodeid = nodeProperty.propData.values.nodeid;
                cswPrivate.value = nodeProperty.propData.values.value;
                
                nodeProperty.onPropChangeBroadcast(function (val) {
                    if (cswPrivate.value !== val.value || cswPrivate.nodeid !== val.nodeid) {
                        cswPrivate.value = val.value;
                        cswPrivate.nodeid = val.nodeid;
                        updateProp(val);
                    }
                });

                var updateProp = function (val) {
                    nodeProperty.propData.values.value = val.value;
                    nodeProperty.propData.values.nodeid = val.nodeid;
                    cswPrivate.quntCtrl.setUnitVal(val.unitid);
                    cswPrivate.quntCtrl.setQtyVal(val.value);
                };

                var onPropChange = function(broadcast) {
                    nodeProperty.propData.values.value = cswPrivate.quntCtrl.value();
                    nodeProperty.propData.values.nodeid = cswPrivate.quntCtrl.selectedUnit();
                    if (false !== broadcast) {
                        nodeProperty.broadcastPropChange({
                            value: nodeProperty.propData.values.value,
                            nodeid: nodeProperty.propData.values.nodeid
                        });
                    }
                };

                var quantity = {};
                quantity.precision = nodeProperty.propData.values.precision;
                quantity.ceilingVal = Csw.number('999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision));
                quantity.selectedNodeId = nodeProperty.propData.values.relatednodeid;
                quantity.unit = nodeProperty.propData.values.name;
                quantity.nodeTypeId = nodeProperty.propData.values.nodetypeid;
                quantity.objectClassId = nodeProperty.propData.values.objectclassid;
                quantity.options = nodeProperty.propData.values.options;
                quantity.relationships = [];
                quantity.quantity = nodeProperty.propData.values.value;
                quantity.minvalue = Csw.number(nodeProperty.propData.values.minvalue);
                quantity.maxvalue = Csw.number(nodeProperty.propData.values.maxvalue);
                quantity.excludeRangeLimits = nodeProperty.propData.values.excludeRangeLimits;
                quantity.fractional = nodeProperty.propData.values.fractional;
                quantity.quantityoptional = nodeProperty.propData.values.quantityoptional;
                quantity.cellCol = 1;
                quantity.name = nodeProperty.name;
                quantity.onChange = nodeProperty.onChange;
                quantity.isMulti = nodeProperty.isMulti();
                quantity.isRequired = nodeProperty.isRequired();
                quantity.isReadOnly = nodeProperty.isReadOnly();
                quantity.propVals = nodeProperty.propData.values;
                quantity.onNumberChange = function() {
                    onPropChange();
                };


                quantity.onQuantityChange = function() {
                    var val = cswPrivate.quntCtrl.selectedUnit();
                    Csw.iterate(quantity.options, function(relatedObj) {
                        if (relatedObj.id === val) {
                            quantity.fractional = Csw.bool(relatedObj.fractional);
                        }
                    });
                    quantity.precision = false === cswPrivate.fractional ? 0 : nodeProperty.propData.values.precision;
                    Csw.tryExec(quantity.onChange, val);
                    onPropChange();
                };

                if (false === cswPrivate.fractional) {
                    quantity.precision = 0;
                }
                if (nodeProperty.isReadOnly()) {
                    nodeProperty.propDiv.span({ text: nodeProperty.propData.gestalt });
                } else {

                    if (false === Csw.isNullOrEmpty(nodeProperty.tabState.relatednodeid) &&
                        Csw.isNullOrEmpty(cswPrivate.selectedNodeId) &&
                        false === nodeProperty.isMulti() &&
                        (Csw.number(nodeProperty.tabState.relatednodetypeid) === Csw.number(cswPrivate.nodeTypeId) ||
                            Csw.number(nodeProperty.tabState.relatedobjectclassid) === Csw.number(cswPrivate.objectClassId))) {
                        cswPrivate.selectedNodeId = nodeProperty.tabState.relatednodeid;
                        cswPrivate.selectedName = nodeProperty.tabState.relatednodename;
                    }
                    var table = nodeProperty.propDiv.table();
                    cswPrivate.quntCtrl = table.quantity(quantity);

                    //Case 29098 - after rendering the ctrl, make sure the internal data is up to date with the selected options
                    onPropChange(false);
                }

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

} ());
        