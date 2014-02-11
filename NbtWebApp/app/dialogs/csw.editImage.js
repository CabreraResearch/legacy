/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('editImage', function (cswPrivate) {
        'use strict';

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || 'Edit Image';
            cswPrivate.height = cswPrivate.height || 405;
            cswPrivate.width = cswPrivate.width || 550;
            cswPrivate.imgHeight = cswPrivate.imgHeight || 230;
            cswPrivate.placeholder = cswPrivate.placeholder || '';
            cswPrivate.propid = cswPrivate.propid || '';
            cswPrivate.selectedImg = cswPrivate.selectedImg || {
                BlobUrl: '',
                FileName: '',
                BlobDataId: '',
                Caption: ''
            };
            cswPrivate.saveCaptionUrl = cswPrivate.saveCaptionUrl || 'BlobData/saveCaption';
            cswPrivate.deleteUrl = cswPrivate.deleteUrl || 'BlobData/clearImage';
            cswPrivate.saveImgUrl = cswPrivate.saveImgUrl || 'Services/BlobData/SaveFile';
            cswPrivate.onEditImg = cswPrivate.onEditImg || function () { };
            cswPrivate.onDeleteImg = cswPrivate.onDeleteImg || function () { };
            cswPrivate.onSave = cswPrivate.onSave || function () { };
            cswPrivate.onOpen = cswPrivate.onOpen || function () { };
            cswPrivate.onClose = cswPrivate.onClose || function () { };
        }());

        return (function () {
            'use strict';
            var editImageDialog = Csw.layouts.dialog({
                title: cswPrivate.title,
                width: cswPrivate.width,
                height: cswPrivate.height,
                onOpen: cswPrivate.onOpen,
                onClose: cswPrivate.onClose
            });

            var div = editImageDialog.div;

            var tbl = div.table({
                cellspacing: 2,
                cellpadding: 2
            });

            var imgCell = tbl.cell(1, 1).css({
                "text-align": "center",
                "vertical-align": "middle",
                "padding-top": "5px"
            });
            imgCell.img({
                src: cswPrivate.selectedImg.BlobUrl,
                alt: cswPrivate.selectedImg.FileName,
                height: cswPrivate.imgHeight
            });

            var makeBtns = function() {
                imgCell.icon({
                    name: 'uploadnewImgBtn',
                    iconType: Csw.enums.iconType.pencil,
                    hovertext: 'Edit this image',
                    isButton: true,
                    onClick: function() {
                        Csw.dialogs.fileUpload({
                            urlMethod: cswPrivate.saveImgUrl,
                            params: {
                                propid: cswPrivate.propid,
                                blobdataid: cswPrivate.selectedImg.BlobDataId,
                                caption: textArea.val()
                            },
                            forceIframeTransport: true,
                            dataType: 'iframe',
                            onSuccess: function(response) {
                                var newImg = {
                                    BlobUrl: Csw.getPropFromIFrame(response, 'BlobUrl', true),
                                    FileName: Csw.getPropFromIFrame(response, 'FileName', true),
                                    BlobDataId: Csw.number(Csw.getPropFromIFrame(response, 'BlobDataId', true), Csw.int32MinVal),
                                    Caption: textArea.val()
                                };

                                imgCell.empty();
                                imgCell.img({
                                    src: Csw.hrefString(newImg.BlobUrl),
                                    alt: newImg.FileName,
                                    height: cswPrivate.imgHeight
                                });
                                cswPrivate.selectedImg = newImg;
                                saveBtn.enable();
                                makeBtns();
                                cswPrivate.onEditImg(newImg);
                            }
                        }).open();
                    }
                });
                if (false == Csw.isNullOrEmpty(cswPrivate.selectedImg.BlobDataId)) {
                    imgCell.icon({
                        name: 'clearImgBtn',
                        iconType: Csw.enums.iconType.trash,
                        hovertext: 'Clear this image',
                        isButton: true,
                        onClick: function() {
                            var confirmDialog = Csw.dialogs.confirmDialog({
                                title: 'Confirm Intent To Delete Image',
                                message: 'Are you sure you want to delete this image?',
                                width: 400,
                                height: 150,
                                onYes: function() {
                                    Csw.ajaxWcf.post({
                                        urlMethod: cswPrivate.deleteUrl,
                                        data: {
                                            Blob: cswPrivate.selectedImg,
                                            propid: cswPrivate.propid
                                        },
                                        success: function(response) {
                                            cswPrivate.onDeleteImg(response);
                                            confirmDialog.close();
                                            editImageDialog.close();
                                        }
                                    });
                                },
                                onNo: function() {
                                    confirmDialog.close();
                                }
                            });
                        }
                    });
                }
            }
            makeBtns();

            var textArea = tbl.cell(2, 1).textArea({
                text: cswPrivate.selectedImg.Caption,
                rows: 3,
                cols: 45
            });

            var saveBtn = div.button({
                name: 'saveChangesBtn',
                enabledText: 'Save Changes',
                onClick: function () {
                    var newCaption = textArea.val();
                    cswPrivate.selectedImg.Caption = newCaption;
                    Csw.ajaxWcf.post({
                        urlMethod: cswPrivate.saveCaptionUrl,
                        data: {
                            Blob: cswPrivate.selectedImg
                        },
                        success: function () {
                            cswPrivate.onSave(newCaption, cswPrivate.selectedImg.BlobDataId);
                            editImageDialog.close();
                        }
                    });
                }
            });
            if (Csw.isNullOrEmpty(cswPrivate.selectedImg.BlobDataId)) {
                saveBtn.disable();
            }

            return editImageDialog;
        }());
    });
}());