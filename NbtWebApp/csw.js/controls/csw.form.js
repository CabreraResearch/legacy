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
            var html = '';
            var $form;
            var isjQuery = Csw.isJQuery(options);
            
            if (options) {
                $.extend(internal, options);
            }
            
            if (isjQuery) {
                $form = options;
            } else {
                html += '<form ';
                html += ' id="' + Csw.string(internal.ID) + '" ';
                html += '>';
                html += '</form>';
                $form = $(html);
            }
            Csw.controls.domExtend($form, external);

            if (false === isjQuery) {
                internal.$parent.append(external.$);
            }
        } ());

        return external;
    }
    Csw.controls.register('form', form);
    Csw.controls.form = Csw.controls.form || form;

} ());

