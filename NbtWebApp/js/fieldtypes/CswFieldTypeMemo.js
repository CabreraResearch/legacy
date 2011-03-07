; (function ($) {
        
    var PluginName = 'CswFieldTypeMemo';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 
        
            var $Div = $(this);
            $Div.contents().remove();

            //var Value = extractCDataValue($xml.children('text'));
            var Value = o.$propxml.children('text').text().trim();
            var rows = o.$propxml.children('text').attr('rows');
            var columns = o.$propxml.children('text').attr('columns');

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $TextArea = $('<textarea id="'+ o.ID +'" name="' + o.ID + '" rows="'+rows+'" cols="'+columns+'">'+ Value +'</textarea>' )
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
                o.$propxml.children('text').text($TextArea.val());
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
