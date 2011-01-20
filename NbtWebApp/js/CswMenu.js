/* Adapted from http://www.noupe.com/tutorial/drop-down-menu-jquery-css.html */

; (function ($) {
    $.fn.CswMenu = function (options) {

        var o = {
        };

        if (options) {
            $.extend(o, options);
        }

        var $MenuUl = $(this);

        $("ul.topnav li")
            .click(function () {

                $this = $(this);

                // Hide all open subnavs
                $this.parent().find("ul.subnav").hide();

                // Show this subnav
                $this.find("ul.subnav")
                        .slideDown('fast')
                        .show()
		                .hover(function () {
          	                   },
                               function () {
                                   $this.find("ul.subnav")
                                        .slideUp('fast');
                               });
            });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
