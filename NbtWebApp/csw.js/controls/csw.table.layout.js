/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function ($) {
    /* stop-gap until we refactor this class */
    var table, buttontable;
    $.fn.CswLayoutTable = function (method) {
        
        "use strict";
        var PluginName = "CswLayoutTable";

        var methods = {
            'init': function (options) {
                var o = {
                    ID: '',
                    cellset: { rows: 1, columns: 1 },
                    onSwap: function () { },
                    onAddClick: function () { },
                    onConfigOn: function () { },
                    onConfigOff: function () { },
                    onRemove: function () { },
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
                if (options) {
                    $.extend(o, options);
                }
                var $parent = $(this);

                var $buttondiv = $('<div />')
                                            .appendTo($parent)
                                            .css({ 'float': 'right' });

                if (o.ReadOnly) $buttondiv.hide();

                table = Csw.controls.table({
                    $parent: $parent,
                    ID: o.ID,
                    TableCssClass: o.TableCssClass + ' CswLayoutTable_table',
                    CellCssClass: o.CellCssClass + ' CswLayoutTable_cell',
                    cellpadding: o.cellpadding,
                    cellspacing: o.cellspacing,
                    cellalign: o.cellalign,
                    OddCellRightAlign: o.OddCellRightAlign,
                    width: o.width,
                    align: o.align,
                    onCreateCell: function (ev, $newcell, realrow, realcolumn) {
                        onCreateCell($newcell, realrow, realcolumn, o.cellset.rows, o.cellset.columns);
                    }
                });
                table.propNonDom({
                    'cellset_rows': o.cellset.rows,
                    'cellset_columns': o.cellset.columns
                });

                setConfigMode(table, 'false');
                table.bind(o.ID + 'CswLayoutTable_onSwap', o.onSwap);
                table.bind(o.ID + 'CswLayoutTable_onRemove', o.onRemove);
                table.bind(o.ID + 'CswLayoutTable_onConfigOn', o.onConfigOn);
                table.bind(o.ID + 'CswLayoutTable_onConfigOff', o.onConfigOff);

                buttontable = Csw.controls.table({
                    $parent: $buttondiv,
                    ID: Csw.controls.dom.makeId(o.ID, 'buttontbl')
                });
                if (o.showAddButton) {
                    buttontable.cell(1, 1).CswImageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Add,
                        AlternateText: 'Add',
                        ID: o.ID + 'addbtn',
                        onClick: function () {
                            o.onAddClick();
                            return Csw.enums.imageButton_ButtonType.None;
                        }
                    }).hide();
                }
                if (o.showRemoveButton) {
                    buttontable.cell(1, 2).CswImageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Delete,
                        AlternateText: 'Remove',
                        ID: o.ID + 'rembtn',
                        onClick: function ($ImageDiv) {
                            _toggleRemove($ImageDiv);
                            return Csw.enums.imageButton_ButtonType.None;
                        }
                    }).hide();
                }
                if (o.showExpandColButton) {
                    buttontable.cell(1, 3).CswImageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.ArrowEast,
                        AlternateText: 'Add Column',
                        ID: o.ID + 'addcolumnbtn',
                        onClick: function () {
                            _addColumn();
                            return Csw.enums.imageButton_ButtonType.None;
                        }
                    }).hide();
                }
                if (o.showExpandRowButton) {
                    buttontable.cell(1, 4).CswImageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.ArrowSouth,
                        AlternateText: 'Add Row',
                        ID: o.ID + 'addrowbtn',
                        onClick: function () {
                            _addRow();
                            return Csw.enums.imageButton_ButtonType.None;
                        }
                    }).hide();
                }
                if (o.showConfigButton) {
                    buttontable.cell(1, 5).CswImageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Configure,
                        AlternateText: 'Configure',
                        ID: o.ID + 'configbtn',
                        onClick: function () {
                            _toggleConfig();
                            return Csw.enums.imageButton_ButtonType.None;
                        }
                    });
                }

                return table;
            },

            'cellset': function (row, column) {
                return _getCellSet(row, column);
            },

            'isConfig': function () {
                return isConfigMode();
            },

            'toggleConfig': function () {
                _toggleConfig();
            },

            'ConfigOn': function () {
                _configOn();
            },

            'ConfigOff': function () {
                _configOff();
            }
        };

        function _getCellSet(row, column) {
            var cellsetrows = Csw.number(table.propNonDom('cellset_rows'));
            var cellsetcolumns = Csw.number(table.propNonDom('cellset_columns'));
            var cellset = new Array();
            for (var r = 1; r <= cellsetrows; r++) {
                for (var c = 1; c <= cellsetcolumns; c++) {
                    if (cellset[r] === undefined) {
                        cellset[r] = new Array();
                    }
                    cellset[r][c] = getCell(row, column, r, c, cellsetrows, cellsetcolumns);
                }
            }
            return cellset;
        }

        function isRemoveMode() {
            return (table.propNonDom('removemode') === "true");
        }
        function isConfigMode() {
            return (table.propNonDom('configmode') === "true");
        }

        function setConfigMode(mode) {
            table.propNonDom('configmode', mode);
        }

        function _toggleRemove($rembtn) {
            if (isRemoveMode(table)) {
                _removeOff(table, $rembtn);
            }
            else {
                table.propNonDom('removemode', 'true');
                $rembtn.addClass('CswLayoutTable_removeEnabled');
            }
        }
        function _removeOff($rembtn) {
            table.propNonDom('removemode', 'false');
            $rembtn.removeClass('CswLayoutTable_removeEnabled');
        }
        function _toggleConfig() {
            if (isConfigMode(table)) {
                _configOff();
            } else {
                _configOn();
            }
        }

        function _configOff() {
            buttontable.$.find('#' + table.id + 'addbtn').hide();
            var $rembtn = buttontable.$.find('#' + table.id + 'rembtn');
            $rembtn.hide();
            buttontable.$.find('#' + table.id + 'addcolumnbtn').hide();
            buttontable.$.find('#' + table.id + 'addrowbtn').hide();

            table.findCell('.CswLayoutTable_cell')
                .removeClass('CswLayoutTable_configcell');

            disableDrag();

            setConfigMode('false');
            table.trigger(table.id + 'CswLayoutTable_onConfigOff');
            _removeOff($rembtn);
        } // _configOff()

        function _configOn() {
            buttontable.$.find('#' + table.id + 'addbtn').show();
            buttontable.$.find('#' + table.id + 'rembtn').show();
            buttontable.$.find('#' + table.id + 'addcolumnbtn').show();
            buttontable.$.find('#' + table.id + 'addrowbtn').show();

            //var cellsetrows = Csw.number(table.propNonDom('cellset_rows'));
            //var cellsetcolumns = Csw.number(table.propNonDom('cellset_columns'));

            table.finish(null);

            table.findCell('.CswLayoutTable_cell')
                .addClass('CswLayoutTable_configcell');

            enableDrag();

            setConfigMode('true');
            table.trigger(table.id + 'CswLayoutTable_onConfigOn');
        } // _configOn()

        function _addRow() {
            var cellsetrows = Csw.number(table.propNonDom('cellset_rows'));
            var cellsetcolumns = Csw.number(table.propNonDom('cellset_columns'));
            var tablemaxrows = table.maxrows();
            var tablemaxcolumns = table.maxcolumns();

            // add a row and column
            getCell(table, (tablemaxrows / cellsetrows) + 1, (tablemaxcolumns / cellsetcolumns), cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
            table.finish(null);

            if (isConfigMode()) {
                table.findCell('.CswLayoutTable_cell')
                    .addClass('CswLayoutTable_configcell');
            }
        } // _addRowAndColumn()

        function _addColumn() {
            var cellsetrows = Csw.number(table.propNonDom('cellset_rows'));
            var cellsetcolumns = Csw.number(table.propNonDom('cellset_columns'));
            var tablemaxrows = table.maxrows();
            var tablemaxcolumns = table.maxcolumns();

            // add a row and column
            getCell(table, (tablemaxrows / cellsetrows), (tablemaxcolumns / cellsetcolumns) + 1, cellsetrows, cellsetcolumns, cellsetrows, cellsetcolumns);
            table.finish(null);

            if (isConfigMode()) {
                table.findCell('.CswLayoutTable_cell')
                    .addClass('CswLayoutTable_configcell');
            }
        } // _addRowAndColumn()

        function getCell(row, column, cellsetrow, cellsetcolumn, cellsetrows, cellsetcolumns) {
            if (row < 1) row = 1;
            if (column < 1) column = 1;
            var realrow = ((row - 1) * cellsetrows) + cellsetrow;
            var realcolumn = ((column - 1) * cellsetcolumns) + cellsetcolumn;
            var $cell = table.cell(realrow, realcolumn);
            return $cell;
        }

        function onCreateCell($cell, realrow, realcolumn, cellsetrows, cellsetcolumns) {
            var row = Math.ceil(realrow / cellsetrows);
            var column = Math.ceil(realcolumn / cellsetcolumns);
            var cellsetrow = Csw.number(cellsetrows - realrow % cellsetrows);
            var cellsetcolumn = Csw.number(cellsetcolumns - realcolumn % cellsetcolumns);

            $cell.CswAttrNonDom({
                row: row,
                column: column,
                cellsetrow: cellsetrow,
                cellsetcolumn: cellsetcolumn
            })
                 .click(function (ev, dd) { onClick(ev, dd, row, column, cellsetrows, cellsetcolumns); })
                 .droppable({
                     hoverClass: 'CswLayoutTable_hover',
                     drop: function (ev, dd) {
                         onDrop(ev, dd, $(this), cellsetrows, cellsetcolumns);
                     }
                 })
                 .hover(function (ev, dd) { onHoverIn(ev, dd, $(this)); },
                        function (ev, dd) { onHoverOut(ev, dd, $(this)); });

            $('<div class="CswLayoutTable_celldiv"></div>').appendTo($cell);
        }

        function enableDrag() {
            table.$.find('.CswLayoutTable_celldiv')
                    .draggable({
                        revert: "invalid",
                        drag: function (ev, dd) { onDrag(ev, dd, $(this)); }
                    });
        }

        function disableDrag() {
            table.$.find('.CswLayoutTable_celldiv')
                    .draggable('destroy');
        }

        function onHoverIn(ev, dd, $cell) {
            if (isRemoveMode()) {
                var $cellset = table.findCell('[row="' + $cell.CswAttrNonDom('row') + '"][column="' + $cell.CswAttrNonDom('column') + '"]');
                $cellset.addClass('CswLayoutTable_remove');
            }
        } // onHoverIn()

        function onHoverOut(ev, dd, $cell) {
            if (isRemoveMode()) {
                var $cellset = table.findCell('[row="' + $cell.CswAttrNonDom('row') + '"][column="' + $cell.CswAttrNonDom('column') + '"]');
                $cellset.removeClass('CswLayoutTable_remove');
            }
        } // onHoverOut()

        function onDrag(ev, dd, $dragdiv) {
            if (isConfigMode(table)) {
                var $dragcell = $dragdiv.parent();

                var $cells = table.findCell('[row="' + $dragcell.CswAttrNonDom('row') + '"][column="' + $dragcell.CswAttrNonDom('column') + '"]');
                $cells.addClass('CswLayoutTable_dragcell');
            }
        } // onDrag

        function onDrop(ev, dd, $dropcell, cellsetrows, cellsetcolumns) {
            if (isConfigMode(table)) {
                //var $dropdiv = $dropcell.children('div');
                var $dragdiv = dd.draggable;
                var $dragcell = $dragdiv.parent();

                var $dragcells = table.findCell('[row="' + $dragcell.CswAttrNonDom('row') + '"][column="' + $dragcell.CswAttrNonDom('column') + '"]');
                var $dropcells = table.findCell('[row="' + $dropcell.CswAttrNonDom('row') + '"][column="' + $dropcell.CswAttrNonDom('column') + '"]');

                $dragcells.removeClass('CswLayoutTable_dragcell');

                // This must happen BEFORE we do the swap, in case the caller relies on the contents of the div being where it was
                table.trigger(table.id + 'CswLayoutTable_onSwap', {
                    table: table,
                    cellset: _getCellSet(table, $dragcell.CswAttrNonDom('row'), $dragcell.CswAttrNonDom('column')),
                    swapcellset: _getCellSet(table, $dropcells.first().CswAttrNonDom('row'), $dropcells.first().CswAttrNonDom('column')),
                    row: $dragcell.CswAttrNonDom('row'),
                    column: $dragcell.CswAttrNonDom('column'),
                    swaprow: $dropcells.first().CswAttrNonDom('row'),
                    swapcolumn: $dropcells.first().CswAttrNonDom('column')
                });

                for (var r = 1; r <= cellsetrows; r += 1) {
                    for (var c = 1; c <= cellsetcolumns; c += 1) {
                        var $thisdragcell = $dragcells.filter('[cellsetrow="' + r + '"][cellsetcolumn="' + c + '"]');
                        var $thisdropcell = $dropcells.filter('[cellsetrow="' + r + '"][cellsetcolumn="' + c + '"]');
                        var $thisdragdiv = $thisdragcell.children('div');
                        var $thisdropdiv = $thisdropcell.children('div');

                        $thisdragcell.append($thisdropdiv);
                        $thisdropcell.append($thisdragdiv);

                        $thisdragdiv.position({
                            my: "left top",
                            at: "left top",
                            of: $thisdropcell,
                            offset: table.propNonDom('cellpadding')
                        });

                        $thisdropdiv.position({
                            my: "left top",
                            at: "left top",
                            of: $thisdragcell,
                            offset: table.propNonDom('cellpadding')
                        });

                    } // for(c = 1; c <= cellsetcolumns; c++)
                } // for(r = 1; r <= cellsetrows; r++)
            } // if(isConfigMode($table))
        } // onDrop()

        function onClick(ev, dd, row, column) {
            if (isRemoveMode(table)) {
                var $removecells = table.$.find('.CswLayoutTable_remove');
                if ($removecells.length > 0) {
                    table.trigger(table.id + 'CswLayoutTable_onRemove', {
                        table: table,
                        cellset: _getCellSet(table, row, column),
                        row: $removecells.CswAttrNonDom('row'),
                        column: $removecells.CswAttrNonDom('column')
                    });
                }
                // contents().hide() doesn't work, jQuery ticket #8586: http://bugs.jquery.com/ticket/8586
                $removecells.children().hide();

                $removecells.removeClass('CswLayoutTable_remove');
            } // if(isRemoveMode($table))
        } // onClick()


        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + PluginName); return false;
        }

    }; // function (options) {
} (jQuery));

