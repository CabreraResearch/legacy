/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function _cswLayoutTable() {
    'use strict';
    function layoutTable(options) {

        var internal = {
            $parent: '',
            ID: '',
            cellset: { rows: 1, columns: 1 },
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
            var $removecells;
            if (internal.isRemoveMode(external.table)) {
                $removecells = external.table.find('.CswLayoutTable_remove');
                if ($removecells.length > 0) {
                    external.table.trigger(internal.tableId + 'CswLayoutTable_onRemove', {
                        table: external.table,
                        cellset: external.cellSet(external.table, row, column),
                        row: $removecells.CswAttrNonDom('row'),
                        column: $removecells.CswAttrNonDom('column')
                    });
                }
                $removecells.children().hide();

                $removecells.removeClass('CswLayoutTable_remove');
            } // if(internal.isRemoveMode($external.table))
        }; // internal.onClick()

        internal.isRemoveMode = function () {
            return (external.table.propNonDom('removemode') === "true");
        };

        internal.setConfigMode = function (mode) {
            external.table.propNonDom('configmode', mode);
        };

        internal.toggleRemove = function ($rembtn) {
            if (internal.isRemoveMode(external.table)) {
                internal.removeOff(external.table, $rembtn);
            } else {
                external.table.propNonDom('removemode', 'true');
                $rembtn.addClass('CswLayoutTable_removeEnabled');
            }
        };

        internal.removeOff = function ($rembtn) {
            external.table.propNonDom('removemode', 'false');
            $rembtn.removeClass('CswLayoutTable_removeEnabled');
        };

        internal.addRow = function () {
            var cellsetrows = Csw.number(external.table.propNonDom('cellset_rows')),
                cellsetcolumns = Csw.number(external.table.propNonDom('cellset_columns')),
                tablemaxrows = external.table.maxrows(),
                tablemaxcolumns = external.table.maxcolumns();

            // add a row and column
            internal.getCell(external.table, (tablemaxrows / cellsetrows) + 1, (tablemaxcolumns / cellsetcolumns), cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
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
            internal.getCell(external.table, (tablemaxrows / cellsetrows), (tablemaxcolumns / cellsetcolumns) + 1, cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
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

            cell.$.click(function (ev, dd) {
                internal.onClick(ev, dd, row, column, cellsetrows, cellsetcolumns);
            })
                .droppable({
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

            cell.append('<div class="CswLayoutTable_celldiv"></div>');
        };

        internal.enableDrag = function () {
            external.table.find('.CswLayoutTable_celldiv')
                .draggable({
                    revert: "invalid",
                    drag: function (ev, dd) {
                        internal.onDrag(ev, dd, $(this));
                    }
                });
        };

        internal.disableDrag = function () {
            external.table.find('.CswLayoutTable_celldiv')
                .draggable('destroy');
        };

        internal.onHoverIn = function (ev, dd, $cell) {
            var $cellset;
            if (internal.isRemoveMode()) {
                $cellset = external.table.findCell('[row="' + $cell.CswAttrNonDom('row') + '"][column="' + $cell.CswAttrNonDom('column') + '"]');
                $cellset.addClass('CswLayoutTable_remove');
            }
        }; // internal.onHoverIn()

        internal.onHoverOut = function (ev, dd, $cell) {
            var $cellset;
            if (internal.isRemoveMode()) {
                $cellset = external.table.findCell('[row="' + $cell.CswAttrNonDom('row') + '"][column="' + $cell.CswAttrNonDom('column') + '"]');
                $cellset.removeClass('CswLayoutTable_remove');
            }
        }; // internal.onHoverOut()

        internal.onDrag = function (ev, dd, $dragdiv) {
            var $dragcell, $cells;
            if (external.isConfig(external.table)) {
                $dragcell = $dragdiv.parent();
                $cells = external.table.findCell('[row="' + $dragcell.CswAttrNonDom('row') + '"][column="' + $dragcell.CswAttrNonDom('column') + '"]');
                $cells.addClass('CswLayoutTable_dragcell');
            }
        }; // internal.onDrag

        internal.onDrop = function (ev, dd, dropCell, cellsetrows, cellsetcolumns) {
            var $dragdiv, dragCell, dragCells, dropCells, r, c, $thisdragcell, $thisdropcell, $thisdragdiv, $thisdropdiv;
            if (external.isConfig(external.table)) {
                $dragdiv = dd.draggable;
                dragCell = Csw.controls.domExtend($dragdiv.parent(), {});

                dragCells = external.table.findCell('[row="' + dragCell.propNonDom('row') + '"][column="' + dragCell.propNonDom('column') + '"]');
                dropCells = external.table.findCell('[row="' + dropCell.propNonDom('row') + '"][column="' + dropCell.propNonDom('column') + '"]');

                dragCells.removeClass('CswLayoutTable_dragcell');

                // This must happen BEFORE we do the swap, in case the caller relies on the contents of the div being where it was
                external.table.trigger(internal.tableId + 'CswLayoutTable_onSwap', {
                    table: external.table,
                    cellset: external.cellSet(external.table, dragCell.propNonDom('row'), dragCell.propNonDom('column')),
                    swapcellset: external.cellSet(external.table, dropCells.$.first().attr('row'), dropCells.$.first().attr('column')),
                    row: dragCell.propNonDom('row'),
                    column: dragCell.propNonDom('column'),
                    swaprow: dropCells.$.first().attr('row'),
                    swapcolumn: dropCells.$.first().attr('column')
                });

                for (r = 1; r <= cellsetrows; r += 1) {
                    for (c = 1; c <= cellsetcolumns; c += 1) {
                        $thisdragcell = dragCells.filter('[cellsetrow="' + r + '"][cellsetcolumn="' + c + '"]');
                        $thisdropcell = dropCells.filter('[cellsetrow="' + r + '"][cellsetcolumn="' + c + '"]');
                        $thisdragdiv = $thisdragcell.children('div');
                        $thisdropdiv = $thisdropcell.children('div');

                        $thisdragcell.append($thisdropdiv);
                        $thisdropcell.append($thisdragdiv);

                        $thisdragdiv.position({
                            my: "left top",
                            at: "left top",
                            of: $thisdropcell,
                            offset: external.table.propNonDom('cellpadding')
                        });

                        $thisdropdiv.position({
                            my: "left top",
                            at: "left top",
                            of: $thisdragcell,
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
                cellset = Csw.array(),
                r, c;
            for (r = 1; r <= cellsetrows; r += 1) {
                for (c = 1; c <= cellsetcolumns; c += 1) {
                    if (cellset[r] === undefined) {
                        cellset[r] = Csw.array();
                    }
                    cellset[r][c] = internal.getCell(row, column, r, c, cellsetrows, cellsetcolumns);
                }
            }
            return cellset;
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
            var $rembtn;

            external.buttonTable.find('#' + internal.tableId + 'addbtn').hide();
            $rembtn = external.buttonTable.find('#' + internal.tableId + 'rembtn');
            $rembtn.hide();
            external.buttonTable.find('#' + internal.tableId + 'addcolumnbtn').hide();
            external.buttonTable.find('#' + internal.tableId + 'addrowbtn').hide();

            external.table.findCell('.CswLayoutTable_cell')
                .removeClass('CswLayoutTable_configcell');

            internal.disableDrag();

            internal.setConfigMode('false');
            external.table.trigger(internal.tableId + 'CswLayoutTable_onConfigOff');
            internal.removeOff($rembtn);
        };

        external.configOn = function () {
            /// <summary>Turn config mode on, on the layout table. </summary>
            /// <returns type="Undefined"></returns>
            external.buttonTable.find('#' + internal.tableId + 'addbtn').show();
            external.buttonTable.find('#' + internal.tableId + 'rembtn').show();
            external.buttonTable.find('#' + internal.tableId + 'addcolumnbtn').show();
            external.buttonTable.find('#' + internal.tableId + 'addrowbtn').show();

            external.table.finish(null);

            external.table.findCell('.CswLayoutTable_cell')
                .addClass('CswLayoutTable_configcell');

            internal.enableDrag();

            internal.setConfigMode('true');
            external.table.trigger(internal.tableId + 'CswLayoutTable_onConfigOn');
        }; // external.configOn()

        /* Ctor: Build the init table */
        (function () {
            if (options) {
                $.extend(internal, options);
            }

            var $buttondiv = $('<div />')
                .appendTo(internal.$parent)
                .css({ 'float': 'right' });

            if (internal.ReadOnly) {
                $buttondiv.hide();
            }

            internal.tableId = internal.ID;
            internal.buttonTableId = Csw.controls.dom.makeId(internal.ID, 'buttontbl');

            external.table = Csw.controls.table({
                $parent: internal.$parent,
                ID: internal.ID,
                TableCssClass: internal.TableCssClass + ' CswLayoutTable_table',
                CellCssClass: internal.CellCssClass + ' CswLayoutTable_cell',
                cellpadding: internal.cellpadding,
                cellspacing: internal.cellspacing,
                cellalign: internal.cellalign,
                OddCellRightAlign: internal.OddCellRightAlign,
                width: internal.width,
                align: internal.align,
                onCreateCell: function (ev, $newcell, realrow, realcolumn) {
                    internal.onCreateCell($newcell, realrow, realcolumn, internal.cellset.rows, internal.cellset.columns);
                }
            });
            external.table.propNonDom({
                'cellset_rows': internal.cellset.rows,
                'cellset_columns': internal.cellset.columns
            });

            internal.setConfigMode(external.table, 'false');
            external.table.bind(internal.ID + 'CswLayoutTable_onSwap', internal.onSwap);
            external.table.bind(internal.ID + 'CswLayoutTable_onRemove', internal.onRemove);
            external.table.bind(internal.ID + 'CswLayoutTable_onConfigOn', internal.onConfigOn);
            external.table.bind(internal.ID + 'CswLayoutTable_onConfigOff', internal.onConfigOff);

            external.buttonTable = Csw.controls.table({
                $parent: $buttondiv,
                ID: Csw.controls.dom.makeId(internal.ID, 'buttontbl')
            });
            if (internal.showAddButton) {
                external.buttonTable.cell(1, 1).$.CswImageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.Add,
                    AlternateText: 'Add',
                    ID: internal.ID + 'addbtn',
                    onClick: function () {
                        Csw.tryExec(internal.onAddClick);
                        return Csw.enums.imageButton_ButtonType.None;
                    }
                }).hide();
            }
            if (internal.showRemoveButton) {
                external.buttonTable.cell(1, 2).$.CswImageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                    AlternateText: 'Remove',
                    ID: internal.ID + 'rembtn',
                    onClick: function ($ImageDiv) {
                        internal.toggleRemove($ImageDiv);
                        return Csw.enums.imageButton_ButtonType.None;
                    }
                }).hide();
            }
            if (internal.showExpandColButton) {
                external.buttonTable.cell(1, 3).$.CswImageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.ArrowEast,
                    AlternateText: 'Add Column',
                    ID: internal.ID + 'addcolumnbtn',
                    onClick: function () {
                        internal.addColumn();
                        return Csw.enums.imageButton_ButtonType.None;
                    }
                }).hide();
            }
            if (internal.showExpandRowButton) {
                external.buttonTable.cell(1, 4).$.CswImageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.ArrowSouth,
                    AlternateText: 'Add Row',
                    ID: internal.ID + 'addrowbtn',
                    onClick: function () {
                        internal.addRow();
                        return Csw.enums.imageButton_ButtonType.None;
                    }
                }).hide();
            }
            if (internal.showConfigButton) {
                external.buttonTable.cell(1, 5).$.CswImageButton({
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

