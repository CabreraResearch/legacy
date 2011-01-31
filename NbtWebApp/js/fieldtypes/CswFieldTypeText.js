; (function ($) {
    $.fn.CswFieldTypeText = function ($propxml) {
        
        var ID = $propxml.attr('id');
        var Value = $propxml.children('text').text();
        var Required = $propxml.attr('required');

        var $Div = $(this);
        
        $Div.children().remove();

        var $TextBox = $('<input type="text" id="'+ ID +'" value="'+ Value +'" />"' )
                         .appendTo($Div); 
        if(Required)
        {
            $TextBox.addClass("required");
        }

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
