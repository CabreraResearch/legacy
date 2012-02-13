/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) { 
    "use strict";        
    var pluginName = 'CswFieldTypeText';

    var methods = {
        init: function (o) { 

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? Csw.string(propVals.text).trim() : Csw.enums.multiEditDefaultValue;
            var length = Csw.number( propVals.length, 14 );

            if(o.ReadOnly) {
                $Div.append(value);
            } else {
                var $TextBox = $Div.CswInput('init', {ID: o.ID,
                                                        type: Csw.enums.inputTypes.text,
                                                        value: value,
                                                        cssclass: 'textinput',
                                                        width: length * 7,
                                                        onChange: o.onChange
                                                      });

                if(o.Required) {
                    $TextBox.addClass("required");
                }

                $TextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function (o) {
            var attributes = {
                text: null
            };
            var $text = o.$propdiv.find('input');
            if (false === Csw.isNullOrEmpty($text)) {
                attributes.text = $text.val();
            }
            Csw.preparePropJsonForSave(o.Multi, o.propData, attributes);
        }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeText = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName ); return false;
        }    
  
    };
})(jQuery);
