/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeLocationContents';

    var methods = {
        init: function(o) { // nodepk, $xml, onchange

                var $Div = $(this);
                $Div.contents().remove();

                var value = tryParseString(o.propData.value).trim();

                $Div.append('[Not Implemented Yet]');
//                if(o.ReadOnly)
//                {
//                    $Div.append(Value);
//                }
//                else 
//                {
//                    
//                }
            },
        save: function(o) {
//          var $TextBox = $propdiv.find('input');
//          $xml.children('barcode').text($TextBox.val());
            preparePropJsonForSave(o.propData);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeLocationContents = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
