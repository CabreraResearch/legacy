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
                    if (cswPrivate.propVals.menuoptions.length > 0) {
                        cswPrivate.menuoptions = cswPrivate.propVals.menuoptions.split(',');
                    }
                    cswPrivate.propVals.displayText = cswPrivate.propVals.displayText || Csw.string(cswPrivate.propVals.text, cswPublic.data.propData.name);

                    var buttonOpts = {
                        displayName: cswPrivate.propVals.displayText,
                        icon: cswPrivate.propVals.icon,
                        value: Csw.string(cswPrivate.propVals.text, cswPublic.data.propData.name),
                        size: cswPublic.data.size,
                        mode: Csw.string(cswPrivate.propVals.mode, 'button'),
                        state: cswPrivate.propVals.state,
                        menuOptions: cswPrivate.menuoptions,
                        selectedText: cswPrivate.propVals.selectedText,
                        confirmmessage: cswPrivate.propVals.confirmmessage,
                        propId: cswPublic.data.propid,
                        tabId: cswPublic.data.tabState.tabId,
                        nodeId: cswPublic.data.tabState.nodeId,
                        onClickSuccess: cswPrivate.onClickSuccess
                        //Case 29142: the server decides whether the button is visible. disabled: cswPublic.data.isDisabled() || cswPublic.data.isReadOnly()
                        
                    };

                    if (cswPublic.data.saveTheCurrentTab) {
                        Object.defineProperty(buttonOpts, 'saveTheCurrentTab', { value: cswPublic.data.saveTheCurrentTab });
                    }

                    cswPublic.control = cswPrivate.propDiv.nodeButton(buttonOpts);
                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));
} ());
