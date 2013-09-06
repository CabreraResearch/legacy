/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.addnode = Csw.dialogs.addnode ||
        Csw.dialogs.register('addnode', function (cswPrivate) {
            'use strict';

            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.name = cswPrivate.name || 'AddNode';
                cswPrivate.action = cswPrivate.action || 'AddNode';
                cswPrivate.title = cswPrivate.title || 'Add';
                cswPrivate.nodeid = cswPrivate.nodeid || '';
                cswPrivate.nodetypeid = cswPrivate.nodetypeid || '';
                cswPrivate.objectClassId = cswPrivate.objectClassId || '';
                cswPrivate.relatednodeid = cswPrivate.relatednodeid || '';
                cswPrivate.relatednodename = cswPrivate.relatednodename || '';
                cswPrivate.propertyData = cswPrivate.propertyData || {};
                cswPrivate.onAddNode = cswPrivate.onAddNode || function () { };
                cswPrivate.onSaveImmediate = cswPrivate.onSaveImmediate || function () { };
            }());

            cswPrivate.addNodeDialog = (function () {
                'use strict';

                var addDialog = Csw.layouts.dialog({
                    title: cswPrivate.title,
                    width: 800,
                    height: 600,
                    onOpen: function () {
                        if (cswPrivate.propertyData && cswPrivate.propertyData.node) {
                            cswPrivate.nodeid = cswPrivate.propertyData.node.nodeid;
                        }
                        cswPublic.tabsAndProps = Csw.layouts.tabsAndProps(addDialog.div, {
                            name: 'tabsAndProps',
                            tabState: {
                                propertyData: cswPrivate.propertyData,
                                ShowAsReport: false,
                                nodeid: cswPrivate.nodeid,
                                nodetypeid: cswPrivate.nodetypeid,
                                objectClassId: cswPrivate.objectClassId,
                                relatednodeid: cswPrivate.relatednodeid,
                                EditMode: Csw.enums.editMode.Add
                            },
                            ReloadTabOnSave: false,
                            onSave: function (nodeid, nodekey, tabcount, nodename, nodelink) {
                                cswPublic.tabsAndProps.tearDown();
                                if (nodeid || nodekey) {
                                    Csw.tryExec(cswPrivate.onAddNode, nodeid, nodekey, nodename, nodelink);
                                }
                                Csw.tryExec(cswPrivate.onSaveImmediate);
                                addDialog.close();
                            },
                            checkQuota: false //Case 29531 - quota has already been checked by layouts.addnode
                        });
                    }
                });

                return addDialog;
            }());

            (function _postCtor() {
                if (Csw.clientChanges.manuallyCheckChanges()) {
                    Csw.ajaxWcf.post({
                        urlMethod: 'Quotas/check',
                        data: {
                            NodeTypeId: Csw.number(cswPrivate.nodetypeid, 0)
                        },
                        success: function(data) {
                            if (data && data.HasSpace) {
                                if (false === Csw.isNullOrEmpty(cswPrivate.action) && cswPrivate.action !== 'AddNode') {
                                    Csw.main.handleAction({ actionname: cswPrivate.action });
                                } else if (Csw.isNullOrEmpty(cswPrivate.propertyData)) {
                                    Csw.ajax.deprecatedWsNbt({
                                        urlMethod: 'getProps',
                                        data: {
                                            EditMode: Csw.enums.editMode.Add,
                                            TabId: 'Add_tab',
                                            NodeTypeId: Csw.string(cswPrivate.nodetypeid),
                                            Date: new Date().toDateString(),
                                            RelatedNodeId: Csw.string(cswPrivate.relatednodeid),
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
                                                cswPrivate.onAddNode(propdata.node.nodeid, null, propdata.node.nodename, propdata.node.nodelink);
                                            } else {
                                                cswPrivate.propertyData = propdata;
                                                cswPrivate.addNodeDialog.open();
                                            }
                                        }
                                    });
                                } else {
                                    cswPrivate.addNodeDialog.open();
                                }
                            } else {
                                Csw.dialogs.alert({
                                    title: 'Quota Exceeded',
                                    width: 450,
                                    height: 160,
                                    message: data.Message
                                }).open();
                            }
                        }
                    });
                }
            }());

            return cswPublic;
        });
}());