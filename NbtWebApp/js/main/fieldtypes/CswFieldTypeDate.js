/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeDate';

    var methods = {
        init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 

            var $Div = $(this);
            $Div.contents().remove();

            //var value = tryParseString(o.propData.value).trim();
            var value = o.propData.value.date;
            var dateFormat = ServerDateFormatToJQuery(o.propData.value.dateformat);

//            if (value === '1/1/0001')
//                value = '';
            
            if(o.ReadOnly)
            {
                $Div.append(value);
            }
            else 
            {
                var $TextBox = $Div.CswInput('init',{ID: o.ID,
                                                     type: CswInput_Types.text,
                                                     value: value,
                                                     onChange: o.onchange,
                                                     cssclass: 'textinput' // date' date validation broken by alternative formats
                                              }); 
                $TextBox.datepicker({ 'dateFormat': dateFormat });
                if(o.Required)
                {
                    $TextBox.addClass("required");
                }
				$TextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function(o) { //$propdiv, $xml
                var $TextBox = o.$propdiv.find('input');
				o.propData.value.date = $TextBox.val();
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeDate = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
