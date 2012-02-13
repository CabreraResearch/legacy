/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswDiv() {
    'use strict';

    function div(options) {
        /// <summary> Create or extend an HTML <div /> and return a Csw.divobject
        ///     &#10;1 - div(options)
        ///     &#10;2 - div($jqueryElement)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the div.</para>
        /// <para>options.name: A name for the div.</para>
        /// <para>options.cssclass: CSS class to asign</para>
        /// <para>options.text: Text to display</para>
        /// </param>
        /// <returns type="div">A div object</returns>
        var internal = {
            $parent: '',
            ID: '',
            name: '',
            cssclass: '',
            text: '',
            title: '',
            align: ''
        };
        var external = {};

        (function () {
            var html = '',
                attr = Csw.controls.dom.attr(),
                style = Csw.controls.dom.style();
            var $div;

            if (options) {
                $.extend(internal, options);
            }

            html += '<div ';
            attr.add('id', internal.ID);
            attr.add('name', Csw.string(internal.name, internal.ID));
            attr.add('class', internal.cssclass);
            attr.add('align', internal.align);
            style.add('align', internal.align);
            
            html += attr.get();
            html += style.get();

            html += '>';
            html += Csw.string(internal.text);
            html += '</div>';
            $div = $(html);
            Csw.controls.factory($div, external);

            internal.$parent.append(external.$);
        } ());

        return external;
    }
    Csw.controls.register('div', div);
    Csw.controls.div = Csw.controls.div || div;

} ());

