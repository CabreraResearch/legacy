///// <reference path="~/app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswFieldTypeLocationContents';

//    var methods = {
//        init: function (o) { // nodepk, $xml, onChange

//            var propDiv = o.propDiv;
//            propDiv.empty();

//            //                var value = Csw.string(o.propData.value).trim();

//            propDiv.append('[Not Implemented Yet]');
//            //                if(o.ReadOnly)
//            //                {
//            //                    $Div.append(Value);
//            //                }
//            //                else 
//            //                {
//            //                    
//            //                }
//        },
//        save: function (o) {
//            //          var $TextBox = $propdiv.find('input');
//            //          $xml.children('barcode').text($TextBox.val());
//            Csw.preparePropJsonForSave(o.propData);
//        }
//    };

//    // Method calling logic
//    $.fn.CswFieldTypeLocationContents = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);
