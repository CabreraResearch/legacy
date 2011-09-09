/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeSequence';

    var methods = {
        init: function(o) { 

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? tryParseString(propVals.sequence).trim() : CswMultiEditDefaultValue;

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
        save: function(o) {
            var attributes = {
                sequence: null
            };
            var $sequence = o.$propdiv.find('input');
            if (false === isNullOrEmpty($sequence)) {
                attributes.sequence = $sequence.val();
            }
            preparePropJsonForSave(o.Multi, o.propData.values, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeSequence = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
