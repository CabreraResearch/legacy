/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function _cswUl() {
    'use strict';


    Csw.literals.ul = Csw.literals.ul ||
        Csw.literals.register('ul', function ul(options) {
            /// <summary> Create a <ul /> </summary>
            /// <param name="options" type="Object">Options to define the ul.</param>
            /// <returns type="ul">A ul object</returns>
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
                    text: ''
                };
                var liExternal = {};

                (function () {
                    Csw.extend(liInternal, liOptions);
                    
                    var $li, 
                        html = '<li>';
                    html += Csw.string(liInternal.text);
                    html += '</li>';
                    
                    $li = $(html);
                    Csw.literals.factory($li, liExternal);
                    cswPublic.append($li);
                } ());

                return liExternal;
            };

            (function () {
                var html = '<ul></ul>';
                var $ul;

                Csw.extend(cswPrivate, options);

                $ul = $(html);
                Csw.literals.factory($ul, cswPublic);

                cswPrivate.$parent.append(cswPublic.$);
            } ());


            return cswPublic;
        });

} ());

