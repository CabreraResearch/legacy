/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function _cswform() {
    'use strict';

    function form(options) {
        /// <summary> Create or extend an HTML <form /> and return a Csw.form object
        ///     &#10;1 - form(options)
        ///     &#10;2 - form($jqueryElement)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the form.</para>
        /// </param>
        /// <returns type="form">A form object</returns>
        var cswPrivateVar = {
            $parent: '',
            ID: ''
        };
        var cswPublicRet = {};

        (function () {
            var html = '',
                attr = Csw.makeAttr();
            var $form;
            
            if (options) {
                $.extend(cswPrivateVar, options);
            }
            
            html += '<form ';
            attr.add('id', cswPrivateVar.ID);
            html += attr.get();
            html += '>';
            html += '</form>';
            $form = $(html);
            
            Csw.literals.factory($form, cswPublicRet);

            cswPrivateVar.$parent.append(cswPublicRet.$);
            
        } ());

        return cswPublicRet;
    }
    Csw.literals.register('form', form);
    Csw.literals.form = Csw.literals.form || form;

} ());

