/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('numericRange', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function() {
            'use strict';
            var cswPrivate = {};

            cswPrivate.precision = nodeProperty.propData.values.precision;
            cswPrivate.ceilingVal = '999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision);

            cswPrivate.lower = nodeProperty.propData.values.lower;
            cswPrivate.target = nodeProperty.propData.values.target;
            cswPrivate.upper = nodeProperty.propData.values.upper;
            cswPrivate.lowerInclusive = nodeProperty.propData.values.lowerinclusive;
            cswPrivate.upperInclusive = nodeProperty.propData.values.upperinclusive;

            nodeProperty.onPropChangeBroadcast(function(val) {
                if (cswPrivate.lower !== val.lower ||
                    cswPrivate.target !== val.target ||
                    cswPrivate.upper !== val.upper ||
                    cswPrivate.lowerInclusive !== val.lowerInclusive ||
                    cswPrivate.upperInclusive !== val.upperInclusive) {
                    cswPrivate.lower = val.lower;
                    cswPrivate.target = val.target;
                    cswPrivate.upper = val.upper;
                    cswPrivate.lowerInclusive = val.lowerInclusive;
                    cswPrivate.upperInclusive = val.upperInclusive;

                    updateProp(val);
                }
            });

            var updateProp = function(val) {
                nodeProperty.propData.values.lower = val.lower;
                nodeProperty.propData.values.target = val.target;
                nodeProperty.propData.values.upper = val.upper;
                nodeProperty.propData.values.lowerInclusive = val.lowerInclusive;
                nodeProperty.propData.values.upperInclusive = val.upperInclusive;

                cswPrivate.lowerNtb.val(val.lower);
                cswPrivate.targetNtb.val(val.target);
                cswPrivate.upperNtb.val(val.upper);
                cswPrivate.lowerIncSel.selectedVal(val.lowerInclusive);
                cswPrivate.upperIncSel.selectedVal(val.upperInclusive);
            };


            if (nodeProperty.isReadOnly()) {
                nodeProperty.propDiv.text(nodeProperty.propData.gestalt);
            } else {

                cswPrivate.table = nodeProperty.propDiv.table();


                cswPrivate.table.cell(1, 1).text('Lower');
                cswPrivate.table.cell(1, 3).text('Target');
                cswPrivate.table.cell(1, 5).text('Upper');

                cswPrivate.lowerNtb = cswPrivate.table.cell(2, 1).numberTextBox({
                    name: nodeProperty.name + '_lower',
                    value: cswPrivate.lower,
                    ceilingVal: cswPrivate.ceilingVal,
                    Precision: cswPrivate.precision,
                    ReadOnly: nodeProperty.isReadOnly(),
                    isRequired: nodeProperty.isRequired(),
                    size: 8,
                    onChange: function(val) {
                        nodeProperty.propData.values.lower = val;
                        nodeProperty.broadcastPropChange({
                            lower: val,
                            target: cswPrivate.target,
                            upper: cswPrivate.upper,
                            lowerInclusive: cswPrivate.lowerInclusive,
                            upperInclusive: cswPrivate.upperInclusive
                        });
                    },
                    isValid: true
                });
                cswPrivate.lowerNtb.required(nodeProperty.propData.required);

                cswPrivate.targetNtb = cswPrivate.table.cell(2, 3).numberTextBox({
                    name: nodeProperty.name + '_target',
                    value: cswPrivate.target,
                    ceilingVal: cswPrivate.ceilingVal,
                    Precision: cswPrivate.precision,
                    ReadOnly: nodeProperty.isReadOnly(),
                    isRequired: nodeProperty.isRequired(),
                    size: 8,
                    onChange: function(val) {
                        nodeProperty.propData.values.target = val;
                        nodeProperty.broadcastPropChange({
                            lower: cswPrivate.lower,
                            target: val,
                            upper: cswPrivate.upper,
                            lowerInclusive: cswPrivate.lowerInclusive,
                            upperInclusive: cswPrivate.upperInclusive
                        });
                    },
                    isValid: true
                });
                cswPrivate.targetNtb.required(nodeProperty.propData.required);

                cswPrivate.upperNtb = cswPrivate.table.cell(2, 5).numberTextBox({
                    name: nodeProperty.name + '_upper',
                    value: cswPrivate.upper,
                    ceilingVal: cswPrivate.ceilingVal,
                    Precision: cswPrivate.precision,
                    ReadOnly: nodeProperty.isReadOnly(),
                    isRequired: nodeProperty.isRequired(),
                    size: 8,
                    onChange: function(val) {
                        nodeProperty.propData.values.upper = val;
                        nodeProperty.broadcastPropChange({
                            lower: cswPrivate.lower,
                            target: cswPrivate.target,
                            upper: val,
                            lowerInclusive: cswPrivate.lowerInclusive,
                            upperInclusive: cswPrivate.upperInclusive
                        });
                    },
                    isValid: true
                });
                cswPrivate.upperNtb.required(nodeProperty.propData.required);

                cswPrivate.lowerIncSel = cswPrivate.table.cell(2, 2).select({
                    name: nodeProperty.name + '_lowerinc',
                    selected: cswPrivate.lowerInclusive,
                    values: [{ display: '<', value: false },
                             { display: '<=', value: true }],
                    onChange: function(val) {
                        nodeProperty.propData.values.lowerinclusive = val;
                        nodeProperty.broadcastPropChange({
                            lower: cswPrivate.lower,
                            target: cswPrivate.target,
                            upper: cswPrivate.upper,
                            lowerInclusive: val,
                            upperInclusive: cswPrivate.upperInclusive
                        });
                    }
                });
                cswPrivate.upperIncSel= cswPrivate.table.cell(2, 4).select({
                    name: nodeProperty.name + '_upperinc',
                    selected: cswPrivate.upperInclusive,
                    values: [{ display: '<', value: false },
                             { display: '<=', value: true }],
                    onChange: function(val) {
                        nodeProperty.propData.values.upperinclusive = val;
                        nodeProperty.broadcastPropChange({
                            lower: cswPrivate.lower,
                            target: cswPrivate.target,
                            upper: cswPrivate.upper,
                            lowerInclusive: cswPrivate.lowerInclusive,
                            upperInclusive: val
                        });
                    }
                });

            } // if-else(isReadonly()))
        }; // render()


        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

} ());
