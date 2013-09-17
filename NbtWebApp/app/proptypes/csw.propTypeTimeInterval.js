/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.register('timeInterval', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            if (false === Csw.bool(nodeProperty.isReadOnly())) {
                nodeProperty.propDiv.timeInterval({
                    Multi: nodeProperty.isMulti(),
                    ReadOnly: nodeProperty.isReadOnly(),
                    isRequired: nodeProperty.isRequired(),
                    rateIntervalValue: nodeProperty.propData.values.Interval.rateintervalvalue,
                    useEditButton: nodeProperty.tabState.EditMode !== Csw.enums.editMode.Add,
                    onChange: function () {
                        //Case 29390: We're already passing by reference; no need to update. No sync for Time Interval.
                        nodeProperty.broadcastPropChange();
                    }
                });
            } else {
                nodeProperty.propDiv.append(nodeProperty.propData.gestalt);
            }
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });
}());
