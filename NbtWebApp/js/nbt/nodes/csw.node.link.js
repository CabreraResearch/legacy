/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function () {
    'use strict';

    Csw.controls.nodeLink = Csw.controls.nodeLink ||
        Csw.controls.register('nodeLink', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                ID: '',
                text: ''
            };
            var cswPublic = {};

            // Look for node references in the text
            cswPrivate.findNodeRef = function () {
                var startmarker = '[[';
                var midmarker = '][';
                var endmarker = ']]';
                var msg = cswPrivate.text;
                var startpos = msg.indexOf(startmarker);
                var endpos = msg.indexOf(endmarker);
                while (startpos > 0) {
                    cswParent.append(msg.substr(0, startpos));

                    var noderef = msg.substr(startpos, endpos - startpos);
                    var midpos = noderef.indexOf(midmarker);
                    var nodeid = noderef.substr(startmarker.length, midpos - startmarker.length);
                    var nodename = noderef.substr(midpos + midmarker.length, noderef.length - (midpos + midmarker.length));

                    cswPrivate.makeNodeLink(nodeid, nodename);

                    msg = msg.substr(endpos + endmarker.length, msg.length - (endpos + endmarker.length));
                    startpos = msg.indexOf(startmarker);
                    endpos = msg.indexOf(endmarker);
                } // while (startpos > 0)
                cswParent.append(msg);
            }; // findNodeRef()

            cswPrivate.makeNodeLink = function (nodeid, nodename) {
                cswParent.a({
                    ID: Csw.makeId(cswPrivate.ID, nodeid),
                    text: nodename,
                    onClick: function () {
                        $.CswDialog('EditNodeDialog', {
                            nodeids: [nodeid],
                            nodenames: [nodename]
                        }); // CswDialog
                    } // onClick
                }); // link
            }; // makeNodeLink()

            (function () {
                if (options) $.extend(cswPrivate, options);

                cswPrivate.findNodeRef();

            } ());

            return cswPublic;
        });
} ());

