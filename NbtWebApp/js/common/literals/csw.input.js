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
        var cswPrivateVar = {
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
        var cswPublicRet = {};

        (function () {
            var html = '',
                attr = Csw.makeAttr(),
                style = Csw.makeStyle();
            var $input;

            if (options) {
                $.extend(cswPrivateVar, options);
            }

            cswPrivateVar.name = Csw.string(cswPrivateVar.name, cswPrivateVar.ID);
            cswPrivateVar.ID = Csw.string(cswPrivateVar.ID, cswPrivateVar.name);

            html += '<input ';
            attr.add('id', cswPrivateVar.ID);
            attr.add('name', cswPrivateVar.name);
            attr.add('class', cswPrivateVar.cssclass);
            attr.add('type', cswPrivateVar.type.name);
            attr.add('placeholder', cswPrivateVar.placeholder);
            attr.add('width', Csw.string(cswPrivateVar.width, cswPrivateVar.type.defaultwidth));
            style.add('width', Csw.string(cswPrivateVar.width, cswPrivateVar.type.defaultwidth));
            attr.add('maxlength', cswPrivateVar.maxlength);
            //attr.add('value', cswPrivateVar.value);//case 26109

            if (Csw.bool(cswPrivateVar.autofocus)) {
                attr.add('autofocus', cswPrivateVar.autofocus);
            }
            if (cswPrivateVar.type.autocomplete === true && cswPrivateVar.autocomplete === 'on') {
                attr.add('autocomplete', 'on');
            }
            cswPrivateVar.canCheck = cswPrivateVar.type === Csw.enums.inputTypes.checkbox || cswPrivateVar.type === Csw.enums.inputTypes.radio
            if (cswPrivateVar.canCheck) {
                if (Csw.bool(cswPrivateVar.checked) || cswPrivateVar.checked === 'checked') {
                    attr.add('checked', true);
                }
            }

            html += attr.get();
            html += style.get();
            html += ' />';

            $input = $(html);
            Csw.literals.factory($input, cswPublicRet);

            cswPublicRet.propDom('value', cswPrivateVar.value);//case 26109
            
            if (Csw.isJQuery(cswPrivateVar.$parent)) {
                cswPrivateVar.$parent.append(cswPublicRet.$);
            }
            if (Csw.isFunction(cswPrivateVar.onChange)) {
                cswPublicRet.bind('change', cswPrivateVar.onChange);
            }
            if (Csw.isFunction(cswPrivateVar.onClick)) {
                cswPublicRet.bind('click', cswPrivateVar.onClick);
            }
        } ());

        cswPublicRet.change = function (func) {
            if (Csw.isFunction(func)) {
                cswPublicRet.bind('change', func);
            } else {
                cswPublicRet.trigger('change');
            }
        };

        cswPublicRet.click = function (func) {
            if (Csw.isFunction(func)) {
                cswPublicRet.bind('click', func);
            } else {
                cswPublicRet.trigger('click');
            }
        };

        cswPublicRet.checked = function (value) {
            var ret = cswPublicRet;
            if (cswPrivateVar.canCheck) {
                if (arguments.length === 1) {
                    if (value) {
                        cswPublicRet.propDom({ 'checked': true });
                    } else {
                        //if (window.internetExplorerVersionNo !== -1) {
                            cswPublicRet.$.removeAttr('checked');
                        //}
                    }
                } else {
                    ret = cswPublicRet.$.is(':checked');
                }
            }
            return ret;
        };

        return cswPublicRet;
    }
    Csw.literals.register('input', input);
    Csw.literals.input = Csw.literals.input || input;

} ());

