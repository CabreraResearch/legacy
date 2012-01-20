/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    $.fn.CswViewListTree = function (options) { 

        var o = {
            ViewUrl: '/NbtWebApp/wsNBT.asmx/getViewTree',
            viewid: '',
            issearchable: false,
            usesession: true,
            onSelect: function () { },
            onSuccess: null //function() { }
        };

        if (options) {
            $.extend(o, options);
        }

        var $viewsdiv = $(this);
        
        var jsonData = {
            IsSearchable: o.issearchable,
            UseSession: o.usesession
        };
                        
        CswAjaxJson({
                url: o.ViewUrl,
                data: jsonData,
                stringify: false,
                success: function (data)
                {
                    var jsonTypes = data.types;
                    var treeData = data.tree;					
                    $viewsdiv.jstree({
                        "json_data": {
                            "data": treeData
                        },
                        "ui": {
                            "select_limit": 1
                        },
                        "core": {
                            "initially_open": ["root"]
                        },
                        "types": {
                            "types": jsonTypes
                        },
                        "plugins": ["themes", "json_data", "ui", "types"]
                    }).bind('select_node.jstree', 
                                function () {
                                    var selected = jsTreeGetSelected($viewsdiv);
                                    var $item = selected.$item;
                                    var isLeaf = isTrue($item.CswAttrNonDom('isleaf'));
                                    if (false === isLeaf) {
                                        var $node = $(this);
                                        $viewsdiv.jstree('close_all');
                                        $node.jstree('toggle_node');    
                                    }
                                    else if (isFunction(o.onSelect)) {
                                        var optSelect = {
                                            $item: $item,
                                            iconurl: selected.iconurl,
                                            type: $item.CswAttrNonDom('viewtype'),
                                            viewid: $item.CswAttrNonDom('viewid'),
                                            viewname: selected.text,
                                            viewmode: $item.CswAttrNonDom('viewmode'),
                                            actionid: $item.CswAttrNonDom('actionid'),
                                            actionname: $item.CswAttrNonDom('actionname'),
                                            actionurl: $item.CswAttrNonDom('actionurl'),
                                            reportid: $item.CswAttrNonDom('reportid')
                                        };
                                        o.onSelect(optSelect); //Selected.SelectedId, Selected.SelectedText, Selected.SelectedIconUrl, Selected.SelectedCswNbtNodeKey
                                    }
                                });

                    if (isFunction(o.onSuccess)) {
                        o.onSuccess();  
                    } 

                } // success{}
            });
        
        // For proper chaining support
        return this;

    }; // function(options) {
})(jQuery);
