/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswGrid() {
    'use strict';
    function grid(options, $parent) {
        ///<summary>Generates a grid</summary>
        ///<param name="options" type="Object">Object defining paramaters for jqGrid construction</param>
        ///<param name="$parent" type="JQuery">Parent element to attach grid to.</param>
        ///<returns type="Object">Object representing a CswGrid</returns>
        var internal = {};
        var external = {};

        internal.insertWhiteSpace = function (num) {
            var ret = '', i;
            for (i = 0; i < num; i += 1) {
                ret += '&nbsp;';
            }
            return ret;
        };

        internal.makeCustomPager = function (pagerDef) {
            var prevButton = {
                caption: internal.insertWhiteSpace(2),
                buttonicon: 'ui-icon-seek-prev',
                position: 'last',
                title: '',
                cursor: '',
                id: internal.gridPagerId + '_prevBtn'
            };
            if (false === Csw.isNullOrEmpty(pagerDef) && Csw.isFunction(pagerDef.onPrevPageClick)) {
                prevButton.onClickButton = function (eventObj) {
                    var nodes = external.$gridTable.jqGrid('getDataIDs'),
                        firstNodeId = nodes[0],
                        lastNodeId = nodes[nodes.length],
                        firstRow = external.$gridTable.jqGrid('getRowData', firstNodeId),
                        lastRow = external.$gridTable.jqGrid('getRowData', lastNodeId);

                    pagerDef.onPrevPageClick(eventObj, firstRow, lastRow);
                };
            }

            var spacer = {
                sepclass: 'ui-separator',
                sepcontent: internal.insertWhiteSpace(24)
            };

            var nextButton = {
                caption: internal.insertWhiteSpace(2),
                buttonicon: 'ui-icon-seek-next',
                onClickButton: '',
                position: 'last',
                title: 'Next',
                cursor: '',
                id: internal.gridPagerId + '_nextBtn'
            };
            if (false === Csw.isNullOrEmpty(pagerDef) && Csw.isFunction(pagerDef.onNextPageClick)) {
                nextButton.onClickButton = function (eventObj) {
                    var nodes = external.$gridTable.jqGrid('getDataIDs'),
                        firstNodeId = nodes[0],
                        lastNodeId = nodes[nodes.length - 1],
                        firstRow = external.$gridTable.jqGrid('getRowData', firstNodeId),
                        lastRow = external.$gridTable.jqGrid('getRowData', lastNodeId);

                    pagerDef.onNextPageClick(eventObj, firstRow, lastRow);
                };
            }
            external.$gridTable.jqGrid('navSeparatorAdd', '#' + internal.gridPagerId, spacer)
                .jqGrid('navButtonAdd', '#' + internal.gridPagerId, prevButton)
                .jqGrid('navButtonAdd', '#' + internal.gridPagerId, nextButton);
        };

        internal.makeGrid = function (o) {
            var table;
            internal.multiEdit = o.gridOpts.multiselect;
            if (Csw.isNullOrEmpty($parent)) {
                $parent = $('<div id="' + internal.gridTableId + '_parent"></div>');
            }
            table = Csw.controls.table({
                ID: internal.gridTableId,
                $parent: $parent
            });
            external.$gridTable = table.$;
            
            external.$gridPager = $parent.CswDiv('init', {ID: internal.gridPagerId});
            o.gridOpts.pager = external.$gridPager;

            if (o.canEdit) {
                $.extend(true, o.optNav, o.optNavEdit);
            }
            if (o.canDelete) {
                $.extend(true, o.optNav, o.optNavDelete);
            }

            if (o.pagermode === 'default' || o.pagermode === 'custom') {
                try {
                    if (false === Csw.contains(o.gridOpts, 'colNames') ||
                        o.gridOpts.colNames.length === 0 ||
                            (Csw.contains(o.gridOpts, 'colModel') && o.gridOpts.colNames.length !== o.gridOpts.colModel.length)) {
                        throw new Error('Cannot create a grid without at least one column defined.');
                    }
                    external.$gridTable.jqGrid(o.gridOpts)
                        .jqGrid('navGrid', '#' + internal.gridPagerId, o.optNav, {}, {}, {}, {}, {}); //Case 24032: Removed jqGrid search
                } catch (e) {
                    Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, e.message));
                }
                if (o.pagermode === 'custom') {
                    internal.makeCustomPager(o.customPager);
                }
            } else {
                external.$gridTable.jqGrid(o.gridOpts);
            }
            external.$gridTable.data(internal.gridTableId + '_data', o);
        };

        /* "Constuctor" */
        (function () {
            var o = {
                canEdit: false,
                canDelete: false,
                pagermode: 'default',
                ID: '',
                gridOpts: {
                    autoencode: true,
                    //autowidth: true,
                    altRows: false,
                    caption: '',
                    datatype: 'local',
                    emptyrecords: 'No Results',
                    height: '300',
                    loadtext: 'Loading...',
                    multiselect: false,
                    pager: external.$gridPager,
                    toppager: false,
                    shrinkToFit: true,
                    sortname: '',
                    sortorder: 'asc',
                    width: '600px',
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
                    groupOps: [{op: "AND", text: "all"}, {op: "OR", text: "any"}],
                    matchText: "match",
                    rulesText: "rules"
                },
                optNavEdit: {
                    edit: true,
                    edittext: "",
                    edittitle: "Edit row",
                    editfunc: null
                },
                optNavDelete: {
                    del: true,
                    deltext: "",
                    deltitle: "Delete row",
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

                    //refresh
                    refreshtext: "",
                    refreshtitle: "Reload Grid",
                    alertcap: "Warning",
                    alerttext: "Please, select row",

                    //view
                    view: true,
                    viewtext: "",
                    viewtitle: "View row"
                //viewfunc: none--use jqGrid built-in function for read-only
                }
            };

            $.extend(true, o, options);

            switch (o.pagermode) {
                case 'none':
                    delete o.gridOpts.pager;
                    delete o.gridOpts.rowNum;
                    delete o.gridOpts.rowList;
                    delete o.gridOpts.pgbuttons;
                    delete o.gridOpts.viewrecords;
                    delete o.gridOpts.pgtext;
                    break;
                case 'default':
                //accept defaults
                    break;
                case 'custom':
                    o.gridOpts.rowNum = null;
                    o.gridOpts.rowList = [];
                    o.gridOpts.pgbuttons = false;
                    o.gridOpts.viewrecords = false;
                    o.gridOpts.pgtext = null;
                    break;
            }

            internal.gridPagerId = Csw.controls.dom.makeId({ID: 'cswGridPager', prefix: o.ID});
            internal.gridTableId = Csw.controls.dom.makeId({ID: 'cswGridTable', prefix: o.ID});

            internal.makeGrid(o);
        }());

        internal.getCell = function (rowid, key) {
            var ret = '';
            if (false === Csw.isNullOrEmpty(rowid) && false === Csw.isNullOrEmpty(key)) {
                ret = external.$gridTable.jqGrid('getCell', rowid, key);
            }
            return ret;
        };

        internal.getSelectedRowId = function () {
            var rowid = external.$gridTable.jqGrid('getGridParam', 'selrow');
            return rowid;
        };

        internal.getSelectedRowsIds = function () {
            var rowid = external.$gridTable.jqGrid('getGridParam', 'selarrrow');
            return rowid;
        };

        internal.getColumn = function (column, returnType) {
            ///<summary>Gets the contents of a jqGrid column</summary>
            ///<param name="column" type="String">Column name</param>
            ///<param name="returnType" type="Boolean">If false, returns a simple array of values. If true, returns an array [{id: id, value: value},{...}]</param>
            ///<returns type="Array">An array of the columns values</returns>
            var ret = external.$gridTable.jqGrid('getCol', column, returnType);
            return ret;
        };

        external.hideColumn = function (id) {
            external.$gridTable.jqGrid('hideCol', id);
        };

        external.scrollToRow = function (rowid) {
            ///<summary>Scrolls the grid to the specified rowid</summary>
            ///<param name="rowid" type="String">Optional. jqGrid rowid. If null, selected row is assumed.</param>
            ///<returns type="Void"></returns>
            if (Csw.isNullOrEmpty(rowid)) {
                rowid = internal.getSelectedRowId();
            }
            var rowHeight = external.getGridRowHeight(external.$gridTable) || 23; // Default height
            var index = external.$gridTable.getInd(rowid);
            external.$gridTable.closest(".ui-jqgrid-bdiv").scrollTop(rowHeight * (index - 1));
        };

        external.getRowIdForVal = function (value, column) {
            ///<summary>Gets a jqGrid rowid by column name and value.</summary>
            ///<param name="value" type="String">Cell value</param>
            ///<param name="column" type="String">Column name</param>
            ///<returns type="String">jqGrid row id.</returns>
            var pks = internal.getColumn(column, true);
            var rowid = 0;
            Csw.each(pks, function (obj) {
                if (Csw.contains(obj, 'value') && Csw.string(obj.value) === Csw.string(value)) {
                    rowid = obj.id;
                }
            });
            return rowid;
        };

        external.getValueForColumn = function (columnname, rowid) {
            ///<summary>Gets a cell value by column name.</summary>
            ///<param name="columnname" type="String">Grid column name.</param>
            ///<param name="rowid" type="String">Optional. If null, selected row is assumed.</param>
            ///<returns type="String">Value of the cell.</returns>
            if (Csw.isNullOrEmpty(rowid)) {
                rowid = internal.getSelectedRowId();
            }
            var ret = internal.getCell(rowid, columnname);
            return ret;
        };

        external.setSelection = function (rowid) {
            ///<summary>Sets the selected row by jqGrid's rowid</summary>
            if (false === Csw.isNullOrEmpty(rowid)) {
                external.$gridTable.setSelection(rowid);
            }
        };

        external.changeGridOpts = function (opts) {
            var currentOpts = external.$gridTable.data(internal.gridTableId + '_data');
            $.extend(true, currentOpts, opts);
            $parent.empty();
            internal.makeGrid(currentOpts);
        };

        external.opGridRows = function (opts, rowid, onSelect, onEmpty) {
            var ret = false;
            var haveSelectedRows = false,
                i;

            var rowids = [];

            function onEachGridRow (prop, key, parent) {
                if (false === Csw.isFunction(parent[key])) {
                    if (Csw.isArray(parent[key])) {
                        rowid = rowids[i];
                        parent[key].push(external.getValueForColumn(key, rowid));
                    } else {
                        parent[key] = external.getValueForColumn(key, rowid);
                    }
                }
                return false;
            }

            if (internal.multiEdit) {
                rowids = internal.getSelectedRowsIds();
            } else if (false === Csw.isNullOrEmpty(rowid)) {
                rowids.push(rowid);
            } else {
                rowids.push(internal.getSelectedRowId());
            }

            if (rowids.length > 0) {
                haveSelectedRows = true;
                for (i = 0; i < rowids.length; i += 1) {
                    Csw.crawlObject(opts, onEachGridRow, false);
                }
            }

            if (haveSelectedRows) {
                if (Csw.isFunction(onSelect)) {
                    opts.Multi = internal.multiEdit;
                    ret = onSelect(opts);
                }
            } else if (Csw.isFunction(onEmpty)) {
                onEmpty(opts);
            }
            return ret;
        };

        external.getAllGridRows = function () {
            return external.$gridTable.jqGrid('getRowData');
        };

        external.print = function () {

            Csw.print(function ($printElement) {
                var printOpts = {},
                    currentOpts = external.$gridTable.data(internal.gridTableId + '_data'),
                    printTableId = Csw.controls.dom.makeId({prefix: internal.gridTableId, ID: 'printTable'}),
                    newGrid, data, i;

                var addRowsToGrid = function (rowData) {
                    if (rowData) {
                        /* Add the rows to the new newGrid */
                        for (i = 0; i <= rowData.length; i += 1) {
                            newGrid.external.$gridTable.jqGrid('addRowData', i + 1, rowData[i]);
                        }
                    }
                };

                $.extend(printOpts, currentOpts);

                /* 
            It is vital that grids have a unique ID--even across window objects. 
            Until this callback returns, we're still sharing a global space.
            */
                printOpts.ID = printTableId;

                /* 
            Nuke any existing options with vanilla defaults.
            Since jqGrid 3.6, there hasn't been an 'All' rowNum option. Just use a really high number.
            */
                delete printOpts.gridOpts.canEdit;
                delete printOpts.gridOpts.canDelete;
                delete printOpts.canEdit;
                delete printOpts.canDelete;

                printOpts.gridOpts.rowNum = 100000;
                printOpts.gridOpts.rowList = [100000];
                printOpts.gridOpts.add = false;
                printOpts.gridOpts.del = false;
                printOpts.gridOpts.edit = false;
                printOpts.gridOpts.autoencode = true;
                //printOpts.gridOpts.autowidth = true;
                printOpts.gridOpts.width = $(window).width() - 40;
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
                data = printOpts.gridOpts.data;
                delete printOpts.gridOpts.data;

                Csw.each(printOpts.gridOpts.colModel, function (column) {
                    /* This provides text wrapping in cells */
                    column.cellattr = function () {
                        return 'style="white-space: normal;"';
                    };
                });

                /* Get a new Csw.newGrid */
                newGrid = grid(printOpts, $printElement);

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

            });

        };

        // Row scrolling adapted from 
        // http://stackoverflow.com/questions/2549466/is-there-a-way-to-make-jqgrid-scroll-to-the-bottom-when-a-new-row-is-added/2549654#2549654
        external.getGridRowHeight = function () {

            var height = null; // Default
            try {
                height = external.$gridTable.find('tbody').find('tr:first').outerHeight();
            } catch (e) {
                //catch and just suppress error
            }
            return height;
        };

        external.isMulti = function () {
            return internal.multiEdit;
        };

        return external;
    }

    Csw.register('grid', grid);
    Csw.controls.grid = Csw.controls.grid || grid;
    
}());
