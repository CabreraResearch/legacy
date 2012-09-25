/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.list = Csw.properties.list ||
        Csw.properties.register('list',
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

                    cswPrivate.value = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.value).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.options = Csw.string(cswPrivate.propVals.options).trim();

                    if (cswPublic.data.isReadOnly()) {
                        cswPublic.control = cswPrivate.parent.append(cswPrivate.value);
                    } else {
                        cswPrivate.values = cswPrivate.options.split(',');
                        if (cswPublic.data.isMulti()) {
                            cswPrivate.values.push(Csw.enums.multiEditDefaultValue);
                        }
                        cswPublic.control = cswPrivate.parent.select({
                            ID: cswPublic.data.ID,
                            cssclass: 'selectinput',
                            onChange: function () {
                                var val = cswPublic.control.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ value: val });
                            },
                            values: cswPrivate.values,
                            selected: cswPrivate.value
                        });
                        cswPublic.control.required(cswPublic.data.isRequired());
                    }

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
