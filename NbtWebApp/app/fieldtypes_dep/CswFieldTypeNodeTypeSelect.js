///// <reference path="~app/CswApp-vsdoc.js" />


//(function ($) {
//    "use strict";
//    var pluginName = 'CswFieldTypeNodeTypeSelect',
//        nameCol = 'label',
//        keyCol = 'key',
//        valueCol = 'value',
//        methods = {
//            init: function (o) {

//                var propDiv = o.propDiv;
//                propDiv.empty();

//                var propVals = o.propData.values;
//                var optData = propVals.options;
//                var selectMode = propVals.selectmode; // Single, Multiple, Blank

//                /*
//                var editMode = Csw.enums.tryParse(Csw.enums.editMode, o.EditMode);
//                Case 24606: Once we can validate the control
//                if(editMode === Csw.enums.editMode.Add) {
//                Csw.each(propVals.options, function (option) {
//                if (Csw.contains(option, 'key')) {
                            
//                var relatedNodeTypeId = Csw.string(o.relatednodetypeid);
//                if (Csw.string(option.key) === relatedNodeTypeId) 
//                one day we can try to set the defaults using the context of the view. Not today.
                            
//                option.value = 'False';
//                }
//                });
//                }
//                */
//                var cbaDiv = propDiv.div()
//                    .checkBoxArray({
//                        ID: o.ID + '_cba',
//                        UseRadios: (selectMode === 'Single'),
//                        Required: o.Required,
//                        ReadOnly: o.ReadOnly,
//                        Multi: o.Multi,
//                        onChange: o.onChange,
//                        dataAry: optData,
//                        nameCol: nameCol,
//                        keyCol: keyCol,
//                        valCol: valueCol,
//                        valColName: 'Include'
//                    });

//                if (o.Required) {
//                    cbaDiv.addClass("required");
//                }

//                return propDiv;
//            },
//            save: function (o) { //$propdiv, $xml
//                var attributes = { options: null };
//                var compare = {};
//                var formdata = Csw.clientDb.getItem(o.ID + '_cba' + '_cswCbaArrayDataStore'); 
//                if (false === o.Multi || false === formdata.MultiIsUnchanged) {
//                    attributes.options = formdata.data;
//                    compare = attributes;
//                }
//                Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
//                return $(this);
//            } // save()
//        };

//    // Method calling logic
//    $.fn.CswFieldTypeNodeTypeSelect = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);





