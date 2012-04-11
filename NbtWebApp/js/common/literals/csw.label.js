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
            var internal = {
                $parent: '',
                ID: '',
                cssclass: '',
                forAttr: '',
                form: '',
                text: '',
                onClick: null //function () {}
            };
            var external = {};

            (function () {
                var html = '',
                    style = Csw.makeStyle(),
                    attr = Csw.makeAttr();
                var $label;

                if (options) {
                    $.extend(internal, options);
                }

                internal.ID = Csw.string(internal.ID, internal.name);

                html += '<label ';
                attr.add('id', internal.ID);
                attr.add('class', 'CswLabel ' + internal.cssclass);
                attr.add('for', internal.forAttr);
                attr.add('form', internal.form);

                html += attr.get();
                html += style.get();

                html += '>';

                html += Csw.string(internal.text, internal.value);

                html += '</label>';
                $label = $(html);

                Csw.literals.factory($label, external);

                if (Csw.isJQuery(internal.$parent)) {
                    internal.$parent.append(external.$);
                }
            } ());

            external.setFor = function (elementId) {
                external.propDom('for', elementId);
            };

            return external;
        });

} ());

