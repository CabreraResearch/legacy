/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('location', function (nodeProperty) {
        'use strict';
        var cswPrivate = {};

        //The render function to be executed as a callback
        var render = function () {
            'use strict';

            var firstSelectHappened = false;

            cswPrivate.location = nodeProperty.propDiv.location({
                //name: nodeProperty.name, //data has no "name"?
                locationobjectclassid: nodeProperty.propData.values.locationobjectclassid,
                locationnodetypeids: nodeProperty.propData.values.locationnodetypeids,
                relatedmatch: (nodeProperty.tabState.relatedobjectclassid === cswPrivate.locationobjectclassid),
                relatednodeid: nodeProperty.tabState.relatednodeid,
                relatednodename: nodeProperty.tabState.relatednodename,
                relatedobjectclassid: nodeProperty.tabState.relatedobjectclassid,
                nodeid: nodeProperty.propData.values.nodeid,
                viewid: nodeProperty.propData.values.viewid,
                //selectedName: nodeProperty.propData.values.namedItem, //"namedItem" doesn't exist anywhere?
                selectedName: nodeProperty.propData.values.name,
                path: nodeProperty.propData.values.path,
                nodeKey: '', //(false === o.Multi) ? Csw.string(propVals.nodekey).trim() : '';
                selectednodelink: nodeProperty.propData.values.selectednodelink,
                Multi: nodeProperty.isMulti(),
                ReadOnly: nodeProperty.isReadOnly(),
                isRequired: nodeProperty.isRequired(),
                onChange: function (nodeid) {
                    //Case 29390: No sync for Location
                    if (firstSelectHappened) { //CIS-52808: onChange fires once for the currently selected value when initializing the tree and we only want to broadcast USER changes
                        nodeProperty.propData.values.nodeid = nodeid;
                        nodeProperty.broadcastPropChange(nodeid);
                    } else {
                        firstSelectHappened = true;
                    }
                },
                EditMode: nodeProperty.tabState.EditMode,
                value: cswPrivate.nodeId
            });
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());



