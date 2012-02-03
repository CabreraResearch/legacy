/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";        
    var pluginName = 'CswFieldTypeScientific';

    var methods = {
        init: function (o) { 

            var $Div = $(this);
            var propVals = o.propData.values;
            if (Csw.bool(o.ReadOnly)) {
                $Div.append(propVals.gestalt);
            } 
            else 
            {
                var $ValueNTB = $Div.CswNumberTextBox({
                    ID: o.ID + '_val',
                    Value: (false === o.Multi) ? Csw.string(propVals.base).trim() : Csw.enums.multiEditDefaultValue,
                    MaxValue: 999999999,
                    Precision: 0,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    onchange: o.onchange,
                    width: '65px'
                });
                $Div.append('E');
                var $ExponentNTB = $Div.CswNumberTextBox({
                    ID:  o.ID + '_exp',
                    Value: (false === o.Multi) ? Csw.string(propVals.exponent).trim() : Csw.enums.multiEditDefaultValue,
                    MaxValue: 999999,
                    Precision: 0,
                    ReadOnly: o.ReadOnly,
                    Required: o.Required,
                    onchange: o.onchange,
                    width: '40px'
                });

                if (!Csw.isNullOrEmpty($ValueNTB) && $ValueNTB.length > 0) {
                    $ValueNTB.clickOnEnter(o.$savebtn);
                }
                if (!Csw.isNullOrEmpty($ExponentNTB) && $ExponentNTB.length > 0) {
                    $ExponentNTB.clickOnEnter(o.$savebtn);
                }
            }
        },
        save: function (o) { //$propdiv, $xml
            var attributes = {
                base: o.$propdiv.CswNumberTextBox('value', o.ID + '_val'),
                exponent: o.$propdiv.CswNumberTextBox('value', o.ID + '_exp')
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
