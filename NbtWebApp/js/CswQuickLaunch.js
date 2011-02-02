; (function ($) {
    $.fn.CswQuickLaunch = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getQuickLaunchItems',
            onLinkClick: function(viewid, actionid) { },
        };

        if (options) {
            $.extend(o, options);
        }
        var $this = $(this);

        CswAjaxJSON({
            url: o.Url,
            data: "{ UserId: '' }",
            success: function ($xml) {
                var $QuickLaunchDiv = $('<div id="quicklaunchdiv"><ul id="breadcrumbs"></ul></div>')
                                    .appendTo($this);
                var $list = $QuickLaunchDiv.children();
                
                $xml.children().each(function() {

                    var $item = $(this);
                    //var $cell = getTableCell($table, $item.attr('displayrow'), $item.attr('displaycol'));
                    switch($item.attr('type'))
                    {
                        case 'View':
                            $('<li><a href="#">' + $item.attr('text') + '</a></li>')
                                 .appendTo($list) 
                                 .children('a')
                                 .click(function() { o.onLinkClick($item.attr('viewid')); return false; });
                            break;
                        case 'Action': 
                            $('<li><a href=' + $item.attr('url') + '>' + $item.attr('text') + '</a></li>') 
                                .appendTo($list);
                            break;
                    }
                });
                
            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);


