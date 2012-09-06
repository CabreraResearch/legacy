/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.composite = Csw.properties.composite ||
        Csw.properties.register('composite',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPublic = {
                    data: propertyOption
                };
                var render = function (o) {

                    var propVals = o.propData.values;
                    cswPublic.control = o.propDiv;
                    var value = (false === o.Multi) ? Csw.string(propVals.value).trim() : Csw.enums.multiEditDefaultValue;
                    cswPublic.control.append(value);

                };

                propertyOption.render(render);
                return cswPublic;
            }));

}());
