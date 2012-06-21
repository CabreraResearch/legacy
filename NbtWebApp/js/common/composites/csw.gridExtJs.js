/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';
    Csw.composites.grid = Csw.composites.grid ||
        Csw.composites.register('grid', function (cswParent, options) {

            var cswPrivate = {
                ID: 'extjsGrid',
                storeId: '',
                title: 'Untitled Grid',
                truncated: false,
                //readonly: false,
                stateId: '',
                usePaging: true,

                ajax: {
                    urlMethod: '',
                    data: {}
                },

                showActionColumn: true,
                showView: true,
                showLock: true,
                showEdit: true,
                showDelete: true,

                canSelectRow: false,

                onLoad: null,   // function()
                onEdit: null,   // function(row)
                onDelete: null, // function(row)
                onSelect: null, // function(row)
                onDeselect: null, // function(row)

                height: '',  // overridden by webservice if paging is on
                width: '100%',

                fields: [],   // [ { name: 'col1', type: 'string' }, ... ]
                columns: [],  // [ { header: 'Col1', dataIndex: 'col1', ... }, ... ]
                data: {},     // { items: [ { col1: val, col2: val ... }, ... ]
                pageSize: ''  // overridden by webservice
            };
            var cswPublic = {};

            cswPrivate.makeActionButton = function (cellId, buttonName, iconType, clickFunc, record, rowIndex, colIndex) {
                // Possible race condition - have to make the button after the cell is added, but it isn't added yet
                setTimeout(function () {
                    var cell = Csw.literals.factory($('#' + cellId));
                    var iconopts = {
                        ID: cswPrivate.ID + '_' + cellId + '_' + buttonName,
                        hovertext: buttonName,
                        iconType: iconType,
                        state: Csw.enums.iconState.normal,
                        isButton: false,
                        size: 18
                    };
                    if (false === Csw.isNullOrEmpty(clickFunc)) {
                        iconopts.isButton = true;
                        iconopts.onClick = function () {
                            Csw.tryExec(clickFunc, record.data);
                        };
                    }
                    cell.icon(iconopts);
                }, 50);
            } // makeActionButton()

            cswPrivate.addActionColumn = function () {
                if (cswPrivate.showActionColumn) {
                    var newfld = { name: 'action' };
                    var newcol = {
                        header: 'Action',
                        dataIndex: 'action',
                        flex: false,
                        width: 60,
                        resizable: false,
                        xtype: 'actioncolumn',
                        renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                            var cell1Id = cswPrivate.ID + '_action_' + rowIndex + '_' + colIndex + '_1';
                            var cell2Id = cswPrivate.ID + '_action_' + rowIndex + '_' + colIndex + '_2';

                            var ret = '<table cellpadding="0"><tr>';
                            ret += '<td id="' + cell1Id + '" style="width: 26px;"/>';
                            ret += '<td id="' + cell2Id + '" style="width: 26px;"/>';
                            ret += '</tr></table>';

                            var canedit = Csw.bool(cswPrivate.showEdit) && Csw.bool(record.data.canedit, true);
                            var canview = Csw.bool(cswPrivate.showView) && Csw.bool(record.data.canview, true);
                            var candelete = Csw.bool(cswPrivate.showDelete) && Csw.bool(record.data.candelete, true);
                            var islocked = Csw.bool(cswPrivate.showLock) && Csw.bool(record.data.islocked, false);

                            // only show one of edit/view/lock
                            if (islocked) {
                                cswPrivate.makeActionButton(cell1Id, 'Locked', Csw.enums.iconType.lock, null, record, rowIndex, colIndex);
                            }
                            else if (canedit) {
                                cswPrivate.makeActionButton(cell1Id, 'Edit', Csw.enums.iconType.pencil, cswPrivate.onEdit, record, rowIndex, colIndex);
                            }
                            else if (canview) {
                                cswPrivate.makeActionButton(cell1Id, 'View', Csw.enums.iconType.magglass, cswPrivate.onEdit, record, rowIndex, colIndex);
                            }

                            if (candelete) {
                                cswPrivate.makeActionButton(cell2Id, 'Delete', Csw.enums.iconType.trash, cswPrivate.onDelete, record, rowIndex, colIndex);
                            }
                            return ret;
                        } // renderer()
                    };
                    cswPrivate.fields.splice(0, 0, newfld);
                    cswPrivate.columns.splice(0, 0, newcol);
                } // if (cswPrivate.showActionColumn)
            } //addActionColumn()

            cswPrivate.initGrid = function () {

                cswPrivate.addActionColumn();
                cswPrivate.storeId = cswPrivate.ID + 'store';

                var storeopts = {
                    storeId: cswPrivate.storeId,
                    fields: cswPrivate.fields,
                    data: cswPrivate.data,
                    proxy: {
                        type: 'memory',
                        reader: {
                            type: 'json',
                            root: 'items'
                        }
                    }
                };
                if (Csw.bool(cswPrivate.usePaging)) {
                    storeopts.pageSize = cswPrivate.pageSize;
                    storeopts.proxy.type = 'pagingmemory';
                }

                cswPrivate.store = Ext.create('Ext.data.Store', storeopts);

                var gridopts = {
                    title: cswPrivate.title,
                    store: cswPrivate.store,
                    columns: cswPrivate.columns,
                    height: cswPrivate.height,
                    width: cswPrivate.width,
                    stateful: true,
                    stateId: cswPrivate.stateId,
                    forceFit: true,               // expand columns to fill width
                    viewConfig: {
                        deferEmptyText: false,
                        emptyText: 'No Results'
                    },
                    listeners: {
                        beforeselect: function() {
                            return Csw.bool(cswPrivate.canSelectRow);
                        },
                        viewready: function () {
                            Csw.tryExec(cswPrivate.onLoad, cswPublic);
                        }
                    },
                    renderTo: cswParent.getId()
                };
                if (Csw.bool(cswPrivate.usePaging)) {
                    gridopts.dockedItems = [{
                        xtype: 'pagingtoolbar',
                        store: cswPrivate.store,
                        dock: 'bottom',
                        displayInfo: true
                    }];
                }

                cswPrivate.gridPanel = Ext.create('Ext.grid.Panel', gridopts);
                cswPrivate.gridPanel.on({
                    select: function (rowModel, record, index, eOpts) {
                        Csw.tryExec(cswPrivate.onSelect, record.data);
                    },
                    deselect: function (rowModel, record, index, eOpts) {
                        Csw.tryExec(cswPrivate.onDeselect, record.data);
                    }
                });

                if (Csw.bool(cswPrivate.truncated)) {
                    cswParent.span({ cssclass: 'truncated', text: 'Results Truncated' });
                }
            }; // initGrid()


            // Old public grid interfaces:

            cswPublic.getCell = function (rowindex, key) {
                ///<summary>Gets the contents of a jqGrid cell by rowid and column key</summary>
                return cswPrivate.store.getAt(rowindex).raw[key];
            };

            //            cswPublic.getDataIds = function () {
            //                //                ///<summary>Gets the contents of a jqGrid column</summary>
            //                //                return cswPublic.gridTable.$.jqGrid('getDataIDs');
            //            };

            cswPublic.getSelectedRowId = function () {
                return cswPrivate.store.indexOf(cswPrivate.gridPanel.getSelectionModel().getSelection()[0]);
            };

            //            cswPublic.hideColumn = function (colName) {
            //                ///<summary>Hides a column by name</summary>
            //                //                cswPublic.gridTable.$.jqGrid('hideCol', colName);
            //            };

            cswPublic.scrollToRow = function (rowindex) {
                ///<summary>Scrolls the grid to the specified index</summary>
                ///<param name="rowid" type="String">Optional. jqGrid rowid. If null, selected row is assumed.</param>
                ///<returns type="Void"></returns>
                if (Csw.isNullOrEmpty(rowindex)) {
                    rowindex = cswPublic.getSelectedRowId();
                }
                cswPrivate.gridPanel.getView().focusRow(rowindex);
            };

            cswPublic.getRowIdForVal = function (column, value) {
                ///<summary>Gets a row index by column name and value.</summary>
                ///<param name="value" type="String">Cell value</param>
                ///<param name="column" type="String">Column name</param>
                ///<returns type="String">row index.</returns>
                return cswPrivate.store.findExact(column, value);
            };

            cswPublic.getValueForColumn = function (columnname, rowindex) {
                ///<summary>Gets a cell value by column name.</summary>
                ///<param name="columnname" type="String">Grid column name.</param>
                ///<param name="rowid" type="String">Optional. If null, selected row is assumed.</param>
                ///<returns type="String">Value of the cell.</returns>
                if (Csw.isNullOrEmpty(rowindex)) {
                    rowindex = cswPublic.getSelectedRowId();
                }
                var ret = cswPublic.getCell(rowindex, columnname);
                return ret;
            };

            //            cswPublic.setRowData = function (rowId, columnName, columnData) {
            //                //                ///<summary>Update a cell with new content.</summary>
            //                //                var cellData = {};
            //                //                cellData[columnName] = columnData;
            //                //                return cswPublic.gridTable.$.jqGrid('setRowData', rowId, cellData);
            //            };

            cswPublic.setSelection = function (rowindex) {
                ///<summary>Sets the selected row by index</summary>
                if (rowindex > -1) {
                    cswPrivate.gridPanel.getSelectionModel().select(rowindex);
                }
            };

            //            cswPublic.resetSelection = function () {
            //                ///<summary>Deselects all grid rows.</summary>
            //                //cswPublic.gridTable.$.jqGrid('resetSelection');
            //            };

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
                                if (Csw.bool(cswPrivate.usePaging)) {
                                    cswPrivate.height = 25 + // title bar
                                                        23 + // grid header
                                                        (cswPrivate.pageSize * 26) + // rows
                                                        14 + // horizontal scrollbar
                                                        27;  // grid footer
                                }
                                if (false === Csw.isNullOrEmpty(result.grid.truncated)) {
                                    cswPrivate.truncated = result.grid.truncated;
                                }
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

            return cswPublic;
        });

} ());
