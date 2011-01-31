; (function ($) {
    $.fn.CswFieldTypeText = function ($propxml) {
        
        var ID = $propxml.attr('id');
        var Required = $propxml.attr('required');
        var ReadOnly = $propxml.attr('readonly');

        var Value = $propxml.children('text').text();
        var Length = $propxml.children('text').attr('length');

        var $Div = $(this);
        
        $Div.children().remove();
        if(ReadOnly)
        {
            $Div.append(Value);
        }
        else 
        {
            var $TextBox = $('<input type="text" class="textinput" size="' + Length + '" id="'+ ID +'" name="' + ID + '" value="'+ Value +'" />"' )
                             .appendTo($Div); 
            if(Required)
            {
                $TextBox.addClass("required");
            }
        }
        
        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
