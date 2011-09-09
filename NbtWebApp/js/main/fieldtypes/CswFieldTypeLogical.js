/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../controls/CswTristateCheckBox.js" />

; (function ($) {
		
	var pluginName = 'CswFieldTypeLogical';

	var methods = {
		init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

			var $Div = $(this);
		    var propVals = o.propData.values;
		    var checkOpt = {
                    Checked: (false === o.Multi) ? tryParseString(propVals.checked).trim() : null,
                    Required: isTrue(o.Required),
                    ReadOnly: isTrue(o.ReadOnly),
		            Multi: o.Multi,
                    onchange: o.onchange
            };

			$Div.CswTristateCheckBox('init',checkOpt);
		},
		save: function(o) { 
			var $Div = o.$propdiv.find('div');
            var attributes = { checked: $Div.CswTristateCheckBox('value') };
		    preparePropJsonForSave(o.Multi, o, attributes);
		}
	};
	
	// Method calling logic
	$.fn.CswFieldTypeLogical = function (method) {
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
		}    
  
	};
})(jQuery);





