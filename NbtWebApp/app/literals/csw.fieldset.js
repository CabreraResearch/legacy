/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.literals.fieldSet = Csw.literals.fieldSet ||
        Csw.literals.register('fieldSet', function (options) {

            var cswPrivate = {
                ID: '',
                name: '',
                cssclass: '',
                form: '',
                disabled: ''
            };
            var cswPublic = {};
            
            cswPublic.legend = function (legendOpts) {
                var legInternal = {
                    value: ''
                };
                var legExternal = {

                };

                (function () {
                    Csw.extend(legInternal, legendOpts);

                    var html = '<legend ',
                        $legend,
                        attr = Csw.makeAttr();

                    html += attr.get();
                    html += '>';
                    html += Csw.string(legInternal.value);
                    html += '</legend>';
                    $legend = $(html);

                    Csw.literals.factory($legend, legExternal);
                    cswPublic.append($legend);
                } ());

                return legExternal;
            };

            (function () {
                var html = '',
                    attr = Csw.makeAttr(),
                    style = Csw.makeStyle();

                Csw.extend(cswPrivate, options);

                cswPrivate.ID = Csw.string(cswPrivate.ID, cswPrivate.name);
                if (false === Csw.isNullOrEmpty(cswPrivate.name)) {
                    cswPrivate.name = cswPrivate.ID;
                }

                attr.add('id', cswPrivate.ID);
                attr.add('class', cswPrivate.cssclass);
                attr.add('form', cswPrivate.form);
                attr.add('disabled', cswPrivate.disabled);
                attr.add('name', cswPrivate.name);

                html += '<fieldset ';
                html += attr.get();
                html += style.get();
                html += '>';
                html += '</fieldset>';

                Csw.literals.factory($(html), cswPublic);

                if (false === Csw.isNullOrEmpty(cswPrivate.$parent)) {
                    cswPrivate.$parent.append(cswPublic.$);
                }
                
            } ());

            return cswPublic;
        });

} ());
