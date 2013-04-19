/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.file = Csw.properties.register('file',
        function(nodeProperty) {
            'use strict';

            //The render function to be executed as a callback
            var render = function() {
                var cswPrivate = Csw.object();

                var table = nodeProperty.propDiv.table();

                if (nodeProperty.isMulti()) {
                    table.cell(1, 1).append('[File display disabled]');
                } else {

                    cswPrivate.fileCell = table.cell(1, 1);
                    cswPrivate.cell12 = table.cell(1, 2).div();
                    cswPrivate.cell13 = table.cell(1, 3).div();
                    
                    var doUpdate = function (val) {
                        cswPrivate.href = val.href;
                        cswPrivate.fileName = val.name;

                        cswPrivate.fileCell.empty();
                        cswPrivate.fileCell.a({ href: Csw.hrefString(val.href), target: '_blank', text: val.name });
                    };
                    doUpdate(nodeProperty.propData.values);
                    
                    nodeProperty.onPropChangeBroadcast(function (val) {
                        if (val.name !== cswPrivate.fileName) {
                            doUpdate(val);
                        }
                    });

                    if (false === nodeProperty.isReadOnly()) {
                        
                        var onChange = function(val) {
                            nodeProperty.propData.values.name = val.name;
                            nodeProperty.propData.values.href = val.href;
                            nodeProperty.propData.values.contenttype = val.contenttype;
                            doUpdate(val);
                            
                            nodeProperty.onPropChangeBroadcast(val);
                        };
                        
                        //Edit button
                        cswPrivate.cell12.icon({
                            name: nodeProperty.name + '_edit',
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
                                            onChange({
                                                href: data.Data.href,
                                                name: data.Data.filename,
                                                contenttype: data.Data.contenttype
                                            });
                                            //nodeProperty.onPropChange(val);
                                        }
                                    }
                                });
                            }
                        });
                        //Clear button
                        cswPrivate.cell13.icon({
                            name: nodeProperty.name + '_clr',
                            iconType: Csw.enums.iconType.trash,
                            hovertext: 'Clear File',
                            size: 16,
                            isButton: true,
                            onClick: function() {
                                /* remember: confirm is globally blocking call */
                                if (confirm("Are you sure you want to clear this file?")) {
                                    var dataJson = {
                                        propid: nodeProperty.propData.id,
                                        IncludeBlob: true
                                    };

                                    Csw.ajaxWcf.post({
                                        urlMethod: 'BlobData/clearBlob',
                                        data: dataJson,
                                        success: function() {
                                            onChange({
                                                href: '',
                                                name: '',
                                                contenttype: ''
                                            });
                                            //nodeProperty.onPropChange(val);
                                            nodeProperty.onReload();
                                        }
                                    });
                                }
                            }
                        });
                    }
                }


            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender(function() {

            return true;
        });

} ());
