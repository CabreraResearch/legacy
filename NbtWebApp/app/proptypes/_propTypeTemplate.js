/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.template = Csw.properties.template ||
        Csw.properties.register('template',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPrivate = { };
                var cswPublic = {
                    data: propertyOption || Csw.nbt.propertyOption(propertyOption)
                };
                
                var render = function() {
                    'use strict';
                    
                    var propVals = cswPublic.data.propData.values;
                    var parent = cswPublic.data.propDiv;

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));
    
}());
