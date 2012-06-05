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
                        autowidth: true,
                        rowNum: 10
                    },
                    canEdit: true,
                    pagermode: 'default',
                    optNav: {
                        add: false,
                        view: false,
                        del: false,
                        refresh: false,
                        edit: true,
                        edittext: "",
                        edittitle: "Edit row"
                    },
                    optNavEdit: {
                        editfunc: function (rowid) {
                            var editOpt = {
                                nodeids: [],
                                nodenames: [],
                                onEditNode: cswPrivate.onEditNode,
                                onAfterButtonClick: cswPrivate.onAfterButtonClick
                            };
                            if (false === Csw.isNullOrEmpty(rowid)) {
                                editOpt.nodeids.push(cswPublic.grid.getValueForColumn('NODEPK', rowid));
                                editOpt.nodenames.push(cswPublic.grid.getValueForColumn('INSPECTION', rowid));
                                $.CswDialog('EditNodeDialog', editOpt);
                            } else {
                                $.CswDialog('AlertDialog', 'Please select a row to edit');
                            }
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
                            var inspGridId = cswPrivate.ID + '_csw_inspGrid_outer';

                            cswPublic.gridParent = cswParent.div({ ID: inspGridId });

                            $.extend(true, cswPrivate.gridOpts, gridJson.jqGridOpt);
                            cswPrivate.gridOpts.data = gridJson.data.rows;

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
                Csw.catchException(exception);
            }
            return cswPublic;
        });
} ());

