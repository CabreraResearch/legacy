; (function ($) {
        
    var PluginName = 'CswFieldTypeList';
    var $propxml;
    var $Div;
    var $SelectBox;

    var methods = {
        init: function(nodepk, $xml) {
                $propxml = $xml;

                $Div = $(this);
                $Div.children().remove();

                var ID = $propxml.attr('id');
                var Required = $propxml.attr('required');
                var ReadOnly = $propxml.attr('readonly');

                var Value = $propxml.children('value').text();
                var Options = $propxml.children('options').text();

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    $SelectBox = $('<select id="'+ ID +'" name="'+ ID +'" />"' )
                                        .appendTo($Div);
            
                    var SplitOptions = Options.split(',')
                    for(var i = 0; i < SplitOptions.length; i++)
                    {
                        $SelectBox.append('<option value="'+SplitOptions[i]+'">'+SplitOptions[i]+'</option>');
                    }
                    $SelectBox.val( Value );

                    if(Required)
                    {
                        $SelectBox.addClass("required");
                    }
                }

            },
        save: function() {
                $propxml.children('value').text($SelectBox.val());
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
