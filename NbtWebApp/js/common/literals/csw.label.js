/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
            var cswPrivateVar = {
                $parent: '',
                ID: '',
                cssclass: '',
                forAttr: '',
                form: '',
                text: '',
                useWide: false,
                onClick: null //function () {}
            };
            var cswPublicRet = {};

            (function () {
                var html = '',
                    style = Csw.makeStyle(),
                    attr = Csw.makeAttr();
                var $label;
                
                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                var cssClass = 'CswLabel ' + cswPrivateVar.cssclass;
                if(cswPrivateVar.useWide) {
                    cssClass += ' CswLabelWide';
                }
                
                cswPrivateVar.ID = Csw.string(cswPrivateVar.ID, cswPrivateVar.name);

                html += '<label ';
                attr.add('id', cswPrivateVar.ID);
                attr.add('class', cssClass);
                attr.add('for', cswPrivateVar.forAttr);
                attr.add('form', cswPrivateVar.form);

                html += attr.get();
                html += style.get();

                html += '>';

                html += Csw.string(cswPrivateVar.text, cswPrivateVar.value);

                html += '</label>';
                $label = $(html);

                Csw.literals.factory($label, cswPublicRet);

                if (Csw.isJQuery(cswPrivateVar.$parent)) {
                    cswPrivateVar.$parent.append(cswPublicRet.$);
                }
            } ());

            cswPublicRet.setFor = function (elementId) {
                cswPublicRet.propDom('for', elementId);
            };

            return cswPublicRet;
        });

} ());

