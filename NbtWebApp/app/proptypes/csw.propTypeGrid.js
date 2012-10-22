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
                    
                    cswPrivate.makeGridMenu = function(grid) {
                        //Case 21741
                        if (cswPublic.data.tabState.EditMode !== Csw.enums.editMode.PrintReport) {

                            var menuOpts = {
                                width: 150,
                                ajax: {
                                    urlMethod: 'getMainMenu',
                                    data: {
                                        ViewId: cswPrivate.viewid,
                                        SafeNodeKey: cswPublic.data.tabState.cswnbtnodekey,
                                        NodeTypeId: '',
                                        PropIdAttr: cswPublic.data.name,
                                        LimitMenuTo: '',
                                        ReadOnly: cswPublic.data.isReadOnly()
                                    }
                                },
                                onAlterNode: function() { 
                                    cswPrivate.reinitGrid(); 
                                },
                                onMultiEdit: function () { 
                                    grid.toggleShowCheckboxes(); 
                                },
                                onEditView: function() { 
                                    Csw.tryExec(cswPublic.data.onEditView, cswPrivate.viewid); 
                                },
                                onPrintView: function () { 
                                    grid.print(); 
                                },
                                Multi: false
                            };
                            cswPrivate.menuDiv.menu(menuOpts);

                        } // if( o.EditMode !== Csw.enums.editMode.PrintReport )
                    }; // makeGridMenu()

                    cswPrivate.makeFullGrid = function (viewid, newDiv) {
                        'use strict';
                        newDiv.empty();
                        cswPrivate.menuDiv = newDiv.div({ name: 'grid_as_fieldtype_menu' }).css({ height: '25px' });
                        var filterDiv = newDiv.div({ name: 'grid_as_fieldtype_filter' });
                        var gridDiv = newDiv.div({ name: 'grid_as_fieldtype' });
                        cswPrivate.reinitGrid = (function () {
                            return function () {
                                cswPrivate.makeFullGrid(viewid, newDiv);
                            };
                        }());
                        Csw.nbt.viewFilters({
                            name: cswPublic.data.name + '_viewfilters',
                            parent: filterDiv,
                            viewid: viewid,
                            onEditFilters: function (newviewid) {
                                cswPrivate.makeFullGrid(newviewid, newDiv);
                            } // onEditFilters
                        }); // viewFilters

                        var gridOpts = {
                            name: cswPublic.data.name + '_fieldtypegrid',
                            viewid: viewid,
                            nodeid: cswPublic.data.tabState.nodeid,
                            cswnbtnodekey: cswPublic.data.tabState.cswnbtnodekey,
                            readonly: cswPublic.data.isReadOnly(),
                            reinit: false,
                            EditMode: cswPublic.data.tabState.EditMode,
                            onEditNode: function () {
                                cswPrivate.reinitGrid();
                            },
                            onDeleteNode: function () {
                                cswPrivate.reinitGrid();
                            },
                            onSuccess: function (grid) {
                                cswPrivate.makeGridMenu(grid);
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
