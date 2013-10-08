/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswUl() {
    'use strict';

    Csw.literals.register('ul', function ul(cswPrivate) {
        /// <summary> Create a <ul /> </summary>
        /// <param name="options" type="Object">Options to define the ul.</param>
        /// <returns type="ul">A ul object</returns>
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
                attr.add('id', cswPrivate.ID + cswPrivate.count); //Don't use '_' in DOM ID; 3rd party libs will sometimes try to split the ID
                attr.add('class', cswPrivate.cssclass);
                attr.add('name', cswPrivate.name + cswPrivate.count);

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

            html += '<ul';
            html += attr.get();
            html += '></ul>';
            var $ul;

            $ul = $(html);
            Csw.literals.factory($ul, cswPublic);

            cswPrivate.$parent.append(cswPublic.$);
        }());


        return cswPublic;
    });

}());

