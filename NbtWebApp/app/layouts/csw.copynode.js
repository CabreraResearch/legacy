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
                cswPrivate.nodetypeid = cswPrivate.nodetypeid || '';
                cswPrivate.onCopyNode = cswPrivate.onCopyNode || function (){};
            }());

            (function _postCtor() {               
                Csw.ajaxWcf.post({
                    urlMethod: 'Quotas/check',
                    data: {
                        NodeTypeId: Csw.number(cswPrivate.nodetypeid, 0)
                    },
                    success: function (data) {
                        if (data && data.HasSpace) {
                            $.CswDialog('CopyNodeDialog', {
                                nodename: cswPrivate.nodename,
                                nodeid: cswPrivate.nodeid,
                                nodetypeid: cswPrivate.nodetypeid,
                                onCopyNode: cswPrivate.onCopyNode
                            });
                        } else {
                            $.CswDialog('AlertDialog', data.Message, 'Quota Exceeded', null, 140, 450);
                        }
                    }
                });
            } ());
            
            return cswPublic;
        });
} ());