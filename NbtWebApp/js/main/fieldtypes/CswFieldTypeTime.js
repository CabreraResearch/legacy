/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
        
    var pluginName = 'CswFieldTypeTime';

    var methods = {
        init: function(o) {

            var $Div = $(this);
            $Div.contents().remove();

            var value = tryParseString(o.propData.value.time).trim();
            var timeFormat = ServerTimeFormatToJQuery(o.propData.value.timeformat);

            if (o.ReadOnly) {
                $Div.append(value);
            } else {
                var $TextBox = $Div.CswInput('init',{ID: o.ID,
                                                      type: CswInput_Types.text,
                                                      cssclass: 'textinput', // validateTime',
                                                      onChange: o.onchange,
                                                      value: value
                                                 }); 
                var $nowbutton = $Div.CswButton('init',{ 'ID': o.ID +'_now',
														'disableOnClick': false,
                                                        'onclick': function() { $TextBox.val( getTimeString(new Date(), timeFormat) ); },
                                                        'enabledText': 'Now'
                                                 }); 
                
//				jQuery.validator.addMethod( "validateTime", function(value, element) { 
//                            return (this.optional(element) || validateTime($(element).val()));
//                        }, 'Enter a valid time (e.g. 12:30 PM)');

				if(o.Required)
                {
                    $TextBox.addClass("required");
                }
				$TextBox.clickOnEnter(o.$savebtn);
            }
        },
        save: function(o) {
                var $TextBox = o.$propdiv.find('input');
                o.propData.value.time = $TextBox.val();
            }
    };
    
    // Method calling logic
    $.fn.CswFieldTypeTime = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
