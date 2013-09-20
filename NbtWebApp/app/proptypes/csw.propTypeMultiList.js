/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.register('multiList', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            cswPrivate.gestalt = nodeProperty.propData.gestalt;
            cswPrivate.options = nodeProperty.propData.values.options;

            if (nodeProperty.isReadOnly()) {
                nodeProperty.propDiv.append(cswPrivate.gestalt);
            } else {
                /* Select Box */
                nodeProperty.propDiv.multiSelect({
                    name: nodeProperty.name,
                    cssclass: 'selectinput',
                    values: cswPrivate.options,
                    readonlyless: nodeProperty.propData.values.readonlyless,
                    readonlymore: nodeProperty.propData.values.readonlymore,
                    isMultiEdit: nodeProperty.isMulti(),
                    EditMode: nodeProperty.tabState.EditMode,
                    onChange: function (sel, val) {
                        //Case 29390: no sync for Multi List
                        nodeProperty.propData.values.value = val;
                        nodeProperty.broadcastPropChange(val);
                    }
                });
            }

        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());
