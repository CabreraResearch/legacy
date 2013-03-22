/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.sequence = Csw.properties.sequence ||
        Csw.properties.register('sequence',
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
                    cswPrivate.value = Csw.string(cswPrivate.propVals.sequence).trim();

                    if (cswPublic.data.isReadOnly() || cswPublic.data.isMulti()) {
                        cswPublic.control = cswPrivate.parent.append(cswPrivate.value);
                    } else {
                        cswPublic.control = cswPrivate.parent.input({
                            name: cswPublic.data.name,
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: function() {
                                var val = cswPublic.control.val();
                                cswPublic.data.onChange();
                                cswPublic.data.onPropChange({ sequence: val });
                            },
                            value: cswPrivate.value,
                            isRequired: cswPublic.data.isRequired()
                        });

                        cswPublic.control.required(cswPublic.data.isRequired());
                        cswPublic.control.clickOnEnter(function () {
                            cswPrivate.publish('CswSaveTabsAndProp_tab' + cswPublic.data.tabState.tabid + '_' + cswPublic.data.tabState.nodeid);
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

