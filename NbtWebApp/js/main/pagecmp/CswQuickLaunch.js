/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

(function ($) {
    "use strict";
    $.fn.CswQuickLaunch = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getQuickLaunchItems',
            onViewClick: function () { },
            onActionClick: function () { },
            onSuccess: function () { }
        };

        if (options) {
            $.extend(o, options);
        }
        var $this = $(this);

        var dataXml = {
            UserId: ''
        };

        Csw.ajax.post({
            url: o.Url,
            data: dataXml,
            stringify: false,
            success: function (data) {
                var $QuickLaunchDiv = $('<div id="quicklaunchdiv"><ul id="launchitems"></ul></div>')
                                    .appendTo($this);
                var $list = $QuickLaunchDiv.children();
                for (var item in data) {
                    if (data.hasOwnProperty(item)) {
                        var qlItem = data[item];
                        var launchtype = Csw.string(qlItem.launchtype);
                        var viewmode = Csw.string(qlItem.viewmode);
                        var text = Csw.string(qlItem.text);
                        var viewid = Csw.string(qlItem.itemid); //actions provide their own links. itemid will only be used as viewid.
                        var actionname = Csw.string(qlItem.actionname);
                        var actionurl = Csw.string(qlItem.actionurl);

                        var $li = $('<li></li>')
                            .appendTo($list);

                        switch (launchtype.toLowerCase()) //webservice converts to lower case
                        {
                            case 'view':
                                $('<a href="#' + text + '_' + launchtype + '_' + viewmode + '_' + viewid + '">' + text + '</a>')
                                    .appendTo($li)
                                    //.click(function () { o.onViewClick(viewid, viewmode); return false; });
                                    .click(Csw.makeDelegate(function (x) { o.onViewClick(x.viewid, x.viewmode); return false; }, 
                                                        { viewid: viewid, viewmode: viewmode }));
                                break;
                            case 'action':
                                text = text.replace('_', ' ');
                                $('<a href="#">' + text + '</a>')
                                    .appendTo($li)
                                    //.click(function () { o.onActionClick(actionname, actionurl); return false; });
                                    .click(Csw.makeDelegate(function (x) { o.onActionClick(x.actionname, x.actionurl); return false; }, 
                                                        { actionname: actionname, actionurl: actionurl }));
                                break;
                        }
                    }
                }

                o.onSuccess();

            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function (options) {
})(jQuery);


