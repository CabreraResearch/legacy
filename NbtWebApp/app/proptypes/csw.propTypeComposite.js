/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.composite = Csw.properties.composite ||
        Csw.properties.register('composite',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = { };
                var cswPublic = {
                    data: propertyOption
                };
                var render = function () {
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.value = (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.value).trim() : Csw.enums.multiEditDefaultValue;
                    
                    cswPublic.control = cswPublic.data.propDiv;
                    cswPublic.control.append(cswPrivate.value);
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
