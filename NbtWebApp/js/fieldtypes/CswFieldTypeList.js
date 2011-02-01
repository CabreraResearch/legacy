; (function ($) {
    $.fn.CswFieldTypeList = function (nodepk, $propxml) {
        
        var ID = $propxml.attr('id');
        var Required = $propxml.attr('required');
        var ReadOnly = $propxml.attr('readonly');

        var Value = $propxml.children('value').text();
        var Options = $propxml.children('options').text();

        var $Div = $(this);
        
        $Div.children().remove();
        if(ReadOnly)
        {
            $Div.append(Value);
        }
        else 
        {
            var $SelectBox = $('<select id="'+ ID +'" name="'+ ID +'" />"' )
                             .appendTo($Div);
            
            var SplitOptions = Options.split(',')
            for(var i = 0; i < SplitOptions.length; i++)
            {
                $SelectBox.append('<option value="'+SplitOptions[i]+'">'+SplitOptions[i]+'</option>');
            }
            $SelectBox.val( Value );

            if(Required)
            {
                $SelectBox.addClass("required");
            }
        }
        
        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
