/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function ($) {
    "use strict";
    var pluginName = 'CswNodeGrid';

    var cswPrivate = {
        selectedRowId: ''
    };

//    function deleteRows(rowid, grid, func) {
//        if (Csw.isNullOrEmpty(rowid)) {
//            rowid = cswPrivate.selectedRowId;
//        }
//        if (Csw.isNullOrEmpty(rowid)) {
//            rowid = grid.getSelectedRowId();
//        }
//        if (Csw.number(rowid) !== -1) { /* Case 24678 */
//            var delOpt = {
//                cswnbtnodekey: [],
//                nodename: []
//            };
//            var delFunc = function (opts) {
//                opts.onDeleteNode = func;
//                opts.publishDeleteEvent = false;
//                Csw.renameProperty(opts, 'cswnbtnodekey', 'cswnbtnodekeys');
//                Csw.renameProperty(opts, 'nodename', 'nodenames');
//                $.CswDialog('DeleteNodeDialog', opts);
//            };
//            var emptyFunc = function () {
//                $.CswDialog('AlertDialog', 'Please select a row to delete');
//            };
//            return grid.opGridRows(delOpt, rowid, delFunc, emptyFunc);
//        }
//    }

//    function editRows(rowid, grid, func, editViewFunc, onRefreshFunc) {
//        if (Csw.isNullOrEmpty(rowid)) {
//            rowid = cswPrivate.selectedRowId;
//        }
//        if (Csw.isNullOrEmpty(rowid)) {
//            rowid = grid.getSelectedRowId();
//        }
//        if (Csw.number(rowid) !== -1) { /* Case 24678 */
//            var editOpt = {
//                cswnbtnodekey: [],
//                nodename: []
//            };
//            var editFunc = function (opts) {
//                opts.onEditNode = func;
//                opts.onEditView = editViewFunc;
//                opts.onRefresh = onRefreshFunc;
//                Csw.renameProperty(opts, 'cswnbtnodekey', 'nodekeys');
//                Csw.renameProperty(opts, 'nodename', 'nodenames');
//                $.CswDialog('EditNodeDialog', opts);
//            };
//            var emptyFunc = function () {
//                $.CswDialog('AlertDialog', 'Please select a row to edit');
//            };
//            return grid.opGridRows(editOpt, rowid, editFunc, emptyFunc);
//        }
//    }

    var methods = {

        'init': function (options) {

            var o = {
                runGridUrl: '/NbtWebApp/wsNBT.asmx/runGrid',
                //gridPageUrl: '/NbtWebApp/wsNBT.asmx/getGridRowsByPage',
                //gridAllRowsUrl: '/NbtWebApp/wsNBT.asmx/getAllGridRows',
                viewid: '',
                showempty: false,
                ID: '',
                nodeid: '',
                cswnbtnodekey: '',
                reinit: false,
                EditMode: Csw.enums.editMode.Edit,
                onEditNode: null,
                onDeleteNode: null,
                onSuccess: null,
                onEditView: null,
                onRefresh: null,
                gridOpts: {
                    multiselect: false
                },
                resizeWithParent: true
            };

            if (options) {
                $.extend(o, options);
            }
            var $parent = $(this);

//            function getGridRowsUrl(isPrint) {
//                var url = o.gridPageUrl + '?ViewId=' + o.viewid + '&IsReport=' + Csw.bool(forReporting || isPrint).toString() + '&IncludeNodeKey=' + o.cswnbtnodekey;
//                if (isPrint) {
//                    url += '&Page=1&Rows=100000000';
//                }
//                return url;
//            }

            if (o.reinit) $parent.empty();

            var forReporting = (o.EditMode === Csw.enums.editMode.PrintReport),
                isMulti = o.gridOpts.multiselect,
                ret, doPaging = false;

            /* fetchGridSkeleton */
            (function () {

                //Get the grid skeleton definition
//                (function () {
//                    Csw.ajax.post({
//                        url: o.runGridUrl,
//                        data: {
//                            ViewId: o.viewid,
//                            IncludeNodeKey: o.cswnbtnodekey,
//                            IncludeInQuickLaunch: true,
//                            ForReport: forReporting
//                        },
//                        success: function (data) {
//                            buildGrid(data);
//                        }
//                    });
//                } ());

//              //jqGrid will handle the rest
//              var buildGrid = function (gridJson) {

//                    var makeGrid = function (pagerMode, data) {
//                        var jqGridOpt = gridJson.jqGridOpt;
//                        var wasTruncated = Csw.bool(data.wastruncated);
//                        var cswGridOpts = {
//                            ID: o.ID,
//                            resizeWithParent: o.resizeWithParent,
//                            resizeWithParentElement: o.resizeWithParentElement,
//                            canEdit: false,
//                            canDelete: false,
//                            pagermode: 'default',
//                            gridOpts: {
//                                onSelectRow: function () {
//                                    if (false === ret.isMulti()) {
//                                        ret.resetSelection();
//                                    }
//                                }
//                            }, //toppager: (jqGridOpt.rowNum >= 50 && Csw.contains(gridJson, 'rows') && gridJson.rows.length >= 49)
//                            optNav: {},
//                            optSearch: {},
//                            optNavEdit: {},
//                            optNavDelete: {}
//                        };

//                        $.extend(true, cswGridOpts.gridOpts, jqGridOpt);

//                        var hasActions = (false === forReporting && false === Csw.bool(cswGridOpts.gridOpts.multiselect));
//                        if (forReporting) {
//                            cswGridOpts.gridOpts.caption = '';
//                        }
//                        else if (hasActions) {
//                            cswGridOpts.gridOpts.canEdit = false;
//                            cswGridOpts.gridOpts.canDelete = false;
//                            cswGridOpts.gridOpts.beforeSelectRow = function (rowid, eventObj) {
//                                function validateNode(className) {
//                                    if (-1 !== className.indexOf('csw-grid-edit')) {
//                                        editRows(rowid, ret, o.onEditNode, o.onEditView, o.onRefresh );
//                                    } else if (-1 !== className.indexOf('csw-grid-delete')) {
//                                        deleteRows(rowid, ret, o.onDeleteNode);
//                                    }
//                                }
//                                cswPrivate.selectedRowId = rowid;
//                                if (false === isMulti) {
//                                    if (Csw.contains(eventObj, 'toElement') && Csw.contains(eventObj.toElement, 'className')) {
//                                        validateNode(eventObj.toElement.className);
//                                    } else if (Csw.contains(eventObj, 'target') && Csw.isString(eventObj.target.className)) {
//                                        validateNode(eventObj.target.className);
//                                    }
//                                }
//                                return true;
//                            };

//                        }
//                        /* We need this to be defined upfront for multi-edit */
//                        cswGridOpts.optNavEdit = {
//                            editfunc: function (rowid) {
//                                return editRows(rowid, ret, o.onEditNode, o.onEditView, o.onRefresh);
//                            }
//                        };

//                        cswGridOpts.optNavDelete = {
//                            delfunc: function (rowid) {
//                                return deleteRows(rowid, ret, o.onDeleteNode);
//                            }
//                        };

//                        switch (pagerMode) {
//                            case 'local':
//                                cswGridOpts.gridOpts.datatype = 'local';
//                                cswGridOpts.gridOpts.data = data.rows;
//                                break;
//                            case 'server':
//                                /*
//                                This is the root of much suffering. 3rd-party libs which use jQuery.ajax() frequently screw it up such that the request is sent with an invalid return type.
//                                .NET will helpfully wrap the response in XML for you. 
//                                This is actually not helpful.

//                                We can either overwrite jqGrid's ajax implementation: in which case we have to build the _WHOLE_ thing,
//                                or we can modify the HTTPContext.Response object directly. 
//                                
//                                In the case of the latter, we need to guarantee that ONLY jqGrid properties defined in the jsonReader property are returned from the server.

//                                cswGridOpts.gridOpts.ajaxGridOptions = {
//                                url: o.gridPageUrl,
//                                dataType: 'json',
//                                contentType: 'application/json; charset=utf-8',
//                                type: 'POST',
//                                data: JSON.stringify({
//                                ViewId: o.viewid, Page: currentPage(), PageSize: 50, IsReport: forReporting  
//                                })
//                                };

//                                */
//                                cswGridOpts.gridOpts.datatype = 'json';
//                                cswGridOpts.gridOpts.url = getGridRowsUrl();
//                                cswGridOpts.gridOpts.jsonReader = {
//                                    root: 'rows',
//                                    page: 'page',
//                                    total: 'total',
//                                    records: 'records',
//                                    repeatitems: false
//                                };
//                                break;
//                        }

//                        var makeRowButtons = function (grid) {
//                            grid = grid || ret;
//                            /* Case 26506: gridComplete won't give us the Csw grid, and ret won't be defined on the first grid load (because it hasn't returned yet). */
//                            if (grid) {
//                                var ids = grid.getDataIds();
//                                for (var i = 0; i < ids.length; i += 1) {
//                                    var rowId = ids[i];
//                                    var cellData = Csw.string(grid.getCell(rowId, 'Action')).split(',');
//                                    var buttonStr = '';
//                                    if (Csw.contains(cellData, 'islocked')) {
//                                        buttonStr += '<img id="' + rowId + '_locked" src="Images/icons/lock.gif" alt="Quota exceeded" title="Quota exceeded" />';
//                                        if (Csw.contains(cellData, 'canview')) {
//                                            buttonStr += '<img id="' + rowId + '_view" src="Images/icons/docs.gif" class="csw-grid-edit" alt="View" title="View" />';
//                                        }
//                                    } else if (Csw.contains(cellData, 'canedit')) {
//                                        buttonStr += '<img id="' + rowId + '_edit" src="Images/icons/edit.gif" class="csw-grid-edit" alt="Edit" title="Edit" />';
//                                    } else if (Csw.contains(cellData, 'canview')) {
//                                        buttonStr += '<img id="' + rowId + '_view" src="Images/icons/docs.gif" class="csw-grid-edit" alt="View" title="View" />';
//                                    }
//                                    if (buttonStr.length > 0) {
//                                        buttonStr += '<img id="' + rowId + '_spacer" src="Images/icons/spacer.png" />';
//                                    }
//                                    if (Csw.contains(cellData, 'candelete')) {
//                                        buttonStr += '<img id="' + rowId + '_delete" src="Images/icons/trash.gif" class="csw-grid-delete" alt="Delete" title="Delete"/>';
//                                    }
//                                    /* Case 26506: We only want to change the cell content once. cellData.length isn't good enough, but buttonStr.length should be */
//                                    if (buttonStr.length > 0) {
//                                        grid.setRowData(rowId, 'Action', buttonStr);
//                                    }
//                                }
//                            }
//                        };
//                        cswGridOpts.onSuccess = makeRowButtons;
//                        /* Case 26506: grid.getDataIds() only fetches the rowids for the current page. gridComplete fires after load, sort and page--so it'll do. */
//                        cswGridOpts.gridOpts.gridComplete = makeRowButtons;

//                        cswGridOpts.printUrl = getGridRowsUrl(true);
//                        var parent = Csw.literals.factory($parent);
//                        ret = parent.grid(cswGridOpts);

//                        if (Csw.isFunction(o.onSuccess)) {
//                            o.onSuccess(ret);
//                        }
//                        ret.resizeWithParent();
//                        if (wasTruncated) {
//                            parent.append('<p>Results were truncated.</p>');
//                        }
//                    }; // makeGrid()

//                    if (false === doPaging) {
//                        Csw.ajax.post({
//                            url: o.gridAllRowsUrl,
//                            data: {
//                                ViewId: o.viewid,
//                                IsReport: forReporting,
//                                IncludeNodeKey: o.cswnbtnodekey
//                            },
//                            success: function (data) {
//                                makeGrid('local', data);

//                            }
//                        });
//                    } else {
//                        makeGrid('server');
//                    }
//                }; // buildGrid()

                var parent = Csw.literals.factory($parent);
                ret = parent.grid({
                                    ID: o.ID,
                                    stateId: o.viewid,
                                    ajax: {
                                        url: o.runGridUrl,
                                        data: {
                                            ViewId: o.viewid,
                                            IncludeNodeKey: o.cswnbtnodekey,
                                            IncludeInQuickLaunch: true,
                                            ForReport: forReporting
                                        }
                                    },
                                    readonly: forReporting,
                                    showActionColumn: true,
                                    onEdit: function(row) {
                                        var nodeid = row['nodeid'];
                                        var nodekey = row['nodekey'];
                                        var nodename = row['nodename'];
                
                                        $.CswDialog('EditNodeDialog', {
                                            nodeids: [ nodeid ],
                                            nodepks: [ nodeid ],
                                            nodekeys: [ nodekey ],
                                            nodenames: [ nodename ],
                                            onEditNode: o.onEditNode,
                                            onEditView: o.onEditView,
                                            onRefresh: o.onRefresh
                                        });
                                    },
                                    onDelete: function(row) {
                                        var nodeid = row['nodeid'];
                                        var nodekey = row['nodekey'];
                                        var nodename = row['nodename'];

                                        $.CswDialog('DeleteNodeDialog', {
                                            nodeids: [ nodeid ],
                                            nodekeys: [ nodekey ],
                                            nodenames: [ nodename ],
                                            onDeleteNode: o.onDeleteNode,
                                            Multi: isMulti,
                                            publishDeleteEvent: false
                                        });
                                    },
                                    width: '100%'
                                });

                Csw.tryExec(o.onSuccess);

            })();
            return ret;
        } // 'init'
    }; // methods

    $.fn.CswNodeGrid = function (method) {
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

