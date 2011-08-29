/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var PluginName = 'CswFieldTypeButton';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey

            var $Div = $(this);
            $Div.contents().remove();

            var Value = o.$propxml.children('text').text().trim();
            var Mode = o.$propxml.children('text').CswAttrXml('mode');

            if(o.ReadOnly)
            {
                $Div.append(Value);
            }
            else 
            {
                var $Ctrl = "";
                if(Mode.toString().toLowerCase()=="button"){
                    $Ctrl = $Div.CswButton('init', {'ID': o.ID,
				                                        'enabledText': Value,
				                                        'disabledText': Value,
				                                        'onclick': function () { alert('clicked!'); }
                                                      });
                }
                else{
                    $Ctrl = $Div.CswLink('init', {'ID': o.ID,
				                                        'value': Value,
                                                        'href': '#',
				                                        'onClick': function() { alert('clicked!'); }
                                                      });
                }


                if(o.Required)
                {
                    $Ctrl.addClass("required");
                }

            }
        },
        save: function(o) {
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeButton = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
