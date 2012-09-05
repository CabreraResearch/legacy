/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.literals.label = Csw.literals.label ||
        Csw.literals.register('label', function (options) {
            /// <summary> Create or extend an HTML <label /> and return a Csw.label object
            ///     &#10;1 - link(options)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the input.</para>
            /// <para>options.cssclass: CSS class to asign</para>
            /// <para>options.text: Text to display</para>
            /// </param>
            /// <returns type="Csw.literals.label">A label object</returns>
            var cswPrivate = {
                $parent: '',
                ID: '',
                cssclass: '',
                forAttr: '',
                form: '',
                text: '',
                useWide: false,
                onClick: null, //function () {}
                isRequired: false
            };
            var cswPublic = {};

            (function () {
                var html = '',
                    style = Csw.makeStyle(),
                    attr = Csw.makeAttr();
                var $label;
                
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                var cssClass = 'CswLabel ' + cswPrivate.cssclass;
                if(cswPrivate.useWide) {
                    cssClass += ' CswLabelWide';
                }
                
                cswPrivate.ID = Csw.string(cswPrivate.ID, cswPrivate.name);

                html += '<label ';
                attr.add('id', cswPrivate.ID);
                attr.add('class', cssClass);
                attr.add('for', cswPrivate.forAttr);
                attr.add('form', cswPrivate.form);

                html += attr.get();
                html += style.get();

                html += '>';

                var labelText = cswPrivate.value;
                if(Csw.bool(cswPrivate.isRequired)){
                    labelText = Csw.makeRequiredName(labelText);
                }

                //html += Csw.string(cswPrivate.text, cswPrivate.value);
                html += Csw.string(cswPrivate.text, labelText);

                html += '</label>';
                $label = $(html);

                Csw.literals.factory($label, cswPublic);

                if (Csw.isJQuery(cswPrivate.$parent)) {
                    cswPrivate.$parent.append(cswPublic.$);
                }
            } ());

            cswPublic.setFor = function (elementId) {
                cswPublic.propDom('for', elementId);
            };

            return cswPublic;
        });

} ());

