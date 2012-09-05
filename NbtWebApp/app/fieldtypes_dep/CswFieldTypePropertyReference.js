///// <reference path="~/app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswFieldTypePropertyReference';

//    var methods = {
//        init: function (o) {

//            var propDiv = o.propDiv;
//            propDiv.empty();

//            var propVals = o.propData.values;
//            var text = (false === o.Multi) ? Csw.string(propVals.value, o.propData.gestalt).trim() : Csw.enums.multiEditDefaultValue;

//            text += '&nbsp;&nbsp;';
//            /* Static Div */
//            propDiv.div({
//                ID: o.ID,
//                cssclass: 'staticvalue',
//                text: text
//            });
//        },
//        save: function (o) { //$propdiv, $xml
//            Csw.preparePropJsonForSave(o.propData);
//        }
//    };

//    // Method calling logic
//    $.fn.CswFieldTypePropertyReference = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);
