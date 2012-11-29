/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
    var cswReconciliationWizardStateName = 'cswReconciliationWizardStateName';

    Csw.nbt.ReconciliationWizard = Csw.nbt.ReconciliationWizard ||
        Csw.nbt.register('ReconciliationWizard', function (cswParent, options) {
            'use strict';

            //#region Properties
            var cswPrivate = {
                name: 'cswReconciliationWizard',
                exitFunc: null,
                numOfSteps: 3,
                startingStep: 1,
                wizard: null,
                buttons: {
                    next: 'next',
                    prev: 'previous',
                    finish: 'finish',
                    cancel: 'cancel'
                },
                divStep1: '',
                divStep2: '',
                divStep3: '',
                stepOneComplete: false,
                stepTwoComplete: false,
                stepThreeComplete: false,
                isCurrent: true,
                touchesIncluded: 0,
                currentDate: '',
                state: {
                    LocationId: '',
                    LocationName: '',
                    IncludeChildLocations: false,
                    StartDate: '',
                    EndDate: '',
                    ContainerActions: [{
                        ContainerId: '',
                        ContainerLocationId: '',
                        LocationId: '',
                        Action: ''
                    }],
                    ContainerLocationTypes: [{
                        Type: '',
                        Enabled: true
                    }]
                }
            };

            var cswPublic = {};
            //#endregion Properties

            //#region Wizard Functions
            cswPrivate.reinitSteps = function (startWithStep) {
                cswPrivate.stepThreeComplete = false;
                cswPrivate.state.ContainerActions = [];
                if (startWithStep <= 2) {
                    cswPrivate.stepTwoComplete = false;
                }
            };

            cswPrivate.toggleButton = function (button, isEnabled, doClick) {
                var btn;
                if (Csw.bool(isEnabled)) {
                    btn = cswPrivate.wizard[button].enable();
                    if (Csw.bool(doClick)) {
                        btn.click();
                    }
                } else {
                    cswPrivate.wizard[button].disable();
                }
                return false;
            };

            cswPrivate.toggleStepButtons = function () {
                cswPrivate.toggleButton(cswPrivate.buttons.cancel, true);
                cswPrivate.toggleButton(cswPrivate.buttons.prev, Csw.bool(cswPrivate.currentStepNo > 1));
                cswPrivate.toggleButton(cswPrivate.buttons.finish, Csw.bool(cswPrivate.currentStepNo === cswPrivate.numOfSteps));
                cswPrivate.toggleButton(cswPrivate.buttons.next, Csw.bool(cswPrivate.currentStepNo < cswPrivate.numOfSteps));
            };
            
            cswPrivate.handleStep = function (newStepNo) {
                cswPrivate.setState();
                cswPrivate.toggleButton(cswPrivate.buttons.finish, false);
                cswPrivate.toggleButton(cswPrivate.buttons.next, false);
                if (Csw.contains(cswPrivate, 'makeStep' + newStepNo)) {
                    cswPrivate.lastStepNo = cswPrivate.currentStepNo;
                    cswPrivate.currentStepNo = newStepNo;
                    cswPrivate['makeStep' + newStepNo]();
                }
            };
            //#endregion Wizard Functions

            //#region State Functions
            cswPrivate.validateState = function () {
                var state;
                if (Csw.isNullOrEmpty(cswPrivate.state.locationId)) {
                    state = cswPrivate.getState();
                    Csw.extend(cswPrivate.state, state);
                }
                cswPrivate.setState();
            };

            cswPrivate.getState = function () {
                return Csw.clientDb.getItem(cswPrivate.name + '_' + cswReconciliationWizardStateName);
            };

            cswPrivate.setState = function () {
                Csw.clientDb.setItem(cswPrivate.name + '_' + cswReconciliationWizardStateName, cswPrivate.state);
            };

            cswPrivate.clearState = function () {
                Csw.clientDb.removeItem(cswPrivate.name + '_' + cswReconciliationWizardStateName);
            };
            //#endregion State Functions

            //#region Step 1: Locations and Dates
            cswPrivate.makeStep1 = (function () {
                cswPrivate.stepOneComplete = false;
                return function () {
                    cswPrivate.toggleStepButtons();

                    if (false === cswPrivate.stepOneComplete) {
                        cswPrivate.divStep1 = cswPrivate.divStep1 || cswPrivate.wizard.div(1);
                        cswPrivate.divStep1.empty();

                        cswPrivate.divStep1.label({
                            text: "Choose the location of the Containers you wish to examine, and select a timeline with which to filter Reconciliation Scans.",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep1.br({ number: 2 });
                        //Props Table
                        var locationDatesTable = cswPrivate.divStep1.table({
                            name: 'setLocationDatesTable',
                            cellpadding: '5px',
                            cellalign: 'left',
                            cellvalign: 'top',
                            FirstCellRightAlign: true
                        });
                        var rowNum = 1;
                        //ViewMode
                        locationDatesTable.cell(rowNum, 1).span({ text: 'View Mode:' }).addClass('propertylabel');
                        var viewModeSelect = locationDatesTable.cell(rowNum, 2).select({
                            name: 'ReconciliationViewModeSelect',
                            values: ['Current','History'],
                            onChange: function () {
                                cswPrivate.isCurrent = Csw.bool(viewModeSelect.val() === 'Current');
                                if(cswPrivate.isCurrent) {
                                    endDatePicker.hide();
                                    staticEndDate.show();
                                    startDatePicker.setMaxDate(cswPrivate.getCurrentDate());
                                    endDatePicker.setMaxDate(cswPrivate.getCurrentDate());
                                    cswPrivate.state.EndDate = cswPrivate.getCurrentDate();
                                } else {
                                    endDatePicker.show();
                                    staticEndDate.hide();
                                    endDatePicker.setMaxDate(cswPrivate.getCurrentDate());
                                    cswPrivate.state.EndDate = endDatePicker.val().date;
                                    startDatePicker.setMaxDate(endDatePicker.val().date);
                                    cswPrivate.state.StartDate = startDatePicker.val().date;
                                }
                                cswPrivate.reinitSteps(2);
                            }
                        });
                        rowNum++;
                        //Location
                        locationDatesTable.cell(rowNum, 1).span({ text: 'Location:' }).addClass('propertylabel');
                        var locationControl = locationDatesTable.cell(rowNum, 2).location({
                            name: '',
                            onChange: function (locationId, locationName) {
                                cswPrivate.state.LocationId = locationId;
                                cswPrivate.state.LocationName = locationName;
                                cswPrivate.reinitSteps(2);
                            }
                        });
                        cswPrivate.state.LocationId = locationControl.val();
                        cswPrivate.state.LocationName = locationControl.selectedName();
                        rowNum++;
                        //IncludeChildLocations
                        var checkBoxTable = locationDatesTable.cell(rowNum, 2).table({
                            name: 'checkboxTable',
                            cellalign: 'left',
                            cellvalign: 'middle'
                        });
                        cswPrivate.childLocationsCheckBox = checkBoxTable.cell(1, 1).checkBox({
                            onChange: Csw.method(function () {
                                cswPrivate.state.IncludeChildLocations = cswPrivate.childLocationsCheckBox.checked();
                                cswPrivate.reinitSteps(2);
                            })
                        });
                        checkBoxTable.cell(1, 2).span({ text: ' Include child locations' });
                        rowNum++;
                        //StartDate
                        locationDatesTable.cell(rowNum, 1).span({ text: 'Start Date:' }).addClass('propertylabel');
                        var startDatePicker = locationDatesTable.cell(rowNum, 2).dateTimePicker({
                            name: 'startDate',
                            Date: cswPrivate.getCurrentDate(),
                            isRequired: true,
                            maxDate: cswPrivate.getCurrentDate(),
                            onChange: function () {
                                cswPrivate.state.StartDate = startDatePicker.val().date;
                                cswPrivate.reinitSteps(2);
                            }
                        });
                        cswPrivate.state.StartDate = startDatePicker.val().date;
                        rowNum++;
                        //EndDate
                        locationDatesTable.cell(rowNum, 1).span({ text: 'End Date:' }).addClass('propertylabel');
                        var endDatePicker = locationDatesTable.cell(rowNum, 2).dateTimePicker({
                            name: 'endDate',
                            Date: cswPrivate.getCurrentDate(),
                            isRequired: true,
                            maxDate: cswPrivate.getCurrentDate(),
                            onChange: function () {
                                cswPrivate.state.EndDate = endDatePicker.val().date;
                                startDatePicker.setMaxDate(cswPrivate.state.EndDate);
                                cswPrivate.state.StartDate = startDatePicker.val().date;
                                cswPrivate.reinitSteps(2);
                            }
                        });
                        endDatePicker.hide();
                        cswPrivate.state.EndDate = endDatePicker.val().date;
                        var staticEndDate = locationDatesTable.cell(rowNum, 2).span({ text: 'Today' });
                        rowNum++;
                        //TypeSelect
                        cswPrivate.state.ContainerLocationTypes = [
                            { Type: 'Scan', Enabled: true },
                            { Type: 'Receipt', Enabled: false },
                            { Type: 'Move', Enabled: false },
                            { Type: 'Dispense', Enabled: false },
                            { Type: 'Dispose', Enabled: false },
                            { Type: 'Undispose', Enabled: false },
                            { Type: 'Missing', Enabled: false }
                        ];
                        locationDatesTable.cell(rowNum, 1).span({ text: 'Type:' }).addClass('propertylabel');
                        var typeSelectTable = locationDatesTable.cell(rowNum, 2).table({
                            name: 'checkboxTable',
                            cellalign: 'left',
                            cellvalign: 'middle'
                        });
                        cswPrivate.typeCheckBox = [];
                        var typeRowNum = 1;
                        Csw.each(cswPrivate.state.ContainerLocationTypes, function(type, key) {
                            cswPrivate.typeCheckBox[typeRowNum - 1] = typeSelectTable.cell(typeRowNum, 1).checkBox({
                                checked: type.Enabled,
                                onChange: Csw.method(function () {
                                    cswPrivate.state.ContainerLocationTypes[key].Enabled = cswPrivate.typeCheckBox[key].checked();
                                    if (cswPrivate.state.ContainerLocationTypes[key].Type != 'Scan'){
                                        if (cswPrivate.state.ContainerLocationTypes[key].Enabled) {
                                            cswPrivate.touchesIncluded++;
                                        } else {
                                            cswPrivate.touchesIncluded--;
                                        }
                                    }
                                    cswPrivate.reinitSteps(2);
                                })
                            });
                            typeSelectTable.cell(typeRowNum, 2).span({ text: ' ' + type.Type });
                            typeRowNum++;
                        });

                        cswPrivate.stepOneComplete = true;
                    }
                };
            }());
            //#endregion Step 1: Locations and Dates

            //#region Step 2: Statistics
            cswPrivate.makeStep2 = (function () {
                cswPrivate.stepTwoComplete = false;
                return function () {                    
                    if (false === cswPrivate.stepTwoComplete) {
                        cswPrivate.divStep2 = cswPrivate.divStep2 || cswPrivate.wizard.div(2);
                        cswPrivate.divStep2.empty();
                        cswPrivate.divStep2.label({
                            text: "Loading Container Statistics...",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep2.br({ number: 2 });

                        Csw.ajaxWcf.post({
                            urlMethod: 'Containers/getContainerStatistics',
                            data: cswPrivate.state,
                            success: function (ajaxdata) {
                                cswPrivate.data = {
                                    ContainerStatistics: [{
                                        Status: '',
                                        ContainerCount: '',
                                        AmountScanned: '',
                                        PercentScanned: ''
                                    }]
                                };
                                Csw.extend(cswPrivate.data, ajaxdata);

                                var StatusMetricsGridFields = [];
                                var StatusMetricsGridColumns = [];
                                var addColumn = function (colName, displayName) {
                                    StatusMetricsGridColumns.push({
                                        dataIndex: colName,
                                        filterable: false,
                                        sortable: false,
                                        format: null,
                                        header: displayName,
                                        id: 'reconciliation_status_' + colName
                                    });
                                    StatusMetricsGridFields.push({
                                        name: colName,
                                        type: 'string',
                                        useNull: true
                                    });
                                };
                                addColumn('status', 'Status at time of Scan');
                                addColumn('numofcontainers', 'Number of Containers');
                                addColumn('percentscanned', 'Percent of Containers');

                                var StatusMetricsGridData = [];
                                var totalContainerCount = 0;
                                var totalContainersScanned = 0;
                                Csw.each(cswPrivate.data.ContainerStatistics, function (row) {
                                    totalContainerCount += row.ContainerCount;
                                });
                                Csw.each(cswPrivate.data.ContainerStatistics, function (row) {
                                    var percent = totalContainerCount > 0 ? Csw.number(row.ContainerCount / totalContainerCount) * 100.0 : 0;
                                    if (row.Status === 'Not Scanned') {
                                        StatusMetricsGridData.push({});
                                    }
                                    if (cswPrivate.touchesIncluded > 0 || row.Status !== 'Received, Moved, Dispensed, or Disposed') {
                                        StatusMetricsGridData.push({
                                            status: row.Status,
                                            numofcontainers: row.ContainerCount,
                                            percentscanned: (Math.round(Csw.number(percent) * 100) / 100) + '%'
                                        });
                                        if(Csw.startsWith(row.Status, 'Scanned')) {
                                            totalContainersScanned += row.ContainerCount;
                                        } else {
                                            StatusMetricsGridData.push({});
                                        }
                                    }
                                });
                                StatusMetricsGridData.push({
                                    status: 'Total',
                                    numofcontainers: totalContainerCount
                                });
                                var percentScanned = totalContainerCount > 0 ? Csw.number(totalContainersScanned / totalContainerCount) * 100.0 : 0;
                                StatusMetricsGridData.push({
                                    status: 'Percent Scanned',
                                    numofcontainers: (Math.round(Csw.number(percentScanned) * 100) / 100) + '%'
                                });

                                var StatusMetricsGridId = 'ReconciliationStatusGrid';
                                cswPrivate.gridOptions = {
                                    name: StatusMetricsGridId,
                                    storeId: StatusMetricsGridId,
                                    title: 'Container Statistics in ' + cswPrivate.state.LocationName
                                        + ' from ' + cswPrivate.state.StartDate
                                        + ' to ' + cswPrivate.state.EndDate,
                                    stateId: StatusMetricsGridId,
                                    usePaging: false,
                                    resizable: false,
                                    showActionColumn: false,
                                    canSelectRow: false,
                                    onLoad: null,
                                    onEdit: null,
                                    onDelete: null,
                                    onSelect: null,
                                    onDeselect: null,
                                    height: StatusMetricsGridData.length * 26,
                                    forcefit: true,
                                    width: '100%',
                                    fields: StatusMetricsGridFields,
                                    columns: StatusMetricsGridColumns,
                                    data: StatusMetricsGridData
                                };
                                cswPrivate.ReconciliationStatusGrid = cswPrivate.divStep2.grid(cswPrivate.gridOptions);
                                cswPrivate.toggleStepButtons();
                            }
                        });
                        cswPrivate.stepTwoComplete = true;
                    } else {
                        cswPrivate.toggleStepButtons();
                    }
                };
            }());
            //#endregion Step 2: Statistics

            //#region Step 3: Containers
            cswPrivate.makeStep3 = (function () {
                cswPrivate.stepThreeComplete = false;
                return function () {
                    if (false === cswPrivate.stepThreeComplete) {
                        cswPrivate.divStep3 = cswPrivate.divStep3 || cswPrivate.wizard.div(3);
                        cswPrivate.divStep3.empty();
                        cswPrivate.divStep3.label({
                            text: "Loading Containers and their Statuses...",
                            cssclass: "wizardHelpDesc"
                        });
                        cswPrivate.divStep3.br({ number: 2 });
                        Csw.ajaxWcf.post({
                            urlMethod: 'Containers/getContainerStatuses',
                            data: cswPrivate.state,
                            success: function (ajaxdata) {
                                cswPrivate.data.ContainerStatuses = [{
                                    ContainerId: '',
                                    ContainerBarcode: '',
                                    LocationId: '',
                                    ContainerLocationId: '',
                                    ContainerStatus: '',
                                    ScanDate: '',
                                    Action: '',
                                    ActionApplied: '',
                                    ActionOptions: []
                                }];
                                Csw.extend(cswPrivate.data.ContainerStatuses, ajaxdata.ContainerStatuses);

                                var ContainersGridFields = [];
                                var ContainersGridColumns = [];
                                var addColumn = function (colName, displayName, hidden, filter) {
                                    ContainersGridColumns.push({
                                        dataIndex: colName,
                                        filterable: true,
                                        format: null,
                                        hidden: hidden,
                                        header: displayName,
                                        id: 'reconciliation_container_' + colName,
                                        filter: filter
                                    });
                                    ContainersGridFields.push({
                                        name: colName,
                                        type: 'string',
                                        useNull: true
                                    });
                                };
                                addColumn('containerid', 'Container Id', true);
                                addColumn('locationid', 'Location Id', true);
                                addColumn('containerlocationid', 'ContainerLocation Id', true);
                                addColumn('actionapplied', 'Action Applied', true);
                                addColumn('actionoptions', 'Action Options', true);
                                addColumn('containerbarcode', 'Container Barcode', false);
                                var StatusOptions = [];
                                Csw.each(cswPrivate.data.ContainerStatistics, function(row) {
                                    StatusOptions.push( row.Status );
                                });
                                addColumn('status', 'Status', false, {
                                    type: 'list',
                                    options: StatusOptions
                                });
                                addColumn('scandate', 'Last Scan Date', false);
                                addColumn('currentaction', 'Current Action', true);
                                if (cswPrivate.isCurrent) {
                                    var actionControlCol = {
                                        header: 'Action',
                                        dataIndex: 'action',
                                        xtype: 'actioncolumn',
                                        renderer: function(value, metaData, record, rowIndex, colIndex, store, view) {
                                            var cell1Id = cswPrivate.name + 'action' + rowIndex + colIndex + '1';
                                            var ret = '<table id="gridActionColumn' + cell1Id + '" cellpadding="0"><tr>';
                                            ret += '<td id="' + cell1Id + '" style="width: 26px;"/>';
                                            ret += '</tr></table>';
                                            cswPrivate.makeActionPicklist(cell1Id, record);
                                            return ret;
                                        }
                                    };
                                    ContainersGridColumns.push(actionControlCol);
                                }

                                var ContainersGridData = [];
                                Csw.each(cswPrivate.data.ContainerStatuses, function (row) {
                                    ContainersGridData.push({
                                        containerid: row.ContainerId,
                                        locationid: row.LocationId,
                                        containerlocationid: row.ContainerLocationId,
                                        actionapplied: row.ActionApplied,
                                        containerbarcode: row.ContainerBarcode,
                                        status: row.ContainerStatus,
                                        scandate: row.ScanDate,
                                        actionoptions: row.ActionOptions.join(','),
                                        currentaction: row.Action
                                    });
                                });

                                var ContainersGridId = 'ReconciliationContainersGrid';
                                cswPrivate.gridOptions = {
                                    name: ContainersGridId,
                                    storeId: ContainersGridId,
                                    title: 'Containers in ' + cswPrivate.state.LocationName
                                        + ' from ' + cswPrivate.state.StartDate
                                        + ' to ' + cswPrivate.state.EndDate,
                                    stateId: ContainersGridId,
                                    usePaging: false,
                                    showActionColumn: false,
                                    canSelectRow: false,
                                    onLoad: null,
                                    onEdit: null,
                                    onDelete: null,
                                    onSelect: null,
                                    onDeselect: null,
                                    height: 200,
                                    forcefit: true,
                                    width: '100%',
                                    fields: ContainersGridFields,
                                    columns: ContainersGridColumns,
                                    data: ContainersGridData
                                };
                                var step3Table = cswPrivate.divStep3.table();
                                //Containers Grid
                                cswPrivate.makeContainersGrid(step3Table.cell(1, 1), true);
                                if (cswPrivate.isCurrent) {
                                    //Filter Correct Containers Checkbox
                                    var filterCorrectTable = step3Table.cell(2, 1).table({ cellvalign: 'center' });
                                    var filterCorrectCheckbox = filterCorrectTable.cell(1, 1).checkBox({
                                        checked: true,
                                        onChange: Csw.method(function () {
                                            cswPrivate.makeContainersGrid(step3Table.cell(1, 1), filterCorrectCheckbox.checked());
                                        })
                                    });
                                    filterCorrectTable.cell(1, 2).span({ text: ' Filter out Correct Containers' });
                                    step3Table.cell(3, 1).br();
                                    //Set Action in Bulk
                                    var bulkActionTable = step3Table.cell(4, 1).table({ cellvalign: 'center' });
                                    bulkActionTable.cell(1, 1).span({ text: 'Set Action of all Containers with Status&nbsp;' });
                                    var statusOptions = [];
                                    Csw.each(cswPrivate.data.ContainerStatistics, function(row) {
                                        if (row.Status.indexOf('Scanned') !== -1 && row.Status !== 'Scanned Correct') {
                                            statusOptions.push(row.Status);
                                        }
                                    });
                                    var statusSelect = bulkActionTable.cell(1, 2).select({
                                        name: 'bulkActionTableStatusSelect',
                                        values: statusOptions,
                                        onChange: function () {
                                            rebuildActionSelect();
                                        }
                                    });
                                    bulkActionTable.cell(1, 3).span({ text: '&nbsp;to&nbsp;' });
                                    var actionSelect;
                                    var rebuildActionSelect = function () {
                                        var actionOptions = cswPrivate.getActionOptions(statusSelect.val());
                                        bulkActionTable.cell(1, 4).empty();
                                        actionSelect = bulkActionTable.cell(1, 4).select({
                                            name: 'bulkActionTableStatusSelect',
                                            values: actionOptions
                                        });
                                        actionSelect.val('');
                                    };
                                    rebuildActionSelect();
                                    bulkActionTable.cell(1, 5).span({ text: '&nbsp;' });
                                    var applyChangesButton = bulkActionTable.cell(1, 6).buttonExt({
                                        enabledText: 'Apply Changes',
                                        size: 'small',
                                        tooltip: { title: 'Apply Changes' },
                                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                                        onClick: function () {
                                            Csw.each(cswPrivate.ReconciliationContainersGrid.getAllGridRows().items, function (row) {
                                                if (row.data.status === statusSelect.val()) {
                                                    cswPrivate.addActionChange(row.data, actionSelect.val());
                                                }
                                            });
                                            cswPrivate.makeContainersGrid(step3Table.cell(1, 1), filterCorrectCheckbox.checked());
                                            applyChangesButton.enable();
                                        }
                                    });
                                }
                                cswPrivate.toggleStepButtons();
                            }
                        });
                        cswPrivate.stepThreeComplete = true;
                    } else {
                        cswPrivate.toggleStepButtons();
                    }
                };
            }());
            //#endregion Step 3: Containers

            //#region Helper Functions
            cswPrivate.getCurrentDate = function () {
                if (Csw.isNullOrEmpty(cswPrivate.currentDate)) {
                    var today = new Date();
                    var dd = today.getDate();
                    var mm = today.getMonth() + 1; //January is 0!
                    var yyyy = today.getFullYear();
                    if (dd < 10) {
                        dd = '0' + dd;
                    }
                    if (mm < 10) {
                        mm = '0' + mm;
                    }
                    today = mm + '/' + dd + '/' + yyyy;
                    cswPrivate.currentDate = today;
                }
                return cswPrivate.currentDate;
            };
            
            cswPrivate.makeActionPicklist = function (cellId, record) {
                // Possible race condition - have to make the button after the cell is added, but it isn't added yet
                Csw.defer(function () {
                    var cell = Csw.literals.factory($('#' + cellId));
                    cell.empty();
                    var actionOptions = record.data.actionoptions.split(',');
                    var selectedOption = record.data.currentaction;
                    Csw.each(cswPrivate.state.ContainerActions, function (row) {
                        if (row.ContainerId === record.data.containerid) {
                            selectedOption = row.Action;
                        }
                    });
                    if (false === Csw.isNullOrEmpty(record.data.currentaction) 
                        && Csw.isNullOrEmpty(record.data.actionoptions)) {
                        actionOptions.push(record.data.currentaction);
                    }
                    var actionSelect = cell.select({
                        ID: cellId,
                        name: cellId,
                        values: actionOptions,
                        selected: selectedOption,
                        onChange: function () {
                            cswPrivate.addActionChange(record.data, actionSelect.val());
                        }
                    });
                    if (Csw.bool(record.data.actionapplied) === true) {
                        actionSelect.disable();
                    }
                    if(Csw.isNullOrEmpty(record.data.actionoptions)) {
                        actionSelect.hide();
                    }
                }, 50);
            };

            cswPrivate.addActionChange = function(container, action) {
                var ContainerAction = {
                    ContainerId: container.containerid,
                    ContainerLocationId: container.containerlocationid,
                    LocationId: container.locationid,
                    Action: action
                };
                var changeExists = false;
                Csw.each(cswPrivate.state.ContainerActions, function(row, key) {
                    if(row.ContainerId === ContainerAction.ContainerId) {
                        cswPrivate.state.ContainerActions[key] = ContainerAction;
                        changeExists = true;
                    }
                });
                if(false === changeExists) {
                    cswPrivate.state.ContainerActions.push(ContainerAction);
                }
            };

            cswPrivate.getActionOptions = function(status) {
                var actionOptions = ['', 'No Action'];
                if (status === 'Scanned, but already marked Disposed') {
                    actionOptions.push('Undispose');
                        }
                if (status === 'Scanned at Wrong Location') {
                    actionOptions.push('Move To Location');
                    }
                if (status === 'Scanned, but Disposed at Wrong Location') {
                    actionOptions.push('Undispose and Move');
                }
                if (status === 'Not Scanned') {
                    actionOptions.push('Mark Missing');
                }
                return actionOptions;
            };

            cswPrivate.makeContainersGrid = function(parentDiv, filterOutCorrect) {
                parentDiv.empty();
                cswPrivate.ReconciliationContainersGrid = parentDiv.grid(cswPrivate.gridOptions);
                var statusFilter = cswPrivate.ReconciliationContainersGrid.extGrid.filters.getFilter('status');
                statusFilter.setActive(true);
                cswPrivate.filterCorrectContainers(filterOutCorrect);
            };

            cswPrivate.filterCorrectContainers = function (filterOutCorrect) {
                var statusFilter = cswPrivate.ReconciliationContainersGrid.extGrid.filters.getFilter('status');
                statusFilter.setActive(true);
                var optionsToFilter = [];
                Csw.each(cswPrivate.data.ContainerStatistics, function (row) {
                    if (false === filterOutCorrect ||
                        (row.Status !== 'Scanned Correct' && row.Status !== 'Received, Moved, Dispensed, or Disposed')) {
                        optionsToFilter.push(row.Status);
                    }
                });
                statusFilter.menu.setSelected(optionsToFilter, true);
            };

            cswPrivate.saveChanges = function() {
                Csw.ajaxWcf.post({
                    urlMethod: 'Containers/saveContainerActions',
                    data: cswPrivate.state
                });
                cswPrivate.state.ContainerActions = [];
            };
            //#endregion Helper Functions

            //#region ctor
            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.validateState();
                cswPrivate.wizardSteps = {
                    1: 'Choose Location and Dates',
                    2: 'Statistics',
                    3: 'Containers'
                };
                cswPrivate.currentStepNo = cswPrivate.startingStep;

                cswPrivate.finalize = function () {
                    cswPrivate.saveChanges();
                    Csw.tryExec(cswPrivate.onFinish);
                };

                cswPrivate.wizard = Csw.layouts.wizard(cswParent.div(), {
                    Title: 'Reconciliation',
                    StepCount: cswPrivate.numOfSteps,
                    Steps: cswPrivate.wizardSteps,
                    StartingStep: cswPrivate.startingStep,
                    FinishText: 'Finish',
                    onNext: cswPrivate.handleStep,
                    onPrevious: cswPrivate.handleStep,
                    onCancel: function () {
                        cswPrivate.clearState();
                        Csw.tryExec(cswPrivate.onCancel);
                    },
                    onFinish: cswPrivate.finalize,
                    doNextOnInit: false
                });

            }());
            //#endregion ctor

            cswPrivate.makeStep1();

            return cswPublic;
        });
} ());