/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
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
            var internal = {
                $parent: '',
                ID: '',
                cssclass: '',
                rows: [],
                width: '',
                height: '',
                cellpadding: 2,
                linkText: 'More...',
                onLinkClick: null,
                isControl: false
            };
            var external = {};

            (function () {
                var row = 1,
                    col;
                if (options) {
                    $.extend(internal, options);
                }

                internal.table = cswParent.table(internal);
                external = Csw.dom({}, internal.table);
                //$.extend(external, Csw.literals.table(internal));

                /* Ignore the header row for now */
                if (internal.rows.length > 0) {
                    internal.rows.splice(0, 1);
                }

                Csw.each(internal.rows, function (value) {
                    col = 1;
                    if (Csw.isArray(value)) {
                        Csw.each(value, function (subVal) {
                            var valString = Csw.string(subVal, '&nbsp;');
                            internal.table.cell(row, col).append(valString);
                            col += 1;
                        });
                    } else {
                        internal.table.cell(row, col).append(value);
                    }
                    row += 1;
                });

                internal.table.cell(row, 1).link({
                    text: internal.linkText,
                    onClick: internal.onLinkClick
                });

            } ());

            return external;
        });

} ());
