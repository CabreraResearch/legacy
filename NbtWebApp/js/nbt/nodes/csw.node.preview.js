/// <reference path="~/js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function() {
    'use strict';

    var $nodepreview;
    var nodeHoverIn = function(event, nodeid, cswnbtnodekey) {
        $nodepreview = $.CswNodePreview('open', {
            ID: nodeid + '_preview',
            nodeid: nodeid,
            cswnbtnodekey: cswnbtnodekey,
            eventArg: event
        });
    };
    Csw.register('nodeHoverIn', nodeHoverIn);
    Csw.nodeHoverIn = Csw.nodeHoverIn || nodeHoverIn;

    var nodeHoverOut = function() {
        if ($nodepreview !== undefined) {
            $nodepreview.CswNodePreview('close');
            $nodepreview = undefined;
        }
    };
    Csw.register('nodeHoverOut', nodeHoverOut);
    Csw.nodeHoverOut = Csw.nodeHoverOut || nodeHoverOut;

}());
