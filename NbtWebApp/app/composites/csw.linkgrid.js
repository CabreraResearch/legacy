/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


    Csw.composites.register('linkGrid', function (cswParent, options) {
        /// <summary>
        /// Create a LINK grid (captioned link with row count display) and return a Csw.linkGrid object
        ///     &#10;1 - table(options)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the linkGrid.</para>
        /// <para>options.linkText: linkGrid link prefix</para>
        /// </param>
        /// <returns type="linkGrid">A linkGrid object</returns>
        'use strict';
        var cswPrivate = {
            name: '',
            cssclass: '',
            linkText: '',
            onLinkClick: null,
            isControl: false,
            hasHeader: false, /* Ignore the header row for now, by default */
            rowCount: 0,
            readonly: false
        };
        var cswPublic = {};

        (function () {
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            cswPrivate.table = cswParent.table(cswPrivate);
            cswPublic = Csw.dom({}, cswPrivate.table);
        }());

        (function () {
            var strLinkText = cswPrivate.linkText;
            if (cswPrivate.rowCount < 1) {
                strLinkText += ' (none defined)';
            }
            else {
                strLinkText += ' (' + cswPrivate.rowCount + ' defined)';
            }
            if (false === cswPrivate.readonly) {
                cswPrivate.table.cell(1, 1).a({
                    text: strLinkText,
                    onClick: cswPrivate.onLinkClick
                });
            } else {
                cswPrivate.table.cell(1, 1).span({ text: strLinkText });
            }

        }());


        return cswPublic;
    });

}());
