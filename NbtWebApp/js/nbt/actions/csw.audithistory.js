/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.auditHistory = Csw.actions.auditHistory ||
        Csw.actions.register('auditHistory', function (cswParent, options) {
            'use strict';

            var cswPrivateVar = {
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
                        if (false === cswPrivateVar.preventSelectTrigger && false === Csw.isNullOrEmpty(selRowid)) {
                            var cellVal = cswPublicRet.grid.getValueForColumn('CHANGEDATE', selRowid);
                            Csw.tryExec(cswPrivateVar.onSelectRow, cellVal);
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
                $.extend(cswPrivateVar, options);
            }
            var cswPublicRet = { };
            cswParent.empty();

            Csw.ajax.post({
                urlMethod: cswPrivateVar.urlMethod,
                data: {
                    NodeId: Csw.string(cswPrivateVar.nodeid),
                    NbtNodeKey: Csw.string(cswPrivateVar.cswnbtnodekey),
                    JustDateColumn: cswPrivateVar.JustDateColumn
                },
                success: function (gridJson) {

                    var auditGridId = cswPrivateVar.ID + '_csw_auditGrid_outer';

                    cswPrivateVar.gridDiv = cswParent.div({ ID: auditGridId });

                    if (Csw.contains(gridJson, 'jqGridOpt')) {

                        $.extend(cswPrivateVar.gridOpts, gridJson.jqGridOpt);

                        if (cswPrivateVar.EditMode === Csw.enums.editMode.PrintReport) {
                            cswPrivateVar.gridOpts.caption = '';
                            cswPrivateVar.hasPager = false;
                        } else {
                            cswPrivateVar.optNavEdit = {
                                editfunc: function (selRowid) {
                                    if (false === Csw.isNullOrEmpty(selRowid)) {
                                        var cellVal = cswPublicRet.grid.getValueForColumn('CHANGEDATE', selRowid);
                                        if (Csw.isFunction(cswPrivateVar.onEditRow)) {
                                            cswPrivateVar.onEditRow(cellVal);
                                        }
                                    } else {
                                        $.CswDialog('AlertDialog', 'Please select a row to edit');
                                    }
                                }
                            };
                        }

                        cswPublicRet.grid = cswPrivateVar.gridDiv.grid(cswPrivateVar);
                        cswPublicRet.grid.gridPager.css({ width: '100%', height: '20px' });

                        // set selected row by date

                        if (false === Csw.isNullOrEmpty(cswPrivateVar.selectedDate)) {
                            cswPrivateVar.preventSelectTrigger = true;
                            var rowid = cswPublicRet.grid.getRowIdForVal('CHANGEDATE', cswPrivateVar.selectedDate.toString());
                            cswPublicRet.grid.setSelection(rowid);
                            cswPrivateVar.preventSelectTrigger = false;
                        }
                    }
                }
            });


        });

} ());
