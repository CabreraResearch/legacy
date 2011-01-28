; (function ($) {
    $.fn.CswViewSelect = function (options) {

        var o = {
            ViewUrl: '/NbtWebApp/wsNBT.asmx/JQueryGetViews',
            viewid: '',
            onSelect: function(viewid) { }
        };

        if (options) {
            $.extend(o, options);
        }

        var SessionId = GetSessionId();

        var $viewsdiv = $('<div id="viewsdiv" />')
                        .appendTo($(this));

        getViewSelect(o.viewid);

        function getViewSelect(selectedviewid)
        {
            starttime = new Date();
            CswAjax({
                url: o.ViewUrl,
                data: '{ SessionId: "'+ SessionId +'" }',
                success: function ($xml)
                {
                    $viewsdiv.children().remove();
                    $select = $('<select name="viewselect" id="viewselect"><option value="">Select A View</option></select>')
                              .appendTo($viewsdiv);
                    $xml.children().each(function() {
                        var $this = $(this);
                        var thisid = $this.attr('id');
                        var option = '<option value="' + thisid + '"';
                        if(thisid == selectedviewid)
                        {
                            option += ' selected';
                            getTree(thisid);
                        }
                        option += '>' + $this.attr('name') + '</option>';
                        $select.append(option);
                    });
                    $select.bind('change', function(e, data) { 
                        //getTree(e.target.value);
                        o.onSelect(e.target.value);
                    });
                } // success{}
            });
        } // getViewSelect()

        
        // For proper chaining support
        return this;

    }; // function(options) {
}) (jQuery);

