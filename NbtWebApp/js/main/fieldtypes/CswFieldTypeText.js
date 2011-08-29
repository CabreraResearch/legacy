/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    var pluginName = 'CswFieldTypeText';

    var methods = {
        init: function(o) { 

            var $Div = $(this);
            $Div.contents().remove();
            var propVals = o.propData.values;
            var value = tryParseString(propVals.text).trim();
            var length = tryParseNumber( propVals.length, 14 );

            if(o.ReadOnly)
            {
                $Div.append(value);
            }
            else 
            {
                var $TextBox = $Div.CswInput('init', {ID: o.ID,
                                                        type: CswInput_Types.text,
                                                        value: value,
                                                        cssclass: 'textinput',
                                                        width: length * 7,
                                                        onChange: o.onchange
                                                      });

                if(o.Required)
                {
                    $TextBox.addClass("required");
                }

				$TextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function(o) {
                var $TextBox = o.$propdiv.find('input');
                o.propData.values.text = $TextBox.val();
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeText = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
