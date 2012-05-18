/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.literals.div = Csw.literals.div ||
        Csw.literals.register('div', function(options) {
            'use strict';
            /// <summary> Create or extend an HTML <div /> and return a Csw.divobject
            ///     &#10;1 - div(options)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the div.</para>
            /// <para>options.name: A name for the div.</para>
            /// <para>options.cssclass: CSS class to asign</para>
            /// <para>options.text: Text to display</para>
            /// </param>
            /// <returns type="div">A div object</returns>
            var cswPrivateVar = {
                $parent: '',
                ID: '',
                name: '',
                cssclass: '',
                text: '',
                value: '',
                title: '',
                align: '',
                height: '',
                width: '',
                styles: { },
                onClick: null
            };
            var cswPublicRet = { };

            (function() {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle(),
                    divText = Csw.string(cswPrivateVar.text);
                var $div;

                attr.add('id', cswPrivateVar.ID);
                attr.add('name', Csw.string(cswPrivateVar.name, cswPrivateVar.ID));
                attr.add('class', cswPrivateVar.cssclass);
                attr.add('value', cswPrivateVar.value);
                attr.add('align', cswPrivateVar.align);
                cswPrivateVar.styles.align = cswPrivateVar.align;
                cswPrivateVar.styles.height = cswPrivateVar.height;
                cswPrivateVar.styles.width = cswPrivateVar.width;
                style.set(cswPrivateVar.styles);

                html += '<div ';

                html += attr.get();
                html += style.get();

                html += '>';
                html += divText;
                html += '</div>';
                $div = $(html);
                Csw.literals.factory($div, cswPublicRet);

                if (Csw.isFunction(cswPrivateVar.onClick)) {
                    cswPublicRet.bind('click', cswPrivateVar.onClick);
                }
                if (false === Csw.isNullOrEmpty(cswPrivateVar.$parent)) {
                    cswPrivateVar.$parent.append(cswPublicRet.$);
                }
            }());

            return cswPublicRet;
        });

} ());

