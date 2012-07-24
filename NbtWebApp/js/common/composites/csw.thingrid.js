/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.composites.thinGrid = Csw.composites.thinGrid ||
        Csw.composites.register('thinGrid', function (cswParent, options) {
            /// <summary>
            /// Create a thin grid (simple HTML table with a 'More..' link) and return a Csw.thinGrid object
            ///     &#10;1 - table(options)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the thinGrid.</para>
            /// <para>options.rows: Row values of grid. 
            ///     <para>[1,2,3] An array of values (single column)</para>	  
            ///     <para>[[a,b,c],[1,2,3]] An array of arrays (multiple columns)</para>
            /// </para>
            /// <para>options.width: thinGrid width</para>
            /// <para>options.height: thinGrid height</para>
            /// </param>
            /// <returns type="thinGrid">A thinGrid object</returns>
            'use strict';
            var cswPrivate = {
                ID: '',
                cssclass: '',
                rows: [],
                width: '',
                height: '',
                cellpadding: 5,
                linkText: 'More...',
                onLinkClick: null,
                isControl: false,
                header: [],
                hasHeader: false, /* Ignore the header row for now, by default */
                rowCount: 0,
                rowElements: [],
                allowDelete: false,
                allowAdd: false,
                makeAddRow: null,
                onAdd: null,
                onDelete: null,
                TableCssClass: 'CswThinGridTable',
                CellCssClass: 'CswThinGridCells'
            };
            var cswPublic = {};

            (function () {
                if (options) {
                    $.extend(cswPrivate, options);
                }
                cswPrivate.div = cswParent.div();
                cswPrivate.table = cswPrivate.div.table(cswPrivate);
                cswPublic = Csw.dom({}, cswPrivate.table);
            } ());

            cswPublic.hide = function () {
                cswPrivate.div.hide();
                return cswPublic;
            };

            cswPublic.show = function () {
                cswPrivate.div.show();
                return cswPublic;
            };

            cswPrivate.isHeaderRow = function (rowid) {
                return (rowid === 1 && cswPrivate.hasHeader);
            };

            cswPublic.addCell = Csw.method(function (value, row, col) {
                var cssClass = '', thisCell;
                if (row === 1) {
                    if (cswPrivate.hasHeader) {
                        if(false === Csw.isNullOrEmpty(value)) {
                            cswPrivate.header[col] = value;
                        }
                        cssClass = 'CswThinGridHeaderShow';
                    } else {
                        cssClass = 'CswThinGridHeaderHide';
                    }
                }

                thisCell = cswPrivate.table.cell(row, col);
                if(false === Csw.isNullOrEmpty(value)) {
                    thisCell.append(Csw.string(value, '&nbsp;'));
                }
                thisCell.addClass(cssClass);
                
                if (false === Csw.isArray(cswPrivate.rowElements[row])) {
                    cswPrivate.rowElements[row] = [thisCell];
                } else {
                    cswPrivate.rowElements[row].push(thisCell);
                }
                return thisCell;
            });

            cswPrivate.addDeleteBtn = Csw.method(function (row, col) {
                var cell = cswPublic.addCell('', row, col);
                cell.buttonExt({
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.trash),
                    size: 'small',
                    tooltip: { title: 'Delete' },
                    disableOnClick: false,
                    onClick: function () {
                        cswPublic.deleteRow(row);
                    }
                });
            });

            cswPrivate.addAddBtn = Csw.method(function (row, col) {
                var cell = cswPublic.addCell('', row, col);
                cell.buttonExt({
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                    size: 'small',
                    tooltip: { title: 'Add' },
                    disableOnClick: false,
                    onClick: function () {
                        cswPublic.deleteRow(row);
                        Csw.tryExec(cswPrivate.onAdd, row);
                        Csw.tryExec(cswPublic.makeAddRow, cswPrivate.makeAddRow);
                    }
                });
            });

            cswPublic.addRows = Csw.method(function(dataRows, row, col) {
                col = col || 0;
                row = row || cswPrivate.rowCount;
                if (Csw.isArray(dataRows)) {
                    Csw.each(dataRows, function (cellVal) {
                        if (Csw.isArray(cellVal)) {
                            cswPublic.addRows(cellVal, row, col);
                        } else {
                            col += 1;
                            cswPublic.addCell(cellVal, row, col);
                        }
                    });
                    if (false === cswPrivate.isHeaderRow(row) && cswPrivate.allowDelete) {
                        col += 1;
                        cswPrivate.addDeleteBtn(row, col);
                    }
                }
                cswPrivate.rowCount += 1;
                return row;
            });
            
            cswPublic.deleteRow = Csw.method(function (rowid) {
                Csw.debug.assert(Csw.contains(cswPrivate.rowElements, rowid), 'No such row exists.');
                if (Csw.contains(cswPrivate.rowElements, rowid)) {
                    Csw.each(cswPrivate.rowElements[rowid], function (cell) {
                        cell.remove();
                    });
                    delete cswPrivate.rowElements[rowid];
                    //cswPrivate.rowCount -= 1;
                    Csw.tryExec(cswPrivate.onDelete, rowid);
                }
            });

            cswPublic.makeAddRow = Csw.method(function(callBack) {
                ///<summary>Create a new cell for each column and pass the column name and cell into the callback method.</summary>
                if (Csw.isFunction(callBack)) {
                    cswPrivate.rowCount += 1;
                    cswPrivate.header.forEach(function(element, index, array) {
                        var cell = cswPublic.addCell('', cswPrivate.rowCount, index);
                        Csw.tryExec(callBack, cell, element, cswPrivate.rowCount);
                    });
                    cswPrivate.addAddBtn(cswPrivate.rowCount, cswPrivate.header.length);
                }
            });
            
            (function () {
                if (cswPrivate.rows.length > 0) {
                    cswPrivate.rowCount = 1;
                } else {
                    cswPrivate.rowCount = 0;
                }

                cswPublic.addRows(cswPrivate.rows);

                cswPrivate.table.cell(cswPrivate.rowCount, 1).a({
                    text: cswPrivate.linkText,
                    onClick: cswPrivate.onLinkClick
                });

                Csw.tryExec(cswPublic.makeAddRow, cswPrivate.makeAddRow);

            } ());


            return cswPublic;
        });

} ());
