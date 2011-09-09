/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeList';
    
    var methods = {
        init: function(o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? tryParseString(propVals.value).trim() : CswMultiEditDefaultValue;
            var options = tryParseString(propVals.options).trim();

            if(o.ReadOnly) {
                $Div.append(value);
            } else {
                var values = options.split(',');
                if (o.Multi) {
                    values.push(CswMultiEditDefaultValue);
                }
                var $SelectBox = $Div.CswSelect('init', { ID: o.ID, 
                                                          cssclass: 'selectinput', 
                                                          onChange: o.onchange,
                                                          values: values,
                                                          selected: value
                                      });

                if(o.Required) {
                    $SelectBox.addClass("required");
                }
            }

        },
        save: function(o) { //$propdiv, $xml
            var attributes = { value: null };
            var $SelectBox = o.$propdiv.find('select');
            if (false === isNullOrEmpty($SelectBox)) {
                attributes.value = $SelectBox.val();
            }
            preparePropJsonForSave(o.Multi, o.propData.values, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeList = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
