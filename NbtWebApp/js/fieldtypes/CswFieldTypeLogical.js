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
                    var thisButtonType;
                    switch(Checked)
                    {
                        case "true": thisButtonType = CswImageButton_ButtonType.CheckboxTrue; break;
                        case "false": thisButtonType = CswImageButton_ButtonType.CheckboxFalse; break;
                        default: thisButtonType = CswImageButton_ButtonType.CheckboxNull; break;
                    }

                    $Div.CswImageButton({ ButtonType: thisButtonType, 
                                          AlternateText: Checked,
                                          onClick: function($ImageDiv) { return onClick($ImageDiv, Required); }
                                        });
                }
            },
        save: function($propdiv, $xml) {
                var $CheckboxImage = $propdiv.find('div');
                $xml.children('checked').text($CheckboxImage.attr('alt'));
            }
    };
    

    function onClick($ImageDiv, Required) 
    {
        var currentValue = $ImageDiv.attr('alt');
	    var newValue = CswImageButton_ButtonType.CheckboxNull;
	    var newAltText = "null";
        if (currentValue == "null") {
		    newValue = CswImageButton_ButtonType.CheckboxTrue;
            newAltText = "true";
	    } else if (currentValue == "false") {
		    if (Required == "true") {
			    newValue = CswImageButton_ButtonType.CheckboxTrue;
                newAltText = "true";
		    } else {
			    newValue = CswImageButton_ButtonType.CheckboxNull;
                newAltText = "null";
		    }
	    } else if (currentValue == "true") {
		    newValue = CswImageButton_ButtonType.CheckboxFalse;
            newAltText = "false";
	    }
        $ImageDiv.attr('alt', newAltText);
        return newValue;
    } // onClick()



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





