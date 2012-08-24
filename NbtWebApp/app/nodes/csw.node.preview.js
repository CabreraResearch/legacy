
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';

    var $nodepreview;
    var nodeHoverIn = function (event, nodeid, cswnbtnodekey, delay) {
        var previewopts = {
            ID: nodeid + '_preview',
            nodeid: nodeid,
            cswnbtnodekey: cswnbtnodekey,
            eventArg: event
        };
        if (Csw.number(delay, -1) >= 0) {
            previewopts.delay = delay;
        }
        $nodepreview = $.CswNodePreview('open', previewopts);
    };
    Csw.register('nodeHoverIn', nodeHoverIn);
    Csw.nodeHoverIn = Csw.nodeHoverIn || nodeHoverIn;

    var nodeHoverOut = function () {
        if ($nodepreview !== undefined) {
            $nodepreview.CswNodePreview('close');
            $nodepreview = undefined;
        }
    };
    Csw.register('nodeHoverOut', nodeHoverOut);
    Csw.nodeHoverOut = Csw.nodeHoverOut || nodeHoverOut;

} ());
