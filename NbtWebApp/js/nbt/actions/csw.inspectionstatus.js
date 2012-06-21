/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.inspectionStatus = Csw.actions.inspectionStatus ||
        Csw.actions.register('inspectionStatus', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                urlMethod: 'getInspectionStatusGrid',
                onEditNode: function () { },
                onAfterButtonClick: null,
                ID: 'CswInspectionStatus',
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
            if (options) {
                $.extend(cswPrivate, options);
            }

            var cswPublic = {};

            Csw.ajax.post({
                urlMethod: cswPrivate.urlMethod,
                data: {},
                success: function (gridJson) {

                    cswParent.empty();
                    var inspGridId = cswPrivate.ID + '_csw_inspGrid_outer';

                    cswPublic.gridParent = cswParent.div({ ID: inspGridId });

                    $.extend(cswPrivate.gridOpts, gridJson);

                    cswPublic.grid = cswPublic.gridParent.grid(cswPrivate);
//                    cswPublic.grid.hideColumn('NODEID');
//                    cswPublic.grid.hideColumn('NODEPK');

                }, // success
                error: function () { }
            });
            return cswPublic;
        });
} ());

