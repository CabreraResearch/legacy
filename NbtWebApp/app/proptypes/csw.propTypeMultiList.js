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
                //The render function to be executed as a callback
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

                    if (cswPublic.data.isReadOnly()) {
                        cswPublic.control = cswPrivate.parent.append(cswPrivate.gestalt);
                    } else {
                        /* Select Box */
                        cswPublic.control = cswPrivate.parent.multiSelect({
                            name: cswPublic.data.name,
                            cssclass: 'selectinput',
                            values: cswPrivate.options,
                            readonlyless: cswPrivate.propVals.readonlyless,
                            readonlymore: cswPrivate.propVals.readonlymore,
                            isMultiEdit: cswPublic.data.isMulti(),
                            EditMode: cswPublic.data.tabState.EditMode,
                            onChange: function () {
                               var val = cswPublic.control.val();
                               Csw.tryExec(cswPublic.data.onChange, val);
                               cswPrivate.sortVals();
                               cswPublic.data.onPropChange({ value: val });
                            }
                        });
                    }

                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

}());
