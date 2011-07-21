/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.2-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var PluginName = 'CswFieldTypeText';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

            var $Div = $(this);
            $Div.contents().remove();

            var Value = o.$propxml.children('text').text().trim();
            var Length = tryParseNumber( o.$propxml.children('text').CswAttrXml('length'), 14 );

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
                                                        width: Length * 7,
                                                        onChange: o.onchange
                                                      });

                if(o.Required)
                {
                    $TextBox.addClass("required");
                }

				$TextBox.clickOnEnter(o.$savebtn);
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
