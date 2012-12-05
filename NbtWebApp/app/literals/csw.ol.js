/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.literals.ol = Csw.literals.ol ||
        Csw.literals.register('ol', function (cswPrivate) {
            /// <summary> Create an <ol /> </summary>
            /// <param name="options" type="Object">Options to define the ol.</param>
            /// <returns type="ol">A ol object</returns>
            
            var cswPublic = {};

            (function _preCtor() {
                cswPrivate = cswPrivate || {};
                cswPrivate.$parent = cswPrivate.$parent || {};
                cswPrivate.ID = cswPrivate.ID || '';
                cswPrivate.name = cswPrivate.name || '';
                cswPrivate.cssclass = cswPrivate.cssclass || '';
            }());
            
            cswPrivate.count = 1;

            cswPublic.li = function (liInternal) {
                /// <summary> Create a <li /> </summary>
                /// <param name="options" type="Object">Options to define the li.</param>
                /// <returns type="li">A li object</returns>
                liInternal = liInternal || {};
                liInternal.text = liInternal.text || '';
                liInternal.cssclass = liInternal.cssclass || '';

                var liExternal = {};

                (function () {
                    
                    var $li,
                        html = '',
                        attr = Csw.makeAttr();

                    cswPrivate.count += 1;
                    attr.add('id', cswPrivate.ID + '_' + cswPrivate.count);
                    attr.add('class', cswPrivate.cssclass);
                    attr.add('name', cswPrivate.name + '_' + cswPrivate.count);

                    html += '<li';
                    html += attr.get();
                    html += '>';
                    html += Csw.string(liInternal.text);
                    html += '</li>';

                    $li = $(html);
                    Csw.literals.factory($li, liExternal);
                    cswPublic.append($li);
                }());

                return liExternal;
            };

            (function _postCtor() {
                var html = '',
                    attr = Csw.makeAttr();

                attr.add('id', cswPrivate.ID);
                attr.add('class', cswPrivate.cssclass);

                html += '<ol';
                html += attr.get();
                html += '></ol>';
                var $ol = $(html);

                Csw.literals.factory($ol, cswPublic);
                cswPrivate.$parent.append(cswPublic.$);
            } ());


            return cswPublic;
        });

} ());

