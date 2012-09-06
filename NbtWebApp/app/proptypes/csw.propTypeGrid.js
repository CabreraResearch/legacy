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
                var render = function (o) {
                    'use strict';
                    o = o || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = o.propData.values;
                    cswPrivate.parent = o.propDiv;
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
                        var menuDiv = newDiv.div({ ID: Csw.makeId(o.ID + window.Ext.id(), 'grid_as_fieldtype_menu') }).css({ height: '25px' });
                        //newDiv.br();
                        var filterDiv = newDiv.div({ ID: Csw.makeId(o.ID + window.Ext.id(), 'grid_as_fieldtype_filter') });
                        //newDiv.br();
                        var gridDiv = newDiv.div({ ID: Csw.makeId(o.ID + window.Ext.id(), 'grid_as_fieldtype') });
                        cswPrivate.reinitGrid = (function () {
                            return function () {
                                cswPrivate.makeFullGrid(viewid, newDiv);
                            };
                        }());
                        Csw.nbt.viewFilters({
                            ID: o.ID + '_viewfilters',
                            parent: filterDiv,
                            viewid: viewid,
                            onEditFilters: function (newviewid) {
                                cswPrivate.makeFullGrid(newviewid, newDiv);
                            } // onEditFilters
                        }); // viewFilters

                        var gridOpts = {
                            ID: o.ID + '_fieldtypegrid',
                            //                        resizeWithParent: false,
                            //                        resizeWithParentElement: $('#nodetabs_props'),
                            viewid: viewid,
                            nodeid: o.nodeid,
                            cswnbtnodekey: o.cswnbtnodekey,
                            readonly: o.ReadOnly,
                            reinit: false,
                            EditMode: o.EditMode,
                            onEditNode: function () {
                                //o.onReload();
                                cswPrivate.reinitGrid();
                            },
                            onDeleteNode: function () {
                                //o.onReload();
                                cswPrivate.reinitGrid();
                            },
                            onSuccess: function (grid) {
                                cswPrivate.makeGridMenu(menuDiv, o, gridOpts, grid, viewid);
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
                                IncludeNodeKey: o.cswnbtnodekey,
                                MaxRows: cswPrivate.maxRows
                            },
                            success: function (data) {
                                cswPublic.control = cswPrivate.parent.thinGrid({
                                    rows: data.rows,
                                    onLinkClick: function () {
                                        $.CswDialog('OpenEmptyDialog', {
                                            title: o.nodename + ' ' + o.propData.name,
                                            onOpen: function (dialogDiv) {
                                                cswPrivate.makeFullGrid(cswPrivate.viewid, dialogDiv);
                                            },
                                            onClose: o.onReload
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
                                IncludeNodeKey: o.cswnbtnodekey
                            },
                            success: function (data) {
                                cswPublic.control = cswPrivate.parent.linkGrid({
                                    rowCount: data.rowCount,
                                    linkText: '',
                                    onLinkClick: function () {
                                        $.CswDialog('OpenEmptyDialog', {
                                            title: o.propData.name,
                                            onOpen: function (dialogDiv) {
                                                cswPrivate.makeFullGrid(cswPrivate.viewid, dialogDiv);
                                            },
                                            onClose: o.onReload
                                        }
                                        );
                                    }
                                });
                            }
                        });
                    };

                    if (o.EditMode === Csw.enums.editMode.AuditHistoryInPopup || o.Multi) {
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

                propertyOption.render(render);
                return cswPublic;
            }));

}());
