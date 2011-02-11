; (function ($) {
        
    var PluginName = 'CswFieldTypeNumber';

    var methods = {
        init: function(nodepk, $xml) {

                var $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = $xml.attr('required');
                var ReadOnly = $xml.attr('readonly');

                var Value = $xml.children('value').text();
                var MinValue = $xml.children('value').attr('minvalue');
                var MaxValue = $xml.children('value').attr('maxvalue');
                var Precision = $xml.children('value').attr('precision');

                if(ReadOnly)
                {
                    $Div.append(Value);
                }
                else 
                {
                    var $TextBox = $('<input type="text" class="textinput" id="'+ ID +'" name="' + ID + '" value="'+ Value +'" />"' )
                                     .appendTo($Div);
                    $TextBox.change(function() { validate($Div, $TextBox, MinValue, MaxValue, Precision) });

                    if(Required)
                    {
                        $TextBox.addClass("required");
                    }
                }
            },
        save: function($propdiv, $xml) {
                var $TextBox = $propdiv.find('input');
                $xml.children('value').text($TextBox.val());
            }
    };
    

    function validate($Div, $textbox, minvalue, maxvalue, precision)
    {
        var strValue = $textbox.val();
        var nValue = parseFloat(strValue);

	    var regex;
	    var msg;
	    var invalidMsg;
	    var isValid = true;
	
	    // PRECISION
	    if (precision > 0)
	    {
		    // Allow any valid number -- we'll round later
		    regex = /^\-?\d*\.?\d*$/g;
		    msg = 'Value must be numeric'; 
	    }
	    else
	    {
		    // Integers Only
		    regex = /^\-?\d*$/g;
		    msg = 'Value must be an integer'; 
	    }
	    if(isValid && !regex.test(strValue))
	    {
		    isValid = false; 
		    invalidMsg = msg;
	    }
	
	    // MINVALUE
	    if(isValid && minvalue != "" && nValue < minvalue)
	    {
		    isValid = false;
		    invalidMsg = 'Value must be greater than or equal to ' + minvalue;
	    }
	    // MAXVALUE
	    if(isValid && maxvalue != "" && nValue > maxvalue)
	    {
		    isValid = false;
		    invalidMsg = 'Value must be less than or equal to ' + maxvalue;
	    }

        $textbox.CswValid(isValid);

    } // validate()


    // Method calling logic
    $.fn.CswFieldTypeNumber = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
