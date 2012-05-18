/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.composites.grid = Csw.composites.grid ||
        Csw.composites.register('grid', function (cswParent, options) {
            ///<summary>Generates a grid</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach grid to.</param>
            ///<param name="options" type="Object">Object defining paramaters for jqGrid construction.</param>
            ///<returns type="Csw.composites.grid">Object representing a CswGrid</returns>
            'use strict';
            var cswPrivateVar = {
                canEdit: false,
                canDelete: false,
                pagermode: 'default',
                ID: '',
                resizeWithParent: false,
                gridOpts: {
                    autoencode: true,
                    autowidth: true,
                    altRows: false, //window.internetExplorerVersionNo === -1,
                    caption: '',
                    datatype: 'local',
                    emptyrecords: 'No Results',
                    height: '300',
                    hidegrid: false,
                    loadtext: 'Loading...',
                    multiselect: false,
                    toppager: false,
                    shrinkToFit: true,
                    sortname: '',
                    sortorder: 'asc',
                    //width: '600px',
                    rowNum: 10,
                    rowList: [10, 25, 50],        /* page size dropdown */
                    pgbuttons: true,     /* page control like next, back button */
                    /*pgtext: null,         pager text like 'Page 0 of 10' */
                    viewrecords: true    /* current view record text like 'View 1-10 of 100' */
                },
                optSearch: {
                    caption: "Search...",
                    Find: "Find",
                    Reset: "Reset",
                    odata: ['equal',
                        'not equal',
                        'less',
                        'less or equal',
                        'greater',
                        'greater or equal',
                        'begins with',
                        'does not begin with',
                        'is in',
                        'is not in',
                        'ends with',
                        'does not end with',
                        'contains',
                        'does not contain'],
                    groupOps: [{ op: "AND", text: "all" }, { op: "OR", text: "any"}],
                    matchText: "match",
                    rulesText: "rules"
                },
                optNavEdit: {
                    edit: true,
                    edittext: "",
                    edittitle: "Edit",
                    editfunc: null
                },
                optNavDelete: {
                    del: true,
                    deltext: "",
                    deltitle: "Delete",
                    delfunc: null
                },
                optNav: {
                    cloneToTop: false,

                    add: false,
                    del: false,
                    edit: false,

                    //search
                    search: false,
                    searchtext: "",
                    searchtitle: "Find records",


                    refresh: false,

                    alertcap: "Warning",
                    alerttext: "Please, select row",

                    //view
                    view: false,
                    viewtext: "",
                    viewtitle: "View"
                    //viewfunc: none--use jqGrid built-in function for read-only
                }
            };
            var cswPublicRet = {};

            cswPrivateVar.insertWhiteSpace = function (num) {
                var ret = '', i;
                for (i = 0; i < num; i += 1) {
                    ret += '&nbsp;';
                }
                return ret;
            };

            cswPrivateVar.makeCustomPager = function (pagerDef) {
                var prevButton = {
                    caption: cswPrivateVar.insertWhiteSpace(2),
                    buttonicon: 'ui-icon-seek-prev',
                    position: 'last',
                    title: '',
                    cursor: '',
                    id: Csw.makeId(cswPrivateVar.gridPagerId, 'prevBtn')
                };
                if (false === Csw.isNullOrEmpty(pagerDef) && Csw.isFunction(pagerDef.onPrevPageClick)) {
                    prevButton.onClickButton = function (eventObj) {
                        var nodes = cswPublicRet.gridTable.$.jqGrid('getDataIDs'),
                            firstNodeId = nodes[0],
                            lastNodeId = nodes[nodes.length],
                            firstRow = cswPublicRet.gridTable.$.jqGrid('getRowData', firstNodeId),
                            lastRow = cswPublicRet.gridTable.$.jqGrid('getRowData', lastNodeId);

                        pagerDef.onPrevPageClick(eventObj, firstRow, lastRow);
                    };
                }

                var spacer = {
                    sepclass: 'ui-separator',
                    sepcontent: cswPrivateVar.insertWhiteSpace(24)
                };

                var nextButton = {
                    caption: cswPrivateVar.insertWhiteSpace(2),
                    buttonicon: 'ui-icon-seek-next',
                    onClickButton: '',
                    position: 'last',
                    title: 'Next',
                    cursor: '',
                    id: Csw.makeId(cswPrivateVar.gridPagerId, 'nextBtn')
                };
                if (false === Csw.isNullOrEmpty(pagerDef) && Csw.isFunction(pagerDef.onNextPageClick)) {
                    nextButton.onClickButton = function (eventObj) {
                        var nodes = cswPublicRet.gridTable.$.jqGrid('getDataIDs'),
                            firstNodeId = nodes[0],
                            lastNodeId = nodes[nodes.length - 1],
                            firstRow = cswPublicRet.gridTable.$.jqGrid('getRowData', firstNodeId),
                            lastRow = cswPublicRet.gridTable.$.jqGrid('getRowData', lastNodeId);

                        pagerDef.onNextPageClick(eventObj, firstRow, lastRow);
                    };
                }
                cswPublicRet.gridTable.$.jqGrid('navSeparatorAdd', '#' + cswPrivateVar.gridPagerId, spacer)
                    .jqGrid('navButtonAdd', '#' + cswPrivateVar.gridPagerId, prevButton)
                    .jqGrid('navButtonAdd', '#' + cswPrivateVar.gridPagerId, nextButton);
            };

            cswPrivateVar.makeGrid = function () {
                cswPrivateVar.multiEdit = cswPrivateVar.gridOpts.multiselect;
                /* Case 25809 */
                cswPrivateVar.gridDiv.empty();

                cswPublicRet.gridTable = cswPrivateVar.gridDiv.table({
                    ID: cswPrivateVar.gridTableId
                });

                cswPublicRet.gridPager = cswPrivateVar.gridDiv.div({ ID: cswPrivateVar.gridPagerId });

                cswPrivateVar.gridOpts.pager = cswPublicRet.gridPager.$;
                if (cswPrivateVar.canEdit) {
                    $.extend(true, cswPrivateVar.optNav, cswPrivateVar.optNavEdit);
                }
                if (cswPrivateVar.canDelete) {
                    $.extend(true, cswPrivateVar.optNav, cswPrivateVar.optNavDelete);
                }

                if (cswPrivateVar.pagermode === 'default' || cswPrivateVar.pagermode === 'custom') {
                    try {
                        if (false === Csw.contains(cswPrivateVar.gridOpts, 'colNames') ||
                            cswPrivateVar.gridOpts.colNames.length === 0 ||
                                (Csw.contains(cswPrivateVar.gridOpts, 'colModel') && cswPrivateVar.gridOpts.colNames.length !== cswPrivateVar.gridOpts.colModel.length)) {
                            throw new Error('Cannot create a grid without at least one column defined.');
                        }
                        cswPublicRet.gridTable.$.jqGrid(cswPrivateVar.gridOpts)
                            .jqGrid('navGrid', '#' + cswPrivateVar.gridPagerId, cswPrivateVar.optNav, {}, {}, {}, {}, {}); //Case 24032: Removed jqGrid search
                    } catch (e) {
                        Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, e.message));
                    }
                    if (cswPrivateVar.pagermode === 'custom') {
                        cswPrivateVar.makeCustomPager(cswPrivateVar.customPager);
                    }
                } else {
                    cswPublicRet.gridTable.$.jqGrid(cswPrivateVar.gridOpts);
                }
            };

            cswPrivateVar.getCell = function (rowid, key) {
                var ret = '';
                if (false === Csw.isNullOrEmpty(rowid) && false === Csw.isNullOrEmpty(key)) {
                    ret = cswPublicRet.gridTable.$.jqGrid('getCell', rowid, key);
                }
                return ret;
            };

            cswPublicRet.getSelectedRowId = function () {
                var rowid = cswPublicRet.gridTable.$.jqGrid('getGridParam', 'selrow');
                return rowid;
            };

            cswPrivateVar.getSelectedRowsIds = function () {
                var rowid = cswPublicRet.gridTable.$.jqGrid('getGridParam', 'selarrrow');
                return rowid;
            };

            cswPrivateVar.getColumn = function (column, returnType) {
                ///<summary>Gets the contents of a jqGrid column</summary>
                ///<param name="column" type="String">Column name</param>
                ///<param name="returnType" type="Boolean">If false, returns a simple array of values. If true, returns an array [{id: id, value: value},{...}]</param>
                ///<returns type="Array">An array of the columns values</returns>
                var ret = cswPublicRet.gridTable.$.jqGrid('getCol', column, returnType);
                return ret;
            };

            cswPublicRet.hideColumn = function (id) {
                cswPublicRet.gridTable.$.jqGrid('hideCol', id);
            };

            cswPublicRet.scrollToRow = function (rowid) {
                ///<summary>Scrolls the grid to the specified rowid</summary>
                ///<param name="rowid" type="String">Optional. jqGrid rowid. If null, selected row is assumed.</param>
                ///<returns type="Void"></returns>
                if (Csw.isNullOrEmpty(rowid)) {
                    rowid = cswPublicRet.getSelectedRowId();
                }
                var rowHeight = cswPublicRet.getGridRowHeight() || 23; // Default height
                var index = cswPublicRet.gridTable.$.getInd(rowid);
                cswPublicRet.gridTable.$.closest(".ui-jqgrid-bdiv").scrollTop(rowHeight * (index - 1));
            };

            cswPublicRet.getRowIdForVal = function (value, column) {
                ///<summary>Gets a jqGrid rowid by column name and value.</summary>
                ///<param name="value" type="String">Cell value</param>
                ///<param name="column" type="String">Column name</param>
                ///<returns type="String">jqGrid row id.</returns>
                var pks = cswPrivateVar.getColumn(column, true);
                var rowid = 0;
                Csw.each(pks, function (obj) {
                    if (Csw.contains(obj, 'value') && Csw.string(obj.value) === Csw.string(value)) {
                        rowid = obj.id;
                    }
                });
                return rowid;
            };

            cswPublicRet.getValueForColumn = function (columnname, rowid) {
                ///<summary>Gets a cell value by column name.</summary>
                ///<param name="columnname" type="String">Grid column name.</param>
                ///<param name="rowid" type="String">Optional. If null, selected row is assumed.</param>
                ///<returns type="String">Value of the cell.</returns>
                if (Csw.isNullOrEmpty(rowid)) {
                    rowid = cswPublicRet.getSelectedRowId();
                }
                var ret = cswPrivateVar.getCell(rowid, columnname);
                return ret;
            };

            cswPublicRet.setSelection = function (rowid) {
                ///<summary>Sets the selected row by jqGrid's rowid</summary>
                if (false === Csw.isNullOrEmpty(rowid)) {
                    cswPublicRet.gridTable.$.setSelection(rowid);
                }
            };

            cswPublicRet.changeGridOpts = function (opts, toggleColumns) {
                var delBtn, editBtn;
                $.extend(true, cswPrivateVar, opts);
                cswPrivateVar.makeGrid(cswPrivateVar);

                Csw.each(toggleColumns, function (val) {
                    if (Csw.contains(cswPrivateVar.gridOpts.colNames, val)) {
                        if (cswPublicRet.isMulti()) {
                            cswPublicRet.gridTable.$.jqGrid('hideCol', val);
                        }
                    }
                });
                if (false === cswPublicRet.isMulti()) {
                    if (false === cswPrivateVar.canEdit) {
                        editBtn = cswPublicRet.gridPager.find('#edit_' + cswPrivateVar.gridTableId);
                        if (Csw.contains(editBtn, 'remove')) {
                            editBtn.remove();
                        }
                    }
                    if (false === cswPrivateVar.canDelete) {
                        delBtn = cswPublicRet.gridPager.find('#del_' + cswPrivateVar.gridTableId).remove();
                        if (Csw.contains(delBtn, 'remove')) {
                            delBtn.remove();
                        }
                    }
                }
                cswPublicRet.resizeWithParent();
            };

            cswPublicRet.opGridRows = function (opts, rowid, onSelect, onEmpty) {
                var ret = false;
                var haveSelectedRows = false,
                    i;

                var rowids = [];

                function onEachGridRow(prop, key, parent) {
                    if (false === Csw.isFunction(parent[key])) {
                        if (Csw.isArray(parent[key])) {
                            rowid = rowids[i];
                            parent[key].push(cswPublicRet.getValueForColumn(key, rowid));
                        } else {
                            parent[key] = cswPublicRet.getValueForColumn(key, rowid);
                        }
                    }
                    return false;
                }

                if (cswPrivateVar.multiEdit) {
                    rowids = cswPrivateVar.getSelectedRowsIds();
                } else if (false === Csw.isNullOrEmpty(rowid)) {
                    rowids.push(rowid);
                } else {
                    rowids.push(cswPublicRet.getSelectedRowId());
                }

                if (rowids.length > 0) {
                    haveSelectedRows = true;
                    for (i = 0; i < rowids.length; i += 1) {
                        Csw.crawlObject(opts, onEachGridRow, false);
                    }
                }

                if (haveSelectedRows) {
                    if (Csw.isFunction(onSelect)) {
                        opts.Multi = cswPrivateVar.multiEdit;
                        ret = onSelect(opts);
                    }
                } else if (Csw.isFunction(onEmpty)) {
                    onEmpty(opts);
                }
                return ret;
            };

            cswPublicRet.getAllGridRows = function () {
                return cswPublicRet.gridTable.$.jqGrid('getRowData');
            };

            cswPublicRet.print = function (onSuccess) {

                try {

                    var outerDiv = cswParent.div();
                    var newDiv = outerDiv.div({ width: '800px' });

                    var printOpts = {},
                        printTableId = Csw.makeId(cswPrivateVar.gridTableId, 'printTable'),
                        newGrid, data, i;

                    var addRowsToGrid = function (rowData) {
                        if (rowData) {
                            /* Add the rows to the new newGrid */
                            for (i = 0; i <= rowData.length; i += 1) {
                                newGrid.gridTable.$.jqGrid('addRowData', i + 1, rowData[i]);
                            }
                        }
                    };

                    /* Case 26020 */
                    $.extend(true, printOpts, cswPrivateVar);

                    /* Nuke anything that might be holding onto a reference */
                    Csw.each(printOpts, function (thisObj, name) {
                        if (Csw.isFunction(thisObj) || Csw.isJQuery(thisObj)) {
                            delete printOpts[name];
                        }
                    });

                    printOpts.ID = printTableId;

                    /* 
                    Nuke any existing options with vanilla defaults.
                    Since jqGrid 3.6, there hasn't been an 'All' rowNum option. Just use a really high number.
                    */
                    delete printOpts.gridOpts.canEdit;
                    delete printOpts.gridOpts.canDelete;
                    delete printOpts.canEdit;
                    delete printOpts.canDelete;

                    printOpts.gridPagerId += '_print';
                    printOpts.gridTableId += '_print';
                    printOpts.gridOpts.rowNum = 100000;
                    printOpts.gridOpts.rowList = [100000];
                    printOpts.gridOpts.add = false;
                    printOpts.gridOpts.del = false;
                    printOpts.gridOpts.edit = false;
                    printOpts.gridOpts.autoencode = true;
                    //printOpts.gridOpts.autowidth = true;
                    printOpts.gridOpts.width = 800;
                    printOpts.gridOpts.altRows = false;
                    printOpts.gridOpts.datatype = 'local';
                    delete printOpts.gridOpts.url;
                    printOpts.gridOpts.emptyrecords = 'No Results';
                    printOpts.gridOpts.height = 'auto';
                    printOpts.gridOpts.multiselect = false;
                    printOpts.gridOpts.toppager = false;
                    //printOpts.gridOpts.forceFit = true;
                    //printOpts.gridOpts.shrinkToFit = true;

                    /*
                    jqGrid cannot seem to handle the communication of the data property between window objects.
                    Just delete it and rebuild instead.
                    */
                    data = cswPrivateVar.gridOpts.data;

                    Csw.each(printOpts.gridOpts.colModel, function (column) {
                        /* This provides text wrapping in cells */
                        column.cellattr = function () {
                            return 'style="white-space: normal;"';
                        };
                    });
                    Csw.tryExec(onSuccess, newDiv);
                    /* Get a new Csw.newGrid */
                    newGrid = newDiv.grid(printOpts);

                    if (Csw.isNullOrEmpty(data) && false === Csw.isNullOrEmpty(printOpts.printUrl)) {
                        Csw.ajax.get({
                            url: printOpts.printUrl,
                            success: function (rows) {
                                addRowsToGrid(rows.rows);
                            }
                        });
                    } else {
                        /* Get the data (rows) from the current grid */
                        addRowsToGrid(data);
                    }
                    

                    
                    Csw.newWindow(outerDiv.$.html());
                    outerDiv.remove();

                } catch (e) {
                    Csw.log(e);
                }

            };

            // Row scrolling adapted from 
            // http://stackoverflow.com/questions/2549466/is-there-a-way-to-make-jqgrid-scroll-to-the-bottom-when-a-new-row-is-added/2549654#2549654
            cswPublicRet.getGridRowHeight = function () {

                var height = null; // Default
                try {
                    height = cswPublicRet.gridTable.$.find('tbody').find('tr:first').outerHeight();
                } catch (e) {
                    //catch and just suppress error
                }
                return height;
            };

            cswPublicRet.isMulti = function () {
                return cswPrivateVar.multiEdit;
            };

            cswPublicRet.setWidth = function (width) {
                cswPublicRet.gridTable.$.jqGrid('setGridWidth', width);
            };

            cswPublicRet.resizeWithParent = function () {
                cswPublicRet.gridTable.$.jqGrid('setGridWidth', cswParent.$.width() - 100);
            };

            /* "Constuctor" */
            (function () {
                $.extend(true, cswPrivateVar, options);

                switch (cswPrivateVar.pagermode) {
                    case 'none':
                        delete cswPrivateVar.gridOpts.pager;
                        //delete cswPrivateVar.gridOpts.rowNum;
                        //delete cswPrivateVar.gridOpts.rowList;
                        delete cswPrivateVar.gridOpts.pgbuttons;
                        delete cswPrivateVar.gridOpts.viewrecords;
                        delete cswPrivateVar.gridOpts.pgtext;
                        break;
                    case 'default':
                        //accept defaults
                        break;
                    case 'custom':
                        cswPrivateVar.gridOpts.rowNum = null;
                        cswPrivateVar.gridOpts.rowList = [];
                        cswPrivateVar.gridOpts.pgbuttons = false;
                        cswPrivateVar.gridOpts.viewrecords = false;
                        cswPrivateVar.gridOpts.pgtext = null;
                        break;
                }

                cswPrivateVar.gridPagerId = cswPrivateVar.gridPagerId || Csw.makeId('cswGridPager', cswPrivateVar.ID);
                cswPrivateVar.gridTableId = cswPrivateVar.gridTableId || Csw.makeId('cswGridTable', cswPrivateVar.ID);

                cswParent.empty();
                cswPrivateVar.gridDiv = cswParent.div({
                    isControl: cswPrivateVar.isControl,
                    ID: cswPrivateVar.ID
                });
                //$.extend(cswPublicRet, Csw.literals.div(cswPrivateVar));
                if (cswPrivateVar.resizeWithParent) {
                    $(window).bind('resize', cswPublicRet.resizeWithParent);
                }
                cswPrivateVar.makeGrid();
            } ());


            return cswPublicRet;
        });

} ());
