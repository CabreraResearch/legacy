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
                var render = function () {
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPublic.control = cswPrivate.parent.table();

                    if (cswPublic.data.isMulti()) {
                        cswPublic.control.cell(1, 1).append(Csw.enums.multiEditDefaultValue);
                    } else {

                        cswPrivate.href = Csw.string(cswPrivate.propVals.href).trim();
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
                                        urlMethod: 'fileForProp',
                                        params: {
                                            PropId: cswPublic.data.propData.id
                                        },
                                        onSuccess: function (data) {
                                            var val = {
                                                href: data.href,
                                                name: data.filename,
                                                contenttype: data.contenttype
                                            };
                                            if (data.success) {
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
                                            PropId: cswPublic.data.propData.id,
                                            IncludeBlob: true
                                        };

                                        Csw.ajax.post({
                                            urlMethod: 'clearProp',
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

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

} ());
