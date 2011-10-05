/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeScientific';

    var methods = {
        init: function(o) { 

            var $Div = $(this);
            var propVals = o.propData.values;
			if (isTrue(o.ReadOnly)) {
				$Div.append(propVals.gestalt);
			} 
			else 
			{
				var $ValueNTB = $Div.CswNumberTextBox({
					ID: o.ID + '_val',
					Value: (false === o.Multi) ? tryParseString(propVals.base).trim() : CswMultiEditDefaultValue,
					Precision: 6,
					ReadOnly: o.ReadOnly,
					Required: o.Required,
					onchange: o.onchange,
					width: '60px'
				});
				$Div.append('E');
				var $ExponentNTB = $Div.CswNumberTextBox({
					ID:  o.ID + '_exp',
					Value: (false === o.Multi) ? tryParseString(propVals.exponent).trim() : CswMultiEditDefaultValue,
					ReadOnly: o.ReadOnly,
					Required: o.Required,
					onchange: o.onchange,
					width: '25px'
				});

				if (!isNullOrEmpty($ValueNTB) && $ValueNTB.length > 0) {
					$ValueNTB.clickOnEnter(o.$savebtn);
				}
				if (!isNullOrEmpty($ExponentNTB) && $ExponentNTB.length > 0) {
					$ExponentNTB.clickOnEnter(o.$savebtn);
				}
			}
        },
        save: function(o) { //$propdiv, $xml
            var attributes = {
                base: o.$propdiv.CswNumberTextBox('value', o.ID + '_val'),
                exponent: o.$propdiv.CswNumberTextBox('value', o.ID + '_exp')
            };
            preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeScientific = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
