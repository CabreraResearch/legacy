/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswGrid.js" />
/// <reference path="../pagecmp/CswDialog.js" />

; (function ($) { /// <param name="$" type="jQuery" />
    
    var pluginName = 'CswNodeGrid';
    
    function deleteRows(rowid, grid, func) {
        var delOpt = {
            cswnbtnodekey: [],
            nodepk: [],
            nodename: []
        };
        var delFunc = function(opts) {
            opts.onDeleteNode = func;
            renameProperty(opts, 'cswnbtnodekey', 'cswnbtnodekeys');
            renameProperty(opts, 'nodepk', 'nodeids');
            renameProperty(opts, 'nodename', 'nodenames');
            $.CswDialog('DeleteNodeDialog', opts);
        };
        var emptyFunc = function(opts) {
            $.CswDialog('AlertDialog', 'Please select a row to delete');
        };
        return grid.opGridRows(delOpt, rowid, delFunc, emptyFunc);
    }
    
    function editRows (rowid, grid, func) {
        var editOpt = {
            cswnbtnodekey: [],
            nodepk: [],
            nodename: []
        };
        var editFunc = function(opts) {
            opts.onEditNode = func;
            renameProperty(opts, 'cswnbtnodekey', 'nodekeys');
            renameProperty(opts, 'nodepk', 'nodeids');
            renameProperty(opts, 'nodename', 'nodenames');
            $.CswDialog('EditNodeDialog', opts);
        };
        var emptyFunc = function(opts) {
            $.CswDialog('AlertDialog', 'Please select a row to edit');
        };
        return grid.opGridRows(editOpt, rowid, editFunc, emptyFunc);
    }
    
    var methods = {
    
        'init': function (optJqGrid) {

            var o = {
                GridUrl: '/NbtWebApp/wsNBT.asmx/getGrid',
                viewid: '',
                showempty: false,
                ID: '',
                nodeid: '',
                cswnbtnodekey: '',
                reinit: false,
                EditMode: EditMode.Edit.name,
                //onAddNode: function(nodeid,cswnbtnodekey){},
                onEditNode: null, //function(nodeid,cswnbtnodekey){},
                onDeleteNode: null, //function(nodeid,cswnbtnodekey){}
                onSuccess: null
            };
        
            if (optJqGrid) {
                $.extend(o, optJqGrid);
            }
            
            var $parent = $(this);
            if (o.reinit) $parent.empty();

            function getGridRowsUrl(nodeKey) {
                return '/NbtWebApp/wsNBT.asmx/getGridRows?viewid=' + o.viewid + '&SafeNodeKey=' + nodeKey + '&ShowEmpty=' + o.showempty;
            }
            
            var forReporting = (o.EditMode === EditMode.PrintReport.name),
                dataJson = { ViewId: o.viewid, SafeNodeKey: o.cswnbtnodekey, ShowEmpty: o.showempty, IsReport: forReporting },
                gridRowsUrl = getGridRowsUrl(o.cswnbtnodekey),
                ret;
            
            $parent.data('firstNodeKey', o.cswnbtnodekey);
            
            function prevClick(eventObj, firstRow) {
                var firstNodeKey = firstRow.cswnbtnodekey,
                    prevNodeKey, currentIdx,
                    pageContext = $parent.data('pageContext');
                if(false === isNullOrEmpty(pageContext)) {
                    currentIdx = cswIndexOf(pageContext, firstNodeKey);
                    if (currentIdx > 0) {
                        prevNodeKey = pageContext[currentIdx - 1];
                    } 
                    if (false === isNullOrEmpty(prevNodeKey)) {
                        gridRowsUrl = getGridRowsUrl(prevNodeKey);
                        ret.changeGridOpts({ gridOpts: { url: gridRowsUrl } });
                    }
                }
            }
    
            function nextClick(eventObj, firstRow) {
                var firstNodeKey = firstRow.cswnbtnodekey,
                    pageContext = $parent.data('pageContext'),
                    moreNodeKey = $parent.data('moreNodeKey');
                
                if (false === isNullOrEmpty(firstNodeKey) &&
                    false === isNullOrEmpty(moreNodeKey)) {
                    if (isNullOrEmpty(pageContext)) {
                        pageContext = [];
                    }

                    if (false === contains(pageContext, firstNodeKey)) {
                        pageContext.push(firstNodeKey);
                    }

                    $parent.data('pageContext', pageContext);
                    gridRowsUrl = getGridRowsUrl(moreNodeKey);
                    ret.changeGridOpts({ gridOpts: { url: gridRowsUrl } });
                }
            }
            
            function onLoadComplete(data) {
                if (false === isNullOrEmpty(data) && contains(data,'moreNodeKey')) {
                    $parent.data('moreNodeKey', data.moreNodeKey);
                }
            }
            
            var fetchGridSkeleton = (function () {

                CswAjaxJson({
                        url: o.GridUrl,
                        data: dataJson,
                        success: function(gridJson) {

                            var jqGridOpt = gridJson.jqGridOpt;

                            var g = {
                                ID: o.ID,
                                canEdit: isTrue(jqGridOpt.CanEdit),
                                canDelete: isTrue(jqGridOpt.CanDelete),
                                pagermode: (forReporting) ? 'none' : 'default', // 'custom', Use 'custom' to revert to Case 24004
                                gridOpts: { }, //toppager: (jqGridOpt.rowNum >= 50 && contains(gridJson, 'rows') && gridJson.rows.length >= 49)
                                optNav: { },
                                optSearch: { },
                                optNavEdit: { },
                                optNavDelete: { }
                            };
                            $.extend(g.gridOpts, jqGridOpt);

                            if (isNullOrEmpty(g.gridOpts.width)) {
                                g.gridOpts.width = '650px';
                            }

                            if (forReporting) {
                                g.gridOpts.caption = '';

                            } else {
//                                g.gridOpts.datatype = 'json';
//                                g.gridOpts.url = gridRowsUrl;
//                                g.gridOpts.loadComplete = onLoadComplete;
//                                g.gridOpts.jsonReader = {
//                                    root: "rows",
//                                    page: "page",
//                                    total: "total",
//                                    records: "records",
//                                    repeatitems: false,
//                                    id: "id",
//                                    cell: "cell",
//                                    userdata: "userdata",
//                                    subgrid: { }
//                                };

//                                g.customPager = {
//                                    prevDisabled: true,
//                                    nextDisabled: false,
//                                    onPrevPageClick: prevClick,
//                                    onNextPageClick: nextClick
//                                };

                                g.optNavEdit = {
                                    editfunc: function(rowid) {
                                        return editRows(rowid, ret, o.onEditNode);
                                    }
                                };

                                g.optNavDelete = {
                                    delfunc: function(rowid) {
                                        return deleteRows(rowid, ret, o.onDeleteNode);
                                    }
                                };

                            } // else
                            ret = new CswGrid(g, $parent);
                            if (isFunction(o.onSuccess)) {
                                o.onSuccess(ret);
                            }
                        } // success{} 
                    }); // ajax
            })();
            return ret;
        } // 'init'
    }; // methods

    $.fn.CswNodeGrid = function(method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }
    };

})(jQuery);

