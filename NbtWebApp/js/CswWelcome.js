; (function ($) {
    $.fn.CswWelcome = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/JQueryGetWelcomeItems',
            SessionId: ''
        };

        if (options) {
            $.extend(o, options);
        }
        var $this = $(this);

        CswAjax({
            url: o.Url,
            data: "{SessionId: '" + o.SessionId + "', RoleId: '' }",
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
                        $cell.append( $('<a href=""><img src="'+ $item.attr('buttonicon') +'"/></a><br/><br/>') );

                    switch($item.attr('type'))
                    {
                        case 'Link':
                            $cell.append( $('<a href="">' + $item.attr('text') + '</a>') );
                            break;
                        case 'Search': 
                            $cell.append( $('<a href="">' + $item.attr('text') + '</a>') );
                            break;
                        case 'Text':
                            $cell.text($item.attr('text'));
                            break;
                        case 'Add': 
                            $cell.append( $('<a href="">' + $item.attr('text') + '</a>') );
                            break;
                    }
                });
                
            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);


