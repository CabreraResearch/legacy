; (function ($) {
    $.fn.CswNodeTree = function (options) {

        var o = {
            ID: '', 
            TreeUrl: '/NbtWebApp/wsNBT.asmx/getTree',
            viewid: '',
            nodeid: '',
            onSelectNode: function(nodeid, nodename, iconurl) { },
            SelectFirstChild: true
        };

        if (options) {
            $.extend(o, options);
        }

        var $treediv = $('<div id="treediv" class="treediv" />')
                        .appendTo($(this));
        var IDPrefix = o.ID + '_';

        CswAjaxXml({
            url: o.TreeUrl,
            data: 'ViewId=' + o.viewid + '&IDPrefix=' + IDPrefix,
            success: function ($xml) {
                var selectid;
                if(o.nodeid != undefined && o.nodeid != '') 
                    selectid = IDPrefix + o.nodeid;
                else
                {
                    if(o.SelectFirstChild)
                        selectid = $xml.find('item').first().find('item').first().attr('id');
                    else
                        selectid = IDPrefix + 'root';
                }

                // make sure selected item is visible
                var $selecteditem = $xml.find('item[id="'+ selectid + '"]');
                var $itemparents = $selecteditem.parents('item').andSelf();
                var initiallyOpen = new Array();
                var i = 0;
                $itemparents.each(function() { initiallyOpen[i] = $(this).attr('id'); i++; });

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
                        "initially_select": selectid
                    },
                    "core": {
                        "initially_open": initiallyOpen
                    },
                    "types": {
                        "types": jsonTypes
                    },
                    "plugins": ["themes", "xml_data", "ui", "types"]
                }).bind('select_node.jstree', 
                                function (e, data) {
                                    var Selected = jsTreeGetSelected($treediv, IDPrefix); 
                                    o.onSelectNode(Selected.SelectedId, Selected.SelectedText, Selected.SelectedIconUrl);
                                });

            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);

