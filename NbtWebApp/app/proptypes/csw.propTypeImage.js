/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.image = Csw.properties.register('image',
        function (nodeProperty) {
            'use strict';

            //The render function to be executed as a callback
            var render = function () {
                'use strict';

                var cswPrivate = Csw.object();
                cswPrivate.thumbnails = [];
                cswPrivate.height = nodeProperty.propData.values.height;
                cswPrivate.width = nodeProperty.propData.values.width;

                if (0 == cswPrivate.height || cswPrivate.height > 230) {
                    cswPrivate.height = 230;
                }
                if (0 == cswPrivate.width || cswPrivate.width > 430) {
                    cswPrivate.width = 430;
                }

                if (nodeProperty.isMulti()) {
                    nodeProperty.propDiv.append('[Image display disabled]');
                } else {

                    cswPrivate.uploadImgDialog = function (propid, blobid, caption) {
                        $.CswDialog('FileUploadDialog', {
                            urlMethod: 'Services/BlobData/SaveFile',
                            params: {
                                propid: propid,
                                blobdataid: blobid,
                                caption: caption
                            },
                            onSuccess: function (data) {
                                if (data.Data.success) {
                                    cswPrivate.makeSelectedImg(data.Data.href, data.Data.filename, data.Data.blobdataid, data.Data.caption);
                                    cswPrivate.init(cswPrivate.makeThumbnails);
                                    nodeProperty.onPropChangeBroadcast();
                                }
                            }
                        });
                    };

                    cswPrivate.makeSelectedImg = function (src, alt, id, caption) {
                        var renderSelectedImg = function () {
                            cswPrivate.selectedImgDiv.empty();
                            cswPrivate.selectedImg = cswPrivate.selectedImgDiv.a({
                                href: src,
                                target: "_blank"
                            }).img({
                                src: src,
                                alt: alt
                            }).css({ 'max-height': cswPrivate.height });

                            cswPrivate.selectedImg.data('ImageUrl', src);
                            cswPrivate.selectedImg.data('FileName', alt);
                            cswPrivate.selectedImg.data('BlobDataId', id);
                            cswPrivate.selectedImg.data('Caption', caption);

                            if (false === Csw.isNullOrEmpty(id) && false === nodeProperty.isReadOnly()) {
                                cswPrivate.captionDiv.empty();
                                cswPrivate.captionDiv.text(caption);
                                cswPrivate.editCaptionDiv.empty();
                                cswPrivate.editCaptionDiv.buttonExt({
                                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                                    enabledText: 'Edit Caption',
                                    disableOnClick: false,
                                    onClick: function () {
                                        var blobdataid = cswPrivate.selectedImg.data('BlobDataId');
                                        var comment = cswPrivate.captionDiv.text();
                                        $.CswDialog('EditCommentDialog', {
                                            comment: comment,
                                            blobdataid: blobdataid,
                                            onSave: function (newCaption) {
                                                cswPrivate.captionDiv.text(newCaption);
                                                cswPrivate.selectedImg.data('Caption', newCaption);
                                                Csw.iterate(cswPrivate.thumbnails, function (thumb) {
                                                    if (thumb.data('BlobDataId') === blobdataid) {
                                                        thumb.data('Caption', newCaption);
                                                    }
                                                });
                                            }
                                        });
                                    }
                                });

                                //Edit selected
                                cswPrivate.editSelectedImgBtn = cswPrivate.selectedImgDiv.buttonExt({
                                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                                    onClick: function () {
                                        cswPrivate.uploadImgDialog(nodeProperty.propid, cswPrivate.selectedImg.data('BlobDataId'), cswPrivate.captionDiv.text());
                                        cswPrivate.editSelectedImgBtn.enable();
                                    }
                                });
                                cswPrivate.editSelectedImgBtn.enable();

                                //Delete selected
                                cswPrivate.deleteSelectedImgBtn = cswPrivate.selectedImgDiv.buttonExt({
                                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.trash),
                                    onClick: function () {
                                        $.CswDialog('ConfirmDialog',
                                            'Are you sure you want to delete this image?',
                                            'Confirm Intent To Delete Image',
                                            function () {
                                                Csw.ajaxWcf.post({
                                                    urlMethod: 'BlobData/clearImage',
                                                    data: {
                                                        blobdataid: cswPrivate.selectedImg.data('BlobDataId'),
                                                        propid: nodeProperty.propid
                                                    },
                                                    success: function (Response) {
                                                        var firstImg = Response.Images[0];
                                                        cswPrivate.makeSelectedImg(firstImg.ImageUrl, firstImg.FileName, firstImg.BlobDataId, firstImg.Caption);
                                                        cswPrivate.makeThumbnails(Response.Images);
                                                    }
                                                });
                                                cswPrivate.deleteSelectedImgBtn.enable();
                                            },
                                            function () {
                                                cswPrivate.deleteSelectedImgBtn.enable();
                                            }
                                        );
                                    }
                                });

                                cswPrivate.selectedImgDiv.div().a({
                                    href: src,
                                    target: '_blank',
                                    text: alt
                                });
                            }

                            window.Ext.get(cswPrivate.selectedImageTbl.getId()).fadeIn({
                                opacity: 1,
                                easing: 'easeOut',
                                duration: 400
                            });
                        };

                        window.Ext.get(cswPrivate.selectedImageTbl.getId()).fadeOut({
                            opacity: 0,
                            easing: 'easeOut',
                            duration: 400,
                            useDisplay: false,
                            remove: false,
                            callback: renderSelectedImg
                        });

                    };

                    cswPrivate.makeGallery = function (images) {

                        nodeProperty.propDiv.empty();
                        cswPrivate.outerTbl = nodeProperty.propDiv.table({
                            cellpadding: 5
                        }).css({
                            "border": "1px solid #DFE1E4",
                            "height": cswPrivate.height + 150,
                            "width": cswPrivate.width + 200, //"85%",
                            "background-color": "#E5F0FF"
                        });
                        cswPrivate.selectedImageTbl = cswPrivate.outerTbl.cell(1, 1).table().css({
                            'margin': 'auto'
                        });
                        cswPrivate.selectedImgCell = cswPrivate.selectedImageTbl.cell(1, 1).css({
                            "height": "265px",
                            "width": "100%",
                            "text-align": "center",
                            "vertical-align": "middle",
                            "padding-top": "15px"
                        });
                        cswPrivate.selectedImgDiv = cswPrivate.selectedImgCell.div();

                        cswPrivate.captionCell = cswPrivate.selectedImageTbl.cell(2, 1).table({
                            cellpadding: 2,
                            cellspacing: 2
                        }).css({
                            //'width': '100%',
                            'margin': 'auto'
                        });
                        cswPrivate.captionScrollable = cswPrivate.captionCell.cell(1, 1).css({
                            'overflow': 'auto'
                        });
                        cswPrivate.captionContainer = cswPrivate.captionScrollable.div().css({
                            'width': '100%'
                        });
                        cswPrivate.captionDiv = cswPrivate.captionContainer.div().css({
                            height: '50px'
                        });
                        cswPrivate.editCaptionDiv = cswPrivate.captionCell.cell(1, 2);

                        cswPrivate.scrollable = cswPrivate.outerTbl.cell(2, 1).div().css({
                            "width": "100%",
                            "overflow": "auto",
                            "border-top": "1px solid #62BBE9"
                        });
                        cswPrivate.container = cswPrivate.scrollable.div().css({
                            "width": "100px",
                            "padding": "8px"
                        });
                        cswPrivate.addBtn = cswPrivate.container.buttonExt({
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.plus),
                            enabledText: 'Add Image',
                            onClick: function () {
                                cswPrivate.uploadImgDialog(nodeProperty.propid, '', '');
                                cswPrivate.addBtn.enable();
                            },
                        });
                        cswPrivate.thumbsTbl = cswPrivate.container.table({
                            cellpadding: 2,
                            cellspacing: 5
                        });

                        //Make the selected image
                        var firstImg = images[0];
                        cswPrivate.makeSelectedImg(firstImg.ImageUrl, firstImg.FileName, firstImg.BlobDataId, firstImg.Caption);
                        cswPrivate.makeThumbnails(images);
                    };

                    cswPrivate.makeThumbnails = function (images) {
                        //Make thumbnails
                        if (images.length > 1) {
                            var renderThumbnails = function () {
                                cswPrivate.thumbsTbl.empty();
                                var colNo = 1;
                                Csw.iterate(images, function (image) {
                                    var thumbCell = cswPrivate.thumbsTbl.cell(1, colNo);
                                    thumbCell.data("ImageUrl", image.ImageUrl);
                                    thumbCell.data("FileName", image.FileName);
                                    thumbCell.data("BlobDataId", image.BlobDataId);
                                    thumbCell.data("Caption", image.Caption);
                                    cswPrivate.thumbnails.push(thumbCell);
                                    thumbCell.css({
                                        "height": "85px",
                                        "width": "85px",
                                        "vertical-align": "middle",
                                        "margin": "2px",
                                        "border": "1px solid #E2EBF4"
                                    });
                                    var img = thumbCell.img({
                                        src: image.ImageUrl,
                                        alt: image.FileName,
                                        width: '75px',
                                        onClick: function () {
                                            cswPrivate.makeSelectedImg(thumbCell.data('ImageUrl'), thumbCell.data('FileName'), thumbCell.data('BlobDataId'), thumbCell.data('Caption'));
                                        }
                                    });

                                    img.$.hover(
                                        function () {
                                            thumbCell.css({
                                                "border": "1px solid #62BBE9"
                                            });
                                        },
                                        function () {
                                            thumbCell.css({
                                                "border": "1px solid #E2EBF4"
                                            });
                                        }
                                    );
                                    colNo++;
                                });

                                window.Ext.get(cswPrivate.thumbsTbl.getId()).fadeIn({
                                    opacity: 1,
                                    easing: 'easeOut',
                                    duration: 400
                                });
                            };

                            window.Ext.get(cswPrivate.thumbsTbl.getId()).fadeOut({
                                opacity: 0,
                                easing: 'easeOut',
                                duration: 400,
                                useDisplay: false,
                                remove: false,
                                callback: renderThumbnails
                            });
                        }
                    };

                    cswPrivate.init = function (onSuccess) {
                        Csw.ajaxWcf.post({
                            urlMethod: 'BlobData/getImageProp',
                            data: {
                                propid: nodeProperty.propid
                            },
                            success: function (Response) {
                                Csw.tryExec(onSuccess, Response.Images);
                            }
                        });
                    };
                    cswPrivate.init(cswPrivate.makeGallery);
                }
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender(function() {

            return true;
        });

}());
