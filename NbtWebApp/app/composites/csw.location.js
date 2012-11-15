
/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function () {
    'use strict';

    Csw.composites.location = Csw.composites.location ||
        Csw.composites.register('location', function (cswParent, options) {
            //#region variables
            var cswPrivate = {
                name: 'csw_location_tree',
                locationobjectclassid: '',
                locationnodetypeids: [],
                relatedmatch: false,
                relatedobjectclassid: '',
                nodeid: '',
                viewid: '',
                path: '',
                nodeKey: '',
                Multi: false,
                ReadOnly: false,
                onChange: null,
                value: ''
            };

            var cswPublic = {};
            //#endregion variables

            //#region safety net
            Csw.tryExec(function () {

                //#region init ctor
                (function _pre() {
                    Csw.extend(cswPrivate, options, true);
                    if (Csw.isNullOrEmpty(cswPrivate.viewid)) {
                        Csw.ajax.post({
                            urlMethod: 'getLocationTree',
                            async: false,
                            data: {
                                NodeId: Csw.string(cswPrivate.nodeid)
                            },
                            success: function (data) {
                                Csw.extend(cswPrivate, data, true);
                            }
                        });
                    }
                    cswParent.empty();

                    cswPrivate.relatedmatch = (cswPrivate.relatedobjectclassid === cswPrivate.locationobjectclassid);
                    Csw.each(cswPrivate.locationnodetypeids, function (thisNTid) {
                        cswPrivate.relatedmatch = (cswPrivate.relatedmatch || thisNTid === cswPrivate.relatednodetypeid);
                    });

                    if (cswPrivate.relatedmatch) {
                        cswPrivate.nodeid = Csw.string(cswPrivate.nodeid, cswPrivate.relatednodeid).trim();
                        cswPrivate.name = Csw.string(cswPrivate.name, cswPrivate.relatednodename).trim();
                        cswPrivate.path = Csw.string(cswPrivate.path, cswPrivate.relatednodename).trim();
                    } else {
                        cswPrivate.nodeid = Csw.string(cswPrivate.nodeid, '').trim();
                        cswPrivate.name = Csw.string(cswPrivate.name, '').trim();
                        cswPrivate.path = Csw.string(cswPrivate.path, '').trim();
                    }
                    cswPrivate.viewid = Csw.string(cswPrivate.viewid).trim();
                    cswPrivate.value = cswPrivate.nodeid;

                    cswPublic.table = cswParent.table();

                    cswPublic.table.cell(1, 1).text(cswPrivate.path);

                    cswPrivate.selectDiv = cswPublic.table.cell(2, 1).div({
                        cssclass: 'locationselect',
                        value: cswPrivate.nodeid
                    });

                    cswPublic.table.cell(2, 1).hide();

                } ());
                //#endregion init ctor

                //#region cswPrivate/cswPublic methods and props

                cswPublic.val = function () {
                    return cswPrivate.value;
                };

                cswPublic.selectedName = function() {
                    return cswPrivate.name;
                };

                cswPrivate.onTreeSelect = function (optSelect) {
                    optSelect = optSelect || { nodeid: '', nodename: '', iconurl: '' };
                    if (optSelect.nodeid === 'root' || optSelect.nodeid === undefined) {
                        optSelect.nodeid = ''; // case 21046   
                    }
                    cswPublic.comboBox.topContent(optSelect.nodename, optSelect.nodeid);
                    if (cswPublic.comboBox.val() !== optSelect.nodeid) {
                        cswPublic.comboBox.val(optSelect.nodeid);
                    }
                    Csw.tryExec(cswPrivate.onChange, optSelect.nodeid, optSelect.nodename);
                    cswPrivate.value = optSelect.nodeid;
                    cswPrivate.name = optSelect.nodename;
                    Csw.defer(function () { cswPublic.comboBox.close(); }, 100);
                };

                //#endregion cswPrivate/cswPublic methods and props

                //#region final ctor
                (function _post() {
                    if (false === cswPrivate.ReadOnly) {
                        cswPublic.table.cell(1, 2).icon({
                            name: cswPrivate.name + '_toggle',
                            iconType: Csw.enums.iconType.pencil,
                            hovertext: 'Edit',
                            size: 16,
                            isButton: true,
                            onClick: function () {

                                cswPublic.table.cell(1, 1).hide();
                                cswPublic.table.cell(1, 2).hide();
                                cswPublic.table.cell(2, 1).show();

                                cswPublic.comboBox = cswPrivate.selectDiv.comboBox({
                                    name: cswPrivate.name + '_combo',
                                    topContent: cswPrivate.name,
                                    selectContent: '',
                                    width: '290px',
                                    onClick: (function () {
                                        var first = true;
                                        return function () {
                                            if (first) {      // only do this once
                                                cswPublic.locationTree.expandAll();
                                                first = false;
                                            }
                                            cswPublic.comboBox.open(); // ensure we're open on click
                                            return false;    // but only close when onTreeSelect fires, below
                                        };
                                    })()
                                });

                                cswPublic.locationTree = Csw.nbt.nodeTree({
                                    name: cswPrivate.name,
                                    parent: cswPublic.comboBox.pickList,
                                    onInitialSelectNode: function (optSelect) {
                                        cswPrivate.onTreeSelect(optSelect);
                                    },
                                    onSelectNode: function (optSelect) {
                                        cswPrivate.onTreeSelect(optSelect);
                                    },
                                    UseScrollbars: false,
                                    ShowToggleLink: false
                                });

                                cswPublic.locationTree.init({
                                    viewid: cswPrivate.viewid,
                                    nodeid: cswPrivate.nodeid,
                                    nodekey: cswPrivate.nodeKey,
                                    IncludeInQuickLaunch: false,
                                    DefaultSelect: Csw.enums.nodeTree_DefaultSelect.root.name
                                });
                            } // onClick
                        }); // imageButton

                        cswParent.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectDiv.propNonDom('value')); },
                                          function (event) { Csw.nodeHoverOut(event, cswPrivate.selectDiv.propNonDom('value')); });
                    }
                } ());
                //#region final ctor

            });
            //#endregion safety net

            return cswPublic;

        });


} ());


