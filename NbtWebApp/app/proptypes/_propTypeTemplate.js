/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.template = Csw.properties.template ||
        Csw.properties.register('template',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPrivate = { };
                var cswPublic = {
                    data: propertyOption
                };
                
                var render = function() {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    var propVals = cswPublic.data.propData.values;
                    var parent = cswPublic.data.propDiv;

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));
    
}());
