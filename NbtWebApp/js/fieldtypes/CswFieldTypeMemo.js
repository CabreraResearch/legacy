; (function ($) {
        
    var PluginName = 'CswFieldTypeMemo';
    var $propxml;

    var methods = {
        init: function(nodepk, $xml) {
                $propxml = $xml;

                var $Div = $(this);
                $Div.children().remove();

                var ID = $propxml.attr('id');
                var Required = $propxml.attr('required');
                var ReadOnly = $propxml.attr('readonly');

                var Value = $propxml.children('text').text();
                var rows = $propxml.children('text').attr('rows');
                var columns = $propxml.children('text').attr('columns');

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $TextArea = $('<textarea id="'+ ID +'" name="' + ID + '" rows="'+rows+'" cols="'+columns+'">'+ Value +'</textarea>' )
                                     .appendTo($Div); 
                    if(Required)
                    {
                        $TextArea.addClass("required");
                    }
                }

            },
        save: function() {
                var $Div = $(this);
                var o = $Div.data(PluginName);
                //var $newPropXml = $propxml;
                //$newPropXml.children('text').text($TextBox.val());
                //SaveProp(nodepk, $newPropXml, function () { });
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
