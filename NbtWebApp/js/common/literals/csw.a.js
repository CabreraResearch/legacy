/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    'use strict';

    Csw.literals.a = Csw.literals.a ||
        Csw.literals.register('a', function (options) {
            /// <summary> Create or extend an HTML <a /> and return a Csw.link object
            ///     &#10;1 - link(options)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the input.</para>
            /// <para>options.cssclass: CSS class to asign</para>
            /// <para>options.text: Text to display</para>
            /// </param>
            /// <returns type="link">A link object</returns>
            var internal = {
                $parent: '',
                ID: '',
                cssclass: '',
                text: '',
                href: '#',
                type: '',
                title: '',
                rel: '',
                media: '',
                target: '',
                onClick: null //function () {}
            };
            var external = {};

            (function () {
                var html = '',
                    style = Csw.makeStyle(),
                    attr = Csw.makeAttr();
                var $link;

                if (options) {
                    $.extend(internal, options);
                }

                internal.ID = Csw.string(internal.ID, internal.name);

                html += '<a ';
                attr.add('id', internal.ID);
                attr.add('class', internal.cssclass);
                attr.add('href', internal.href);
                attr.add('type', internal.type);
                //attr.add('title', internal.title);//case 25692
                attr.add('rel', internal.rel);
                attr.add('media', internal.media);
                attr.add('target', internal.target);

                html += attr.get();
                html += style.get();

                html += '>';

                html += Csw.string(internal.text, internal.value);

                html += '</a>';
                $link = $(html);

                Csw.literals.factory($link, external);

                external.propDom('title', internal.title);//case 25692

                // Click binding

                if (Csw.isFunction(internal.onClick)) {
                    external.bind('click', function (event, ui) {
                        internal.click();
                        var retval = Csw.tryExec(internal.onClick, event, ui);
                        if (internal.href === '#') {
                            return false;
                        } else {
                            return retval;
                        }
                    });
                } else {
                    external.bind('click', internal.click);
                }

                internal.$parent.append(external.$);
            } ());

            internal.click = function () {
                internal.toggle();
            };

            internal.toggle = function () {
                if (external.toggleState === Csw.enums.toggleState.on) {
                    external.toggleState = Csw.enums.toggleState.off;
                } else if (external.toggleState === Csw.enums.toggleState.off) {
                    external.toggleState = Csw.enums.toggleState.on;
                }
            };

            external.toggleState = Csw.enums.toggleState.off;

            external.click = function (func) {
                if (Csw.isFunction(func)) {
                    return external.bind('click', func);
                } else {
                    return external;
                }
            };

            external.disable = function () {
                external.addClass('disabled');
            };
            external.enable = function () {
                external.removeClass('disabled');
            };

            return external;
        });

} ());

