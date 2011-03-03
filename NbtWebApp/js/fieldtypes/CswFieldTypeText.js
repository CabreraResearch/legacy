; (function ($) {
        
    var PluginName = 'CswFieldTypeText';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

            var $Div = $(this);
            $Div.children().remove();

            var Value = o.$propxml.children('text').text().trim();
            var Length = o.$propxml.children('text').attr('length');

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $TextBox = $('<input type="text" class="textinput" size="' + Length + '" id="'+ o.ID +'" name="' + o.ID + '" value="'+ Value +'" />"' )
                                    .appendTo($Div)
                                    .change(o.onchange);
                if(o.Required)
                {
                    $TextBox.addClass("required");
                }
            }
        },
        save: function(o) {
                var $TextBox = o.$propdiv.find('input');
                o.$propxml.children('text').text($TextBox.val());
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
