/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswLi() {
    'use strict';

    function li(options) {
        /// <summary> Create a <li /> </summary>
        /// <param name="options" type="Object">Options to define the li.</param>
        /// <returns type="li">A li object</returns>
        var internal = {
            $parent: '',
            number: 1
        };
        var external = {};

        (function () {
            var html = '<li></li>';
            var $li;

            $.extend(internal, options);

            $li = $(html);
            Csw.controls.factory($li, external);

            internal.$parent.append(external.$);
        } ());

        return external;
    }
    Csw.controls.register('li', li);
    Csw.controls.li = Csw.controls.li || li;

} ());

