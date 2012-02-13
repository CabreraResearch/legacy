/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswSpan() {
    'use strict';

    function span(options) {
        /// <summary> Create or extend an HTML <span /> and return a Csw.span object
        ///     &#10;1 - span(options)
        ///     &#10;2 - span('Text')
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the span.</para>
        /// <para>options.cssclass: CSS class to asign</para>
        /// <para>options.text: Text to display</para>
        /// </param>
        /// <returns type="span">A span object</returns>
        var internal = {
            $parent: '',
            ID: '',
            cssclass: '',
            text: ''
        };
        var external = {};

        (function () {
            var html = '',
                attr = Csw.controls.dom.attr();
            var $span;
            var spanText;

            html += '<span ';
            if (options) {
                $.extend(internal, options);
                attr.add('id', internal.ID);
                attr.add('class', internal.cssclass);
                spanText = Csw.string(internal.text);
            } else {
                spanText = Csw.string(options);
            }
            html += attr.get();
            html += '>';
            html += spanText;
            html += '</span>';
            $span = $(html);
            Csw.controls.factory($span, external);

            internal.$parent.append(external.$);
        } ());

        return external;
    }
    Csw.controls.register('span', span);
    Csw.controls.span = Csw.controls.span || span;

} ());

