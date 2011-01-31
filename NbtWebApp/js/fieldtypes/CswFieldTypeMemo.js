﻿; (function ($) {
    $.fn.CswFieldTypeMemo = function ($propxml) {
        
        var ID = $propxml.attr('id');
        var rows = $propxml.children('text').attr('rows');
        var columns = $propxml.children('text').attr('columns');
        var Value = $propxml.children('text').text();
        var Required = $propxml.attr('required');
        var ReadOnly = $propxml.attr('readonly');

        var $Div = $(this);
        
        $Div.children().remove();
        if(ReadOnly)
        {
            $Div.append(Value);
        }
        else 
        {
            var $TextArea = $('<textarea id="'+ ID +'" rows="'+rows+'" cols="'+columns+'">'+ Value +'</textarea>' )
                             .appendTo($Div); 
            if(Required)
            {
                $TextArea.addClass("required");
            }
        }

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
