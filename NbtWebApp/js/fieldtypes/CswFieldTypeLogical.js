﻿; (function ($) {
        
    var PluginName = 'CswFieldTypeLogical';
    var $propxml;

    var methods = {
        init: function(nodepk, $xml) {
                $propxml = $xml;

                var $Div = $(this);
                $Div.children().remove();

                var ID = $propxml.attr('id');
                var Required = $propxml.attr('required');
                var ReadOnly = $propxml.attr('readonly');

                var Checked = $propxml.children('checked').text();

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
                    var $CheckboxImage = $('<div id="'+ ID +'" class="divbutton" alt="' + Checked + '" />"' )
                                           .appendTo($Div)
                                           .click(function() { onClick($CheckboxImage, Required); });

                    updateOffset($CheckboxImage, Checked);
                }
            },
        save: function() {
                var $Div = $(this);
                var o = $Div.data(PluginName);
                //var $newPropXml = $propxml;
                //$newPropXml.children('text').text($TextBox.val());
                //SaveProp(nodepk, $newPropXml, function () { });
            }
    };
    

    function onClick($CheckboxImage, Required) 
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
        updateOffset($CheckboxImage, newValue);
        	
	    return false;
    } // onClick()

    function updateOffset($CheckboxImage, val)
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
