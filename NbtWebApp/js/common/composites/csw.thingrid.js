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
                hasHeader: false, /* Ignore the header row for now, by default */
                rowCount: 0,
                TableCssClass: 'CswThinGridTable',
                CellCssClass: 'CswThinGridCells'
            };
            var cswPublic = {};

            (function () {
                if (options) {
                    $.extend(cswPrivate, options);
                }

                cswPrivate.table = cswParent.table(cswPrivate);
                cswPublic = Csw.dom({}, cswPrivate.table);
            } ());

            cswPublic.addCell = function (value, row, col) {
                var cssClass = '';
                if (row === 1) {
                    if (cswPrivate.hasHeader) {
                        cssClass = 'CswThinGridHeaderShow';
                    } else {
                        cssClass = 'CswThinGridHeaderHide';
                    }
                }
                cswPrivate.table.cell(row, col).append(Csw.string(value, '&nbsp;')).addClass(cssClass);
            };

            cswPublic.addRows = function (dataRows, row, col) {
                col = col || 0;
                row = row || cswPrivate.rowCount;
                if (Csw.isArray(dataRows)) {
                    Csw.each(dataRows, function (cellVal) {
                        if (Csw.isArray(cellVal)) {
                            cswPublic.addRows(cellVal, row, col);
                        } else {
                            col += 1;
                            cswPublic.addCell(cellVal, cswPrivate.rowCount, col);
                        }
                    });
                }
                cswPrivate.rowCount += 1;
            };

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

            } ());


            return cswPublic;
        });

} ());
