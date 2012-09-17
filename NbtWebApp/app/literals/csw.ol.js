/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.literals.ol = Csw.literals.ol ||
        Csw.literals.register('ol', function (options) {
            /// <summary> Create an <ol /> </summary>
            /// <param name="options" type="Object">Options to define the ol.</param>
            /// <returns type="ol">A ol object</returns>
            var cswPrivate = {
                $parent: '',
                number: 1
            };
            var cswPublic = {};

            cswPublic.li = function (liOptions) {
                /// <summary> Create a <li /> </summary>
                /// <param name="options" type="Object">Options to define the li.</param>
                /// <returns type="li">A li object</returns>
                var liInternal = {
                    number: 1
                };
                var liExternal = {};

                (function() {
                    Csw.extend(liInternal, liOptions);

                    var $li,
                        html = '<li>';
                    html += Csw.string(liInternal.text);
                    html += '</li>';

                    $li = $(html);
                    Csw.literals.factory($li, liExternal);
                    cswPublic.append($li);
                }());

                return liExternal;
            };

            (function () {
                var html = '<ol></ol>';
                var $ol;

                Csw.extend(cswPrivate, options);

                $ol = $(html);
                Csw.literals.factory($ol, cswPublic);

                cswPrivate.$parent.append(cswPublic.$);
            } ());


            return cswPublic;
        });

} ());

