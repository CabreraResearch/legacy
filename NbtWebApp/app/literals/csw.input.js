/// <reference path="~/app/CswApp-vsdoc.js" />


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
            size: '',
            maxlength: '',
            autofocus: false,
            autocomplete: 'on',
            checked: false,
            canCheck: false, /* if this is a radio or checkbox */
            onChange: null,
            onClick: null,
            onKeyEnter: null,
            isRequired: false
        };
        var cswPublic = {};

        (function () {
            var html = '',
                attr = Csw.makeAttr(),
                style = Csw.makeStyle();
            var $input;

            Csw.extend(cswPrivate, options);
            
            html += '<input ';
            attr.add('id', cswPrivate.ID);
            attr.add('name', cswPrivate.name);
            attr.add('class', cswPrivate.cssclass);
            attr.add('type', cswPrivate.type.name);
            attr.add('placeholder', cswPrivate.placeholder);
            attr.add('size', Csw.string(cswPrivate.size, cswPrivate.type.defaultsize));
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
                Csw.tryExec(cswPrivate.onChange, cswPublic.val(), cswPublic);
            });

            cswPublic.bind('click', function () {
                Csw.tryExec(cswPrivate.onClick, cswPublic.val(), cswPublic);
            });
            
            if(false === Csw.isNullOrEmpty(cswPrivate.onKeyEnter)) {
                cswPublic.bind('keydown', function (event) {
                    if(event.keyCode === 13) {
                        Csw.tryExec(cswPrivate.onKeyEnter, cswPublic.val(), cswPublic);
                    }
                });
            }

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

