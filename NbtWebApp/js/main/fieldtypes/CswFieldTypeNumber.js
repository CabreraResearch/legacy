/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function (o) {

            var propDiv = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values,
                precision = Csw.number(propVals.precision, 6),
                ceilingVal = '999999999' + Csw.getMaxValueForPrecision(precision);
            
            var $NumberTextBox = propDiv.$.CswNumberTextBox({
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

            if(false === Csw.isNullOrEmpty($NumberTextBox) && $NumberTextBox.length > 0) {
                $NumberTextBox.clickOnEnter(o.saveBtn);
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = { value: o.propDiv.$.CswNumberTextBox('value', o.ID) };
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
