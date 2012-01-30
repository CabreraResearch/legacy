/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeSequence';

    var methods = {
        init: function (o) { 

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.sequence).trim() : CswMultiEditDefaultValue;

            if (o.ReadOnly || o.Multi) {
                $Div.append(value);
            } else {
                var $TextBox = $Div.CswInput('init',{ID: o.ID,
                                                      type: CswInput_Types.text,
                                                      cssclass: 'textinput',
                                                      onChange: o.onchange,
                                                      value: value
                                                 }); 

                if(o.Required) {
                    $TextBox.addClass("required");
                }
                $TextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function (o) {
            var attributes = {
                sequence: null
            };
            var $sequence = o.$propdiv.find('input');
            if (false === Csw.isNullOrEmpty($sequence)) {
                attributes.sequence = $sequence.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
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
