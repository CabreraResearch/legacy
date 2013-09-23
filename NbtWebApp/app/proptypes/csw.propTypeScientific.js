/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('scientific', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            cswPrivate.minValue = Csw.number(nodeProperty.propData.values.minvalue);
            cswPrivate.precision = nodeProperty.propData.values.precision;
            cswPrivate.ceilingVal = '999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision);
            cswPrivate.realValue = '';
            cswPrivate.base = nodeProperty.propData.values.base;
            cswPrivate.exponent = nodeProperty.propData.values.exponent;

            nodeProperty.onPropChangeBroadcast(function (val) {
                if (cswPrivate.base !== val.base || cswPrivate.exponent !== val.exponent) {
                    cswPrivate.base = val.base;
                    cswPrivate.exponent = val.exponent;
                    updateProp(val);
                }
            });

            var updateProp = function (val) {
                nodeProperty.propData.values.base = val.base;
                nodeProperty.propData.values.exponent = val.exponent;
                cswPrivate.valueNtb.val(val.base);
                cswPrivate.exponentNtb.val(val.exponent);
            };

            if (Csw.bool(nodeProperty.isReadOnly())) {
                nodeProperty.propDiv.span({ text: nodeProperty.propData.values.gestalt });
            } else {
                var div = nodeProperty.propDiv.div();
                cswPrivate.valueNtb = div.numberTextBox({
                    name: nodeProperty.name + '_val',
                    value: Csw.string(nodeProperty.propData.values.base).trim(),
                    ceilingVal: Csw.number(cswPrivate.ceilingVal),
                    Precision: cswPrivate.precision,
                    ReadOnly: nodeProperty.isReadOnly(),
                    isRequired: nodeProperty.isRequired(),
                    onChange: function (val) {
                        nodeProperty.propData.values.base = val;
                        nodeProperty.broadcastPropChange({
                            base: val,
                            exponent: nodeProperty.propData.values.exponent
                        });
                    },
                    width: '65px'
                });
                div.append('E');
                cswPrivate.exponentNtb = div.numberTextBox({
                    name: nodeProperty.name + '_exp',
                    value: Csw.string(nodeProperty.propData.values.exponent).trim(),
                    Precision: 0,
                    ReadOnly: nodeProperty.isReadOnly(),
                    isRequired: nodeProperty.isRequired(),
                    onChange: function (val) {
                        nodeProperty.propData.values.exponent = val;
                        nodeProperty.broadcastPropChange({
                            base: nodeProperty.propData.values.base,
                            exponent: val
                        });
                    },
                    width: '40px'
                });

                //Case 24645 - scientific-specific number validation (i.e. - ensuring that the evaluated number is positive)
                if (Csw.isNumber(cswPrivate.minValue) && Csw.isNumeric(cswPrivate.minValue)) {
                    $.validator.addMethod('validateMinValue', function () {
                        var realValue = Csw.number(cswPrivate.valueNtb.val()) * Math.pow(10, Csw.number(cswPrivate.exponentNtb.val()));
                        return (realValue > cswPrivate.minValue || Csw.string(realValue).length === 0);
                    }, 'Evaluated expression must be greater than ' + cswPrivate.minValue);
                    cswPrivate.valueNtb.addClass('validateMinValue');
                    //exponentNtb.addClass('validateMinValue');//Case 26668, Review 26672
                }

                $.validator.addMethod('validateExponentPresent', function (value, element) {
                    return (false === Csw.isNullOrEmpty(cswPrivate.exponentNtb.val()) || Csw.isNullOrEmpty(cswPrivate.valueNtb.val()));
                }, 'Exponent must be defined if Base is defined.');
                cswPrivate.exponentNtb.addClass('validateExponentPresent');

                $.validator.addMethod('validateBasePresent', function (value, element) {
                    return (false === Csw.isNullOrEmpty(cswPrivate.valueNtb.val()) || Csw.isNullOrEmpty(cswPrivate.exponentNtb.val()));
                }, 'Base must be defined if Exponent is defined.');
                cswPrivate.exponentNtb.addClass('validateBasePresent');
            }
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());