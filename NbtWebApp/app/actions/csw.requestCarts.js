
(function () {

    Csw.actions.requestCarts = Csw.actions.requestCarts ||
        Csw.actions.register('requestCarts', function (cswParent, cswPrivate) {
            'use strict';

            //#region _preCtor

            var cswPublic = {};
            
            if (Csw.isNullOrEmpty(cswParent)) {
                Csw.error.throwException('Cannot create a Submit Request action without a valid Csw Parent object.', 'Csw.actions.requestCarts', 'csw.requestCarts.js', 14);
            }
            (function _preCtor() {
                cswPrivate.name = cswPrivate.name || 'CswSubmitRequest';
                cswPrivate.onSubmit = cswPrivate.onSubmit || function() {};
                cswPrivate.onCancel = cswPrivate.onCancel || function () {};
                cswPrivate.cartnodeid = cswPrivate.cartnodeid || '';
                cswPrivate.cartviewid = cswPrivate.cartviewid || '';
                cswPrivate.materialnodeid = cswPrivate.materialnodeid || '';
                cswPrivate.containernodeid = cswPrivate.containernodeid || '';
                
                cswParent.empty();
            }());

            cswPrivate.requestName = Csw.cookie.get(Csw.cookie.cookieNames.Username) + ' ' + Csw.todayAsString();
            cswPrivate.gridOpts = {};
            cswPrivate.state = {};
            cswPrivate.currentTab = 'Pending';
            
            //#endregion _preCtor
            
            //#region AJAX methods

            cswPrivate.getCart = function() {
                Csw.ajaxWcf.get({
                    urlMethod: 'Requests/cart',
                    success: function (data) {
                        cswPrivate.state.pendingItemsViewId = data.PendingItemsView.SessionViewId;
                        cswPrivate.state.favoritesListViewId = data.FavoritesView.SessionViewId;
                        cswPrivate.state.favoriteItemsViewId = data.FavoriteItemsView.SessionViewId;
                        cswPrivate.state.recurringItemsViewId = data.RecurringItemsView.SessionViewId;
                        cswPrivate.state.submittedItemsViewId = data.SubmittedItemsView.SessionViewId;

                        cswPrivate.onTabSelect(cswPrivate.currentTab);
                    }
                });
            };

            cswPrivate.makeRequestCreateMaterial = function () {
                Csw.ajaxWcf.get({
                    urlMethod: 'Requests/findMaterialCreate',
                    success: function (data) {
                        if (data.NodeTypeId) {
                            $.CswDialog('AddNodeDialog', {
                                text: 'New Create Material Request',
                                nodetypeid: data.NodeTypeId,
                                onAddNode: function () {
                                    cswPublic.grid.reload();
                                    Csw.publish(Csw.enums.events.main.refreshHeader);
                                }
                            });
                        }
                    }
                });
            };

            cswPrivate.submitRequest = function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Requests/place',
                    data: {
                        NodeId: cswPrivate.cartnodeid,
                        NodeName: cswPrivate.requestName
                    },
                    success: function (json) {
                        if (json.Succeeded) {
                            Csw.tryExec(cswPrivate.onSubmit);
                        }
                    }
                });
            }; // cswPrivate.submitRequest

            cswPrivate.addFavorite = function(callBack) {
                Csw.ajaxWcf.put({
                    urlMethod: 'Requests/Favorites/create',
                    success: function (json) {
                        if (json.Succeeded) {
                            $.CswDialog('EditNodeDialog', {
                                currentNodeId: json.RequestId,
                                filterToPropId: json.RequestName,
                                title: 'Add new Favorite',
                                onEditNode: function (nodeid, nodekey, close) {
                                    cswPrivate.lastCreatedFavorite = nodeid;
                                    Csw.ajaxWcf.put({
                                        urlMethod: 'Requests/Favorites/create',
                                        data: {
                                            RequestId: cswPrivate.lastCreatedFavorite
                                        },
                                        success: callBack
                                    });
                                }
                            });
                        }
                    }
                });
            };

            cswPrivate.copyToRequest = function (copyToRequest, copyRequestItems) {
                if (copyToRequest && copyRequestItems) {
                    Csw.ajaxWcf.post({
                        urlMethod: 'Requests/Favorites/copy',
                        data: {
                            RequestItems: copyRequestItems,
                            RequestId: copyToRequest
                        },
                        onSuccess: function() {
                            Csw.publish(Csw.enums.events.main.refreshHeader);
                        }
                    });
                }
            }; // copyRequest()  

            //#endregion AJAX methods

            //#region Tab construction

            cswPrivate.onTabSelect = function (tabName, el, eventObj, callBack) {
                if (tabName && tabName.length > 0) {
                    cswPrivate.currentTab = tabName;
                    switch (tabName) {
                        case 'Pending':
                            cswPrivate.action.finish.enable();
                            cswPrivate.tabs.setTitle('Request Items Pending Submission');
                            cswPrivate.makePendingTab();
                            break;
                        case 'Submitted':
                            cswPrivate.action.finish.disable();
                            break;
                        case 'Recurring':
                            cswPrivate.action.finish.disable();
                            break;
                        case 'Favorites':
                            cswPrivate.action.finish.disable();
                            break;
                        }
                }
            };

            cswPrivate.makeGridForTab = function(opts) {
                opts = opts || {
                    onSuccess: function() {},
                    onSelectChange: function() {}
                };

                opts.parent.grid({
                    name: opts.gridId,
                    storeId: opts.gridId + '_store',
                    stateId: opts.gridId,
                    usePaging: true,
                    height: 180,
                    width: cswPrivate.tabs.getWidth() - 20,
                    ajax: {
                        urlMethod: 'getRequestItemGrid',
                        data: {
                            SessionViewId: opts.sessionViewId
                        }
                    },
                    showCheckboxes: true,
                    showActionColumn: true,
                    canSelectRow: false,

                    onLoad: function(grid, json) {
                        cswPrivate.cartnodeid = json.cartnodeid;
                        cswPrivate.cartviewid = json.cartviewid;
                        Csw.tryExec(opts.onSuccess);
                    },
                    onEdit: function(rows) {
                        // this works for both Multi-edit and regular
                        var nodekeys = Csw.delimitedString(),
                            nodeids = Csw.delimitedString(),
                            nodenames = [],
                            currentNodeId, currentNodeKey;


                        Csw.each(rows, function(row) {
                            currentNodeId = currentNodeId || row.nodeid;
                            currentNodeKey = currentNodeKey || row.nodekey;
                            nodekeys.add(row.nodekey);
                            nodeids.add(row.nodeid);
                            nodenames.push(row.nodename);
                        });

                        $.CswDialog('EditNodeDialog', {
                            currentNodeId: currentNodeId,
                            currentNodeKey: currentNodeKey,
                            selectedNodeIds: nodeids,
                            selectedNodeKeys: nodekeys,
                            nodenames: nodenames,
                            Multi: (nodeids.length > 1),
                            title: 'Request',
                            onEditNode: function() {
                                Csw.tryExec(opts.onEditNode); //Case 27619--don't pass the function by reference, because we want to control the parameters with which it is called
                            }
                        });
                    }, // onEdit
                    onDelete: function(rows) {
                        // this works for both Multi-edit and regular
                        var cswnbtnodekeys = [],
                            nodeids = [],
                            nodenames = [];

                        Csw.each(rows, function(row) {
                            cswnbtnodekeys.push(row.nodekey);
                            nodeids.push(row.nodeid);
                            nodenames.push(row.nodename);
                        });

                        $.CswDialog('DeleteNodeDialog', {
                            nodeids: nodeids,
                            nodepks: nodeids,
                            nodekeys: cswnbtnodekeys,
                            nodenames: nodenames,
                            onDeleteNode: Csw.method(function() {
                                Csw.publish(Csw.enums.events.main.refreshHeader);
                                Csw.tryExec(opts.onEditNode);
                            }),
                            Multi: (nodeids.length > 1),
                            publishDeleteEvent: false
                        });                                  
                    }, // onDelete
                    onSelectChange: function(rowCount) {
                        Csw.tryExec(opts.onSelectChange, rowCount);
                    }
                });
            };

            cswPrivate.prepTab = function(tab, title, headerText) {
                cswPrivate.tabs.setTitle(title);

                tab.csw.empty().css({ margin: '10px' });

                var ol = tab.csw.ol();

                ol.li().span({
                    text: headerText
                });
                ol.li().br({ number: 2 });

                return ol;
            };

            cswPrivate.makePendingTab = function (opts) {
                cswPrivate.action.finish.enable();

                var ol = cswPrivate.prepTab(cswPrivate.pendingTab, 'Request Items Pending Submission', 'Edit any of the Request Items in your cart. When you are finished, click "Place Request" to submit your cart.');
                
                var inpTbl = ol.li().table({
                    width: '100%',
                    FirstCellRightAlign: true
                });

                inpTbl.cell(1, 1).span().setLabelText('Request Name: ');
                inpTbl.cell(1,2).input({
                    value: cswPrivate.requestName,
                    onChange: function(val) {
                        cswPrivate.requestName = val;
                    }
                });

                inpTbl.cell(1, 3)
                    .css({ 'text-align': 'center' })
                    .buttonExt({
                    enabledText: 'Request Create Material',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cart),
                    onClick: cswPrivate.makeRequestCreateMaterial
                });
                
                ol.li().br({ number: 1 });

                var hasOneRowSelected = false;
                var favoriteSelect;
                
                var toggleSaveBtn = function() {
                    if (favoriteSelect) {
                        var nodeId = favoriteSelect.selectedNodeId();
                        if (nodeId) {
                            cswPrivate.selectedFavoriteId = nodeId;
                        }
                    }
                    if (hasOneRowSelected && cswPrivate.selectedFavoriteId) {
                        saveBtn.enable();
                    } else {
                        saveBtn.disable();
                    }  
                };

                // This really ought to be a CswNodeGrid
                opts = opts || {
                    onSuccess: function () { }
                };
                opts.sessionViewId = cswPrivate.state.pendingItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_srgrid';
                opts.onSelectChange = function(rowCount) {
                    hasOneRowSelected = rowCount > 0;
                    toggleSaveBtn();
                };
                opts.onEditNode = cswPrivate.makePendingTab;

                cswPublic.grid = cswPrivate.makeGridForTab(opts);

                ol.li().br();
                
                var btmTbl = ol.li().table();
                var saveBtn = btmTbl.cell(1, 1).buttonExt({
                    enabledText: 'Save to Favorites',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                    onClick: function () {
                        var nodes = [];
                        cswPublic.grid.getSelectedRowsVals('nodeid').forEach(function (nodeId) {
                            nodes.push({ NodeId: nodeId });
                        });
                        cswPrivate.copyToRequest(cswPrivate.selectedFavoriteId, nodes);
                    }
                }).disable();
                
                var picklistCell = btmTbl.cell(1, 2);

                var makePicklist = function () {
                    picklistCell.empty();
                    favoriteSelect = picklistCell.nodeSelect({
                        width: '50px',
                        selectedNodeId: cswPrivate.lastCreatedFavorite || '',
                        showSelectOnLoad: true,
                        viewid: cswPrivate.state.favoritesListViewId,
                        allowAdd: false,
                        onSelectNode: toggleSaveBtn,
                        onSuccess: toggleSaveBtn
                    });
                };
                makePicklist();
                btmTbl.cell(1, 3).buttonExt({
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                    onClick: function () {
                        Csw.tryExec(cswPrivate.addFavorite, makePicklist);
                    }
                });

            }; // cswPrivate.makePendingTab()

            cswPrivate.makeSubmittedTab = function() {
                var ol = cswPrivate.prepTab(cswPrivate.pendingTab, 'Submitted Request Itens', 'View any previously submitted Request Items.');

                Csw.nbt.viewFilters({
                    name: 'submitted_viewfilters',
                    parent: ol.li(),
                    viewid: o.viewid,
                    onEditFilters: function (newviewid) {
                        var newopts = o;
                        newopts.viewid = newviewid;
                        // set the current view to be the session view, so filters are saved
                        Csw.clientState.setCurrentView(newviewid, Csw.enums.viewMode.grid.name);
                        //getViewGrid(newopts);
                    } // onEditFilters
                }); // viewFilters

            };

            //#endregion Tab construction  

            //#region _postCtor            

            (function _postCtor() {

                cswPrivate.action = Csw.layouts.action(cswParent, {
                    title: 'Requests',
                    finishText: 'Place Request',
                    cancelText: 'Close',
                    onFinish: cswPrivate.submitRequest,
                    onCancel: cswPrivate.onCancel
                });
                cswPrivate.action.finish.disable();

                cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({
                    name: cswPrivate.name + '_tbl',
                    align: 'center'
                }).css('width', '95%');

                cswPrivate.action.setHeader('Pending Request Items');

                cswPrivate.tabs = cswPrivate.actionTbl.cell(2, 1).tabStrip({
                    onTabSelect: cswPrivate.onTabSelect
                });

                cswPrivate.pendingTab = cswPrivate.tabs.addTab({
                    title: 'Pending'
                });

                cswPrivate.submittedTab = cswPrivate.tabs.addTab({
                    title: 'Submitted'
                });

                cswPrivate.recurringTab = cswPrivate.tabs.addTab({
                    title: 'Recurring'
                });

                cswPrivate.favoritesTab = cswPrivate.tabs.addTab({
                    title: 'Favorites'
                });

                cswPrivate.tabs.setActiveTab(0);

                cswPrivate.getCart();
                
            }());

            return cswPublic;
            
            //#endregion _postCtor
        });
}());