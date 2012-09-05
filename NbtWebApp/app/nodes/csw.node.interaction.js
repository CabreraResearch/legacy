
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    var copyNode = function (options) {
        var o = {
            nodeid: '',
            nodekey: '',
            onSuccess: function () {
            },
            onError: function () {
            }
        };
        if (options) {
            Csw.extend(o, options);
        }

        var dataJson = {
            NodePk: o.nodeid
        };

        Csw.ajax.post({
            url: '/NbtWebApp/wsNBT.asmx/CopyNode',
            data: {
                NodeId: Csw.string(o.nodeid),
                NodeKey: Csw.string(o.nodekey)
            },
            success: function (result) {
                o.onSuccess(result.NewNodeId, '');
            },
            error: o.onError
        });
    };
    Csw.register('copyNode', copyNode);
    Csw.copyNode = Csw.copyNode || copyNode;

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

        Csw.ajax.post({
            url: '/NbtWebApp/wsNBT.asmx/DeleteNodes',
            data: jData,
            success: function (data) {
                /* clear selected node cookies */
                o.nodeid = Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeId);
                o.cswnbtnodekey = Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeKey);
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
    Csw.deleteNodes = Csw.deleteNodes || deleteNodes;


} ());
