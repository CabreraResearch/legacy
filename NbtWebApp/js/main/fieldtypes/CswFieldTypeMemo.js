/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeMemo';

    var methods = {
        init: function(o) {
        
            var $Div = $(this);
            $Div.contents().remove();

            //var Value = extractCDataValue($xml.children('text'));
            var value = tryParseString(o.propData.text).trim();
            var rows = tryParseString(o.propData.rows);
            var columns = tryParseString(o.propData.columns);

            if(o.ReadOnly)
            {
                $Div.append(value);
            }
            else 
            {
                var $TextArea = $('<textarea id="'+ o.ID +'" name="' + o.ID + '" rows="'+rows+'" cols="'+columns+'">'+ value +'</textarea>' )
                                    .appendTo($Div)
                                    .change(o.onchange); 
                if(o.Required)
                {
                    $TextArea.addClass("required");
                }
            }

        },
        save: function(o) { //$propdiv, $xml
                var $TextArea = o.$propdiv.find('textarea');
                o.propData.text = $TextArea.val();
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeMemo = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
