; (function ($) {
        
    var PluginName = 'CswFieldTypePropertyReference';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 
                
                var $Div = $(this);
                $Div.children().remove();
                 
                var Text = o.$propxml.children('value').text().trim();
                Text += '&nbsp;';

                var $StaticDiv = $('<div id="'+ o.ID +'" class="staticvalue">' + Text + '</div>' )
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
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
