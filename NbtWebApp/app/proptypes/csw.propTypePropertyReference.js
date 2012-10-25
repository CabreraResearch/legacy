/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.propertyReference = Csw.properties.propertyReference ||
        Csw.properties.register('propertyReference',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption || Csw.nbt.propertyOption(propertyOption)
                };

                var render = function () {
                    'use strict';

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.text = Csw.string(cswPublic.data.propData.gestalt).trim();
                    cswPrivate.text += '&nbsp;&nbsp;';
                    /* Static Div */
                    cswPrivate.parent.div({
                        name: cswPublic.data.name,
                        cssclass: 'staticvalue',
                        text: cswPrivate.text
                    });
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

} ());

