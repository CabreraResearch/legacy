/// <reference path="~/app/CswApp-vsdoc.js" />


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
            Csw.tryExec(function () {
                if (options) {
                    Csw.extend(cswPrivate, options, true);
                }
                var submitRequest = function () {
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

                cswParent.empty();
                cswPrivate.action = Csw.layouts.action(cswParent, {
                    Title: 'Submit Request',
                    FinishText: 'Place Request',
                    onFinish: submitRequest,
                    onCancel: cswPrivate.onCancel
                });

                cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({
                    name: cswPrivate.name + '_tbl',
                    align: 'center'
                }).css('width', '95%');

                cswPrivate.actionTbl.cell(1, 1)
                    .css('text-align', 'left')
                    .span({ text: 'Edit any of the Request Items in your cart. When you are finished, click "Place Request" to submit your cart.' });


                cswPrivate.actionTbl.cell(3, 1).br({ number: 2 });
                cswPrivate.gridId = cswPrivate.name + '_csw_requestGrid_outer';
                cswPublic.gridParent = cswPrivate.actionTbl.cell(4, 1).div({
                    name: cswPrivate.gridId
                }); //, align: 'center' });

                cswPrivate.initGrid = function (opts) {

                    // This really ought to be a CswNodeGrid
                    opts = opts || {
                        urlMethod: 'getCurrentRequest',
                        data: {},
                        onSuccess: null 
                    };

                    var gridId = cswPrivate.name + '_srgrid';
                    cswPublic.grid = cswPublic.gridParent.grid({
                        name: gridId,
                        storeId: gridId + '_store',
                        stateId: gridId,
                        title: 'Your Cart',
                        usePaging: true,
                        height: 180,

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
                            var cswnbtnodekeys = [],
                                nodeids = [],
                                nodenames = [];

                            Csw.each(rows, function (row) {
                                cswnbtnodekeys.push(row.nodekey);
                                nodeids.push(row.nodeid);
                                nodenames.push(row.nodename);
                            });

                            $.CswDialog('EditNodeDialog', {
                                nodeids: nodeids,
                                nodepks: nodeids,
                                nodekeys: cswnbtnodekeys,
                                nodenames: nodenames,
                                Multi: (nodeids.length > 1),
                                onEditNode: function () {
                                    cswPrivate.initGrid(); //Case 27619--don't pass the function by reference, because we want to control the parameters with which it is called
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
                                    cswPrivate.initGrid();
                                }),
                                Multi: (nodeids.length > 1),
                                publishDeleteEvent: false
                            });
                        }, // onDelete
                        onSelect: null, // function(row)
                        onDeselect: null // function(row)
                    });
                }; // initGrid()

                cswPrivate.initGrid();

                cswPrivate.copyRequest = function () {
                    cswPrivate.initGrid({
                        urlMethod: 'copyRequest',
                        data: {
                            CopyFromRequestId: cswPrivate.copyFromNodeId,
                            CopyToRequestId: cswPrivate.cartnodeid
                        },
                        onSuccess: function() {
                            Csw.publish(Csw.enums.events.main.refreshHeader);
                        }
                    });
                }; // copyRequest()

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
                });

                cswPrivate.saveRequestTbl = cswPrivate.actionTbl.cell(6, 1).table({ align: 'right', cellvalign: 'middle', cellpadding: '2px' });
                //cswPrivate.saveRequestTbl.cell(1, 4).span({ text: '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' });
                cswPrivate.saveRequestTxt = cswPrivate.saveRequestTbl.cell(2, 1).input().hide();
                cswPrivate.saveRequestTbl.cell(1, 1).span({ text: 'Save Request' });
                cswPrivate.saveRequestChk = cswPrivate.saveRequestTbl.cell(1, 2).checkBox({
                    onChange: Csw.method(function () {
                        var val;
                        if (cswPrivate.saveRequestChk.checked()) {
                            val = Csw.cookie.get(Csw.cookie.cookieNames.Username) + ' ' + Csw.todayAsString();
                            cswPrivate.saveRequestTxt.show();
                        } else {
                            val = '';
                            cswPrivate.saveRequestTxt.hide();
                        }
                        cswPrivate.saveRequestTxt.val(val);
                    })
                });

            });
            return cswPublic;
        });
}());

