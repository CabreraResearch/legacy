/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswLayoutTable() {
    'use strict';
    function layoutTable(options) {

        var internal = {
            $parent: '',
            ID: '',
            cellSet: { rows: 1, columns: 1 },
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
        var external = {};

        internal.onClick = function (ev, dd, row, column) {
            var removeCells;
            if (internal.isRemoveMode(external.table)) {
                removeCells = external.table.find('.CswLayoutTable_remove');
                if (removeCells.length() > 0) {
                    external.table.trigger(internal.ID + 'CswLayoutTable_onRemove', {
                        table: external.table,
                        cellSet: external.cellSet(row, column),
                        row: removeCells.propNonDom('row'),
                        column: removeCells.propNonDom('column')
                    });
                }
                removeCells.children().hide();

                removeCells.removeClass('CswLayoutTable_remove');
            } // if(internal.isRemoveMode($external.table))
        }; // internal.onClick()

        internal.isRemoveMode = function () {
            return (external.table.propNonDom('removemode') === "true");
        };

        internal.setConfigMode = function (mode) {
            external.table.propNonDom('configmode', mode);
        };

        internal.toggleRemove = function () {
            if (internal.isRemoveMode(external.table)) {
                external.table.propNonDom('removemode', 'false');
                if (internal.removeBtn) {
                    internal.removeBtn.removeClass('CswLayoutTable_removeEnabled');
                }
            } else {
                external.table.propNonDom('removemode', 'true');
                if (internal.removeBtn) {
                    internal.removeBtn.addClass('CswLayoutTable_removeEnabled');
                }
            }
        };

        internal.addRow = function () {
            var cellsetrows = Csw.number(external.table.propNonDom('cellset_rows')),
                cellsetcolumns = Csw.number(external.table.propNonDom('cellset_columns')),
                tablemaxrows = external.table.maxrows(),
                tablemaxcolumns = external.table.maxcolumns();

            // add a row and column
            internal.getCell((tablemaxrows / cellsetrows) + 1, (tablemaxcolumns / cellsetcolumns), cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
            external.table.finish(null);

            if (external.isConfig()) {
                external.table.findCell('.CswLayoutTable_cell')
                    .addClass('CswLayoutTable_configcell');
            }
        }; // _addRowAndColumn()

        internal.addColumn = function () {
            var cellsetrows = Csw.number(external.table.propNonDom('cellset_rows')),
                cellsetcolumns = Csw.number(external.table.propNonDom('cellset_columns')),
                tablemaxrows = external.table.maxrows(),
                tablemaxcolumns = external.table.maxcolumns();

            // add a row and column
            internal.getCell((tablemaxrows / cellsetrows), (tablemaxcolumns / cellsetcolumns) + 1, cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
            external.table.finish(null);

            if (external.isConfig()) {
                external.table.findCell('.CswLayoutTable_cell')
                    .addClass('CswLayoutTable_configcell');
            }
        }; // internal.addColumn()

        internal.getCell = function (row, column, cellsetrow, cellsetcolumn, cellsetrows, cellsetcolumns) {
            var realrow, realcolumn, cell;
            if (row < 1) {
                row = 1;
            }
            if (column < 1) {
                column = 1;
            }
            realrow = ((row - 1) * cellsetrows) + cellsetrow;
            realcolumn = ((column - 1) * cellsetcolumns) + cellsetcolumn;
            cell = external.table.cell(realrow, realcolumn);
            return cell;
        };

        internal.onCreateCell = function (cell, realrow, realcolumn, cellsetrows, cellsetcolumns) {
            var row = Math.ceil(realrow / cellsetrows),
                column = Math.ceil(realcolumn / cellsetcolumns),
                cellsetrow = Csw.number(cellsetrows - realrow % cellsetrows),
                cellsetcolumn = Csw.number(cellsetcolumns - realcolumn % cellsetcolumns);

            cell.propNonDom({
                row: row,
                column: column,
                cellsetrow: cellsetrow,
                cellsetcolumn: cellsetcolumn
            });

            cell.bind('click', function (ev, dd) {
                internal.onClick(ev, dd, row, column, cellsetrows, cellsetcolumns);
            }).$.droppable({
                hoverClass: 'CswLayoutTable_hover',
                drop: function (ev, dd) {
                    internal.onDrop(ev, dd, $(this), cellsetrows, cellsetcolumns);
                }
            })
                .hover(function (ev, dd) {
                    internal.onHoverIn(ev, dd, $(this));
                }, function (ev, dd) {
                    internal.onHoverOut(ev, dd, $(this));
                });

            cell.div({ cssclass: 'CswLayoutTable_celldiv' });
        };

        internal.enableDrag = function () {
            external.table.find('.CswLayoutTable_celldiv')
                .$.draggable({
                    revert: 'invalid',
                    drag: function (ev, dd) {
                        internal.onDrag(ev, dd, $(this));
                    }
                });
        };

        internal.disableDrag = function () {
            external.table.find('.CswLayoutTable_celldiv')
                .$.draggable('destroy');
        };

        internal.onHoverIn = function (ev, dd, $cell) {
            var cellSet;
            if (internal.isRemoveMode()) {
                cellSet = external.table.findCell('[row="' + $cell.attr('row') + '"][column="' + $cell.attr('column') + '"]');
                cellSet.addClass('CswLayoutTable_remove');
            }
        }; // internal.onHoverIn()

        internal.onHoverOut = function (ev, dd, $cell) {
            var cellSet;
            if (internal.isRemoveMode()) {
                cellSet = external.table.findCell('[row="' + $cell.attr('row') + '"][column="' + $cell.attr('column') + '"]');
                cellSet.removeClass('CswLayoutTable_remove');
            }
        }; // internal.onHoverOut()

        internal.onDrag = function (ev, dd, $dragDiv) {
            var $dragCell, cells;
            if (external.isConfig(external.table)) {
                $dragCell = $dragDiv.parent();
                cells = external.table.findCell('[row="' + $dragCell.attr('row') + '"][column="' + $dragCell.attr('column') + '"]');
                cells.addClass('CswLayoutTable_dragcell');
            }
        }; // internal.onDrag

        internal.onDrop = function (ev, dd, $dropCell, cellsetrows, cellsetcolumns) {
            var dragDiv, dragCell, dragCells, dropCells, r, c, thisDragCell, thisDropCell, thisDragDiv, thisDropDiv;
            if (external.isConfig(external.table)) {
                dragDiv = dd.draggable;
                dragCell = Csw.controls.factory(dragDiv.parent(), {});

                dragCells = external.table.findCell('[row="' + dragCell.propNonDom('row') + '"][column="' + dragCell.propNonDom('column') + '"]');
                dropCells = external.table.findCell('[row="' + $dropCell.attr('row') + '"][column="' + $dropCell.attr('column') + '"]');

                dragCells.removeClass('CswLayoutTable_dragcell');

                // This must happen BEFORE we do the swap, in case the caller relies on the contents of the div being where it was
                external.table.trigger(internal.ID + 'CswLayoutTable_onSwap', {
                    table: external.table,
                    cellSet: external.cellSet(dragCell.first().propNonDom('row'), dragCell.first().propNonDom('column')),
                    swapcellset: external.cellSet(dropCells.first().propNonDom('row'), dropCells.first().propNonDom('column')),
                    row: dragCell.propNonDom('row'),
                    column: dragCell.propNonDom('column'),
                    swaprow: dropCells.first().propNonDom('row'),
                    swapcolumn: dropCells.first().propNonDom('column')
                });

                for (r = 1; r <= cellsetrows; r += 1) {
                    for (c = 1; c <= cellsetcolumns; c += 1) {
                        thisDragCell = dragCells.filter('[cellsetrow="' + r + '"][cellsetcolumn="' + c + '"]');
                        thisDropCell = dropCells.filter('[cellsetrow="' + r + '"][cellsetcolumn="' + c + '"]');
                        thisDragDiv = thisDragCell.children('div');
                        thisDropDiv = thisDropCell.children('div');

                        thisDragCell.append(thisDropDiv);
                        thisDropCell.append(thisDragDiv);

                        thisDragDiv.$.position({
                            my: 'left top',
                            at: 'left top',
                            of: thisDropCell.$,
                            offset: external.table.propNonDom('cellpadding')
                        });

                        thisDropDiv.$.position({
                            my: "left top",
                            at: "left top",
                            of: thisDragCell.$,
                            offset: external.table.propNonDom('cellpadding')
                        });

                    } // for(c = 1; c <= cellsetcolumns; c++)
                } // for(r = 1; r <= cellsetrows; r++)
            } // if(external.isConfig($external.table))
        }; // internal.onDrop()

        external.isConfig = function () {
            /// <summary>Determines whether layout table is in config mode.</summary>
            /// <returns type="Boolean">True if config mode is on.</returns>
            return Csw.bool(external.table.propNonDom('configmode'));
        };

        external.cellSet = function (row, column) {
            /// <summary>Get the set of cells representing a layout item.</summary>
            /// <param name="row" type="Number">Row number</param>
            /// <param name="column" type="Number">Column number</param>
            /// <returns type="Array">An array of the content.</returns>
            var cellsetrows = Csw.number(external.table.propNonDom('cellset_rows')),
                cellsetcolumns = Csw.number(external.table.propNonDom('cellset_columns')),
                cellSet = Csw.array(),
                r, c;
            for (r = 1; r <= cellsetrows; r += 1) {
                for (c = 1; c <= cellsetcolumns; c += 1) {
                    if (cellSet[r] === undefined) {
                        cellSet[r] = Csw.array();
                    }
                    cellSet[r][c] = internal.getCell(row, column, r, c, cellsetrows, cellsetcolumns);
                }
            }
            return cellSet;
        };

        external.toggleConfig = function () {
            /// <summary>Toggles config mode.</summary>
            /// <returns type="Boolean">The resulting config state.</returns>
            var ret = false;
            if (external.isConfig(external.table)) {
                external.configOff();
            } else {
                ret = true;
                external.configOn();
            }
            return ret;
        };

        external.configOff = function () {
            /// <summary>Turn config mode off on the layout table.</summary>
            /// <returns type="Undefined"></returns>
            if (internal.addBtn) {
                internal.addBtn.hide();
            }
            if (internal.removeBtn) {
                internal.removeBtn.hide();
            }
            if (internal.expandColBtn) {
                internal.expandColBtn.hide();
            }
            if (internal.expandRowBtn) {
                internal.expandRowBtn.hide();
            }
            external.table.findCell('.CswLayoutTable_cell')
                .removeClass('CswLayoutTable_configcell');

            internal.disableDrag();

            internal.setConfigMode('false');
            external.table.trigger(internal.ID + 'CswLayoutTable_onConfigOff');
            internal.removeOff();
        };

        external.configOn = function () {
            /// <summary>Turn config mode on, on the layout table. </summary>
            /// <returns type="Undefined"></returns>
            if (internal.addBtn) {
                internal.addBtn.show();
            }
            if (internal.removeBtn) {
                internal.removeBtn.show();
            }
            if (internal.expandColBtn) {
                internal.expandColBtn.show();
            }
            if (internal.expandRowBtn) {
                internal.expandRowBtn.show();
            }
            external.table.finish(null);

            external.table.findCell('.CswLayoutTable_cell')
                .addClass('CswLayoutTable_configcell');

            internal.enableDrag();

            internal.setConfigMode('true');
            external.table.trigger(internal.ID + 'CswLayoutTable_onConfigOn');
        }; // external.configOn()

        /* Ctor: Build the init table */
        (function () {
            if (options) {
                $.extend(internal, options);
            }
            $.extend(external, Csw.controls.div(internal));
            var buttonDiv = external.div({
                ID: Csw.controls.dom.makeId(internal.ID, 'btnDiv')
            });
            buttonDiv.css({ 'float': 'right' });

            if (internal.ReadOnly) {
                buttonDiv.hide();
            }

            internal.tableId = Csw.controls.dom.makeId(internal.ID, 'tbl');
            internal.buttonTableId = Csw.controls.dom.makeId(internal.ID, 'buttontbl');

            external.table = external.table({
                ID: internal.tableId,
                TableCssClass: internal.TableCssClass + ' CswLayoutTable_table',
                CellCssClass: internal.CellCssClass + ' CswLayoutTable_cell',
                cellpadding: internal.cellpadding,
                cellspacing: internal.cellspacing,
                cellalign: internal.cellalign,
                OddCellRightAlign: internal.OddCellRightAlign,
                width: internal.width,
                align: internal.align,
                onCreateCell: function (ev, newCell, realrow, realcolumn) {
                    if (newCell.isValid) {
                        internal.onCreateCell(newCell, realrow, realcolumn, internal.cellSet.rows, internal.cellSet.columns);
                    }
                }
            });
            external.table.propNonDom({
                'cellset_rows': internal.cellSet.rows,
                'cellset_columns': internal.cellSet.columns
            });

            internal.setConfigMode(external.table, 'false');
            external.table.bind(internal.ID + 'CswLayoutTable_onSwap', internal.onSwap);
            external.table.bind(internal.ID + 'CswLayoutTable_onRemove', internal.onRemove);
            external.table.bind(internal.ID + 'CswLayoutTable_onConfigOn', internal.onConfigOn);
            external.table.bind(internal.ID + 'CswLayoutTable_onConfigOff', internal.onConfigOff);

            external.buttonTable = buttonDiv.table({
                ID: Csw.controls.dom.makeId(internal.ID, 'buttontbl')
            });
            if (internal.showAddButton) {
                internal.addBtn = external.buttonTable.cell(1, 1).imageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.Add,
                    AlternateText: 'Add',
                    ID: internal.ID + 'addbtn',
                    onClick: function () {
                        Csw.tryExec(internal.onAddClick);
                        return Csw.enums.imageButton_ButtonType.None;
                    }
                });
                internal.addBtn.hide();
            }
            if (internal.showRemoveButton) {
                internal.removeBtn = external.buttonTable.cell(1, 2).imageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                    AlternateText: 'Remove',
                    ID: internal.ID + 'rembtn',
                    onClick: function () {
                        internal.toggleRemove();
                        return Csw.enums.imageButton_ButtonType.None;
                    }
                });
                internal.removeBtn.hide();
            }
            if (internal.showExpandColButton) {
                internal.expandColBtn = external.buttonTable.cell(1, 3).imageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.ArrowEast,
                    AlternateText: 'Add Column',
                    ID: internal.ID + 'addcolumnbtn',
                    onClick: function () {
                        internal.addColumn();
                        return Csw.enums.imageButton_ButtonType.None;
                    }
                });
                internal.expandColBtn.hide();
            }
            if (internal.showExpandRowButton) {
                internal.expandRowBtn = external.buttonTable.cell(1, 4).imageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.ArrowSouth,
                    AlternateText: 'Add Row',
                    ID: internal.ID + 'addrowbtn',
                    onClick: function () {
                        internal.addRow();
                        return Csw.enums.imageButton_ButtonType.None;
                    }
                });
                internal.expandRowBtn.hide();
            }
            if (internal.showConfigButton) {
                internal.configBtn = external.buttonTable.cell(1, 5).imageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.Configure,
                    AlternateText: 'Configure',
                    ID: internal.ID + 'configbtn',
                    onClick: function () {
                        external.toggleConfig();
                        return Csw.enums.imageButton_ButtonType.None;
                    }
                });
            }

        } ());

        return external;
    }

    Csw.controls.register('layoutTable', layoutTable);
    Csw.controls.layoutTable = Csw.controls.layoutTable || layoutTable;

} ());

