; (function ($) {
    $.fn.CswViewTree = function (options) {

        var o = {
            TreeUrl: '/NbtWebApp/wsNBT.asmx/getTree',
            viewid: '',
            onSelectNode: function(nodeid) { }
        };

        if (options) {
            $.extend(o, options);
        }

        var SelectedNodePk;

        var $treediv = $('<div id="treediv" class="treediv" />')
                        .appendTo($(this));

        CswAjaxXml({
            url: o.TreeUrl,
            data: 'ViewId=' + o.viewid,
            success: function ($xml) {
                var firstid = $xml.find('item').first().find('item').first().attr('id');
                var strTypes = $xml.find('types').text();
                var jsonTypes = $.parseJSON(strTypes);
                var $treexml = $xml.find('tree').children('root')
                var treexmlstring = xmlToString($treexml);

                $treediv.jstree({
                    "xml_data": {
                        "data": treexmlstring,
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

