/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.nodeTypeSelect = Csw.properties.nodeTypeSelect ||
        Csw.properties.register('nodeTypeSelect', Csw.method(function (propertyOption) {
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
                cswPrivate.options = cswPrivate.propVals.options;
                cswPrivate.selectMode = cswPrivate.propVals.selectmode; // Single, Multiple, Blank

                cswPublic.control = cswPrivate.parent.checkBoxArray({
                    ID: cswPublic.data.ID + '_cba',
                    cols: cswPrivate.options.columns,
                    data: cswPrivate.options.data,
                    UseRadios: (cswPrivate.selectMode === 'Single'),
                    Required: cswPublic.data.isRequired(),
                    ReadOnly: cswPublic.data.isReadOnly(),
                    Multi: cswPublic.data.isMulti(),
                    onChange: function () {
                        var val = cswPublic.control.val();
                        Csw.tryExec(cswPublic.data.onChange, val);
                        if (false === cswPublic.data.isMulti() || false === cswPublic.control.MultiIsUnchanged() ) {
                            cswPublic.data.onPropChange({ options: val.data });
                        }
                    }
                }); // checkBoxArray
                cswPublic.control.required(cswPublic.data.isRequired());
            }; // render()

            cswPublic.data.bindRender(render);
            return cswPublic;
        }));
}());