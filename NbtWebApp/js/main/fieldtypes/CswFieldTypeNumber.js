/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);

			var $NumberTextBox = $Div.CswNumberTextBox({
				ID: o.ID,
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
        },
        save: function(o) { //$propdiv, $xml
				o.propData.value = o.$propdiv.CswNumberTextBox('value', o.ID);
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
