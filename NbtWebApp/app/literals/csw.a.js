/// <reference path="~app/CswApp-vsdoc.js" />


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
            var cswPrivate = {
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
            var cswPublic = {};

            (function () {
                var html = '',
                    style = Csw.makeStyle(),
                    attr = Csw.makeAttr();
                var $link;

                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                cswPrivate.ID = Csw.string(cswPrivate.ID, cswPrivate.name);

                html += '<a ';
                attr.add('id', cswPrivate.ID);
                attr.add('class', cswPrivate.cssclass);
                attr.add('href', cswPrivate.href);
                attr.add('type', cswPrivate.type);
                //attr.add('title', cswPrivate.title);//case 25692
                attr.add('rel', cswPrivate.rel);
                attr.add('media', cswPrivate.media);
                attr.add('target', cswPrivate.target);

                html += attr.get();
                html += style.get();

                html += '>';

                html += Csw.string(cswPrivate.text, cswPrivate.value);

                html += '</a>';
                $link = $(html);

                Csw.literals.factory($link, cswPublic);

                cswPublic.propDom('title', cswPrivate.title);//case 25692

                // Click binding

                if (Csw.isFunction(cswPrivate.onClick)) {
                    cswPublic.bind('click', function (event, ui) {
                        cswPrivate.click();
                        var retval = Csw.tryExec(cswPrivate.onClick, event, ui);
                        if (cswPrivate.href === '#') {
                            return false;
                        } else {
                            return retval;
                        }
                    });
                } else {
                    cswPublic.bind('click', cswPrivate.click);
                }

                cswPrivate.$parent.append(cswPublic.$);
            } ());

            cswPrivate.click = function () {
                cswPrivate.toggle();
            };

            cswPrivate.toggle = function () {
                if (cswPublic.toggleState === Csw.enums.toggleState.on) {
                    cswPublic.toggleState = Csw.enums.toggleState.off;
                } else if (cswPublic.toggleState === Csw.enums.toggleState.off) {
                    cswPublic.toggleState = Csw.enums.toggleState.on;
                }
            };

            cswPublic.toggleState = Csw.enums.toggleState.off;

            cswPublic.click = function (func) {
                if (Csw.isFunction(func)) {
                    return cswPublic.bind('click', func);
                } else {
                    return cswPublic;
                }
            };

            cswPublic.disable = function () {
                cswPublic.addClass('disabled');
            };
            cswPublic.enable = function () {
                cswPublic.removeClass('disabled');
            };

            return cswPublic;
        });

} ());

