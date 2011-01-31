; (function ($) {
    $.fn.CswFieldTypeLogical = function ($propxml) {

        var ID = $propxml.attr('id');
        var Checked = $propxml.children('checked').text();
        var Required = $propxml.attr('required');
        var ReadOnly = $propxml.attr('readonly');

        var $Div = $(this);
        $Div.children().remove();

        var $CheckboxImage = $('<div id="'+ ID +'" class="divbutton" alt="' + Checked + '" />"' )
                               .appendTo($Div)
                               .click(function() { onClick($CheckboxImage, Required); });

        updateOffset($CheckboxImage, Checked);

        
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



        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
