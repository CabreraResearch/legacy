///// <reference path="~/app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswFieldTypeScientific';

//    var methods = {
//        init: function (o) {

//            var propDiv = o.propDiv;
//            propDiv.empty();
//            var propVals = o.propData.values,
//                minValue = Csw.number(propVals.minvalue),
//                precision = Csw.number(propVals.precision, 6),
//                ceilingVal = '999999999' + Csw.getMaxValueForPrecision(precision),
//                realValue;
//            if (Csw.bool(o.ReadOnly)) {
//                propDiv.append(propVals.gestalt);
//            }
//            else {
//                var valueNtb = propDiv.numberTextBox({
//                    ID: o.ID + '_val',
//                    value: (false === o.Multi) ? Csw.string(propVals.base).trim() : Csw.enums.multiEditDefaultValue,
//                    ceilingVal: Csw.number(ceilingVal),
//                    Precision: precision,
//                    ReadOnly: o.ReadOnly,
//                    Required: o.Required,
//                    onChange: o.onChange,
//                    width: '65px'
//                });
//                propDiv.append('E');
//                var exponentNtb = propDiv.numberTextBox({
//                    ID: o.ID + '_exp',
//                    value: (false === o.Multi) ? Csw.string(propVals.exponent).trim() : Csw.enums.multiEditDefaultValue,
//                    Precision: 0,
//                    ReadOnly: o.ReadOnly,
//                    Required: o.Required,
//                    onChange: o.onChange,
//                    width: '40px'
//                });

//                if (valueNtb && valueNtb.length() > 0) {
//                    valueNtb.clickOnEnter(o.saveBtn);
//                }
//                if (exponentNtb && exponentNtb.length() > 0) {
//                    exponentNtb.clickOnEnter(o.saveBtn);
//                }
//                //Case 24645 - scientific-specific number validation (i.e. - ensuring that the evaluated number is positive)
//                if (Csw.isNumber(minValue) && Csw.isNumeric(minValue)) {
//                    $.validator.addMethod('validateMinValue', function () {
//                        var realValue = Csw.number(valueNtb.val()) * Math.pow(10, Csw.number(exponentNtb.val()));
//                        return (realValue > minValue || Csw.string(realValue).length === 0);
//                    }, 'Evaluated expression must be greater than ' + minValue);
//                    valueNtb.addClass('validateMinValue');
//                    //exponentNtb.addClass('validateMinValue');//Case 26668, Review 26672
//                }

//                $.validator.addMethod('validateExponentPresent', function (value, element) {
//                    return (false === Csw.isNullOrEmpty(exponentNtb.val()) || Csw.isNullOrEmpty(valueNtb.val()));
//                }, 'Exponent must be defined if Base is defined.');
//                exponentNtb.addClass('validateExponentPresent');

//                $.validator.addMethod('validateBasePresent', function (value, element) {
//                    return (false === Csw.isNullOrEmpty(valueNtb.val()) || Csw.isNullOrEmpty(exponentNtb.val()));
//                }, 'Base must be defined if Exponent is defined.');
//                exponentNtb.addClass('validateBasePresent');
//            }
//        },
//        save: function (o) { //$propdiv, $xml
//            var attributes = {
//                base: null,
//                exponent: null
//            };
//            var compare = {};
//            var base = o.propDiv.find('#' + o.ID + '_val');
//            if (false === Csw.isNullOrEmpty(base)) {
//                attributes.base = base.val();
//                compare = attributes;
//            }
//            var exp = o.propDiv.find('#' + o.ID + '_exp');
//            if (false === Csw.isNullOrEmpty(exp)) {
//                attributes.exponent = exp.val();
//                compare = attributes;
//            }
//            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
//        }
//    };

//    // Method calling logic
//    $.fn.CswFieldTypeScientific = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);
