/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function(o) {

            var $Div = $(this);
            var propVals = o.propData.values;
			var $NumberTextBox = $Div.CswNumberTextBox({
				ID: o.ID,
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
        },
        save: function(o) { //$propdiv, $xml
			o.propData.values.value = o.$propdiv.CswNumberTextBox('value', o.ID);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeNumber = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
