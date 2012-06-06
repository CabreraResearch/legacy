/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.submitRequest = Csw.actions.submitRequest ||
        Csw.actions.register('submitRequest', function (cswParent, options) {
            'use strict';
            var cswPublic = {};
            var cswPrivate = {};
            try {
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException('Cannot create a Submit Request action without a valid Csw Parent object.', 'Csw.actions.submitRequest', 'csw.submitrequest.js', 14);
                }
                cswPrivate.urlMethod = 'getCurrentRequest';
                cswPrivate.ID = 'CswSubmitRequest';

                if (options) {
                    $.extend(cswPrivate, options);
                }

                cswParent.empty();

                cswPrivate = {
                    urlMethod: 'getCurrentRequest',
                    ID: 'CswSubmitRequest',
                    /* Csw.grid specific options */
                    gridOpts: {
                        autowidth: false,
                        rowNum: 10,
                        width: 600,
                        height: 'auto'
                    },
                    pagermode: 'default',
                    optNav: {
                        add: false,
                        view: false,
                        del: false,
                        refresh: false,
                        edit: false
                    },
                    optNavEdit: {
                        editfunc: function (rowid) {

                        }
                    }
                    /* End Csw.grid specific options */
                };

                Csw.ajax.post({
                    urlMethod: cswPrivate.urlMethod,
                    data: {},
                    success: function (gridJson) {
                        try {
                            if (Csw.isNullOrEmpty(gridJson.jqGridOpt)) {
                                Csw.error.throwException('The Submit Request action encountered an error attempting to render the grid.', 'Csw.actions.submitRequest', 'csw.submitrequest.js', 68);
                            }
                            cswParent.empty();
                            var inspGridId = cswPrivate.ID + '_csw_requestGrid_outer';

                            cswPrivate.actionTbl = cswParent.table({ ID: cswPrivate.ID + '_tbl' }).css({'text-align': 'center'});
                            cswPrivate.actionTbl.cell(3, 3);

                            $.extend(true, cswPrivate.gridOpts, gridJson.jqGridOpt);
                            cswPrivate.gridOpts.data = gridJson.data.rows;

                            cswPublic.gridParent = cswPrivate.actionTbl.cell(2, 2).div({ ID: inspGridId });

                            cswPublic.grid = cswPublic.gridParent.grid(cswPrivate);
                            Csw.nbt.gridViewMethods.makeActionColumnButtons(cswPublic.grid);
                        }
                        catch (exception) {
                            Csw.error.catchException(exception);
                        }
                    } // success
                });


            }
            catch (exception) {
                Csw.error.catchException(exception);
            }
            return cswPublic;
        });
} ());

