
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
                        cswPrivate.state.pendingCartId = data.CurrentRequest.NodeId;

                        cswPrivate.onTabSelect(cswPrivate.currentTab);

                        cswPrivate.getCartCounts();
                    }
                });
            };

            cswPrivate.getCartCounts = function () {
                Csw.ajaxWcf.get({
                    urlMethod: 'Requests/counts',
                    data: {
                        CartId: cswPrivate.state.pendingCartId
                    },
                    success: function (data) {
                        cswPrivate.pendingTab.ext.setTitle('Pending (' + data.Counts.PendingRequestItems + ')');
                        cswPrivate.submittedTab.ext.setTitle('Submitted (' + data.Counts.SubmittedRequestItems + ')');
                        cswPrivate.recurringTab.ext.setTitle('Recurring (' + data.Counts.RecurringRequestItems + ')');
                        cswPrivate.favoritesTab.ext.setTitle('Favorites (' + data.Counts.FavoriteRequestItems + ')');
                    }
                });
            };

            cswPrivate.makeRequestCreateMaterial = function (grid) {
                Csw.ajaxWcf.get({
                    urlMethod: 'Requests/findMaterialCreate',
                    success: function (data) {
                        if (data.NodeTypeId) {
                            $.CswDialog('AddNodeDialog', {
                                text: 'New Create Material Request',
                                nodetypeid: data.NodeTypeId,
                                onAddNode: function () {
                                    grid.reload();
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
                        NodeId: cswPrivate.state.pendingCartId,
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
                    cswPrivate.currentTab = tabName.split(' ')[0];
                    switch (cswPrivate.currentTab) {
                        case 'Pending':
                            cswPrivate.tabs.setTitle('Request Items Pending Submission');
                            cswPrivate.makePendingTab();
                            break;
                        case 'Submitted':
                            cswPrivate.makeSubmittedTab();
                            break;
                        case 'Recurring':
                            cswPrivate.makeRecurringTab();
                            break;
                        case 'Favorites':
                            cswPrivate.makeFavoritesTab();
                            break;
                        }
                }
            };

            cswPrivate.makeGridForTab = function(opts) {
                opts = opts || {
                    onSuccess: function() {},
                    onSelectChange: function() {}
                };

                return opts.parent.grid({
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
                    showCheckboxes: opts.showCheckboxes,
                    showActionColumn: true,
                    canSelectRow: false,
                    groupField: opts.groupField || '',
                    onLoad: function(grid, json) {
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
                
                tab.csw.empty().css({ margin: '10px' });

                var ol = tab.csw.ol();

                ol.li().span({
                    text: headerText
                });
                ol.li().br({ number: 2 });

                return ol;
            };

            cswPrivate.makePendingTab = function (opts) {
                var ol = cswPrivate.prepTab(cswPrivate.pendingTab, 'Request Items Pending Submission', 'Edit any of the Request Items in your cart. When you are finished, click "Place Request" to submit your cart.');
                
                var inpTbl = ol.li().table({
                    width: '99%'
                });

                inpTbl.cell(1, 1).span().setLabelText('Request Name: ');
                inpTbl.cell(1, 1).input({
                    value: cswPrivate.requestName,
                    onChange: function(val) {
                        cswPrivate.requestName = val;
                    }
                });

                inpTbl.cell(1, 2)
                    .css({ 'text-align': 'right' })
                    .buttonExt({
                        enabledText: 'Request Create Material',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cart),
                        onClick: function() {
                            if (cswPublic.pendingGrid) {
                                cswPrivate.makeRequestCreateMaterial(cswPublic.pendingGrid);
                            }
                        }
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

                opts = opts || {
                    onSuccess: function () { }
                };
                opts.sessionViewId = cswPrivate.state.pendingItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_pendingGrid';
                opts.showCheckboxes = true;

                opts.onSelectChange = function (rowCount) {
                    hasOneRowSelected = rowCount > 0;
                    toggleSaveBtn();
                };
                opts.onEditNode = cswPrivate.makePendingTab;

                cswPublic.pendingGrid = cswPrivate.makeGridForTab(opts);

                ol.li().br();
                
                var btmTbl = ol.li().table();
                var saveBtn = btmTbl.cell(1, 1).buttonExt({
                    enabledText: 'Save to Favorites',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                    onClick: function () {
                        var nodes = [];
                        cswPublic.pendingGrid.getSelectedRowsVals('nodeid').forEach(function (nodeId) {
                            nodes.push({ NodeId: nodeId });
                        });
                        cswPrivate.copyToRequest(cswPrivate.selectedFavoriteId, nodes);
                        toggleSaveBtn();
                    }
                }).disable();
                
                var picklistCell = btmTbl.cell(1, 2);

                var makePicklist = function () {
                    picklistCell.empty();
                    favoriteSelect = picklistCell.nodeSelect({
                        width: '100px',
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

                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), true);

            }; // cswPrivate.makePendingTab()

            cswPrivate.makeSubmittedTab = function() {
                var ol = cswPrivate.prepTab(cswPrivate.submittedTab, 'Submitted Request Items', 'View any previously submitted Request Items.');

                Csw.nbt.viewFilters({
                    name: 'submitted_viewfilters',
                    parent: ol.li(),
                    viewid: cswPrivate.state.submittedItemsViewId,
                    onEditFilters: function (newviewid) {
                        cswPrivate.state.submittedItemsViewId = newviewid;
                        cswPrivate.makeSubmittedTab();
                    } // onEditFilters
                }); // viewFilters

                ol.br();

                var opts = {
                    onSuccess: function () { }
                };
                opts.showCheckboxes = false;
                opts.sessionViewId = cswPrivate.state.submittedItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_submittedItemsGrid';
                
                opts.onEditNode = cswPrivate.makeSubmittedTab;

                cswPublic.submittedGrid = cswPrivate.makeGridForTab(opts);

                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), false);
            };

            cswPrivate.makeRecurringTab = function() {
                var ol = cswPrivate.prepTab(cswPrivate.recurringTab, 'Recurring Request Items', 'View any recurring Request Items.');
                ol.br();

                var opts = {
                    onSuccess: function () { }
                };
                opts.showCheckboxes = false;
                opts.sessionViewId = cswPrivate.state.recurringItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_recurringItemsGrid';
                
                opts.onEditNode = cswPrivate.makeRecurringTab;

                cswPublic.recurringGrid = cswPrivate.makeGridForTab(opts);
                
                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), false);

            };

            cswPrivate.makeFavoritesTab = function() {
                var ol = cswPrivate.prepTab(cswPrivate.favoritesTab, 'Favorite Request Itens', 'View any favorite Request Items.');
                Csw.nbt.viewFilters({
                    name: 'favorites_viewfilters',
                    parent: ol.li(),
                    viewid: cswPrivate.state.favoriteItemsViewId,
                    onEditFilters: function (newviewid) {
                        cswPrivate.state.favoriteItemsViewId = newviewid;
                        cswPrivate.makeFavoritesTab();
                    } // onEditFilters
                }); // viewFilters

                ol.br();

                var hasOneRowSelected = false;

                var toggleSaveBtn = function () {
                    if (hasOneRowSelected) {
                        copyBtn.enable();
                    } else {
                        copyBtn.disable();
                    }
                };

                var opts = {
                    onSuccess: function () { }
                };
                opts.showCheckboxes = true;
                opts.sessionViewId = cswPrivate.state.favoriteItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_favoriteItemsGrid';
                
                opts.onEditNode = cswPrivate.makeFavoritesTab;
                opts.onSelectChange = function (rowCount) {
                    hasOneRowSelected = rowCount > 0;
                    toggleSaveBtn();
                };
                cswPublic.favoritesGrid = cswPrivate.makeGridForTab(opts);

                ol.li().br();

                var copyBtn = ol.li().buttonExt({
                    enabledText: 'Copy to Current Cart',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.copy),
                    onClick: function () {
                        var nodes = [];
                        cswPublic.favoritesGrid.getSelectedRowsVals('nodeid').forEach(function (nodeId) {
                            nodes.push({ NodeId: nodeId });
                        });
                        cswPrivate.copyToRequest(cswPrivate.state.pendingCartId, nodes);
                        Csw.publish(Csw.enums.events.main.refreshHeader);
                        toggleSaveBtn();
                    }
                }).disable();

                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), false);

            };

            cswPrivate.addBtnGroup = function(el, hasFinish) {
                var tbl = el.table({ width: '99%' });

                if (hasFinish) {
                    tbl.cell(1, 1).buttonExt({
                        enabledText: 'Place Request',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.check),
                        onClick: function () {
                            Csw.tryExec(cswPrivate.onFinish);
                        }
                    });
                }
                tbl.cell(1, 2).css({'text-align': 'right'}).buttonExt({
                    enabledText: 'Close',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cancel),
                    onClick: function () {
                        Csw.tryExec(cswPrivate.onCancel);
                    }
                });
            };

            //#endregion Tab construction

            //#region _postCtor            

            (function _postCtor() {

                cswParent.empty();
                cswPrivate.tabs = cswParent.tabStrip({
                    onTabSelect: cswPrivate.onTabSelect
                });
                cswPrivate.tabs.setTitle('Requests');
                
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