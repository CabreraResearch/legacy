/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.image = Csw.properties.image ||
        Csw.properties.register('image',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {
                    width: '',
                    thumbnails: []
                };
                var cswPublic = {
                    data: propertyOption
                };

                //The render function to be executed as a callback
                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    if (cswPublic.data.isMulti()) {
                        cswPublic.control = cswPrivate.parent.append('[Image display disabled]');
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
                                    alt: alt,
                                    height: cswPrivate.propVals.height,
                                    width: cswPrivate.propVals.width
                                }).css({ 'max-height': '230px' });

                                cswPrivate.selectedImg.data('ImageUrl', src);
                                cswPrivate.selectedImg.data('FileName', alt);
                                cswPrivate.selectedImg.data('BlobDataId', id);
                                cswPrivate.selectedImg.data('Caption', caption);

                                if (false === Csw.isNullOrEmpty(id) && false === cswPublic.data.isReadOnly()) {
                                    cswPrivate.captionCell.empty();
                                    cswPrivate.caption = cswPrivate.captionCell.textArea({
                                        text: caption,
                                        cols: 50,
                                        rows: 2,
                                        onChange: function () {
                                            var newCaption = cswPrivate.caption.val();
                                            var blobid = cswPrivate.selectedImg.data('BlobDataId');

                                            cswPrivate.selectedImg.data('BlobDataId', newCaption);
                                            Csw.iterate(cswPrivate.thumbnails, function (thumbnail) {
                                                if (thumbnail.data('BlobDataId') === blobid) {
                                                    thumbnail.data('Caption', newCaption);
                                                }
                                            });

                                            Csw.ajaxWcf.post({
                                                urlMethod: 'BlobData/saveCaption',
                                                data: {
                                                    blobdataid: blobid,
                                                    caption: newCaption
                                                },
                                                success: function () {
                                                    alert('saved');
                                                }
                                            });
                                        }
                                    });

                                    //Edit selected
                                    cswPrivate.editSelectedImgBtn = cswPrivate.selectedImgDiv.buttonExt({
                                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                                        onClick: function () {
                                            cswPrivate.uploadImgDialog(cswPublic.data.propData.id, cswPrivate.selectedImg.data('BlobDataId'), cswPrivate.caption.val());
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
                                                            propid: cswPublic.data.propData.id
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
                                    
                                } else {
                                    cswPrivate.captionCell.text(caption);
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

                            cswPrivate.parent.empty();
                            cswPrivate.outerTbl = cswPrivate.parent.table({
                                cellpadding: 5
                            }).css({
                                "border": "1px solid #CCCCFF",
                                "width": "85%",
                                "background-color": "#EEEEFF"
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

                            cswPrivate.captionCell = cswPrivate.selectedImageTbl.cell(2, 1).css({
                                'text-align': 'center'
                            });

                            cswPrivate.scrollable = cswPrivate.outerTbl.cell(2, 1).div().css({
                                "width": "100%",
                                "overflow": "auto",
                                "border-top": "1px solid #AAAAFF"
                            });
                            cswPrivate.container = cswPrivate.scrollable.div().css({
                                "width": "100px",
                                "padding": "8px"
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
                                        "border": "1px solid #EEEEFF"
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
                                                "border": "1px solid #AAAAFF"
                                            });
                                        },
                                        function () {
                                            thumbCell.css({
                                                "border": "1px solid #EEEEFF"
                                            });
                                        }
                                    );
                                    var fileNameCell = cswPrivate.thumbsTbl.cell(2, colNo).css({
                                        'text-align': 'center',
                                        'font-size': '80%'
                                        //'border': '1px solid blue'
                                    });
                                    fileNameCell.text(image.FileName);
                                    colNo++;
                                });

                                if (false === cswPublic.data.isReadOnly()) {
                                    //create an Add button
                                    var addCell = cswPrivate.thumbsTbl.cell(1, colNo).css({
                                        'text-align': 'center',
                                        'vertical-align': 'middle'
                                    });
                                    cswPrivate.addBtn = addCell.buttonExt({
                                        icon: 'add',//Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.plus),
                                        size: 'medium',
                                        enabledText: 'Add Image',
                                        onClick: function () {
                                            cswPrivate.uploadImgDialog(cswPublic.data.propData.id, '', '');
                                            cswPrivate.addBtn.enable();
                                        },
                                    });
                                }

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
                        };

                        cswPrivate.init = function (onSuccess) {
                            Csw.ajaxWcf.post({
                                urlMethod: 'BlobData/getImageProp',
                                data: {
                                    propid: cswPublic.data.propData.id
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
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

}());
