/// <reference path="~/js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function() {
    'use strict';

    var copyNode = function(options) {
        var o = {
            'nodeid': '',
            'nodekey': '',
            'onSuccess': function() {
            },
            'onError': function() {
            }
        };
        if (options) {
            $.extend(o, options);
        }

        var dataJson = {
            NodePk: o.nodeid
        };

        Csw.ajax.post({
            url: '/NbtWebApp/wsNBT.asmx/CopyNode',
            data: dataJson,
            success: function(result) {
                o.onSuccess(result.NewNodeId, '');
            },
            error: o.onError
        });
    };
    Csw.register('copyNode', copyNode);
    Csw.copyNode = Csw.copyNode || copyNode;

    var deleteNodes = function(options) {
        var o = {
            nodeids: Csw.array(),
            nodekeys: Csw.array(),
            onSuccess: null,
            onError: null
        };
        if (options) {
            $.extend(o, options);
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
            success: function() {
                /* clear selected node cookies */
                o.nodeid = Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeId);
                o.cswnbtnodekey = Csw.cookie.clear(Csw.cookie.cookieNames.CurrentNodeKey);
                /* returning '' will reselect the first node in the tree */
                Csw.tryExec(o.onSuccess, '', '');
            },
            error: o.onError
        });
    };
    Csw.register('deleteNodes', deleteNodes);
    Csw.deleteNodes = Csw.deleteNodes || deleteNodes;


}());
