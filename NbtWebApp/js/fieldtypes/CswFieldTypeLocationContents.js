; (function ($) {
        
    var PluginName = 'CswFieldTypeLocationContents';

    var methods = {
        init: function(o) { // nodepk, $xml, onchange

                var $Div = $(this);
                $Div.contents().remove();

                var Value = o.$propxml.children('value').text().trim();

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
//                var $TextBox = $propdiv.find('input');
//                $xml.children('barcode').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeLocationContents = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
