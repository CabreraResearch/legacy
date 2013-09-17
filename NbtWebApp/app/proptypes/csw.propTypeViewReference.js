/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.register('viewReference', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            cswPrivate.viewId = nodeProperty.propData.values.viewid;
            cswPrivate.viewMode = nodeProperty.propData.values.viewmode;

            var table = nodeProperty.propDiv.table();

            if (false === nodeProperty.isMulti()) {
                table.cell(1, 1).$.CswViewContentTree({
                    viewid: cswPrivate.viewId
                });

                table.cell(1, 2).icon({
                    name: nodeProperty.name + '_view',
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
                if (false === nodeProperty.isReadOnly()) {
                    table.cell(1, 3).icon({
                        name: nodeProperty.name + '_edit',
                        hovertext: 'Edit',
                        iconType: Csw.enums.iconType.pencil,
                        size: 16,
                        isButton: true,
                        onClick: function () {
                            nodeProperty.onEditView(cswPrivate.viewId, cswPrivate.viewMode);
                        }
                    });
                }
            }
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());
