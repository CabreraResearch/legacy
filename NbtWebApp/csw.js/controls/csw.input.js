/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswInput() {
    'use strict';

    function input(options) {
        /// <summary> Create or extend an HTML <input /> and return a Csw.input object
        ///     &#10;1 - input(options)
        ///     &#10;2 - input($jqueryElement)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the input.</para>
        /// <para>options.cssclass: CSS class to asign</para>
        /// <para>options.text: Text to display</para>
        /// </param>
        /// <returns type="input">A input object</returns>
        var internal = {
            $parent: '',
            ID: '',
            name: '',
            type: Csw.enums.inputTypes.text,
            placeholder: '',
            cssclass: '',
            value: '',
            width: '',
            maxlength: '',
            autofocus: false,
            autocomplete: 'on',
            onChange: null //function () {}
        };
        var external = {};

        (function () {
            var html = '',
                style = Csw.controls.dom.style();
            var $input;

            if (options) {
                $.extend(internal, options);
            }

            internal.name = Csw.string(internal.name, internal.ID);
            internal.ID = Csw.string(internal.ID, internal.name);

            html += '<input ';
            html += ' id="' + Csw.string(internal.ID) + '" ';
            html += ' name="' + Csw.string(internal.name) + '" ';
            html += ' class="' + Csw.string(internal.cssclass) + '" ';

            if (false === Csw.isNullOrEmpty(internal.type)) {
                html += ' type="' + Csw.string(internal.type.name) + '" ';
                if (Csw.bool(internal.type.placeholder) && false === Csw.isNullOrEmpty(internal.placeholder)) {
                    html += ' placeholder="' + internal.placeholder + '" ';
                }
                if (internal.type.autocomplete === true && internal.autocomplete === 'on') {
                    html += ' autocomplete="on" ';
                }

                internal.value = Csw.string(internal.value);
                if (Csw.bool(internal.type.value.required) || false === Csw.isNullOrEmpty(internal.value)) {
                    html += ' value="' + Csw.string(internal.value) + '" ';
                }
            }
            internal.width = Csw.string(internal.width, internal.type.defaultwidth);

            if (false === Csw.isNullOrEmpty(internal.width)) {
                style.add('width', internal.width);
            }
            if (Csw.bool(internal.autofocus)) {
                html += ' autofocus="' + internal.autofocus + '" ';
            }
            if (false === Csw.isNullOrEmpty(internal.maxlength)) {
                html += ' maxlength="' + internal.maxlength + '" ';
            }

            html += style.get();

            html += ' />';
            $input = $(html);
            if (Csw.isFunction(internal.onChange)) {
                $input.change(internal.onChange);
            }

            Csw.controls.domExtend($input, external);

            internal.$parent.append(external.$);

        }());

        external.change = function (func) {
            if (Csw.isFunction(func)) {
                external.bind('change', func);
            }
        };

        return external;
    }
    Csw.controls.register('input', input);
    Csw.controls.input = Csw.controls.input || input;

} ());

