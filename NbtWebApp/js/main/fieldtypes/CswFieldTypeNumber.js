/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function(o) {

            var $Div = $(this),
                propVals = o.propData.values,
                precision = tryParseNumber(propVals.precision, 6),
                ceilingVal = '999999999' + ChemSW.tools.getMaxValueForPrecision(precision);
            
            var $NumberTextBox = $Div.CswNumberTextBox({
                ID: o.ID,
                Value: (false === o.Multi) ? tryParseString(propVals.value).trim() : CswMultiEditDefaultValue,
                MinValue: tryParseNumber(propVals.minvalue),
                MaxValue: tryParseNumber(propVals.maxvalue),
                ceilingVal: ceilingVal,
                Precision: precision,
                ReadOnly: isTrue(o.ReadOnly),
                Required: isTrue(o.Required),
                onchange: o.onchange
            });

            if(!isNullOrEmpty($NumberTextBox) && $NumberTextBox.length > 0) {
                $NumberTextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function(o) { //$propdiv, $xml
            var attributes = { value: o.$propdiv.CswNumberTextBox('value', o.ID) };
            preparePropJsonForSave(o.Multi, o.propData, attributes);
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
