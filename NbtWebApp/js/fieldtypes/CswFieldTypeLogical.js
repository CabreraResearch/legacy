; (function ($) {
		
	var PluginName = 'CswFieldTypeLogical';

	var methods = {
		init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

			$Div = $(this);
			var checkOpt = {
                    Checked: o.$propxml.children('checked').text().trim(),
                    '$propxml': o.$propxml
            };

			$Div.CswTristateCheckBox('init',checkOpt);
		},
		save: function(o) { //$propdiv, $xml
			$Div = $(this);
            var checkOpt = {
                    '$propxml': o.$propxml
            };
            $Div.CswTristateCheckBox('save',checkOpt);
		}
	};
	
	// Method calling logic
	$.fn.CswFieldTypeLogical = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
})(jQuery);





