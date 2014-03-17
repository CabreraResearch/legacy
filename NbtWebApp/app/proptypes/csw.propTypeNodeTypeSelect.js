/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('nodeTypeSelect', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            cswPrivate.selectMode = nodeProperty.propData.values.selectmode; // Single, Multiple, Blank

            var cols = [];
            var data = nodeProperty.propData.values;
            if (false === nodeProperty.isReadOnly() && nodeProperty.propData.values.options) {
                cols = nodeProperty.propData.values.options.columns;
                data = nodeProperty.propData.values.options.data;
            }

            var cba = nodeProperty.propDiv.checkBoxArray({
                name: nodeProperty.name + '_cba',
                cols: cols,
                data: data,
                UseRadios: (cswPrivate.selectMode === 'Single'),
                isRequired: nodeProperty.isRequired(),
                ReadOnly: nodeProperty.isReadOnly(),
                Multi: nodeProperty.isMulti(),
                onChange: function () {
                    //Case 29390: We're already passing by reference; no need to update. No sync for Node Type Select.        
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