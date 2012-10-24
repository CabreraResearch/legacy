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
                    cswPrivate.value = Csw.string(cswPrivate.propVals.text).trim();
                    cswPrivate.size = Csw.number(cswPrivate.propVals.size, 14);
                    cswPrivate.maxlength = Csw.number(cswPrivate.propVals.maxlength, 14);

                    if (cswPublic.data.isReadOnly()) {
                        cswPublic.control = cswPrivate.parent.append(cswPrivate.value);
                    } else {
                        cswPublic.control = cswPrivate.parent.input({
                            name: cswPublic.data.name,
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.value,
                            cssclass: 'textinput',
                            size: cswPrivate.size,
                            onChange: function() {
                                var val = cswPublic.control.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ text: val });
                            },
                            required: cswPublic.data.isRequired(),
                            maxlength: cswPrivate.maxlength
                        });
                        
                        cswPublic.control.required(cswPublic.data.isRequired());
                        cswPublic.control.clickOnEnter(cswPublic.data.saveBtn);
                    }

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());