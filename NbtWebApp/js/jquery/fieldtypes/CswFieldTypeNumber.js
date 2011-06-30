; (function ($) {
        
    var PluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);

			var $NumberTextBox = $Div.CswNumberTextBox({
				'ID': o.ID,
				'Value': o.$propxml.children('value').text().trim(),
				'MinValue': o.$propxml.children('value').CswAttrXml('minvalue'),
				'MaxValue': o.$propxml.children('value').CswAttrXml('maxvalue'),
				'Precision': o.$propxml.children('value').CswAttrXml('precision'),
				'ReadOnly': o.ReadOnly,
				'Required': o.Required,
				'onchange': o.onchange
			});

			if(!isNullOrEmpty($NumberTextBox) && $NumberTextBox.length > 0)
			{
				$NumberTextBox.clickOnEnter(o.$savebtn);
			}
        },
        save: function(o) { //$propdiv, $xml
				o.$propxml.children('value').text(o.$propdiv.CswNumberTextBox('value'));
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeNumber = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
