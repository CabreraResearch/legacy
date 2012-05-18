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
            var cswPrivateVar = {
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
            var cswPublicRet = {};

            (function () {
                var html = '',
                    style = Csw.makeStyle(),
                    attr = Csw.makeAttr();
                var $link;

                if (options) {
                    $.extend(cswPrivateVar, options);
                }

                cswPrivateVar.ID = Csw.string(cswPrivateVar.ID, cswPrivateVar.name);

                html += '<a ';
                attr.add('id', cswPrivateVar.ID);
                attr.add('class', cswPrivateVar.cssclass);
                attr.add('href', cswPrivateVar.href);
                attr.add('type', cswPrivateVar.type);
                //attr.add('title', cswPrivateVar.title);//case 25692
                attr.add('rel', cswPrivateVar.rel);
                attr.add('media', cswPrivateVar.media);
                attr.add('target', cswPrivateVar.target);

                html += attr.get();
                html += style.get();

                html += '>';

                html += Csw.string(cswPrivateVar.text, cswPrivateVar.value);

                html += '</a>';
                $link = $(html);

                Csw.literals.factory($link, cswPublicRet);

                cswPublicRet.propDom('title', cswPrivateVar.title);//case 25692

                // Click binding

                if (Csw.isFunction(cswPrivateVar.onClick)) {
                    cswPublicRet.bind('click', function (event, ui) {
                        cswPrivateVar.click();
                        var retval = Csw.tryExec(cswPrivateVar.onClick, event, ui);
                        if (cswPrivateVar.href === '#') {
                            return false;
                        } else {
                            return retval;
                        }
                    });
                } else {
                    cswPublicRet.bind('click', cswPrivateVar.click);
                }

                cswPrivateVar.$parent.append(cswPublicRet.$);
            } ());

            cswPrivateVar.click = function () {
                cswPrivateVar.toggle();
            };

            cswPrivateVar.toggle = function () {
                if (cswPublicRet.toggleState === Csw.enums.toggleState.on) {
                    cswPublicRet.toggleState = Csw.enums.toggleState.off;
                } else if (cswPublicRet.toggleState === Csw.enums.toggleState.off) {
                    cswPublicRet.toggleState = Csw.enums.toggleState.on;
                }
            };

            cswPublicRet.toggleState = Csw.enums.toggleState.off;

            cswPublicRet.click = function (func) {
                if (Csw.isFunction(func)) {
                    return cswPublicRet.bind('click', func);
                } else {
                    return cswPublicRet;
                }
            };

            cswPublicRet.disable = function () {
                cswPublicRet.addClass('disabled');
            };
            cswPublicRet.enable = function () {
                cswPublicRet.removeClass('disabled');
            };

            return cswPublicRet;
        });

} ());

