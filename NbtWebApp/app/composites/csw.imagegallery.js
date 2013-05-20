/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.composites.imageGallery = Csw.composites.imageGallery ||
        Csw.composites.register('imageGallery', function (cswParent, options) {
            /// <summary>
            /// 
            ///</summary>
            'use strict';
            var cswPrivate = {
                saveImageUrl: 'Services/BlobData/SaveFile',
                saveCaptionUrl: 'BlobData/SaveCaption',
                deleteUrl: 'BlobData/clearImage',
                readOnly: false,
                maxImages: 1,
                height: 230,
                width: 430,
                onImageEdit: function () { },
                onCaptionEdit: function () { },
                onImageDelete: function () { },
                propid: '',
                placeholder: '',
                images: [], //[{ImageUrl: '', FileName: '',  BlobDataId: '', Caption: ''}, {...}]
                thumbnails: [],
                selectedImg: {
                    BlobDataId: Csw.int32MinVal,
                    FileName: '',
                    Caption: '',
                    ImageUrl: ''
                }
            };
            var cswPublic = {

            };

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                if (0 == cswPrivate.height || cswPrivate.height > 230) {
                    cswPrivate.height = 230;
                }
                if (0 == cswPrivate.Width || cswPrivate.width > 430) {
                    cswPrivate.width = 430;
                }

                cswPrivate.mainDiv = cswParent.div();

                cswPrivate.onEditImage = function(response) {
                    if (response.Data.success) {

                        var newImg = response.Data.Image;
                        cswPrivate.makeSelectedImg(newImg.ImageUrl, newImg.FileName, newImg.BlobDataId, newImg.Caption);

                        if (cswPrivate.thumbnails.length > 1 && cswPrivate.thumbnails[0].data('BlobDataId') === Csw.int32MinVal) {
                            cswPrivate.thumbnails = [];
                        }
                        if (cswPrivate.images[0].BlobDataId === Csw.int32MinVal) {
                            cswPrivate.images = [];
                        }

                        var doAdd = true;
                        Csw.iterate(cswPrivate.images, function (img) {
                            if (img.BlobDataId === newImg.BlobDataId) {
                                var idx = cswPrivate.images.indexOf(img);
                                cswPrivate.images[idx] = newImg;
                                doAdd = false;
                            }
                        });
                        if (doAdd) {
                            cswPrivate.images.push(newImg);
                        }
                        cswPrivate.makeThumbnails(cswPrivate.images);
                        cswPrivate.onImageEdit();
                    }
                };

                cswPrivate.uploadImgDialog = function (ImageUrl, FileName, BlobDataId, Caption) {
                    $.CswDialog('EditImageDialog', {
                        selectedImg: {
                            ImageUrl: ImageUrl,
                            FileName: FileName,
                            BlobDataId: BlobDataId,
                            Caption: Caption
                        },
                        placeholder: cswPrivate.placeholder,
                        height: cswPrivate.height,
                        deleteUrl: cswPrivate.deleteUrl,
                        saveCaptionUrl: cswPrivate.saveCaptionUrl,
                        saveImgUrl: cswPrivate.saveImageUrl,
                        propid: cswPrivate.propid,
                        onEditImg: function (response) {
                            cswPrivate.onEditImage(response);
                        },
                        onDeleteImg: function (response) {
                            var firstImg = response.Images[0];
                            cswPrivate.makeSelectedImg(firstImg.ImageUrl, firstImg.FileName, firstImg.BlobDataId, firstImg.Caption);
                            cswPrivate.images = response.Images;
                            cswPrivate.makeThumbnails(response.Images);
                            cswPrivate.toggleAddBtn();
                            cswPrivate.onImageDelete();
                        },
                        onSave: function (newCaption, blobdataid) {
                            cswPrivate.captionDiv.text(newCaption);
                            cswPrivate.selectedImg.Caption = newCaption;
                            Csw.iterate(cswPrivate.images, function(img) {
                                if (img.BlobDataId === blobdataid) {
                                    img.Caption = newCaption;
                                }
                            });
                            cswPrivate.makeThumbnails(cswPrivate.images);
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

                        cswPrivate.selectedImg.ImageUrl = src;
                        cswPrivate.selectedImg.FileName = alt;
                        cswPrivate.selectedImg.BlobDataId = id;
                        cswPrivate.selectedImg.Caption = caption;

                        cswPrivate.captionDiv.empty();
                        cswPrivate.captionDiv.text(caption);

                        if (false === cswPrivate.readOnly) {
                            //Edit selected
                            if (Csw.int32MinVal !== id) {
                                cswPrivate.editSelectedImgBtn = cswPrivate.selectedImgDiv.buttonExt({
                                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                                    disableOnClick: false,
                                    onClick: function () {
                                        cswPrivate.uploadImgDialog(
                                            cswPrivate.selectedImg.ImageUrl,
                                            cswPrivate.selectedImg.FileName,
                                            cswPrivate.selectedImg.BlobDataId,
                                            cswPrivate.captionDiv.text());
                                    }
                                });
                            }
                            cswPrivate.toggleAddBtn();
                        }
                        
                        if (imgTblInNode.Ext) {
                            imgTblInNode.Ext.fadeIn({
                                opacity: 1,
                                easing: 'easeOut',
                                duration: 400
                            });
                        }
                    };
                    
                    var imgTblInNode = Csw.domNode({ ID: cswPrivate.selectedImageTbl.getId() });
                    if (imgTblInNode.Ext) {
                        imgTblInNode.Ext.fadeOut({
                            opacity: 0,
                            easing: 'easeOut',
                            duration: 400,
                            useDisplay: false,
                            remove: false,
                            callback: renderSelectedImg
                        });
                    }
                };

                cswPrivate.toggleAddBtn = function () {
                    if (cswPrivate.maxImages !== 1) {
                        if (cswPrivate.thumbnails.length >= cswPrivate.maxImages) {
                            cswPrivate.addBtn.disable();
                        } else {
                            cswPrivate.addBtn.enable();
                        }
                    }
                    if (cswPrivate.readOnly || (cswPrivate.maxImages === 1) && cswPrivate.selectedImg.BlobDataId !== Csw.int32MinVal) {
                        cswPrivate.addBtn.hide();
                    } else {
                        cswPrivate.addBtn.show();
                    }
                };

                cswPrivate.makeGallery = function (images) {

                    cswPrivate.mainDiv.empty();
                    cswPrivate.outerTbl = cswPrivate.mainDiv.table({
                        cellpadding: 5
                    }).css({
                        "border": "1px solid #DFE1E4",
                        "height": cswPrivate.height,
                        "width": cswPrivate.width + 20,
                        "background-color": "#E5F0FF"
                    });
                    cswPrivate.selectedImageTbl = cswPrivate.outerTbl.cell(1, 1).table().css({
                        'margin': 'auto'
                    });
                    cswPrivate.selectedImgCell = cswPrivate.selectedImageTbl.cell(1, 1).css({
                        "height": cswPrivate.height, //explicit for smoothness
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
                        'margin': 'auto'
                    });
                    cswPrivate.captionScrollable = cswPrivate.captionCell.cell(1, 1).css({
                        'overflow': 'auto'
                    });
                    cswPrivate.captionContainer = cswPrivate.captionScrollable.div().css({
                        'width': '100%'
                    });
                    cswPrivate.captionDiv = cswPrivate.captionContainer.div();

                    //Make the selected image
                    var firstImg = images[0];
                    cswPrivate.makeSelectedImg(firstImg.ImageUrl, firstImg.FileName, firstImg.BlobDataId, firstImg.Caption);

                    cswPrivate.scrollable = cswPrivate.outerTbl.cell(2, 1).div().css({
                        "width": "100%",
                        "overflow": "auto",
                        "border-top": cswPrivate.maxImages === 1 ? "" : "1px solid #62BBE9"
                    });
                    cswPrivate.container = cswPrivate.scrollable.div().css({
                        "width": "100px"
                    });

                    cswPrivate.addBtn = cswPrivate.container.div().buttonExt({
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.plus),
                        enabledText: 'Add Image',
                        disableOnClick: false,
                        onClick: function () {
                            $.CswDialog("FileUploadDialog", {
                                urlMethod: cswPrivate.saveImageUrl,
                                params: {
                                    propid: cswPrivate.propid
                                },
                                onSuccess: function (response) {
                                    cswPrivate.onEditImage(response);
                                    cswPrivate.uploadImgDialog(response.Data.Image.ImageUrl, response.Data.Image.FileName, response.Data.Image.BlobDataId, '');
                                }
                            });
                        },
                    }).css('padding', '3px');

                    if (cswPrivate.maxImages > 1) {
                        cswPrivate.thumbsTbl = cswPrivate.container.table({
                            cellpadding: 2,
                            cellspacing: 5
                        });
                        cswPrivate.makeThumbnails(images);
                    }
                };

                cswPrivate.makeThumbnails = function (images) {
                    //Make thumbnails
                    if (cswPrivate.thumbsTbl) {
                        cswPrivate.thumbsTbl.empty();
                        if (images.length > 1 && cswPrivate.maxImages > 1) {
                            var renderThumbnails = function () {
                                cswPrivate.thumbnails = [];
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

                                cswPrivate.toggleAddBtn();
                                if (imgThumbsNode.Ext) {
                                    imgThumbsNode.Ext.fadeIn({
                                        opacity: 1,
                                        easing: 'easeOut',
                                        duration: 400
                                    });
                                }
                            };

                            var imgThumbsNode = Csw.domNode({ ID: cswPrivate.thumbsTbl.getId() });
                            if (imgThumbsNode.Ext) {
                                imgThumbsNode.Ext.fadeOut({
                                    opacity: 0,
                                    easing: 'easeOut',
                                    duration: 400,
                                    useDisplay: false,
                                    remove: false,
                                    callback: renderThumbnails
                                });
                            }
                        }
                    }
                };

                cswPrivate.makeGallery(cswPrivate.images);

            }());



            return cswPublic;
        });

}());
