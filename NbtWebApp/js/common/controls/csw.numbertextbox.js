/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {


    Csw.controls.numberTextBox = Csw.controls.numberTextBox ||
        Csw.controls.register('numberTextBox', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
                ID: '',
                value: '',
                cssclass: '',
                type: Csw.enums.inputTypes.text,
                MinValue: '',
                MaxValue: '',
                Precision: '',
                ReadOnly: false,
                Required: false,
                onChange: function () {
                },
                width: '',
                ceilingVal: 999999999.999999
            };
            var cswPublicRet = {};
            //        cswPublicRet.val = function (newValue) {
            //            var $Div = $(this);
            //            var $TextBox = $Div.find('input[id="' + id + '"]');
            //            if (newValue !== undefined) {
            //                $TextBox.val(newValue);
            //            }
            //            return $TextBox.val();
            //        };

            (function () {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }

                var ceilingVal = Csw.number(cswPrivateVar.ceilingVal),
                    minValue = Csw.number(cswPrivateVar.MinValue),
                    maxValue = Csw.number(cswPrivateVar.MaxValue, ceilingVal),
                    maxLength = Math.abs(maxValue).toString().length,
                    precision = Csw.number(cswPrivateVar.Precision);

                if (cswPrivateVar.ReadOnly) {
                    cswPrivateVar.div = cswParent.div(cswPrivateVar);
                    cswPublicRet = Csw.dom({}, cswPrivateVar.div);
                    //$.extend(cswPublicRet, Csw.literals.div(cswPrivateVar));
                    cswPublicRet.append(cswPrivateVar.value);
                } else {
                    /* Case 24499: Client-side logic to validate numbers. */
                    if (maxLength <= 0 ||
                        maxValue === 0 ||
                            maxLength > 9) {

                        maxLength = 9;
                    }
                    if (precision > 0) {
                        maxLength += (precision + 1); /*Decimal occupies a character.*/
                    }
                    cswPrivateVar.width = cswPrivateVar.width || (maxLength * 8) + 'px';
                    cswPrivateVar.cssclass += ' textinput number ';
                    cswPrivateVar.maxlength = maxLength;

                    cswPrivateVar.input = cswParent.input(cswPrivateVar);
                    cswPublicRet = Csw.dom({ }, cswPrivateVar.input);
                    //$.extend(cswPublicRet, Csw.literals.input(cswPrivateVar));

                    cswPublicRet.bind('change', function () {
                        cswPrivateVar.value = cswPublicRet.val();
                    });

                    if (Csw.isNumber(minValue) && Csw.isNumeric(minValue)) {
                        $.validator.addMethod(cswPrivateVar.ID + '_validateFloatMinValue', function (value, element) {
                            return (this.optional(element) || Csw.validateFloatMinValue($(element).val(), minValue));
                        }, 'Number must be greater than or equal to ' + minValue);
                        cswPublicRet.addClass(cswPrivateVar.ID + '_validateFloatMinValue');
                    }
                    if (Csw.isNumber(maxValue) &&
                        Csw.isNumeric(maxValue) &&
                            maxValue > minValue) {
                        $.validator.addMethod(cswPrivateVar.ID + '_validateFloatMaxValue', function (value, element) {
                            return (this.optional(element) || Csw.validateFloatMaxValue($(element).val(), maxValue));
                        }, 'Number must be less than or equal to ' + maxValue);
                        cswPublicRet.addClass(cswPrivateVar.ID + '_validateFloatMaxValue');
                    }
                    if (cswPrivateVar.Precision === undefined || cswPrivateVar.Precision <= 0) {
                        $.validator.addMethod(cswPrivateVar.ID + '_validateInteger', function (value, element) {
                            return (this.optional(element) || Csw.validateInteger($(element).val()));
                        }, 'Value must be an integer');
                        cswPublicRet.addClass(cswPrivateVar.ID + '_validateInteger');
                    } else {
                        $.validator.addMethod(cswPrivateVar.ID + '_validateFloatPrecision', function (value, element) {
                            return (this.optional(element) || Csw.validateFloatPrecision($(element).val(), cswPrivateVar.Precision));
                        }, 'Value must be numeric');
                        cswPublicRet.addClass(cswPrivateVar.ID + '_validateFloatPrecision');
                    }

                    if (0 < ceilingVal) {
                        //Independant of any other validation, no number can be greater than this.
                        cswPublicRet.propDom('max', ceilingVal);
                        $.validator.addMethod(cswPrivateVar.ID + '_validateDb_15_6_FieldLength', function (value, element) {
                            return Csw.validateFloatMaxValue($(element).val(), ceilingVal);
                        }, 'Value cannot be greater than ' + ceilingVal + '.');
                        cswPublicRet.addClass(cswPrivateVar.ID + '_validateDb_15_6_FieldLength');
                    }
                    if (cswPrivateVar.Required) {
                        cswPublicRet.addClass('required');
                    }
                } /* else */
            } ());
            return cswPublicRet;
        });

})(jQuery);
