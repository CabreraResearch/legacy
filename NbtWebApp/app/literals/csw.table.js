/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.literals.table = Csw.literals.table ||
        Csw.literals.register('table', function(options) {
            'use strict';
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
            var cswPrivate = {
                $parent: '',
                ID: '',
                name: 'table',
                TableCssClass: '',
                CellCssClass: '',
                cellpadding: 0,
                cellspacing: 0,
                align: '',
                width: '',
                cellalign: 'left',
                cellvalign: 'top',
                onCreateCell: function() {
                },
                FirstCellRightAlign: false,
                OddCellRightAlign: false,
                border: 0, 
                styles: {}
            };
            var cswPublic = { };

            (function() {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                var $table = $('<table id="' + cswPrivate.ID + '" name="' + cswPrivate.name + '"></table>');
                var isjQuery = Csw.isJQuery(options);

                if (isjQuery) {
                    $table = options;
                }
                Csw.literals.factory($table, cswPublic);

                cswPublic.bind('CswTable_onCreateCell', function(e, cell, row, column) {
                    Csw.tryExec(cswPrivate.onCreateCell(e, cell, row, column));
                    e.stopPropagation(); // prevents events from triggering in nested tables
                });
                cswPublic.trigger('CswTable_onCreateCell', [cswPublic.find('td'), 1, 1]);

                if (false === isjQuery) {
                    cswPublic.addClass(cswPrivate.TableCssClass);
                    cswPublic.propDom({
                        width: cswPrivate.width,
                        align: cswPrivate.align
                    });
                    cswPublic.css(cswPrivate.styles);
                    cswPublic.propDom({
                        cellpadding: cswPrivate.cellpadding,
                        cellspacing: cswPrivate.cellspacing,
                        border: cswPrivate.border,
                        cellalign: cswPrivate.cellalign
                    });
                    cswPublic.data({
                        cellcssclass: cswPrivate.CellCssClass,
                        FirstCellRightAlign: cswPrivate.FirstCellRightAlign,
                        OddCellRightAlign: cswPrivate.OddCellRightAlign
                    });
                    cswPublic.css({
                        'text-align': cswPrivate.align,
                        'vertical-align': cswPrivate.cellvalign
                    });
                    cswPrivate.$parent.append(cswPublic.$);
                }
            }());

            cswPublic.cell = function(row, col) {
                /// <summary>Get a cell from the table</summary>
                /// <param name="row" type="Number">Row number</param>
                /// <param name="col" type="Number">Column number</param>
                /// <returns type="Object">A Csw table cell object.</returns>
                var thisRow, align, newCell, retCell = { }, html,
                    thisCol, id,
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle();

                if (cswPublic.length() > 0 &&
                    false === Csw.isNullOrEmpty(row) &&
                        false === Csw.isNullOrEmpty(col)) {
                    if (row <= 0) {
                        Csw.debug.log("table.cell() error: row must be greater than 1, got: " + row);
                        row = 1;
                    }
                    if (col <= 0) {
                        Csw.debug.log("table.cell() error: col must be greater than 1, got: " + col);
                        col = 1;
                    }

                    if (cswPrivate.ID) {
                        retCell = cswPublic.find('#' + cswPrivate.ID + 'row' + row + 'col' + col);
                    }
                    if (Csw.isNullOrEmpty(retCell)) {
                        retCell = cswPublic.children('tbody')
                            .children('tr:eq(' + Csw.number(row - 1) + ')')
                            .children('td:eq(' + Csw.number(col - 1) + ')');
                    }

                    if (Csw.isNullOrEmpty(retCell)) {
                        while (row > cswPublic.children('tbody').children('tr').length()) {
                            cswPublic.append('<tr></tr>');
                        }
                        thisRow = cswPublic.children('tbody').children('tr:eq(' + Csw.number(row - 1) + ')');
                        thisCol = thisRow.children('td').length();

                        while (col > thisCol) {
                            html = '';
                            thisCol += 1;
                            id = cswPrivate.ID + 'row' + row + 'col' + thisCol;
                            align = cswPublic.data('cellalign');
                            if ((thisRow.children('td').length() === 0 && Csw.bool(cswPublic.data('FirstCellRightAlign'))) ||
                                (thisRow.children('td').length() % 2 === 0 && Csw.bool(cswPublic.data('OddCellRightAlign')))) {
                                align = 'right';
                            }
                            html += '<td ';
                            if (false === Csw.isNullOrEmpty(id)) {
                                attr.add('id', id);
                            }
                            attr.add('data-realrow', row);
                            attr.add('data-realcol', thisCol);
                            attr.add('class', cswPrivate.cellcssclass);
                            attr.add('align', align);
                            style.add('vertical-align', cswPrivate.cellvalign);
                            html += attr.get();
                            html += style.get();
                            html += '>';
                            html += '</td>';
                            newCell = thisRow.attach(html);

                            cswPublic.trigger('CswTable_onCreateCell', [newCell, row, thisCol]);
                            if (thisCol === col) {
                                retCell = newCell;
                            }
                        }
                    }

                    retCell.align = function(alignTo) {
                        retCell.css('text-align', alignTo);
                        retCell.propDom('align', alignTo);
                        return retCell;
                    };
                }
                return retCell;
            };

            cswPublic.maxrows = function() {
                /// <summary>Get the maximum table row number</summary>
                /// <returns type="Number">Number of rows</returns>
                var rows = cswPublic.children('tbody').children('tr');
                return rows.length();
            };

            cswPublic.maxcolumns = function() {
                /// <summary>Get the maximum table column number</summary>
                /// <returns type="Number">Number of columns</returns>
                var body = cswPublic.children('tbody'),
                    maxrows = cswPublic.maxrows(),
                    maxcolumns = 0,
                    r, c, columns, row;

                for (r = 0; r < maxrows; r += 1) {
                    row = body.children('tr:eq(' + r + ')');
                    columns = row.children('td');
                    if (columns.length() > maxcolumns) {
                        maxcolumns = columns.length();
                    }
                }
                return maxcolumns;
            };

            cswPublic.finish = function(onEmptyCell, startingRow, startingCol) {
                /// <summary>Finish</summary>
                /// <returns type="undefined"></returns>
                var maxrows = cswPublic.maxrows(),
                    maxcolumns = cswPublic.maxcolumns(),
                    r, c, cell;

                // make missing cells, and add &nbsp; to empty cells
                for (r = Csw.number(startingRow, 1); r <= maxrows; r += 1) {
                    for (c = Csw.number(startingCol, 1); c <= maxcolumns; c += 1) {
                        cell = cswPublic.cell(r, c);
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
            cswPublic.findRow = function(criteria) {
                /// <summary>Find a row by jQuery search criteria</summary>
                /// <param name="criteria" type="String"></param>
                /// <returns type="Object">Rows matching search</returns>
                var rows = cswPublic.children('tbody').children('tr'),
                    ret = { };
                if (false === Csw.isNullOrEmpty(criteria)) {
                    ret = rows.filter(criteria);
                }
                return ret;
            };

            cswPublic.findCell = function(criteria) {
                /// <summary>Find a cells by jQuery search criteria</summary>
                /// <param name="criteria" type="String"></param>
                /// <returns type="Object">Cells matching search</returns>
                var cells, ret = { };
                if (Csw.contains(criteria, 'row') &&
                    Csw.contains(criteria, 'column')) {
                    ret = cswPublic.jquery($(cswPublic.$[0].rows[criteria.row].cells[criteria.column]));
                } else {
                    cells = cswPublic.children('tbody').children('tr').children('td');
                    if (cells.isValid && false === Csw.isNullOrEmpty(criteria)) {
                        ret = cells.filter(criteria);
                    }
                }
                return ret;
            };

            cswPublic.rowFindCell = function(row, criteria) {
                /// <summary>Given a row, find a cell by jQuery search criteria</summary>
                /// <param name="row" type="Object"></param>
                /// <param name="criteria" type="String"></param>
                /// <returns type="Object">Cells matching search</returns>
                var cells = row.children('td'),
                    $cells, ret = { };

                if (false === Csw.isNullOrEmpty(criteria)) {
                    $cells = cells.$.filter(criteria);
                    ret = cswPublic.jquery($cells);
                }
                return ret;
            };

            return cswPublic;
        });

} ());

