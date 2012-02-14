/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeScientific';

    var methods = {
        init: function (o) { 

            var propDiv  = o.propDiv;
            propDiv.empty();
            var propVals = o.propData.values;
            if (Csw.bool(o.ReadOnly)) {
                propDiv.append(propVals.gestalt);
            } 
            else 
            {
                var $ValueNTB = propDiv.$.CswNumberTextBox({
                    ID: o.ID + '_val',
                    Value: (false === o.Multi) ? Csw.string(propVals.base).trim() : Csw.enums.multiEditDefaultValue,
                    MaxValue: 999999999,
                    Precision: 0,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    onChange: o.onChange,
                    width: '65px'
                });
                propDiv.append('E');
                var $ExponentNTB = propDiv.$.CswNumberTextBox({
                    ID:  o.ID + '_exp',
                    Value: (false === o.Multi) ? Csw.string(propVals.exponent).trim() : Csw.enums.multiEditDefaultValue,
                    MaxValue: 999999,
                    Precision: 0,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    onChange: o.onChange,
                    width: '40px'
                });

                if (false === Csw.isNullOrEmpty($ValueNTB) && $ValueNTB.length > 0) {
                    $ValueNTB.clickOnEnter(o.saveBtn);
                }
                if (false === Csw.isNullOrEmpty($ExponentNTB) && $ExponentNTB.length > 0) {
                    $ExponentNTB.clickOnEnter(o.saveBtn);
                }
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = {
                base: o.propDiv.$.CswNumberTextBox('value', o.ID + '_val'),
                exponent: o.propDiv.$.CswNumberTextBox('value', o.ID + '_exp')
            };
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeScientific = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
