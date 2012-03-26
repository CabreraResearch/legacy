/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function _cswBr() {

    Csw.controls.br = Csw.controls.br ||
        Csw.controls.register('br', function(options) {
            /// <summary> Create a <br /> </summary>
            /// <param name="options" type="Object">Options to define the br.</param>
            /// <returns type="br">A br object</returns>
            'use strict';
            var internal = {
                $parent: '',
                number: 1
            };
            var external = { };

            (function() {
                var html = '<br />';
                var $br, i;

                $.extend(internal, options);
                for (i = 1; i < Csw.number(internal.number); i += 1) {
                    html += '<br />';
                }
                $br = $(html);
                Csw.controls.factory($br, external);

                internal.$parent.append(external.$);
            }());

            return external;
        });

} ());

