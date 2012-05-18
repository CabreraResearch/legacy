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
            var cswPrivateVar = {
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
            var cswPublicRet = {};

            (function () {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }

                cswPrivateVar.table = cswParent.table(cswPrivateVar);
                cswPublicRet = Csw.dom({}, cswPrivateVar.table);
            } ());

            cswPublicRet.addCell = function (value, row, col) {
                var cssClass = '';
                if (row === 1) {
                    if (cswPrivateVar.hasHeader) {
                        cssClass = 'CswThinGridHeaderShow';
                    } else {
                        cssClass = 'CswThinGridHeaderHide';
                    }
                }
                cswPrivateVar.table.cell(row, col).append(Csw.string(value, '&nbsp;')).addClass(cssClass);
            };

            cswPublicRet.addRows = function (dataRows, row, col) {
                col = col || 0;
                row = row || cswPrivateVar.rowCount;
                if (Csw.isArray(dataRows)) {
                    Csw.each(dataRows, function (cellVal) {
                        if (Csw.isArray(cellVal)) {
                            cswPublicRet.addRows(cellVal, row, col);
                        } else {
                            col += 1;
                            cswPublicRet.addCell(cellVal, cswPrivateVar.rowCount, col);
                        }
                    });
                }
                cswPrivateVar.rowCount += 1;
            };

            (function () {
                if (cswPrivateVar.rows.length > 0) {
                    cswPrivateVar.rowCount = 1;
                } else {
                    cswPrivateVar.rowCount = 0;
                }

                cswPublicRet.addRows(cswPrivateVar.rows);

                cswPrivateVar.table.cell(cswPrivateVar.rowCount, 1).a({
                    text: cswPrivateVar.linkText,
                    onClick: cswPrivateVar.onLinkClick
                });

            } ());


            return cswPublicRet;
        });

} ());
