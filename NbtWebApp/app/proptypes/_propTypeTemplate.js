/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.template = Csw.properties.template ||
        Csw.properties.register('template',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPublic = {
                    data: propertyOption
                };
                var render = function(o) {

                    var propVals = o.propData.values;
                    var parent = o.propDiv;

                };

                propertyOption.render(render);
                return cswPublic;
            }));
    
}());
