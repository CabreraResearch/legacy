
(function () {

    Csw.actions.submitRequest = Csw.actions.submitRequest ||
        Csw.actions.register('submitRequest', function (cswParent, options) {
            'use strict';
            var cswPublic = {};
            var cswPrivate = {
                name: 'CswSubmitRequest',
                onSubmit: null,
                onCancel: null,
                gridOpts: {},
                cartnodeid: '',
                cartviewid: '',
                materialnodeid: '',
                containernodeid: ''
            };

            if (Csw.isNullOrEmpty(cswParent)) {
                Csw.error.throwException('Cannot create a Submit Request action without a valid Csw Parent object.', 'Csw.actions.submitRequest', 'csw.submitrequest.js', 14);
            }
            (function _preCtor() {
                Csw.extend(cswPrivate, options, true);

                cswParent.empty();
            }());

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
                Csw.ajax.post({
                    urlMethod: 'submitRequest',
                    data: {
                        RequestId: cswPrivate.cartnodeid,
                        RequestName: Csw.string(cswPrivate.saveRequestTxt.val())
                    },
                    success: function (json) {
                        if (json.succeeded) {
                            Csw.tryExec(cswPrivate.onSubmit);
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

                cswPrivate.pendingTab.csw.empty();
                
                var pendingTbl = cswPrivate.pendingTab.csw.table({
                    TableCssClass: 'CswAction_ContentTable'
                });
                
                pendingTbl.cell(1, 1).label({ text: 'Edit any of the Request Items in your cart. When you are finished, click "Place Request" to submit your cart.' });
                pendingTbl.cell(1, 1).br({ number: 2 });

                pendingTbl.cell(2, 1).input({
                    labelText: 'Request Name: ',
                    value: Csw.cookie.get(Csw.cookie.cookieNames.Username) + ' ' + Csw.todayAsString(),
                    onChange: function(val) {
                        cswPrivate.requestName = val;
                    }
                });

                pendingTbl.cell(2, 1).br({ number: 2 });

                cswPublic.grid = pendingTbl.cell(3, 1).grid({
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

                    showCheckboxes: false,
                    showActionColumn: true,
                    canSelectRow: false,

                    onLoad: function (grid, json) {
                        cswPrivate.cartnodeid = json.cartnodeid;
                        cswPrivate.cartviewid = json.cartviewid;
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
                    onSelect: null, // function(row)
                    onDeselect: null, // function(row)

                    topToolbar: [cswPrivate.requestCreateMaterial]
                });
            }; // initGrid()

            cswPrivate.copyRequest = function () {
                cswPrivate.makePendingTab({
                    urlMethod: 'copyRequest',
                    data: {
                        CopyFromRequestId: cswPrivate.copyFromNodeId,
                        CopyToRequestId: cswPrivate.cartnodeid
                    },
                    onSuccess: function () {
                        Csw.publish(Csw.enums.events.main.refreshHeader);
                    }
                });
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
                    Title: 'Requests',
                    FinishText: 'Place Request',
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