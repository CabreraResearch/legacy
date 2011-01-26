; (function ($) {
    $.fn.CswWelcome = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/JQueryGetWelcomeItems',
        };

        if (options) {
            $.extend(o, options);
        }

        var $WelcomeDiv = $(this);

        var ThisSessionId = GetSessionId();

        CswAjax({
            url: o.Url,
            data: "{SessionId: '" + ThisSessionId + "', RoleId: '' }",
            success: function ($xml) {
                


                
                
            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);


