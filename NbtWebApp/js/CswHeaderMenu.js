; (function ($) {
    $.fn.CswHeaderMenu = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/JQueryGetHeaderMenu',
            SessionId: ''
        };

        if (options) {
            $.extend(o, options);
        }

        var $MenuDiv = $(this);

        $.ajax({
            type: 'POST',
            url: o.Url,
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            data: "{SessionId: '" + SessionId + "'}",
            success: function (data, textStatus, XMLHttpRequest) {
                var $data = $(data.d);
                var $ul = $('<ul class="topnav"></ul>');

                $data.children().each(function() {
                    var $this = $(this);
                    if($this.attr('text') != undefined)
                    {
                        var $li;
                        if($this.attr('href') != undefined && $this.attr('href') != '' ) {
                            $li = $('<li><a href="' + $this.attr('href') + '">'+ $this.attr('text') +'</a></li>')
                        }
                        else {
                            $li = $('<li>' + $this.attr('text') +'</li>')
                        }
                        $li.appendTo($ul);

                        if($this.children().length > 1) {
                            var $subul = $('<ul class="subnav"></ul>')
                                            .appendTo($li);
                            $this.children().each(function() {
                                var $subthis = $(this);
                                var $li = $('<li><a href="' + $subthis.attr('href') + '">'+ $subthis.attr('text') +'</a></li>')
                                          .appendTo($subul);
                            });
                        }
                    }

                    $MenuDiv.text('')
                            .append($ul);
                });

                $ul.CswMenu();


            }, // success{}
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //_handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
            }
        }); // $.ajax({


        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);

