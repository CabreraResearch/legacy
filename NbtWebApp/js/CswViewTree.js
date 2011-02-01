; (function ($) {
    $.fn.CswViewTree = function (options) {

        var o = {
            TreeUrl: '/NbtWebApp/wsNBT.asmx/jQueryGetTree',
            viewid: '',
            onSelectNode: function(nodeid) { }
        };

        if (options) {
            $.extend(o, options);
        }

        var SelectedNodePk;

        var $treediv = $('<div id="treediv" class="treediv" />')
                        .appendTo($(this));

        CswAjax({
            url: o.TreeUrl,
            data: '{ ViewId: "' + o.viewid + '" }',
            success: function ($xml) {
                var firstid = $xml.find('item').first().find('item').first().attr('id');
                var treexml = $xml.find('tree').get(0).innerHTML;
                var strTypes = $xml.find('types').text();
                var jsonTypes = $.parseJSON(strTypes);

                $treediv.jstree({
                    "xml_data": {
                        "data": treexml,
                        "xsl": "nest"
                    },
                    "ui": {
                        "select_limit": 1,
                        "initially_select": firstid
                    },
                    "core": {
                        "initially_open": [ "root", firstid ]
                    },
                    "types": {
                        "types": jsonTypes
                    },
                    "plugins": ["themes", "xml_data", "ui", "types"]
                }).bind('select_node.jstree', 
                            function (e, data) {
                                SelectedNodePk = data.rslt.obj.attr('id');
                                o.onSelectNode(SelectedNodePk);
                            });

            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);

