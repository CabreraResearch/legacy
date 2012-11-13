/// <reference path="~/app/CswApp-vsdoc.js" />


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
        var cswPrivate = {
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
        var cswPublic = {};

        (function () {
            var html = '',
                attr = Csw.makeAttr(),
                style = Csw.makeStyle();

            var $img;

            Csw.extend(cswPrivate, options);
            
            html += '<img ';
            attr.add('id', cswPrivate.ID);
            attr.add('class', cswPrivate.cssclass);
            attr.add('src', cswPrivate.src);
            attr.add('alt', cswPrivate.alt);
            attr.add('title', cswPrivate.title);
            attr.add('height', cswPrivate.height);
            attr.add('ismap', cswPrivate.ismap);
            attr.add('usemap', cswPrivate.usemap);
            attr.add('width', cswPrivate.width);
            
            style.add('border', cswPrivate.border);

            html += attr.get();
            html += style.get();
            
            html += ' />';
            $img = $(html);

            Csw.literals.factory($img, cswPublic);
            if (Csw.isFunction(cswPrivate.onClick)) {
                cswPublic.bind('click', cswPrivate.onClick);
            }

            cswPrivate.$parent.append(cswPublic.$);

        } ());

        cswPublic.click = function (func) {
            if (Csw.isFunction(func)) {
                cswPublic.bind('click', func);
            }
        };

        return cswPublic;
    }
    Csw.literals.register('img', img);
    Csw.literals.img = Csw.literals.img || img;

} ());

