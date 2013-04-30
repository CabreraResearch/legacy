/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.image = Csw.properties.register('image',
        function(nodeProperty) {
            'use strict';

            //The render function to be executed as a callback
            var render = function() {
                'use strict';

                var cswPrivate = Csw.object();
                
                if (nodeProperty.isMulti()) {
                    nodeProperty.propDiv.append('[Image display disabled]');
                } else {

                    cswPrivate.href = nodeProperty.propData.values.href;
                    cswPrivate.fileName = nodeProperty.propData.values.name;
                    
                    var width = 100;
                    if (nodeProperty.propData.values.width > 0) {
                        width = Math.abs(Csw.number(nodeProperty.propData.values.width, 100) - 36);
                    } 
                    var height = 100;
                    if (nodeProperty.propData.values.height > 0) {
                        height = nodeProperty.propData.values.height;
                    }
                    
                    var table = nodeProperty.propDiv.table();
                    cswPrivate.cell11 = table.cell(1, 1).propDom('colspan', '3');
                    cswPrivate.cell21 = table.cell(2, 1).propDom('width', width);
                    cswPrivate.cell22 = table.cell(2, 2).propDom({ align: 'right', width: '20px' }).div();
                    cswPrivate.cell23 = table.cell(2, 3).propDom({ align: 'right', width: '20px' }).div();

                    nodeProperty.onPropChangeBroadcast(function (val) {
                        if (cswPrivate.fileName !== val.name || cswPrivate.href != val.href) {
                            updateProp(val);
                        }
                    });

                    var updateProp = function(val) {
                        cswPrivate.fileName = val.name;
                        cswPrivate.href = val.href;

                        nodeProperty.propData.values.href = val.href;
                        nodeProperty.propData.values.fileName = val.name;

                        makeImg(val);
                    };

                    var broadcastUpdate = function(val) {
                        updateProp(val);
                        nodeProperty.broadcastPropChange(val);
                    };

                    var makeClr = function() {
                        cswPrivate.cell23.empty();
                        if (cswPrivate.fileName) {
                            //Clear button
                            cswPrivate.cell23.icon({
                                name: 'clear',
                                iconType: Csw.enums.iconType.trash,
                                hovertext: 'Clear Image',
                                size: 16,
                                isButton: true,
                                onClick: function() {
                                    /* remember: confirm is globally blocking call */
                                    if (confirm("Are you sure you want to clear this image?")) {
                                        
                                        Csw.ajaxWcf.post({
                                            urlMethod: 'BlobData/clearBlob',
                                            data: {
                                                propid: nodeProperty.propData.id,
                                                IncludeBlob: true
                                            },
                                            success: function() {
                                                broadcastUpdate({
                                                    href: nodeProperty.propData.values.placeholder,
                                                    name: '',
                                                    contenttype: ''
                                                });
                                            }
                                        });
                                    }
                                }
                            }); // icon
                        } // if (false === Csw.isNullOrEmpty(fileName)) {
                    };

                    var makeImg = function(val) {
                        cswPrivate.cell11.empty();
                        cswPrivate.cell21.empty();
                        if (val) {
                            var href = val.href;
                            if (href !== nodeProperty.propData.values.placeholder) {
                                href = Csw.hrefString(val.href);
                            }

                            cswPrivate.fileName = val.fileName;
                            cswPrivate.cell11.a({
                                href: val.href,
                                target: '_blank'
                            })
                                .img({
                                    src: href,
                                    alt: val.fileName,
                                    height: height,
                                    width: width
                                });
                            cswPrivate.cell21.a({
                                href: href,
                                target: '_blank',
                                text: val.fileName
                            });
                        }
                        makeClr();
                    };

                    makeImg(nodeProperty.propData.values);

                    if (false === nodeProperty.isReadOnly()) {
                        //Clear button
                        makeClr();

                        //Edit button
                        cswPrivate.cell22.icon({
                            name: 'edit',
                            iconType: Csw.enums.iconType.pencil,
                            hovertext: 'Edit',
                            size: 16,
                            isButton: true,
                            onClick: function() {
                                $.CswDialog('FileUploadDialog', {
                                    urlMethod: 'Services/BlobData/SaveFile',
                                    params: {
                                        propid: nodeProperty.propData.id
                                    },
                                    onSuccess: function(data) {
                                        if (data.Data.success) {
                                            broadcastUpdate({
                                                href: data.Data.href,
                                                name: data.Data.filename,
                                                fileName: data.Data.filename,
                                                contenttype: data.Data.contenttype
                                            });
                                        }
                                    }
                                });
                            }
                        }); // icon
                    } // if (false === o.ReadOnly && o.EditMode !== Csw.enums.editMode.Add) {
                }
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender(function() {
              
            return true;
        });

} ());
