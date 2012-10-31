/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.timeInterval = Csw.properties.timeInterval ||
        Csw.properties.register('timeInterval', Csw.method(function (propertyOption) {
            'use strict';
            var cswPrivate = {};
            var cswPublic = {
                data: propertyOption
            };

            //The render function to be executed as a callback
            var render = function () {
                'use strict';
                cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                cswPrivate.propVals = cswPublic.data.propData.values;
                cswPrivate.parent = cswPublic.data.propDiv;

                if (false === Csw.bool(cswPublic.data.isReadOnly())) {
                    cswPublic.control = cswPrivate.parent.timeInterval({
                        Multi: cswPublic.data.isMulti(),
                        ReadOnly: cswPublic.data.isReadOnly(),
                        Required: cswPublic.data.isRequired(),
                        rateIntervalValue: cswPrivate.propVals.Interval.rateintervalvalue,
                        useEditButton: cswPublic.data.tabState.EditMode !== Csw.enums.editMode.Add,
                        onChange: function () {
                            // We're bypassing this to avoid having to deal with the complexity of multiple copies of the RateInterval JSON
                            // cswPublic.data.onPropChange(compare);
                            cswPublic.data.propData.wasmodified = true;
                        }
                    });
                } else {
                    cswPublic.control = cswPrivate.parent.append(cswPublic.data.propData.gestalt);
                }
            };

            //Bind the callback to the render event
            cswPublic.data.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //cswPublic.data.unBindRender();

            return cswPublic;
        }));
} ());
