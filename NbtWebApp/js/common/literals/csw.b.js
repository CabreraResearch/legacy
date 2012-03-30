/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
            var internal = {
                $parent: '',
                text: ''
            };
            var external = {};

            (function () {
                if (options) {
                    $.extend(internal, options);
                }
                var html = '', $b;

                html += '<b>';
                html += Csw.string(internal.text);
                html += '</b>';
                $b = $(html);
                Csw.literals.factory($b, external);

                if (internal.$parent) {
                    internal.$parent.append(external.$);
                }
            } ());

            return external;
        });

} ());

