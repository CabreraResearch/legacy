/// <reference path="~app/CswApp-vsdoc.js" />


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
            var cswPrivate = {
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
            var cswPublic = { };

            (function() {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle(),
                    divText = Csw.string(cswPrivate.text);
                var $div;

                attr.add('id', cswPrivate.ID);
                attr.add('name', Csw.string(cswPrivate.name, cswPrivate.ID));
                attr.add('class', cswPrivate.cssclass);
                attr.add('value', cswPrivate.value);
                attr.add('align', cswPrivate.align);
                cswPrivate.styles.align = cswPrivate.align;
                cswPrivate.styles.height = cswPrivate.height;
                cswPrivate.styles.width = cswPrivate.width;
                style.set(cswPrivate.styles);

                html += '<div ';

                html += attr.get();
                html += style.get();

                html += '>';
                html += divText;
                html += '</div>';
                $div = $(html);
                Csw.literals.factory($div, cswPublic);

                if (Csw.isFunction(cswPrivate.onClick)) {
                    cswPublic.bind('click', cswPrivate.onClick);
                }
                if (false === Csw.isNullOrEmpty(cswPrivate.$parent)) {
                    cswPrivate.$parent.append(cswPublic.$);
                }
            }());

            return cswPublic;
        });

} ());

