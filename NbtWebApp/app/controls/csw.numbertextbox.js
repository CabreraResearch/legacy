/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {


    Csw.controls.numberTextBox = Csw.controls.numberTextBox ||
        Csw.controls.register('numberTextBox', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
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
                ceilingVal: 999999999.999999,
                isValid: null,
                isClosedSet: true
            };
            var cswPublic = {};
            //        cswPublic.val = function (newValue) {
            //            var $Div = $(this);
            //            var $TextBox = $Div.find('input[id="' + id + '"]');
            //            if (newValue !== undefined) {
            //                $TextBox.val(newValue);
            //            }
            //            return $TextBox.val();
            //        };

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                var ceilingVal = Csw.number(cswPrivate.ceilingVal),
                    minValue = Csw.number(cswPrivate.MinValue),
                    maxValue = Csw.number(cswPrivate.MaxValue, ceilingVal),
                    maxLength = Math.abs(maxValue).toString().length,
                    precision = Csw.number(cswPrivate.Precision, 6);

                if (cswPrivate.ReadOnly) {
                    cswPrivate.div = cswParent.div(cswPrivate);
                    cswPublic = Csw.dom({}, cswPrivate.div);
                    //Csw.extend(cswPublic, Csw.literals.div(cswPrivate));
                    cswPublic.append(cswPrivate.value);
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
                    cswPrivate.width = cswPrivate.width || (maxLength * 8) + 'px';
                    cswPrivate.cssclass += ' textinput ';
                    cswPrivate.maxLength = maxLength;

                    cswPrivate.input = cswParent.input(cswPrivate);
                    cswPublic = Csw.dom({}, cswPrivate.input);
                    //Csw.extend(cswPublic, Csw.literals.input(cswPrivate));

                    cswPublic.bind('change', function () {
                        cswPrivate.value = cswPublic.val();
                    });

                    if (precision === undefined || precision <= 0) {
                        $.validator.addMethod(cswPrivate.ID + '_validateInteger', function (value, element) {
                            return (Csw.tryExec(cswPrivate.isValid, value) || this.optional(element) || Csw.validateInteger($(element).val()));
                        }, 'Value must be an integer');
                        cswPublic.addClass(cswPrivate.ID + '_validateInteger');
                    } else {
                        $.validator.addMethod(cswPrivate.ID + '_validateFloatPrecision', function (value, element) {
                            return (Csw.tryExec(cswPrivate.isValid, value) || this.optional(element) || Csw.validateFloatPrecision($(element).val(), precision));
                        }, 'Value must be numeric');
                        cswPublic.addClass(cswPrivate.ID + '_validateFloatPrecision');
                    }
                    if (Csw.isNumber(cswPrivate.maxLength)) {
                        $.validator.addMethod(cswPrivate.ID + '_validateMaxLength', function (value, element) {
                            return (Csw.tryExec(cswPrivate.isValid, value) || this.optional(element) || Csw.validateMaxLength($(element).val(), cswPrivate.maxLength));
                        }, 'Number must contain a most ' + cswPrivate.maxLength + ' digits');
                        cswPublic.addClass(cswPrivate.ID + '_validateMaxLength');
                    }

                    if (Csw.isNumber(minValue) && Csw.isNumeric(minValue)) {
                        $.validator.addMethod(cswPrivate.ID + '_validateFloatMinValue', function (value, element) {
                            return (Csw.tryExec(cswPrivate.isValid, value) || this.optional(element) || Csw.validateFloatMinValue($(element).val(), minValue, cswPrivate.isClosedSet));
                        }, 'Number must be greater than' + (Csw.bool(cswPrivate.isClosedSet) ? ' or equal to ' : ' ') + minValue);
                        cswPublic.addClass(cswPrivate.ID + '_validateFloatMinValue');
                    }
                    if (Csw.isNumber(maxValue) &&
                        Csw.isNumeric(maxValue) &&
                            maxValue > minValue) {
                        $.validator.addMethod(cswPrivate.ID + '_validateFloatMaxValue', function (value, element) {
                            return (Csw.tryExec(cswPrivate.isValid, value) || this.optional(element) || Csw.validateFloatMaxValue($(element).val(), maxValue, cswPrivate.isClosedSet));
                        }, 'Number must be less than' + (Csw.bool(cswPrivate.isClosedSet) ? ' or equal to ' : ' ') + maxValue);
                        cswPublic.addClass(cswPrivate.ID + '_validateFloatMaxValue');
                    }

                    if (0 < ceilingVal) {
                        //Independant of any other validation, no number can be greater than this.
                        $.validator.addMethod(cswPrivate.ID + '_validateDb_15_6_FieldLength', function (value, element) {
                            return Csw.validateFloatMaxValue(Math.abs($(element).val()), ceilingVal);
                        }, 'Value precision cannot exceed ' + ceilingVal + '.');
                        cswPublic.addClass(cswPrivate.ID + '_validateDb_15_6_FieldLength');
                    }
                    if (cswPrivate.Required) {
                        cswPublic.addClass('required');
                    }
                } /* else */
            } ());
            return cswPublic;
        });

})(jQuery);
