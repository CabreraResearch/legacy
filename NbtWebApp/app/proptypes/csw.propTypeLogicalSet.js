/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    var nameCol = 'name';
    var keyCol = 'key';

    Csw.properties.logicalSet = Csw.properties.logicalSet ||
        Csw.properties.register('logicalSet',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    var propVals = cswPublic.data.propData.values;
                    var parent = cswPublic.data.propDiv;

                    var logicalSetJson = propVals.logicalsetjson;

                    cswPublic.control = parent.div()
                           .checkBoxArray({
                               ID: cswPublic.data.ID + '_cba',
                               ReadOnly: cswPublic.data.isReadOnly(),
                               dataAry: logicalSetJson.data,
                               cols: logicalSetJson.columns,
                               nameCol: nameCol,
                               keyCol: keyCol,
                               Multi: cswPublic.data.isMulti(),
                               onChange: function() {
                                   var val = cswPublic.control.val();
                                   Csw.tryExec(cswPublic.data.onChange, val);
                                   if (false === cswPublic.data.isMulti() || false === val.MultiIsUnchanged) {
                                       cswPublic.data.onPropChange({ logicalsetjson: val });
                                   }
                               }
                           });

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());