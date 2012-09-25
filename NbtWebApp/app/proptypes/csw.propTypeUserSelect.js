/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    var nameCol = 'label';
    var keyCol = 'key';
    var valueCol = 'value';

    Csw.properties.userSelect = Csw.properties.userSelect ||
        Csw.properties.register('userSelect',
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
                    cswPrivate.options = cswPrivate.propVals.options;

                    cswPublic.control = cswPrivate.parent.div()
                           .checkBoxArray({
                               ID: cswPublic.data.ID + '_cba',
                               UseRadios: false,
                               Required: cswPublic.data.isRequired(),
                               Multi: cswPublic.data.isMulti(),
                               ReadOnly: cswPublic.data.isReadOnly(),
                               dataAry: cswPrivate.options,
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

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());

