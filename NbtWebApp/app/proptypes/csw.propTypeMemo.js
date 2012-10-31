/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.memo = Csw.properties.memo ||
        Csw.properties.register('memo',
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
                    cswPrivate.rows = Csw.string(cswPrivate.propVals.rows);
                    cswPrivate.columns = Csw.string(cswPrivate.propVals.columns);

                    cswPublic.control = cswPrivate.parent.textArea({
                        onChange: function () {
                            var val = cswPublic.control.val();
                            Csw.tryExec(cswPublic.data.onChange, val);
                            cswPublic.data.onPropChange({ text: val });
                        },
                        name: cswPublic.data.name,
                        rows: cswPrivate.rows,
                        cols: cswPrivate.columns,
                        value: cswPrivate.value,
                        disabled: cswPublic.data.isReadOnly(),
                        isRequired: cswPublic.data.isRequired(),
                        readonly: cswPublic.data.isReadOnly()
                    });

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
