/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.logicalSet = Csw.properties.logicalSet ||
        Csw.properties.register('logicalSet', Csw.method(function (propertyOption) {
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
                cswPrivate.logicalSetJson = cswPrivate.propVals.logicalsetjson;

                cswPublic.control = cswPrivate.parent.checkBoxArray({
                    name: cswPublic.data.name + '_cba',
                    cols: cswPrivate.logicalSetJson.columns,
                    data: cswPrivate.logicalSetJson.data,
                    UseRadios: false,
                    isRequired: cswPublic.data.isRequired(),
                    ReadOnly: cswPublic.data.isReadOnly(),
                    Multi: cswPublic.data.isMulti(),
                    onChange: function () {
                        // We're bypassing this to avoid having to deal with the complexity of multiple copies of the checkboxarray JSON
                        //cswPublic.data.onPropChange({ options: val.data });
                        cswPublic.data.propData.wasmodified = true;
                    }
                }); // checkBoxArray
                cswPublic.control.required(cswPublic.data.isRequired());
            }; // render()

            //Bind the callback to the render event
            cswPublic.data.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //cswPublic.data.unBindRender();

            return cswPublic;
        }));
}());