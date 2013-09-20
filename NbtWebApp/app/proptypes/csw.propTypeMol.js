/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('mol', function (nodeProperty) {
        'use strict';

        //The render function to be executed as a callback
        var render = function () {
            'use strict';
            var cswPrivate = Csw.object();

            cswPrivate.mol = nodeProperty.propData.values.mol;
            cswPrivate.href = nodeProperty.propData.values.href;
            cswPrivate.placeholder = nodeProperty.propData.values.placeholder;

            cswPrivate.width = 200;

            var table = nodeProperty.propDiv.table();
            cswPrivate.cell11 = table.cell(1, 1).propDom('colspan', '3');
            table.cell(2, 1).css('width', cswPrivate.width - 36);
            cswPrivate.cell22 = table.cell(2, 2).css('textAlign', 'right');
            cswPrivate.cell23 = table.cell(2, 3).css('textAlign', 'right');

            nodeProperty.onPropChangeBroadcast(function (val) {
                if (cswPrivate.mol !== val.mol || cswPrivate.href !== val.href) {
                    cswPrivate.mol = val.mol;
                    cswPrivate.href = val.href;
                    updateProp(val);
                }
                //nodeProperty.onPropChange({ href: data.href, mol: data.molString });
            });

            var updateProp = function (val) {
                nodeProperty.propData.values.mol = val.mol;
                nodeProperty.propData.values.href = val.href;

                cswPrivate.initMol();
            };

            cswPrivate.initMol = function () {
                cswPrivate.cell11.empty();

                var href = cswPrivate.href;
                if (href) {
                    href = Csw.hrefString(cswPrivate.href);
                } else {
                    href = cswPrivate.placeholder;
                }

                if (nodeProperty.isDisabled()) {
                    cswPrivate.cell11.img({
                        src: href, //case 27492 - FF and IE cache URLs, so we have to make it unique to get new content to display
                        height: nodeProperty.propData.values.height,
                        width: cswPrivate.width


                    });
                } else {
                    cswPrivate.cell11.a({
                        href: href,
                        target: '_blank'
                    }).img({
                        src: href,  //case 27492 - FF and IE cache URLs, so we have to make it unique to get new content to display
                        height: nodeProperty.propData.values.height,
                        width: cswPrivate.width
                    });
                }
            };
            cswPrivate.initMol();

            if (false === nodeProperty.isReadOnly()) {
                /* Edit Button */
                cswPrivate.cell22.div()
                    .icon({
                        name: nodeProperty.name + '_edit',
                        iconType: Csw.enums.iconType.pencil,
                        hovertext: 'Edit',
                        size: 16,
                        isButton: true,
                        onClick: function () {
                            $.CswDialog('EditMolDialog', {
                                PropId: nodeProperty.propData.id,
                                molData: cswPrivate.mol,
                                onSuccess: function (data) {
                                    Csw.properties.publish(nodeProperty.eventName, {
                                        mol: data.molString,
                                        href: data.href
                                    });
                                }
                            });
                        }
                    });

                /* Clear Button */
                cswPrivate.cell23.div()
                    .icon({
                        name: nodeProperty.name + '_clr',
                        iconType: Csw.enums.iconType.trash,
                        hovertext: 'Clear Mol',
                        size: 16,
                        isButton: true,
                        onClick: function () {
                            /* remember: confirm is globally blocking call */
                            if (confirm("Are you sure you want to clear this structure?")) {

                                Csw.ajaxWcf.post({
                                    urlMethod: 'BlobData/clearBlob',
                                    data: {
                                        propid: nodeProperty.propData.id
                                    },
                                    success: function () {
                                        Csw.properties.publish(nodeProperty.eventName, {
                                            mol: '',
                                            href: ''
                                        });
                                    }
                                });

                                Csw.ajaxWcf.post({
                                    urlMethod: 'Mol/ClearMolFingerprint',
                                    data: {
                                        nodeId: Csw.cookie.get(Csw.cookie.cookieNames.CurrentNodeId)
                                    }
                                });
                            }
                        }
                    });

            }
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());



