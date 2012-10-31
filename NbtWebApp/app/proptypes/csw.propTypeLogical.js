/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.logical = Csw.properties.logical ||
        Csw.properties.register('logical',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                //The render function to be executed as a callback
                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    var propVals = cswPublic.data.propData.values;
                    var parent = cswPublic.data.propDiv;
                    var checkOpt = {
                        Checked: Csw.string(propVals.checked).trim(),
                        isRequired: Csw.bool(cswPublic.data.isRequired()),
                        ReadOnly: Csw.bool(cswPublic.data.isReadOnly()),
                        Multi: cswPublic.data.isMulti(),
                        onChange: function () {
                            var val = cswPublic.control.val();
                            Csw.tryExec(cswPublic.data.onChange, val);
                            cswPublic.data.onPropChange({ checked: val });
                        }
                    };

                    cswPublic.control = parent.triStateCheckBox(checkOpt);
                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

}());






