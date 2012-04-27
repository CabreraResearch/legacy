/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.auditHistory = Csw.actions.auditHistory ||
        Csw.actions.register('auditHistory', function (cswParent, options) {
            'use strict';

            var internal = {
                urlMethod: 'getAuditHistoryGrid',
                ID: '',
                nodeid: '',
                EditMode: Csw.enums.editMode.Edit,
                JustDateColumn: false,
                onEditRow: null, //function (date) {},
                onSelectRow: null, //function (date) {},
                selectedDate: '',
                allowEditRow: true,
                preventSelectTrigger: false,
                canEdit: true,
                pagermode: 'default',
                gridOpts: {
                    height: 180,
                    rowNum: 10,
                    onSelectRow: function (selRowid) {
                        if (false === internal.preventSelectTrigger && false === Csw.isNullOrEmpty(selRowid)) {
                            var cellVal = external.grid.getValueForColumn('CHANGEDATE', selRowid);
                            Csw.tryExec(internal.onSelectRow, cellVal);
                        }
                    },
                    add: false,
                    view: false,
                    del: false,
                    refresh: false,
                    search: false
                }
            };
            if (options) {
                $.extend(internal, options);
            }
            var external = { };
            cswParent.empty();

            Csw.ajax.post({
                urlMethod: internal.urlMethod,
                data: {
                    NodeId: Csw.string(internal.nodeid),
                    NbtNodeKey: Csw.string(internal.cswnbtnodekey),
                    JustDateColumn: internal.JustDateColumn
                },
                success: function (gridJson) {

                    var auditGridId = internal.ID + '_csw_auditGrid_outer';

                    internal.gridDiv = cswParent.div({ ID: auditGridId });

                    if (Csw.contains(gridJson, 'jqGridOpt')) {

                        $.extend(internal.gridOpts, gridJson.jqGridOpt);

                        if (internal.EditMode === Csw.enums.editMode.PrintReport) {
                            internal.gridOpts.caption = '';
                            internal.hasPager = false;
                        } else {
                            internal.optNavEdit = {
                                editfunc: function (selRowid) {
                                    if (false === Csw.isNullOrEmpty(selRowid)) {
                                        var cellVal = external.grid.getValueForColumn('CHANGEDATE', selRowid);
                                        if (Csw.isFunction(internal.onEditRow)) {
                                            internal.onEditRow(cellVal);
                                        }
                                    } else {
                                        $.CswDialog('AlertDialog', 'Please select a row to edit');
                                    }
                                }
                            };
                        }

                        external.grid = internal.gridDiv.grid(internal);
                        external.grid.gridPager.css({ width: '100%', height: '20px' });

                        // set selected row by date

                        if (false === Csw.isNullOrEmpty(internal.selectedDate)) {
                            internal.preventSelectTrigger = true;
                            var rowid = external.grid.getRowIdForVal('CHANGEDATE', internal.selectedDate.toString());
                            external.grid.setSelection(rowid);
                            internal.preventSelectTrigger = false;
                        }
                    }
                }
            });


        });

} ());
