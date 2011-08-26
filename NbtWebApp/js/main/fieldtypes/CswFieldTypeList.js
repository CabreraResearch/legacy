/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeList';
    
    var methods = {
        init: function(o) {

            var $Div = $(this);
            $Div.contents().remove();

            var value = tryParseString(o.propData.value).trim();
            var options = tryParseString(o.propData.options).trim();

            if(o.ReadOnly)
            {
                $Div.append(value);
            }
            else 
            {
                var $SelectBox = $('<select id="'+ o.ID +'" name="'+ o.ID +'" class="selectinput" />"' )
                                    .appendTo($Div)
                                    .change(o.onchange);

                var splitOptions = options.split(',');
                for(var i = 0; i < splitOptions.length; i++)
                {
                    $SelectBox.append('<option value="' + splitOptions[i] + '">' + splitOptions[i] + '</option>');
                }
                $SelectBox.val( value );

                if(o.Required)
                {
                    $SelectBox.addClass("required");
                }
            }

        },
        save: function(o) { //$propdiv, $xml
                var $SelectBox = o.$propdiv.find('select');
                o.propData.value = $SelectBox.val();
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
