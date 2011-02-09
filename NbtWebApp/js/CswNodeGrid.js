; (function ($) {
    $.fn.CswNodeGrid = function (options) {

        var o = {
            GridUrl: '/NbtWebApp/wsNBT.asmx/getGridJson',
            viewid: '',
            onSelectNode: function(nodeid) { }
        };

        if (options) {
            $.extend(o, options);
        }

        var SelectedNodePk;
        var grid;
        
        var options = {
            enableCellNavigation: false,
            enableColumnReorder: false
        };

        console.log("hey");
        var $griddiv = $('<div id="griddiv" class="griddiv" />')
                        .appendTo($(this));

        CswAjaxJSON({
            url: o.GridUrl,
            data: "{ViewId: '" +  o.viewid + "'}",
            success: function (data) {
                
                console.log(data);
                
                var columns = data.columns;
                var griddata = data.grid;
                
                grid = new Slick.Grid($griddiv,griddata,columns,options);

//                $treediv.jstree({
//                    "xml_data": {
//                        "data": treexmlstring,
//                        "xsl": "nest"
//                    },
//                    "ui": {
//                        "select_limit": 1,
//                        "initially_select": firstid
//                    },
//                    "core": {
//                        "initially_open": [ "root", firstid ]
//                    },
//                    "types": {
//                        "types": jsonTypes
//                    },
//                    "plugins": ["themes", "xml_data", "ui", "types"]
//                }).bind('select_node.jstree', 
//                            function (e, data) {
//                                SelectedNodePk = data.rslt.obj.attr('id');
//                                o.onSelectNode(SelectedNodePk);
//                            });

            } // success{}
        });

        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);

