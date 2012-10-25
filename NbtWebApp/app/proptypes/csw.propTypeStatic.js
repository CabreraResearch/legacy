/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties['static'] = Csw.properties['static'] ||
        Csw.properties.register('static',
            Csw.method(function (propertyOption) {
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                var render = function () {
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.text = Csw.string(cswPrivate.propVals.text).trim();
                    cswPrivate.columns = Csw.number(cswPrivate.propVals.columns);
                    cswPrivate.rows = Csw.number(cswPrivate.propVals.rows);
                    cswPrivate.overflow = 'auto';
                    cswPrivate.width = '';
                    cswPrivate.height = '';

                    if (cswPrivate.columns > 0 && cswPrivate.rows > 0) {
                        cswPrivate.overflow = 'scroll';
                        cswPrivate.width = Math.round(cswPrivate.columns + 2 - (cswPrivate.columns / 2.25)) + 'em';
                        cswPrivate.height = Math.round(cswPrivate.rows + 2.5 + (cswPrivate.rows / 5)) + 'em';
                    }
                    else if (cswPrivate.columns > 0) {
                        cswPrivate.width = Math.round(cswPrivate.columns - (cswPrivate.columns / 2.25)) + 'em';
                    }
                    else if (cswPrivate.rows > 0) {
                        cswPrivate.height = Math.round(cswPrivate.rows + 0.5 + (cswPrivate.rows / 5)) + 'em';
                    }

                    cswPublic.control = cswPrivate.parent.div({
                        cssclass: 'staticvalue',
                        text: Csw.string(cswPrivate.text, '&nbsp;&nbsp;')
                    }).css({
                        overflow: cswPrivate.overflow,
                        width: cswPrivate.width,
                        height: cswPrivate.height
                    });

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());

