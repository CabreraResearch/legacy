/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.grid = Csw.properties.register('grid',
        function(nodeProperty) {
            'use strict';
            var cswPrivate = Csw.object();
            cswPrivate.ajaxCollection = [];
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';

                cswPrivate.gridMode = nodeProperty.propData.values.gridmode;
                cswPrivate.maxRows = nodeProperty.propData.values.maxrows;
                cswPrivate.viewid = nodeProperty.propData.values.viewid;
                cswPrivate.hasHeader = nodeProperty.propData.values.hasHeader;

                cswPrivate.makeGridMenu = function(nodeGrid, gridParentDiv, inDialog) {
                    //Case 21741
                    if (nodeProperty.tabState.EditMode !== Csw.enums.editMode.PrintReport) {

                        var menuOpts = {
                            width: 150,
                            ajax: {
                                urlMethod: 'getMainMenu',
                                data: {
                                    ViewId: cswPrivate.viewid,
                                    SafeNodeKey: Csw.string(nodeProperty.tabState.nodekey),
                                    NodeId: Csw.string(nodeProperty.tabState.nodeid),
                                    NodeTypeId: '',
                                    PropIdAttr: nodeProperty.name,
                                    LimitMenuTo: '',
                                    ReadOnly: nodeProperty.isReadOnly()
                                }
                            },
                            onAlterNode: function() {
                                cswPrivate.reinitGrid();
                            },
                            onMultiEdit: function() {
                                nodeGrid.grid.toggleShowCheckboxes();
                            },
                            onEditView: function() {
                                if (inDialog) {
                                    Csw.tryExec(gridParentDiv.$.dialog('close'));
                                }
                                Csw.tryExec(nodeProperty.onEditView, cswPrivate.viewid);
                            },
                            onPrintView: function() {
                                nodeGrid.grid.print();
                            },
                            Multi: false,
                            nodeGrid: nodeGrid
                        };
                        var menu = cswPrivate.menuDiv.menu(menuOpts);
                        cswPrivate.ajaxCollection.push(menu.ajax);
                    } // if( o.EditMode !== Csw.enums.editMode.PrintReport )
                }; // makeGridMenu()

                cswPrivate.makeFullGrid = function(viewid, newDiv, inDialog) {
                    'use strict';
                    newDiv.empty();
                    cswPrivate.menuDiv = newDiv.div({ name: 'grid_as_fieldtype_menu' }).css({ height: '25px' });

                    var filterDiv = newDiv.div({ name: 'grid_as_fieldtype_filter' });
                    var gridDiv = newDiv.div({ name: 'grid_as_fieldtype' });
                    cswPrivate.reinitGrid = (function() {
                        return function() {
                            nodeGrid.grid.reload(true);
                        };
                    }());
                    Csw.nbt.viewFilters({
                        name: nodeProperty.name + '_viewfilters',
                        parent: filterDiv,
                        viewid: viewid,
                        onEditFilters: function(newviewid) {
                            cswPrivate.makeFullGrid(newviewid, newDiv, inDialog);
                        } // onEditFilters
                    }); // viewFilters

                    var gridOpts = {
                        name: nodeProperty.name + '_fieldtypegrid',
                        title: nodeProperty.propData.name,
                        viewid: viewid,
                        nodeid: nodeProperty.tabState.nodeid,
                        nodekey: nodeProperty.tabState.nodekey,
                        readonly: nodeProperty.isReadOnly(),
                        reinit: false,
                        EditMode: nodeProperty.tabState.EditMode,
                        onEditNode: function() {
                            nodeGrid.grid.reload(true);
                        },
                        onDeleteNode: function() {
                            nodeGrid.grid.reload(true);
                        },
                        onSuccess: function(nodeGrid) {
                            cswPrivate.makeGridMenu(nodeGrid, newDiv, inDialog);
                        }
                    };

                    var nodeGrid = Csw.nbt.nodeGrid(gridDiv, gridOpts);
                    cswPrivate.ajaxCollection.push(nodeGrid.grid.ajax);
                };

                cswPrivate.makeSmallGrid = function() {
                    'use strict';
                    var smallAjax = Csw.ajax.post({
                        urlMethod: 'getThinGrid',
                        data: {
                            ViewId: cswPrivate.viewid,
                            NodeId: nodeProperty.tabState.nodeid,
                            MaxRows: cswPrivate.maxRows
                        },
                        success: function(data) {
                            nodeProperty.propDiv.thinGrid({
                                rows: data.rows,
                                hasHeader: cswPrivate.hasHeader,
                                onLinkClick: function() {
                                    $.CswDialog('OpenEmptyDialog', {
                                            title: nodeProperty.tabState.nodename + ' ' + nodeProperty.propData.name,
                                            onOpen: function(dialogDiv) {
                                                cswPrivate.makeFullGrid(cswPrivate.viewid, dialogDiv, true);
                                            },
                                            onClose: nodeProperty.onReload
                                        }
                                    );
                                }
                            });
                        }
                    });
                    cswPrivate.ajaxCollection.push(smallAjax);
                };

                cswPrivate.makeLinkGrid = function() {
                    'use strict';
                    var linkAjax = Csw.ajax.post({
                        urlMethod: 'getGridRowCount',
                        data: {
                            ViewId: cswPrivate.viewid,
                            NodeId: nodeProperty.tabState.nodeid
                        },
                        success: function(data) {
                            nodeProperty.propDiv.linkGrid({
                                rowCount: data.rowCount,
                                linkText: '',
                                onLinkClick: function() {
                                    $.CswDialog('OpenEmptyDialog', {
                                            title: nodeProperty.propData.name,
                                            onOpen: function(dialogDiv) {
                                                cswPrivate.makeFullGrid(cswPrivate.viewid, dialogDiv, true);
                                            },
                                            onClose: nodeProperty.onReload
                                        }
                                    );
                                }
                            });
                        }
                    });
                    cswPrivate.ajaxCollection.push(linkAjax);
                };

                if (false == nodeProperty.isReport() &&
                    nodeProperty.isMulti()) {

                    nodeProperty.propDiv.append('[Grid display disabled]');
                } else {

                    switch (cswPrivate.gridMode.toLowerCase()) {
                    case 'small':
                        cswPrivate.makeSmallGrid();
                        break;
                    case 'link':
                        cswPrivate.makeLinkGrid();
                        break;
                    default:
                        cswPrivate.makeFullGrid(cswPrivate.viewid, nodeProperty.propDiv, false);
                        break;
                    }

                } // if(o.EditMode !== Csw.enums.editMode.AuditHistoryInPopup)

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests
            nodeProperty.unBindRender(function() {
                Csw.iterate(cswPrivate.ajaxCollection, function(async) {
                    if (async) {
                        async.abort();
                    }
                });
            });
            return true;
        });

} ());
