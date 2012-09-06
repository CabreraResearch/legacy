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
                var render = function (o) {
                    o = o || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propVals = o.propData.values;
                    cswPrivate.value = (false === o.Multi) ? Csw.string(cswPrivate.propVals.value).trim() : Csw.enums.multiEditDefaultValue;
                    
                    cswPublic.control = o.propDiv;
                    cswPublic.control.append(cswPrivate.value);
                };

                propertyOption.render(render);
                return cswPublic;
            }));

}());
