/* Adapted from http://www.noupe.com/tutorial/drop-down-menu-jquery-css.html */

; (function ($) {
    $.fn.CswMenu = function (options) {

        var o = {
        };

        if (options) {
            $.extend(o, options);
        }

        var $MenuUl = $(this);

        $("ul.topnav").children('li').click(TopMenuClick);
        $("ul.subnav").children('li').click(SubMenuClick);

        function TopMenuClick()
        {
            $this = $(this);

            HideAllSubMenus();

            // Show this subnav
            $this.find("ul.subnav")
                    .slideDown('fast')
                    .show()
		            .hover(function () {
          	                },
                            function () {
                                HideAllSubMenus();
                            });
        }

        function SubMenuClick(data, e)
        {
            HideAllSubMenus();
            // Prevent subnav elements from triggering topnav click
            event.stopPropagation();
        }
        
        function HideAllSubMenus()
        {
             $MenuUl.find("ul.subnav").slideUp('fast');
        }


        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
