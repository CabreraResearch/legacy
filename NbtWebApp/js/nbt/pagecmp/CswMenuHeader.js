/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function ($) {
    "use strict";
    $.fn.CswMenuHeader = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getHeaderMenu',
            onLogout: function () { },
            onQuotas: function () { },
            onModules: function () { },
            onSessions: function () { },
            onSubscriptions: function () { },
            onImpersonate: null,
            onEndImpersonation: null,
            onSuccess: function () { },
            onSubmitRequest: function() { }
        };

        if (options) {
            Csw.extend(o, options);
        }

        var $MenuDiv = $(this);

        var menuOpts = { 
            width: '100%',
            ajax: { 
                urlMethod: 'getHeaderMenu', 
                data: {} 
            },
            onLogout: o.onLogout,
            onQuotas: o.onQuotas,
            onModules: o.onModules,
            onSessions: o.onSessions,
            onSubscriptions: o.onSubscriptions,
            onImpersonate: o.onImpersonate,
            onEndImpersonation: o.onEndImpersonation,
            onSubmitRequest: o.onSubmitRequest 
        };
        Csw.composites.menu( Csw.literals.factory($MenuDiv), menuOpts ); // menu()

//        Csw.ajax.post({
//            url: o.Url,
//            data: {},
//            success: function (data) {
//                var $ul = $('<ul class="topnav"></ul>');
//                $MenuDiv.text('')
//                        .append($ul);
//                
//                for (var menuItem in data) {
//                    if (data.hasOwnProperty(menuItem)) {
//                        var thisItem = data[menuItem];
//                        if (!Csw.isNullOrEmpty(menuItem))
//                        {
//                            var $li = Csw.handleMenuItem({ $ul: $ul, 
//                                                        itemKey: menuItem,
//                                                        itemJson: thisItem, 
//                                                        onLogout: o.onLogout,
//                                                        onQuotas: o.onQuotas,
//                                                        onModules: o.onModules,
//                                                        onSessions: o.onSessions,
//                                                        onImpersonate: o.onImpersonate,
//                                                        onEndImpersonation: o.onEndImpersonation,
//                                                        onSubmitRequest: o.onSubmitRequest });

//                            if (Csw.bool(thisItem.haschildren)) {
//                                delete thisItem.haschildren;
//                                var $subul = $('<ul class="subnav"></ul>')
//                                    .appendTo($li);
//                                for (var childItem in thisItem) {
//                                    if (thisItem.hasOwnProperty(childItem)) {
//                                        var thisChild = thisItem[childItem];
//                                        Csw.handleMenuItem({ $ul: $subul, 
//                                                         itemKey: childItem,
//                                                         itemJson: thisChild, 
//                                                         onLogout: o.onLogout,
//                                                         onQuotas: o.onQuotas,
//                                                         onModules: o.onModules,
//                                                         onSessions: o.onSessions,
//                                                         onImpersonate: o.onImpersonate,
//                                                         onEndImpersonation: o.onEndImpersonation,
//                                                         onSubmitRequest: o.onSubmitRequest
//                                                     });
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//                $ul.CswMenu();

//                o.onSuccess();

//            } // success{}
//        }); // $.ajax({

        // For proper chaining support
        return this;

    }; // function (options) {
})(jQuery);

