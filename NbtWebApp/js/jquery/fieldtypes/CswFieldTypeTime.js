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
                var $TextBox = $Div.CswInput('init',{ID: o.ID,
                                                      type: CswInput_Types.text,
                                                      cssclass: 'textinput validateTime',
                                                      onChange: o.onchange,
                                                      value: Value
                                                 }); 
                var $nowbutton = $Div.CswButton('init',{ 'ID': o.ID +'_now',
														'disableOnClick': false,
                                                        'onclick': function() { $TextBox.val(getTimeString(new Date())); },
                                                        'enabledText': 'Now'
                                                 }); 
                
				jQuery.validator.addMethod( "validateTime", function(value, element) { 
                            return (this.optional(element) || validateTime($(element).val()));
                        }, 'Enter a valid time (e.g. 12:30 PM)');

				if(o.Required)
                {
                    $TextBox.addClass("required");
                }
				$TextBox.clickOnEnter(o.$savebtn);
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
