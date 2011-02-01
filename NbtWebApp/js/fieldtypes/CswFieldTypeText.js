; (function ($) {
        
    var PluginName = 'CswFieldTypeText';
    var $propxml;
    var $Div;
    var $TextBox;

    var methods = {
        init: function(nodepk, $xml) {
                $propxml = $xml;

                $Div = $(this);
                $Div.children().remove();

                var ID = $propxml.attr('id');
                var Required = $propxml.attr('required');
                var ReadOnly = $propxml.attr('readonly');

                var Value = $propxml.children('text').text();
                var Length = $propxml.children('text').attr('length');

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    $TextBox = $('<input type="text" class="textinput" size="' + Length + '" id="'+ ID +'" name="' + ID + '" value="'+ Value +'" />"' )
                                 .appendTo($Div);
                    if(Required)
                    {
                        $TextBox.addClass("required");
                    }
                }
            },
        save: function() {
                $propxml.children('text').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeText = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
