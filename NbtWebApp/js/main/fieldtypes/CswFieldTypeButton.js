/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypeButton';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

            var $Div = $(this);
            $Div.contents().remove();

            var propVals = o.propData.values;
            var value = tryParseString(propVals.text,o.propData.name);
            var mode = tryParseString(propVals.mode,'button');

            if(o.ReadOnly) {
                $Div.append(value);
            } else {
                var $Ctrl = "";
                if(mode==="button"){
                    $Ctrl = $Div.CswButton('init', {'ID': o.ID,
				                                        'enabledText': value,
				                                        'disabledText': value,
                                                        'disableOnClick': false,
				                                        'onclick': function () { alert('clicked!'); }
                                                      });
                }
                else{
                    $Ctrl = $Div.CswLink('init', {'ID': o.ID,
				                                        'value': value,
                                                        'href': '#',
				                                        'onClick': function() { alert('clicked!'); }
                                                      });
                }


                if(o.Required) {
                    $Ctrl.addClass("required");
                }
            }
        },
        save: function(o) {
            preparePropJsonForSave(o.propData);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeButton = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
