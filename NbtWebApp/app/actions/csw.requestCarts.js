/*global Csw:true*/
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
                cswPrivate.onSubmit = cswPrivate.onSubmit || function () { };
                cswPrivate.onCancel = cswPrivate.onCancel || function () { };
                cswPrivate.materialnodeid = cswPrivate.materialnodeid || '';
                cswPrivate.containernodeid = cswPrivate.containernodeid || '';

                cswParent.empty();
            }());

            cswPrivate.requestName = "";
            cswPrivate.gridOpts = {};
            cswPrivate.ajaxii = {};

            cswPrivate.currentTab = 'Pending';

            cswPrivate.restoreState = function () {
                var state = Csw.clientDb.getItem(cswPrivate.name + '_state');
                cswPrivate.state = state || {
                    pendingItemsViewId: '',
                    favoritesListViewId: '',
                    favoriteItemsViewId: '',
                    recurringItemsViewId: '',
                    submittedItemsViewId: '',
                    pendingCartId: '',
                    copyableId: 0,
                    cartCounts: {
                        PendingRequestItems: 0,
                        SubmittedRequestItems: 0,
                        RecurringRequestItems: 0,
                        FavoriteRequestItems: 0
                    }
                };
            };
            cswPrivate.restoreState();

            cswPrivate.saveState = function () {
                Csw.clientDb.setItem(cswPrivate.name + '_state', cswPrivate.state);
            };

            cswPrivate.clearState = function () {
                Csw.clientDb.removeItem(cswPrivate.name + '_state');
            };

            //#endregion _preCtor

            //#region AJAX methods

            cswPrivate.getCart = function (ActiveTab, TabName, onSuccess) {
                Csw.ajaxWcf.get({
                    urlMethod: 'Requests/cart',
                    success: function (data) {
                        cswPrivate.state.pendingItemsViewId = data.PendingItemsView.SessionViewId;
                        cswPrivate.state.favoritesListViewId = data.FavoritesView.SessionViewId;
                        cswPrivate.state.favoriteItemsViewId = data.FavoriteItemsView.SessionViewId;
                        cswPrivate.state.recurringItemsViewId = data.RecurringItemsView.SessionViewId;
                        cswPrivate.state.submittedItemsViewId = data.SubmittedItemsView.SessionViewId;
                        cswPrivate.state.pendingCartId = data.CurrentRequest.NodeId;
                        cswPrivate.state.copyableId = data.CopyableObjectClassId;

                        cswPrivate.saveState();

                        if (ActiveTab && TabName) {
                            cswPrivate.tabs.setActiveTab(ActiveTab);
                            cswPrivate.onTabSelect(TabName);
                        } else {
                            cswPrivate.onTabSelect(cswPrivate.currentTab);
                        }

                        if (onSuccess) {
                            Csw.tryExec(onSuccess);
                        }
                    }
                });
            };

            //Case 30082: Should the Cached cart counts get out of sync, instrument a call to reset
            cswPrivate.resetCart = function () {
                return $.ajax({
                    type: 'POST',
                    url: 'Services/Requests/Cart/reset',
                    xhrFields: {
                        withCredentials: true
                    },
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8'
                });
            };

            cswPrivate.getCartCounts = function () {

                cswPrivate.resetCart(); //Fire this reset immediately to correct any non-toward counts. See case 30496 for improvements against this hack.
                
                if (cswPrivate.ajaxii.getCartCounts && cswPrivate.ajaxii.getCartCounts.ajax) {
                    cswPrivate.ajaxii.getCartCounts.abort();
                }
                cswPrivate.ajaxii.getCartCounts = Csw.ajaxWcf.get({
                    urlMethod: 'Requests/counts',
                    data: {
                        CartId: cswPrivate.state.pendingCartId
                    },
                    success: function (data) {
                        var oldPendingCnt = cswPrivate.state.cartCounts.PendingRequestItems;
                        var oldSubmittedCnt = cswPrivate.state.cartCounts.SubmittedRequestItems;
                        var oldRecurringCnt = cswPrivate.state.cartCounts.RecurringRequestItems;
                        var oldFavoritesCnt = cswPrivate.state.cartCounts.FavoriteRequestItems;

                        cswPrivate.state.cartCounts = data.Counts;
                        cswPrivate.saveState();
                        if (oldPendingCnt !== cswPrivate.state.cartCounts.PendingRequestItems) {
                            Csw.publish(Csw.enums.events.main.refreshHeader);
                            cswPrivate.pendingTab.ext.setTitle('Pending (' + cswPrivate.state.cartCounts.PendingRequestItems + ')');
                        }
                        if (oldSubmittedCnt !== cswPrivate.state.cartCounts.SubmittedRequestItems) {
                            cswPrivate.submittedTab.ext.setTitle('Submitted (' + cswPrivate.state.cartCounts.SubmittedRequestItems + ')');
                        }
                        if (oldRecurringCnt !== cswPrivate.state.cartCounts.RecurringRequestItems) {
                            cswPrivate.recurringTab.ext.setTitle('Recurring (' + cswPrivate.state.cartCounts.RecurringRequestItems + ')');
                        }
                        if (oldFavoritesCnt !== cswPrivate.state.cartCounts.FavoriteRequestItems) {
                            cswPrivate.favoritesTab.ext.setTitle('Favorites (' + cswPrivate.state.cartCounts.FavoriteRequestItems + ')');
                        }
                        cswPrivate.tabs.resetWidth();
                    }
                });
            };

            cswPrivate.makeRequestCreateMaterial = function (grid) {
                cswPrivate.ajaxii.makeRequestCreateMaterial = Csw.ajaxWcf.get({
                    urlMethod: 'Requests/findMaterialCreate',
                    success: function (data) {
                        if (data.NodeTypeId) {
                            Csw.layouts.addnode({
                                title: 'New Create Material Request',
                                nodetypeid: data.NodeTypeId,
                                onAddNode: function () {
                                    grid.reload(true);
                                    cswPrivate.getCartCounts();
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
                            cswPrivate.getCart(1, "Submitted", cswPrivate.onSubmit);
                        }
                    }
                });
            }; // cswPrivate.submitRequest

            cswPrivate.addFavorite = function (callBack) {
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

            cswPrivate.copyFavorites = function (copyToRequest, copyRequestItems, onSuccess) {
                if (copyToRequest && copyRequestItems) {
                    Csw.ajaxWcf.post({
                        urlMethod: 'Requests/Favorites/copy',
                        data: {
                            RequestItems: copyRequestItems,
                            RequestId: copyToRequest
                        },
                        success: function () {
                            cswPrivate.getCartCounts();
                            onSuccess();
                        }
                    });
                }
            }; // copyFavorites()  

            cswPrivate.copyRecurring = function (copyRequestItems, onError) {
                if (copyRequestItems) {
                    Csw.ajaxWcf.post({
                        urlMethod: 'Requests/Recurring/copy',
                        data: {
                            RequestItems: copyRequestItems,
                            RequestId: cswPrivate.state.pendingCartId
                        },
                        success: function () {
                            cswPrivate.openTab('Recurring');
                        },
                        error: function () {
                            onError();
                        }
                    });
                }
            }; // copyRecurring()

            //#endregion AJAX methods

            //#region Tab construction

            cswPrivate.openTab = function (tabName) {
                var idx = cswPrivate.tabNames.indexOf(tabName);
                if (idx !== -1) {
                    cswPrivate.tabs.setActiveTab(idx);
                    cswPrivate.onTabSelect(tabName);
                }
            };

            cswPrivate.destroyOtherTabs = function (preserveTabName) {
                if (preserveTabName !== 'Favorites' && cswPublic.favoritesGrid) {
                    cswPublic.favoritesGrid.ajax.abort();
                    cswPublic.favoritesGrid.destroy();
                    cswPrivate.favoritesTab.csw.empty();
                }
                if (preserveTabName !== 'Recurring' && cswPublic.recurringGrid) {
                    cswPublic.recurringGrid.ajax.abort();
                    cswPublic.recurringGrid.destroy();
                    cswPrivate.recurringTab.csw.empty();
                }
                if (preserveTabName !== 'Submitted' && cswPublic.submittedGrid) {
                    cswPublic.submittedGrid.ajax.abort();
                    cswPublic.submittedGrid.destroy();
                    cswPrivate.submittedTab.csw.empty();
                }
                if (preserveTabName !== 'Pending' && cswPublic.pendingGrid) {
                    cswPublic.pendingGrid.ajax.abort();
                    cswPublic.pendingGrid.destroy();
                    cswPrivate.pendingTab.csw.empty();
                }
            };

            cswPrivate.tabNames = ['Pending', 'Submitted', 'Recurring', 'Favorites'];

            cswPrivate.onTabSelect = function (tabName, el, eventObj, callBack) {
                var newTabName = tabName.split(' ')[0].trim(); //remove the '(#)' part from name
                if (newTabName.length > 0) {
                    cswPrivate.currentTab = newTabName;
                    cswPrivate.destroyOtherTabs(cswPrivate.currentTab);
                    switch (cswPrivate.currentTab) {
                        case 'Pending':
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
                    cswPrivate.getCartCounts();
                }
            };

            cswPrivate.makeCopyRecurringBtn = function (cswNode, cswGrid, onError) {
                var ret = cswNode.buttonExt({
                    enabledText: 'Copy to Recurring',
                    disabledText: 'Copy to Recurring',
                    disableOnClick: true,
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docexport),
                    onClick: function () {
                        var nodes = [];
                        cswGrid.getSelectedRowsVals('nodeid').forEach(function (nodeId) {
                            nodes.push({ NodeId: nodeId });
                        });
                        cswPrivate.copyRecurring(nodes, function () {
                            cswGrid.deselectAll();
                            onError();
                        });
                        ret.disable();
                    }
                }).disable();
                return ret;
            };

            cswPrivate.makeGridForTab = function (opts) {
                opts = opts || {
                    onSuccess: function () { },
                    onSelectChange: function () { }
                };

                return opts.parent.grid({
                    name: opts.gridId,
                    storeId: opts.gridId + '_store',
                    stateId: opts.gridId,
                    title: opts.title,
                    usePaging: true,
                    height: 350,
                    width: cswPrivate.tabs.getWidth() - 20,
                    ajax: {
                        urlMethod: 'getRequestItemGrid',
                        data: {
                            SessionViewId: opts.sessionViewId
                        }
                    },
                    showCheckboxes: opts.showCheckboxes,
                    showMultiEditToolbar: false,
                    showActionColumn: true,
                    canSelectRow: false,
                    reapplyViewReadyOnLayout: opts.reapplyViewReadyOnLayout,
                    groupField: opts.groupField || '',
                    onLoad: function (grid, json) {
                        Csw.tryExec(opts.onSuccess);
                    },
                    onEdit: function (rows) {
                        // this works for both Multi-edit and regular
                        var nodekeys = Csw.delimitedString(),
                            nodeids = Csw.delimitedString(),
                            nodenames = [],
                            currentNodeId, currentNodeKey;


                        Csw.each(rows, function (row) {
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
                            onEditNode: function () {
                                Csw.tryExec(opts.onEditNode); //Case 27619--don't pass the function by reference, because we want to control the parameters with which it is called
                                cswPrivate.getCartCounts();
                            }
                        });
                    }, // onEdit
                    onDelete: function (rows) {
                        // this works for both Multi-edit and regular
                        var nodes = [];

                        Csw.iterate(rows, function (row) {
                            nodes.push({
                                nodeid: row.nodeid,
                                nodename: row.nodename,
                                nodekey: row.nodekey
                            });
                        });

                        $.CswDialog('DeleteNodeDialog', {
                            nodes: nodes,
                            onDeleteNode: Csw.method(function () {
                                Csw.tryExec(opts.onEditNode);
                                cswPrivate.getCartCounts();
                            }),
                            Multi: (nodes.length > 1),
                            publishDeleteEvent: false
                        });
                    }, // onDelete
                    onBeforeSelect: opts.onBeforeSelect,
                    onSelectChange: function (rowCount) {
                        if (opts.showCheckboxes) {
                            Csw.tryExec(opts.onSelectChange, rowCount);
                        }
                    },
                    onPreview: function (o, nodeObj, event) {
                        var preview = Csw.nbt.nodePreview(Csw.main.body, {
                            nodeid: nodeObj.nodeid,
                            nodekey: nodeObj.nodekey,
                            nodename: nodeObj.nodename,
                            event: event
                        });
                        preview.open();
                    }
                });
            };

            cswPrivate.prepTab = function (tab, title, headerText) {

                tab.csw.empty().css({ margin: '10px' });

                var ol = tab.csw.ol();

                ol.li().span({
                    text: headerText
                });
                ol.li().br({ number: 2 });

                return ol;
            };

            cswPrivate.makePendingTab = function (opts) {
                var ol = cswPrivate.prepTab(cswPrivate.pendingTab, 'Request Items Pending Submission', 'Edit any of the Request Items in your cart. When you are finished, click "Submit these Requests" to submit the items in your cart.');

                var inpTbl = ol.li().table({
                    width: '99%',
                    cellpadding: '5px'
                });

                inpTbl.cell(1, 1).span().setLabelText('Request Name: ');
                inpTbl.cell(1, 1).input({
                    value: cswPrivate.requestName,
                    onChange: function (val) {
                        cswPrivate.requestName = val;
                    }
                });

                inpTbl.cell(1, 2)
                    .css({ 'text-align': 'right' })
                    .buttonExt({
                        enabledText: 'Request Create Material',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cart),
                        onClick: function () {
                            if (cswPublic.pendingGrid) {
                                cswPrivate.makeRequestCreateMaterial(cswPublic.pendingGrid);
                            }
                        }
                    });

                ol.li().br({ number: 1 });

                var hasOneRowSelected = false;
                var favoriteSelect;

                var toggleCopyButtons = function () {
                    if (hasOneRowSelected) {
                        copyBtn.enable();
                        if (favoriteSelect) {
                            cswPrivate.selectedFavoriteId = favoriteSelect.selectedNodeId();
                            if (cswPrivate.selectedFavoriteId) {
                                saveBtn.enable();
                            } else {
                                saveBtn.disable();
                            }
                        }
                    } else {
                        copyBtn.disable();
                        saveBtn.disable();
                    }
                };

                opts = opts || {};
                opts.sessionViewId = cswPrivate.state.pendingItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_pendingGrid';
                opts.showCheckboxes = true;
                opts.title = 'Select any Pending items to save to a Favorite list or to copy to a new Recurring request.';
                opts.onBeforeSelect = function (record, node) {
                    var ret = true;
                    if (record.raw.objectclassid != cswPrivate.state.copyableId) {
                        ret = false;
                    }
                    return ret;
                };
                opts.onSelectChange = function (rowCount) {
                    hasOneRowSelected = rowCount > 0;
                    toggleCopyButtons();
                };
                opts.reapplyViewReadyOnLayout = true;
                opts.onSuccess = function () {
                    cswPublic.pendingGrid.iterateRows(function (record, node) {
                        if (record.raw.objectclassid != cswPrivate.state.copyableId) {
                            $(node).find('.x-grid-row-checker').remove();
                        }
                    });
                };
                opts.onEditNode = cswPrivate.makePendingTab;

                cswPublic.pendingGrid = cswPrivate.makeGridForTab(opts);

                var btmTbl = ol.li().table({ cellpadding: '2px' });
                var saveBtn = btmTbl.cell(1, 1).buttonExt({
                    enabledText: 'Save to Favorites',
                    disabledText: 'Save to Favorites',
                    disableOnClick: true,
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                    onClick: function () {
                        var nodes = [];
                        cswPublic.pendingGrid.getSelectedRowsVals('nodeid').forEach(function (nodeId) {
                            nodes.push({ NodeId: nodeId });
                        });
                        cswPrivate.copyFavorites(cswPrivate.selectedFavoriteId, nodes, function () {
                            cswPublic.pendingGrid.deselectAll();
                            toggleCopyButtons();
                        });
                        saveBtn.disable();
                    }
                }).disable();

                var picklistCell = btmTbl.cell(1, 2);

                var makePicklist = function () {
                    picklistCell.empty();
                    favoriteSelect = picklistCell.nodeSelect({
                        selectedNodeId: cswPrivate.lastCreatedFavorite || '',
                        showSelectOnLoad: true,
                        usePreview: false,
                        viewid: cswPrivate.state.favoritesListViewId,
                        allowAdd: false,
                        onSelectNode: toggleCopyButtons,
                        onSuccess: toggleCopyButtons
                    });
                };

                makePicklist();
                btmTbl.cell(1, 3).buttonExt({
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                    onClick: function () {
                        Csw.tryExec(cswPrivate.addFavorite, makePicklist);
                    }
                });

                var copyBtn = cswPrivate.makeCopyRecurringBtn(btmTbl.cell(1, 4), cswPublic.pendingGrid, toggleCopyButtons);

                ol.li().br({ number: 2 });

                cswPrivate.addBtnGroup(ol.li(), true);

            }; // cswPrivate.makePendingTab()

            cswPrivate.makeSubmittedTab = function () {
                var ol = cswPrivate.prepTab(cswPrivate.submittedTab, 'Submitted Request Items', 'View any previously submitted Request Items.');

                Csw.nbt.viewFilters({
                    name: 'submitted_viewfilters',
                    isTreeView: false,
                    parent: ol.li(),
                    viewid: cswPrivate.state.submittedItemsViewId,
                    onEditFilters: function (newviewid) {
                        cswPrivate.state.submittedItemsViewId = newviewid;
                        cswPrivate.makeSubmittedTab();
                    } // onEditFilters
                }); // viewFilters

                ol.br();

                var hasOneRowSelected = false;
                var toggleCopyBtn = function () {
                    if (hasOneRowSelected) {
                        copyBtn.enable();
                    } else {
                        copyBtn.disable();
                    }
                };

                var opts = {
                    onSuccess: function () { }
                };
                opts.title = 'Select any Submitted items to copy to a new Recurring request.';
                opts.showCheckboxes = true;
                opts.sessionViewId = cswPrivate.state.submittedItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_submittedItemsGrid';
                opts.onSelectChange = function (rowCount) {
                    hasOneRowSelected = rowCount > 0;
                    toggleCopyBtn();
                };
                opts.onEditNode = cswPrivate.makeSubmittedTab;
                opts.onBeforeSelect = function (record, node) {
                    var ret = true;
                    if (record.raw.objectclassid != cswPrivate.state.copyableId) {
                        ret = false;
                    }
                    return ret;
                };
                opts.reapplyViewReadyOnLayout = true;
                opts.onSuccess = function () {
                    cswPublic.submittedGrid.iterateRows(function (record, node) {
                        if (record.raw.objectclassid != cswPrivate.state.copyableId) {
                            $(node).find('.x-grid-row-checker').remove();
                        }
                    });
                };

                cswPublic.submittedGrid = cswPrivate.makeGridForTab(opts);

                var btmTbl = ol.li().table({ cellpadding: '2px' });
                var copyBtn = cswPrivate.makeCopyRecurringBtn(btmTbl.cell(1, 1), cswPublic.submittedGrid, toggleCopyBtn);

                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), false);
            };

            cswPrivate.makeRecurringTab = function () {
                var ol = cswPrivate.prepTab(cswPrivate.recurringTab, 'Recurring Request Items', 'View your recurring Request Items.');
                Csw.nbt.viewFilters({
                    name: 'recurring_viewfilters',
                    isTreeView: false,
                    parent: ol.li(),
                    viewid: cswPrivate.state.recurringItemsViewId,
                    onEditFilters: function (newviewid) {
                        cswPrivate.state.recurringItemsViewId = newviewid;
                        cswPrivate.makeRecurringTab();
                    } // onEditFilters
                }); // viewFilters

                ol.br();

                var opts = {
                    onSuccess: function () { }
                };
                opts.title = 'Set the Recurring Frequency of any recurring item to schedule automatic reordering.';
                opts.showCheckboxes = false;
                opts.sessionViewId = cswPrivate.state.recurringItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_recurringItemsGrid';

                opts.onEditNode = cswPrivate.makeRecurringTab;

                cswPublic.recurringGrid = cswPrivate.makeGridForTab(opts);

                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), false);

            };

            cswPrivate.makeFavoritesTab = function () {
                var ol = cswPrivate.prepTab(cswPrivate.favoritesTab, 'Favorite Request Itens', 'View your favorite Request Items.');
                Csw.nbt.viewFilters({
                    name: 'favorites_viewfilters',
                    isTreeView: false,
                    parent: ol.li(),
                    viewid: cswPrivate.state.favoriteItemsViewId,
                    onEditFilters: function (newviewid) {
                        cswPrivate.state.favoriteItemsViewId = newviewid;
                        cswPrivate.makeFavoritesTab();
                    } // onEditFilters
                }); // viewFilters

                ol.br();

                var hasOneRowSelected = false;

                var toggleCopyBtn = function () {
                    if (hasOneRowSelected) {
                        copyBtn.enable();
                    } else {
                        copyBtn.disable();
                    }
                };

                var opts = {
                    onSuccess: function () { }
                };
                opts.title = 'Select any Favorite request to copy to your Pending cart.';
                opts.showCheckboxes = true;
                opts.sessionViewId = cswPrivate.state.favoriteItemsViewId;
                opts.parent = ol.li();
                opts.gridId = cswPrivate.name + '_favoriteItemsGrid';

                opts.onEditNode = cswPrivate.makeFavoritesTab;
                opts.onSelectChange = function (rowCount) {
                    hasOneRowSelected = rowCount > 0;
                    toggleCopyBtn();
                };
                cswPublic.favoritesGrid = cswPrivate.makeGridForTab(opts);

                ol.li().br();

                var copyBtn = ol.li().buttonExt({
                    enabledText: 'Copy to Current Cart',
                    disabledText: 'Copy to Current Cart',
                    disableOnClick: true,
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.copy),
                    onClick: function () {
                        var nodes = [];
                        cswPublic.favoritesGrid.getSelectedRowsVals('nodeid').forEach(function (nodeId) {
                            nodes.push({ NodeId: nodeId });
                        });
                        cswPrivate.copyFavorites(cswPrivate.state.pendingCartId, nodes, function () {
                            cswPublic.favoritesGrid.deselectAll();
                            toggleCopyBtn();
                        });
                        copyBtn.disable();
                    }
                }).disable();

                ol.li().br();

                cswPrivate.addBtnGroup(ol.li(), false);

            };

            cswPrivate.addBtnGroup = function (el, hasFinish) {
                var tbl = el.table({ width: '99%', cellpadding: '5px' });

                if (hasFinish) {
                    tbl.cell(1, 1).buttonExt({
                        enabledText: 'Submit these Requests',
                        disabledText: 'Submit these Requests',
                        disableOnClick: true,
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.check),
                        onClick: function () {
                            cswPrivate.submitRequest();
                            cswPrivate.clearState();
                        }
                    });
                }
                tbl.cell(1, 2).css({ 'text-align': 'right' }).buttonExt({
                    enabledText: 'Close',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cancel),
                    onClick: function () {
                        Csw.tryExec(cswPrivate.onCancel);
                        cswPrivate.clearState();
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
                    title: 'Pending (' + cswPrivate.state.cartCounts.PendingRequestItems + ')'
                });

                cswPrivate.submittedTab = cswPrivate.tabs.addTab({
                    title: 'Submitted (' + cswPrivate.state.cartCounts.SubmittedRequestItems + ')'
                });

                cswPrivate.recurringTab = cswPrivate.tabs.addTab({
                    title: 'Recurring (' + cswPrivate.state.cartCounts.RecurringRequestItems + ')'
                });

                cswPrivate.favoritesTab = cswPrivate.tabs.addTab({
                    title: 'Favorites (' + cswPrivate.state.cartCounts.FavoriteRequestItems + ')'
                });

                cswPrivate.tabs.setActiveTab(0);
                //cswPrivate.openTab('Pending');

                cswPrivate.getCart();

            }());

            return cswPublic;

            //#endregion _postCtor
        });
}());