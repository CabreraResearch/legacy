/// <reference path="~/app/CswApp-vsdoc.js" />
(function () { 

    Csw.dialogs.copynode = Csw.dialogs.copynode ||
        Csw.dialogs.register('copynode', function (cswPrivate) {
            'use strict';
            
            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.copyType = cswPrivate.copyType || '';
                cswPrivate.nodename = cswPrivate.nodename || '';
                cswPrivate.nodeid = cswPrivate.nodeid || '';
                cswPrivate.nodekey = cswPrivate.nodekey || '';
                cswPrivate.nodetypeid = cswPrivate.nodetypeid || '';
                cswPrivate.onCopyNode = cswPrivate.onCopyNode || function (){};
            }());
            
            cswPrivate.copyNodeDialog = (function () {
                'use strict';
                var copyDialog = Csw.layouts.dialog({
                    title: 'Confirm Copy',
                    width: 400,
                    height: 300
                });

                copyDialog.div.span({ text: 'Copying: ' + cswPrivate.nodename }).br({ number: 2 });
                var tbl = copyDialog.div.table({ name: 'CopyNodeDialogDiv_table' });
                var copyBtn = tbl.cell(1, 1).button({
                    name: 'copynode_submit',
                    enabledText: 'Copy',
                    disabledText: 'Copying',
                    onClick: function () {
                        Csw.ajax.post({
                            urlMethod: 'CopyNode',
                            data: {
                                NodeId: cswPrivate.nodeid,
                                NodeKey: Csw.string(cswPrivate.nodekey, cswPrivate.nodekey[0])
                            },
                            success: function (result) {
                                cswPrivate.onCopyNode(result.NewNodeId);
                                copyDialog.close();
                            },
                            error: function () {
                                copyBtn.enable();
                            }
                        });
                    }
                });
                tbl.cell(1, 2).button({
                    name: 'copynode_cancel',
                    enabledText: 'Cancel',
                    disabledText: 'Canceling',
                    onClick: function () {
                        copyDialog.close();
                    }
                });
                
                return copyDialog;
            }());

            (function _postCtor() {               
                Csw.ajaxWcf.post({
                    urlMethod: 'Quotas/check',
                    data: {
                        NodeTypeId: Csw.number(cswPrivate.nodetypeid, 0)
                    },
                    success: function (data) {
                        if (data && data.HasSpace) {
                            if (false === Csw.isNullOrEmpty(cswPrivate.copyType)) {
                                Csw.ajaxWcf.post({
                                    urlMethod: 'Nodes/getCopyData',
                                    data: {
                                        NodeId: cswPrivate.nodeid,
                                        CopyType: cswPrivate.copyType
                                    },
                                    success: function (data) {
                                        Csw.main.handleAction(data[cswPrivate.copyType]);
                                    }
                                });
                            } else {
                                cswPrivate.copyNodeDialog.open();
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
            }());          
            
            return cswPublic;
        });
} ());