/// <reference path="~/app/CswApp-vsdoc.js" />
(function () { 

    Csw.layouts.addnode = Csw.layouts.addnode ||
        Csw.layouts.register('addnode', function (cswPrivate) {
            'use strict';
            
            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.name = cswPrivate.name || 'AddNode';
                cswPrivate.action = cswPrivate.action || 'AddNode';
                cswPrivate.dialogOptions = cswPrivate.dialogOptions || {};
                cswPrivate.nodetypeid = cswPrivate.nodetypeid || cswPrivate.dialogOptions.nodetypeid;
            }());

            (function _postCtor() {               
                Csw.ajaxWcf.post({
                    watchGlobal: cswPrivate.AjaxWatchGlobal,
                    urlMethod: 'Quotas/check',
                    data: {
                        NodeTypeId: Csw.number(cswPrivate.nodetypeid, 0)
                    },
                    success: function (data) {
                        if (data && data.HasSpace) {
                            if (false === Csw.isNullOrEmpty(cswPrivate.action) && cswPrivate.action !== 'AddNode') {
                                Csw.main.handleAction({ actionname: cswPrivate.action });
                            } else {
                                Csw.ajax.post({
                                    urlMethod: 'getProps',
                                    data: {
                                        EditMode: Csw.enums.editMode.Add,
                                        TabId: 'Add_tab',
                                        NodeTypeId: Csw.string(cswPrivate.nodetypeid),
                                        Date: new Date().toDateString(),
                                        RelatedNodeId: Csw.string(cswPrivate.dialogOptions.relatednodeid),
                                        RelatedNodeTypeId: Csw.string(cswPrivate.dialogOptions.relatednodetypeid),
                                        RelatedObjectClassId: Csw.string(cswPrivate.dialogOptions.relatedobjectclassid),
                                        NodeId: 'newnode',
                                        SafeNodeKey: Csw.string(''),
                                        Multi: false,
                                        filterToPropId: Csw.string(''),
                                        ConfigMode: false,
                                        GetIdentityTab: false,
                                        ForceReadOnly: false
                                    },
                                    success: function(propdata) {
                                        if (Csw.isNullOrEmpty(propdata.properties)) {
                                            cswPrivate.dialogOptions.onAddNode(propdata.node.nodeid, null, propdata.node.nodename, propdata.node.nodelink);
                                        } else {
                                            cswPrivate.dialogOptions.propertyData = propdata;
                                            $.CswDialog('AddNodeDialog', cswPrivate.dialogOptions);
                                        }
                                    }
                                });
                            }
                        } else {
                            $.CswDialog('AlertDialog', data.Message, 'Quota Exceeded', null, 140, 450);
                        }
                    }
                });
            } ());
            
            return cswPublic;
        });
} ());