; (function ($) {
        
    var PluginName = 'CswFieldTypeQuantity';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();
            
			var $NumberTextBox = $Div.CswNumberTextBox({
				'ID': o.ID + '_qty',
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

			var selectedUnit = o.$propxml.children('units').contents().first().text();
			var $unitsel = $('<select id="'+ o.ID + '_units" />')
							.appendTo($Div)
							.change(o.onchange);
			o.$propxml.children('units').children().each(function() {
				var unit = $(this).CswAttrXml('value');
				var $option = $('<option value="' + unit + '">' + unit + '</option>')
								.appendTo($unitsel);
				if(selectedUnit === unit)
				{
					$option.CswAttrDom('selected', 'true');
				}
			});

        },
        save: function(o) {
				o.$propxml.children('value').text(o.$propdiv.CswNumberTextBox('value'));
				var unit = o.$propdiv.find('#' + o.ID + '_units').val();
				o.$propxml.children('units').text(unit);
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeQuantity = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
