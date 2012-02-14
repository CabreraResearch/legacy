/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function (o) {

            var $Div = $(this),
                propVals = o.propData.values,
                precision = Csw.number(propVals.precision, 6),
                ceilingVal = '999999999' + Csw.getMaxValueForPrecision(precision);
            
            var $NumberTextBox = $Div.CswNumberTextBox({
                ID: o.ID,
                Value: (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue,
                MinValue: Csw.number(propVals.minvalue),
                MaxValue: Csw.number(propVals.maxvalue),
                ceilingVal: ceilingVal,
                Precision: precision,
                ReadOnly: Csw.bool(o.ReadOnly),
                Required: Csw.bool(o.Required),
                onChange: o.onChange
            });

            if(!Csw.isNullOrEmpty($NumberTextBox) && $NumberTextBox.length > 0) {
                $NumberTextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = { value: o.$propdiv.CswNumberTextBox('value', o.ID) };
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeNumber = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
