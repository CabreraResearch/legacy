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
                
                var render = function(o) {
                    'use strict';
                    o = o || Csw.nbt.propertyOption(propertyOption);

                    var propVals = o.propData.values;
                    var parent = o.propDiv;

                };

                propertyOption.render(render);
                return cswPublic;
            }));
    
}());
