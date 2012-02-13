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
                attr = Csw.controls.dom.attr(),
                style = Csw.controls.dom.style();
            var $input;

            if (options) {
                $.extend(internal, options);
            }

            internal.name = Csw.string(internal.name, internal.ID);
            internal.ID = Csw.string(internal.ID, internal.name);

            html += '<input ';
            attr.add('id', internal.ID);
            attr.add('name', internal.name);
            attr.add('class', internal.class);
            attr.add('type', internal.type);
            attr.add('placeholder', internal.placeholder);
            attr.add('width', Csw.string(internal.width, internal.type.defaultwidth));
            attr.add('autofocus', internal.autofocus);
            attr.add('maxlength', internal.maxlength);

            if (internal.type.autocomplete === true && internal.autocomplete === 'on') {
                attr.add('autocomplete', 'on');
            }
            if (Csw.bool(internal.type.value.required)) {
                attr.add('value', internal.value);
            }

            html += attr.get();
            html += style.get();

            html += ' />';
            $input = $(html);
            if (Csw.isFunction(internal.onChange)) {
                $input.change(internal.onChange);
            }

            Csw.controls.domExtend($input, external);

            internal.$parent.append(external.$);

        } ());

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

