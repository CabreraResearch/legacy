
(function () {

    Csw.actions.submitRequest = Csw.actions.submitRequest ||
        Csw.actions.register('submitRequest', function (cswParent, cswPrivate) {
            'use strict';
            var cswPublic = {};
            
            if (Csw.isNullOrEmpty(cswParent)) {
                Csw.error.throwException('Cannot create a Submit Request action without a valid Csw Parent object.', 'Csw.actions.submitRequest', 'csw.submitrequest.js', 14);
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

            cswPrivate.makePendingTab = function (opts) {
                cswPrivate.action.finish.enable();
                cswPrivate.tabs.setTitle('Request Items Pending Submission');

                // This really ought to be a CswNodeGrid
                opts = opts || {
                    urlMethod: 'getCurrentRequest',
                    data: {},
                    onSuccess: null
                };

                var gridId = cswPrivate.name + '_srgrid';

                cswPrivate.pendingTab.csw.empty().css({margin: '10px'});
                
                var ol = cswPrivate.pendingTab.csw.ol();

                ol.li().span({
                    text: 'Edit any of the Request Items in your cart. When you are finished, click "Place Request" to submit your cart.'
                });
                ol.li().br({ number: 2 });
                
                var inpTbl = ol.li().table({ width: '100%' });
                
                inpTbl.cell(1,1).ol().li().input({
                    labelText: 'Request Name:',
                    value: cswPrivate.requestName,
                    onChange: function(val) {
                        cswPrivate.requestName = val;
                    }
                });

                inpTbl.cell(1, 2)
                    .css({ 'text-align': 'center' })
                    .buttonExt({
                    enabledText: 'Request Create Material',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cart),
                    onClick: cswPrivate.makeRequestCreateMaterial
                });
                
                ol.li().br({ number: 1 });

                cswPublic.grid = ol.li().grid({
                    name: gridId,
                    storeId: gridId + '_store',
                    stateId: gridId,
                    usePaging: true,
                    height: 180,
                    width: cswPrivate.tabs.getWidth() - 20,
                    ajax: {
                        urlMethod: opts.urlMethod,
                        data: opts.data
                    },

                    showCheckboxes: true,
                    showActionColumn: true,
                    canSelectRow: false,

                    onLoad: function (grid, json) {
                        cswPrivate.cartnodeid = json.cartnodeid;
                        cswPrivate.cartviewid = json.cartviewid;
                        Csw.tryExec(opts.onSuccess);
                        window.requestGrid = cswPublic.grid;
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
                                cswPrivate.makePendingTab(); //Case 27619--don't pass the function by reference, because we want to control the parameters with which it is called
                            }
                        });
                    }, // onEdit
                    onDelete: function (rows) {
                        // this works for both Multi-edit and regular
                        var cswnbtnodekeys = [],
                            nodeids = [],
                            nodenames = [];

                        Csw.each(rows, function (row) {
                            cswnbtnodekeys.push(row.nodekey);
                            nodeids.push(row.nodeid);
                            nodenames.push(row.nodename);
                        });

                        $.CswDialog('DeleteNodeDialog', {
                            nodeids: nodeids,
                            nodepks: nodeids,
                            nodekeys: cswnbtnodekeys,
                            nodenames: nodenames,
                            onDeleteNode: Csw.method(function () {
                                Csw.publish(Csw.enums.events.main.refreshHeader);
                                cswPrivate.makePendingTab();
                            }),
                            Multi: (nodeids.length > 1),
                            publishDeleteEvent: false
                        });
                    }, // onDelete
                    onSelect: function(row) {}, 
                    onDeselect: function(row) {}
                });

                ol.li().br();
                
                var btmTbl = ol.li().table();
                btmTbl.cell(1, 1).buttonExt({
                    enabledText: 'Save to Favorites',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                    onClick: cswPrivate.saveToFavorites
                });

                Csw.ajaxWcf.get({
                    urlMethod: 'Requests/cart',
                    success: function(data) {
                        var picklistCell = btmTbl.cell(1, 2);
                        var makePicklist = function () {
                            picklistCell.empty();
                            picklistCell.nodeSelect({
                                width: '50px',
                                selectedNodeId: cswPrivate.lastCreatedFavorite || '',
                                showSelectOnLoad: true,
                                viewid: data.FavoriteItemsViewId,
                                allowAdd: false
                            });
                        };
                        makePicklist();
                        btmTbl.cell(1, 3).buttonExt({
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.add),
                            onClick: function() {
                                Csw.tryExec(cswPrivate.addFavorite, makePicklist);
                            }
                        });
                    }
                });

                
            }; // cswPrivate.makePendingTab()

            cswPrivate.saveToFavorites = function () {
                //cswPrivate.makePendingTab({
                //    urlMethod: 'copyRequest',
                //    data: {
                //        CopyFromRequestId: cswPrivate.copyFromNodeId,
                //        CopyToRequestId: cswPrivate.cartnodeid
                //    },
                //    onSuccess: function () {
                //        Csw.publish(Csw.enums.events.main.refreshHeader);
                //    }
                //});
            }; // copyRequest()    

            cswPrivate.onTabSelect = function (tabName, el, eventObj, callBack) {
                switch (tabName) {
                    case 'Pending':
                        cswPrivate.action.finish.enable();
                        cswPrivate.tabs.setTitle('Request Items Pending Submission');
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


                /*
            
            cswPrivate.actionTbl.cell(3, 1).br({ number: 2 });
      cswPrivate.gridId = cswPrivate.name + '_csw_requestGrid_outer';
      cswPublic.gridParent = cswPrivate.actionTbl.cell(4, 1).div({
          name: cswPrivate.gridId
      }); //, align: 'center' });

      cswPrivate.initGrid();

      cswPrivate.historyTbl = cswPrivate.actionTbl.cell(5, 1).table({ align: 'left', cellvalign: 'middle' });
      Csw.ajax.post({
          urlMethod: 'getRequestHistory',
          data: {},
          success: function (json) {
              if (json.count > 0) {
                  delete json.count;
                  cswPrivate.historyTbl.cell(1, 1).span({ text: '</br>&nbsp;Past Requests: ' });
                  cswPrivate.historySelect = cswPrivate.historyTbl.cell(2, 1).select({
                      onChange: function () {
                          cswPrivate.copyFromNodeId = cswPrivate.historySelect.val();
                      }
                  });
                  json = json || {};

                  Csw.each(json, function (prop, name) {
                      var display = Csw.string(prop['name']) + ' (' + Csw.string(prop['submitted date']) + ')';
                      cswPrivate.historySelect.option({ value: prop['requestnodeid'], display: display });
                  });
                  cswPrivate.copyFromNodeId = cswPrivate.historySelect.val();
                  cswPrivate.copyHistoryBtn = cswPrivate.historyTbl.cell(2, 2).buttonExt({
                      icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.copy),
                      size: 'small',
                      enabledText: 'Copy to Cart',
                      disabledText: 'Copying...',
                      onClick: cswPrivate.copyRequest
                  });
              }
          }
                   
            
            */

            };

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

                cswPrivate.makePendingTab();

            }());

            return cswPublic;
        });
}());