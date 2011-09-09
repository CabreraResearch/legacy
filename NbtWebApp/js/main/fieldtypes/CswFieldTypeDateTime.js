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
            var propVals = o.propData.values;
            var date = (false === o.Multi) ? tryParseString(propVals.value.date).trim() : CswMultiEditDefaultValue;
            var time = (false === o.Multi) ? tryParseString(propVals.value.time).trim() : CswMultiEditDefaultValue;
            
            var $DTPickerDiv = $Div.CswDateTimePicker('init', {
													ID: o.ID,
													Date: date,
													Time: time,
													DateFormat: ServerDateFormatToJQuery(propVals.value.dateformat),
													TimeFormat: ServerTimeFormatToJQuery(propVals.value.timeformat),
													DisplayMode: propVals.displaymode,
													ReadOnly: o.ReadOnly,
													Required: o.Required,
													OnChange: o.onchange
												});

            $DTPickerDiv.find('input').clickOnEnter(o.$savebtn);
        },
        save: function(o) { //$propdiv, $xml
            var attributes = { value: null };
            var $DTPickerDiv = o.$propdiv.find('#' + o.ID);
			if (false === isNullOrEmpty($DTPickerDiv)) {
			    attributes.value = $DTPickerDiv.CswDateTimePicker('value');
			}
            var subSubFunc = function(dateval, key) {
                if (key === 'value') {
                    dateval.date = attributes.value.Date;
                    dateval.time = attributes.value.Time;
                }
            };
            preparePropJsonForSave(o.Multi, o, attributes, subSubFunc);
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
