/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeSequence';

    var methods = {
        init: function (o) { 

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.sequence).trim() : Csw.enums.multiEditDefaultValue;

            if (o.ReadOnly || o.Multi) {
                propDiv.append(value);
            } else {
                var textBox = propDiv.input({
                    ID: o.ID,
                    type: Csw.enums.inputTypes.text,
                    cssclass: 'textinput',
                    onChange: o.onChange,
                    value: value,
                    required: o.Required
                }); 

                if(o.Required) {
                    textBox.addClass('required');
                }
                textBox.clickOnEnter(o.saveBtn);
            }
        },
        save: function (o) {
            var attributes = {
                sequence: null
            };
            var compare = {};
            var sequence = o.propDiv.find('input');
            if (false === Csw.isNullOrEmpty(sequence)) {
                attributes.sequence = sequence.val();
                compare = attributes;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, compare);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeSequence = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
