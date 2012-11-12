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
                
                //The render function to be executed as a callback
                var render = function () {
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.value = Csw.string(cswPrivate.propVals.value).trim();
                    
                    cswPublic.control = cswPublic.data.propDiv;
                    cswPublic.control.append(cswPrivate.value);
                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

}());
