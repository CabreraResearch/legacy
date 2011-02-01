; (function ($) {
        
    var PluginName = 'CswFieldTypeMemo';
    var $propxml;
    var $Div;
    var $TextArea;

    var methods = {
        init: function(nodepk, $xml) {
                $propxml = $xml;

                $Div = $(this);
                $Div.children().remove();

                var ID = $propxml.attr('id');
                var Required = $propxml.attr('required');
                var ReadOnly = $propxml.attr('readonly');

                var Value = extractCDataValue($propxml.children('text'));
                var rows = $propxml.children('text').attr('rows');
                var columns = $propxml.children('text').attr('columns');

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    $TextArea = $('<textarea id="'+ ID +'" name="' + ID + '" rows="'+rows+'" cols="'+columns+'">'+ Value +'</textarea>' )
                                  .appendTo($Div); 
                    if(Required)
                    {
                        $TextArea.addClass("required");
                    }
                }

            },
        save: function() {
                $propxml.children('text').text($TextArea.val());
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
