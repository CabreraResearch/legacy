/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('propertyReference', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';

            /* Static Div */
            nodeProperty.propDiv.div({
                name: nodeProperty.name,
                cssclass: 'staticvalue',
                text: nodeProperty.propData.gestalt + '&nbsp;&nbsp;'
            });
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());

