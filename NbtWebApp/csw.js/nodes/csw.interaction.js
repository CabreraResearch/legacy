/// <reference path="~/csw.js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {
    'use strict';

    var copyNode = function (options) {
        var o = {
            'nodeid': '',
            'nodekey': '',
            'onSuccess': function () {
            },
            'onError': function () {
            }
        };
        if (options) {
            $.extend(o, options);
        }

        var dataJson = {
            NodePk: o.nodeid
        };

        Csw.ajax({
            url: '/NbtWebApp/wsNBT.asmx/CopyNode',
            data: dataJson,
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
            'nodeids': [],
            'nodekeys': [],
            'onSuccess': function () {
            },
            'onError': function () {
            }
        };
        if (options) {
            $.extend(o, options);
        }

        if (!isArray(o.nodeids))  // case 22722
        {
            o.nodeids = [o.nodeids];
            o.nodekeys = [o.nodekeys];
        }

        var jData = {
            NodePks: o.nodeids,
            NodeKeys: o.nodekeys
        };

        Csw.ajax({
            url: '/NbtWebApp/wsNBT.asmx/DeleteNodes',
            data: jData,
            success: function () {
                // clear selected node cookies
                o.nodeid = $.CswCookie('clear', CswCookieName.CurrentNodeId);
                o.cswnbtnodekey = $.CswCookie('clear', CswCookieName.CurrentNodeKey);
                // returning '' will reselect the first node in the tree
                o.onSuccess('', '');
            },
            error: o.onError
        });
    };
    Csw.register('deleteNodes', deleteNodes);
    Csw.deleteNodes = Csw.deleteNodes || deleteNodes;

}());