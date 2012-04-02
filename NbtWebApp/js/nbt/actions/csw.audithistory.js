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
                allowEditRow: true
            };
            if (options) {
                $.extend(internal, options);
            }

            cswParent.empty();

            Csw.ajax.post({
                urlMethod: internal.urlMethod,
                data: {
                    NodeId: Csw.string(internal.nodeid),
                    NbtNodeKey: Csw.string(internal.cswnbtnodekey),
                    JustDateColumn: internal.JustDateColumn
                },
                success: function (gridJson) {

                    var preventSelectTrigger = false;

                    var auditGridId = internal.ID + '_csw_auditGrid_outer';
                    var $auditGrid = $Div.find('#' + auditGridId);
                    if (Csw.isNullOrEmpty($auditGrid) || $auditGrid.length === 0) {
                        $auditGrid = $('<div id="' + internal.ID + '"></div>').appendTo($Div);
                    } else {
                        $auditGrid.empty();
                    }

                    var g = {
                        ID: internal.ID,
                        canEdit: Csw.bool(internal.allowEditRow),
                        pagermode: 'default',
                        gridOpts: {
                            height: 180,
                            rowNum: 10,
                            onSelectRow: function (selRowid) {
                                if (!preventSelectTrigger && false === Csw.isNullOrEmpty(selRowid)) {
                                    var cellVal = grid.getValueForColumn('CHANGEDATE', selRowid);
                                    if (Csw.isFunction(internal.onSelectRow)) {
                                        internal.onSelectRow(cellVal);
                                    }
                                }
                            },
                            add: false,
                            view: false,
                            del: false,
                            refresh: false,
                            search: false
                        }
                    };

                    if (Csw.contains(gridJson, 'jqGridOpt')) {

                        $.extend(g.gridOpts, gridJson.jqGridOpt);

                        if (internal.EditMode === Csw.enums.editMode.PrintReport) {
                            g.gridOpts.caption = '';
                            g.hasPager = false;
                        } else {
                            g.optNavEdit = {
                                editfunc: function (selRowid) {
                                    if (false === Csw.isNullOrEmpty(selRowid)) {
                                        var cellVal = grid.getValueForColumn('CHANGEDATE', selRowid);
                                        if (Csw.isFunction(internal.onEditRow)) {
                                            internal.onEditRow(cellVal);
                                        }
                                    } else {
                                        $.CswDialog('AlertDialog', 'Please select a row to edit');
                                    }
                                }
                            };
                        }
                        //g.$parent = $auditGrid;
                        var parent = Csw.literals.factory($auditGrid);
                        var grid = parent.grid(g);
                        grid.gridPager.css({ width: '100%', height: '20px' });

                        // set selected row by date

                        if (!Csw.isNullOrEmpty(internal.selectedDate)) {
                            preventSelectTrigger = true;
                            var rowid = grid.getRowIdForVal('CHANGEDATE', internal.selectedDate.toString());
                            grid.setSelection(rowid);
                            preventSelectTrigger = false;
                        }
                    }
                }
            });


        });

} ());
