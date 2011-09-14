/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeQuantity';

    var methods = {
        init: function(o) {

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
			var $NumberTextBox = $Div.CswNumberTextBox({
				ID: o.ID + '_qty',
				Value: (false === o.Multi) ? tryParseString(propVals.value).trim() : CswMultiEditDefaultValue,
				MinValue: tryParseString(propVals.minvalue),
				MaxValue: tryParseString(propVals.maxvalue),
				Precision: tryParseString(propVals.precision),
				ReadOnly: isTrue(o.ReadOnly),
				Required: isTrue(o.Required),
				onchange: o.onchange
			});
			
			if(!isNullOrEmpty($NumberTextBox) && $NumberTextBox.length > 0) {
				$NumberTextBox.clickOnEnter(o.$savebtn);
			}

            //this is an array
            var units = propVals.units;
            var selectedUnit = units[0];
            if (o.Multi) {
                units.push(CswMultiEditDefaultValue);
                selectedUnit = CswMultiEditDefaultValue;
            }
            
            $Div.CswSelect('init', {
                    ID: o.ID,
                    onChange: o.onchange,
                    values: units,
                    selected: selectedUnit
                }); 
        },
        save: function(o) {
            var attributes = {
                value: o.$propdiv.CswNumberTextBox('value', o.ID + '_qty'),
                units: null
            };
            
            var $unit = o.$propdiv.find('#' + o.ID + '_units');
            if (false === isNullOrEmpty($unit)) {
                attributes.units = $unit.val();
            } 
            preparePropJsonForSave(o.Multi, o.propData, attributes);
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
