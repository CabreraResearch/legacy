/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.button = Csw.properties.button ||
        Csw.properties.register('button',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {
                    size: 'small'
                };
                var cswPublic = {
                    data: propertyOption
                };

                //The render function to be executed as a callback
                var render = function() {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propDiv = cswPublic.data.propDiv;

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.value = Csw.string(cswPrivate.propVals.text, cswPublic.data.propData.name);
                    cswPrivate.mode = Csw.string(cswPrivate.propVals.mode, 'button');
                    cswPrivate.menuoptions = '';
                    if (cswPrivate.propVals.menuoptions.length > 0) {
                        cswPrivate.menuoptions = cswPrivate.propVals.menuoptions.split(',');
                    }
                    cswPrivate.state = cswPrivate.propVals.state;
                    cswPrivate.text = cswPrivate.propVals.text;
                    cswPrivate.selectedText = cswPrivate.propVals.selectedText;

                    cswPublic.control = cswPrivate.propDiv.nodeButton({
                        name: cswPrivate.text,
                        value: cswPrivate.value,
                        size: cswPublic.data.size,
                        mode: cswPrivate.mode,
                        state: cswPrivate.state,
                        menuOptions: cswPrivate.menuoptions,
                        selectedText: cswPrivate.selectedText,
                        confirmmessage: cswPrivate.propVals.confirmmessage,
                        propId: cswPublic.data.propid,
                        onClickSuccess: cswPrivate.onClickSuccess,
                        disabled: cswPublic.data.isDisabled() || cswPublic.data.isReadOnly()
                    });
                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));
} ());
