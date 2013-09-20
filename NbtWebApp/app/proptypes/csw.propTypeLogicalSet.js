/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('logicalSet', function (nodeProperty) {
        'use strict';


        //The render function to be executed as a callback
        var render = function () {
            'use strict';

            var cswPrivate = Csw.object();
            cswPrivate.logicalSetJson = nodeProperty.propData.values.logicalsetjson;

            var cba = nodeProperty.propDiv.checkBoxArray({
                name: nodeProperty.name + '_cba',
                cols: cswPrivate.logicalSetJson.columns,
                data: cswPrivate.logicalSetJson.data,
                UseRadios: false,
                isRequired: nodeProperty.isRequired(),
                ReadOnly: nodeProperty.isReadOnly(),
                Multi: nodeProperty.isMulti(),
                onChange: function () {
                    //Case 29390: We're already passing by reference; no need to update. No sync for for Logical Set
                    nodeProperty.broadcastPropChange();
                }
            }); // checkBoxArray
            cba.required(nodeProperty.isRequired());
        }; // render()

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });
}());