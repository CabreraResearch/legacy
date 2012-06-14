/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.auditHistory = Csw.actions.auditHistory ||
        Csw.actions.register('auditHistory', function (cswParent, options) {
            'use strict';

            var cswPrivate = {
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
                        if (false === cswPrivate.preventSelectTrigger && false === Csw.isNullOrEmpty(selRowid)) {
                            var cellVal = cswPublic.grid.getValueForColumn('CHANGEDATE', selRowid);
                            Csw.tryExec(cswPrivate.onSelectRow, cellVal);
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
                $.extend(cswPrivate, options);
            }
            var cswPublic = { };
            cswParent.empty();

            var gridId = cswPrivate.ID + '_auditGrid';
            cswParent.grid({
                ID: gridId,
                storeId: gridId,
                title: 'History',
                stateId: '',
                usePaging: (cswPrivate.EditMode !== Csw.enums.editMode.PrintReport),
                ajax: {
                    urlMethod: cswPrivate.urlMethod,
                    data: {
                        NodeId: Csw.string(cswPrivate.nodeid),
                        NbtNodeKey: Csw.string(cswPrivate.cswnbtnodekey),
                        JustDateColumn: cswPrivate.JustDateColumn
                    }
                },
                showDelete: false,
                onEdit: function (row) {
                    Csw.tryExec(cswPrivate.onEditRow, row.changedate);
                }
            });
//                        // set selected row by date

//                        if (false === Csw.isNullOrEmpty(cswPrivate.selectedDate)) {
//                            cswPrivate.preventSelectTrigger = true;
//                            var rowid = cswPublic.grid.getRowIdForVal('CHANGEDATE', cswPrivate.selectedDate.toString());
//                            cswPublic.grid.setSelection(rowid);
//                            cswPrivate.preventSelectTrigger = false;
//                        }

        });
} ());
