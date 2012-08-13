/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function _cswBr() {

    Csw.literals.br = Csw.literals.br ||
        Csw.literals.register('br', function(options) {
            /// <summary> Create a <br /> </summary>
            /// <param name="options" type="Object">Options to define the br.</param>
            /// <returns type="br">A br object</returns>
            'use strict';
            var cswPrivate = {
                $parent: '',
                number: 1
            };
            var cswPublic = { };

            (function() {
                var html = '<br />';
                var $br, i;

                Csw.extend(cswPrivate, options);
                for (i = 1; i < Csw.number(cswPrivate.number); i += 1) {
                    html += '<br />';
                }
                $br = $(html);
                Csw.literals.factory($br, cswPublic);

                cswPrivate.$parent.append(cswPublic.$);
            }());

            return cswPublic;
        });

} ());

