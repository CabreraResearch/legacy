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

                        cswPrivate.href = Csw.hrefString(cswPrivate.propVals.href);
                        if (cswPrivate.href.length > 0) {
                            cswPrivate.href += '&usenodetypeasplaceholder=false'; // case 27596
                        }

                        if (false === Csw.isNullOrEmpty(cswPrivate.propVals.width) &&
                            Csw.isNumeric(cswPrivate.propVals.width)) {
                            cswPrivate.width = Math.abs(Csw.number(cswPrivate.propVals.width, 100) - 36);
                        }

                        cswPrivate.fileName = Csw.string(cswPrivate.propVals.name).trim();

                        cswPublic.control = cswPrivate.parent.table();
                        cswPrivate.cell11 = cswPublic.control.cell(1, 1).propDom('colspan', '3');
                        cswPrivate.cell21 = cswPublic.control.cell(2, 1).propDom('width', cswPrivate.width);
                        cswPrivate.cell22 = cswPublic.control.cell(2, 2).propDom({ align: 'right', width: '20px' }).div();
                        cswPrivate.cell23 = cswPublic.control.cell(2, 3).propDom({ align: 'right', width: '20px' }).div();

                        cswPrivate.makeClr = function () {
                            cswPrivate.cell23.empty();
                            if (false === Csw.isNullOrEmpty(cswPrivate.fileName)) {
                                //Clear button
                                cswPrivate.cell23.icon({
                                    name: 'clear',
                                    iconType: Csw.enums.iconType.trash,
                                    hovertext: 'Clear Image',
                                    size: 16,
                                    isButton: true,
                                    onClick: function () {
                                        /* remember: confirm is globally blocking call */
                                        if (confirm("Are you sure you want to clear this image?")) {
                                            var dataJson = {
                                                propid: cswPublic.data.propData.id,
                                                IncludeBlob: true
                                            };

                                            Csw.ajaxWcf.post({
                                                urlMethod: 'BlobData/clearBlob',
                                                data: dataJson,
                                                success: function () {
                                                    var val = {
                                                        href: '',
                                                        name: '',
                                                        contenttype: ''
                                                    };
                                                    cswPrivate.makeImg(null);
                                                    cswPublic.data.onPropChange(val);
                                                    Csw.publish(Csw.enums.events.main.refreshSelected, {});
                                                }
                                            });
                                        }
                                    }
                                }); // icon
                            } // if (false === Csw.isNullOrEmpty(fileName)) {
                        };

                        cswPrivate.makeImg = function (imgData) {
                            cswPrivate.cell11.empty();
                            cswPrivate.cell21.empty();
                            if (false == Csw.isNullOrEmpty(imgData)) {
                                cswPrivate.fileName = imgData.fileName;
                                cswPrivate.cell11.a({
                                    href: imgData.href,
                                    target: '_blank'
                                })
                                    .img({
                                        src: imgData.href,
                                        alt: imgData.fileName,
                                        height: cswPrivate.propVals.height,
                                        width: cswPrivate.propVals.width
                                    });
                                cswPrivate.cell21.a({
                                    href: imgData.href,
                                    target: '_blank',
                                    text: imgData.fileName
                                });
                            }
                            cswPrivate.makeClr();
                        };
                        cswPrivate.makeImg(cswPrivate);

                        if (false === cswPublic.data.isReadOnly()) {
                            //Clear button
                            cswPrivate.makeClr();

                            //Edit button
                            cswPrivate.cell22.icon({
                                name: 'edit',
                                iconType: Csw.enums.iconType.pencil,
                                hovertext: 'Edit',
                                size: 16,
                                isButton: true,
                                onClick: function () {
                                    $.CswDialog('FileUploadDialog', {
                                        urlMethod: 'Services/BlobData/SaveFile',
                                        params: {
                                            propid: cswPublic.data.propData.id
                                        },
                                        onSuccess: function (data) {
                                            var val = {
                                                href: data.Data.href,
                                                name: data.Data.filename,
                                                fileName: data.Data.filename,
                                                contenttype: data.Data.contenttype
                                            };
                                            if (data.Data.success) {
                                                cswPrivate.makeImg(val);
                                                cswPublic.data.onPropChange(val);
                                            }
                                        }
                                    });
                                }
                            }); // icon

                        } // if (false === o.ReadOnly && o.EditMode !== Csw.enums.editMode.Add) {
                    }


                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());
