/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
            var cswPrivateVar = {
                $parent: '',
                ID: '',
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
                border: 0
            };
            var cswPublicRet = { };

            (function() {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                var $table = $('<table id="' + cswPrivateVar.ID + '"></table>');
                var isjQuery = Csw.isJQuery(options);

                if (isjQuery) {
                    $table = options;
                }
                Csw.literals.factory($table, cswPublicRet);

                cswPublicRet.bind('CswTable_onCreateCell', function(e, cell, row, column) {
                    Csw.tryExec(cswPrivateVar.onCreateCell(e, cell, row, column));
                    e.stopPropagation(); // prevents events from triggering in nested tables
                });
                cswPublicRet.trigger('CswTable_onCreateCell', [cswPublicRet.find('td'), 1, 1]);

                if (false === isjQuery) {
                    cswPublicRet.addClass(cswPrivateVar.TableCssClass);
                    cswPublicRet.propDom({
                        width: cswPrivateVar.width,
                        align: cswPrivateVar.align
                    });
                    cswPublicRet.propNonDom({
                        cellpadding: cswPrivateVar.cellpadding,
                        cellspacing: cswPrivateVar.cellspacing,
                        border: cswPrivateVar.border,
                        cellalign: cswPrivateVar.cellalign,
                        cellvalign: cswPrivateVar.cellvalign,
                        cellcssclass: cswPrivateVar.CellCssClass,
                        FirstCellRightAlign: cswPrivateVar.FirstCellRightAlign,
                        OddCellRightAlign: cswPrivateVar.OddCellRightAlign
                    });
                    cswPublicRet.css('text-align', cswPrivateVar.align);
                    cswPrivateVar.$parent.append(cswPublicRet.$);
                }
            }());

            cswPublicRet.cell = function(row, col) {
                /// <summary>Get a cell from the table</summary>
                /// <param name="row" type="Number">Row number</param>
                /// <param name="col" type="Number">Column number</param>
                /// <returns type="Object">A Csw table cell object.</returns>
                var thisRow, align, newCell, retCell = { }, html,
                    thisCol, id,
                    attr = Csw.makeAttr();

                if (cswPublicRet.length() > 0 &&
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

                    if (cswPrivateVar.ID) {
                        retCell = cswPublicRet.find('#' + Csw.makeId(cswPrivateVar.ID, 'row_' + row, 'col_' + col, '', false));
                    }
                    if (Csw.isNullOrEmpty(retCell)) {
                        retCell = cswPublicRet.children('tbody')
                            .children('tr:eq(' + Csw.number(row - 1) + ')')
                            .children('td:eq(' + Csw.number(col - 1) + ')');
                    }

                    if (Csw.isNullOrEmpty(retCell)) {
                        while (row > cswPublicRet.children('tbody').children('tr').length()) {
                            cswPublicRet.append('<tr></tr>');
                        }
                        thisRow = cswPublicRet.children('tbody').children('tr:eq(' + Csw.number(row - 1) + ')');
                        thisCol = thisRow.children('td').length();

                        while (col > thisCol) {
                            html = '';
                            thisCol += 1;
                            id = Csw.makeId(cswPrivateVar.ID, 'row_' + row, 'col_' + thisCol, '', false);
                            align = cswPublicRet.propNonDom('cellalign');
                            if ((thisRow.children('td').length() === 0 && Csw.bool(cswPublicRet.propNonDom('FirstCellRightAlign'))) ||
                                (thisRow.children('td').length() % 2 === 0 && Csw.bool(cswPublicRet.propNonDom('OddCellRightAlign')))) {
                                align = 'right';
                            }
                            html += '<td ';
                            if (false === Csw.isNullOrEmpty(id)) {
                                attr.add('id', id);
                            }
                            attr.add('realrow', row);
                            attr.add('realcol', thisCol);
                            attr.add('class', cswPublicRet.propNonDom('cellcssclass'));
                            attr.add('align', align);
                            attr.add('valign', cswPublicRet.propNonDom('cellvalign'));
                            html += attr.get();
                            html += '>';
                            html += '</td>';
                            newCell = thisRow.attach(html);

                            cswPublicRet.trigger('CswTable_onCreateCell', [newCell, row, thisCol]);
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

            //        cswPublicRet.add = function (row, col, content, id) {
            //            /// <summary>Add content to a cell of this table.</summary>
            //            /// <param name="row" type="Number">Row number.</param>
            //            /// <param name="col" type="Number">Column number.</param>
            //            /// <param name="content" type="String">Content to add.</param>
            //            /// <returns type="Object">The specified cell.</returns>
            //            var retCell = cswPublicRet.cell(row, col, id);
            //            retCell.append(content);
            //            return retCell;
            //        };

            cswPublicRet.maxrows = function() {
                /// <summary>Get the maximum table row number</summary>
                /// <returns type="Number">Number of rows</returns>
                var rows = cswPublicRet.children('tbody').children('tr');
                return rows.length();
            };

            cswPublicRet.maxcolumns = function() {
                /// <summary>Get the maximum table column number</summary>
                /// <returns type="Number">Number of columns</returns>
                var body = cswPublicRet.children('tbody'),
                    maxrows = cswPublicRet.maxrows(),
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

            cswPublicRet.finish = function(onEmptyCell, startingRow, startingCol) {
                /// <summary>Finish</summary>
                /// <returns type="undefined"></returns>
                var maxrows = cswPublicRet.maxrows(),
                    maxcolumns = cswPublicRet.maxcolumns(),
                    r, c, cell;

                // make missing cells, and add &nbsp; to empty cells
                for (r = Csw.number(startingRow, 1); r <= maxrows; r += 1) {
                    for (c = Csw.number(startingCol, 1); c <= maxcolumns; c += 1) {
                        cell = cswPublicRet.cell(r, c);
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
            cswPublicRet.findRow = function(criteria) {
                /// <summary>Find a row by jQuery search criteria</summary>
                /// <param name="criteria" type="String"></param>
                /// <returns type="Object">Rows matching search</returns>
                var rows = cswPublicRet.children('tbody').children('tr'),
                    ret = { };
                if (false === Csw.isNullOrEmpty(criteria)) {
                    ret = rows.filter(criteria);
                }
                return ret;
            };

            cswPublicRet.findCell = function(criteria) {
                /// <summary>Find a cells by jQuery search criteria</summary>
                /// <param name="criteria" type="String"></param>
                /// <returns type="Object">Cells matching search</returns>
                var cells, ret = { };
                if (Csw.contains(criteria, 'row') &&
                    Csw.contains(criteria, 'column')) {
                    ret = cswPublicRet.jquery($(cswPublicRet.$[0].rows[criteria.row].cells[criteria.column]));
                } else {
                    cells = cswPublicRet.children('tbody').children('tr').children('td');
                    if (cells.isValid && false === Csw.isNullOrEmpty(criteria)) {
                        ret = cells.filter(criteria);
                    }
                }
                return ret;
            };

            cswPublicRet.rowFindCell = function(row, criteria) {
                /// <summary>Given a row, find a cell by jQuery search criteria</summary>
                /// <param name="row" type="Object"></param>
                /// <param name="criteria" type="String"></param>
                /// <returns type="Object">Cells matching search</returns>
                var cells = row.children('td'),
                    $cells, ret = { };

                if (false === Csw.isNullOrEmpty(criteria)) {
                    $cells = cells.$.filter(criteria);
                    ret = cswPublicRet.jquery($cells);
                }
                return ret;
            };

            return cswPublicRet;
        });

} ());

