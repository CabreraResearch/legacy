; (function ($) {
        
    var PluginName = 'CswFieldTypeMemo';

    var methods = {
        init: function(nodepk, $xml, onchange) {
        
                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = ($xml.attr('required') == "true");
                var ReadOnly = ($xml.attr('readonly') == "true");

                //var Value = extractCDataValue($xml.children('text'));
                var Value = $xml.children('text').text().trim();
                var rows = $xml.children('text').attr('rows');
                var columns = $xml.children('text').attr('columns');

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $TextArea = $('<textarea id="'+ ID +'" name="' + ID + '" rows="'+rows+'" cols="'+columns+'">'+ Value +'</textarea>' )
                                     .appendTo($Div)
                                     .change(onchange); 
                    if(Required)
                    {
                        $TextArea.addClass("required");
                    }
                }

            },
        save: function($propdiv, $xml) {
                var $TextArea = $propdiv.find('textarea');
                $xml.children('text').text($TextArea.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeMemo = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
