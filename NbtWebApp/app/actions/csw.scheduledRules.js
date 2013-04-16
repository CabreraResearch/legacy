/*global Csw:true*/
(function () {

    Csw.actions.scheduledRules = Csw.actions.scheduledRules ||
        Csw.actions.register('scheduledRules', function (cswParent, cswPrivate) {
            'use strict';

            Ext.require([
                'Ext.selection.CellModel',
                'Ext.grid.*',
                'Ext.data.*',
                'Ext.util.*',
                'Ext.state.*',
                'Ext.form.*',
                'Ext.ux.CheckColumn'
            ]);

            //#region _preCtor

            var cswPublic = {};

            (function _preCtor() {
                cswPrivate.name = cswPrivate.name || 'CswScheduledRules';
                cswPrivate.onCancel = cswPrivate.onCancel || function () { };
                cswParent.empty();
            }());


            //#endregion _preCtor

            //#region AJAX methods



            //#endregion AJAX methods

            //#region Tab construction

            cswPrivate.openTab = function (tabName) {
                var idx = cswPrivate.tabNames.indexOf(tabName);
                if (idx !== -1) {
                    cswPrivate.tabs.setActiveTab(idx);
                    //cswPrivate.onTabSelect(tabName);
                    cswPrivate.makeRulesTab();
                }
            };

            cswPrivate.tabNames = ['Rules'];

            cswPrivate.tryParseTabName = function (tabName, elTarget, eventObjText) {
                var tab = '', ret = '';
                if (tabName) {
                    tab = tabName.split(' ')[0].trim();
                    if (cswPrivate.tabNames.indexOf(tab) === -1) {
                        if (eventObjText) {
                            tab = cswPrivate.tryParseTabName(eventObjText, elTarget);
                            if (cswPrivate.tabNames.indexOf(tab) === -1) {
                                tab = cswPrivate.tryParseTabName(elTarget);
                                if (cswPrivate.tabNames.indexOf(tab) !== -1) {
                                    ret = tab;
                                }
                            } else {
                                ret = tab;
                            }
                        }
                    } else {
                        ret = tab;
                    }
                }
                return ret;
            };

            cswPrivate.onTabSelect = function (tabName, el, eventObj, callBack) {
                var tgtTxt = null, evtTxt;
                if (tabName.indexOf('<') === 0 &&
                    tabName.lastIndexOf('>') === tabName.length - 1) {
                    if (el) {
                        tgtTxt = el.target.innerText;
                    }

                    if (eventObj) {
                        evtTxt = eventObj.innerText;
                    }
                } else {
                    if (tabName.length > 20) {
                        // yuck. Clicking anywhere inside the tab fires this event. That includes clicking a grid row whose nodename begins [Tabname].
                        tabName = '';
                    }
                }
                var newTabName = cswPrivate.tryParseTabName(tabName, tgtTxt, evtTxt);
                if (newTabName.length > 0) {
                    cswPrivate.makeRulesTab();
                }
            };

            cswPrivate.prepTab = function (tab, title, headerText) {

                tab.csw.empty().css({ margin: '10px' });

                var ol = tab.csw.ol();

                ol.li().span({
                    text: headerText
                });
                ol.li().br({ number: 2 });

                return ol;
            };

            cswPrivate.makeRulesTab = function () {
                var ol = cswPrivate.prepTab(cswPrivate.rulesTab, 'Rules', 'Select a Customer to review and make any necessary edits to the Scheduled Rules for their schema.');

                cswPrivate.makeCustomerIdSelect(ol.li());

                ol.li().br({ number: 2 });

                cswPrivate.makeScheduledRulesGrid(ol.li());

                ol.li().br({ number: 2 });

                cswPrivate.addBtnGroup(ol.li());
            };


            cswPrivate.makeCustomerIdSelect = function (cswNode) {
                var customerIdTable, customerIdSelect;

                customerIdTable = cswNode.table({
                    name: 'inspectionTable',
                    FirstCellRightAlign: true,
                    cellpadding: '2px'
                });

                customerIdTable.cell(1, 1).span({ text: 'Customer ID&nbsp' })
                    .css({ 'padding': '1px', 'vertical-align': 'middle' });

                customerIdSelect = customerIdTable.cell(1, 2)
                    .select({
                        name: 'customerIdSelect',
                        onChange: function () {
                            cswPrivate.selectedCustomerId = customerIdSelect.val();
                            cswPrivate.makeScheduledRulesGrid();
                        }
                    });

                Csw.ajax.post({
                    urlMethod: 'getActiveAccessIds',
                    async: false,
                    success: function (data) {
                        cswPrivate.customerIds = data.customerids;
                        if (cswPrivate.customerIds.length > 1) {
                            customerIdSelect.setOptions(cswPrivate.customerIds);
                            cswPrivate.selectedCustomerId = customerIdSelect.find(':selected').val();
                        } else {
                            customerIdSelect.empty();
                            customerIdSelect.option({ value: cswPrivate.customerIds[0], display: cswPrivate.customerIds[0], isSelected: true });
                            cswPrivate.selectedCustomerId = cswPrivate.customerIds[0];
                        }
                    }
                });

                customerIdTable.cell(1, 3).buttonExt({
                    name: 'updateRules',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                    enabledText: 'Save Changes',
                    disabledText: 'Saving . . . ',
                    onClick: function () {
                        var req = Csw.extend({}, cswPrivate.schedulerRequest, true);
                        req.Grid.columns.forEach(function (col) {
                            delete col.editable;
                            delete col.editor;
                        });

                        Csw.ajaxWcf.post({
                            urlMethod: 'Scheduler/save',
                            data: req,
                            success: cswPrivate.makeStepTwo
                        });
                    }
                });

            };

            cswPrivate.makeScheduledRulesGrid = function (parentDiv) {
                var gridId = 'rulesGrid';
                cswPrivate.gridDiv = cswPrivate.gridDiv || parentDiv;
                

                cswPrivate.gridAjax = Csw.ajaxWcf.post({
                    urlMethod: 'Scheduler/get',
                    data: cswPrivate.selectedCustomerId,
                    success: function (result) {

                        cswPrivate.schedulerRequest = result;
                        var parsedRows = [];
                        if (result && result.Grid.data && result.Grid.data.items) {
                            result.Grid.data.items.forEach(function (row) {
                                var parsedRow = {
                                    RowNo: row.RowNo,
                                    canDelete: false,
                                    canEdit: false,
                                    canView: false,
                                    isDisabled: false,
                                    isLocked: false
                                };
                                if (row && row.Row) {
                                    Object.keys(row.Row).forEach(function (key) {
                                        if (key === 'reprobate' || key === 'disabled') {
                                            parsedRow[key] = Csw.bool(row.Row[key]);
                                        } else {
                                            parsedRow[key] = row.Row[key];
                                        }
                                    });
                                }
                                parsedRow.Row = row.Row;
                                parsedRows.push(parsedRow);
                            });
                            result.Grid.data.items = parsedRows;
                        }

                        var columns = result.Grid.columns;
                        columns.forEach(function (col) {
                            switch (col.header) {
                                case result.ColumnIds.failed_cnt:
                                    col.editable = true;
                                    Object.defineProperty(col, 'editor', {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true,
                                        value: {
                                            xtype: 'numberfield',
                                            allowBlank: false,
                                            minValue: 0,
                                            maxValue: 10
                                        }
                                    });
                                    break;
                                case result.ColumnIds.freq:
                                    col.editable = true;
                                    Object.defineProperty(col, 'editor', {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true,
                                        value: {
                                            xtype: 'numberfield',
                                            allowBlank: false,
                                            //TODO - these min/max values should be within the context of the selected Type (Minutes: 15-60, Daily: 1, DayOfYear: 1-365, etc)
                                            minValue: 1,
                                            maxValue: 365
                                        }
                                    });
                                    break;
                                case result.ColumnIds.type:
                                    col.editable = true;
                                    col.filter = {
                                        type: 'list',
                                        options: result.RecurrenceOptions
                                    };
                                    Object.defineProperty(col, 'editor', {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true,
                                        value: new Ext.form.field.ComboBox({
                                            typeAhead: true,
                                            typeAheadDelay: 0,
                                            triggerAction: 'all',
                                            selectOnTab: true,
                                            allowBlank: false,
                                            valueNotFoundText: 'Could not find that value',
                                            validator: function(val) {
                                                return result.RecurrenceOptions.indexOf(val) !== -1;
                                            },
                                            store: [
                                                [result.RecurrenceOptions[0], result.RecurrenceOptions[0]],//Never
                                                [result.RecurrenceOptions[1], result.RecurrenceOptions[1]],//Daily
                                                [result.RecurrenceOptions[2], result.RecurrenceOptions[2]],//DayOfMonth
                                                [result.RecurrenceOptions[3], result.RecurrenceOptions[3]],//DayOfWeek
                                                [result.RecurrenceOptions[4], result.RecurrenceOptions[4]],//DayOfYear
                                                [result.RecurrenceOptions[5], result.RecurrenceOptions[5]],//Hourly
                                                [result.RecurrenceOptions[6], result.RecurrenceOptions[6]],//NHours
                                                [result.RecurrenceOptions[7], result.RecurrenceOptions[7]] //NMinutes
                                            ],
                                            lazyRender: true
                                        })
                                    });
                                    break;
                                case result.ColumnIds.max_fail:
                                    col.editable = true;
                                    Object.defineProperty(col, 'editor', {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true,
                                        value: {
                                            xtype: 'numberfield',
                                            allowBlank: false,
                                            minValue: 0,
                                            maxValue: 100
                                        }
                                    });
                                    break;
                                case result.ColumnIds.max_ms:
                                    col.editable = true;
                                    Object.defineProperty(col, 'editor', {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true,
                                        value: {
                                            xtype: 'numberfield',
                                            allowBlank: false,
                                            minValue: 1000,
                                            maxValue: 10000000
                                        }
                                    });
                                    break;
                                case result.ColumnIds.reprobate:
                                    col.editable = true;
                                    col.xtype = 'checkcolumn';
                                    col.listeners = {
                                        checkchange: function (checkbox, rowNum, isChecked) {
                                            cswPrivate.schedulerRequest.Grid.data.items[rowNum]['reprobate'] = isChecked;
                                            cswPrivate.schedulerRequest.Grid.data.items[rowNum].Row['reprobate'] = isChecked;
                                            cswPrivate.schedulerRequest.Grid.data.items[rowNum].Row['has_changed'] = 'true';
                                        }
                                    };
                                    col.editor = {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true
                                    };
                                    break;
                                case result.ColumnIds.rogue_cnt:
                                    col.editable = true;
                                    Object.defineProperty(col, 'editor', {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true,
                                        value: {
                                            xtype: 'numberfield',
                                            minValue: 0,
                                            maxValue: 10
                                        }
                                    });
                                    break;
                                case result.ColumnIds.status_message:
                                    col.editable = true;
                                    Object.defineProperty(col, 'editor', {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true,
                                        value: {
                                            allowBlank: true
                                        }
                                    });
                                    break;
                                case result.ColumnIds.priority:
                                    col.editable = true;
                                    Object.defineProperty(col, 'editor', {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true,
                                        value: {
                                            xtype: 'numberfield',
                                            allowBlank: false,
                                            minValue: 0,
                                            maxValue: 100
                                        }
                                    });
                                    break;
                                case result.ColumnIds.disabled:
                                    col.editable = true;
                                    col.xtype = 'checkcolumn';
                                    col.listeners = {
                                        checkchange: function (checkbox, rowNum, isChecked) {
                                            cswPrivate.schedulerRequest.Grid.data.items[rowNum]['disabled'] = isChecked;
                                            cswPrivate.schedulerRequest.Grid.data.items[rowNum].Row['disabled'] = isChecked;
                                            cswPrivate.schedulerRequest.Grid.data.items[rowNum].Row['has_changed'] = 'true';
                                        }
                                    };
                                    col.editor = {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true
                                    };
                                    break;
                                case result.ColumnIds.has_changed:
                                    col.editable = true;
                                    col.hidden = true;
                                    Object.defineProperty(col, 'editor', {
                                        writable: true,
                                        configurable: true,
                                        enumerable: true,
                                        value: {
                                            xtype: 'booleancolumn',
                                            trueText: 'true',
                                            falseText: 'false',
                                        }
                                    });
                                    break;
                            }
                        });

                        if (cswPrivate.scheduledRulesGrid && cswPrivate.scheduledRulesGrid.destroy) {
                            cswPrivate.scheduledRulesGrid.destroy();
                        }
                        cswPrivate.gridDiv.empty();
                        cswPrivate.scheduledRulesGrid = cswPrivate.gridDiv.grid({
                            name: gridId,
                            storeId: gridId,
                            data: result.Grid,
                            stateId: gridId,
                            height: 375,
                            width: '95%',
                            title: 'Scheduled Rules',
                            usePaging: true,
                            onRefresh: cswPrivate.makeScheduledRulesGrid,
                            showActionColumn: false,
                            canSelectRow: false,
                            selModel: {
                                selType: 'cellmodel'
                            },
                            plugins: [
                                Ext.create('Ext.grid.plugin.CellEditing', {
                                    clicksToEdit: 1,
                                    listeners: {
                                        edit: cswPrivate.onGridEdit
                                    }
                                })
                            ]
                        });

                    } // success
                });
            };

            cswPrivate.onGridEdit = function (grid, row, opts) {
                if (cswPrivate.schedulerRequest.Grid.data.items[row.rowIdx][row.field] !== Csw.string(row.value)) {
                    cswPrivate.schedulerRequest.Grid.data.items[row.rowIdx].Row['has_changed'] = 'true';
                }
                cswPrivate.schedulerRequest.Grid.data.items[row.rowIdx][row.field] = row.value;
                cswPrivate.schedulerRequest.Grid.data.items[row.rowIdx].Row[row.field] = row.value;
            };

            cswPrivate.addBtnGroup = function (el) {
                var tbl = el.table({ width: '99%', cellpadding: '5px' });

                tbl.cell(1, 2).css({ 'text-align': 'right' }).buttonExt({
                    enabledText: 'Close',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cancel),
                    onClick: function () {
                        Csw.tryExec(cswPrivate.onCancel);
                        cswPrivate.clearState();
                    }
                });
            };

            //#endregion Tab construction

            //#region _postCtor            

            (function _postCtor() {

                cswParent.empty();
                cswPrivate.tabs = cswParent.tabStrip({
                    //onTabSelect: cswPrivate.onTabSelect
                });
                cswPrivate.tabs.setTitle('Scheduled Rules by Customer ID');

                cswPrivate.rulesTab = cswPrivate.tabs.addTab({
                    title: 'Rules'
                });

                cswPrivate.openTab('Rules');
                //cswPrivate.tabs.setActiveTab(0);

            }());

            return cswPublic;

            //#endregion _postCtor
        });
}());