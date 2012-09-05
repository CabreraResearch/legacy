///// <reference path="~/app/CswApp-vsdoc.js" />


//(function ($) {
//    'use strict';
//    var pluginName = 'CswFieldTypeText';

//    var methods = {
//        init: function (o) {

//            var propDiv = o.propDiv;
//            propDiv.empty();
//            var propVals = o.propData.values;
//            var value = (false === o.Multi) ? Csw.string(propVals.text).trim() : Csw.enums.multiEditDefaultValue;
//            var length = Csw.number(propVals.length, 14);

//            if (o.ReadOnly) {
//                propDiv.append(value);
//            } else {
//                var textBox = propDiv.input({
//                    ID: o.ID,
//                    type: Csw.enums.inputTypes.text,
//                    value: value,
//                    cssclass: 'textinput',
//                    width: length * 7,
//                    onChange: o.onChange,
//                    required: o.Required
//                });

//                if (o.Required) {
//                    textBox.addClass('required');
//                }

//                textBox.clickOnEnter(o.saveBtn);
//            }
//        },
//        save: function (o) {
//            var attributes = {
//                text: null
//            };
//            var compare = {};
//            var text = o.propDiv.find('#' + o.ID);
//            if (false === Csw.isNullOrEmpty(text)) {
//                attributes.text = text.val();
//                compare = attributes;
//            }
//            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
//        }
//    };

//    // Method calling logic
//    $.fn.CswFieldTypeText = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);
