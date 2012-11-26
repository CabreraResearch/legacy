/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />
/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.composites.grid = Csw.composites.grid ||
        Csw.composites.register('grid', function (cswParent, options) {

            var cswPrivate = {
                name: 'extjsGrid',
                //storeId: '',
                title: 'Untitled Grid',
                truncated: false,
                //stateId: '',
                usePaging: true,
                forceFit: false,   // expand all columns to fill width (makes column resizing weird)

                ajax: {
                    urlMethod: '',
                    data: {}
                },

                showCheckboxes: false,
                showActionColumn: true,
                showView: true,
                showLock: true,
                showEdit: true,
                showDelete: true,

                canSelectRow: false,

                onLoad: null,   // function(grid, ajaxResult)
                onEdit: null,   // function(rows)
                onDelete: null, // function(rows)
                onSelect: null, // function(row)
                onDeselect: null, // function(row)

                height: '',  // overridden by webservice if paging is on
                //width: '100%',
                width: '',

                fields: [],   // [ { name: 'col1', type: 'string' }, ... ]
                columns: [],  // [ { header: 'Col1', dataIndex: 'col1', ... }, ... ]
                data: {},     // { items: [ { col1: val, col2: val ... }, ... ]
                pageSize: '',  // overridden by webservice

                actionDataIndex: 'action',
                
                topToolbar: []
            };
            var cswPublic = {};

            window.Ext.require('Ext.ux.grid.FiltersFeature');

            cswPrivate.makeActionButton = function (cellId, buttonName, iconType, clickFunc, record, rowIndex, colIndex) {
                // Possible race condition - have to make the button after the cell is added, but it isn't added yet
                Csw.defer(function () {
                    var cell = Csw.domNode({ ID: cellId });
                    cell.empty();
                    var iconopts = {
                        name: cswPrivate.name + cellId + buttonName,
                        hovertext: buttonName,
                        iconType: iconType,
                        state: Csw.enums.iconState.normal,
                        isButton: false,
                        size: 18
                    };
                    if (false === Csw.isNullOrEmpty(clickFunc)) {
                        iconopts.isButton = true;
                        iconopts.onClick = function () {
                            Csw.tryExec(clickFunc, [record.data]);
                        };
                    }
                    cell.icon(iconopts);
                }, 50);
            }; // makeActionButton()


            cswPrivate.makeStore = Csw.method(function (storeId, usePaging) {
                var fields = Csw.extend([], cswPrivate.fields);

                var storeopts = {
                    storeId: storeId,
                    fields: fields,
                    data: cswPrivate.data,
                    autoLoad: true,
                    proxy: {
                        type: 'memory',
                        reader: {
                            type: 'json',
                            root: 'items'
                        }
                    }
                };
                if (cswPrivate.showActionColumn && false === cswPrivate.showCheckboxes) {
                    var newfld = { name: cswPrivate.actionDataIndex };
                    storeopts.fields.splice(0, 0, newfld);
                }
                if (Csw.bool(usePaging)) {
                    storeopts.pageSize = cswPrivate.pageSize;
                    storeopts.proxy.type = 'pagingmemory';
                }

                return window.Ext.create('Ext.data.Store', storeopts);
            }); // makeStore()


            cswPrivate.makeGrid = Csw.method(function (renderTo, store) {
                var columns = Csw.extend([], cswPrivate.columns);

                Csw.each(columns, function (val) {
                    val.filterable = true;
                });                

                var gridopts = {
                    id: cswPrivate.ID + 'grid',
                    itemId: cswPrivate.name,
                    title: cswPrivate.title,
                    store: store,
                    renderTo: renderTo,
                    columns: columns,
                    height: cswPrivate.height,
                    width: cswPrivate.width,
                    minWidth: 400,
                    resizable: true,               // client side grid resizing
                    stateful: true,
                    stateId: cswPrivate.name,
                    forceFit: cswPrivate.forceFit,
                    viewConfig: {
                        deferEmptyText: false,
                        emptyText: 'No Results',
                        getRowClass: function (record, index) {
                            var ret = '';
                            if (record && record.raw) {
                                var disabled = Csw.bool(record.raw.isdisabled);
                                if (disabled) {
                                    ret = 'disabled';
                                }
                            }
                            return ret;
                        }
                    },
                    listeners: {
                        viewready: function () {
                            Csw.tryExec(cswPrivate.onLoad, cswPublic, cswPrivate.ajaxResult);
                        }
                    },
                    dockedItems: [],
                    features: [{
                        ftype: 'filters',
                        autoReload: false,
                        encode: false,
                        local: true
                    }]
                };

                // Action column
                if (cswPrivate.showActionColumn &&
                    false === cswPrivate.showCheckboxes) {

                    var newcol = {
                        header: 'Action',
                        dataIndex: cswPrivate.actionDataIndex,
                        width: 60,
                        flex: false,
                        resizable: false,
                        xtype: 'actioncolumn',
                        renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {                            
                            var cell1Id = cswPrivate.name + 'action' + rowIndex + colIndex + '1';
                            var cell2Id = cswPrivate.name + 'action' + rowIndex + colIndex + '2';
                            //$('#gridActionColumn' + cell1Id).remove();
                            var ret = '<table id="gridActionColumn' + cell1Id + '" cellpadding="0"><tr>';
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
                            } else if (canedit) {
                                cswPrivate.makeActionButton(cell1Id, 'Edit', Csw.enums.iconType.pencil, cswPrivate.onEdit, record, rowIndex, colIndex);
                            } else if (canview) {
                                cswPrivate.makeActionButton(cell1Id, 'View', Csw.enums.iconType.magglass, cswPrivate.onEdit, record, rowIndex, colIndex);
                            }

                            if (candelete) {
                                cswPrivate.makeActionButton(cell2Id, 'Delete', Csw.enums.iconType.trash, cswPrivate.onDelete, record, rowIndex, colIndex);
                            }

                            return ret;
                        } // renderer()
                    }; // newcol
                    gridopts.columns.splice(0, 0, newcol);
                } // if(cswPrivate.showActionColumn && false === cswPrivate.showCheckboxes) {

                //Render buttons in a callback
                if (false === cswPrivate.showCheckboxes &&
                    cswPrivate.data.buttons &&
                    cswPrivate.data.buttons.length > 0) {

                    var colNames = Csw.delimitedString('', { spaceToDelimiter: false });
                    Csw.each(cswPrivate.data.buttons, function (val, key) {
                        //Get the column names, delimitedString will handle dupes for us automatically
                        colNames.add(val.selectedtext);
                    });

                    var cols = cswPrivate.columns.filter(function (col) {
                        return colNames.contains(col.header);
                    });
                    var i = 0;
                    Csw.each(cols, function (colObj, key) {
                        colObj.renderer = function (value, metaData, record, rowIndex, colIndex, store, view) {
                            i += 1;
                            var id = cswPrivate.ID + 'nodebutton' + i;
                            var thisBtn = cswPrivate.data.buttons.filter(function (btn) {
                                return btn.index === colObj.dataIndex && btn.rowno === rowIndex;
                            });
                            if (thisBtn.length === 1) {
                                Csw.defer(function() {
                                    var div = Csw.domNode({ ID: id });
                                    div.nodeButton({
                                        value: colObj.header,
                                        size: 'small',
                                        propId: thisBtn[0].propattr
                                    });
                                },100);
                            }
                            return '<div id="' + id + '"></div>';

                        };
                    });

                }

                // Selection mode
                if (cswPrivate.showCheckboxes) {
                    gridopts.selType = 'checkboxmodel';
                    gridopts.selModel = { mode: 'Simple' };
                    gridopts.listeners.selectionchange = function (t, selected, eOpts) {
                        if(cswPrivate.editAllButton && cswPrivate.deleteAllButton) {
                            if (Csw.isNullOrEmpty(selected) || selected.length === 0) {
                                cswPrivate.editAllButton.disable();
                                cswPrivate.deleteAllButton.disable();
                            } else {
                                cswPrivate.editAllButton.enable();
                                cswPrivate.deleteAllButton.enable();
                            }
                        }
                    };
                } else {
                    gridopts.selType = 'rowmodel';
                    gridopts.listeners.beforeselect = function () {
                        return Csw.bool(cswPrivate.canSelectRow);
                    };
                }

                // Paging
                if (Csw.bool(cswPrivate.usePaging)) {
                    gridopts.dockedItems.push({
                        xtype: 'pagingtoolbar',
                        store: cswPrivate.store,
                        dock: 'bottom',
                        displayInfo: true,
                        itemId: 'bottomtoolbar',
                        doRefresh: cswPublic.reload
                    });

                    var rows = cswPrivate.data.items.length;
                    if (false === Csw.isNumber(cswPrivate.height) || cswPrivate.height <= 0 || Csw.isNullOrEmpty(cswPrivate.height)) {
                        if (rows === 0) {
                            gridopts.height = cswPrivate.calculateHeight(1);
                        } else if (rows <= cswPrivate.pageSize) {
                            gridopts.height = cswPrivate.calculateHeight(rows);
                        } else {
                            gridopts.height = cswPrivate.calculateHeight(cswPrivate.pageSize);
                        }
                    }
                }

                // Multi-Edit
                if (cswPrivate.showCheckboxes &&
                    cswPrivate.showActionColumn) {

                    cswPrivate.editAllButton = window.Ext.create('Ext.button.Button', {
                        id: cswPrivate.ID + 'edit',
                        xtype: 'button',
                        text: 'Edit Selected',
                        icon: 'Images/newicons/16/pencil.png',
                        disabled: true,
                        handler: function () {
                            var rows = [];
                            Csw.each(cswPublic.extGrid.getSelectionModel().getSelection(), function (selectedRow) {
                                rows.push(selectedRow.raw);
                            });
                            cswPrivate.onEdit(rows);
                        } // edit handler
                    });
                    cswPrivate.topToolbar.push(cswPrivate.editAllButton);

                    cswPrivate.deleteAllButton = window.Ext.create('Ext.button.Button', {
                        id: cswPrivate.ID + 'delete',
                        xtype: 'button',
                        text: 'Delete Selected',
                        icon: 'Images/newicons/16/trash.png',
                        disabled: true,
                        handler: function () {
                            var rows = [];
                            Csw.each(cswPublic.extGrid.getSelectionModel().getSelection(), function (selectedRow) {
                                rows.push(selectedRow.raw);
                            });
                            cswPrivate.onDelete(rows);
                        } // delete handler
                    });
                    cswPrivate.topToolbar.push(cswPrivate.deleteAllButton);
                } // if(cswPrivate.showCheckboxes && cswPrivate.showActionColumn)
                
                if(cswPrivate.topToolbar.length === '1') {
                    gridopts.dockedItems.push({
                        xtype: 'toolbar',
                        dock: 'top',
                        items: cswPrivate.topToolbar
                    }); // panelopts.dockedItems
                }

                if (Csw.isElementInDom(cswParent.getId())) {
                    cswPublic.extGrid = window.Ext.create('Ext.grid.Panel', gridopts);
                } else {
                    cswPublic.extGrid = window.Ext.create('Ext.grid.Panel');
                }
                return cswPublic.extGrid;
            }); // makeGrid()

            cswPublic.reload = function () {
                cswPrivate.getData(function (result) {
                    if (result && result.grid && result.grid.data && result.grid.data.items) {
                        cswPrivate.data = result.grid.data;
                        cswPrivate.store.destroy();
                        cswPrivate.store = cswPrivate.makeStore(cswPrivate.name + 'store', cswPrivate.usePaging);
                        cswPrivate.grid.reconfigure(cswPrivate.store);
                    } else {
                        Csw.debug.error('Failed to reload grid');
                    }
                });
            };

            cswPrivate.removeAll = function () {
                if (cswPrivate.store) {
                    cswPrivate.store.removeAll();
                    cswPrivate.store.destroy();
                }
                if (cswPrivate.grid) {
                    cswPrivate.grid.removeAll();
                    cswPrivate.grid.destroy();
                }
                cswParent.empty();
            };

            cswPrivate.init = Csw.method(function () {
                cswPrivate.removeAll();

                cswPrivate.rootDiv = cswParent.div();

                cswPrivate.store = cswPrivate.makeStore(cswPrivate.name + 'store', cswPrivate.usePaging);
                cswPrivate.grid = cswPrivate.makeGrid(cswPrivate.rootDiv.getId(), cswPrivate.store);

                cswPrivate.grid.on({
                    select: function (rowModel, record, index, eOpts) {
                        Csw.tryExec(cswPrivate.onSelect, record.data);
                    },
                    deselect: function (rowModel, record, index, eOpts) {
                        Csw.tryExec(cswPrivate.onDeselect, record.data);
                    },
                    afterrender: function (component) {
                        var bottomToolbar = component.getDockedComponent('bottomtoolbar');
                        if (false === Csw.isNullOrEmpty(bottomToolbar)) {
                            bottomToolbar.items.get('refresh').hide();
                        }
                    }
                });

                if (Csw.bool(cswPrivate.truncated)) {
                    cswPrivate.rootDiv.span({ cssclass: 'truncated', text: 'Results Truncated' });
                }

            }); // init()


            cswPublic.getCell = Csw.method(function (rowindex, key) {
                ///<summary>Gets the contents of a jqGrid cell by rowid and column key</summary>
                return cswPrivate.store.getAt(rowindex).raw[key];
            });

            cswPublic.getSelectedRowId = Csw.method(function () {
                return cswPrivate.store.indexOf(cswPrivate.grid.getSelectionModel().getSelection()[0]);
            });

            cswPublic.getSelectedRows = Csw.method(function () {
                var ret = [];
                Csw.each(cswPrivate.grid.getSelectionModel().getSelection(), function(val) {
                     if(val.data) {
                         ret.push(val.data);
                     }
                });
                return ret;
            });

            cswPublic.getSelectedRowsVals = Csw.method(function (key) {
                var ret = [];
                Csw.each(cswPublic.getSelectedRows(), function (val) {
                    if (val[key]) {
                        ret.push(val[key]);
                    }
                });
                return ret;
            });

            cswPublic.scrollToRow = Csw.method(function (rowindex) {
                ///<summary>Scrolls the grid to the specified index</summary>
                ///<param name="rowid" type="String">Optional. jqGrid rowid. If null, selected row is assumed.</param>
                ///<returns type="Void"></returns>
                if (Csw.isNullOrEmpty(rowindex)) {
                    rowindex = cswPublic.getSelectedRowId();
                }
                cswPrivate.grid.getView().focusRow(rowindex);
            });

            cswPublic.getRowIdForVal = Csw.method(function (column, value) {
                ///<summary>Gets a row index by column name and value.</summary>
                ///<param name="value" type="String">Cell value</param>
                ///<param name="column" type="String">Column name</param>
                ///<returns type="String">row index.</returns>
                return cswPrivate.store.findExact(column, value);
            });

            cswPublic.getValueForColumn = Csw.method(function (columnname, rowindex) {
                ///<summary>Gets a cell value by column name.</summary>
                ///<param name="columnname" type="String">Grid column name.</param>
                ///<param name="rowid" type="String">Optional. If null, selected row is assumed.</param>
                ///<returns type="String">Value of the cell.</returns>
                if (Csw.isNullOrEmpty(rowindex)) {
                    rowindex = cswPublic.getSelectedRowId();
                }
                var ret = cswPublic.getCell(rowindex, columnname);
                return ret;
            });

            cswPublic.setSelection = Csw.method(function (rowindex) {
                ///<summary>Sets the selected row by index</summary>
                if (rowindex > -1) {
                    cswPrivate.grid.getSelectionModel().select(rowindex);
                }
            });

            cswPublic.getAllGridRows = function () {
                return cswPrivate.store.data;
            };

            cswPublic.print = Csw.method(function (onSuccess) {
                // turn paging off
                var printStore = cswPrivate.makeStore(cswPrivate.name + 'printstore', false);
                var printGrid = cswPrivate.makeGrid('', printStore);

                window.Ext.ux.grid.Printer.stylesheetPath = 'js/thirdparty/extJS-4.1.0/ux/grid/gridPrinterCss/print.css';
                window.Ext.ux.grid.Printer.print(printGrid);
            });

            cswPublic.toggleShowCheckboxes = Csw.method(function (val) {
                cswPrivate.showCheckboxes = (false === cswPrivate.showCheckboxes);
                if (false === cswPrivate.showCheckboxes) {
                    if(options.topToolbar) {
                        cswPrivate.topToolbar = options.topToolbar;
                    } else {
                        cswPrivate.topToolbar = [];
                    }
                }
                cswPrivate.init();
            });

            cswPrivate.calculateHeight = Csw.method(function (rows) {
                return 25 + // title bar
                       23 + // grid header
                       (rows * 28) + // rows
                       14 + // horizontal scrollbar
                       27;  // grid footer
            });

            cswPrivate.getData = function (onSuccess) {
                cswPublic.ajax = Csw.ajax.post({
                    url: cswPrivate.ajax.url,
                    urlMethod: cswPrivate.ajax.urlMethod,
                    data: cswPrivate.ajax.data,
                    success: function (result) {
                        if (false === Csw.isNullOrEmpty(result.grid)) {
                            Csw.tryExec(onSuccess, result);
                        } // if(false === Csw.isNullOrEmpty(data.griddata)) {
                    } // success
                }); // ajax.post()
            };

            cswPrivate.reInit = function (forceRefresh) {
                if (Csw.isNullOrEmpty(cswPrivate.data) || Csw.bool(forceRefresh)) {
                    cswPrivate.getData(function (result) {
                        if (false === Csw.isNullOrEmpty(result.grid)) {
                            cswPrivate.pageSize = Csw.number(result.grid.pageSize);
                            if (false === Csw.isNullOrEmpty(result.grid.truncated)) {
                                cswPrivate.truncated = result.grid.truncated;
                            }
                            if (false === Csw.isNullOrEmpty(result.grid.title)) {
                                cswPrivate.title = result.grid.title;
                            }
                            cswPrivate.fields = result.grid.fields;
                            cswPrivate.columns = result.grid.columns;
                            cswPrivate.data = result.grid.data;
                            cswPrivate.ajaxResult = result;
                            cswPrivate.init();

                        } // if(false === Csw.isNullOrEmpty(data.griddata)) {
                    });
                } else {
                    cswPrivate.init();
                }
            };

            //constructor
            (function () {
                Csw.extend(cswPrivate, options);
                cswPrivate.ID = cswPrivate.ID || cswParent.getId();
                cswPrivate.ID += cswPrivate.suffix;
                cswPrivate.reInit();
            }());

            return cswPublic;
        });

}());
