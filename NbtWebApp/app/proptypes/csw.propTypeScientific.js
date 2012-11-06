/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.scientific = Csw.properties.scientific ||
        Csw.properties.register('scientific',
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
                    cswPrivate.minValue = Csw.number(cswPrivate.propVals.minvalue);
                    cswPrivate.precision = Csw.number(cswPrivate.propVals.precision, 6);
                    cswPrivate.ceilingVal = '999999999' + Csw.getMaxValueForPrecision(cswPrivate.precision);
                    cswPrivate.realValue = '';
                    cswPublic.control = cswPrivate.parent.div();
                    
                    if (Csw.bool(cswPublic.data.isReadOnly())) {
                        cswPublic.control.append(cswPrivate.propVals.gestalt);
                    }
                    else {
                        cswPrivate.valueNtb = cswPublic.control.numberTextBox({
                            name: cswPublic.data.name + '_val',
                            value: Csw.string(cswPrivate.propVals.base).trim(),
                            ceilingVal: Csw.number(cswPrivate.ceilingVal),
                            Precision: cswPrivate.precision,
                            ReadOnly: cswPublic.data.isReadOnly(),
                            isRequired: cswPublic.data.isRequired(),
                            onChange: function () {
                                var val = cswPrivate.valueNtb.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ base: val });
                            },
                            width: '65px'
                        });
                        cswPublic.control.append('E');
                        cswPrivate.exponentNtb = cswPublic.control.numberTextBox({
                            name: cswPublic.data.name + '_exp',
                            value: Csw.string(cswPrivate.propVals.exponent).trim(),
                            Precision: 0,
                            ReadOnly: cswPublic.data.isReadOnly(),
                            isRequired: cswPublic.data.isRequired(),
                            onChange: function () {
                                var val = cswPrivate.exponentNtb.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ exponent: val });
                            },
                            width: '40px'
                        });

                        if (cswPrivate.valueNtb && cswPrivate.valueNtb.length() > 0) {
                            cswPrivate.valueNtb.clickOnEnter(cswPublic.data.saveBtn);
                        }
                        if (cswPrivate.exponentNtb && cswPrivate.exponentNtb.length() > 0) {
                            cswPrivate.exponentNtb.clickOnEnter(cswPublic.data.saveBtn);
                        }
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
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

}()); 