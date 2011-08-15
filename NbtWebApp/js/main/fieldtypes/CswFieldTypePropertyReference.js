/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypePropertyReference';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 
                
                var $Div = $(this);
                $Div.contents().remove();
                 
                var text = tryParseString(o.propData.value).trim();
                text += '&nbsp;';

                var $StaticDiv = $('<div id="'+ o.ID +'" class="staticvalue">' + text + '</div>' )
                               .appendTo($Div); 
            },
        save: function(o) { //$propdiv, $xml
                // no changes to save
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypePropertyReference = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
