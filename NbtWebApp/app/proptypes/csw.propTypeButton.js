/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.button = Csw.properties.register('button',
        function(nodeProperty, tabsAndProps) {
            'use strict';
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';

                var cswPrivate = {
                    size: 'small'
                };

                if (nodeProperty.propData.values.menuoptions.length > 0) {
                    cswPrivate.menuoptions = nodeProperty.propData.values.menuoptions.split(',');
                }
                
                var buttonOpts = {
                    displayName: nodeProperty.propData.values.displayText,
                    icon: nodeProperty.propData.values.icon,
                    value: Csw.string(nodeProperty.propData.values.text, nodeProperty.propData.name),
                    size: nodeProperty.size,
                    mode: Csw.string(nodeProperty.propData.values.mode, 'button'),
                    state: nodeProperty.propData.values.state,
                    menuOptions: cswPrivate.menuoptions,
                    selectedText: nodeProperty.propData.values.selectedText,
                    confirmmessage: nodeProperty.propData.values.confirmmessage,
                    propId: nodeProperty.propid,
                    tabId: nodeProperty.tabState.tabid,
                    nodeId: nodeProperty.tabState.nodeId,
                    onClickSuccess: cswPrivate.onClickSuccess,
                    tabsAndProps: tabsAndProps
                    //Case 29142: the server decides whether the button is visible. disabled: nodeProperty.isDisabled() || nodeProperty.isReadOnly()
                };
                
                nodeProperty.propDiv.nodeButton(buttonOpts);
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });
} ());
