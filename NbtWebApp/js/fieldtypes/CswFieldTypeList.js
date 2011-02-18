; (function ($) {
        
    var PluginName = 'CswFieldTypeList';
    
    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = ($xml.attr('required') == "true");
                var ReadOnly = ($xml.attr('readonly') == "true");

                var Value = $xml.children('value').text();
                var Options = $xml.children('options').text();

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $SelectBox = $('<select id="'+ ID +'" name="'+ ID +'" class="selectinput" />"' )
                                        .appendTo($Div);
            
                    var SplitOptions = Options.split(',')
                    for(var i = 0; i < SplitOptions.length; i++)
                    {
                        $SelectBox.append('<option value="' + SplitOptions[i] + '">' + SplitOptions[i] + '</option>');
                    }
                    $SelectBox.val( Value );

                    if(Required)
                    {
                        $SelectBox.addClass("required");
                    }
                }

            },
        save: function($propdiv, $xml) {
                var $SelectBox = $propdiv.find('select');
                $xml.children('value').text($SelectBox.val());
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
