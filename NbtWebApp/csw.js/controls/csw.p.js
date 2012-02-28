/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswP() {
    'use strict';

    function p(options) {
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
            ID: '',
            name: '',
            cssclass: '',
            text: '',
            styles: {}
        };
        var external = {};

        (function () {
            if (options) {
                $.extend(internal, options);
            }
            var html = '',
                attr = Csw.controls.dom.attributes(),
                style = Csw.controls.dom.style();

            var $p;

            attr.add('id', internal.ID);
            attr.add('class', internal.cssclass);
            style.set(internal.styles);

            html += '<p ';

            html += attr.get();
            html += style.get();

            html += '>';
            html += Csw.string(internal.text);
            html += '</p>';
            $p = $(html);
            Csw.controls.factory($p, external);

            if (Csw.isFunction(internal.onClick)) {
                external.bind('click', internal.onClick);
            }
            if (internal.$parent) {
                internal.$parent.append(external.$);
            }
        } ());

        return external;
    }
    Csw.controls.register('p', p);
    Csw.controls.p = Csw.controls.p || p;

} ());

