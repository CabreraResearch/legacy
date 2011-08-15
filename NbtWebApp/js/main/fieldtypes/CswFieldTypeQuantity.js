; (function ($) {
        
    var PluginName = 'CswFieldTypeQuantity';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();
            
			var $NumberTextBox = $Div.CswNumberTextBox({
				'ID': o.ID + '_qty',
				'Value': o.propData.children('value').text().trim(),
				'MinValue': o.propData.children('value').CswAttrXml('minvalue'),
				'MaxValue': o.propData.children('value').CswAttrXml('maxvalue'),
				'Precision': o.propData.children('value').CswAttrXml('precision'),
				'ReadOnly': o.ReadOnly,
				'Required': o.Required,
				'onchange': o.onchange
			});
			
			if(!isNullOrEmpty($NumberTextBox) && $NumberTextBox.length > 0)
			{
				$NumberTextBox.clickOnEnter(o.$savebtn);
			}

			var selectedUnit = o.propData.children('units').contents().first().text();
			var $unitsel = $('<select id="'+ o.ID + '_units" />')
							.appendTo($Div)
							.change(o.onchange);
			o.propData.children('units').children().each(function() {
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
				o.propData.children('value').text(o.$propdiv.CswNumberTextBox('value', o.ID + '_qty'));
				var unit = o.$propdiv.find('#' + o.ID + '_units').val();
				o.propData.children('units').text(unit);
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
