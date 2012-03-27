/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
                var valueNtb = propDiv.numberTextBox({
                    ID: o.ID + '_val',
                    value: (false === o.Multi) ? Csw.string(propVals.base).trim() : Csw.enums.multiEditDefaultValue,
                    MaxValue: 999999999,
                    Precision: 0,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    onChange: o.onChange,
                    width: '65px'
                });
                propDiv.append('E');
                var exponentNtb = propDiv.numberTextBox({
                    ID:  o.ID + '_exp',
                    value: (false === o.Multi) ? Csw.string(propVals.exponent).trim() : Csw.enums.multiEditDefaultValue,
                    MaxValue: 999999,
                    Precision: 0,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    onChange: o.onChange,
                    width: '40px'
                });

                if (valueNtb && valueNtb.length() > 0) {
                    valueNtb.clickOnEnter(o.saveBtn);
                }
                if (exponentNtb && exponentNtb.length() > 0) {
                    exponentNtb.clickOnEnter(o.saveBtn);
                }
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = {
                base: o.propDiv.find('#' + o.ID + '_val').val(),
                exponent: o.propDiv.find('#' + o.ID + '_exp').val()
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
