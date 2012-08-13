///// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
///// <reference path="../../globals/Global.js" />

///* Adapted from http://www.noupe.com/tutorial/drop-down-menu-jquery-css.html */

//(function ($) { 
//    "use strict";
//    $.fn.CswMenu = function (options) {

//        var o = {
//        };

//        if (options) {
//            Csw.extend(o, options);
//        }

//        var $MenuUl = $(this);

//        $MenuUl.children('li')
//                      .click(TopMenuClick)
//                      .hover(TopMenuClick, HideAllSubMenus);
//        $MenuUl.find(".subnav").children('li').click(SubMenuClick);

//        function TopMenuClick()
//        {
//            var $this = $(this);

//            HideAllSubMenus();

//            // Show this subnav
//            $this.find("ul.subnav")
//                    .slideDown('fast')
//                    .show();
//        }

//        function SubMenuClick(event)
//        {
//            HideAllSubMenus();
//            // Prevent subnav elements from triggering topnav click
//            if(event)
//            {
//                event.stopPropagation();
//            }
//        }
//        
//        function HideAllSubMenus()
//        {
//             $MenuUl.find('ul').stop(true, true);
//             $MenuUl.find("ul.subnav").slideUp('fast');
//        }


//        // For proper chaining support
//        return this;

//    }; // function (options) {
//})(jQuery);
