/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeScientific';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values,
                minValue = Csw.number(propVals.minvalue),
                precision = Csw.number(propVals.precision, 6),
                ceilingVal = '999999999' + Csw.getMaxValueForPrecision(precision),
                realValue;
            if (Csw.bool(o.ReadOnly)) {
                propDiv.append(propVals.gestalt);
            }
            else {
                var valueNtb = propDiv.numberTextBox({
                    ID: o.ID + '_val',
                    value: (false === o.Multi) ? Csw.string(propVals.base).trim() : Csw.enums.multiEditDefaultValue,
                    ceilingVal: Csw.number(ceilingVal),
                    Precision: precision,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    onChange: o.onChange,
                    width: '65px'
                });
                propDiv.append('E');
                var exponentNtb = propDiv.numberTextBox({
                    ID: o.ID + '_exp',
                    value: (false === o.Multi) ? Csw.string(propVals.exponent).trim() : Csw.enums.multiEditDefaultValue,
                    Precision: 0,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    onChange: o.onChange,
                    width: '40px'
                });

                if (valueNtb && valueNtb.length() > 0) {
                    valueNtb.clickOnEnter(o.saveBtn);
                }
                if (exponentNtb && exponentNtb.length() > 0) {
                    exponentNtb.clickOnEnter(o.saveBtn);
                }
                //Case 24645 - scientific-specific number validation (i.e. - ensuring that the evaluated number is positive)
                if (Csw.isNumber(minValue) && Csw.isNumeric(minValue)) {
                    $.validator.addMethod('validateMinValue', function () {
                        var realValue = Csw.number(valueNtb.val()) * Math.pow(10, Csw.number(exponentNtb.val()));
                        return (realValue > minValue || Csw.string(realValue).length === 0);
                    }, 'Value must be greater than ' + minValue);
                    valueNtb.addClass('validateMinValue');
                    exponentNtb.addClass('validateMinValue');
                }
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = {
                base: o.propDiv.find('#' + o.ID + '_val').val(),
                exponent: o.propDiv.find('#' + o.ID + '_exp').val()
            };
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };

    // Method calling logic
    $.fn.CswFieldTypeScientific = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
