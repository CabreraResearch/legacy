/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.composites.layoutTable = Csw.composites.layoutTable ||
        Csw.composites.register('layoutTable', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: '',
                cellSet: { rows: 1, columns: 1 },
                firstRow: null,
                firstCol: null,
                onSwap: null,
                onAddClick: null,
                onConfigOn: null,
                onConfigOff: null,
                onRemove: null,
                TableCssClass: '',
                CellCssClass: '',
                cellalign: '',
                cellpadding: 0,
                cellspacing: 0,
                width: '100%',
                styles: {},
                align: 'left',
                showConfigButton: false,
                showAddButton: false,
                showRemoveButton: false,
                showExpandRowButton: false,
                showExpandColButton: false,
                OddCellRightAlign: false,
                ReadOnly: false,

                configMode: false,
                removeMode: false
            };
            var cswPublic = {};

            cswPrivate.onClick = function (ev, dd, row, column) {
                var removeCells;
                if (cswPrivate.isRemoveMode()) {
                    removeCells = cswPublic.table.find('.CswLayoutTable_remove');
                    if (removeCells.length() > 0) {
                        cswPublic.table.trigger(cswPrivate.name + 'CswLayoutTable_onRemove', {
                            table: cswPublic.table,
                            cellSet: cswPublic.cellSet(row, column),
                            row: removeCells.data('row'),
                            column: removeCells.data('column')
                        });
                        removeCells.children().hide();

                        removeCells.removeClass('CswLayoutTable_remove');
                    }

                } // if(cswPrivate.isRemoveMode($cswPublic.table))
            }; // cswPrivate.onClick()

            cswPrivate.isRemoveMode = function () {
                return cswPrivate.removeMode;
            };

            cswPrivate.toggleRemove = function () {
                if (cswPrivate.isRemoveMode()) {
                    cswPrivate.removeMode = false;  // cswPublic.table.data('removemode', 'false');
                    if (cswPrivate.removeBtn) {
                        cswPrivate.removeBtn.removeClass('CswLayoutTable_removeEnabled');
                    }
                } else {
                    cswPrivate.removeMode = true;   // cswPublic.table.data('removemode', 'true');
                    if (cswPrivate.removeBtn) {
                        cswPrivate.removeBtn.addClass('CswLayoutTable_removeEnabled');
                    }
                }
            };

            cswPrivate.expandLayoutTable = function (expRow, expCol) {
                /// <summary>Expand the layout table by an additional cellSet row or column or both.</summary>
                /// <param name="expRow" type="Boolean">True to expand by a cellSet row.</param>
                /// <param name="expCol" type="Boolean">True to expand by a cellSet column.</param>
                /// <returns type="undefined"></returns>
                var tablemaxrows = cswPublic.table.maxrows(),
                    tablemaxcolumns = cswPublic.table.maxcolumns(),
                    rowCount = 0, colCount = 0,
                    requestRow, requestCol;
                if (expRow) {
                    rowCount = cswPrivate.cellSet.rows;
                }
                if (expCol) {
                    colCount = cswPrivate.cellSet.columns;
                }
                if (rowCount > 0 || colCount > 0) {
                    requestRow = tablemaxrows + rowCount,
                    requestCol = tablemaxcolumns + colCount;

                    cswPublic.table.cell(requestRow, requestCol);
                    cswPublic.table.finish(null, cswPrivate.firstRow, cswPrivate.firstCol);

                    if (cswPublic.isConfig()) {
                        cswPublic.table.findCell('.CswLayoutTable_cell')
                            .addClass('CswLayoutTable_configcell');
                    }
                }
            };

            cswPrivate.addRow = function () {
                cswPrivate.expandLayoutTable(true, false);
            }; // _addRowAndColumn()

            cswPrivate.addColumn = function () {
                cswPrivate.expandLayoutTable(false, true);
            }; // cswPrivate.addColumn()

            cswPrivate.getCell = function (getRow, getColumn, cellsetrow, cellsetcolumn) {
                var row = Csw.number(getRow),
                    column = Csw.number(getColumn),
                    realrow, realcolumn, cell;
                if (row < 1) {
                    row = 1;
                }
                if (column < 1) {
                    column = 1;
                }
                realrow = ((row - 1) * cswPrivate.cellSet.rows) + Csw.number(cellsetrow, 1);
                realcolumn = ((column - 1) * cswPrivate.cellSet.columns) + Csw.number(cellsetcolumn, 1);

                cell = cswPublic.table.cell(realrow, realcolumn);

                return cell;
            };

            cswPrivate.enableDrop = function (cell) {
                var cellObj;
                if (cell) {
                    cellObj = cell;
                } else {
                    cellObj = cswPublic.table.find('.CswLayoutTable_celldiv');
                }
                try {
                    cellObj.$.droppable('destroy');
                } catch (e) {
                    Csw.debug.error('destroy() was called on a droppable before droppable was instanced on the control. This is non-fatal, but it should not happen.');
                    Csw.debug.error(e);
                }
                cellObj.$.droppable({
                    hoverClass: 'CswLayoutTable_hover',
                    drop: function (ev, dd) {
                        cswPrivate.onDrop(ev, dd, $(this));
                    }
                }).hover(function (ev, dd) {
                    cswPrivate.onHoverIn(ev, dd, $(this));
                }, function (ev, dd) {
                    cswPrivate.onHoverOut(ev, dd, $(this));
                });
            };

            cswPrivate.onCreateCell = function (cell, realRow, realCol) {
                var thisRow = Math.ceil(realRow / cswPrivate.cellSet.rows),
                    thisCol = Math.ceil(realCol / cswPrivate.cellSet.columns),
                    cellsetrow = Csw.number(cswPrivate.cellSet.rows - realRow % cswPrivate.cellSet.rows),
                    cellsetcolumn = Csw.number(cswPrivate.cellSet.columns - realCol % cswPrivate.cellSet.columns);

                if (null === cswPrivate.firstRow || thisRow < cswPrivate.firstRow) {
                    cswPrivate.firstRow = thisRow;
                }

                if (null === cswPrivate.firstCol || thisCol < cswPrivate.firstCol) {
                    cswPrivate.firstCol = thisCol;
                }

                cell.data({
                    row: thisRow,
                    column: thisCol,
                    cellsetrow: cellsetrow,
                    cellsetcolumn: cellsetcolumn
                });
                cell.bind('click', function (ev, dd) {
                    if (cswPublic.isConfig()) {
                        cswPrivate.onClick(ev, dd, thisRow, thisCol);
                    }
                });
                cswPrivate.enableDrop(cell);
                cell.div({ cssclass: 'CswLayoutTable_celldiv' });
            };

            cswPrivate.enableDrag = function () {
                cswPublic.table.find('.CswLayoutTable_celldiv')
                    .$.draggable({
                        revert: 'invalid',
                        drag: function (ev, dd) {
                            cswPrivate.onDrag(ev, dd, $(this));
                        }
                    });
            };

            cswPrivate.disableDrag = function () {
                cswPublic.table.find('.CswLayoutTable_celldiv')
                    .$.draggable('destroy');
            };

            cswPrivate.onHoverIn = function (ev, dd, $cell) {
                var cellSet;
                if (cswPrivate.isRemoveMode()) {
                    cellSet = cswPublic.cellSet($cell.attr('data-row'), $cell.attr('data-column'));
                    cswPrivate.eachCell(cellSet, function (cell) {
                        cell.addClass('CswLayoutTable_remove');
                    });
                }
            }; // cswPrivate.onHoverIn()

            cswPrivate.onHoverOut = function (ev, dd, $cell) {
                var cellSet;
                if (cswPrivate.isRemoveMode()) {
                    cellSet = cswPublic.cellSet($cell.attr('data-row'), $cell.attr('data-column'));
                    cswPrivate.eachCell(cellSet, function (cell) {
                        cell.removeClass('CswLayoutTable_remove');
                    });
                }
            }; // cswPrivate.onHoverOut()

            cswPrivate.onDrag = function (ev, dd, $dragDiv) {
                var $dragCell, cells;
                if (cswPublic.isConfig(cswPublic.table)) {
                    $dragCell = $dragDiv.parent();
                    cells = cswPublic.cellSet($dragCell.attr('data-row'), $dragCell.attr('data-column'));
                    cswPrivate.eachCell(cells, function (cell) {
                        cell.addClass('CswLayoutTable_dragcell');
                    });
                }
            }; // cswPrivate.onDrag

            cswPrivate.eachCell = function (cellSet, func) {
                for (var r = 1; r <= cswPrivate.cellSet.rows; r += 1) {
                    for (var c = 1; c <= cswPrivate.cellSet.columns; c += 1) {
                        if (cellSet[r] === undefined) {
                            cellSet[r] = Csw.array();
                        }
                        Csw.tryExec(func, cellSet[r][c], r, c);
                    }
                }
            };

            cswPrivate.onDrop = function (ev, dd, $dropCell) {
                var $dragDiv, dragCell, dragCells, dropCells;
                if (cswPublic.isConfig()) {
                    $dragDiv = dd.draggable;
                    dragCell = Csw.literals.factory($dragDiv.parent(), {});

                    dragCells = cswPublic.cellSet(dragCell.data('row'), dragCell.data('column'));
                    dropCells = cswPublic.cellSet($dropCell.attr('data-row'), $dropCell.attr('data-column'));

                    // This must happen BEFORE we do the swap, in case the caller relies on the contents of the div being where it was
                    cswPublic.table.trigger(cswPrivate.name + 'CswLayoutTable_onSwap', {
                        table: cswPublic.table,
                        cellSet: dragCells,
                        swapcellset: dropCells,
                        row: dragCell.data('row'),
                        column: dragCell.data('column'),
                        swaprow: dropCells[1][1].data('row'),
                        swapcolumn: dropCells[1][1].data('column')
                    });


                    for (var r = 1; r <= cswPrivate.cellSet.rows; r += 1) {
                        for (var c = 1; c <= cswPrivate.cellSet.columns; c += 1) {
                            var thisDragCell = dragCells[r][c];
                            var thisDropCell = dropCells[r][c];

                            thisDragCell.removeClass('CswLayoutTable_dragcell');
                            var $dragCellDiv = thisDragCell.children('div').$;
                            thisDropCell.removeClass('CswLayoutTable_dragcell');
                            var $dropCellDiv = thisDropCell.children('div').$;

                            thisDropCell.$.append($dragCellDiv);
                            thisDragCell.$.append($dropCellDiv);

                            $dragCellDiv.position({
                                my: "left top",
                                at: "left top",
                                of: thisDropCell.$,
                                offset: Csw.string(cswPrivate.cellpadding)
                            });

                            $dropCellDiv.position({
                                my: "left top",
                                at: "left top",
                                of: thisDragCell.$,
                                offset: Csw.string(cswPrivate.cellpadding)
                            });
                        }
                    }

                } // if(cswPublic.isConfig($cswPublic.table))
            }; // cswPrivate.onDrop()

            cswPublic.isConfig = function () {
                /// <summary>Determines whether layout table is in config mode.</summary>
                /// <returns type="Boolean">True if config mode is on.</returns>
                return cswPrivate.configMode; // Csw.bool(cswPublic.table.data('configmode'));
            };

            cswPublic.cellSet = function (row, column) {
                /// <summary>Get the set of cells representing a layout item.</summary>
                /// <param name="row" type="Number">Row number</param>
                /// <param name="column" type="Number">Column number</param>
                /// <returns type="Array">An array of the content.</returns>
                var cellSet = Csw.array(),
                    r, c;
                for (r = 1; r <= cswPrivate.cellSet.rows; r += 1) {
                    for (c = 1; c <= cswPrivate.cellSet.columns; c += 1) {
                        if (cellSet[r] === undefined) {
                            cellSet[r] = Csw.array();
                        }
                        cellSet[r][c] = cswPrivate.getCell(row, column, r, c);
                    }
                }
                return cellSet;
            };

            cswPublic.addCellSetAttributes = function (cellSet, attributes) {

                function applyAttributes(cell) {
                    if (false === Csw.isNullOrEmpty(attributes)) {
                        cell.data(attributes);
                    }
                }

                cswPrivate.eachCell(cellSet, applyAttributes);
            };

            cswPublic.toggleConfig = function () {
                /// <summary>Toggles config mode.</summary>
                /// <returns type="Boolean">The resulting config state.</returns>
                var ret = false;
                if (cswPublic.isConfig()) {
                    cswPublic.configOff();
                } else {
                    ret = true;
                    cswPublic.configOn();
                }
                return ret;
            };

            cswPublic.configOff = function () {
                /// <summary>Turn config mode off on the layout table.</summary>
                /// <returns type="Undefined"></returns>
                cswPrivate.configMode = false;

                if (cswPrivate.addBtn) {
                    cswPrivate.addBtn.hide();
                }
                if (cswPrivate.removeBtn) {
                    if (cswPrivate.isRemoveMode()) {
                        cswPrivate.toggleRemove();
                    }
                    cswPrivate.removeBtn.hide();
                }
                if (cswPrivate.expandColBtn) {
                    cswPrivate.expandColBtn.hide();
                }
                if (cswPrivate.expandRowBtn) {
                    cswPrivate.expandRowBtn.hide();
                }
                if (cswPublic.table.children().length() > 0) {
                    cswPublic.table.findCell('.CswLayoutTable_cell')
                        .removeClass('CswLayoutTable_configcell');

                    cswPrivate.disableDrag();
                }
                cswPublic.table.trigger(cswPrivate.name + 'CswLayoutTable_onConfigOff');
            };

            cswPublic.configOn = function () {
                /// <summary>Turn config mode on, on the layout table. </summary>
                /// <returns type="Undefined"></returns>
                cswPrivate.configMode = true;

                if (cswPrivate.addBtn) {
                    cswPrivate.addBtn.show();
                }
                if (cswPrivate.removeBtn) {
                    cswPrivate.removeBtn.show();
                }
                if (cswPrivate.expandColBtn) {
                    cswPrivate.expandColBtn.show();
                }
                if (cswPrivate.expandRowBtn) {
                    cswPrivate.expandRowBtn.show();
                }
                cswPublic.table.finish(null, cswPrivate.firstRow, cswPrivate.firstCol);

                if (cswPublic.table.children().length() > 0) {
                    cswPublic.table.findCell('.CswLayoutTable_cell')
                        .addClass('CswLayoutTable_configcell');

                    cswPrivate.enableDrag();
                    //cswPrivate.enableDrop();
                }
                cswPublic.table.trigger(cswPrivate.name + 'CswLayoutTable_onConfigOn');
            }; // cswPublic.configOn()

            /* Ctor: Build the init table */
            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                var layoutDiv = cswParent.div();

                //Csw.extend(cswPublic, Csw.literals.div(cswPrivate));
                var buttonDiv = layoutDiv.div();
                buttonDiv.css({ 'float': 'right' });

                if (cswPrivate.ReadOnly) {
                    buttonDiv.hide();
                }

                cswPrivate.firstRow = 1;
                cswPrivate.firstCol = 1;

                cswPublic.table = layoutDiv.table({
                    TableCssClass: cswPrivate.TableCssClass + ' CswLayoutTable_table',
                    CellCssClass: cswPrivate.CellCssClass + ' CswLayoutTable_cell',
                    cellpadding: cswPrivate.cellpadding,
                    cellspacing: cswPrivate.cellspacing,
                    cellalign: cswPrivate.cellalign,
                    OddCellRightAlign: cswPrivate.OddCellRightAlign,
                    width: cswPrivate.width,
                    align: cswPrivate.align,
                    onCreateCell: function (ev, newCell, realrow, realcolumn) {
                        if (false === Csw.isNullOrEmpty(newCell)) {
                            cswPrivate.onCreateCell(newCell, realrow, realcolumn);
                        }
                    },
                    styles: cswPrivate.styles
                });
                cswPublic.table.data({
                    'cellset_rows': cswPrivate.cellSet.rows,
                    'cellset_columns': cswPrivate.cellSet.columns
                });

                //cswPrivate.setConfigMode('false');
                cswPrivate.configMode = false;
                cswPublic.table.bind(cswPrivate.name + 'CswLayoutTable_onSwap', cswPrivate.onSwap);
                cswPublic.table.bind(cswPrivate.name + 'CswLayoutTable_onRemove', cswPrivate.onRemove);
                cswPublic.table.bind(cswPrivate.name + 'CswLayoutTable_onConfigOn', cswPrivate.onConfigOn);
                cswPublic.table.bind(cswPrivate.name + 'CswLayoutTable_onConfigOff', cswPrivate.onConfigOff);

                cswPublic.buttonTable = buttonDiv.table();
                if (cswPrivate.showAddButton) {
                    cswPrivate.addBtn = cswPublic.buttonTable.cell(1, 1).icon({
                        name: cswPrivate.name + 'addbtn',
                        hovertext: 'Add',
                        iconType: Csw.enums.iconType.plus,
                        isButton: true,
                        onClick: function () {
                            Csw.tryExec(cswPrivate.onAddClick);
                        },
                        size: 18
                    });

                    cswPrivate.addBtn.hide();
                }
                if (cswPrivate.showRemoveButton) {
                    cswPrivate.removeBtn = cswPublic.buttonTable.cell(1, 2).icon({
                        name: cswPrivate.name + 'rembtn',
                        hovertext: 'Remove',
                        iconType: Csw.enums.iconType.trash,
                        isButton: true,
                        onClick: function () {
                            cswPrivate.toggleRemove();
                        },
                        size: 18
                    });
                    cswPrivate.removeBtn.hide();
                }
                if (cswPrivate.showExpandColButton) {
                    cswPrivate.expandColBtn = cswPublic.buttonTable.cell(1, 3).icon({
                        name: cswPrivate.name + 'addcolumnbtn',
                        hovertext: 'Add Column',
                        iconType: Csw.enums.iconType.right,
                        isButton: true,
                        onClick: function () {
                            cswPrivate.addColumn();
                        },
                        size: 18
                    });
                    cswPrivate.expandColBtn.hide();
                }
                if (cswPrivate.showExpandRowButton) {
                    cswPrivate.expandRowBtn = cswPublic.buttonTable.cell(1, 4).icon({
                        name: cswPrivate.name + 'addrowbtn',
                        hovertext: 'Add Row',
                        iconType: Csw.enums.iconType.down,
                        isButton: true,
                        onClick: function () {
                            cswPrivate.addRow();
                        },
                        size: 18
                    });
                    cswPrivate.expandRowBtn.hide();
                }
                if (cswPrivate.showConfigButton) {
                    cswPrivate.configBtn = cswPublic.buttonTable.cell(1, 5).icon({
                        name: cswPrivate.name + 'configbtn',
                        hovertext: 'Configure',
                        iconType: Csw.enums.iconType.wrench,
                        isButton: true,
                        onClick: function () {
                            cswPublic.toggleConfig();
                        },
                        size: 18
                    });
                }

            } ());

            return cswPublic;
        });


} ());

