/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.location = Csw.properties.location ||
        Csw.properties.register('location',
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

                    cswPrivate.location = cswPrivate.parent.location({
                        name: cswPublic.data.name,
                        locationobjectclassid: cswPrivate.propVals.locationobjectclassid,
                        locationnodetypeids: cswPrivate.propVals.locationnodetypeids,
                        relatedmatch: (cswPublic.data.tabState.relatedobjectclassid === cswPrivate.locationobjectclassid),
                        relatednodeid: cswPublic.data.tabState.relatednodeid,
                        relatednodename: cswPublic.data.tabState.relatednodename,
                        relatedobjectclassid: cswPublic.data.tabState.relatedobjectclassid,
                        nodeid: cswPrivate.propVals.nodeid,
                        viewid: cswPrivate.propVals.viewid,
                        selectedName: cswPrivate.propVals.namedItem,
                        path: cswPrivate.propVals.path,
                        nodeKey: '', //(false === o.Multi) ? Csw.string(propVals.nodekey).trim() : '';
                        selectednodelink: cswPrivate.propVals.selectednodelink,
                        Multi: cswPublic.data.isMulti(),
                        ReadOnly: cswPublic.data.isReadOnly(),
                        isRequired: cswPublic.data.isRequired(),
                        onChange: function (nodeid) {
                            Csw.tryExec(cswPublic.data.onChange());
                            cswPublic.data.onPropChange({ nodeid: nodeid });
                        },
                        EditMode: cswPublic.data.tabState.EditMode,
                        value: cswPrivate.nodeId
                    });
                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

}());



