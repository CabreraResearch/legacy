; (function ($) {
    $.fn.CswFieldTypeLogical = function ($propxml) {

        var ID = $propxml.attr('id');
        var Checked = $propxml.children('checked').text();
        var Required = $propxml.attr('required');
        var ReadOnly = $propxml.attr('readonly');

        var TrueOffset = 20;
        var FalseOffset = 18;
        var NullOffset = 19;
        var Multiplier = -18;

        var $Div = $(this);
        $Div.children().remove();

        var $CheckboxImage = $('<div id="'+ ID +'" class="divbutton" alt="' + Checked + '" />"' )
                               .appendTo($Div)
                               .click(function() { onClick($CheckboxImage, Required); });

        var Offset = 0;
        updateOffset(Checked);

        if(Required)
        {
            
        }

        


        function onClick($CheckboxImage, Required) 
        {
        	var currentValue = $CheckboxImage.attr('value');
	        var newValue = "Null";

	        if (currentValue == "Null") {
		        newValue = "True";
	        } else if (currentValue == "False") {
		        if (Required) {
			        newValue = "True";
		        } else {
			        newValue = "Null";
		        }
	        } else if (currentValue == "True") {
		        newValue = "False";
	        }
            updateOffset(newValue);
        	$CheckboxImage.attr('alt', newValue);
	    
            var checkbutton = document.getElementById(checkbuttondivid);
	        checkbutton.style.backgroundPosition = "0px " + Offset * Multipler + "px";
	        checkbutton.onmouseover = function() { this.style.backgroundPosition = "-18px " + Offset * Multipler * -18 + "px"; }
	        checkbutton.onmouseout = function() { this.style.backgroundPosition = "0px " + Offset * Multipler * -18 + "px"; }
	
	        if (autopostback == "true")
		        document.getElementById(pbbuttonid).click();
	        return false;
        }

        function updateOffset($CheckboxImage, val)
        {
            var Offset;
            switch(val)
            {
                case "True":  Offset = TrueOffset;  break;
                case "False": Offset = FalseOffset; break;
                case "Null":  Offset = NullOffset;  break;
                default:      Offset = NullOffset;  break;
            }
            $CheckboxImage.style.background = "url(\'Images/buttons/buttons18.gif\') 0px '+ Offset * Multipler +'px no-repeat";
            $CheckboxImage.bind('mouseover', function() { this.style.backgroundPosition = '-18px ' + Offset * Multipler + 'px'; })
            $CheckboxImage.bind('mouseout', function() { this.style.backgroundPosition = '0px ' + Offset * Multipler + 'px'; })
        }



        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
