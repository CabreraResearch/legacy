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
            }; // makeActionButton()


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
            }; //addActionColumn()


            cswPrivate.makeStore = function(storeId, usePaging) {
                var storeopts = {
                    storeId: storeId,
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
                if (Csw.bool(usePaging)) {
                    storeopts.pageSize = cswPrivate.pageSize;
                    storeopts.proxy.type = 'pagingmemory';
                }

                return Ext.create('Ext.data.Store', storeopts);
            }; // makeStore()

            
            cswPrivate.makeGrid = function (renderTo, store) {
                var gridopts = {
                    title: cswPrivate.title,
                    store: store,
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
                        beforeselect: function () {
                            return Csw.bool(cswPrivate.canSelectRow);
                        },
                        viewready: function () {
                            Csw.tryExec(cswPrivate.onLoad, cswPublic);
                        }
                    }
                };
                if(false === Csw.isNullOrEmpty(renderTo))
                {
                    gridopts.renderTo = renderTo;
                }
                if (Csw.bool(cswPrivate.usePaging)) {
                    gridopts.dockedItems = [{
                        xtype: 'pagingtoolbar',
                        store: cswPrivate.store,
                        dock: 'bottom',
                        displayInfo: true
                    }];
                    
                    var rows = cswPrivate.data.items.length;
                    if(rows === 0){
                        gridopts.height = cswPrivate.calculateHeight(1);
                    } else if( rows <= cswPrivate.pageSize) {
                        gridopts.height = cswPrivate.calculateHeight(rows);
                    } else {
                        gridopts.height = cswPrivate.calculateHeight(cswPrivate.pageSize);
                    }
                }

                return Ext.create('Ext.grid.Panel', gridopts);
            }; // makeGrid()


            cswPrivate.init = function () {
                cswPrivate.addActionColumn();
                cswPrivate.store = cswPrivate.makeStore(cswPrivate.ID + '_store', cswPrivate.usePaging);
                cswPrivate.gridPanel = cswPrivate.makeGrid(cswParent.getId(), cswPrivate.store);

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
            }; // init()


            cswPublic.getCell = function (rowindex, key) {
                ///<summary>Gets the contents of a jqGrid cell by rowid and column key</summary>
                return cswPrivate.store.getAt(rowindex).raw[key];
            };

            cswPublic.getSelectedRowId = function () {
                return cswPrivate.store.indexOf(cswPrivate.gridPanel.getSelectionModel().getSelection()[0]);
            };

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

            cswPublic.setSelection = function (rowindex) {
                ///<summary>Sets the selected row by index</summary>
                if (rowindex > -1) {
                    cswPrivate.gridPanel.getSelectionModel().select(rowindex);
                }
            };

            cswPublic.getAllGridRows = function () {
                return cswPrivate.store.data;
            };

            cswPublic.print = function (onSuccess) {
                // turn paging off
                var printStore = cswPrivate.makeStore(cswPrivate.ID + '_printstore', false);
                var printGrid = cswPrivate.makeGrid('', printStore);

                Ext.ux.grid.Printer.stylesheetPath = '/NbtWebApp/js/thirdparty/extJS-4.1.0/ux/grid/gridPrinterCss/print.css';
                Ext.ux.grid.Printer.print(printGrid);
            };

            cswPublic.isMulti = function () {
                return cswPrivate.multiEdit;
            };

            cswPrivate.calculateHeight = function(rows) {
                return 25 + // title bar
                       23 + // grid header
                       (rows * 28) + // rows
                       14 + // horizontal scrollbar
                       27;  // grid footer
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
                                if (false === Csw.isNullOrEmpty(result.grid.truncated)) {
                                    cswPrivate.truncated = result.grid.truncated;
                                }
                                cswPrivate.title = result.grid.title;
                                cswPrivate.fields = result.grid.fields;
                                cswPrivate.columns = result.grid.columns;
                                cswPrivate.data = result.grid.data;
                                cswPrivate.init();

                            } // if(false === Csw.isNullOrEmpty(data.griddata)) {
                        } // success
                    }); // ajax.post()
                } else {
                    cswPrivate.init();
                }
            } ());

            return cswPublic;
        });

} ());
