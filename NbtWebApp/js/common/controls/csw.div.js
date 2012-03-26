/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.div = Csw.controls.div ||
        Csw.controls.register('div', function(options) {
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
            var internal = {
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
            var external = { };

            (function() {
                if (options) {
                    $.extend(internal, options);
                }
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle(),
                    divText = Csw.string(internal.text);
                var $div;

                attr.add('id', internal.ID);
                attr.add('name', Csw.string(internal.name, internal.ID));
                attr.add('class', internal.cssclass);
                attr.add('value', internal.value);
                attr.add('align', internal.align);
                internal.styles.align = internal.align;
                internal.styles.height = internal.height;
                internal.styles.width = internal.width;
                style.set(internal.styles);

                html += '<div ';

                html += attr.get();
                html += style.get();

                html += '>';
                html += divText;
                html += '</div>';
                $div = $(html);
                Csw.controls.factory($div, external);

                if (Csw.isFunction(internal.onClick)) {
                    external.bind('click', internal.onClick);
                }
                if (false === Csw.isNullOrEmpty(internal.$parent)) {
                    internal.$parent.append(external.$);
                }
            }());

            return external;
        });

} ());

