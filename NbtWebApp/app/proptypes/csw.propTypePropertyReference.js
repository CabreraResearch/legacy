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
                    cswPrivate.text = (false === o.Multi) ? Csw.string(cswPrivate.propVals.value, o.propData.gestalt).trim() : Csw.enums.multiEditDefaultValue;

                    cswPrivate.text += '&nbsp;&nbsp;';
                    /* Static Div */
                    cswPrivate.parent.div({
                        ID: cswPublic.data.ID,
                        cssclass: 'staticvalue',
                        text: cswPrivate.text
                    });

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());

