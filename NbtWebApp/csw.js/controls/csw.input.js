/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswInput() {
    'use strict';

    function input(options) {
        /// <summary> Create or extend an HTML <input /> and return a Csw.input object
        ///     &#10;1 - input(options)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the input.</para>
        /// <para>options.cssclass: CSS class to asign</para>
        /// <para>options.text: Text to display</para>
        /// </param>
        /// <returns type="input">An input object</returns>
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
            checked: false,
            canCheck: false, /* if this is a radio or checkbox */
            onChange: null,
            onClick: null
        };
        var external = {};

        (function () {
            var html = '',
                attr = Csw.controls.dom.attributes(),
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
            attr.add('class', internal.cssclass);
            attr.add('type', internal.type.name);
            attr.add('placeholder', internal.placeholder);
            attr.add('width', Csw.string(internal.width, internal.type.defaultwidth));
            style.add('width', Csw.string(internal.width, internal.type.defaultwidth));
            attr.add('maxlength', internal.maxlength);
            attr.add('value', internal.value);

            if (Csw.bool(internal.autofocus)) {
                attr.add('autofocus', internal.autofocus);
            }
            if (internal.type.autocomplete === true && internal.autocomplete === 'on') {
                attr.add('autocomplete', 'on');
            }
            internal.canCheck = internal.type === Csw.enums.inputTypes.checkbox || internal.type === Csw.enums.inputTypes.radio
            if (internal.canCheck) {
                if (Csw.bool(internal.checked) || internal.checked === 'checked') {
                    attr.add('checked', true);
                }
            }

            html += attr.get();
            html += style.get();
            html += ' />';

            $input = $(html);
            Csw.controls.factory($input, external);
            internal.$parent.append(external.$);

            if (Csw.isFunction(internal.onChange)) {
                external.bind('change', internal.onChange);
            }
            if (Csw.isFunction(internal.onClick)) {
                external.bind('click', internal.onClick);
            }
        } ());

        external.change = function (func) {
            if (Csw.isFunction(func)) {
                external.bind('change', func);
            } else {
                external.trigger('change');
            }
        };

        external.click = function (func) {
            if (Csw.isFunction(func)) {
                external.bind('click', func);
            } else {
                external.trigger('click');
            }
        };

        external.checked = function (value) {
            var ret = external;
            if (internal.canCheck) {
                if (internal.type == arguments.length === 1) {
                    if (value) {
                        external.propDom({ 'checked': true });
                    } else {
                        if(window.abandonHope) {
                            external.$.removeAttr('checked');    
                        }
                    }
                } else {
                    ret = external.$.is(':checked');
                }
            }
            return ret;
        };

        return external;
    }
    Csw.controls.register('input', input);
    Csw.controls.input = Csw.controls.input || input;

} ());

