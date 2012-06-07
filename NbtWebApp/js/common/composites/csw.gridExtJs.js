/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';
    Csw.composites.grid = Csw.composites.grid ||
        Csw.composites.register('grid', function (cswParent, options) {

            var cswPrivate = {
                ID: 'extjsGrid',
                title: 'Untitled Grid',
                readonly: false,
                stateId: '',

                ajax: {
                    urlMethod: '',
                    data: {}
                    },

                showActionColumn: true,
                onEdit: null,   // function(row)
                onDelete: null, // function(row)

                maxHeight: '',
                height: 300,  // overridden by webservice
                width: 600,

                urlMethod: 'runGrid',
                storeId: '',

                fields: [],   // [ { name: 'col1', type: 'string' }, ... ]
                columns: [],  // [ { header: 'Col1', dataIndex: 'col1', ... }, ... ]
                data: {},     // { items: [ { col1: val, col2: val ... }, ... ]
                pageSize: ''  // overridden by webservice
            };
            var cswPublic = {};

            cswPrivate.addActionColumn = function () {
                if (cswPrivate.showActionColumn) {
                    var newfld = { name: 'action' };
                    var newcol = {
                        header: 'Action',
                        dataIndex: 'action',
                        flex: false,
                        width: 60,
                        xtype: 'actioncolumn',
                        items: [{
                            icon: 'Images/newicons/18/pencil.png',
                            tooltip: 'Edit',
                            handler: function (grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                Csw.tryExec(cswPrivate.onEdit, rec.data);
                            }
                        },
                        {
                            icon: 'Images/blank.png'
                        },
                        {
                            icon: 'Images/newicons/18/trash.png',
                            tooltip: 'Delete',
                            handler: function (grid, rowIndex, colIndex) {
                                var rec = grid.getStore().getAt(rowIndex);
                                Csw.tryExec(cswPrivate.onDelete, rec.data);
                            }
                        }]
                    };
                    cswPrivate.fields.splice(0,0,newfld);
                    cswPrivate.columns.splice(0,0,newcol);
                } // if (cswPrivate.showActionColumn)
            } //addActionColumn()

            cswPrivate.initGrid = function () {

                cswPrivate. addActionColumn();

                cswPrivate.storeId = cswPrivate.ID + 'store';
                if (false === Csw.isNullOrEmpty(cswPrivate.maxHeight) && cswPrivate.height > cswPrivate.maxHeight) {
                    cswPrivate.height = cswPrivate.maxHeight;
                }
                var gridStore = Ext.create('Ext.data.Store', {
                    storeId: cswPrivate.storeId,
                    fields: cswPrivate.fields,
                    data: cswPrivate.data,
                    pageSize: cswPrivate.pageSize,
                    proxy: {
                        type: 'pagingmemory',
                        reader: {
                            type: 'json',
                            root: 'items'
                        }
                    }
                });

                gridStore.loadPage(1);

                // Apparently there's a race condition between creating the store and using it?
                setTimeout(function () {

                    Ext.create('Ext.grid.Panel', {
                        title: cswPrivate.title,
                        store: gridStore,
                        columns: cswPrivate.columns,
                        height: cswPrivate.height,
                        width: cswPrivate.width,
                        stateful: true,
                        stateId: cswPrivate.stateId,
                        viewConfig: {
                            forceFit: true,
                            emptyText: 'No Results'
                        },
                        dockedItems: [{
                            xtype: 'pagingtoolbar',
                            store: gridStore,
                            dock: 'bottom',
                            displayInfo: true
                        }],
                        renderTo: cswParent.getId()
                    });

                }, 50);

            }; // initGrid()

            //constructor
            (function () {
                if (options) $.extend(cswPrivate, options);

                if (Csw.isNullOrEmpty(cswPrivate.data)) {
                    Csw.ajax.post({
                        url: cswPrivate.ajax.url,
                        urlMethod: cswPrivate.ajax.urlMethod,
                        data: cswPrivate.ajax.data,
                        success: function (result) {
                            if (false === Csw.isNullOrEmpty(result.grid)) {
                                cswPrivate.pageSize = Csw.number(result.grid.pageSize);
                                cswPrivate.height = 25 + // title bar
                                                    23 + // grid header
                                                    (cswPrivate.pageSize * 24.5) + // rows
                                                    14 + // horizontal scrollbar
                                                    27;  // grid footer
                                cswPrivate.title = result.grid.title;
                                cswPrivate.fields = result.grid.fields;
                                cswPrivate.columns = result.grid.columns;
                                cswPrivate.data = result.grid.data;
                                cswPrivate.initGrid();

                            } // if(false === Csw.isNullOrEmpty(data.griddata)) {
                        } // success
                    }); // ajax.post()
                } else {
                    cswPrivate.initGrid();
                }
            } ());


            // Old public grid interfaces:

            cswPublic.getCell = function (rowid, key) {
                //                ///<summary>Gets the contents of a jqGrid cell by rowid and column key</summary>
                //                var ret = '';
                //                if (false === Csw.isNullOrEmpty(rowid) && false === Csw.isNullOrEmpty(key)) {
                //                    ret = cswPublic.gridTable.$.jqGrid('getCell', rowid, key);
                //                }
                //                return ret;
            };

            cswPublic.getDataIds = function () {
                //                ///<summary>Gets the contents of a jqGrid column</summary>
                //                return cswPublic.gridTable.$.jqGrid('getDataIDs');
            };

            cswPublic.getSelectedRowId = function () {
                //                var rowid = cswPublic.gridTable.$.jqGrid('getGridParam', 'selrow');
                //                return rowid;
            };

            cswPublic.hideColumn = function (colName) {
                ///<summary>Hides a column by name</summary>
                //                cswPublic.gridTable.$.jqGrid('hideCol', colName);
            };

            cswPublic.scrollToRow = function (rowid) {
                //                ///<summary>Scrolls the grid to the specified rowid</summary>
                //                ///<param name="rowid" type="String">Optional. jqGrid rowid. If null, selected row is assumed.</param>
                //                ///<returns type="Void"></returns>
                //                if (Csw.isNullOrEmpty(rowid)) {
                //                    rowid = cswPublic.getSelectedRowId();
                //                }
                //                var rowHeight = cswPublic.getGridRowHeight() || 23; // Default height
                //                var index = cswPublic.gridTable.$.getInd(rowid);
                //                cswPublic.gridTable.$.closest(".ui-jqgrid-bdiv").scrollTop(rowHeight * (index - 1));
            };

            cswPublic.getRowIdForVal = function (value, column) {
                //                ///<summary>Gets a jqGrid rowid by column name and value.</summary>
                //                ///<param name="value" type="String">Cell value</param>
                //                ///<param name="column" type="String">Column name</param>
                //                ///<returns type="String">jqGrid row id.</returns>
                //                var pks = cswPrivate.getColumn(column, true);
                //                var rowid = 0;
                //                Csw.each(pks, function (obj) {
                //                    if (Csw.contains(obj, 'value') && Csw.string(obj.value) === Csw.string(value)) {
                //                        rowid = obj.id;
                //                    }
                //                });
                //                return rowid;
            };

            cswPublic.getValueForColumn = function (columnname, rowid) {
                //                ///<summary>Gets a cell value by column name.</summary>
                //                ///<param name="columnname" type="String">Grid column name.</param>
                //                ///<param name="rowid" type="String">Optional. If null, selected row is assumed.</param>
                //                ///<returns type="String">Value of the cell.</returns>
                //                if (Csw.isNullOrEmpty(rowid)) {
                //                    rowid = cswPublic.getSelectedRowId();
                //                }
                //                var ret = cswPublic.getCell(rowid, columnname);
                //                return ret;
            };

            cswPublic.setRowData = function (rowId, columnName, columnData) {
                //                ///<summary>Update a cell with new content.</summary>
                //                var cellData = {};
                //                cellData[columnName] = columnData;
                //                return cswPublic.gridTable.$.jqGrid('setRowData', rowId, cellData);
            };

            cswPublic.setSelection = function (rowid) {
                //                ///<summary>Sets the selected row by jqGrid's rowid</summary>
                //                if (Csw.isNullOrEmpty(rowid)) {
                //                    rowid = cswPublic.getSelectedRowId();
                //                }
                //                if (false === Csw.isNullOrEmpty(rowid)) {
                //                    cswPublic.gridTable.$.jqGrid('setSelection', rowid);
                //                }
            };

            cswPublic.resetSelection = function () {
                ///<summary>Deselects all grid rows.</summary>
                //cswPublic.gridTable.$.jqGrid('resetSelection');
            };

            cswPublic.changeGridOpts = function (opts, toggleColumns) {
                //                var delBtn, editBtn;
                //                $.extend(true, cswPrivate, opts);
                //                cswPrivate.makeGrid(cswPrivate);

                //                Csw.each(toggleColumns, function (val) {
                //                    if (Csw.contains(cswPrivate.gridOpts.colNames, val)) {
                //                        if (cswPublic.isMulti()) {
                //                            cswPublic.gridTable.$.jqGrid('hideCol', val);
                //                        }
                //                    }
                //                });
                //                if (false === cswPublic.isMulti()) {
                //                    if (false === cswPrivate.canEdit) {
                //                        editBtn = cswPublic.gridPager.find('#edit_' + cswPrivate.gridTableId);
                //                        if (Csw.contains(editBtn, 'remove')) {
                //                            editBtn.remove();
                //                        }
                //                    }
                //                    if (false === cswPrivate.canDelete) {
                //                        delBtn = cswPublic.gridPager.find('#del_' + cswPrivate.gridTableId).remove();
                //                        if (Csw.contains(delBtn, 'remove')) {
                //                            delBtn.remove();
                //                        }
                //                    }
                //                }
                //                cswPublic.resizeWithParent(cswPrivate.resizeWithParentElement);
            };

            cswPublic.opGridRows = function (opts, rowid, onSelect, onEmpty) {
                //                var ret = false;
                //                var haveSelectedRows = false,
                //                    i;

                //                var rowids = [];

                //                function onEachGridRow(prop, key, parent) {
                //                    if (false === Csw.isFunction(parent[key])) {
                //                        if (Csw.isArray(parent[key])) {
                //                            rowid = rowids[i];
                //                            parent[key].push(cswPublic.getValueForColumn(key, rowid));
                //                        } else {
                //                            parent[key] = cswPublic.getValueForColumn(key, rowid);
                //                        }
                //                    }
                //                    return false;
                //                }

                //                if (cswPrivate.multiEdit) {
                //                    rowids = cswPrivate.getSelectedRowsIds();
                //                } else if (false === Csw.isNullOrEmpty(rowid)) {
                //                    rowids.push(rowid);
                //                } else {
                //                    rowids.push(cswPublic.getSelectedRowId());
                //                }

                //                if (rowids.length > 0) {
                //                    haveSelectedRows = true;
                //                    for (i = 0; i < rowids.length; i += 1) {
                //                        Csw.crawlObject(opts, onEachGridRow, false);
                //                    }
                //                }

                //                if (haveSelectedRows) {
                //                    if (Csw.isFunction(onSelect)) {
                //                        opts.Multi = cswPrivate.multiEdit;
                //                        ret = onSelect(opts);
                //                    }
                //                } else if (Csw.isFunction(onEmpty)) {
                //                    onEmpty(opts);
                //                }
                //                return ret;
            };

            cswPublic.getAllGridRows = function () {
                //                return cswPublic.gridTable.$.jqGrid('getRowData');
            };

            cswPublic.print = function (onSuccess) {

                //                try {

                //                    var outerDiv = cswParent.div();
                //                    var newDiv = outerDiv.div({ width: '800px' });

                //                    var printOpts = {},
                //                        printTableId = Csw.makeId(cswPrivate.gridTableId, 'printTable'),
                //                        newGrid, data, i;

                //                    var addRowsToGrid = function (rowData) {
                //                        if (rowData) {
                //                            /* Add the rows to the new newGrid */
                //                            for (i = 0; i <= rowData.length; i += 1) {
                //                                newGrid.gridTable.$.jqGrid('addRowData', i + 1, rowData[i]);
                //                            }
                //                        }
                //                    };

                //                    /* Case 26020 */
                //                    $.extend(true, printOpts, cswPrivate);

                //                    /* Nuke anything that might be holding onto a reference */
                //                    Csw.each(printOpts, function (thisObj, name) {
                //                        if (Csw.isFunction(thisObj) || Csw.isJQuery(thisObj)) {
                //                            delete printOpts[name];
                //                        }
                //                    });

                //                    printOpts.ID = printTableId;

                //                    /* 
                //                    Nuke any existing options with vanilla defaults.
                //                    Since jqGrid 3.6, there hasn't been an 'All' rowNum option. Just use a really high number.
                //                    */
                //                    delete printOpts.gridOpts.canEdit;
                //                    delete printOpts.gridOpts.canDelete;
                //                    delete printOpts.canEdit;
                //                    delete printOpts.canDelete;

                //                    printOpts.gridPagerId += '_print';
                //                    printOpts.gridTableId += '_print';
                //                    printOpts.gridOpts.rowNum = 100000;
                //                    printOpts.gridOpts.rowList = [100000];
                //                    printOpts.gridOpts.add = false;
                //                    printOpts.gridOpts.del = false;
                //                    printOpts.gridOpts.edit = false;
                //                    printOpts.gridOpts.autoencode = true;
                //                    //printOpts.gridOpts.autowidth = true;
                //                    printOpts.gridOpts.width = 800;
                //                    printOpts.gridOpts.altRows = false;
                //                    printOpts.gridOpts.datatype = 'local';
                //                    delete printOpts.gridOpts.url;
                //                    printOpts.gridOpts.emptyrecords = 'No Results';
                //                    printOpts.gridOpts.height = 'auto';
                //                    printOpts.gridOpts.multiselect = false;
                //                    printOpts.gridOpts.toppager = false;
                //                    //printOpts.gridOpts.forceFit = true;
                //                    //printOpts.gridOpts.shrinkToFit = true;

                //                    /*
                //                    jqGrid cannot seem to handle the communication of the data property between window objects.
                //                    --Just delete it and rebuild instead.
                //                    DON'T just delete it. You're deleting the reference to the current grid rows. Bad idea.
                //                    */
                //                    data = cswPrivate.gridOpts.data;

                //                    Csw.each(printOpts.gridOpts.colModel, function (column) {
                //                        /* This provides text wrapping in cells */
                //                        column.cellattr = function () {
                //                            return 'style="white-space: normal;"';
                //                        };
                //                    });
                //                    Csw.tryExec(onSuccess, newDiv);
                //                    /* Get a new Csw.newGrid */
                //                    newGrid = newDiv.grid(printOpts);
                //                    newGrid.gridTable.$.jqGrid('hideCol', 'Action');

                //                    if (Csw.isNullOrEmpty(data) && false === Csw.isNullOrEmpty(printOpts.printUrl)) {
                //                        Csw.ajax.get({
                //                            url: printOpts.printUrl,
                //                            success: function (rows) {
                //                                addRowsToGrid(rows.rows);
                //                            }
                //                        });
                //                    } 
                //                    
                //                    Csw.newWindow(outerDiv.$.html());
                //                    outerDiv.remove();

                //                } catch (e) {
                //                    Csw.log(e);
                //                }

            };

            // Row scrolling adapted from 
            // http://stackoverflow.com/questions/2549466/is-there-a-way-to-make-jqgrid-scroll-to-the-bottom-when-a-new-row-is-added/2549654#2549654
            cswPublic.getGridRowHeight = function () {

                //                var height = null; // Default
                //                try {
                //                    height = cswPublic.gridTable.$.find('tbody').find('tr:first').outerHeight();
                //                } catch (e) {
                //                    //catch and just suppress error
                //                }
                //                return height;
            };

            cswPublic.isMulti = function () {
                //                return cswPrivate.multiEdit;
            };

            cswPublic.setWidth = function (width) {
                //                cswPublic.gridTable.$.jqGrid('setGridWidth', width);
            };

            cswPublic.resizeWithParent = function () {
                //                var i = 0;
                //                function handleRestoreDownRecursive($elem) {
                //                    i += 1;
                //                    if ($elem.width() !== null &&
                //                        $elem.parent().width() !== null) {
                //                        if ($elem.parent().width() < $elem.width()) {
                //                            element = $elem.parent();
                //                        } else if (i <= 15) {
                //                            handleRestoreDownRecursive($elem.parent());
                //                        }
                //                    }
                //                }
                //                var element = cswPrivate.resizeWithParentElement || cswParent.$;
                //                handleRestoreDownRecursive(element);
                //                var width = element.width() - 50;
                //                cswPublic.setWidth(width);
            };

            return cswPublic;
        });

} ());
