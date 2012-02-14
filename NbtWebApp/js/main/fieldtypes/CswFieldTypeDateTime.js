/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";    
    var pluginName = 'CswFieldTypeDateTime';

    var methods = {
        init: function (o) {

            var $Div = $(this);
            var propVals = o.propData.values;
            var date = (false === o.Multi) ? Csw.string(propVals.value.date).trim() : Csw.enums.multiEditDefaultValue;
            var time = (false === o.Multi) ? Csw.string(propVals.value.time).trim() : Csw.enums.multiEditDefaultValue;
            
            if(o.ReadOnly) {
                $Div.append(o.propData.gestalt);    
            } else {
                var $DTPickerDiv = $Div.CswDateTimePicker('init', {
                    ID: o.ID,
                    Date: date,
                    Time: time,
                    DateFormat: Csw.serverDateFormatToJQuery(propVals.value.dateformat),
                    TimeFormat: Csw.serverTimeFormatToJQuery(propVals.value.timeformat),
                    DisplayMode: propVals.displaymode,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    OnChange: o.onChange
                });

                $DTPickerDiv.find('input').clickOnEnter(o.$savebtn);
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes, $DTPickerDiv, dateVal;
            attributes = { 
                value: {
                    date: null,
                    time: null
                } 
            };
            $DTPickerDiv = o.$propdiv.find('#' + o.ID);
            if (false === Csw.isNullOrEmpty($DTPickerDiv)) {
                dateVal = $DTPickerDiv.CswDateTimePicker('value', o.propData.readonly);
                attributes.value.date = dateVal.date;
                attributes.value.time = dateVal.time;
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeDateTime = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
