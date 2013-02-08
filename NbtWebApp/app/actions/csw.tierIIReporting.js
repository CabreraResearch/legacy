/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
Csw.actions.tierIIReporting = Csw.actions.template ||
    Csw.actions.register('tierIIReporting', function (cswParent, cswPrivate) {
        'use strict';
        var cswPublic = {};
        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException('Cannot create an action without a valid Csw Parent object.', 'Csw.actions.tierIIReporting', 'csw.tierIIReporting.js', 10);
        }
        
        //#region _preCtor
        (function _preCtor() {
            cswPrivate.name = cswPrivate.name || 'Tier II Reporting';
            cswPrivate.onCancel = cswPrivate.onCancel || function () { };

            cswPrivate.LocationId = cswPrivate.LocationId || '';
            cswPrivate.LocationName = cswPrivate.LocationName || '';

            cswParent.empty();
        }());
        //#endregion _preCtor

        //#region Action Functions
        cswPrivate.onCancelClick = function() {
            Csw.tryExec(cswPrivate.onCancel);
        };
        
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
        //#endregion Action Functions
        
        //#region Setup Controls
        cswPrivate.createSetupControls = function() {
            //Location
            cswPrivate.controlTbl.cell(1, 1).span({ text: 'Location:' }).addClass('propertylabel');
            var locationControl = cswPrivate.controlTbl.cell(1, 2).location({
                name: '',
                onChange: function (locationId, locationName) {
                    if (Csw.isNullOrEmpty(locationId)) {
                        locationControl.comboBox.topContent(cswPrivate.LocationName, cswPrivate.LocationId);
                        locationControl.comboBox.val(cswPrivate.LocationId);
                    } else {
                        cswPrivate.LocationId = locationId;
                        cswPrivate.LocationName = locationName;
                    }
                }
            });
            cswPrivate.LocationId = locationControl.val();
            cswPrivate.LocationName = locationControl.selectedName();
            //StartDate
            cswPrivate.controlTbl.cell(2, 1).span({ text: 'Start Date:' }).addClass('propertylabel');
            var startDatePicker = cswPrivate.controlTbl.cell(2, 2).dateTimePicker({
                name: 'startDate',
                Date: cswPrivate.getCurrentDate(),
                isRequired: true,
                maxDate: cswPrivate.getCurrentDate(),
                onChange: function () {
                    if (startDatePicker.val().date > cswPrivate.EndDate) {
                        startDatePicker.setMaxDate(cswPrivate.EndDate);
                    }
                    cswPrivate.StartDate = startDatePicker.val().date;
                }
            });
            cswPrivate.StartDate = startDatePicker.val().date;
            //EndDate
            cswPrivate.controlTbl.cell(3, 1).span({ text: 'End Date:' }).addClass('propertylabel');
            var endDatePicker = cswPrivate.controlTbl.cell(3, 2).dateTimePicker({
                name: 'endDate',
                Date: cswPrivate.getCurrentDate(),
                isRequired: true,
                maxDate: cswPrivate.getCurrentDate(),
                onChange: function () {
                    if (endDatePicker.val().date > cswPrivate.getCurrentDate()) {
                        endDatePicker.setMaxDate(cswPrivate.getCurrentDate());
                    }
                    cswPrivate.EndDate = endDatePicker.val().date;
                    startDatePicker.setMaxDate(cswPrivate.EndDate);
                    cswPrivate.StartDate = startDatePicker.val().date;
                }
            });
            cswPrivate.EndDate = endDatePicker.val().date;
            //UpdateButton
            var updateButton = cswPrivate.controlTbl.cell(4, 2).buttonExt({
                enabledText: 'View Report',
                size: 'small',
                tooltip: { title: 'View Report' },
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.magglass),
                onClick: function() {
                    cswPrivate.loadGrid({
                        LocationId: cswPrivate.LocationId,
                        StartDate: cswPrivate.StartDate,
                        EndDate: cswPrivate.EndDate
                    });
                    updateButton.enable();
                }
            });
        };
        //#endregion Setup Controls
        
        //#region Grid Control
        cswPrivate.loadGrid = function (tierIIRequestData) {
            Csw.ajaxWcf.post({
                urlMethod: 'RegulatoryReporting/getTierIIData',
                data: tierIIRequestData,
                success: function (ajaxdata) {
                    //Grid Data
                    cswPrivate.Materials = [{
                        MaterialId: '',
                        TradeName: '',
                        CASNo: '',
                        MaterialType: '',
                        PhysicalState: '',
                        EHS: false,
                        TradeSecret: false,
                        HazardCategories: '',
                        MaxQty: '',
                        AverageQty: '',
                        DaysOnSite: '',
                        Unit: '',
                        Storage: [{
                            UseType: '',
                            Pressure: '',
                            Temperature: ''
                        }],
                        Locations: [{
                            Location: '',
                            LocationId: ''
                        }]
                    }];
                    Csw.extend(cswPrivate.Materials, ajaxdata.Materials);
                    if (ajaxdata.Materials.length === 0) {
                        cswPrivate.Materials = [];
                    }

                    var TierIIGridData = [];
                    Csw.each(cswPrivate.Materials, function (row) {
                        if (false === Csw.isNullOrEmpty(row.TradeName)) {
                            var HazardCategories = '',
                                UseTypes = '',
                                Pressures = '',
                                Temperatures = '',
                                Locations = '';
                            Csw.each(row.HazardCategories, function (hazard, key) {
                                var newline = key === 0 ? '' : '<br/>';
                                HazardCategories = HazardCategories + newline + hazard;
                            });
                            Csw.each(row.Storage, function (storagecodes, key) {
                                var newline = key === 0 ? '' : '<br/>';
                                UseTypes = UseTypes + newline + storagecodes.UseType;
                                Pressures = Pressures + newline + storagecodes.Pressure;
                                Temperatures = Temperatures + newline + storagecodes.Temperature;
                            });
                            Csw.each(row.Locations, function(location, key) {
                                var newline = key === 0 ? '' : '<br/>';
                                Locations = Locations + newline + location.Location;
                            });
                            TierIIGridData.push({
                                tradename: row.TradeName,
                                casno: row.CASNo,
                                materialtype: row.MaterialType,
                                physicalstate: row.PhysicalState,
                                ehs: row.EHS ? 'Y' : 'N',
                                tradesecret: row.TradeSecret ? 'Y' : 'N',
                                hazardcategories: HazardCategories,
                                mazqty: row.MaxQty + ' ' + row.Unit,
                                avgqty: row.AverageQty + ' ' + row.Unit,
                                daysonsite: row.DaysOnSite,
                                usetype: UseTypes,
                                pressure: Pressures,
                                temperature: Temperatures,
                                locations: Locations
                            });
                        }
                    });

                    //Grid Fields and Columns
                    var TierIIGridColumns = [];
                    var TierIIGridFields = [];
                    var addColumn = function (colName, displayName) {
                        TierIIGridColumns.push({
                            dataIndex: colName,
                            filterable: true,
                            header: displayName,
                            id: 'hmis_' + colName
                        });
                        TierIIGridFields.push({
                            name: colName,
                            type: 'string',
                            useNull: true
                        });
                    };
                    
                    addColumn('tradename', 'TradeName');
                    addColumn('casno', 'CAS No');
                    addColumn('materialtype', 'Material<br/>Type');
                    addColumn('physicalstate', 'Physical<br/>State');
                    addColumn('ehs', 'EHS');
                    addColumn('tradesecret', 'Trade<br/>Secret');
                    addColumn('hazardcategories', 'Physical and<br/>Health Hazards');
                    addColumn('mazqty', 'Max Qty');
                    addColumn('avgqty', 'Average Qty ');
                    addColumn('daysonsite', 'Days On Site');
                    addColumn('usetype', 'Container Type');
                    addColumn('pressure', 'Pressure');
                    addColumn('temperature', 'Temperature');
                    addColumn('locations', 'Storage Locations');

                    //Grid Control
                    var TierIIGridId = 'TierIIGrid';
                    cswPrivate.gridOptions = {
                        name: TierIIGridId,
                        storeId: TierIIGridId,
                        title: 'Tier II Data for ' + cswPrivate.LocationName 
                                + ' from ' + cswPrivate.StartDate
                                + ' to ' + cswPrivate.EndDate,
                        stateId: TierIIGridId,
                        usePaging: false,
                        showActionColumn: false,
                        height: 300,
                        forcefit: true,
                        width: '100%',
                        fields: TierIIGridFields,
                        columns: TierIIGridColumns,
                        data: TierIIGridData,
                        printingEnabled: true
                    };
                    cswPrivate.gridTbl.cell(1, 1).empty();
                    cswPrivate.TierIIGrid = cswPrivate.gridTbl.cell(1, 1).grid(cswPrivate.gridOptions);
                }
            });
        };
        //#endregion Grid Control

        //#region _postCtor
        (function _postCtor() {
            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: 'Tier II Reporting',
                useFinish: false,
                cancelText: 'Close',
                onCancel: cswPrivate.onCancelClick,
                hasButtonGroup: true
            });
            
            cswPrivate.controlTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_control_tbl',
                cellpadding: '5px',
                cellalign: 'left',
                cellvalign: 'middle',
                width: '95%',
                FirstCellRightAlign: true
            });
            
            cswPrivate.gridTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_control_tbl',
                cellpadding: '5px',
                cellalign: 'left',
                align: 'center',
                cellvalign: 'middle',
                width: '95%'
            });

            cswPrivate.createSetupControls();
        }());
        //#endregion _postCtor
        
        return cswPublic;
    });
}());