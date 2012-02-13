/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

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
        var internal = {
            $parent: '',
            ID: ''
        };
        var external = {};

        (function () {
            var html = '',
                attr = Csw.controls.dom.attributes();
            var $form;
            
            if (options) {
                $.extend(internal, options);
            }
            
            html += '<form ';
            attr.add('id', internal.ID);
            html += attr.get();
            html += '>';
            html += '</form>';
            $form = $(html);
            
            Csw.controls.factory($form, external);

            internal.$parent.append(external.$);
            
        } ());

        return external;
    }
    Csw.controls.register('form', form);
    Csw.controls.form = Csw.controls.form || form;

} ());

