/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.text = Csw.properties.text ||
        Csw.properties.register('text',
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
                    cswPrivate.value = (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.text).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.length = Csw.number(cswPrivate.propVals.length, 14);

                    if (cswPublic.data.ReadOnly) {
                        cswPublic.control = cswPrivate.parent.append(cswPrivate.value);
                    } else {
                        cswPublic.control = cswPrivate.parent.input({
                            ID: cswPublic.data.ID,
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.value,
                            cssclass: 'textinput',
                            width: cswPrivate.length * 7,
                            onChange: function() {
                                var val = cswPublic.control.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ text: val });
                            },
                            required: cswPublic.data.Required
                        });
                        
                        cswPublic.control.required(cswPublic.data.Required);
                        cswPublic.control.clickOnEnter(cswPublic.data.saveBtn);
                    }

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());