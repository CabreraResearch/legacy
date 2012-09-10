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

                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    var propVals = cswPublic.data.propData.values;
                    var parent = cswPublic.data.propDiv;
                    var checkOpt = {
                        Checked: (false === cswPublic.data.Multi) ? Csw.string(propVals.checked).trim() : null,
                        Required: Csw.bool(cswPublic.data.Required),
                        ReadOnly: Csw.bool(cswPublic.data.ReadOnly),
                        Multi: cswPublic.data.Multi,
                        onChange: function () {
                            var val = cswPublic.control.val();
                            Csw.tryExec(cswPublic.data.onChange, val);
                            cswPublic.data.onPropChange({ checked: val });
                        }
                    };

                    cswPublic.control = parent.triStateCheckBox(checkOpt);
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());






