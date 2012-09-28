/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.sequence = Csw.properties.sequence ||
        Csw.properties.register('sequence',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.value = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.sequence).trim() : Csw.enums.multiEditDefaultValue;

                    if (cswPublic.data.isReadOnly() || cswPublic.data.isMulti()) {
                        cswPublic.control = cswPrivate.parent.append(cswPrivate.value);
                    } else {
                        cswPublic.control = cswPrivate.parent.input({
                            ID: cswPublic.data.ID,
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: function() {
                                var val = cswPublic.control.val();
                                cswPublic.data.onChange();
                                cswPublic.data.onPropChange({ sequence: val });
                            },
                            value: cswPrivate.value,
                            required: cswPublic.data.isRequired()
                        });

                        cswPublic.control.required(cswPublic.data.isRequired());
                        cswPublic.control.clickOnEnter(cswPublic.data.saveBtn);
                    }

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());

