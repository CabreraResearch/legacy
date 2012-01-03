/// <reference path='/js/../Scripts/jquery-1.7.1-vsdoc.js' />
/// <reference path='../../globals/Global.js' />
/// <reference path='../../globals/CswGlobalTools.js' />
/// <reference path='../../globals/CswEnums.js' />

(function ($) { /// <param name='$' type='jQuery' />
    'use strict';
    var pluginName = 'CswNumberTextBox';

    var methods = {
        init: function (options)
        {
            var o = {
                ID: '',
                Value: '',
                MinValue: '',
                MaxValue: '',
                Precision: '',
                ReadOnly: false,
                Required: false,
                onchange: function() { },
                width: ''
            };
            if(options) $.extend(o, options);

            var $Div = $(this),
                dbMax = 999999999.999999,
                width, maxLength, $TextBox,
                minValue = +o.MinValue,
                maxValue = +o.MaxValue,
                precision = +o.Precision;
            
            //$Div.contents().remove();
            
            if (o.ReadOnly) {
                $Div.append(o.Value);
            } else {
                /* Case 24499: Client-side logic to validate numbers. */
                maxLength = Math.abs(maxValue).toString().length;
                if(maxLength <= 0 || 
                   maxValue === 0 || 
                   maxLength > 9) {
                    maxLength = 9;
                }
                if(precision > 0) {
                    maxLength += (precision + 1); /*Decimal occupies a character.*/
                }
                width = o.width || (maxLength * 8) + 'px';

                $TextBox = $Div.CswInput('init',{ID: o.ID,
                                                        type: CswInput_Types.text,
                                                        value: o.Value,
                                                        cssclass: 'textinput number',
                                                        onChange: o.onchange,
                                                        width: width
                                                     });

                //Limit the character length max
                $TextBox.CswAttrDom('maxlength', maxLength);
                
                if (isNumber(minValue) && isNumeric(minValue)) {
                    jQuery.validator.addMethod(o.ID + '_validateFloatMinValue', function (value, element) {
                        return (this.optional(element) || validateFloatMinValue($(element).val(), minValue));
                    }, 'Number must be greater than or equal to ' + minValue);
                    $TextBox.addClass(o.ID + '_validateFloatMinValue');
                }
                if (isNumber(maxValue) && isNumeric(maxValue)) {
                    jQuery.validator.addMethod(o.ID + '_validateFloatMaxValue', function (value, element) {
                        return (this.optional(element) || validateFloatMaxValue($(element).val(), maxValue));
                    }, 'Number must be less than or equal to ' + maxValue);
                    $TextBox.addClass(o.ID + '_validateFloatMaxValue');
                }
                if (o.Precision === undefined || o.Precision <= 0) {
                    jQuery.validator.addMethod(o.ID + '_validateInteger', function (value, element) {
                        return (this.optional(element) || validateInteger($(element).val()));
                    }, 'Value must be an integer');
                    $TextBox.addClass(o.ID + '_validateInteger');
                } else {
                    jQuery.validator.addMethod(o.ID + '_validateFloatPrecision', function (value, element) {
                        return (this.optional(element) || validateFloatPrecision($(element).val(), o.Precision));
                    }, 'Value must be numeric');
                    $TextBox.addClass(o.ID + '_validateFloatPrecision');
                }

                //Independant of any other validation, no number can be greater than this.
                $TextBox.CswAttrDom('max', '999999999.999999');
                jQuery.validator.addMethod(o.ID + '_validateDb_15_6_FieldLength', function (value, element) {
                    return validateFloatMaxValue($(element).val(), dbMax);
                }, 'Value cannot be greater than ' + dbMax + '.');
                $TextBox.addClass(o.ID + '_validateDb_15_6_FieldLength');
                
                if (o.Required) {
                    $TextBox.addClass('required');
                }
                return $TextBox;
            }
        },
        value: function (id)
        {
            var $Div = $(this);
            var $TextBox = $Div.find('input[id="'+id+'"]');
            return $TextBox.val();
        },
        setValue: function (id, newvalue)
        {
            var $Div = $(this);
            var $TextBox = $Div.find('input[id="'+id+'"]');
            if( newvalue !== undefined )
            {
                $TextBox.val( newvalue );
            }
        }
    };

    // Method calling logic
    $.fn.CswNumberTextBox = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } 
        else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
