/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.multiList = Csw.properties.multiList ||
        Csw.properties.register('multiList',
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
                    cswPrivate.gestalt = Csw.string(cswPublic.data.propData.gestalt).trim();
                    cswPrivate.options = cswPrivate.propVals.options;

                    cswPrivate.sortVals = function() {
                        //multiSelect sorts the val for us, sort values.value to make comparision work
                        var uniqueVals = Csw.delimitedString(cswPrivate.propVals.value.split(','));
                        cswPrivate.propVals.value = uniqueVals.toString();
                    };

                    if (cswPublic.data.ReadOnly) {
                        cswPublic.control = propDiv.append(cswPrivate.gestalt);
                    } else {
                        /* Select Box */
                        cswPublic.control = propDiv.multiSelect({
                            ID: cswPublic.data.ID,
                            cssclass: 'selectinput',
                            values: cswPrivate.options,
                            readonlyless: cswPrivate.propVals.readonlyless,
                            readonlymore: cswPrivate.propVals.readonlymore,
                            isMultiEdit: cswPublic.data.Multi,
                            EditMode: cswPublic.data.EditMode,
                            onChange: function () {
                               var val = cswPublic.control.val();
                               Csw.tryExec(cswPublic.data.onChange, val);
                               cswPrivate.sortVals();
                               cswPublic.data.onPropChange({ value: val });
                            }
                        });
                    }

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
