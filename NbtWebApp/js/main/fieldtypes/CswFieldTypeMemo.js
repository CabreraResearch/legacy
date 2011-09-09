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
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? tryParseString(propVals.text).trim() : CswMultiEditDefaultValue;
            var rows = tryParseString(propVals.rows);
            var columns = tryParseString(propVals.columns);

            if(o.ReadOnly) {
                $Div.append(value);
            } else {
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
            var attributes = { text: null };
            var $TextArea = o.$propdiv.find('textarea');
            if (false === isNullOrEmpty($TextArea)) {
                attributes.text = $TextArea.val();
            }
            preparePropJsonForSave(o.Multi, o.propData.values, attributes);
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
