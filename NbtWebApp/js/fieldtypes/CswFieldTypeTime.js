; (function ($) {
        
    var PluginName = 'CswFieldTypeTime';

    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = ($xml.attr('required') == "true");
                var ReadOnly = ($xml.attr('readonly') == "true");

                var Value = $xml.children('value').text();

                $Div.append('[Not Implemented Yet]');
//                if(ReadOnly)
//                {
//                    $Div.append(Value);
//                }
//                else 
//                {
//                    
//                }
            },
        save: function($propdiv, $xml) {
//                var $TextBox = $propdiv.find('input');
//                $xml.children('barcode').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeTime = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
