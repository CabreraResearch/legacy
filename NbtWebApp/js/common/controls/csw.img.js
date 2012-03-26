/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

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
        var internal = {
            $parent: '',
            ID: '',
            cssclass: '',
            src: '',
            alt: '',
            height: '',
            ismap: '',
            usemap: '',
            border: 0,
            width: '',
            onClick: null //function () {}
        };
        var external = {};

        (function () {
            var html = '',
                attr = Csw.makeAttr(),
                style = Csw.makeStyle();

            var $img;

            if (options) {
                $.extend(internal, options);
            }

            internal.ID = Csw.string(internal.ID, internal.name);

            html += '<img ';
            attr.add('id', internal.ID);
            attr.add('class', internal.cssclass);
            attr.add('src', internal.src);
            attr.add('alt', internal.alt);
            attr.add('height', internal.height);
            attr.add('ismap', internal.ismap);
            attr.add('usemap', internal.usemap);
            attr.add('width', internal.width);
            
            style.add('border', internal.border);

            html += attr.get();
            html += style.get();
            
            html += ' />';
            $img = $(html);

            Csw.controls.factory($img, external);
            if (Csw.isFunction(internal.onClick)) {
                external.bind('click', internal.onClick);
            }

            internal.$parent.append(external.$);

        } ());

        external.click = function (func) {
            if (Csw.isFunction(func)) {
                external.bind('click', func);
            }
        };

        return external;
    }
    Csw.controls.register('img', img);
    Csw.controls.img = Csw.controls.img || img;

} ());

