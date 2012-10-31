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

                //The render function to be executed as a callback
                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.gridMode = Csw.string(cswPrivate.propVals.gridmode);
                    cswPrivate.maxRows = Csw.string(cswPrivate.propVals.maxrows);
                    cswPrivate.viewid = Csw.string(cswPrivate.propVals.viewid).trim();
                    
                    cswPrivate.makeGridMenu = function (grid, gridParentDiv) {
                        //Case 21741
                        if (cswPublic.data.tabState.EditMode !== Csw.enums.editMode.PrintReport) {

                            var menuOpts = {
                                width: 150,
                                ajax: {
                                    urlMethod: 'getMainMenu',
                                    data: {
                                        ViewId: cswPrivate.viewid,
                                        SafeNodeKey: Csw.string(cswPublic.data.tabState.nodekey),
                                        NodeId: Csw.string(cswPublic.data.tabState.nodeid),
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
                                onEditView: function () {
                                    Csw.tryExec(gridParentDiv.$.dialog('close'));
                                    Csw.tryExec(cswPublic.data.onEditView, cswPrivate.viewid); 
                                },
                                onPrintView: function () { 
                                    grid.print(); 
                                },
                                Multi: false
                            };
                            cswPrivate.menu = cswPrivate.menuDiv.menu(menuOpts);

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
                                cswPublic.control.reload();
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
                            nodekey: cswPublic.data.tabState.nodekey,
                            readonly: cswPublic.data.isReadOnly(),
                            reinit: false,
                            EditMode: cswPublic.data.tabState.EditMode,
                            onEditNode: function () {
                                cswPublic.control.reload();
                            },
                            onDeleteNode: function () {
                                cswPublic.control.reload();
                            },
                            onSuccess: function (grid) {
                                cswPrivate.makeGridMenu(grid, newDiv);
                            }
                        };
                        cswPublic.control = Csw.nbt.nodeGrid(gridDiv, gridOpts);
                    };

                    cswPrivate.makeSmallGrid = function () {
                        'use strict';
                        cswPrivate.smallAjax = Csw.ajax.post({
                            urlMethod: 'getThinGrid',
                            data: {
                                ViewId: cswPrivate.viewid,
                                IncludeNodeKey: cswPublic.data.tabState.nodekey,
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
                        cswPrivate.linkAjax = Csw.ajax.post({
                            urlMethod: 'getGridRowCount',
                            data: {
                                ViewId: cswPrivate.viewid,
                                IncludeNodeKey: cswPublic.data.tabState.nodekey
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

                    if (cswPublic.data.isReadOnly() ||
                        cswPublic.data.isMulti()) {

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

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests
                cswPublic.data.unBindRender(function() {
                    if (cswPublic.control && cswPublic.control.ajax && cswPublic.control.ajax.ajax) {
                        cswPublic.control.ajax.ajax.abort();
                    }
                    if (cswPrivate.linkAjax && cswPrivate.linkAjax.ajax) {
                        cswPrivate.linkAjax.ajax.abort();
                    }
                    if (cswPrivate.smallAjax && cswPrivate.smallAjax.ajax) {
                        cswPrivate.smallAjax.ajax.abort();
                    }
                    if(cswPrivate.menu && cswPrivate.menu.ajax && cswPrivate.menu.ajax.ajax) {
                        cswPrivate.menu.ajax.ajax.abort();
                    }
                });
                return cswPublic;
            }));

}());
