/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('image', function (nodeProperty) {
        'use strict';

        var cswPrivate = Csw.object();

        //The render function to be executed as a callback
        var render = function () {
            'use strict';

            cswPrivate.height = nodeProperty.propData.values.height;
            cswPrivate.width = nodeProperty.propData.values.width;
            cswPrivate.maxFiles = nodeProperty.propData.values.maxfiles;
            cswPrivate.placeholder = nodeProperty.propData.values.placeholder;

            if (0 == cswPrivate.height || cswPrivate.height > 230) {
                cswPrivate.height = 230;
            }
            if (0 == cswPrivate.width || cswPrivate.width > 430) {
                cswPrivate.width = 430;
            }

            if (nodeProperty.isMulti()) {
                nodeProperty.propDiv.append('[Image display disabled]');
            } else {

                var onEdit = function () {
                    nodeProperty.onPropChangeBroadcast();
                };

                cswPrivate.init = function (onSuccess) {
                    cswPrivate.ajax = Csw.ajaxWcf.post({
                        urlMethod: 'BlobData/getImageProp',
                        data: {
                            propid: nodeProperty.propid,
                            date: nodeProperty.tabState.date
                        },
                        success: function (response) {
                            nodeProperty.propDiv.imageGallery({
                                height: cswPrivate.height,
                                width: cswPrivate.width,
                                images: response.Images,
                                maxImages: cswPrivate.maxFiles,
                                propid: nodeProperty.propid,
                                onImageEdit: onEdit,
                                onCaptionEdit: onEdit,
                                onImageDelete: onEdit,
                                readOnly: nodeProperty.isReadOnly(),
                                placeholder: cswPrivate.placeholder,
                                date: nodeProperty.tabState.date
                            });
                        }
                    });
                };
                cswPrivate.init(cswPrivate.makeGallery);
            }
        };

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        nodeProperty.unBindRender(function () {
            if (cswPrivate.ajax) {
                cswPrivate.ajax.abort();
            }
        });

        return true;
    });

}());
