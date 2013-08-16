/// <reference path="~/app/CswApp-vsdoc.js" />
(function () { 

    Csw.layouts.copynode = Csw.layouts.copynode ||
        Csw.layouts.register('copynode', function (cswPrivate) {
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
                        Csw.copyNode({
                            nodeid: cswPrivate.nodeid,
                            nodekey: Csw.string(cswPrivate.nodekey, cswPrivate.nodekey[0]),
                            onSuccess: function (nodeid, nodekey) {
                                cswPrivate.onCopyNode(nodeid, nodekey);
                                copyDialog.close();
                            },
                            onError: function () {
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
                            //TODO - if copyType is not null, make a call to getCopyData and execute main.handleAction - else:
                            cswPrivate.copyNodeDialog.open();
                        } else {
                            $.CswDialog('AlertDialog', data.Message, 'Quota Exceeded', null, 140, 450);
                        }
                    }
                });
            }());          
            
            return cswPublic;
        });
} ());