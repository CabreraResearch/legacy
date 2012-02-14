/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

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
        var internal = {
            $parent: '',
            ID: '',
            name: '',
            placeholder: '',
            cssclass: '',
            value: '',
            text: '',
            width: '',
            autofocus: false,
            onChange: null //function () {}
        };
        var external = {};

        (function () {
            if (options) {
                $.extend(internal, options);
            }

            var html = '',
                value = Csw.string(internal.value, internal.text),
                attr = Csw.controls.dom.attributes(),
                style = Csw.controls.dom.style();
            var $textArea;

            internal.name = Csw.string(internal.name, internal.ID);
            internal.ID = Csw.string(internal.ID, internal.name);

            html += '<textarea ';
            attr.add('id', internal.ID);
            attr.add('name', internal.name);
            attr.add('class', internal.cssclass);
            attr.add('placeholder', internal.placeholder);
            attr.add('width', Csw.string(internal.width, internal.type.defaultwidth));
            if (Csw.bool(internal.autofocus)) {
                attr.add('autofocus', internal.autofocus);
            }

            if (internal.type === Csw.enums.inputTypes.checkbox || internal.type === Csw.enums.inputTypes.radio) {
                if (Csw.bool(internal.checked) || internal.checked === 'checked') {
                    attr.add('checked', true);
                }
            }
            html += attr.get();
            html += style.get();
            html += '>';
            html += value;
            html += '</textarea>';
            $textArea = $(html);
            Csw.controls.factory($textArea, external);
            internal.$parent.append(external.$);

            if (Csw.isFunction(internal.onChange)) {
                external.bind('change', internal.onChange);
            }

        } ());

        external.change = function (func) {
            if (Csw.isFunction(func)) {
                external.bind('change', func);
            } else {
                external.trigger('change');
            }
        };

        return external;
    }
    Csw.controls.register('textArea', textArea);
    Csw.controls.textArea = Csw.controls.textArea || textArea;

} ());

