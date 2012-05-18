/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
        var cswPrivateVar = {
            $parent: '',
            ID: '',
            cssclass: '',
            text: '',
            value: '',
            nobr: false
        };
        var cswPublicRet = {};

        (function () {
            var html = '',
                attr = Csw.makeAttr();
            var $span;
            var spanText;

            html += '<span ';
            if (options) {
                $.extend(cswPrivateVar, options);
            } 
            
            attr.add('id', cswPrivateVar.ID);
            attr.add('class', cswPrivateVar.cssclass);
            spanText = Csw.string(cswPrivateVar.text, cswPrivateVar.value);

            html += attr.get();
            html += '>';
            if(cswPrivateVar.nobr) {
                html += '<nobr>';
            }
            html += spanText;
            if(cswPrivateVar.nobr) {
                html += '</nobr>';
            }
            html += '</span>';
            $span = $(html);
            Csw.literals.factory($span, cswPublicRet);

            if (cswPrivateVar.$parent) {
                cswPrivateVar.$parent.append(cswPublicRet.$);
            }
        } ());

        return cswPublicRet;
    }
    Csw.literals.register('span', span);
    Csw.literals.span = Csw.literals.span || span;

} ());

