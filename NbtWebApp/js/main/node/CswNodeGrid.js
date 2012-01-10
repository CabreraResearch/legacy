/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswGrid.js" />
/// <reference path="../pagecmp/CswDialog.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";    
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
        var emptyFunc = function() {
            $.CswDialog('AlertDialog', 'Please select a row to delete');
        };
        return grid.opGridRows(delOpt, rowid, delFunc, emptyFunc);
    }
    
    function editRows (rowid, grid, func, editViewFunc) {
        var editOpt = {
            cswnbtnodekey: [],
            nodepk: [],
            nodename: []
        };
        var editFunc = function(opts) {
            opts.onEditNode = func;
            opts.onEditView = editViewFunc;
            renameProperty(opts, 'cswnbtnodekey', 'nodekeys');
            renameProperty(opts, 'nodepk', 'nodeids');
            renameProperty(opts, 'nodename', 'nodenames');
            $.CswDialog('EditNodeDialog', opts);
        };
        var emptyFunc = function() {
            $.CswDialog('AlertDialog', 'Please select a row to edit');
        };
        return grid.opGridRows(editOpt, rowid, editFunc, emptyFunc);
    }
    
    var methods = {
    
        'init': function (optJqGrid) {

            var o = {
                runGridUrl: '/NbtWebApp/wsNBT.asmx/runGrid',
                gridPageUrl: '/NbtWebApp/wsNBT.asmx/getGridPage',
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
                onSuccess: null,
                onEditView: null // function(viewid){}
            };
        
            if (optJqGrid) {
                $.extend(o, optJqGrid);
            }
            var $parent = $(this);
            var currentPage = (function() {
                var page = 0;
                return function(direction) {
                    var ret = page;
                    switch(direction) {
                        case 'forward':
                            ret += 1;
                            break;
                        case 'backward':
                            if(ret > 0) {
                                ret -= 1;
                            }
                            break;
                    }
                    return ret;
                };
            }());

            function getGridRowsUrl(nodeKey) {
                return o.gridPageUrl + '?viewid=' + o.viewid + '&page=' + currentPage() + '&pagesize=50' + '&IsReport=' + forReporting;
            }

            function prevClick(eventObj, firstRow) {
                var firstNodeKey = firstRow.cswnbtnodekey,
                    prevNodeKey, currentIdx,
                    pageContext = $parent.data('pageContext');
                currentPage('backward');
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
                currentPage('forward');
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
            
            if (o.reinit) $parent.empty();

            var forReporting = (o.EditMode === EditMode.PrintReport.name),
                gridRowsUrl = getGridRowsUrl(o.cswnbtnodekey),
                ret;

            $parent.data('firstNodeKey', o.cswnbtnodekey);
            
            /* fetchGridSkeleton */
            (function () {

                var prepTree = (function() {
                    CswAjaxJson({
                        url: o.runGridUrl,
                        data: {
                            ViewId: o.viewid,
                            IdPrefix: '',
                            IncludeNodeKey: o.cswnbtnodekey,
                            IncludeInQuickLaunch: true
                        },
                        success: function(data) {
                            buildGrid(data);
                        }
                    });
                }());

                var buildGrid = function(gridJson) {
                    CswAjaxJson({
                        url: o.gridPageUrl,
                        data: {
                            ViewId: o.viewid,
                            Page: currentPage(),
                            PageSize: 50,
                            IsReport: forReporting
                        },
                        success: function(rows) {
                            log(rows);
                            var jqGridOpt = gridJson.jqGridOpt;
                            $.extend(jqGridOpt, rows);
                            var g = {
                                ID: o.ID,
                                canEdit: isTrue(jqGridOpt.CanEdit),
                                canDelete: isTrue(jqGridOpt.CanDelete),
                                pagermode: (forReporting) ? 'none' : 'default', 
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
                                g.gridOpts.datatype = 'local';
//                                g.gridOpts.url = getGridRowsUrl();
//                                g.gridOpts.loadComplete = onLoadComplete;
//                                g.gridOpts.jsonReader = {
//                                    root: "rows",
//                                    page: "page",
//                                    total: "total",
//                                    records: "records",
//                                    repeatitems: false,
//                                    id: "id",
//                                    cell: "cell"
//                                };

                                g.customPager = {
                                    prevDisabled: true,
                                    nextDisabled: false,
                                    onPrevPageClick: prevClick,
                                    onNextPageClick: nextClick
                                };

                                g.optNavEdit = {
                                    editfunc: function(rowid) {
                                        return editRows(rowid, ret, o.onEditNode, o.onEditView);
                                    }
                                };

                                g.optNavDelete = {
                                    delfunc: function(rowid) {
                                        return deleteRows(rowid, ret, o.onDeleteNode);
                                    }
                                };

                            } // else
                            ret = CswGrid(g, $parent);
                            
                            if (isFunction(o.onSuccess)) {
                                o.onSuccess(ret);
                            }
                        }, // success{} 
                        error: function (e) {
                                debugger;
                        }
                    }); // ajax
                };
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
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }
    };

})(jQuery);

