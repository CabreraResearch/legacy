; (function ($) {
        
    var PluginName = 'CswFieldTypeLogical';

    var methods = {
        init: function(nodepk, $xml) {

                $Div = $(this);
                $Div.children().remove();

                var ID = $xml.attr('id');
                var Required = $xml.attr('required');
                var ReadOnly = $xml.attr('readonly');

                var Checked = $xml.children('checked').text();

                if(ReadOnly)
                {
                    switch(Checked)
                    {
                        case "true": $Div.append('Yes'); break;
                        case "false": $Div.append('No'); break;
                    }
                } 
                else 
                {
                    $CheckboxImage = $('<div id="'+ ID +'" class="divbutton" alt="' + Checked + '" />"' )
                                       .appendTo($Div)
                                       .click(function() { onClick(Required); });

                    updateOffset(Checked);
                }
            },
        save: function($propdiv, $xml) {
                var $CheckboxImage = $propdiv.find('div');
                $xml.children('checked').text($CheckboxImage.attr('alt'));
            }
    };
    

    function onClick(Required) 
    {
        var currentValue = $CheckboxImage.attr('alt');
	    var newValue = "null";

	    if (currentValue == "null") {
		    newValue = "true";
	    } else if (currentValue == "false") {
		    if (Required == "true") {
			    newValue = "true";
		    } else {
			    newValue = "null";
		    }
	    } else if (currentValue == "true") {
		    newValue = "false";
	    }

        $CheckboxImage.attr('alt', newValue);
        updateOffset(newValue);
        	
	    return false;
    } // onClick()

    function updateOffset(val)
    {
        var TrueOffset = 20;
        var FalseOffset = 18;
        var NullOffset = 19;
        var Multiplier = -18;

        var Offset;
        switch(val)
        {
            case "true":  Offset = TrueOffset;  break;
            case "false": Offset = FalseOffset; break;
            case "null":  Offset = NullOffset;  break;
            default:      Offset = NullOffset;  break;
        }

        $CheckboxImage.get(0).style.background = 'url(\'Images/buttons/buttons18.gif\') 0px '+ Offset * Multiplier + 'px no-repeat';
        $CheckboxImage.unbind('mouseover');
        $CheckboxImage.unbind('mouseout');
        $CheckboxImage.bind('mouseover', function() { this.style.backgroundPosition = '-18px ' + Offset * Multiplier + 'px'; })
        $CheckboxImage.bind('mouseout', function() { this.style.backgroundPosition = '0px ' + Offset * Multiplier + 'px'; })
    } // updateOffset()

    // Method calling logic
    $.fn.CswFieldTypeLogical = function (method) {
        
        if ( methods[method] ) {
          return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
          return methods.init.apply( this, arguments );
        } else {
          $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
        }    
  
    };
})(jQuery);
