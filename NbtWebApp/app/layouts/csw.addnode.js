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
                                $.CswDialog('AddNodeDialog', cswPrivate.dialogOptions);
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

