; (function ($) {
    $.fn.CswWelcome = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getWelcomeItems',
            onLinkClick: function(viewid, actionid, reportid) { },
            onSearchClick: function(viewid) { },
            onAddClick: function(nodetypeid) { }
        };

        if (options) {
            $.extend(o, options);
        }
        var $this = $(this);

        CswAjaxXml({
            url: o.Url,
            data: "RoleId=",
            success: function ($xml) {
                var $WelcomeDiv = $('<div id="welcomediv"><table class="WelcomeTable" align="center" cellpadding="20"></table></div>')
                                    .appendTo($this);
                var $table = $WelcomeDiv.children('table');
                
                $xml.children().each(function() {

                    //<item id=" + WelcomeRow["welcomeid"].ToString() + "\"";
                    //      type=\"" + WelcomeRow["componenttype"].ToString() + "\"";
                    //      buttonicon=\"" + IconImageRoot + "/" + WelcomeRow["buttonicon"].ToString() + "\"";
                    //      text=\"" + LinkText + "\"";
                    //      displayrow=\"" + WelcomeRow["display_row"].ToString() + "\"";
                    //      displaycol=\"" + WelcomeRow["display_col"].ToString() + "\"";

                    var $item = $(this);
                    var $cell = getTableCell($table, $item.attr('displayrow'), $item.attr('displaycol'));

                    if($item.attr('buttonicon') != undefined && $item.attr('buttonicon') != '')
                        $cell.append( $('<a href=""><img border="0" src="'+ $item.attr('buttonicon') +'"/></a><br/><br/>') );

                    switch($item.attr('type'))
                    {
                        case 'Link':
                            $cell.append( $('<a href="">' + $item.attr('text') + '</a>') );
                            $cell.find('a').click(function() { o.onLinkClick($item.attr('viewid'),$item.attr('actionid'),$item.attr('reportid')); return false; });
                            break;
                        case 'Search': 
                            $cell.append( $('<a href="">' + $item.attr('text') + '</a>') );
                            $cell.find('a').click(function() { o.onSearchClick($item.attr('viewid')); return false; });
                            break;
                        case 'Text':
                            $cell.text($item.attr('text'));
                            break;
                        case 'Add': 
                            $cell.append( $('<a href="">' + $item.attr('text') + '</a>') );
                            $cell.find('a').click(function() { o.onAddClick($item.attr('nodetypeid')); return false; });
                            break;
                    }
                });
                
            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);


