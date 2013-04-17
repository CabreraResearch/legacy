/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.composite = Csw.properties.register('composite',
        function(nodeProperty) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                var cswPrivate = Csw.object();

                cswPrivate.value = nodeProperty.propData.values.value;
                nodeProperty.propDiv.append(cswPrivate.value);
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

}());
