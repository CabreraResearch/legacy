/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswFieldTypeNodeTypeSelect',
        nameCol = 'label',
        keyCol = 'key',
        valueCol = 'value',
        methods = {
            init: function (o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

                var $Div = $(this);

                var propVals = o.propData.values;
                var optData = propVals.options;
                var selectMode = propVals.selectmode; // Single, Multiple, Blank
                var editMode = Csw.enums.tryParse(Csw.enums.editMode, o.EditMode);

                /*
                Case 24606: Once we can validate the control
                if(editMode === Csw.enums.editMode.AddInPopup) {
                    Csw.each(propVals.options, function (option) {
                        if (Csw.contains(option, 'key')) {
                            
                              var relatedNodeTypeId = Csw.string(o.relatednodetypeid);
                              if (Csw.string(option.key) === relatedNodeTypeId) 
                              one day we can try to set the defaults using the context of the view. Not today.
                            
                                option.value = 'False';
                        }
                    });
                }
                */
                var $cbaDiv = $('<div />')
                                .CswCheckBoxArray('init', {
                                    ID: o.ID + '_cba',
                                    UseRadios: (selectMode === 'Single'),
                                    Required: o.Required,
                                    ReadOnly: o.ReadOnly,
                                    Multi: o.Multi,
                                    onchange: o.onchange,
                                    dataAry: optData,
                                    nameCol: nameCol,
                                    keyCol: keyCol,
                                    valCol: valueCol,
                                    valColName: 'Include'
                                });
                
                if(o.Required) {
                    $cbaDiv.addClass("required");
                }
                
                $Div.contents().remove();
                $Div.append($cbaDiv);            
                return $Div;
            },
            save: function (o) { //$propdiv, $xml
                var attributes = { options: null };
                var $cbaDiv = o.$propdiv.children('div').first();
                var formdata = $cbaDiv.CswCheckBoxArray( 'getdata', { 'ID': o.ID + '_cba' } );
                if(false === o.Multi || false === formdata.MultiIsUnchanged) {
                    attributes.options = formdata.data;
                } 
                Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
                return $(this);
            } // save()
    };

    // Method calling logic
    $.fn.CswFieldTypeNodeTypeSelect = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);





