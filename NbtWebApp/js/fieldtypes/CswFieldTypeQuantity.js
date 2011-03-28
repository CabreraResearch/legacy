; (function ($) {
        
    var PluginName = 'CswFieldTypeQuantity';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();
            
			$Div.CswNumberTextBox({
				'ID': o.ID + '_qty',
				'Value': o.$propxml.children('value').text().trim(),
				'MinValue': o.$propxml.children('value').attr('minvalue'),
				'MaxValue': o.$propxml.children('value').attr('maxvalue'),
				'Precision': o.$propxml.children('value').attr('precision'),
				'ReadOnly': o.ReadOnly,
				'Required': o.Required,
				'onchange': o.onchange
			});
			
			var selectedUnit = o.$propxml.children('units').contents().first().text();
			var $unitsel = $('<select id="'+ o.ID + '_units" />')
							.appendTo($Div)
							.change(o.onchange);
			o.$propxml.children('units').children().each(function() {
				var unit = $(this).attr('value');
				var $option = $('<option value="' + unit + '">' + unit + '</option>')
								.appendTo($unitsel);
				if(selectedUnit == unit)
				{
					$option.attr('selected', 'true');
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
