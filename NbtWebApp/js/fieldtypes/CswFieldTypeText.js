﻿; (function ($) {
        
    var PluginName = 'CswFieldTypeText';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

            var $Div = $(this);
            $Div.contents().remove();

            var Value = o.$propxml.children('text').text().trim();
            var Length = o.$propxml.children('text').CswAttrXml('length');

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $TextBox = $Div.CswInput('init', {ID: o.ID,
                                                        type: CswInput_Types.text,
                                                        value: Value,
                                                        cssclass: 'textinput',
                                                        width: Length,
                                                        onChange: o.onchange
                                                      });

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
