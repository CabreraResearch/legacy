/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />

(function ($) {
        
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
            var attributes, $DTPickerDiv, dateVal;
            attributes = { 
                value: {
                    date: null,
                    time: null
                } 
            };
            $DTPickerDiv = o.$propdiv.find('#' + o.ID);
            if (false === isNullOrEmpty($DTPickerDiv)) {
                dateVal = $DTPickerDiv.CswDateTimePicker('value', o.propData.readonly);
                attributes.value.date = dateVal.Date;
                attributes.value.time = dateVal.Time;
            }
            preparePropJsonForSave(o.Multi, o.propData, attributes);
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
