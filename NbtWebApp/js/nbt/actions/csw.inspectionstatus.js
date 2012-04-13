/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.inspectionStatus = Csw.actions.inspectionStatus ||
        Csw.actions.register('inspectionStatus', function (cswParent, options) {
            'use strict';
            var internal = {
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
                            onEditNode: internal.onEditNode,
                            onAfterButtonClick: internal.onAfterButtonClick
                        };
                        if (false === Csw.isNullOrEmpty(rowid)) {
                            editOpt.nodeids.push(external.grid.getValueForColumn('NODEPK', rowid));
                            $.CswDialog('EditNodeDialog', editOpt);
                        } else {
                            $.CswDialog('AlertDialog', 'Please select a row to edit');
                        }
                    }
                }
                /* End Csw.grid specific options */
            };
            if (options) {
                $.extend(internal, options);
            }

            var external = {};

            Csw.ajax.post({
                urlMethod: internal.urlMethod,
                data: {},
                success: function (gridJson) {

                    cswParent.empty();
                    var inspGridId = internal.ID + '_csw_inspGrid_outer';

                    external.gridParent = cswParent.div({ ID: inspGridId });

                    $.extend(internal.gridOpts, gridJson);

                    external.grid = external.gridParent.grid(internal);
                    external.grid.hideColumn('NODEID');
                    external.grid.hideColumn('NODEPK');

                }, // success
                error: function () { }
            });
            return external;
        });
} ());

