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

            cswPrivate.location = nodeProperty.propDiv.location({
                locationobjectclassid: nodeProperty.propData.values.locationobjectclassid,
                locationnodetypeids: nodeProperty.propData.values.locationnodetypeids,
                relatedmatch: (nodeProperty.tabState.relatedobjectclassid === cswPrivate.locationobjectclassid),
                relatednodeid: nodeProperty.tabState.relatednodeid,
                relatednodename: nodeProperty.tabState.relatednodename,
                relatedobjectclassid: nodeProperty.tabState.relatedobjectclassid,
                nodeid: nodeProperty.propData.values.nodeid,
                viewid: nodeProperty.propData.values.viewid,
                selectedName: nodeProperty.propData.values.name,
                path: nodeProperty.propData.values.path,
                nodeKey: '', //(false === o.Multi) ? Csw.string(propVals.nodekey).trim() : '';
                selectednodelink: nodeProperty.propData.values.selectednodelink,
                Multi: nodeProperty.isMulti(),
                ReadOnly: nodeProperty.isReadOnly(),
                isRequired: nodeProperty.isRequired(),
                onChange: function (nodeid) {
                        nodeProperty.propData.values.nodeid = nodeid;
                        nodeProperty.broadcastPropChange(nodeid);
                },
                EditMode: nodeProperty.tabState.EditMode,
                value: cswPrivate.nodeId,
                options: nodeProperty.propData.values.options,
                search: nodeProperty.propData.values.search
            });
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());



