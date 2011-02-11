
; (function ($) {
    $.fn.CswValid = function (isValid, msg) {

        var $Parent = $(this);
        if(!isValid)
        {
            $Parent.animate({ backgroundColor: '#ff6666'});
        } else {
            $Parent.css('background-color', '#66ff66');
            setTimeout(function() { $Parent.animate({ backgroundColor: 'transparent'}); }, 500);
        }
        
    };
})(jQuery);
