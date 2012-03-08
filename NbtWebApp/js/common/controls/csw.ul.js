/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function _cswUl() {
    'use strict';

    function ul(options) {
        /// <summary> Create a <ul /> </summary>
        /// <param name="options" type="Object">Options to define the ul.</param>
        /// <returns type="ul">A ul object</returns>
        var internal = {
            $parent: '',
            number: 1
        };
        var external = {};

        (function () {
            var html = '<ul></ul>';
            var $ul;

            $.extend(internal, options);

            $ul = $(html);
            Csw.controls.factory($ul, external);

            internal.$parent.append(external.$);
        } ());

        return external;
    }
    Csw.controls.register('ul', ul);
    Csw.controls.ul = Csw.controls.ul || ul;

} ());

