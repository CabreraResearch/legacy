/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function _cswImg() {
    'use strict';

    function img(options) {
        /// <summary> Create or extend an HTML <a /> and return a Csw.img object
        ///     &#10;1 - img(options)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the input.</para>
        /// <para>options.cssclass: CSS class to asign</para>
        /// <para>options.text: Text to display</para>
        /// </param>
        /// <returns type="img">A img object</returns>
        var cswPrivateVar = {
            $parent: '',
            ID: '',
            cssclass: '',
            src: '',
            alt: '',
            title: '',
            height: '',
            ismap: '',
            usemap: '',
            border: 0,
            width: '',
            onClick: null //function () {}
        };
        var cswPublicRet = {};

        (function () {
            var html = '',
                attr = Csw.makeAttr(),
                style = Csw.makeStyle();

            var $img;

            if (options) {
                $.extend(cswPrivateVar, options);
            }

            cswPrivateVar.ID = Csw.string(cswPrivateVar.ID, cswPrivateVar.name);

            html += '<img ';
            attr.add('id', cswPrivateVar.ID);
            attr.add('class', cswPrivateVar.cssclass);
            attr.add('src', cswPrivateVar.src);
            attr.add('alt', cswPrivateVar.alt);
            attr.add('title', cswPrivateVar.title);
            attr.add('height', cswPrivateVar.height);
            attr.add('ismap', cswPrivateVar.ismap);
            attr.add('usemap', cswPrivateVar.usemap);
            attr.add('width', cswPrivateVar.width);
            
            style.add('border', cswPrivateVar.border);

            html += attr.get();
            html += style.get();
            
            html += ' />';
            $img = $(html);

            Csw.literals.factory($img, cswPublicRet);
            if (Csw.isFunction(cswPrivateVar.onClick)) {
                cswPublicRet.bind('click', cswPrivateVar.onClick);
            }

            cswPrivateVar.$parent.append(cswPublicRet.$);

        } ());

        cswPublicRet.click = function (func) {
            if (Csw.isFunction(func)) {
                cswPublicRet.bind('click', func);
            }
        };

        return cswPublicRet;
    }
    Csw.literals.register('img', img);
    Csw.literals.img = Csw.literals.img || img;

} ());

