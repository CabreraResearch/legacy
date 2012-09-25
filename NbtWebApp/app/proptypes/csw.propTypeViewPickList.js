/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    var nameCol = 'label',
        keyCol = 'key',
        valueCol = 'value';

    Csw.properties.viewPickList = Csw.properties.viewPickList ||
        Csw.properties.register('viewPickList',
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
                    cswPrivate.optData = cswPrivate.propVals.options;
                    cswPrivate.selectMode = cswPrivate.propVals.selectmode; // Single, Multiple, Blank

                    cswPublic.control = cswPrivate.parent.div()
                           .checkBoxArray({
                               ID: cswPublic.data.ID + '_cba',
                               UseRadios: (cswPrivate.selectMode === 'Single'),
                               Required: cswPublic.data.isRequired(),
                               ReadOnly: cswPublic.data.isReadOnly(),
                               Multi: cswPublic.data.isMulti(),
                               dataAry: cswPrivate.optData,
                               nameCol: nameCol,
                               keyCol: keyCol,
                               valCol: valueCol,
                               valColName: 'Include',
                               onChange: function () {
                                   var val = cswPublic.control.val();
                                   Csw.tryExec(cswPublic.data.onChange, val);
                                   if (false === cswPublic.data.isMulti() || false === val.MultiIsUnchanged) {
                                       cswPublic.data.onPropChange({ options: val.data });
                                   }
                               }
                           });
                    cswPublic.control.required(cswPublic.data.isRequired());
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
