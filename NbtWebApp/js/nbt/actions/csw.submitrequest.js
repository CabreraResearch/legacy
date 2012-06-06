/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.submitRequest = Csw.actions.submitRequest ||
        Csw.actions.register('submitRequest', function (cswParent, options) {
            'use strict';
            var cswPublic = {};
            var cswPrivate = {
                urlMethod: 'getCurrentRequest',
                ID: 'CswSubmitRequest',
                onSubmit: null,
                onCancel: null,
                gridOpts: {}
            };

            if (Csw.isNullOrEmpty(cswParent)) {
                Csw.error.throwException('Cannot create a Submit Request action without a valid Csw Parent object.', 'Csw.actions.submitRequest', 'csw.submitrequest.js', 14);
            }
            Csw.tryExec(function () {
                if (options) {
                    $.extend(true, cswPrivate, options);
                }

                cswParent.empty();
                cswPrivate.action = Csw.layouts.action(cswParent, {
                    Title: 'Submit Request',
                    FinishText: 'Submit',
                    onFinish: cswPrivate.onSubmit,
                    onCancel: cswPrivate.onCancel
                });
                cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({ ID: cswPrivate.ID + '_tbl' });

                cswPrivate.gridId = cswPrivate.ID + '_csw_requestGrid_outer';
                cswPublic.gridParent = cswPrivate.actionTbl.cell(1, 1).div({ ID: cswPrivate.gridId });

                Csw.ajax.post({
                    urlMethod: cswPrivate.urlMethod,
                    data: {},
                    success: function (gridJson) {
                        if (Csw.isNullOrEmpty(gridJson.jqGridOpt)) {
                            Csw.error.throwException('The Submit Request action encountered an error attempting to render the grid.', 'Csw.actions.submitRequest', 'csw.submitrequest.js', 68);
                        }
                        Csw.tryExec(function () {
                            $.extend(true, cswPrivate.gridOpts, gridJson.jqGridOpt);
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
                            cswPrivate.gridOpts.data = gridJson.data.rows;

                            cswPublic.grid = cswPublic.gridParent.grid(cswPrivate);
                            Csw.nbt.gridViewMethods.makeActionColumnButtons(cswPublic.grid);
                        });
                    } // success
                });

            });
            return cswPublic;
        });
} ());

