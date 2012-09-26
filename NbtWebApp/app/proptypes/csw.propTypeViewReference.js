/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.viewReference = Csw.properties.viewReference ||
        Csw.properties.register('viewReference',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.viewId = Csw.string(cswPrivate.propVals.viewid).trim();
                    cswPrivate.viewMode = Csw.string(cswPrivate.propVals.viewmode).trim().toLowerCase();
                    /* var viewName = Csw.string(propVals.name).trim(); */

                    cswPublic.control = cswPrivate.parent.table({
                        ID: Csw.makeId(cswPublic.data.ID, 'tbl')
                    });

                    if (false === cswPublic.data.isMulti()) {
                        cswPublic.control.cell(1, 1).$.CswViewContentTree({
                            viewid: cswPrivate.viewId
                        });

                        cswPublic.control.cell(1, 2).icon({
                            ID: cswPublic.data.ID + '_view',
                            iconType: Csw.enums.iconType.magglass,
                            hovertext: 'View',
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                Csw.clientState.setCurrentView(cswPrivate.viewId, cswPrivate.viewMode);
                                /* case 20958 - so that it doesn't treat the view as a Grid Property view */
                                Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeId);
                                Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeKey);

                                Csw.window.location(Csw.getGlobalProp('homeUrl'));
                            }
                        });
                        if (false === cswPublic.data.isReadOnly()) {
                            cswPublic.control.cell(1, 3).icon({
                                ID: cswPublic.data.ID + '_edit',
                                hovertext: 'Edit',
                                iconType: Csw.enums.iconType.pencil,
                                size: 16,
                                isButton: true,
                                onClick: function () {
                                    cswPublic.data.onEditView(cswPrivate.viewId);
                                }
                            });
                        }
                    } 

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
