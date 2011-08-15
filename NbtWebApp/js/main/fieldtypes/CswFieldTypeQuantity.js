/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeQuantity';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.$propxml, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();
            
			var $NumberTextBox = $Div.CswNumberTextBox({
				ID: o.ID + '_qty',
				Value: tryParseString(o.propData.value).trim(),
				MinValue: tryParseString(o.propData.minvalue),
				MaxValue: tryParseString(o.propData.maxvalue),
				Precision: tryParseString(o.propData.precision),
				ReadOnly: isTrue(o.ReadOnly),
				Required: isTrue(o.Required),
				onchange: o.onchange
			});
			
			if(!isNullOrEmpty($NumberTextBox) && $NumberTextBox.length > 0)
			{
				$NumberTextBox.clickOnEnter(o.$savebtn);
			}

            //this is an array
            var units = o.propData.units;
            var selectedUnit = units[0];
			var $unitsel = $('<select id="'+ o.ID + '_units" />')
							.appendTo($Div)
							.change(o.onchange);
			for (var i=0; i < units.length; i++) {
			    var unit = units[i];
			    var $option = $('<option value="' + unit + '">' + unit + '</option>')
    			                .appendTo($unitsel);
			    if (selectedUnit === unit)
			    {
			        $option.CswAttrDom('selected', 'true');
			    }
			}

        },
        save: function(o) {
				o.propData.value = o.$propdiv.CswNumberTextBox('value', o.ID + '_qty');
				var unit = o.$propdiv.find('#' + o.ID + '_units').val();
				o.propData.units = unit;
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeQuantity = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
