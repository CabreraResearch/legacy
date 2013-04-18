/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.image = Csw.properties.image ||
        Csw.properties.register('image',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {
                    width: ''
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

                        cswPrivate.uploadImgDialog = function (propid, blobid) {
                            $.CswDialog('FileUploadDialog', {
                                urlMethod: 'Services/BlobData/SaveFile',
                                params: {
                                    propid: propid,
                                    blobdataid: blobid
                                },
                                onSuccess: function (data) {
                                    if (data.Data.success) {
                                        cswPrivate.makeSelectedImg(data.Data.href, data.Data.filename, data.Data.blobdataid);
                                        cswPrivate.init(cswPrivate.makeThumbnails);
                                    }
                                }
                            });
                        };

                        cswPrivate.makeSelectedImg = function (src, alt, id) {
                            var renderSelectedImg = function () {
                                cswPrivate.selectedImgDiv.empty();
                                cswPrivate.selectedImg = cswPrivate.selectedImgDiv.img({
                                    src: src,
                                    alt: alt,
                                    height: cswPrivate.propVals.height,
                                    width: cswPrivate.propVals.width
                                }).css({ 'max-height': '230px' });
                                cswPrivate.selectedImg.data('ImageUrl', src);
                                cswPrivate.selectedImg.data('FileName', alt);
                                cswPrivate.selectedImg.data('BlobDataId', id);

                                if (false === Csw.isNullOrEmpty(id)) {
                                    //Edit selected
                                    cswPrivate.editSelectedImgBtn = cswPrivate.selectedImgDiv.buttonExt({
                                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.pencil),
                                        onClick: function () {
                                            cswPrivate.uploadImgDialog(cswPublic.data.propData.id, cswPrivate.selectedImg.data('BlobDataId'));
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
                                                            cswPrivate.makeSelectedImg(firstImg.ImageUrl, firstImg.FileName, firstImg.BlobDataId);
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
                                }

                                window.Ext.get(cswPrivate.selectedImgDiv.getId()).fadeIn({
                                    opacity: 1,
                                    easing: 'easeOut',
                                    duration: 400
                                });
                            };

                            window.Ext.get(cswPrivate.selectedImgDiv.getId()).fadeOut({
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
                                //"border": "1px solid black",
                                "width": "600px",
                                "background-color": "#dddddd",
                                "border-top": "2px solid #f0f0f0",
                                "border-left": "2px solid #f0f0f0",
                                "border-bottom": "2px solid #b0b0b0",
                                "border-right": "2px solid #b0b0b0"
                            });
                            cswPrivate.selectedImgCell = cswPrivate.outerTbl.cell(1, 1).css({
                                //"border": "1px solid green",
                                "height": "235px",
                                "width": "100%",
                                "text-align": "center",
                                "vertical-align": "middle",
                                "padding-top": "15px"
                            });
                            cswPrivate.selectedImgDiv = cswPrivate.selectedImgCell.div();
                            cswPrivate.scrollable = cswPrivate.outerTbl.cell(2, 1).div().css({
                                "width": "100%",
                                "overflow": "auto",
                                //"border": "1px solid blue"
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
                            cswPrivate.makeSelectedImg(firstImg.ImageUrl, firstImg.FileName, firstImg.BlobDataId);
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
                                    thumbCell.css({
                                        //"border": "1px solid orange",
                                        "height": "85px",
                                        "width": "85px",
                                        "vertical-align": "middle"
                                    });
                                    thumbCell.img({
                                        src: image.ImageUrl,
                                        alt: image.FileName,
                                        width: '75px',
                                        onClick: function () {
                                            cswPrivate.selectedImgDiv.$.fadeOut(400, function () {
                                                cswPrivate.makeSelectedImg(thumbCell.data('ImageUrl'), thumbCell.data('FileName'), thumbCell.data('BlobDataId'));
                                            });
                                        }
                                    });
                                    var fileNameCell = cswPrivate.thumbsTbl.cell(2, colNo).css({
                                        'text-align': 'center',
                                        'font-size': '80%'
                                        //'border': '1px solid blue'
                                    });
                                    fileNameCell.text(image.FileName);
                                    colNo++;
                                });

                                //create an Add button
                                var addCell = cswPrivate.thumbsTbl.cell(1, colNo).css({
                                    'text-align': 'center',
                                    'vertical-align': 'middle'
                                });
                                debugger;
                                cswPrivate.addBtn = addCell.buttonExt({
                                    icon: 'add',//Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.plus),
                                    size: 'medium',
                                    enabledText: 'Add Image',
                                    onClick: function () {
                                        cswPrivate.uploadImgDialog(cswPublic.data.propData.id, '');
                                        cswPrivate.addBtn.enable();
                                    },
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

                        //cswPrivate.href = Csw.hrefString(cswPrivate.propVals.href);
                        //
                        //if (false === Csw.isNullOrEmpty(cswPrivate.propVals.width) &&
                        //    Csw.isNumeric(cswPrivate.propVals.width)) {
                        //    cswPrivate.width = Math.abs(Csw.number(cswPrivate.propVals.width, 100) - 36);
                        //}
                        //
                        //cswPrivate.fileName = Csw.string(cswPrivate.propVals.name).trim();
                        //
                        //cswPublic.control = cswPrivate.parent.table();
                        //cswPrivate.cell11 = cswPublic.control.cell(1, 1).propDom('colspan', '3');
                        //cswPrivate.cell21 = cswPublic.control.cell(2, 1).propDom('width', cswPrivate.width);
                        //cswPrivate.cell22 = cswPublic.control.cell(2, 2).propDom({ align: 'right', width: '20px' }).div();
                        //cswPrivate.cell23 = cswPublic.control.cell(2, 3).propDom({ align: 'right', width: '20px' }).div();
                        //
                        //cswPrivate.makeClr = function () {
                        //    cswPrivate.cell23.empty();
                        //    if (false === Csw.isNullOrEmpty(cswPrivate.fileName)) {
                        //        //Clear button
                        //        cswPrivate.cell23.icon({
                        //            name: 'clear',
                        //            iconType: Csw.enums.iconType.trash,
                        //            hovertext: 'Clear Image',
                        //            size: 16,
                        //            isButton: true,
                        //            onClick: function () {
                        //                /* remember: confirm is globally blocking call */
                        //                if (confirm("Are you sure you want to clear this image?")) {
                        //                    var dataJson = {
                        //                        propid: cswPublic.data.propData.id,
                        //                        IncludeBlob: true
                        //                    };
                        //
                        //                    Csw.ajaxWcf.post({
                        //                        urlMethod: 'BlobData/clearBlob',
                        //                        data: dataJson,
                        //                        success: function () {
                        //                            var val = {
                        //                                href: '',
                        //                                name: '',
                        //                                contenttype: ''
                        //                            };
                        //                            cswPrivate.makeImg(null);
                        //                            cswPublic.data.onPropChange(val);
                        //                            Csw.publish(Csw.enums.events.main.refreshSelected, {});
                        //                        }
                        //                    });
                        //                }
                        //            }
                        //        }); // icon
                        //    } // if (false === Csw.isNullOrEmpty(fileName)) {
                        //};
                        //
                        //cswPrivate.makeImg = function (imgData) {
                        //    cswPrivate.cell11.empty();
                        //    cswPrivate.cell21.empty();
                        //    if (false == Csw.isNullOrEmpty(imgData)) {
                        //        cswPrivate.fileName = imgData.fileName;
                        //        cswPrivate.cell11.a({
                        //            href: imgData.href,
                        //            target: '_blank'
                        //        })
                        //            .img({
                        //                src: imgData.href,
                        //                alt: imgData.fileName,
                        //                height: cswPrivate.propVals.height,
                        //                width: cswPrivate.propVals.width
                        //            });
                        //        cswPrivate.cell21.a({
                        //            href: imgData.href,
                        //            target: '_blank',
                        //            text: imgData.fileName
                        //        });
                        //    }
                        //    cswPrivate.makeClr();
                        //};
                        //cswPrivate.makeImg(cswPrivate);
                        //
                        //if (false === cswPublic.data.isReadOnly()) {
                        //    //Clear button
                        //    cswPrivate.makeClr();
                        //
                        //    //Edit button
                        //    cswPrivate.cell22.icon({
                        //        name: 'edit',
                        //        iconType: Csw.enums.iconType.pencil,
                        //        hovertext: 'Edit',
                        //        size: 16,
                        //        isButton: true,
                        //        onClick: function () {
                        //            $.CswDialog('FileUploadDialog', {
                        //                urlMethod: 'Services/BlobData/SaveFile',
                        //                params: {
                        //                    propid: cswPublic.data.propData.id
                        //                },
                        //                onSuccess: function (data) {
                        //                    var val = {
                        //                        href: data.Data.href,
                        //                        name: data.Data.filename,
                        //                        fileName: data.Data.filename,
                        //                        contenttype: data.Data.contenttype
                        //                    };
                        //                    if (data.Data.success) {
                        //                        cswPrivate.makeImg(val);
                        //                        cswPublic.data.onPropChange(val);
                        //                    }
                        //                }
                        //            });
                        //        }
                        //    }); // icon
                        //
                        //} // if (false === o.ReadOnly && o.EditMode !== Csw.enums.editMode.Add) {
                    }


                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

}());
