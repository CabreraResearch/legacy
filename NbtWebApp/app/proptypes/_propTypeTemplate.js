/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    if (false) { //Remove to begin implementing new fieldtype
        
        Csw.properties.template = Csw.properties.register('template',
            function(nodeProperty) {
                'use strict';


                var render = function() {
                    'use strict';

                    var cswPrivate = Csw.object();

                };

                nodeProperty.bindRender(render);
                return true;
            });
        
    }
}());
