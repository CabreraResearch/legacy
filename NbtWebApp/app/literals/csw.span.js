/// <reference path="~app/CswApp-vsdoc.js" />


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
        var cswPrivate = {
            $parent: '',
            ID: '',
            cssclass: '',
            text: '',
            value: '',
            nobr: false
        };
        var cswPublic = {};

        (function () {
            var html = '',
                attr = Csw.makeAttr();
            var $span;
            var spanText;

            html += '<span ';
            if (options) {
                Csw.extend(cswPrivate, options);
            } 
            
            attr.add('id', cswPrivate.ID);
            attr.add('class', cswPrivate.cssclass);
            spanText = Csw.string(cswPrivate.text, cswPrivate.value);

            html += attr.get();
            html += '>';
            if(cswPrivate.nobr) {
                html += '<nobr>';
            }
            html += spanText;
            if(cswPrivate.nobr) {
                html += '</nobr>';
            }
            html += '</span>';
            $span = $(html);
            Csw.literals.factory($span, cswPublic);

            if (cswPrivate.$parent) {
                cswPrivate.$parent.append(cswPublic.$);
            }
        } ());

        return cswPublic;
    }
    Csw.literals.register('span', span);
    Csw.literals.span = Csw.literals.span || span;

} ());

