; (function ($) {
        
    var PluginName = 'CswFieldTypeList';
    
    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            var Value = o.$propxml.children('value').text().trim();
            var Options = o.$propxml.children('options').text().trim();

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $SelectBox = $('<select id="'+ o.ID +'" name="'+ o.ID +'" class="selectinput" />"' )
                                    .appendTo($Div)
                                    .change(o.onchange);
            
                var SplitOptions = Options.split(',')
                for(var i = 0; i < SplitOptions.length; i++)
                {
                    $SelectBox.append('<option value="' + SplitOptions[i] + '">' + SplitOptions[i] + '</option>');
                }
                $SelectBox.val( Value );

                if(o.Required)
                {
                    $SelectBox.addClass("required");
                }
            }

        },
        save: function(o) { //$propdiv, $xml
                var $SelectBox = o.$propdiv.find('select');
                o.$propxml.children('value').text($SelectBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeList = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
