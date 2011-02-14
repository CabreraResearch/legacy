; (function ($) {
        
    var PluginName = 'CswFieldTypeDate';

    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = $xml.attr('required');
                var ReadOnly = $xml.attr('readonly');

                var Value = $xml.children('value').text();
                if(Value == '1/1/0001')
                    Value = '';

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $TextBox = $('<input type="text" class="textinput date" id="'+ ID +'" name="' + ID + '" value="'+ Value +'" />"' )
                                     .appendTo($Div)
                                     .datepicker();
                    if(Required)
                    {
                        $TextBox.addClass("required");
                    }
                }
            },
        save: function($propdiv, $xml) {
                var $TextBox = $propdiv.find('input');
                $xml.children('value').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeDate = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
