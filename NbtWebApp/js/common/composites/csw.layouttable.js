/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.composites.layoutTable = Csw.composites.layoutTable ||
        Csw.composites.register('layoutTable', function(cswParent, options) {
            'use strict';
            var cswPrivateVar = {
                ID: '',
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
                width: '',
                align: '',
                showConfigButton: false,
                showAddButton: false,
                showRemoveButton: false,
                showExpandRowButton: false,
                showExpandColButton: false,
                OddCellRightAlign: false,
                ReadOnly: false
            };
            var cswPublicRet = { };

            cswPrivateVar.onClick = function(ev, dd, row, column) {
                var removeCells;
                if (cswPrivateVar.isRemoveMode(cswPublicRet.table)) {
                    removeCells = cswPublicRet.table.find('.CswLayoutTable_remove');
                    if (removeCells.length() > 0) {
                        cswPublicRet.table.trigger(cswPrivateVar.ID + 'CswLayoutTable_onRemove', {
                            table: cswPublicRet.table,
                            cellSet: cswPublicRet.cellSet(row, column),
                            row: removeCells.propNonDom('row'),
                            column: removeCells.propNonDom('column')
                        });
                        removeCells.children().hide();

                        removeCells.removeClass('CswLayoutTable_remove');
                    }

                } // if(cswPrivateVar.isRemoveMode($cswPublicRet.table))
            }; // cswPrivateVar.onClick()

            cswPrivateVar.isRemoveMode = function() {
                return (cswPublicRet.table.propNonDom('removemode') === "true");
            };

            cswPrivateVar.setConfigMode = function(mode) {
                cswPublicRet.table.propNonDom('configmode', mode);
            };

            cswPrivateVar.toggleRemove = function() {
                if (cswPrivateVar.isRemoveMode(cswPublicRet.table)) {
                    cswPublicRet.table.propNonDom('removemode', 'false');
                    if (cswPrivateVar.removeBtn) {
                        cswPrivateVar.removeBtn.removeClass('CswLayoutTable_removeEnabled');
                    }
                } else {
                    cswPublicRet.table.propNonDom('removemode', 'true');
                    if (cswPrivateVar.removeBtn) {
                        cswPrivateVar.removeBtn.addClass('CswLayoutTable_removeEnabled');
                    }
                }
            };

            cswPrivateVar.expandLayoutTable = function(expRow, expCol) {
                /// <summary>Expand the layout table by an additional cellSet row or column or both.</summary>
                /// <param name="expRow" type="Boolean">True to expand by a cellSet row.</param>
                /// <param name="expCol" type="Boolean">True to expand by a cellSet column.</param>
                /// <returns type="undefined"></returns>
                var tablemaxrows = cswPublicRet.table.maxrows(),
                    tablemaxcolumns = cswPublicRet.table.maxcolumns(),
                    rowCount = 0, colCount = 0,
                    requestRow, requestCol;
                if (expRow) {
                    rowCount = cswPrivateVar.cellSet.rows;
                }
                if (expCol) {
                    colCount = cswPrivateVar.cellSet.columns;
                }
                if (rowCount > 0 || colCount > 0) {
                    requestRow = tablemaxrows + rowCount,
                    requestCol = tablemaxcolumns + colCount;

                    cswPublicRet.table.cell(requestRow, requestCol);
                    cswPublicRet.table.finish(null, cswPrivateVar.firstRow, cswPrivateVar.firstCol);

                    if (cswPublicRet.isConfig()) {
                        cswPublicRet.table.findCell('.CswLayoutTable_cell')
                            .addClass('CswLayoutTable_configcell');
                    }
                }
            };

            cswPrivateVar.addRow = function() {
                cswPrivateVar.expandLayoutTable(true, false);
            }; // _addRowAndColumn()

            cswPrivateVar.addColumn = function() {
                cswPrivateVar.expandLayoutTable(false, true);
            }; // cswPrivateVar.addColumn()

            cswPrivateVar.getCell = function(getRow, getColumn, cellsetrow, cellsetcolumn) {
                var row = Csw.number(getRow),
                    column = Csw.number(getColumn),
                    realrow, realcolumn, cell;
                if (row < 1) {
                    row = 1;
                }
                if (column < 1) {
                    column = 1;
                }
                realrow = ((row - 1) * cswPrivateVar.cellSet.rows) + Csw.number(cellsetrow, 1);
                realcolumn = ((column - 1) * cswPrivateVar.cellSet.columns) + Csw.number(cellsetcolumn, 1);

                cell = cswPublicRet.table.cell(realrow, realcolumn);

                return cell;
            };

            cswPrivateVar.enableDrop = function(cell) {
                var cellObj;
                if (cell) {
                    cellObj = cell;
                } else {
                    cellObj = cswPublicRet.table.find('.CswLayoutTable_celldiv');
                }
                cellObj.$.droppable('destroy');
                cellObj.$.droppable({
                    hoverClass: 'CswLayoutTable_hover',
                    drop: function(ev, dd) {
                        cswPrivateVar.onDrop(ev, dd, $(this));
                    }
                })
                    .hover(function(ev, dd) {
                        cswPrivateVar.onHoverIn(ev, dd, $(this));
                    }, function(ev, dd) {
                        cswPrivateVar.onHoverOut(ev, dd, $(this));
                    });
            };

            cswPrivateVar.onCreateCell = function(cell, realRow, realCol) {
                var thisRow = Math.ceil(realRow / cswPrivateVar.cellSet.rows),
                    thisCol = Math.ceil(realCol / cswPrivateVar.cellSet.columns),
                    cellsetrow = Csw.number(cswPrivateVar.cellSet.rows - realRow % cswPrivateVar.cellSet.rows),
                    cellsetcolumn = Csw.number(cswPrivateVar.cellSet.columns - realCol % cswPrivateVar.cellSet.columns);

                if (null === cswPrivateVar.firstRow || thisRow < cswPrivateVar.firstRow) {
                    cswPrivateVar.firstRow = thisRow;
                }

                if (null === cswPrivateVar.firstCol || thisCol < cswPrivateVar.firstCol) {
                    cswPrivateVar.firstCol = thisCol;
                }

                cell.propNonDom({
                    row: thisRow,
                    column: thisCol,
                    cellsetrow: cellsetrow,
                    cellsetcolumn: cellsetcolumn
                });

                cell.bind('click', function(ev, dd) {
                    cswPrivateVar.onClick(ev, dd, thisRow, thisCol);
                });
                cswPrivateVar.enableDrop(cell);
                cell.div({ cssclass: 'CswLayoutTable_celldiv' });
            };

            cswPrivateVar.enableDrag = function() {
                cswPublicRet.table.find('.CswLayoutTable_celldiv')
                    .$.draggable({
                        revert: 'invalid',
                        drag: function(ev, dd) {
                            cswPrivateVar.onDrag(ev, dd, $(this));
                        }
                    });
            };

            cswPrivateVar.disableDrag = function() {
                cswPublicRet.table.find('.CswLayoutTable_celldiv')
                    .$.draggable('destroy');
            };

            cswPrivateVar.onHoverIn = function(ev, dd, $cell) {
                var cellSet;
                if (cswPrivateVar.isRemoveMode()) {
                    cellSet = cswPublicRet.cellSet($cell.attr('row'), $cell.attr('column'));
                    cswPrivateVar.eachCell(cellSet, function(cell) {
                        cell.addClass('CswLayoutTable_remove');
                    });
                }
            }; // cswPrivateVar.onHoverIn()

            cswPrivateVar.onHoverOut = function(ev, dd, $cell) {
                var cellSet;
                if (cswPrivateVar.isRemoveMode()) {
                    cellSet = cswPublicRet.cellSet($cell.attr('row'), $cell.attr('column'));
                    cswPrivateVar.eachCell(cellSet, function(cell) {
                        cell.removeClass('CswLayoutTable_remove');
                    });
                }
            }; // cswPrivateVar.onHoverOut()

            cswPrivateVar.onDrag = function(ev, dd, $dragDiv) {
                var $dragCell, cells;
                if (cswPublicRet.isConfig(cswPublicRet.table)) {
                    $dragCell = $dragDiv.parent();
                    cells = cswPublicRet.cellSet($dragCell.attr('row'), $dragCell.attr('column'));
                    cswPrivateVar.eachCell(cells, function(cell) {
                        cell.addClass('CswLayoutTable_dragcell');
                    });
                }
            }; // cswPrivateVar.onDrag

            cswPrivateVar.eachCell = function(cellSet, func) {
                for (var r = 1; r <= cswPrivateVar.cellSet.rows; r += 1) {
                    for (var c = 1; c <= cswPrivateVar.cellSet.columns; c += 1) {
                        if (cellSet[r] === undefined) {
                            cellSet[r] = Csw.array();
                        }
                        Csw.tryExec(func, cellSet[r][c], r, c);
                    }
                }
            };

            cswPrivateVar.onDrop = function(ev, dd, $dropCell) {
                var $dragDiv, dragCell, dragCells, dropCells;
                if (cswPublicRet.isConfig(cswPublicRet.table)) {
                    $dragDiv = dd.draggable;
                    dragCell = Csw.literals.factory($dragDiv.parent(), { });

                    dragCells = cswPublicRet.cellSet(dragCell.propNonDom('row'), dragCell.propNonDom('column')); // .table.findCell('[row="' + Csw.number(dragCell.propNonDom('row')) + '"][column="' + Csw.number(dragCell.propNonDom('column')) + '"]');
                    dropCells = cswPublicRet.cellSet($dropCell.attr('row'), $dropCell.attr('column')); //table.findCell('[row="' + Csw.number($dropCell.attr('row')) + '"][column="' + Csw.number($dropCell.attr('column')) + '"]');

                    // This must happen BEFORE we do the swap, in case the caller relies on the contents of the div being where it was
                    cswPublicRet.table.trigger(cswPrivateVar.ID + 'CswLayoutTable_onSwap', {
                        table: cswPublicRet.table,
                        cellSet: dragCells,
                        swapcellset: dropCells,
                        row: dragCell.propNonDom('row'),
                        column: dragCell.propNonDom('column'),
                        swaprow: dropCells[1][1].propNonDom('row'),
                        swapcolumn: dropCells[1][1].propNonDom('column')
                    });


                    for (var r = 1; r <= cswPrivateVar.cellSet.rows; r += 1) {
                        for (var c = 1; c <= cswPrivateVar.cellSet.columns; c += 1) {
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
                                offset: cswPublicRet.table.propNonDom('cellpadding')
                            });

                            $dropCellDiv.position({
                                my: "left top",
                                at: "left top",
                                of: thisDragCell.$,
                                offset: cswPublicRet.table.propNonDom('cellpadding')
                            });
                        }
                    }

                } // if(cswPublicRet.isConfig($cswPublicRet.table))
            }; // cswPrivateVar.onDrop()

            cswPublicRet.isConfig = function() {
                /// <summary>Determines whether layout table is in config mode.</summary>
                /// <returns type="Boolean">True if config mode is on.</returns>
                return Csw.bool(cswPublicRet.table.propNonDom('configmode'));
            };

            cswPublicRet.cellSet = function(row, column) {
                /// <summary>Get the set of cells representing a layout item.</summary>
                /// <param name="row" type="Number">Row number</param>
                /// <param name="column" type="Number">Column number</param>
                /// <returns type="Array">An array of the content.</returns>
                var cellSet = Csw.array(),
                    r, c;
                for (r = 1; r <= cswPrivateVar.cellSet.rows; r += 1) {
                    for (c = 1; c <= cswPrivateVar.cellSet.columns; c += 1) {
                        if (cellSet[r] === undefined) {
                            cellSet[r] = Csw.array();
                        }
                        cellSet[r][c] = cswPrivateVar.getCell(row, column, r, c);
                    }
                }
                return cellSet;
            };

            cswPublicRet.addCellSetAttributes = function(cellSet, attributes) {

                function applyAttributes(cell) {
                    if (false === Csw.isNullOrEmpty(attributes)) {
                        cell.propNonDom(attributes);
                    }
                }

                cswPrivateVar.eachCell(cellSet, applyAttributes);
            };

            cswPublicRet.toggleConfig = function() {
                /// <summary>Toggles config mode.</summary>
                /// <returns type="Boolean">The resulting config state.</returns>
                var ret = false;
                if (cswPublicRet.isConfig(cswPublicRet.table)) {
                    cswPublicRet.configOff();
                } else {
                    ret = true;
                    cswPublicRet.configOn();
                }
                return ret;
            };

            cswPublicRet.configOff = function() {
                /// <summary>Turn config mode off on the layout table.</summary>
                /// <returns type="Undefined"></returns>
                if (cswPrivateVar.addBtn) {
                    cswPrivateVar.addBtn.hide();
                }
                if (cswPrivateVar.removeBtn) {
                    cswPrivateVar.removeBtn.hide();
                }
                if (cswPrivateVar.expandColBtn) {
                    cswPrivateVar.expandColBtn.hide();
                }
                if (cswPrivateVar.expandRowBtn) {
                    cswPrivateVar.expandRowBtn.hide();
                }
                if (cswPublicRet.table.children().length() > 0) {
                    cswPublicRet.table.findCell('.CswLayoutTable_cell')
                        .removeClass('CswLayoutTable_configcell');

                    cswPrivateVar.disableDrag();
                }
                cswPrivateVar.setConfigMode('false');
                cswPublicRet.table.trigger(cswPrivateVar.ID + 'CswLayoutTable_onConfigOff');
                //cswPrivateVar.toggleRemove();
            };

            cswPublicRet.configOn = function() {
                /// <summary>Turn config mode on, on the layout table. </summary>
                /// <returns type="Undefined"></returns>
                if (cswPrivateVar.addBtn) {
                    cswPrivateVar.addBtn.show();
                }
                if (cswPrivateVar.removeBtn) {
                    cswPrivateVar.removeBtn.show();
                }
                if (cswPrivateVar.expandColBtn) {
                    cswPrivateVar.expandColBtn.show();
                }
                if (cswPrivateVar.expandRowBtn) {
                    cswPrivateVar.expandRowBtn.show();
                }
                cswPublicRet.table.finish(null, cswPrivateVar.firstRow, cswPrivateVar.firstCol);

                if (cswPublicRet.table.children().length() > 0) {
                    cswPublicRet.table.findCell('.CswLayoutTable_cell')
                        .addClass('CswLayoutTable_configcell');

                    cswPrivateVar.enableDrag();
                }
                cswPrivateVar.setConfigMode('true');
                cswPublicRet.table.trigger(cswPrivateVar.ID + 'CswLayoutTable_onConfigOn');
            }; // cswPublicRet.configOn()

            /* Ctor: Build the init table */
            (function() {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                var layoutDiv = cswParent.div(cswPrivateVar);
                
                //$.extend(cswPublicRet, Csw.literals.div(cswPrivateVar));
                var buttonDiv = layoutDiv.div({
                    ID: Csw.makeId(cswPrivateVar.ID, 'btnDiv')
                });
                buttonDiv.css({ 'float': 'right' });

                if (cswPrivateVar.ReadOnly) {
                    buttonDiv.hide();
                }

                cswPrivateVar.tableId = Csw.makeId(cswPrivateVar.ID, 'tbl');
                cswPrivateVar.buttonTableId = Csw.makeId(cswPrivateVar.ID, 'buttontbl');
                cswPrivateVar.firstRow = 1;
                cswPrivateVar.firstCol = 1;

                cswPublicRet.table = layoutDiv.table({
                    ID: cswPrivateVar.tableId,
                    TableCssClass: cswPrivateVar.TableCssClass + ' CswLayoutTable_table',
                    CellCssClass: cswPrivateVar.CellCssClass + ' CswLayoutTable_cell',
                    cellpadding: cswPrivateVar.cellpadding,
                    cellspacing: cswPrivateVar.cellspacing,
                    cellalign: cswPrivateVar.cellalign,
                    OddCellRightAlign: cswPrivateVar.OddCellRightAlign,
                    width: cswPrivateVar.width,
                    align: cswPrivateVar.align,
                    onCreateCell: function(ev, newCell, realrow, realcolumn) {
                        if (false === Csw.isNullOrEmpty(newCell)) {
                            cswPrivateVar.onCreateCell(newCell, realrow, realcolumn);
                        }
                    }
                });
                cswPublicRet.table.propNonDom({
                    'cellset_rows': cswPrivateVar.cellSet.rows,
                    'cellset_columns': cswPrivateVar.cellSet.columns
                });

                cswPrivateVar.setConfigMode('false');
                cswPublicRet.table.bind(cswPrivateVar.ID + 'CswLayoutTable_onSwap', cswPrivateVar.onSwap);
                cswPublicRet.table.bind(cswPrivateVar.ID + 'CswLayoutTable_onRemove', cswPrivateVar.onRemove);
                cswPublicRet.table.bind(cswPrivateVar.ID + 'CswLayoutTable_onConfigOn', cswPrivateVar.onConfigOn);
                cswPublicRet.table.bind(cswPrivateVar.ID + 'CswLayoutTable_onConfigOff', cswPrivateVar.onConfigOff);

                cswPublicRet.buttonTable = buttonDiv.table({
                    ID: Csw.makeId(cswPrivateVar.ID, 'buttontbl')
                });
                if (cswPrivateVar.showAddButton) {
                    cswPrivateVar.addBtn = cswPublicRet.buttonTable.cell(1, 1).imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Add,
                        AlternateText: 'Add',
                        ID: cswPrivateVar.ID + 'addbtn',
                        onClick: function() {
                            Csw.tryExec(cswPrivateVar.onAddClick);
                        }
                    });
                    cswPrivateVar.addBtn.hide();
                }
                if (cswPrivateVar.showRemoveButton) {
                    cswPrivateVar.removeBtn = cswPublicRet.buttonTable.cell(1, 2).imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                        AlternateText: 'Remove',
                        ID: cswPrivateVar.ID + 'rembtn',
                        onClick: function() {
                            cswPrivateVar.toggleRemove();
                        }
                    });
                    cswPrivateVar.removeBtn.hide();
                }
                if (cswPrivateVar.showExpandColButton) {
                    cswPrivateVar.expandColBtn = cswPublicRet.buttonTable.cell(1, 3).imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.ArrowEast,
                        AlternateText: 'Add Column',
                        ID: cswPrivateVar.ID + 'addcolumnbtn',
                        onClick: function() {
                            cswPrivateVar.addColumn();
                        }
                    });
                    cswPrivateVar.expandColBtn.hide();
                }
                if (cswPrivateVar.showExpandRowButton) {
                    cswPrivateVar.expandRowBtn = cswPublicRet.buttonTable.cell(1, 4).imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.ArrowSouth,
                        AlternateText: 'Add Row',
                        ID: cswPrivateVar.ID + 'addrowbtn',
                        onClick: function() {
                            cswPrivateVar.addRow();
                        }
                    });
                    cswPrivateVar.expandRowBtn.hide();
                }
                if (cswPrivateVar.showConfigButton) {
                    cswPrivateVar.configBtn = cswPublicRet.buttonTable.cell(1, 5).imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Configure,
                        AlternateText: 'Configure',
                        ID: cswPrivateVar.ID + 'configbtn',
                        onClick: function() {
                            cswPublicRet.toggleConfig();
                        }
                    });
                }

            }());

            return cswPublicRet;
        });


} ());

