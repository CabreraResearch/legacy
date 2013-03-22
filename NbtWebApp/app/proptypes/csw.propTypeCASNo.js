/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.CASNo = Csw.properties.CASNo ||
        Csw.properties.register('CASNo',
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
                    cswPrivate.value = Csw.string(cswPrivate.propVals.text).trim();
                    cswPrivate.size = Csw.number(cswPrivate.propVals.size, 14);

                    if (cswPublic.data.isReadOnly()) {
                        cswPublic.control = cswPrivate.parent.append(cswPrivate.value);
                    } else {
                        cswPublic.control = cswPrivate.parent.CASNoTextBox({
                            name: cswPublic.data.name,
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.value,
                            cssclass: 'textinput',
                            size: cswPrivate.size,
                            onChange: function() {
                                var val = cswPublic.control.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ text: val });
                            },
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