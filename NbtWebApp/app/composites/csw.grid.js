/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />
/// <reference path="~/app/CswApp-vsdoc.js" />

/*global Csw:true*/
(function () {
    'use strict';
    Csw.composites.grid = Csw.composites.grid ||
        Csw.composites.register('grid', function (cswParent, options) {

            //#region _preCtor

            var cswPrivate;
            var cswPublic;

            (function () {

                cswPublic = cswParent.div();

                cswPrivate = {
                    name: 'extjsGrid',
                    title: '',
                    truncated: false,
                    usePaging: true,
                    forceFit: false,   // expand all columns to fill width (makes column resizing weird)

                    ajax: {
                        urlMethod: '',
                        data: {}
                    },

                    showMultiEditToolbar: true,
                    showCheckboxes: false,
                    showActionColumn: true,
                    showView: true,
                    showPreview: true,
                    showLock: true,
                    showEdit: true,
                    showDelete: true,

                    canSelectRow: false,
                    reapplyViewReadyOnLayout: false, //This is not normal. See Requesting for the specific use case.
                    
                    onLoad: function (grid, ajaxResult) { },
                    onEdit: function (rows) { },
                    onPreview: function (rows) { },
                    onDelete: function (rows) { },
                    onSelect: function (rows) { },
                    onDeselect: function (row) { },
                    onSelectChange: function (rowCount) { },
                    onMouseEnter: function (rowCount) { },
                    onMouseExit: function (rowCount) { },

                    height: '',  // overridden by webservice if paging is on
                    width: '',

                    fields: [],   // [ { name: 'col1', type: 'string' }, ... ]
                    columns: [],  // [ { header: 'Col1', dataIndex: 'col1', ... }, ... ]
                    data: {},     // { items: [ { col1: val, col2: val ... }, ... ]
                    pageSize: '',  // overridden by webservice

                    actionDataIndex: 'action',

                    topToolbar: [],
                    groupField: '',
                    plugins: null,
                    groupHeaderTpl: '{columnName}: {name}',
                    summaryEnabled: false,
                    printingEnabled: false,
                    gridToPrint: function (grid) {
                        return grid;
                    },
                    onPrintSuccess: function () { },
                    dockedItems: [],
                    sorters: []
                };

                Csw.extend(cswPrivate, options);
                cswPrivate.ID = cswPrivate.ID || cswPublic.getId();
                cswPrivate.ID += cswPrivate.suffix;


            }());

            //#endregion _preCtor

            //#region AJAX

            cswPrivate.getData = function (onSuccess) {
                if (cswPrivate.ajax.urlMethod !== '') {
                    cswPublic.ajax = Csw.ajax.post({
                        url: cswPrivate.ajax.url,
                        urlMethod: cswPrivate.ajax.urlMethod,
                        data: cswPrivate.ajax.data,
                        success: function (result) {
                            if (false === Csw.isNullOrEmpty(result.grid)) {
                                Csw.tryExec(onSuccess, result);
                            }
                        } // success
                    });
                } // ajax.post()
            };

            //#endregion AJAX

            //#region Grid Control Constructors

            cswPrivate.makeActionCell = function (tblObj) {
                if (Csw.isElementInDom(tblObj.cellId)) {
                    var div = Csw.domNode({
                        ID: tblObj.cellId,
                        tagName: 'DIV'
                    });
                    div.empty();

                    var table = div.table({ cellpadding: 0 });

                    var previewCell = table.cell(1, 1).css({ width: '26px' });
                    var editCell = table.cell(1, 2).css({ width: '26px' });
                    var delCel = table.cell(1, 3).css({ width: '26px' });

                    var canedit = Csw.bool(cswPrivate.showEdit) && Csw.bool(tblObj.cellData.canedit, true);
                    var canpreview = Csw.bool(cswPrivate.showPreview) && Csw.bool(tblObj.cellData.canview, true);
                    var canview = Csw.bool(cswPrivate.showView) && Csw.bool(tblObj.cellData.canview, true);
                    var candelete = Csw.bool(cswPrivate.showDelete) && Csw.bool(tblObj.cellData.candelete, true);
                    var islocked = Csw.bool(cswPrivate.showLock) && Csw.bool(tblObj.cellData.islocked, false);

                    if (canpreview) {
                        cswPrivate.makeActionButton(previewCell, 'Preview', Csw.enums.iconType.magglass, cswPrivate.onPreview, tblObj);
                    }

                    // only show one of edit/view/lock
                    if (islocked) {
                        cswPrivate.makeActionButton(editCell, 'Locked', Csw.enums.iconType.lock, null, tblObj.cellData);
                    } else if (canedit) {
                        cswPrivate.makeActionButton(editCell, 'Edit', Csw.enums.iconType.pencil, cswPrivate.onEdit, tblObj);
                    } else if (canview) {
                        cswPrivate.makeActionButton(editCell, 'View', Csw.enums.iconType.pencil, cswPrivate.onEdit, tblObj);
                    }

                    if (candelete) {
                        cswPrivate.makeActionButton(delCel, 'Delete', Csw.enums.iconType.trash, cswPrivate.onDelete, tblObj);
                    }
                } // if (Csw.isElementInDom(tblObj.cellId)) {
            }; // makeActionCell()

            cswPrivate.makeActionButton = function (tableCell, buttonName, iconType, clickFunc, cell) {
                if (cswPrivate.showActionColumn) {
                    var iconopts = {
                        name: cswPrivate.name + buttonName,
                        hovertext: buttonName,
                        iconType: iconType,
                        state: Csw.enums.iconState.normal,
                        isButton: false,
                        size: 18
                    };
                    if (false === Csw.isNullOrEmpty(clickFunc)) {
                        iconopts.isButton = true;
                        iconopts.onClick = function (event) {
                            Csw.tryExec(clickFunc, [cell.cellData], cell.raw, event);
                        };
                    }
                    tableCell.icon(iconopts);
                }
            }; // makeActionButton()


            cswPrivate.makeStore = Csw.method(function (storeId, usePaging) {
                var fields = Csw.extend([], cswPrivate.fields);

                var storeopts = {
                    storeId: storeId,
                    fields: fields,
                    data: cswPrivate.rows,
                    autoLoad: true,
                    proxy: {
                        type: 'memory',
                        reader: {
                            type: 'json',
                            root: 'items'
                        }
                    },
                    groupField: cswPrivate.groupField,
                    sorters: cswPrivate.sorters
                };

                if (cswPrivate.showActionColumn && false === cswPrivate.showCheckboxes) {
                    var newfld = { name: cswPrivate.actionDataIndex };
                    storeopts.fields.splice(0, 0, newfld);
                }
                if (Csw.bool(usePaging)) {
                    storeopts.pageSize = cswPrivate.pageSize;
                    storeopts.proxy.type = 'pagingmemory';
                }

                var store = window.Ext.create('Ext.data.Store', storeopts);

                //Case 28476 - manually collapse all groups to fix a bug in ExtJS
                store.on('load', function (store, records, success) {
                    Csw.tryExec(cswPrivate.toggleGroups, true);
                });

                return store;
            }); // makeStore()

            cswPrivate.makeListeners = function () {
                //Case 28555: ExtJS documentation is awful, 
                //so we frequently need to blindly test their events to discover when they fire and why.
                //To do so, uncomment any of the available events (current as of Ext 4.1.3) and fiddle with the grid. 

                //var debugFunc = function () {
                //    debugger;
                //};

                return {
                    afterrender: function (grid) {
                        //of the methods we're listening to here, called 2nd, 
                        //still nothing to see in the DOM
                        grid.filters.createFilters();
                    },
                    viewready: function () {
                        //of the methods we're listening to here, called 4th. Will also trigger afterlayout.
                        Csw.tryExec(cswPrivate.onLoad, cswPublic, cswPrivate.ajaxResult);
                    },
                    afterlayout: function() {
                        if (cswPrivate.reapplyViewReadyOnLayout) {
                            Csw.tryExec(cswPrivate.onLoad, cswPublic, cswPrivate.ajaxResult);
                        }
                    }
                    //sortchange: function () { debugFunc(); },
                    //                    render: function () {
                    //                        //of the methods we're listening to here, called 1st
                    //                        //debugFunc();
                    //                    },
                    //activate: function () { debugFunc(); },
                    //add: function () { debugFunc(); },
                    //added: function () { debugFunc(); },
                    //                    afterlayout: function () {
                    //                        //of the methods we're listening to here, called 3rd on a new grid.
                    //                        //empty grid is now rendered

                    //                        //Also called on any mutation to grid (sort, columns show/hide, etc)
                    //                    }
                    //beforeactivate: function () { debugFunc(); },
                    //beforeadd: function () { debugFunc(); },
                    //beforeclose: function () { debugFunc(); },
                    //beforecollapse: function () { debugFunc(); },
                    //beforecontainerclick: function () { debugFunc(); },
                    //beforecontainercontextmenu: function () { debugFunc(); },
                    //beforecontainerdblclick: function () { debugFunc(); },
                    //beforecontainermousedown: function () { debugFunc(); },
                    //beforecontainermouseout: function () { debugFunc(); },
                    //beforecontainermouseover: function () { debugFunc(); },
                    //beforecontainermouseup: function () { debugFunc(); },
                    //beforedeactivate: function () { debugFunc(); },
                    //beforedeselect: function () { debugFunc(); },
                    //beforedestroy: function () { debugFunc(); },
                    //beforeedit: function () { debugFunc(); },
                    //beforeexpand: function () { debugFunc(); },
                    //beforehide: function () { debugFunc(); },
                    //beforeitemclick: function () { debugFunc(); },
                    //beforeitemcontextmenu: function () { debugFunc(); },
                    //beforeitemdblclick: function () { debugFunc(); },
                    //beforeitemmousedown: function () { debugFunc(); },
                    //beforeitemmouseenter: function () { debugFunc(); },
                    //beforeitemmouseleave: function () { debugFunc(); },
                    //beforeitemmouseup: function () { debugFunc(); },
                    //beforereconfigure: function () { debugFunc(); },
                    //beforeremove: function () { debugFunc(); },
                    //beforerender: function () { debugFunc(); },
                    //beforeselect: function () { debugFunc(); },
                    //beforeshow: function () { debugFunc(); },
                    //beforestaterestore: function () { debugFunc(); },
                    //beforestatesave: function () { debugFunc(); },
                    //blur: function () { debugFunc(); },
                    //boxready: function () { debugFunc(); },
                    //canceledit: function () { debugFunc(); },
                    //cellclick: function () { debugFunc(); },
                    //celldblclick: function () { debugFunc(); },
                    //close: function () { debugFunc(); },
                    //collapse: function () { debugFunc(); },
                    //columnhide: function () { debugFunc(); },
                    //columnmove: function () { debugFunc(); },
                    //columnresize: function () { debugFunc(); },
                    //columnshow: function () { debugFunc(); },
                    //containerclick: function () {
                    //    cswPrivate.makeActionColumns(0);
                    //    debugFunc();
                    //},
                    //containercontextmenu: function () { debugFunc(); },
                    //containerdblclick: function () { debugFunc(); },
                    //containermouseout: function () { debugFunc(); },
                    //containermouseover: function () { debugFunc(); },
                    //containermouseup: function () { debugFunc(); },
                    //deactivate: function () { debugFunc(); },
                    //deselect: function () { debugFunc(); },
                    //destroy: function () { debugFunc(); },
                    //disable: function () { debugFunc(); },
                    //edit: function () { debugFunc(); },
                    //enable: function () { debugFunc(); },
                    //expand: function () { debugFunc(); },
                    //focus: function () { debugFunc(); },
                    //hide: function () { debugFunc(); },
                    //iconchange: function () { debugFunc(); },
                    //iconclschange: function () { debugFunc(); },
                    //itemclick: function () {
                    //    cswPrivate.makeActionColumns(0);
                    //    debugFunc();
                    //},
                    //itemcontextmenu: function () {
                    //    cswPrivate.makeActionColumns(0);
                    //    debugFunc();
                    //},
                    //itemdblclick: function () { debugFunc(); },
                    //itemmousedown: function () { debugFunc(); },
                    //itemmouseenter: function () { debugFunc(); },
                    //itemmouseleave: function () { debugFunc(); },
                    //itemmouseup: function () { debugFunc(); },
                    //move: function () { debugFunc(); },
                    //reconfigure: function () { debugFunc(); },
                    //remove: function () { debugFunc(); },
                    //removed: function () { debugFunc(); },
                    //resize: function () { debugFunc(); },
                    //select: function () { debugFunc(); },
                    //selectionchange: function () { debugFunc(); },
                    //show: function () { debugFunc(); },
                    //staterestore: function () { debugFunc(); },
                    //statesave: function () { debugFunc(); },
                    //titlechange: function () { debugFunc(); },
                    //unfloat: function () { debugFunc(); },
                    //validateedit: debugFunc
                };
            };


            cswPrivate.makeDockedItems = function () {
                cswPrivate.dockedItems = [];
                var topToolbarItems = [];

                //Printing
                if (cswPrivate.printingEnabled) {
                    topToolbarItems.push({
                        tooltip: 'Print the contents of the grid',
                        text: 'Print',
                        handler: function () {
                            var gridToPrint = cswPrivate.gridToPrint(cswPublic);
                            gridToPrint.print();
                        }
                    });
                }

                //Grouping and Group Summary
                if (cswPrivate.groupField &&
                    cswPrivate.groupField.length > 0) {
                    cswPrivate.groupField = cswPrivate.groupField.replace(' ', '_');


                    cswPrivate.toggleGroups = function (collapse) {
                        if (cswPrivate.grid.view) {
                            Csw.each(cswPrivate.grid.view.features, function(feature) {
                                if (feature.ftype === 'grouping' || feature.ftype === 'groupingsummary') {
                                    if (collapse) {
                                        feature.collapseAll();
                                    } else {
                                        feature.collapseAll(); //for some reason expandAll() only works after collapseAll() has been called
                                        feature.expandAll();
                                    }
                                }
                            });
                        }
                    };
                    if (topToolbarItems.length > 0) {
                        topToolbarItems.push({ xtype: 'tbseparator' });
                    }
                    topToolbarItems.push({
                        xtype: 'button',
                        text: 'Expand all Rows',
                        handler: function () {
                            cswPrivate.toggleGroups(false);
                        }
                    });
                    topToolbarItems.push({ xtype: 'tbseparator' });
                    topToolbarItems.push({
                        xtype: 'button',
                        text: 'Collapse all Rows',
                        handler: function () {
                            cswPrivate.toggleGroups(true);
                        }
                    });
                    if (cswPrivate.summaryEnabled) {
                        topToolbarItems.push({ xtype: 'tbseparator' });
                        var showSummary = true;
                        topToolbarItems.push({
                            tooltip: 'Toggle the visibility of the summary row',
                            text: 'Toggle Summary',
                            enableToggle: true,
                            pressed: true,
                            handler: function () {
                                showSummary = !showSummary;
                                Csw.each(cswPrivate.grid.view.features, function (feature) {
                                    if (feature.ftype === 'groupingsummary') {
                                        feature.toggleSummaryRow(showSummary);
                                        cswPrivate.grid.view.refresh();
                                    }
                                });
                            }
                        });
                    }
                }

                // Multi-Edit
                if (cswPrivate.showMultiEditToolbar !== false &&
                    cswPrivate.showCheckboxes &&
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
                    topToolbarItems.push(cswPrivate.editAllButton);

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
                    topToolbarItems.push(cswPrivate.deleteAllButton);
                } // if(cswPrivate.showCheckboxes && cswPrivate.showActionColumn)

                if (topToolbarItems.length > 0) {
                    cswPrivate.dockedItems.push({
                        xtype: 'toolbar',
                        dock: 'top',
                        items: topToolbarItems
                    });
                }
            };

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
                    listeners: cswPrivate.makeListeners(),
                    dockedItems: cswPrivate.dockedItems,
                    features: [{
                        ftype: 'filters',
                        autoReload: false,
                        encode: false,
                        local: true
                    },
                    {
                        id: 'group',
                        ftype: 'grouping' + (cswPrivate.summaryEnabled ? 'summary' : ''),
                        groupHeaderTpl: cswPrivate.groupHeaderTpl,
                        hideGroupedHeader: true,
                        enableGroupingMenu: false,
                        startCollapsed: true
                    }]
                };

                if (cswPrivate.plugins) {
                    gridopts.plugins = cswPrivate.plugins;
                }

                // Action column
                if (cswPrivate.showActionColumn) { //&& false === cswPrivate.showCheckboxes
                    var newcol = {
                        header: 'Action',
                        dataIndex: cswPrivate.actionDataIndex,
                        width: 70,
                        flex: false,
                        resizable: false,
                        xtype: 'actioncolumn',
                        renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                            //Terrible choice in words, "renderer" means the event that will run sometime after this based on the HTML string you define.
                            var divId = cswPrivate.name + 'action' + rowIndex + colIndex;
                            Csw.defer(Csw.method(function () {
                                cswPrivate.makeActionCell({
                                    cellId: divId,
                                    cellData: record.data,
                                    raw: record.raw
                                });
                            }), 100);
                            return '<div id="' + divId + '" style="height:18px;"></div>';
                        } // renderer()
                    }; // newcol
                    gridopts.columns.splice(0, 0, newcol);
                } // if(cswPrivate.showActionColumn && false === cswPrivate.showCheckboxes) {

                //Render buttons in a callback
                if (false === cswPrivate.showCheckboxes &&
                    cswPrivate.rows.buttons &&
                    cswPrivate.rows.buttons.length > 0) {

                    var colNames = Csw.delimitedString('', { spaceToDelimiter: false });
                    Csw.each(cswPrivate.rows.buttons, function (val, key) {
                        //Get the column names, delimitedString will handle dupes for us automatically
                        colNames.add(val.selectedtext);
                    });

                    var cols = cswPrivate.columns.filter(function (col) {
                        return colNames.contains(col.header);
                    });
                    var i = 0;
                    Csw.each(cols, function (colObj, key) {
                        colObj.renderer = function (value, metaData, record, rowIndex, colIndex, store, view) {
                            //NOTE: this can now be moved to the viewrender event. See action column logic.
                            i += 1;
                            var id = cswPrivate.ID + 'nodebutton' + i;
                            var thisBtn = cswPrivate.rows.buttons.filter(function (btn) {
                                return btn.index === colObj.dataIndex && btn.rowno === rowIndex;
                            });
                            if (thisBtn.length === 1) {
                                Csw.defer(function _tryMakeBtn() {
                                    //Case 28343. The problem here is that 
                                    // a) our div is not in the DOM until this method returns and 
                                    // b) we're not always guaranteed to be in the writable portion of the cell--the div we return might be thrown away by Ext
                                    if (Csw.isElementInDom(id)) {
                                        var div = Csw.domNode({ ID: id });
                                        div.nodeButton({
                                            value: colObj.header,
                                            size: 'small',
                                            propId: thisBtn[0].propattr
                                        });
                                    }
                                }, 100);
                            }
                            return '<div id="' + id + '"></div>';

                        };
                    });

                }

                if (!cswPrivate.selModel || !cswPrivate.selModel.selType) {
                    // Selection mode
                    if (cswPrivate.showCheckboxes) {
                        gridopts.selType = 'checkboxmodel';
                        gridopts.selModel = { mode: 'Simple' };
                        gridopts.listeners.selectionchange = function (t, selected, eOpts) {
                            if (cswPrivate.editAllButton && cswPrivate.deleteAllButton) {
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
                } else if (cswPrivate.selModel) {
                    gridopts.selType = cswPrivate.selModel.selType;
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

                    var rows = cswPrivate.rows.items.length;
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

                if (Csw.isElementInDom(cswPublic.getId())) {
                    cswPublic.extGrid = window.Ext.create('Ext.grid.Panel', gridopts);
                } else {
                    cswPublic.extGrid = window.Ext.create('Ext.panel.Panel');
                }
                
                return cswPublic.extGrid;
            }); // makeGrid()

            cswPrivate.calculateHeight = Csw.method(function (rows) {
                return 25 + // title bar
                       23 + // grid header
                       (rows * 28) + // rows
                       14 + // horizontal scrollbar
                       27;  // grid footer
            });

            cswPrivate.removeAll = function () {
                if (cswPrivate.store) {
                    cswPrivate.store.removeAll();
                    cswPrivate.store.destroy();
                }
                if (cswPrivate.grid) {
                    cswPrivate.grid.removeAll();
                    cswPrivate.grid.destroy();
                }
                cswPublic.empty();
            };

            cswPublic.destroy = function () {
                cswPrivate.removeAll();
            };

            //#endregion Grid Control Constructors

            //#region Grid Init

            cswPrivate.init = Csw.method(function () {
                cswPrivate.removeAll();

                cswPrivate.rootDiv = cswPublic.div();

                cswPrivate.makeDockedItems();
                cswPrivate.store = cswPrivate.makeStore(cswPrivate.name + 'store', cswPrivate.usePaging);
                cswPrivate.grid = cswPrivate.makeGrid(cswPrivate.rootDiv.getId(), cswPrivate.store);

                if (cswPrivate.grid) {
                    cswPrivate.grid.on({
                        beforeSelect: function (rowModel, record, index, eOpts) {
                            return Csw.tryExec(cswPrivate.onBeforeSelect, record);
                        },
                        select: function (rowModel, record, index, eOpts) {
                            Csw.tryExec(cswPrivate.onSelect, record.data);
                        },
                        deselect: function (rowModel, record, index, eOpts) {
                            Csw.tryExec(cswPrivate.onDeselect, record.data);
                        },
                        selectionchange: function (rowModel, selected, eOpts) {
                            Csw.tryExec(cswPrivate.onSelectChange, cswPublic.getSelectedRowCount());
                        }
                    }); // init()

                    if (Csw.bool(cswPrivate.truncated)) {
                        cswPrivate.rootDiv.span({ cssclass: 'truncated', text: 'Results Truncated' });
                    }
                }
            });

            cswPrivate.setInternalGridState = function (data) {
                if (data) {
                    cswPrivate.data = data;
                    cswPrivate.pageSize = Csw.number(data.pageSize);
                    if (false === Csw.isNullOrEmpty(data.truncated)) {
                        cswPrivate.truncated = data.truncated;
                    }
                    if (cswPrivate.title.length === 0 && data.title && data.title.length > 0) {
                        cswPrivate.title = data.title;
                    }
                    if (data.fields) {
                        cswPrivate.fields = data.fields;
                    }
                    if (data.columns) {
                        cswPrivate.columns = data.columns;
                    }
                    if (data.data) {
                        cswPrivate.rows = data.data;
                    } else {
                        cswPrivate.rows = data;
                    }
                    if (data.groupfield) {
                        cswPrivate.groupField = data.groupfield;
                    }
                }
            };

            cswPrivate.reInit = function (forceRefresh) {
                if (Csw.isNullOrEmpty(cswPrivate.data) || Csw.bool(forceRefresh)) {
                    cswPrivate.getData(function (result) {
                        if (false === Csw.isNullOrEmpty(result.grid)) {
                            cswPrivate.setInternalGridState(result.grid);
                            cswPrivate.ajaxResult = result;
                            cswPrivate.init();
                        }
                    });
                } else {
                    cswPrivate.setInternalGridState(cswPrivate.data);
                    cswPrivate.init();
                }
            };

            //#endregion Grid Init

            //#region Public methods

            cswPublic.reload = function (forceRefresh) {
                cswPrivate.reInit(forceRefresh);
            };

            cswPublic.getCell = Csw.method(function (rowindex, key) {
                ///<summary>Gets the contents of a jqGrid cell by rowid and column key</summary>
                return cswPrivate.store.getAt(rowindex).raw[key];
            });

            cswPublic.getSelectedRowId = Csw.method(function () {
                return cswPrivate.store.indexOf(cswPrivate.grid.getSelectionModel().getSelection()[0]);
            });

            cswPublic.getSelectedRowCount = Csw.method(function () {
                return cswPrivate.grid.getSelectionModel().getSelection().length;
            });

            cswPublic.getSelectedRows = Csw.method(function () {
                var ret = [];
                Csw.each(cswPrivate.grid.getSelectionModel().getSelection(), function (val) {
                    if (val.data) {
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

            cswPublic.deselectAll = Csw.method(function () {
                ///<summary>Deselect all records</summary>
                cswPrivate.grid.getSelectionModel().deselectAll();
            });

            cswPublic.setSelection = Csw.method(function (rowindex) {
                ///<summary>Sets the selected row by index</summary>
                if (rowindex > -1) {
                    cswPrivate.grid.getSelectionModel().select(rowindex);
                }
            });

            cswPublic.getAllGridRows = function () {
                /// <summary>
                /// Returns the raw store data in its current state
                /// </summary>
                var return_val = null;

                if (cswPrivate.store) {
                    return_val = cswPrivate.store.data;
                }

                return return_val;
            };

            cswPrivate.extractRows = function (rawRows) {
                var ret = [];
                if (rawRows && rawRows.length > 0) {
                    rawRows.forEach(function (item) {
                        if (item && item.data) {
                            ret.push(item.data);
                        }
                    });
                }
                return ret;
            };

            cswPublic.getGridItems = function () {
                /// <summary>
                /// Returns the data store as an Array that conforms to the array used to construct the grid
                /// </summary>
                /// <returns type="Array"></returns>
                var return_val = [];

                if (cswPrivate.store && cswPrivate.store.data) {
                    return_val = cswPrivate.extractRows(cswPrivate.store.data);
                }

                return return_val;
            };

            cswPublic.getUpdatedGridItems = function () {
                /// <summary>
                /// Returns the updated items in the data store as an Array that conforms to the array used to construct the grid
                /// </summary>
                /// <returns type="Array"></returns>
                var return_val = [];

                if (cswPrivate.store && cswPrivate.store.getUpdatedRecords) {
                    return_val = cswPrivate.extractRows(cswPrivate.store.getUpdatedRecords());
                }

                return return_val;
            };

            cswPublic.getModifiedGridItems = function () {
                /// <summary>
                /// Returns the modified items in the data store as an Array that conforms to the array used to construct the grid
                /// </summary>
                /// <returns type="Array"></returns>
                var return_val = [];

                if (cswPrivate.store && cswPrivate.store.getModifiedRecords) {
                    return_val = cswPrivate.extractRows(cswPrivate.store.getModifiedRecords());
                }

                return return_val;
            };

            cswPublic.iterateRows = function(callBack) {
                /// <summary>
                /// Iterate each row in the grid and execute a callback with the row record and node as the parameters
                /// </summary>
                if (cswPrivate.grid && cswPrivate.store) {
                    var view = cswPrivate.grid.getView();
                    Csw.iterate(cswPrivate.store.getRange(), function _callback(row) {
                        var node = view.getNode(row);
                        callBack(row, node);
                    });
                }
            };

            cswPublic.print = Csw.method(function () {
                // turn paging off
                var printStore = cswPrivate.makeStore(cswPrivate.name + 'printstore', false);
                var printGrid = cswPrivate.makeGrid('', printStore);

                window.Ext.ux.grid.Printer.stylesheetPath = 'vendor/extJS-4.1.0/ux/grid/gridPrinterCss/print.css';
                window.Ext.ux.grid.Printer.print(printGrid);
                Csw.tryExec(cswPrivate.onPrintSuccess);
            });

            cswPublic.toggleShowCheckboxes = Csw.method(function (val) {
                cswPrivate.showCheckboxes = (false === cswPrivate.showCheckboxes);
                if (false === cswPrivate.showCheckboxes) {
                    cswPrivate.dockedItems = cswPrivate.dockedItems.filter(function (docked) {
                        return (docked.dock !== 'top');
                    });
                    if (options.topToolbar) {
                        cswPrivate.topToolbar = options.topToolbar;
                    } else {
                        cswPrivate.topToolbar = [];
                    }
                }
                cswPrivate.init();
            });

            //#endregion Public methods

            //#region _postCtor

            //constructor
            (function _postCtor() {
                cswPrivate.reInit();
            }());

            return cswPublic;

            //#endregion _postCtor
        });

}());
