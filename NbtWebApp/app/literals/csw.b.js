/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.literals.b = Csw.literals.b ||
        Csw.literals.register('b', function (options) {
            'use strict';
            /// <summary> Create or extend an HTML <p /> and return a Csw.divobject
            ///     &#10;1 - div(options)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the p.</para>
            /// <para>options.cssclass: CSS class to asign</para>
            /// <para>options.text: Text to display</para>
            /// </param>
            /// <returns type="p">A p object</returns>
            var cswPrivate = {
                $parent: '',
                text: ''
            };
            var cswPublic = {};

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                var html = '', $b;

                html += '<b>';
                html += Csw.string(cswPrivate.text);
                html += '</b>';
                $b = $(html);
                Csw.literals.factory($b, cswPublic);

                if (cswPrivate.$parent) {
                    cswPrivate.$parent.append(cswPublic.$);
                }
            } ());

            return cswPublic;
        });

} ());

