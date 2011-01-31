; (function ($) {
    $.fn.CswHeaderMenu = function (options) {

        var o = {
            Url: '/NbtWebApp/wsNBT.asmx/JQueryGetHeaderMenu',
        };

        if (options) {
            $.extend(o, options);
        }

        var $MenuDiv = $(this);

        CswAjax({
            url: o.Url,
            data: "",
            success: function ($xml) {
                var $ul = $('<ul class="topnav"></ul>');

                $MenuDiv.text('')
                        .append($ul);

                $xml.children().each(function() {
                    var $this = $(this);
                    if($this.attr('text') != undefined)
                    {
                        var $li = HandleMenuItem($ul, $this);
                        
                        if($this.children().length > 1) {
                            var $subul = $('<ul class="subnav"></ul>')
                                            .appendTo($li);
                            $this.children().each(function() {
                                HandleMenuItem($subul, $(this));
                            });
                        }
                    }

                });

                $ul.CswMenu();

            } // success{}
        }); // $.ajax({


        function HandleMenuItem($ul, $this)
        {
            var $li;
            if ($this.attr('href') != undefined && $this.attr('href') != '') {
                $li = $('<li><a href="' + $this.attr('href') + '">' + $this.attr('text') + '</a></li>')
                        .appendTo($ul)
            }
            else if($this.attr('popup') != undefined && $this.attr('popup') != '' ) {
                $li = $('<li class="headermenu_dialog">'+ $this.attr('text') +'</li>')
                        .appendTo($ul)
                        .click(function() { OpenDialog($this.attr('text'), $this.attr('popup')); });
            }
            else {
                $li = $('<li>' + $this.attr('text') +'</li>')
                        .appendTo($ul)
            }
            return $li;
        }


        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);

