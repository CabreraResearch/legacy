/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function ($) {
    "use strict";
    $.fn.CswMenuHeader = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getHeaderMenu',
            onLogout: function () { },
            onQuotas: function () { },
            onSessions: function () { },
            onImpersonate: null,
            onEndImpersonation: null,
            onSuccess: function () { }
        };

        if (options) {
            $.extend(o, options);
        }

        var $MenuDiv = $(this);

        Csw.ajax.post({
            url: o.Url,
            data: {},
            success: function (data) {
                var $ul = $('<ul class="topnav"></ul>');
                $MenuDiv.text('')
                        .append($ul);
                
                for (var menuItem in data) {
                    if (data.hasOwnProperty(menuItem)) {
                        var thisItem = data[menuItem];
                        if (!Csw.isNullOrEmpty(menuItem))
                        {
                            var $li = Csw.handleMenuItem({ $ul: $ul, 
                                                        itemKey: menuItem,
                                                        itemJson: thisItem, 
                                                        onLogout: o.onLogout,
                                                        onQuotas: o.onQuotas,
                                                        onSessions: o.onSessions,
                                                        onImpersonate: o.onImpersonate,
                                                        onEndImpersonation: o.onEndImpersonation });

                            if (Csw.bool(thisItem.haschildren)) {
                                delete thisItem.haschildren;
                                var $subul = $('<ul class="subnav"></ul>')
                                    .appendTo($li);
                                for (var childItem in thisItem) {
                                    if (thisItem.hasOwnProperty(childItem)) {
                                        var thisChild = thisItem[childItem];
                                        Csw.handleMenuItem({ $ul: $subul, 
                                                         itemKey: childItem,
                                                         itemJson: thisChild, 
                                                         onLogout: o.onLogout,
                                                         onQuotas: o.onQuotas,
                                                         onSessions: o.onSessions,
                                                         onImpersonate: o.onImpersonate,
                                                         onEndImpersonation: o.onEndImpersonation });
                                    }
                                }
                            }
                        }
                    }
                }
                $ul.CswMenu();

                o.onSuccess();

            } // success{}
        }); // $.ajax({

        // For proper chaining support
        return this;

    }; // function (options) {
})(jQuery);

