/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeQuantity';

    var methods = {
        init: function(o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
			var $NumberTextBox = $Div.CswNumberTextBox({
				ID: o.ID + '_qty',
				Value: tryParseString(propVals.value).trim(),
				MinValue: tryParseString(propVals.minvalue),
				MaxValue: tryParseString(propVals.maxvalue),
				Precision: tryParseString(propVals.precision),
				ReadOnly: isTrue(o.ReadOnly),
				Required: isTrue(o.Required),
				onchange: o.onchange
			});
			
			if(!isNullOrEmpty($NumberTextBox) && $NumberTextBox.length > 0)
			{
				$NumberTextBox.clickOnEnter(o.$savebtn);
			}

            //this is an array
            var units = propVals.units;
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
			var propVals = o.propData.values;	
            propVals.value = o.$propdiv.CswNumberTextBox('value', o.ID + '_qty');
			var unit = o.$propdiv.find('#' + o.ID + '_units').val();
			propVals.units = unit;
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
