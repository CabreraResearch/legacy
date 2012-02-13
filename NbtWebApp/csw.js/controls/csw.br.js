/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswBr() {
    'use strict';

    function br(options) {
        /// <summary> Create a <br /> </summary>
        /// <param name="options" type="Object">Options to define the br.</param>
        /// <returns type="br">A br object</returns>
        var internal = {
            $parent: '',
            number: 1
        };
        var external = {};

        (function () {
            var html = '<br />';
            var $br, i;

            $.extend(internal, options);
            for (i = 1; i < Csw.number(internal.number); i += 1) {
                html += '<br />';
            }
            $br = $(html);
            Csw.controls.domExtend($br, external);

            internal.$parent.append(external.$);
        } ());

        return external;
    }
    Csw.controls.register('br', br);
    Csw.controls.br = Csw.controls.br || br;

} ());

