/// <reference path="/js/../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../main/tools/CswPrint.js" />

function CswGrid(options, $parent) {
    ///<summary>Generates a grid</summary>
    ///<param name="options" type="Object">Object defining paramaters for jqGrid construction</param>
    ///<param name="$parent" type="JQuery">Parent element to attach grid to.</param>
    ///<returns type="CswGrid">CswGrid</returns>
    "use strict";
    var $gridTable, $gridPager, $topPager,
        multiEdit = false,
        gridTableId, gridPagerId;
    //#region private

    (function() {
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
                pager: $gridPager,
                toppager: false,
                shrinkToFit: true,
                sortname: '',
                sortorder: 'asc',
                width: '600px',
                rowNum: 10,
                rowList: [10, 25, 50],        //page size dropdown
                pgbuttons: true,     //page control like next, back button
                //pgtext: null,         //pager text like 'Page 0 of 10'
                viewrecords: true    //current view record text like 'View 1-10 of 100'
            },
            optSearch: {
                caption: "Search...",
                Find: "Find",
                Reset: "Reset",
                odata: ['equal', 'not equal', 'less', 'less or equal', 'greater', 'greater or equal', 'begins with', 'does not begin with', 'is in', 'is not in', 'ends with', 'does not end with', 'contains', 'does not contain'],
                groupOps: [{ op: "AND", text: "all" }, { op: "OR", text: "any" }],
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

        gridPagerId = makeId({ ID: 'CswGridPager', prefix: o.ID });
        gridTableId = makeId({ ID: 'CswGridTable', prefix: o.ID });

        makeGrid(o);
    })();

    function insertWhiteSpace(num) {
        var ret = '', i;
        for (i = 0; i < num; i += 1) {
            ret += '&nbsp;';
        }
        return ret;
    }

    function makeCustomPager(pagerDef) {
        var prevButton = {
            caption: insertWhiteSpace(2),
            buttonicon: 'ui-icon-seek-prev',
            position: 'last',
            title: '',
            cursor: '',
            id: gridPagerId + '_prevBtn'
        };
        if (false === isNullOrEmpty(pagerDef) && isFunction(pagerDef.onPrevPageClick)) {
            prevButton.onClickButton = function(eventObj) {
                var nodes = $gridTable.jqGrid('getDataIDs'),
                    firstNodeId = nodes[0],
                    lastNodeId = nodes[nodes.length],
                    firstRow = $gridTable.jqGrid('getRowData', firstNodeId),
                    lastRow = $gridTable.jqGrid('getRowData', lastNodeId);

                pagerDef.onPrevPageClick(eventObj, firstRow, lastRow);
            };
        }

        var spacer = {
            sepclass: 'ui-separator',
            sepcontent: insertWhiteSpace(24)
        };

        var nextButton = {
            caption: insertWhiteSpace(2),
            buttonicon: 'ui-icon-seek-next',
            onClickButton: '',
            position: 'last',
            title: 'Next',
            cursor: '',
            id: gridPagerId + '_nextBtn'
        };
        if (false === isNullOrEmpty(pagerDef) && isFunction(pagerDef.onNextPageClick)) {
            nextButton.onClickButton = function(eventObj) {
                var nodes = $gridTable.jqGrid('getDataIDs'),
                    firstNodeId = nodes[0],
                    lastNodeId = nodes[nodes.length - 1],
                    firstRow = $gridTable.jqGrid('getRowData', firstNodeId),
                    lastRow = $gridTable.jqGrid('getRowData', lastNodeId);

                pagerDef.onNextPageClick(eventObj, firstRow, lastRow);
            };
        }
        $gridTable.jqGrid('navSeparatorAdd', '#' + gridPagerId, spacer)
            .jqGrid('navButtonAdd', '#' + gridPagerId, prevButton)
            .jqGrid('navButtonAdd', '#' + gridPagerId, nextButton);
    }

    function makeGrid(o) {
        multiEdit = o.gridOpts.multiselect;
        if (isNullOrEmpty($parent)) {
            $parent = $('<div id="' + gridTableId + '_parent"></div>');
        }

        $gridTable = $parent.CswTable('init', { ID: gridTableId });


        $gridPager = $parent.CswDiv('init', { ID: gridPagerId });
        o.gridOpts.pager = $gridPager;

        if (o.canEdit) {
            $.extend(true, o.optNav, o.optNavEdit);
        }
        if (o.canDelete) {
            $.extend(true, o.optNav, o.optNavDelete);
        }

        if (o.pagermode === 'default' || o.pagermode === 'custom') {
            $gridTable.jqGrid(o.gridOpts)
                .jqGrid('navGrid', '#' + gridPagerId, o.optNav, { }, { }, { }, { }, { }); //Case 24032: Removed jqGrid search

            if (o.pagermode === 'custom') {
                makeCustomPager(o.customPager);
            }
        } else {
            $gridTable.jqGrid(o.gridOpts);
        }
        $gridTable.data(gridTableId + '_data', o);
    }

    // Row scrolling adapted from 
    // http://stackoverflow.com/questions/2549466/is-there-a-way-to-make-jqgrid-scroll-to-the-bottom-when-a-new-row-is-added/2549654#2549654
    function getGridRowHeight() {

        var height = null; // Default
        try {
            height = $gridTable.find('tbody').find('tr:first').outerHeight();
        }
        catch(e) {
            //catch and just suppress error
        }
        return height;
    }

    function getCell(rowid, key) {
        var ret = '';
        if (false === isNullOrEmpty(rowid) && false === isNullOrEmpty(key)) {
            ret = $gridTable.jqGrid('getCell', rowid, key);
        }
        return ret;
    }

    function getSelectedRowId() {
        var rowid = $gridTable.jqGrid('getGridParam', 'selrow');
        return rowid;
    }

    function getSelectedRowsIds() {
        var rowid = $gridTable.jqGrid('getGridParam', 'selarrrow');
        return rowid;
    }

    function getColumn(column, returnType) {
        ///<summary>Gets the contents of a jqGrid column</summary>
        ///<param name="column" type="String">Column name</param>
        ///<param name="returnType" type="Boolean">If false, returns a simple array of values. If true, returns an array [{id: id, value: value},{...}]</param>
        ///<returns type="Array">An array of the columns values</returns>
        var ret = $gridTable.jqGrid('getCol', column, returnType);
        return ret;
    }

    //#region private

    //#region public, priveleged

    function hideColumn(id) {
        $gridTable.jqGrid('hideCol', id);
    }

    function scrollToRow(rowid) {
        ///<summary>Scrolls the grid to the specified rowid</summary>
        ///<param name="rowid" type="String">Optional. jqGrid rowid. If null, selected row is assumed.</param>
        ///<returns type="Void"></returns>
        if (isNullOrEmpty(rowid)) {
            rowid = getSelectedRowId();
        }
        var rowHeight = getGridRowHeight($gridTable) || 23; // Default height
        var index = $gridTable.getInd(rowid);
        $gridTable.closest(".ui-jqgrid-bdiv").scrollTop(rowHeight * (index - 1));
    }

    function getRowIdForVal(value, column) {
        ///<summary>Gets a jqGrid rowid by column name and value.</summary>
        ///<param name="value" type="String">Cell value</param>
        ///<param name="column" type="String">Column name</param>
        ///<returns type="String">jqGrid row id.</returns>
        var pks = getColumn(column, true);
        var rowid = 0;
        each(pks, function(obj) {
            if (contains(obj, 'value') && tryParseString(obj.value) === tryParseString(value)) {
                rowid = obj.id;
            }
        });
        return rowid;
    }

    function getValueForColumn(columnname, rowid) {
        ///<summary>Gets a cell value by column name.</summary>
        ///<param name="columnname" type="String">Grid column name.</param>
        ///<param name="rowid" type="String">Optional. If null, selected row is assumed.</param>
        ///<returns type="String">Value of the cell.</returns>
        if (isNullOrEmpty(rowid)) {
            rowid = getSelectedRowId();
        }
        var ret = getCell(rowid, columnname);
        return ret;
    }

    function setSelection(rowid) {
        ///<summary>Sets the selected row by jqGrid's rowid</summary>
        if (false === isNullOrEmpty(rowid)) {
            $gridTable.setSelection(rowid);
        }
    }

    function changeGridOpts(opts) {
        var currentOpts = $gridTable.data(gridTableId + '_data');
        $.extend(true, currentOpts, opts);
        $parent.empty();
        makeGrid(currentOpts);
    }

    function opGridRows(opts, rowid, onSelect, onEmpty) {
        var ret = false;
        var haveSelectedRows = false;

        var rowids = [];
        if (multiEdit) {
            rowids = getSelectedRowsIds();
        }
        else if (false === isNullOrEmpty(rowid)) {
            rowids.push(rowid);
        } else {
            rowids.push(getSelectedRowId());
        }

        if (rowids.length > 0) {
            haveSelectedRows = true;
            for (var i = 0; i < rowids.length; i++) {
                crawlObject(opts, function(prop, key, parent) {
                    if (false === isFunction(parent[key])) {
                        if (isArray(parent[key])) {
                            rowid = rowids[i];
                            parent[key].push(getValueForColumn(key, rowid));
                        } else {
                            parent[key] = getValueForColumn(key, rowid);
                        }
                    }
                    return false;
                }, false);
            }
        }

        if (haveSelectedRows) {
            if (isFunction(onSelect)) {
                opts.Multi = multiEdit;
                ret = onSelect(opts);
            }
        }
        else if (isFunction(onEmpty)) {
            onEmpty(opts);
        }
        return ret;
    }

    var getAllGridRows = function() {
        return $gridTable.jqGrid('getRowData');
    };
    
    var print = function() {
        
        CswPrint(function ($printElement) {
            var printOpts = { },
                currentOpts = $gridTable.data(gridTableId + '_data'),
                printTableId = makeId({ prefix: gridTableId, ID: 'printTable' }),
                grid, data, i;

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
            printOpts.gridOpts.rowNum = 100000;
            printOpts.gridOpts.rowList = [100000];
            printOpts.gridOpts.add = false;
            printOpts.gridOpts.del = false;
            printOpts.gridOpts.edit = false;
            printOpts.gridOpts.autoencode = true;
            //printOpts.gridOpts.autowidth = true;
            printOpts.gridOpts.width = $(window).width();
            printOpts.gridOpts.altRows = false;
            printOpts.gridOpts.datatype = 'local';
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

            each(printOpts.gridOpts.colModel, function(column) {
                /* This provides text wrapping in cells */
                column.cellattr = function () { return 'style="white-space: normal;"'; };
            });
            
            /* Get a new CswGrid */
            grid = CswGrid(printOpts, $printElement);
            /* Get the data (rows) from the current grid */
            
            
            /* Add the rows to the new grid */
            for(i = 0; i <= data.length ; i += 1) {
                grid.$gridTable.jqGrid('addRowData', i + 1, data[i]);
            }

        });
        
    };

    //#region public, priveleged

    return {
        $gridTable: $gridTable,
        $gridPager: $gridPager,
        $topPager: $topPager || null,
        getAllGridRows: getAllGridRows,
        getGridRowHeight: getGridRowHeight,
        scrollToRow: scrollToRow,
        hideColumn: hideColumn,
        getRowIdForVal: getRowIdForVal,
        setSelection: setSelection,
        getValueForColumn: getValueForColumn,
        changeGridOpts: changeGridOpts,
        opGridRows: opGridRows,
        print: print,
        isMulti: function() { return multiEdit; }
    };
    
    //#endregion public, priveleged
}