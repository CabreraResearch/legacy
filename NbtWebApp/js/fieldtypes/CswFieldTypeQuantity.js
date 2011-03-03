; (function ($) {
        
    var PluginName = 'CswFieldTypeQuantity';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.children().remove();

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
        save: function(o) { //$propdiv, $xml
//                var $TextBox = $propdiv.find('input');
//                $xml.children('barcode').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeQuantity = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
