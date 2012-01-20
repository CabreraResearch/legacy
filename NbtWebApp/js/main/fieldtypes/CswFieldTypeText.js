/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";        
    var pluginName = 'CswFieldTypeText';

    var methods = {
        init: function(o) { 

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = (false === o.Multi) ? tryParseString(propVals.text).trim() : CswMultiEditDefaultValue;
            var length = tryParseNumber( propVals.length, 14 );

            if(o.ReadOnly) {
                $Div.append(value);
            } else {
                var $TextBox = $Div.CswInput('init', {ID: o.ID,
                                                        type: CswInput_Types.text,
                                                        value: value,
                                                        cssclass: 'textinput',
                                                        width: length * 7,
                                                        onChange: o.onchange
                                                      });

                if(o.Required) {
                    $TextBox.addClass("required");
                }

                $TextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function(o) {
            var attributes = {
                text: null
            };
            var $text = o.$propdiv.find('input');
            if (false === isNullOrEmpty($text)) {
                attributes.text = $text.val();
            }
            preparePropJsonForSave(o.Multi, o.propData, attributes);
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
