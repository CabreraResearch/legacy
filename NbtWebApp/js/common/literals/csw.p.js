/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    
    Csw.literals.p = Csw.literals.p ||
        Csw.literals.register('p', function(options) {
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
                ID: '',
                name: '',
                cssclass: '',
                text: '',
                styles: { }
            };
            var cswPublic = { };

            (function() {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle();

                var $p;

                attr.add('id', cswPrivate.ID);
                attr.add('class', cswPrivate.cssclass);
                style.set(cswPrivate.styles);

                html += '<p ';

                html += attr.get();
                html += style.get();

                html += '>';
                html += Csw.string(cswPrivate.text);
                html += '</p>';
                $p = $(html);
                Csw.literals.factory($p, cswPublic);

                if (Csw.isFunction(cswPrivate.onClick)) {
                    cswPublic.bind('click', cswPrivate.onClick);
                }
                if (cswPrivate.$parent) {
                    cswPrivate.$parent.append(cswPublic.$);
                }
            }());

            return cswPublic;
        });

} ());

