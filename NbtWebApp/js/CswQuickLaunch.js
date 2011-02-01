; (function ($) {
    $.fn.CswQuickLaunch = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/jQueryGetQuickLaunchItems',
            onLinkClick: function(viewid, actionid) { },
        };

        if (options) {
            $.extend(o, options);
        }
        var $this = $(this);

        CswAjax({
            url: o.Url,
            data: "{ UserId: '' }",
            success: function ($xml) {
                var $QuickLaunchDiv = $('<div id="quicklaunchdiv"><table class="QuickLaunchTable" align="center" cellpadding="20"></table></div>')
                                    .appendTo($this);
                var $table = $QuickLaunchDiv.children('table');
                
                $xml.children().each(function() {

                    var $item = $(this);
                    var $cell = getTableCell($table, '0', '0');

                    switch($item.attr('type'))
                    {
                        case 'View':
                            $cell.append( $('<a href="">' + $item.attr('text') + '</a>') );
                            $cell.find('a').click(function() { o.onLinkClick($item.attr('viewid')); return false; });
                            break;
                        case 'Action': 
                            $cell.append( $('<a href=' + $item.attr('url') + '>' + $item.attr('text') + '</a>') );
                            $cell.find('a').click(function() { o.onSearchClick($item.attr('actionid')); return false; });
                            break;
                    }
                });
                
            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);


