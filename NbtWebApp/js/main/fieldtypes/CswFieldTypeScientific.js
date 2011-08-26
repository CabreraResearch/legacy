/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeScientific';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);

			if (isTrue(o.ReadOnly)) {
				$Div.append(o.propData.gestalt);
			} 
			else 
			{
				var $ValueNTB = $Div.CswNumberTextBox({
					ID: o.ID + '_val',
					Value: tryParseString(o.propData.base).trim(),
					Precision: 6,
					ReadOnly: o.ReadOnly,
					Required: o.Required,
					onchange: o.onchange,
					width: '60px'
				});
				$Div.append('E');
				var $ExponentNTB = $Div.CswNumberTextBox({
					ID:  o.ID + '_exp',
					Value: tryParseString(o.propData.exponent).trim(),
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
				o.propData.base = o.$propdiv.CswNumberTextBox('value', o.ID + '_val');
				o.propData.exponent =o.$propdiv.CswNumberTextBox('value', o.ID + '_exp');
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
