/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
        var cswPrivate = {
            $parent: '',
            ID: '',
            name: '',
            type: Csw.enums.inputTypes.text,
            placeholder: '',
            cssclass: '',
            value: '',
            labelText: null,
            width: '',
            maxlength: '',
            autofocus: false,
            autocomplete: 'on',
            checked: false,
            canCheck: false, /* if this is a radio or checkbox */
            onChange: null,
            onClick: null
        };
        var cswPublic = {};

        (function () {
            var html = '',
                attr = Csw.makeAttr(),
                style = Csw.makeStyle();
            var $input;

            if (options) {
                $.extend(cswPrivate, options);
            }

            cswPrivate.name = Csw.string(cswPrivate.name, cswPrivate.ID);
            cswPrivate.ID = Csw.string(cswPrivate.ID, cswPrivate.name);

            html += '<input ';
            attr.add('id', cswPrivate.ID);
            attr.add('name', cswPrivate.name);
            attr.add('class', cswPrivate.cssclass);
            attr.add('type', cswPrivate.type.name);
            attr.add('placeholder', cswPrivate.placeholder);
            attr.add('width', Csw.string(cswPrivate.width, cswPrivate.type.defaultwidth));
            style.add('width', Csw.string(cswPrivate.width, cswPrivate.type.defaultwidth));
            attr.add('maxlength', cswPrivate.maxlength);
            //attr.add('value', cswPrivate.value);//case 26109

            if (Csw.bool(cswPrivate.autofocus)) {
                attr.add('autofocus', cswPrivate.autofocus);
            }
            if (cswPrivate.type.autocomplete === true && cswPrivate.autocomplete === 'on') {
                attr.add('autocomplete', 'on');
            }
            cswPrivate.canCheck = cswPrivate.type === Csw.enums.inputTypes.checkbox || cswPrivate.type === Csw.enums.inputTypes.radio;
            if (cswPrivate.canCheck) {
                if (Csw.bool(cswPrivate.checked) || cswPrivate.checked === 'checked') {
                    attr.add('checked', true);
                }
            }

            html += attr.get();
            html += style.get();
            html += ' />';

            $input = $(html);
            Csw.literals.factory($input, cswPublic);

            cswPublic.propDom('value', cswPrivate.value); //case 26109

            if (Csw.isJQuery(cswPrivate.$parent)) {
                cswPrivate.$parent.append(cswPublic.$);
            }
            cswPublic.bind('change', function () {
                Csw.tryExec(cswPrivate.onChange, cswPublic.val());
            });

            cswPublic.bind('click', function () {
                Csw.tryExec(cswPrivate.onClick, cswPublic.val());
            });
        } ());

        cswPublic.change = function (func) {
            if (Csw.isFunction(func)) {
                cswPublic.bind('change', func);
            } else {
                cswPublic.trigger('change');
            }
        };

        cswPublic.click = function (func) {
            if (Csw.isFunction(func)) {
                cswPublic.bind('click', func);
            } else {
                cswPublic.trigger('click');
            }
        };

        cswPublic.checked = function (value) {
            var ret = cswPublic;
            if (cswPrivate.canCheck) {
                if (arguments.length === 1) {
                    if (value) {
                        cswPublic.propDom({ 'checked': true });
                    } else {
                        //if (window.internetExplorerVersionNo !== -1) {
                        cswPublic.$.removeAttr('checked');
                        //}
                    }
                } else {
                    ret = cswPublic.$.is(':checked');
                }
            }
            return ret;
        };

        return cswPublic;
    }
    Csw.literals.register('input', input);
    Csw.literals.input = Csw.literals.input || input;

} ());

