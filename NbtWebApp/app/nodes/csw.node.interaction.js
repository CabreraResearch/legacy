
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    var deleteNodes = function (options) {
        var o = {
            nodeids: Csw.array(),
            nodekeys: Csw.array(),
            onSuccess: null,
            onError: null
        };
        if (options) {
            Csw.extend(o, options);
        }

        if (false === Csw.isArray(o.nodeids)) {  /* case 22722 */
            o.nodeids = Csw.array(o.nodeids);
            o.nodekeys = Csw.array(o.nodekeys);
        }

        var jData = {
            NodePks: o.nodeids,
            NodeKeys: o.nodekeys
        };

        Csw.ajax.deprecatedWsNbt({
            urlMethod: 'DeleteNodes',
            data: jData,
            success: function (data) {
                /* clear selected node cookies */
                o.nodeid = Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeId);
                o.nodekey = Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeKey);
                /* returning '' will reselect the first node in the tree */
                Csw.tryExec(o.onSuccess, '', '');
                if (false === Csw.isNullOrEmpty(data.batch)) {
                    $.CswDialog('BatchOpDialog', {
                        opname: 'multi-delete',
                        onViewBatchOperation: function () {
                            Csw.publish(Csw.enums.events.main.refresh, {
                                nodeid: data.batch,
                                viewid: '',
                                viewmode: 'tree',
                                IncludeNodeRequired: true
                            });
                        }
                    });
                }
            },
            error: o.onError
        });
    };
    Csw.register('deleteNodes', deleteNodes);

}());
