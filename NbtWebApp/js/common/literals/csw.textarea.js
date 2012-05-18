/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function _cswtextArea() {
    'use strict';

    function textArea(options) {
        /// <summary> Create or extend an HTML <textarea /> and return a Csw.textarea object
        ///     &#10;1 - textarea(options)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the textarea.</para>
        /// <para>options.cssclass: CSS class to asign</para>
        /// <para>options.value: Text to display</para>
        /// </param>
        /// <returns type="textarea">A textarea object</returns>
        var cswPrivateVar = {
            $parent: '',
            ID: '',
            name: '',
            placeholder: '',
            cssclass: '',
            value: '',
            text: '',
            maxlength: '',
            autofocus: false,
            required: false,
            rows: 3,
            cols: 25,
            disabled: false,
            readonly: false,
            form: '',
            wrap: '',
            onChange: null //function () {}
        };
        var cswPublicRet = {};

        (function () {
            if (options) {
                $.extend(cswPrivateVar, options);
            }

            var html = '',
                value = Csw.string(cswPrivateVar.value, cswPrivateVar.text),
                attr = Csw.makeAttr(),
                style = Csw.makeStyle();
            var $textArea;

            cswPrivateVar.name = Csw.string(cswPrivateVar.name, cswPrivateVar.ID);
            cswPrivateVar.ID = Csw.string(cswPrivateVar.ID, cswPrivateVar.name);

            html += '<textarea ';
            attr.add('id', cswPrivateVar.ID);
            attr.add('name', cswPrivateVar.name);
            attr.add('placeholder', cswPrivateVar.placeholder);
            attr.add('maxlength', cswPrivateVar.maxlength);
            if (Csw.bool(cswPrivateVar.disabled)) {
                attr.add('disabled', 'disabled');
            }
            if (Csw.bool(cswPrivateVar.required)) {
                attr.add('required', 'required');
                cswPrivateVar.cssclass += ' required ';
            }
            if (Csw.bool(cswPrivateVar.readonly)) {
                attr.add('readonly', 'readonly');
            }
            if (Csw.bool(cswPrivateVar.autofocus)) {
                attr.add('autofocus', cswPrivateVar.autofocus);
            }
            attr.add('class', cswPrivateVar.cssclass);
            attr.add('maxlength', cswPrivateVar.maxlength);
            attr.add('form', cswPrivateVar.form);
            if (cswPrivateVar.wrap === 'hard' || cswPrivateVar.wrap === 'soft') {
                attr.add('wrap', cswPrivateVar.wrap);
            }
            attr.add('rows', cswPrivateVar.rows);
            attr.add('cols', cswPrivateVar.cols);

            html += attr.get();
            html += style.get();
            html += '>';
            html += value;
            html += '</textarea>';
            $textArea = $(html);
            Csw.literals.factory($textArea, cswPublicRet);
            cswPrivateVar.$parent.append(cswPublicRet.$);

            if (Csw.isFunction(cswPrivateVar.onChange)) {
                cswPublicRet.bind('change', cswPrivateVar.onChange);
            }

        } ());

        cswPublicRet.change = function (func) {
            if (Csw.isFunction(func)) {
                cswPublicRet.bind('change', func);
            } else {
                cswPublicRet.trigger('change');
            }
        };

        return cswPublicRet;
    }
    Csw.literals.register('textArea', textArea);
    Csw.literals.textArea = Csw.literals.textArea || textArea;

} ());

