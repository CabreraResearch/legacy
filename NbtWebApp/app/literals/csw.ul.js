/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswUl() {
    'use strict';


    Csw.literals.ul = Csw.literals.ul ||
        Csw.literals.register('ul', function ul(options) {
            /// <summary> Create a <ul /> </summary>
            /// <param name="options" type="Object">Options to define the ul.</param>
            /// <returns type="ul">A ul object</returns>
            var cswPrivate = {
                $parent: '',
                number: 1,
                ID: '',
                cssclass: ''
            };
            var cswPublic = {};

            cswPublic.li = function (liOptions) {
                /// <summary> Create a <li /> </summary>
                /// <param name="options" type="Object">Options to define the li.</param>
                /// <returns type="li">A li object</returns>
                var liInternal = {
                    text: ''  ,
                    ID: '',
                    cssclass: ''
                };
                var liExternal = {};

                (function () {
                    Csw.extend(liInternal, liOptions);
                    
                    var $li, 
                        html = '',
                        attr = Csw.makeAttr();

                    attr.add('id', cswPrivate.ID);
                    attr.add('class', cswPrivate.cssclass);

                    html += '<li';
                    html += attr.get();
                    html += '>';
                    html += Csw.string(liInternal.text);
                    html += '</li>';
                    
                    $li = $(html);
                    Csw.literals.factory($li, liExternal);
                    cswPublic.append($li);
                } ());

                return liExternal;
            };

            (function () {
                var html = '',
                    attr = Csw.makeAttr();

                attr.add('id', cswPrivate.ID);
                attr.add('class', cswPrivate.cssclass);

                html += '<ul';
                html += attr.get();
                html += '></ul>';
                var $ul;

                Csw.extend(cswPrivate, options);

                $ul = $(html);
                Csw.literals.factory($ul, cswPublic);

                cswPrivate.$parent.append(cswPublic.$);
            } ());


            return cswPublic;
        });

} ());

