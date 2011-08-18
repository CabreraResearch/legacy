/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeDateTime';

    var methods = {
        init: function(o) {

            var $Div = $(this);

			var $DTPickerDiv = $Div.CswDateTimePicker('init', {
													ID: o.ID,
													Date: tryParseString(o.propData.value.date).trim(),
													Time: tryParseString(o.propData.value.time).trim(),
													DateFormat: ServerDateFormatToJQuery(o.propData.value.dateformat),
													TimeFormat: ServerTimeFormatToJQuery(o.propData.value.timeformat),
													DisplayMode: o.propData.value.displaymode,
													ReadOnly: o.ReadOnly,
													Required: o.Required,
													OnChange: o.onchange
												});

            $DTPickerDiv.find('input').clickOnEnter(o.$savebtn);
        },
        save: function(o) { //$propdiv, $xml
            var $DTPickerDiv = o.$propdiv.find('#' + o.ID);
			var dateval = $DTPickerDiv.CswDateTimePicker('value');
			o.propData.value.date = dateval.Date;
			o.propData.value.time = dateval.Time;
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeDateTime = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
