/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswTable() {
    'use strict';

    function table(options) {
        /// <summary>
        /// Create or extend an HTML table and return a Csw.table object
        ///     &#10;1 - table(options)
        ///     &#10;2 - table($jqueryElement)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the table.</para>
        /// <para>options.align: Align value</para>
        /// <para>options.width: Table width</para>
        /// </param>
        /// <returns type="table">A table object</returns>
        var internal = {
            $parent: '',
            ID: '',
            TableCssClass: '',
            CellCssClass: '',
            cellpadding: 0,
            cellspacing: 0,
            align: '',
            width: '',
            cellalign: 'top',
            cellvalign: 'center',
            onCreateCell: function () {},
            FirstCellRightAlign: false,
            OddCellRightAlign: false,
            border: 0
        };
        var external = {};

        (function () {
            if (options) {
                $.extend(internal, options);
            }
            var $table = $('<table id="' + internal.ID + '"></table>');
            var isjQuery = Csw.isJQuery(options);

            if (isjQuery) {
                $table = options;
            }
            Csw.controls.factory($table, external);

            external.bind('CswTable_onCreateCell', function (e, cell, row, column) {
                Csw.tryExec(internal.onCreateCell(e, cell, row, column));
                e.stopPropagation(); // prevents events from triggering in nested tables
            });
            external.trigger('CswTable_onCreateCell', [external.find('td'), 1, 1]);

            if (false === isjQuery) {
                external.addClass(internal.TableCssClass);
                external.propDom({
                    width: internal.width,
                    align: internal.align
                });
                external.propNonDom({
                    cellpadding: internal.cellpadding,
                    cellspacing: internal.cellspacing,
                    border: internal.border,
                    cellalign: internal.cellalign,
                    cellvalign: internal.cellvalign,
                    cellcssclass: internal.CellCssClass,
                    FirstCellRightAlign: internal.FirstCellRightAlign,
                    OddCellRightAlign: internal.OddCellRightAlign
                });
                external.css('text-align', internal.align);
                internal.$parent.append(external.$);
            }
        } ());

        external.cell = function (row, col, id) {
            /// <summary>Get a cell from the table</summary>
            /// <param name="row" type="Number">Row number</param>
            /// <param name="col" type="Number">Column number</param>
            /// <returns type="Object">A Csw table cell object.</returns>
            var $cell = null,
                thisRow, align, newCell, retCell = {}, html = '',
                attr = Csw.controls.dom.attributes();

            if (external.length() > 0 &&
                false === Csw.isNullOrEmpty(row) &&
                false === Csw.isNullOrEmpty(col)) {
                if (row <= 0) {
                    Csw.log("table.cell() error: row must be greater than 1, got: " + row);
                    row = 1;
                }
                if (col <= 0) {
                    Csw.log("table.cell() error: col must be greater than 1, got: " + col);
                    col = 1;
                }

                while (row > external.children('tbody').children('tr').length()) {
                    external.append('<tr></tr>');
                }
                thisRow = external.children('tbody').children('tr:eq(' + Csw.number(row - 1) + ')');
                while (col > thisRow.children('td').length()) {
                    align = external.propNonDom('cellalign');
                    if ((thisRow.children('td').length() === 0 && Csw.bool(external.propNonDom('FirstCellRightAlign'))) ||
                        (thisRow.children('td').length() % 2 === 0 && Csw.bool(external.propNonDom('OddCellRightAlign')))) {
                        align = 'right';
                    }
                    html += '<td ';
                    if (false === Csw.isNullOrEmpty(id)) {
                        attr.add('id', id);
                    }
                    attr.add('class', external.propNonDom('cellcssclass'));
                    attr.add('align', align);
                    attr.add('valign', external.propNonDom('cellvalign'));
                    html += attr.get();
                    html += '>';
                    html += '</td>';
                    newCell = thisRow.append(html);
                    external.trigger('CswTable_onCreateCell', [newCell, row, thisRow.children('td').length()]);
                }
                $cell = thisRow.children('td:eq(' + Csw.number(col - 1) + ')').$;
            }
            Csw.controls.factory($cell, retCell);
            retCell.align = function (alignTo) {
                retCell.css('text-align', alignTo);
                retCell.propDom('align', alignTo);
                return retCell;
            };

            return retCell;
        };

        external.add = function (row, col, content, id) {
            /// <summary>Add content to a cell of this table.</summary>
            /// <param name="row" type="Number">Row number.</param>
            /// <param name="col" type="Number">Column number.</param>
            /// <param name="content" type="String">Content to add.</param>
            /// <returns type="Object">The specified cell.</returns>
            var retCell = external.cell(row, col, id);
            retCell.append(content);
            return retCell;
        };

        external.maxrows = function () {
            /// <summary>Get the maximum table row number</summary>
            /// <returns type="Number">Number of rows</returns>
            var rows = external.children('tbody').children('tr');
            return rows.length();
        };

        external.maxcolumns = function () {
            /// <summary>Get the maximum table column number</summary>
            /// <returns type="Number">Number of columns</returns>
            var rows = external.children('tbody').children('tr'),
                maxcolumns = 0,
                r, columns;
            for (r = 0; r < rows.length(); r += 1) {
                columns = rows.find('td:eq(' + r.toString() + ')');
                if (columns.length() > maxcolumns) {
                    maxcolumns = columns.length();
                }
            }
            return maxcolumns;
        };

        external.finish = function (onEmptyCell) {
            /// <summary>Finish</summary>
            /// <returns type="undefined"></returns>
            var maxrows = internal.getMaxRows(),
                maxcolumns = external.maxcolumns(),
                r, c, cell;

            // make missing cells, and add &nbsp; to empty cells
            for (r = 1; r <= maxrows; r += 1) {
                for (c = 1; c <= maxcolumns; c += 1) {
                    cell = external.cell(r, c);
                    if (cell.length() === 0) {
                        if (onEmptyCell !== null) {
                            onEmptyCell(cell, r, c);
                        } else {
                            cell.append('&nbsp;');
                        }
                    }
                }
            }
        };

        // These are safe for nested tables, since using $.find() is not
        external.findRow = function (criteria) {
            /// <summary>Find a row by jQuery search criteria</summary>
            /// <param name="criteria" type="String"></param>
            /// <returns type="Object">Rows matching search</returns>
            var rows = external.children('tbody').children('tr'),
                $rows, ret = {};
            if (false === Csw.isNullOrEmpty(criteria)) {
                $rows = rows.$.filter(criteria);
                ret = external.jquery($rows);
            }
            return ret;
        };

        external.findCell = function (criteria) {
            /// <summary>Find a cells by jQuery search criteria</summary>
            /// <param name="criteria" type="String"></param>
            /// <returns type="Object">Cells matching search</returns>
            var $retCell = null,
                cells;
            if (Csw.contains(criteria, 'row') &&
                Csw.contains(criteria, 'column')) {
                $retCell = $(external.$[0].rows[criteria.row].cells[criteria.column]);
            } else {
                cells = external.children('tbody').children('tr').children('td');
                if (false === Csw.isNullOrEmpty(criteria)) {
                    $retCell = cells.$.filter(criteria);
                }
            }
            return external.jquery($retCell);
        };

        external.rowFindCell = function (row, criteria) {
            /// <summary>Given a row, find a cell by jQuery search criteria</summary>
            /// <param name="row" type="Object"></param>
            /// <param name="criteria" type="String"></param>
            /// <returns type="Object">Cells matching search</returns>
            var cells = row.children('td'),
                $cells, ret = {};

            if (false === Csw.isNullOrEmpty(criteria)) {
                $cells = cells.$.filter(criteria);
                ret = external.jquery($cells);
            }
            return ret;
        };

        return external;
    }
    Csw.controls.register('table', table);
    Csw.controls.table = Csw.controls.table || table;

} ());

