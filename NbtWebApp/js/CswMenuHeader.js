; (function ($) {
    $.fn.CswMenuHeader = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/getHeaderMenu',
            onLogout: function() { },
			onSuccess: function() { }
        };

        if (options) {
            $.extend(o, options);
        }

        var $MenuDiv = $(this);

        CswAjaxXml({
            url: o.Url,
            data: "",
            success: function ($xml) {
                var $ul = $('<ul class="topnav"></ul>');

                $MenuDiv.text('')
                        .append($ul);

                $xml.children().each(function() {
                    var $this = $(this);
                    if($this.CswAttrXml('text') !== undefined)
                    {
						var $li = HandleMenuItem({ '$ul': $ul, '$itemxml': $this, 'onLogout': o.onLogout});
                        
                        if($this.children().length > 1) {
                            var $subul = $('<ul class="subnav"></ul>')
                                            .appendTo($li);
                            $this.children().each(function() {
								HandleMenuItem({ '$ul': $subul, '$itemxml': $(this), 'onLogout': o.onLogout});
                            });
                        }
                    }

                });

                $ul.CswMenu();

				o.onSuccess();

            } // success{}
        }); // $.ajax({

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);

