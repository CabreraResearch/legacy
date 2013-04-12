/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.file = Csw.properties.file ||
        Csw.properties.register('file',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };
                
                //The render function to be executed as a callback
                var render = function () {
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPublic.control = cswPrivate.parent.table();

                    if (cswPublic.data.isMulti()) {
                        cswPublic.control.cell(1, 1).append('[File display disabled]');
                    } else {

                        cswPrivate.href = Csw.hrefString(cswPrivate.propVals.href);
                        cswPrivate.fileName = Csw.string(cswPrivate.propVals.name).trim();

                        cswPrivate.fileCell = cswPublic.control.cell(1, 1);
                        cswPrivate.fileCell.a({ href: cswPrivate.href, target: '_blank', text: cswPrivate.fileName });

                        cswPrivate.cell12 = cswPublic.control.cell(1, 2).div();
                        cswPrivate.cell13 = cswPublic.control.cell(1, 3).div();

                        if (false === cswPublic.data.isReadOnly()) {
                            //Edit button
                            cswPrivate.cell12.icon({
                                name: cswPublic.data.name + '_edit',
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
                                                contenttype: data.Data.contenttype
                                            };
                                            if (data.Data.success) {
                                                cswPrivate.fileCell.empty();
                                                cswPrivate.fileCell.a({ href: val.href, target: '_blank', text: val.name });
                                                cswPublic.data.onPropChange(val);
                                            }
                                        }
                                    });
                                }
                            });
                            //Clear button
                            cswPrivate.cell13.icon({
                                name: cswPublic.data.name + '_clr',
                                iconType: Csw.enums.iconType.trash,
                                hovertext: 'Clear File',
                                size: 16,
                                isButton: true,
                                onClick: function () {
                                    /* remember: confirm is globally blocking call */
                                    if (confirm("Are you sure you want to clear this file?")) {
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
                                                cswPublic.data.onPropChange(val);
                                                cswPublic.data.onReload();
                                            }
                                        });
                                    }
                                }
                            });
                        }
                    }


                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());
