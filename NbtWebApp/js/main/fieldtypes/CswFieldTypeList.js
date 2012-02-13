/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeList';
    
    var methods = {
        init: function (o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue;
            var options = Csw.string(propVals.options).trim();

            if(o.ReadOnly) {
                $Div.append(value);
            } else {
                var values = options.split(',');
                if (o.Multi) {
                    values.push(Csw.enums.multiEditDefaultValue);
                }
                var $SelectBox = $Div.CswSelect('init', { ID: o.ID, 
                                                          cssclass: 'selectinput', 
                                                          onChange: o.onChange,
                                                          values: values,
                                                          selected: value
                                      });

                if(o.Required) {
                    $SelectBox.addClass("required");
                }
            }

        },
        save: function (o) { //$propdiv, $xml
            var attributes = { value: null };
            var $SelectBox = o.$propdiv.find('select');
            if (false === Csw.isNullOrEmpty($SelectBox)) {
                attributes.value = $SelectBox.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeList = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
