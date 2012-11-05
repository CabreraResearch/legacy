/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswtextArea() {
    'use strict';
    Csw.literals.textArea = Csw.literals.textArea ||
        Csw.literals.register('textArea', function(options) {
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
            var cswPrivate = {
                $parent: '',
                ID: '',
                name: '',
                placeholder: '',
                cssclass: '',
                value: '',
                text: '',
                maxlength: '',
                autofocus: false,
                isRequired: false,
                rows: 3,
                cols: 25,
                disabled: false,
                readonly: false,
                form: '',
                wrap: '',
                onChange: null //function () {}
            };
            var cswPublic = { };

            (function() {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                var html = '',
                    value = Csw.string(cswPrivate.value, cswPrivate.text),
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle();
                var $textArea;

                cswPrivate.name = Csw.string(cswPrivate.name, cswPrivate.ID);
                cswPrivate.ID = Csw.string(cswPrivate.ID, cswPrivate.name);

                html += '<textarea ';
                attr.add('id', cswPrivate.ID);
                attr.add('name', cswPrivate.name);
                attr.add('placeholder', cswPrivate.placeholder);
                attr.add('maxlength', cswPrivate.maxlength);
                if (Csw.bool(cswPrivate.disabled)) {
                    attr.add('disabled', 'disabled');
                }
                if (Csw.bool(cswPrivate.isRequired)) {
                    attr.add('required', 'required');
                    cswPrivate.cssclass += ' required ';
                }
                if (Csw.bool(cswPrivate.readonly)) {
                    attr.add('readonly', 'readonly');
                }
                if (Csw.bool(cswPrivate.autofocus)) {
                    attr.add('autofocus', cswPrivate.autofocus);
                }
                attr.add('class', cswPrivate.cssclass);
                attr.add('maxlength', cswPrivate.maxlength);
                attr.add('form', cswPrivate.form);
                if (cswPrivate.wrap === 'hard' || cswPrivate.wrap === 'soft') {
                    attr.add('wrap', cswPrivate.wrap);
                }
                attr.add('rows', cswPrivate.rows);
                attr.add('cols', cswPrivate.cols);

                html += attr.get();
                html += style.get();
                html += '>';
                html += value;
                html += '</textarea>';
                $textArea = $(html);
                Csw.literals.factory($textArea, cswPublic);
                cswPrivate.$parent.append(cswPublic.$);

                cswPublic.bind('change', function() {
                    Csw.tryExec(cswPrivate.onChange, cswPublic.val());
                });

            }());

            cswPublic.change = function(func) {
                if (Csw.isFunction(func)) {
                    cswPublic.bind('change', func);
                } else {
                    cswPublic.trigger('change');
                }
            };

            return cswPublic;
        });

} ());

