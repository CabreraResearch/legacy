
/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/js/thirdparty/extjs-4.1.0/ext-all-debug.js" />

(function() {
    'use strict';

    Csw.composites.location = Csw.composites.location ||
        Csw.composites.register('location', function (cswParent, options) {
            //#region variables
            var cswPrivate = {
                ID: 'csw_location_tree',
                locationobjectclassid: '',
                locationnodetypeids: [],
                relatedmatch: false,
                relatedobjectclassid: '',
                nodeId: '',
                viewId: '',
                name: '',
                path: '',
                nodeKey: '',
                Multi: false,
                ReadOnly: false
            };

            var cswPublic = { };
            //#endregion variables

            //#region safety net
            Csw.tryExec(function() {

                //#region init ctor
                (function _pre() {
                    Csw.extend(cswPrivate, options, true);
                    cswParent.empty();
                    
                    cswPrivate.relatedmatch = (cswPrivate.relatedobjectclassid === cswPrivate.locationobjectclassid);
                    Csw.each(cswPrivate.locationnodetypeids, function (thisNTid) {
                        cswPrivate.relatedmatch = (cswPrivate.relatedmatch || thisNTid === cswPrivate.relatednodetypeid);
                    });

                    if (cswPrivate.relatedmatch) {
                        cswPrivate.nodeId = (false === cswPrivate.Multi) ? Csw.string(cswPrivate.nodeId, cswPrivate.relatednodeid).trim() : '';
                        cswPrivate.name = (false === cswPrivate.Multi) ? Csw.string(cswPrivate.name, cswPrivate.relatednodename).trim() : Csw.enums.multiEditDefaultValue;
                        cswPrivate.path = (false === cswPrivate.Multi) ? Csw.string(cswPrivate.path, cswPrivate.relatednodename).trim() : Csw.enums.multiEditDefaultValue;
                    } else {
                        cswPrivate.nodeId = (false === cswPrivate.Multi) ? Csw.string(cswPrivate.nodeId, '').trim() : '';
                        cswPrivate.name = (false === cswPrivate.Multi) ? Csw.string(cswPrivate.name, '').trim() : Csw.enums.multiEditDefaultValue;
                        cswPrivate.path = (false === cswPrivate.Multi) ? Csw.string(cswPrivate.path, '').trim() : Csw.enums.multiEditDefaultValue;
                    }
                    cswPrivate.viewId = Csw.string(cswPrivate.viewId).trim();
                    
                    cswPublic.table = cswParent.table({
                        ID: Csw.makeId(cswPrivate.ID, 'tbl')
                    });

                    cswPublic.table.cell(1, 1).text(cswPrivate.path);

                    cswPrivate.selectDiv = cswPublic.table.cell(2, 1).div({
                        cssclass: 'locationselect',
                        value: cswPrivate.nodeId
                    });

                    cswPublic.table.cell(2, 1).hide();
                    
                }());
                //#endregion init ctor
                
                //#region cswPrivate/cswPublic methods and props

                cswPublic.val = function() {
                    var ret = '';
                    if (false === Csw.isNullOrEmpty(cswPrivate.selectDiv)) {
                        ret = cswPrivate.selectDiv.propNonDom('value');
                    }
                    return ret;
                };


                cswPrivate.onTreeSelect = function(itemid, text, onChange) {
                    if (itemid === 'root' || itemid === undefined) {
                        itemid = ''; // case 21046
                    }
                    cswPublic.comboBox.topContent(text, itemid);
                    if (cswPublic.comboBox.val() !== itemid) {
                        cswPublic.comboBox.val(itemid);
                        onChange();
                    }
                    cswPrivate.selectDiv.propNonDom('value', itemid);
                    setTimeout(function () { cswPublic.comboBox.close(); }, 100);
                };

                //#endregion cswPrivate/cswPublic methods and props

                //#region final ctor
                (function _post() {
                    if (false === cswPrivate.ReadOnly) {
                        cswPublic.table.cell(1, 2).imageButton({
                            ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                            AlternateText: 'Edit',
                            ID: Csw.makeId(cswPrivate.ID, 'toggle'),
                            onClick: function () {

                                cswPublic.table.cell(1, 1).hide();
                                cswPublic.table.cell(1, 2).hide();
                                cswPublic.table.cell(2, 1).show();

                                cswPublic.comboBox = cswPrivate.selectDiv.comboBox({
                                    ID: cswPrivate.ID + '_combo',
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
                                    ID: cswPrivate.ID,
                                    parent: cswPublic.comboBox.pickList,
                                    onInitialSelectNode: function (optSelect) {
                                        cswPrivate.onTreeSelect(cswPrivate.selectDiv, cswPublic.comboBox, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, function () { });
                                    },
                                    onSelectNode: function (optSelect) {
                                        cswPrivate.onTreeSelect(cswPrivate.selectDiv, cswPublic.comboBox, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, cswPrivate.onChange);
                                    },
                                    UseScrollbars: false,
                                    ShowToggleLink: false
                                });

                                cswPublic.locationTree.init({
                                    viewid: cswPrivate.viewId,
                                    nodeid: cswPrivate.nodeId,
                                    cswnbtnodekey: cswPrivate.nodeKey,
                                    IncludeInQuickLaunch: false,
                                    DefaultSelect: Csw.enums.nodeTree_DefaultSelect.root.name
                                });
                            } // onClick
                        }); // imageButton

                        cswParent.$.hover(function (event) {
                            Csw.nodeHoverIn(event, cswPrivate.selectDiv.propNonDom('value'));
                        }, Csw.nodeHoverOut);
                    }
                }());
                //#region final ctor

            });
            //#endregion safety net

            return cswPublic;

        });


}());


