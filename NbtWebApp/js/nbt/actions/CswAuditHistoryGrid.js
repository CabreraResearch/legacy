///// <reference path="~/js/CswNbt-vsdoc.js" />
///// <reference path="~/js/CswCommon-vsdoc.js" />

//(function ($) {
//    "use strict";
//    var pluginName = 'CswAuditHistoryGrid';

//    var methods = {
//        init: function (options) {
//            var o = {
//                Url: '/NbtWebApp/wsNBT.asmx/getAuditHistoryGrid',
//                ID: '',
//                nodeid: '',
//                EditMode: Csw.enums.editMode.Edit,
//                JustDateColumn: false,
//                onEditRow: null, //function (date) {},
//                onSelectRow: null, //function (date) {},
//                selectedDate: '',
//                allowEditRow: true
//            };
//            if (options) $.extend(o, options);

//            var $Div = $(this);
//            $Div.contents().remove();

//            Csw.ajax.post({
//                url: o.Url,
//                data: {
//                    NodeId: Csw.string(o.nodeid),
//                    NbtNodeKey: Csw.string(o.cswnbtnodekey),
//                    JustDateColumn: o.JustDateColumn
//                },
//                success: function (gridJson) {

//                    var preventSelectTrigger = false;

//                    var auditGridId = o.ID + '_csw_auditGrid_outer';
//                    var $auditGrid = $Div.find('#' + auditGridId);
//                    if (Csw.isNullOrEmpty($auditGrid) || $auditGrid.length === 0) {
//                        $auditGrid = $('<div id="' + o.ID + '"></div>').appendTo($Div);
//                    } else {
//                        $auditGrid.empty();
//                    }

//                    var g = {
//                        ID: o.ID,
//                        canEdit: Csw.bool(o.allowEditRow),
//                        pagermode: 'default',
//                        gridOpts: {
//                            height: 180,
//                            rowNum: 10,
//                            onSelectRow: function (selRowid) {
//                                if (!preventSelectTrigger && false === Csw.isNullOrEmpty(selRowid)) {
//                                    var cellVal = grid.getValueForColumn('CHANGEDATE', selRowid);
//                                    if (Csw.isFunction(o.onSelectRow)) {
//                                        o.onSelectRow(cellVal);
//                                    }
//                                }
//                            },
//                            add: false,
//                            view: false,
//                            del: false,
//                            refresh: false,
//                            search: false
//                        }
//                    };

//                    if (Csw.contains(gridJson, 'jqGridOpt')) {

//                        $.extend(g.gridOpts, gridJson.jqGridOpt);

//                        if (o.EditMode === Csw.enums.editMode.PrintReport) {
//                            g.gridOpts.caption = '';
//                            g.hasPager = false;
//                        }
//                        else {
//                            g.optNavEdit = {
//                                editfunc: function (selRowid) {
//                                    if (false === Csw.isNullOrEmpty(selRowid)) {
//                                        var cellVal = grid.getValueForColumn('CHANGEDATE', selRowid);
//                                        if (Csw.isFunction(o.onEditRow)) {
//                                            o.onEditRow(cellVal);
//                                        }
//                                    } else {
//                                        $.CswDialog('AlertDialog', 'Please select a row to edit');
//                                    }
//                                }
//                            };
//                        }
//                        //g.$parent = $auditGrid;
//                        var parent = Csw.literals.factory($auditGrid);
//                        var grid = parent.grid(g);
//                        grid.gridPager.css({ width: '100%', height: '20px' });

//                        // set selected row by date

//                        if (!Csw.isNullOrEmpty(o.selectedDate)) {
//                            preventSelectTrigger = true;
//                            var rowid = grid.getRowIdForVal('CHANGEDATE', o.selectedDate.toString());
//                            grid.setSelection(rowid);
//                            preventSelectTrigger = false;
//                        }
//                    }
//                }
//            });


//        }
//    };

//    // Method calling logic
//    $.fn.CswAuditHistoryGrid = function (method) {

//        if (methods[method]) {
//            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
//        } else if (typeof method === 'object' || !method) {
//            return methods.init.apply(this, arguments);
//        } else {
//            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
//        }

//    };
//})(jQuery);
