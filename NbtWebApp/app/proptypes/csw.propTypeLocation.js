/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.location = Csw.properties.location ||
        Csw.properties.register('location',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.onTreeSelect = function (optSelect) {
                        optSelect = optSelect || { nodeid: '', nodename: '', iconurl: '' };
                        if (optSelect.nodeid === 'root' ||
                            optSelect.nodeid === undefined) {

                            optSelect.nodeid = ''; // case 21046   
                        }
                        cswPrivate.comboBox.topContent(optSelect.nodename, optSelect.nodeid);
                        if (cswPrivate.comboBox.val() !== optSelect.nodeid) {
                            cswPrivate.comboBox.val(optSelect.nodeid);

                            Csw.tryExec(cswPublic.data.onChange, optSelect.nodeid);
                        }
                        cswPublic.data.onPropChange({ nodeid: optSelect.nodeid });
                        Csw.defer(function () { cswPrivate.comboBox.close(); }, 100);
                    };

                    cswPrivate.makeLocationCombo = function () {
                        var locationTree;

                        cswPrivate.table.cell(1, 1).hide();
                        cswPrivate.table.cell(1, 2).hide();
                        cswPrivate.table.cell(2, 1).show();

                        cswPrivate.comboBox = cswPrivate.selectDiv.comboBox({
                            ID: cswPublic.data.ID + '_combo',
                            topContent: cswPrivate.name,
                            selectContent: '',
                            width: '290px',
                            onClick: (function () {
                                var first = true;
                                return function () {
                                    if (first) { // only do this once
                                        locationTree.expandAll();
                                        first = false;
                                    }
                                    cswPrivate.comboBox.open(); // ensure we're open on click
                                    return false; // but only close when onTreeSelect fires, below
                                };
                            })()
                        });
                        cswPrivate.comboBox.required(cswPublic.data.Required);
                        locationTree = Csw.nbt.nodeTree({
                            ID: cswPublic.data.ID,
                            parent: cswPrivate.comboBox.pickList,
                            onInitialSelectNode: function (optSelect) {
                                cswPrivate.onTreeSelect(optSelect);
                            },
                            onSelectNode: function (optSelect) {
                                cswPrivate.onTreeSelect(optSelect);
                            },
                            UseScrollbars: false,
                            ShowToggleLink: false
                        });

                        locationTree.init({
                            viewid: cswPrivate.viewId,
                            nodeid: cswPrivate.nodeId,
                            cswnbtnodekey: cswPrivate.nodeKey,
                            IncludeInQuickLaunch: false,
                            DefaultSelect: Csw.enums.nodeTree_DefaultSelect.root.name
                        });
                    }; // makeLocationCombo()

                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.propVals = cswPublic.data.propData.values;

                    cswPrivate.locationobjectclassid = cswPrivate.propVals.locationobjectclassid;
                    cswPrivate.locationnodetypeids = cswPrivate.propVals.locationnodetypeids;  // array

                    cswPrivate.relatedmatch = (cswPublic.data.relatedobjectclassid === cswPrivate.locationobjectclassid);
                    Csw.each(cswPrivate.locationnodetypeids, function (thisNTid) {
                        cswPrivate.relatedmatch = (cswPrivate.relatedmatch || thisNTid === cswPublic.data.relatednodetypeid);
                    });

                    cswPrivate.nodeKey = ''; //(false === o.Multi) ? Csw.string(propVals.nodekey).trim() : '';
                    if (Csw.bool(cswPublic.data.Multi)) {
                        cswPrivate.nodeId = '';
                        cswPrivate.name = Csw.enums.multiEditDefaultValue;
                        cswPrivate.path = Csw.enums.multiEditDefaultValue;
                    }
                    else if (cswPrivate.relatedmatch) {
                        cswPrivate.nodeId = Csw.string(cswPrivate.propVals.nodeid, cswPublic.data.relatednodeid).trim();
                        cswPrivate.name = Csw.string(cswPrivate.propVals.name, cswPublic.data.relatednodename).trim();
                        cswPrivate.path = Csw.string(cswPrivate.propVals.path, cswPublic.data.relatednodename).trim();
                    } else {
                        cswPrivate.nodeId = Csw.string(cswPrivate.propVals.nodeid, '').trim();
                        cswPrivate.name = Csw.string(cswPrivate.propVals.name, '').trim();
                        cswPrivate.path = Csw.string(cswPrivate.propVals.path, '').trim();
                    }
                    cswPrivate.viewId = Csw.string(cswPrivate.propVals.viewid).trim();

                    cswPrivate.table = cswPrivate.parent.table({
                        ID: Csw.makeId(cswPublic.data.ID, 'tbl')
                    });

                    cswPrivate.table.cell(1, 1).text(cswPrivate.path);

                    cswPrivate.selectDiv = cswPrivate.table.cell(2, 1).div({
                        cssclass: 'locationselect',
                        value: cswPrivate.nodeId,
                        onChange: function () {
                            var val = cswPrivate.selectDiv.val();
                        }
                    });

                    cswPrivate.table.cell(2, 1).hide();
                    if (false === cswPublic.data.ReadOnly) {
                        
                        if (cswPublic.data.EditMode === Csw.enums.editMode.Add) {
                            cswPrivate.makeLocationCombo();
                        } else {
                            cswPrivate.table.cell(1, 2).icon({
                                ID: Csw.makeId(cswPublic.data.ID, 'toggle'),
                                iconType: Csw.enums.iconType.pencil,
                                hovertext: 'Edit',
                                size: 16,
                                isButton: true,
                                onClick: cswPrivate.makeLocationCombo
                            });
                        }

                        cswPrivate.parent.$.hover(function (event) { Csw.nodeHoverIn(event, cswPrivate.selectDiv.propNonDom('value')); },
                                        function (event) { Csw.nodeHoverOut(event, cswPrivate.selectDiv.propNonDom('value')); });
                    }
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());



