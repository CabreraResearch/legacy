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
                    cswPrivate.onTabSelect(tabName);
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

            cswPrivate.makeRulesTab = function() {
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
                    FirstCellRightAlign: true
                });

                customerIdTable.cell(1, 1).span({ text: 'Customer ID&nbsp' })
                    .css({ 'padding': '1px', 'vertical-align': 'middle' });

                customerIdSelect = customerIdTable.cell(1, 2)
                    .select({
                        name: 'customerIdSelect',
                        selected: '',
                        values: [{ value: '[ None ]', display: '[ None ]' }],
                        onChange: function () {
                            var selected = customerIdSelect.find(':selected');
                            cswPrivate.selectedCustomerId = selected.val();
                        }
                    });

                Csw.ajax.post({
                    urlMethod: 'getActiveAccessIds',
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

                cswPrivate.selectedCustomerId = customerIdSelect.find(':selected').val();

                customerIdTable.cell(1, 3).buttonExt({
                    name: 'updateRules',
                    enabledText: 'Update Rules',
                    disabledText: 'Updating . . . ',
                    onClick: function () {
                        var temp = cswPrivate.scheduledRulesGrid.getAllGridRows();
                        Csw.ajax.post({
                            urlMethod: 'updateAllScheduledRules',
                            data: { AccessId: cswPrivate.selectedCustomerId, Action: 'ClearAllReprobates' },
                            success: cswPrivate.makeStepTwo
                        });
                    }
                });

            };

            cswPrivate.makeScheduledRulesGrid = function (cswNode) {
                var gridId = 'rulesGrid';

                cswPrivate.gridAjax = Csw.ajaxWcf.post({
                    urlMethod: 'Scheduler/getScheduledRulesGrid',
                    data: cswPrivate.selectedCustomerId,
                    success: function (result) {

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
                                        if (key === 'reprobate') {

                                            parsedRow[key] = Csw.bool(row.Row[key]);
                                        } else {
                                            parsedRow[key] = row.Row[key];
                                        }
                                    });
                                }
                                parsedRows.push(parsedRow);
                            });
                            result.Grid.data.items = parsedRows;
                        }

                        var columns = result.Grid.columns;
                        columns.forEach(function (col) {
                            switch (col.header) {
                                case result.ColumnIds.failed_cnt:
                                    col.editable = true;
                                    col.editor = {
                                        xtype: 'numberfield',
                                        allowBlank: false,
                                        minValue: 0,
                                        maxValue: 10
                                    };
                                    break;
                                case result.ColumnIds.freq:
                                    col.editable = true;
                                    col.editor = {
                                        xtype: 'numberfield',
                                        allowBlank: false,
                                        minValue: 1,
                                        maxValue: 100
                                    };
                                    break;
                                case result.ColumnIds.type:
                                    col.editable = true;
                                    col.editor = new Ext.form.field.ComboBox({
                                        typeAhead: true,
                                        triggerAction: 'all',
                                        selectOnTab: true,
                                        store: [
                                            [result.RecurrenceOptions[0], result.RecurrenceOptions[0]],
                                            [result.RecurrenceOptions[1], result.RecurrenceOptions[1]],
                                            [result.RecurrenceOptions[2], result.RecurrenceOptions[2]],
                                            [result.RecurrenceOptions[3], result.RecurrenceOptions[3]],
                                            [result.RecurrenceOptions[4], result.RecurrenceOptions[4]],
                                            [result.RecurrenceOptions[5], result.RecurrenceOptions[5]],
                                            [result.RecurrenceOptions[6], result.RecurrenceOptions[6]],
                                            [result.RecurrenceOptions[7], result.RecurrenceOptions[7]]
                                        ],
                                        lazyRender: true,
                                        listClass: 'x-combo-list-small'
                                    });
                                    break;
                                case result.ColumnIds.max_fail:
                                    col.editable = true;
                                    col.editor = {
                                        xtype: 'numberfield',
                                        allowBlank: false,
                                        minValue: 0,
                                        maxValue: 100
                                    };
                                    break;
                                case result.ColumnIds.max_ms:
                                    col.editable = true;
                                    col.editor = {
                                        xtype: 'numberfield',
                                        allowBlank: false,
                                        minValue: 1000,
                                        maxValue: 10000000
                                    };
                                    break;
                                case result.ColumnIds.reprobate:
                                    col.editable = true;
                                    col.xtype = 'checkcolumn';
                                    col.editor = new Ext.form.field.Checkbox({
                                        
                                    });
                                    //col.width = 60;
                                    //col.stopSelection = false;
                                    //col.editor = {
                                    //    xtype: 'checkcolumn',
                                    //    cls: 'x-grid-checkheader-editor'
                                    //};
                                    break;
                                case result.ColumnIds.rogue_cnt:
                                    col.editable = true;
                                    col.editor = {
                                        xtype: 'numberfield',
                                        minValue: 0,
                                        maxValue: 10
                                    };
                                    break;
                                case result.ColumnIds.status_message:
                                    col.editable = true;
                                    col.editor = {
                                        allowBlank: true
                                    };
                                    break;
                            }
                        });


                        cswPrivate.scheduledRulesGrid = cswNode.grid({
                            name: gridId,
                            storeId: gridId,
                            data: result.Grid,
                            stateId: gridId,
                            title: 'Scheduled Rules',
                            usePaging: false,
                            showActionColumn: false,
                            canSelectRow: false,
                            selModel: {
                                selType: 'cellmodel'
                            },
                            plugins: [
                                Ext.create('Ext.grid.plugin.CellEditing', {
                                    clicksToEdit: 1
                                })
                            ]
                        });

                    } // success
                });
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
                    onTabSelect: cswPrivate.onTabSelect
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