/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.inspectionStatus = Csw.actions.inspectionStatus ||
        Csw.actions.register('inspectionStatus', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
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
                            onEditNode: cswPrivateVar.onEditNode,
                            onAfterButtonClick: cswPrivateVar.onAfterButtonClick
                        };
                        if (false === Csw.isNullOrEmpty(rowid)) {
                            editOpt.nodeids.push(cswPublicRet.grid.getValueForColumn('NODEPK', rowid));
                            editOpt.nodenames.push(cswPublicRet.grid.getValueForColumn('INSPECTION', rowid));
                            $.CswDialog('EditNodeDialog', editOpt);
                        } else {
                            $.CswDialog('AlertDialog', 'Please select a row to edit');
                        }
                    }
                }
                /* End Csw.grid specific options */
            };
            if (options) {
                $.extend(cswPrivateVar, options);
            }

            var cswPublicRet = {};

            Csw.ajax.post({
                urlMethod: cswPrivateVar.urlMethod,
                data: {},
                success: function (gridJson) {

                    cswParent.empty();
                    var inspGridId = cswPrivateVar.ID + '_csw_inspGrid_outer';

                    cswPublicRet.gridParent = cswParent.div({ ID: inspGridId });

                    $.extend(cswPrivateVar.gridOpts, gridJson);

                    cswPublicRet.grid = cswPublicRet.gridParent.grid(cswPrivateVar);
                    cswPublicRet.grid.hideColumn('NODEID');
                    cswPublicRet.grid.hideColumn('NODEPK');

                }, // success
                error: function () { }
            });
            return cswPublicRet;
        });
} ());

