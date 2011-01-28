; (function ($) {
    $.fn.CswViewTree = function (options) {

        var o = {
            TreeUrl: '/NbtWebApp/wsNBT.asmx/JQueryGetTree',
            viewid: '',
            onSelectNode: function(nodeid) { }
        };

        if (options) {
            $.extend(o, options);
        }

        var SessionId = GetSessionId();
        var SelectedNodePk;

        var $treediv = $('<div id="treediv" class="treediv" />')
                        .appendTo($(this));

        CswAjax({
            url: o.TreeUrl,
            data: '{ SessionId: "' + SessionId + '", ViewId: "' + o.viewid + '" }',
            success: function ($xml, xml) {
                var firstid = $xml.find('item').first().attr('id');
                $treediv.jstree({
                    "xml_data": {
                        "data": xml,
                        "xsl": "nest"
                    },
                    "ui": {
                        "select_limit": 1,
                        "initially_select": firstid
                    },
                    "core": {
                        "initially_open": firstid
                    },
                    "plugins": ["themes", "xml_data", "ui"]
                })  // .jstree({
                    .bind('select_node.jstree', 
                            function (e, data) {
                                SelectedNodePk = data.args[0].parentNode.id;
                                o.onSelectNode(SelectedNodePk);
                                //getTabs(SelectedNodePk);
                            });

                if(firstid != '')
                    o.onSelectNode(firstid);

            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);

