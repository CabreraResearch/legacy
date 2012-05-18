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
            var cswPrivateVar = {
                $parent: '',
                ID: '',
                name: '',
                cssclass: '',
                text: '',
                styles: { }
            };
            var cswPublicRet = { };

            (function() {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle();

                var $p;

                attr.add('id', cswPrivateVar.ID);
                attr.add('class', cswPrivateVar.cssclass);
                style.set(cswPrivateVar.styles);

                html += '<p ';

                html += attr.get();
                html += style.get();

                html += '>';
                html += Csw.string(cswPrivateVar.text);
                html += '</p>';
                $p = $(html);
                Csw.literals.factory($p, cswPublicRet);

                if (Csw.isFunction(cswPrivateVar.onClick)) {
                    cswPublicRet.bind('click', cswPrivateVar.onClick);
                }
                if (cswPrivateVar.$parent) {
                    cswPrivateVar.$parent.append(cswPublicRet.$);
                }
            }());

            return cswPublicRet;
        });

} ());

