/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.grid = Csw.properties.grid ||
        Csw.properties.register('grid',
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
                    cswPrivate.gridMode = Csw.string(cswPrivate.propVals.gridmode);
                    cswPrivate.maxRows = Csw.string(cswPrivate.propVals.maxrows);
                    cswPrivate.viewid = Csw.string(cswPrivate.propVals.viewid).trim();
                    
                    cswPrivate.makeGridMenu = function(menuDiv, o, gridOpts, cswGrid, viewid) {
                        //Case 21741
                        if (o.EditMode !== Csw.enums.editMode.PrintReport) {

                            var menuOpts = {
                                width: 150,
                                ajax: {
                                    urlMethod: 'getMainMenu',
                                    data: {
                                        ViewId: viewid,
                                        SafeNodeKey: o.cswnbtnodekey,
                                        NodeTypeId: '',
                                        PropIdAttr: o.ID,
                                        LimitMenuTo: '',
                                        ReadOnly: o.ReadOnly
                                    }
                                },
                                onAlterNode: function() { cswPrivate.reinitGrid(); },
                                onMultiEdit: function() { cswGrid.toggleShowCheckboxes(); },
                                onEditView: function() { Csw.tryExec(o.onEditView, viewid); },
                                onSaveView: null,
                                onPrintView: function() { cswGrid.print(); },
                                Multi: false,
                                nodeTreeCheck: ''
                            };
                            menuDiv.menu(menuOpts);

                        } // if( o.EditMode !== Csw.enums.editMode.PrintReport )
                    }; // makeGridMenu()

                    cswPrivate.makeFullGrid = function (viewid, newDiv) {
                        'use strict';
                        newDiv.empty();
                        var menuDiv = newDiv.div({ ID: Csw.makeId(cswPublic.data.ID + window.Ext.id(), 'grid_as_fieldtype_menu') }).css({ height: '25px' });
                        //newDiv.br();
                        var filterDiv = newDiv.div({ ID: Csw.makeId(cswPublic.data.ID + window.Ext.id(), 'grid_as_fieldtype_filter') });
                        //newDiv.br();
                        var gridDiv = newDiv.div({ ID: Csw.makeId(cswPublic.data.ID + window.Ext.id(), 'grid_as_fieldtype') });
                        cswPrivate.reinitGrid = (function () {
                            return function () {
                                cswPrivate.makeFullGrid(viewid, newDiv);
                            };
                        }());
                        Csw.nbt.viewFilters({
                            ID: cswPublic.data.ID + '_viewfilters',
                            parent: filterDiv,
                            viewid: viewid,
                            onEditFilters: function (newviewid) {
                                cswPrivate.makeFullGrid(newviewid, newDiv);
                            } // onEditFilters
                        }); // viewFilters

                        var gridOpts = {
                            ID: cswPublic.data.ID + '_fieldtypegrid',
                            //                        resizeWithParent: false,
                            //                        resizeWithParentElement: $('#nodetabs_props'),
                            viewid: viewid,
                            nodeid: cswPublic.data.tabState.nodeid,
                            cswnbtnodekey: cswPublic.data.tabState.cswnbtnodekey,
                            readonly: cswPublic.data.isReadOnly(),
                            reinit: false,
                            EditMode: cswPublic.data.tabState.EditMode,
                            onEditNode: function () {
                                //o.onReload();
                                cswPrivate.reinitGrid();
                            },
                            onDeleteNode: function () {
                                //o.onReload();
                                cswPrivate.reinitGrid();
                            },
                            onSuccess: function (grid) {
                                cswPrivate.makeGridMenu(menuDiv, cswPublic.data, gridOpts, grid, viewid);
                            }
                        };
                        cswPublic.control = gridDiv.$.CswNodeGrid('init', gridOpts);
                    };

                    cswPrivate.makeSmallGrid = function () {
                        'use strict';
                        Csw.ajax.post({
                            urlMethod: 'getThinGrid',
                            data: {
                                ViewId: cswPrivate.viewid,
                                IncludeNodeKey: cswPublic.data.tabState.cswnbtnodekey,
                                MaxRows: cswPrivate.maxRows
                            },
                            success: function (data) {
                                cswPublic.control = cswPrivate.parent.thinGrid({
                                    rows: data.rows,
                                    hasHeader: true,
                                    onLinkClick: function () {
                                        $.CswDialog('OpenEmptyDialog', {
                                            title: cswPublic.data.tabState.nodename + ' ' + cswPublic.data.propData.name,
                                            onOpen: function (dialogDiv) {
                                                cswPrivate.makeFullGrid(cswPrivate.viewid, dialogDiv);
                                            },
                                            onClose: cswPublic.data.onReload
                                        }
                                        );
                                    }
                                });
                            }
                        });
                    };

                    cswPrivate.makeLinkGrid = function () {
                        'use strict';
                        Csw.ajax.post({
                            urlMethod: 'getGridRowCount',
                            data: {
                                ViewId: cswPrivate.viewid,
                                IncludeNodeKey: cswPublic.data.tabState.cswnbtnodekey
                            },
                            success: function (data) {
                                cswPublic.control = cswPrivate.parent.linkGrid({
                                    rowCount: data.rowCount,
                                    linkText: '',
                                    onLinkClick: function () {
                                        $.CswDialog('OpenEmptyDialog', {
                                            title: cswPublic.data.propData.name,
                                            onOpen: function (dialogDiv) {
                                                cswPrivate.makeFullGrid(cswPrivate.viewid, dialogDiv);
                                            },
                                            onClose: cswPublic.data.onReload
                                        }
                                        );
                                    }
                                });
                            }
                        });
                    };

                    if (cswPublic.data.tabState.EditMode === Csw.enums.editMode.AuditHistoryInPopup || cswPublic.data.isMulti()) {
                        cswPublic.control = cswPrivate.parent.append('[Grid display disabled]');
                    } else {
                        
                        switch (cswPrivate.gridMode.toLowerCase()) {
                            case 'small':
                                cswPrivate.makeSmallGrid();
                                break;
                            case 'link':
                                cswPrivate.makeLinkGrid();
                                break;
                            default:
                                cswPrivate.makeFullGrid(cswPrivate.viewid, cswPrivate.parent);
                                break;
                        }

                    } // if(o.EditMode !== Csw.enums.editMode.AuditHistoryInPopup)

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());
