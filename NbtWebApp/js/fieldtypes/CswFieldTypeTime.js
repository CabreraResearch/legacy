; (function ($) {
        
    var PluginName = 'CswFieldTypeTime';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

            var $Div = $(this);
            $Div.contents().remove();

            var Value = o.$propxml.children('value').text().trim();

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $TextBox = $('<input type="text" class="textinput validateTime" id="'+ o.ID +'" name="' + o.ID + '" value="'+ Value +'" />"' )
                                    .appendTo($Div)
                                    .change(o.onchange);
                var $nowbutton = $('<input type="button" id="'+ o.ID +'_now" value="Now" />"' )
                                    .appendTo($Div)
                                    .click(function() { $TextBox.val(getTimeString(new Date())); });
                
				jQuery.validator.addMethod( "validateTime", function(value, element) { 
                            return (this.optional(element) || validateTime($(element).val()));
                        }, 'Enter a valid time (e.g. 12:30 PM)');

				if(o.Required)
                {
                    $TextBox.addClass("required");
                }
            }
        },
        save: function(o) {
                var $TextBox = o.$propdiv.find('input');
                o.$propxml.children('value').text($TextBox.val());
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeTime = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
