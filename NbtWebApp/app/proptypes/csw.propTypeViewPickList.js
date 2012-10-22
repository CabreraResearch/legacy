/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.viewPickList = Csw.properties.viewPickList ||
        Csw.properties.register('viewPickList', Csw.method(function (propertyOption) {
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
                    name: cswPublic.data.name + '_cba',
                    cols: cswPrivate.options.columns,
                    data: cswPrivate.options.data,
                    UseRadios: (cswPrivate.selectMode === 'Single'),
                    Required: cswPublic.data.isRequired(),
                    ReadOnly: cswPublic.data.isReadOnly(),
                    Multi: cswPublic.data.isMulti(),
                    onChange: function () {
                        // We're bypassing this to avoid having to deal with the complexity of multiple copies of the checkboxarray JSON
                        //var val = cswPublic.control.val();
                        //Csw.tryExec(cswPublic.data.onChange, val);
                        //if (false === cswPublic.data.isMulti() || false === cswPublic.control.MultiIsUnchanged() ) {
                            //cswPublic.data.onPropChange({ options: val.data });
                        //}
                        cswPublic.data.propData.wasmodified = true;
                    }
                }); // checkBoxArray
                cswPublic.control.required(cswPublic.data.isRequired());
            }; // render()

            cswPublic.data.bindRender(render);
            return cswPublic;
        }));

}());
