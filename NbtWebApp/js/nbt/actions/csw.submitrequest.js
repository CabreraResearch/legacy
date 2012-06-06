/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.submitRequest = Csw.actions.submitRequest ||
        Csw.actions.register('submitRequest', function (cswParent, options) {
            'use strict';
            var cswPublic = {};
            var cswPrivate = {
                ID: 'CswSubmitRequest',
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
                    $.extend(true, cswPrivate, options);
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
                    FinishText: 'Submit',
                    onFinish: submitRequest,
                    onCancel: cswPrivate.onCancel
                });
                cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({ ID: cswPrivate.ID + '_tbl' }).css('width', '100%');

                cswPrivate.gridId = cswPrivate.ID + '_csw_requestGrid_outer';
                cswPublic.gridParent = cswPrivate.actionTbl.cell(1, 1).div({ ID: cswPrivate.gridId, align: 'center' });

                Csw.ajax.post({
                    urlMethod: 'getCurrentRequest',
                    data: {},
                    success: function (json) {
                        if (Csw.isNullOrEmpty(json.jqGridOpt)) {
                            Csw.error.throwException('The Submit Request action encountered an error attempting to render the grid.', 'Csw.actions.submitRequest', 'csw.submitrequest.js', 68);
                        }
                        Csw.tryExec(function () {
                            cswPrivate.cartnodeid = json.cartnodeid;
                            cswPrivate.cartviewid = json.cartviewid;
                            if (false === Csw.isNullOrEmpty(cswPrivate.materialnodeid) ||
                                false === Csw.isNullOrEmpty(cswPrivate.containernodeid)) {

                                cswPrivate.actionTbl.cell(2, 1).$.CswMenuMain({
                                    nodeid: cswPrivate.cartnodeid,
                                    viewid: cswPrivate.cartviewid,
                                    limitMenuTo: 'Add'
                                });
                            }
                            $.extend(true, cswPrivate.gridOpts, json.jqGridOpt);
                            cswPrivate.resizeWithParent = true;
                            cswPrivate.resizeWithParentElement = cswPrivate.action.actionDiv.$;
                            cswPrivate.gridOpts.rowNum = 10;
                            cswPrivate.gridOpts.height = 180;
                            cswPrivate.gridOpts.caption = 'Your Cart';
                            cswPrivate.gridOpts.pagermode = 'default';
                            cswPrivate.gridOpts.optNav = {
                                add: false,
                                view: false,
                                del: false,
                                refresh: false,
                                edit: false
                            };
                            cswPrivate.gridOpts.data = json.data.rows;

                            cswPublic.grid = cswPublic.gridParent.grid(cswPrivate);
                            cswPublic.grid.gridPager.css({ width: '100%', height: '20px' });

                            Csw.nbt.gridViewMethods.makeActionColumnButtons(cswPublic.grid);
                        });
                    } // success
                });


                cswPrivate.historyTbl = cswPrivate.actionTbl.cell(3, 1).table({ align: 'left', cellvalign: 'middle' });
                Csw.ajax.post({
                    urlMethod: 'getRequestHistory',
                    data: {},
                    success: function (json) {
                        if (json.count > 0) {
                            delete json.count;
                            cswPrivate.historyTbl.cell(1, 1).span({ text: '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Past Requests: ' });
                            cswPrivate.historySelect = cswPrivate.historyTbl.cell(1, 2).select();
                            json = json || {};

                            Csw.each(json, function (prop, name) {
                                var display = Csw.string(prop['name']) + ' (' + Csw.string(prop['submitted date']) + ')';
                                cswPrivate.historySelect.option({ value: prop['requestnodeid'], display: display });
                            });
                            cswPrivate.copyHistoryBtn = cswPrivate.historyTbl.cell(1, 3).button({
                                enabledText: 'Copy to Cart',
                                disabledText: 'Copying...',
                                onclick: function () {
                                    /*Copy Cart Contents*/
                                }
                            });
                        }
                    }
                });

                cswPrivate.saveRequestTbl = cswPrivate.actionTbl.cell(4, 1).table({ align: 'right', cellvalign: 'middle', cellpadding: '2px' });
                cswPrivate.saveRequestTbl.cell(1, 4).span({ text: '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' });
                cswPrivate.saveRequestTxt = cswPrivate.saveRequestTbl.cell(1, 3).input().hide();
                cswPrivate.saveRequestTbl.cell(1, 1).span({ text: 'Save Request' });
                cswPrivate.saveRequestChk = cswPrivate.saveRequestTbl.cell(1, 2).checkBox({
                    onChange: function () {
                        var val;
                        if (cswPrivate.saveRequestChk.checked()) {
                            val = Csw.cookie.get(Csw.cookie.cookieNames.Username) + ' ' + Csw.nowAsString();
                            cswPrivate.saveRequestTxt.show();
                        } else {
                            val = '';
                            cswPrivate.saveRequestTxt.hide();
                        }
                        cswPrivate.saveRequestTxt.val(val);
                    }
                });

            });
            return cswPublic;
        });
} ());

