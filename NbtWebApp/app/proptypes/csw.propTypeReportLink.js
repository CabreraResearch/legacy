/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('reportlink', function (nodeProperty, tabsAndProps) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';

            var reportid = nodeProperty.propData.values.reportid;
            
            var cswPrivate = {
                size: 'small'
            };
            var buttonOpts = {
                displayName: nodeProperty.propData.name,
                value: nodeProperty.propData.name,
                size: nodeProperty.size,
                mode: 'link',
                propId: nodeProperty.propid,
                tabId: nodeProperty.tabState.tabid,
                identityTabId: nodeProperty.identityTabId,
                nodeId: nodeProperty.tabState.nodeId,
                onClickAction: function() {
                    Csw.main.handleReport(reportid);
                },
                tabsAndProps: tabsAndProps,
                onRefresh: nodeProperty.onRefresh,
                disabled: nodeProperty.tabState.Config //disable in Config mode
            };

            nodeProperty.propDiv.nodeButton(buttonOpts);
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });
}());
