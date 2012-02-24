/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    'use strict';

    function numberTextBox(options) {

        var internal = {
            $parent: '',
            ID: '',
            value: '',
            cssclass: '',
            type: Csw.enums.inputTypes.text,
            MinValue: '',
            MaxValue: '',
            Precision: '',
            ReadOnly: false,
            Required: false,
            onChange: function () { },
            width: '',
            ceilingVal: 999999999.999999
        };
        var external = {};
        //        external.val = function (newValue) {
        //            var $Div = $(this);
        //            var $TextBox = $Div.find('input[id="' + id + '"]');
        //            if (newValue !== undefined) {
        //                $TextBox.val(newValue);
        //            }
        //            return $TextBox.val();
        //        };

        (function () {
            if (options) {
                $.extend(internal, options);
            }

            var ceilingVal = Csw.number(internal.ceilingVal),
                minValue = Csw.number(internal.MinValue),
                maxValue = Csw.number(internal.MaxValue, ceilingVal),
                maxLength = Math.abs(maxValue).toString().length,
                precision = Csw.number(internal.Precision);

            if (internal.ReadOnly) {
                $.extend(external, Csw.controls.div(internal));
                external.append(internal.value);
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
                internal.width = internal.width || (maxLength * 8) + 'px';
                internal.cssclass += ' textinput number ';
                internal.maxlength = maxLength;

                $.extend(external, Csw.controls.input(internal));

                external.bind('change', function () {
                    internal.value = external.val();
                });

                if (Csw.isNumber(minValue) && Csw.isNumeric(minValue)) {
                    $.validator.addMethod(internal.ID + '_validateFloatMinValue', function (value, element) {
                        return (this.optional(element) || Csw.validateFloatMinValue($(element).val(), minValue));
                    }, 'Number must be greater than or equal to ' + minValue);
                    external.addClass(internal.ID + '_validateFloatMinValue');
                }
                if (Csw.isNumber(maxValue) &&
                    Csw.isNumeric(maxValue) &&
                        maxValue > minValue) {
                    $.validator.addMethod(internal.ID + '_validateFloatMaxValue', function (value, element) {
                        return (this.optional(element) || Csw.validateFloatMaxValue($(element).val(), maxValue));
                    }, 'Number must be less than or equal to ' + maxValue);
                    external.addClass(internal.ID + '_validateFloatMaxValue');
                }
                if (internal.Precision === undefined || internal.Precision <= 0) {
                    $.validator.addMethod(internal.ID + '_validateInteger', function (value, element) {
                        return (this.optional(element) || Csw.validateInteger($(element).val()));
                    }, 'Value must be an integer');
                    external.addClass(internal.ID + '_validateInteger');
                } else {
                    $.validator.addMethod(internal.ID + '_validateFloatPrecision', function (value, element) {
                        return (this.optional(element) || Csw.validateFloatPrecision($(element).val(), internal.Precision));
                    }, 'Value must be numeric');
                    external.addClass(internal.ID + '_validateFloatPrecision');
                }

                if (0 < ceilingVal) {
                    //Independant of any other validation, no number can be greater than this.
                    external.propDom('max', ceilingVal);
                    $.validator.addMethod(internal.ID + '_validateDb_15_6_FieldLength', function (value, element) {
                        return Csw.validateFloatMaxValue($(element).val(), ceilingVal);
                    }, 'Value cannot be greater than ' + ceilingVal + '.');
                    external.addClass(internal.ID + '_validateDb_15_6_FieldLength');
                }
                if (internal.Required) {
                    external.addClass('required');
                }
            } /* else */
        } ());
        return external;
    }
    Csw.controls.register('numberTextBox', numberTextBox);
    Csw.controls.numberTextBox = Csw.controls.numberTextBox || numberTextBox;

})(jQuery);
