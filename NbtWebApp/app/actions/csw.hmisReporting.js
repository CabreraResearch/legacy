/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {
Csw.actions.hmisReporting = Csw.actions.template ||
    Csw.actions.register('hmisReporting', function (cswParent, cswPrivate) {
        'use strict';
        var cswPublic = {};
        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException('Cannot create an action without a valid Csw Parent object.', 'Csw.actions.hmisReporting', 'csw.hmisReporting.js', 10);
        }
        
        //#region _preCtor
        (function _preCtor() {
            cswPrivate.name = cswPrivate.name || 'HMIS Reporting';
            cswPrivate.onCancel = cswPrivate.onCancel || function () {};
            
            cswPrivate.controlZoneId = cswPrivate.controlZoneId || '';
            cswPrivate.FireClassExemptAmountSet = cswPrivate.FireClassExemptAmountSet || '';
            cswPrivate.Materials = cswPrivate.Materials || [];
            cswPrivate.CachedHMISData = [];

            cswParent.empty();
        }());
        //#endregion _preCtor

        //#region Action Functions
        cswPrivate.onCancelClick = function() {
            Csw.tryExec(cswPrivate.onCancel);
        };
        //#endregion Action Functions

        //#region Control Zone Control
        cswPrivate.makeControlZoneControl = function() {
            cswPrivate.controlTbl.cell(1, 1).span({ text: 'Select Control Zone: ' }).addClass('propertylabel');
            cswPrivate.controlZoneSelect = cswPrivate.controlTbl.cell(1, 2).nodeSelect({
                name: 'Control Zone',
                async: false,
                width: '200px',
                showSelectOnLoad: true,
                isRequired: false,
                nodeTypeName: 'Control Zone',
                objectClassName: 'GenericClass',
                onChange: function () {
                    if (cswPrivate.controlZoneSelect.val() !== '') {
                        cswPrivate.controlZoneId = cswPrivate.controlZoneSelect.val();
                        var cached = false;
                        Csw.each(cswPrivate.CachedHMISData, function(czData) {
                            if (czData.controlZoneId === cswPrivate.controlZoneId) {
                                cached = true;
                                cswPrivate.gridTbl.cell(1, 1).empty();
                                cswPrivate.HMISGrid = cswPrivate.gridTbl.cell(1, 1).grid(czData.gridOptions);
                            }
                        });
                        if (false === cached) {
                            cswPrivate.loadGrid({
                                ControlZoneId: cswPrivate.controlZoneId
                            });
                        }
                    }
                }
            });
        };
        //#endregion Control Zone Control

        //#region Grid Control
        cswPrivate.loadGrid = function (hmisRequestData) {
            cswPrivate.gridTbl.cell(2, 1).span({ text: 'Loading Grid...' }).addClass('propertylabel');
            Csw.ajaxWcf.post({
                urlMethod: 'RegulatoryReporting/getHMISData',
                data: hmisRequestData,
                success: function (ajaxdata) {
                    cswPrivate.gridTbl.cell(2, 1).empty();

                    //FireClassExemptAmountSet Label
                    cswPrivate.FireClassExemptAmountSet = ajaxdata.FireClassExemptAmountSet || 'N/A';
                    cswPrivate.controlTbl.cell(2, 1).empty();
                    cswPrivate.controlTbl.cell(2, 1).span({ text: 'Fire Class Exempt Amount Set: ' }).addClass('propertylabel');
                    cswPrivate.controlTbl.cell(2, 2).empty();
                    cswPrivate.controlTbl.cell(2, 2).span({ text: cswPrivate.FireClassExemptAmountSet });

                    //Grid Data
                    cswPrivate.Materials = [{
                        Material: '',
                        HazardClass: '',
                        Storage: {
                            Solid: { MAQ: '', Qty: '' },
                            Liquid: { MAQ: '', Qty: '' },
                            Gas: { MAQ: '', Qty: '' }
                        },
                        Closed: {
                            Solid: { MAQ: '', Qty: '' },
                            Liquid: { MAQ: '', Qty: '' },
                            Gas: { MAQ: '', Qty: '' }
                        },
                        Open: {
                            Solid: { MAQ: '', Qty: '' },
                            Liquid: { MAQ: '', Qty: '' }
                        }
                    }];
                    Csw.extend(cswPrivate.Materials, ajaxdata.Materials);
                    if (ajaxdata.Materials.length === 0) {
                        cswPrivate.Materials = [];
                    }
                    var HMISGridData = [];
                    Csw.each(cswPrivate.Materials, function (row) {
                        if (false === Csw.isNullOrEmpty(row.Material)) {
                            HMISGridData.push({
                                material: row.Material,
                                hazardclass: row.HazardCategory + ' ' + row.Class,
                                storagesolidmaq: row.Storage.Solid.MAQ,
                                storagesolidqty: row.Storage.Solid.Qty,
                                storageliquidmaq: row.Storage.Liquid.MAQ,
                                storageliquidqty: row.Storage.Liquid.Qty,
                                storagegasmaq: row.Storage.Gas.MAQ,
                                storagegasqty: row.Storage.Gas.Qty,
                                closedsolidmaq: row.Closed.Solid.MAQ,
                                closedsolidqty: row.Closed.Solid.Qty,
                                closedliquidmaq: row.Closed.Liquid.MAQ,
                                closedliquidqty: row.Closed.Liquid.Qty,
                                closedgasmaq: row.Closed.Gas.MAQ,
                                closedgasqty: row.Closed.Gas.Qty,
                                opensolidmaq: row.Open.Solid.MAQ,
                                opensolidqty: row.Open.Solid.Qty,
                                openliquidmaq: row.Open.Liquid.MAQ,
                                openliquidqty: row.Open.Liquid.Qty
                            });
                        }
                    });

                    //Grid Fields and Columns
                    var HMISGridColumns = [];
                    var HMISGridFields = [];
                    var addColumn = function(colName, displayName, summaryType, summaryRenderer) {
                        HMISGridColumns.push({
                            dataIndex: colName,
                            filterable: false,
                            header: displayName,
                            id: 'hmis_' + colName,
                            summaryType: summaryType,
                            summaryRenderer: summaryRenderer
                        });
                        HMISGridFields.push({
                            name: colName,
                            type: 'string',
                            useNull: true
                        });
                    };
                    var addQtyColumn = function(Columns, maqColName, qtyColName, headerDisplay) {
                        Columns.push({
                            text: headerDisplay,
                            columns: [{
                                dataIndex: maqColName,
                                filterable: false,
                                header: 'MAQ',
                                id: 'hmis_' + maqColName,
                                width: 80,
                                summaryType: 'max',
                                summaryRenderer: function(value, summaryData, dataIndex) {
                                    return value === '' ? '' : value;
                                }
                            }, {
                                dataIndex: qtyColName,
                                filterable: false,
                                header: 'Qty',
                                id: 'hmis_' + qtyColName,
                                width: 80,
                                summaryType: 'sum'
                            }]
                        });
                        HMISGridFields.push({
                            name: maqColName,
                            type: 'string',
                            useNull: true
                        });
                        HMISGridFields.push({
                            name: qtyColName,
                            type: 'float',
                            useNull: true
                        });
                    };
                    var addUseTypeColumn = function(useType, headerDisplay, useGas) {
                        var Columns = [];
                        addQtyColumn(Columns, useType + 'solidmaq', useType + 'solidqty', 'Solid - lbs');
                        addQtyColumn(Columns, useType + 'liquidmaq', useType + 'liquidqty', 'Liquid - gal (lbs)');
                        if (useGas) {
                            addQtyColumn(Columns, useType + 'gasmaq', useType + 'gasqty', 'Gas - cu.ft.');
                        }
                        HMISGridColumns.push({
                            text: headerDisplay,
                            columns: Columns
                        });
                    };
                    addColumn('material', 'Material', 'count', function(value, summaryData, dataIndex) {
                        return ((value === 1) ? '(1 Material)' : '(' + value + ' Materials)');
                    });
                    addColumn('hazardclass', 'Hazard Class');
                    addUseTypeColumn('storage', 'Storage', true);
                    addUseTypeColumn('closed', 'Closed', true);
                    addUseTypeColumn('open', 'Open', false);

                    //Grid Control
                    var HMISGridId = 'HMISGrid';
                    cswPrivate.gridOptions = {
                        name: HMISGridId,
                        storeId: HMISGridId,
                        title: 'HMIS Data for ' + cswPrivate.controlZoneSelect.selectedName(),
                        stateId: HMISGridId,
                        usePaging: false,
                        showActionColumn: false,
                        height: 300,
                        forcefit: true,
                        width: '100%',
                        fields: HMISGridFields,
                        columns: HMISGridColumns,
                        data: HMISGridData,
                        groupField: 'hazardclass',
                        groupHeaderTpl: '{name}',
                        summaryEnabled: true,
                        printingEnabled: true,
                        gridToPrint: function(grid) {
                            return cswPrivate.makeSummaryGrid(grid);
                        }
                    };
                    cswPrivate.CachedHMISData.push({ 
                        controlZoneId: cswPrivate.controlZoneId, 
                        gridOptions: cswPrivate.gridOptions
                    });
                    cswPrivate.gridTbl.cell(1, 1).empty();
                    cswPrivate.HMISGrid = cswPrivate.gridTbl.cell(1, 1).grid(cswPrivate.gridOptions);
                }
            });
        };
        //#endregion Grid Control
        
        //#region Summary Grid

        cswPrivate.makeSummaryGrid = function(grid) {
            var SummaryGridColumns = [];
            var SummaryGridFields = [];
            var SummaryGridData = cswPrivate.buildHazardClassEmptyDataset();
            var gridColumns = [
                { name: 'material', display: '&nbsp;<br/>&nbsp;<br/>QtyMaterial' },
                { name: 'hazardclass', display: '&nbsp;<br/>&nbsp;<br/>HazardClass' },
                { name: 'storagesolidmaq', display: 'Storage<br/>Solid - lbs<br/>MAQ' },
                { name: 'storagesolidqty', display: 'Storage<br/>Solid - lbs<br/>Qty' },
                { name: 'storageliquidmaq', display: 'Storage<br/>Liquid - gal (lbs)<br/>MAQ' },
                { name: 'storageliquidqty', display: 'Storage<br/>Liquid - gal<br/>Qty' },
                { name: 'storagegasmaq', display: 'Storage<br/>Gas - cu.ft.<br/>MAQ' },
                { name: 'storagegasqty', display: 'Storage<br/>Gas - cu.ft.<br/>Qty' },
                { name: 'closedsolidmaq', display: 'Closed<br/>Solid - lbs<br/>MAQ' },
                { name: 'closedsolidqty', display: 'Closed<br/>Solid - lbs<br/>Qty' },
                { name: 'closedliquidmaq', display: 'Closed<br/>Liquid - gal (lbs)<br/>MAQ' },
                { name: 'closedliquidqty', display: 'Closed<br/>Liquid - gal<br/>Qty' },
                { name: 'closedgasmaq', display: 'Closed<br/>Gas - cu.ft.<br/>MAQ' },
                { name: 'closedgasqty', display: 'Closed<br/>Gas - cu.ft.<br/>Qty' },
                { name: 'opensolidmaq', display: 'Open<br/>Solid - lbs<br/>MAQ' },
                { name: 'opensolidqty', display: 'Open<br/>Solid - lbs<br/>Qty' },
                { name: 'openliquidmaq', display: 'Open<br/>Liquid - gal (lbs)<br/>MAQ' },
                { name: 'openliquidqty', display: 'Open<br/>Liquid - gal<br/>Qty' }
            ];
            var rowIdx = 0;
            window.Ext.Array.each(grid.extGrid.view.el.query('tr.x-grid-row-summary'), function (group) {
                var GroupName = grid.extGrid.getStore().getGroups()[rowIdx].name;
                Csw.each(SummaryGridData, function (row) {
                    if (row.hazardclass === GroupName) {
                        var colIdx = 0;
                        window.Ext.Array.each(group.childNodes, function(cell) {
                            if (colIdx > 0) {
                                var summaryValue = colIdx === 1 ? GroupName : cell.textContent;
                                if (rowIdx === 0) {
                                    SummaryGridColumns.push({
                                        dataIndex: gridColumns[colIdx].name,
                                        header: gridColumns[colIdx].display,
                                        id: 'hmis_' + gridColumns[colIdx].name
                                    });
                                    SummaryGridFields.push({
                                        name: gridColumns[colIdx].name,
                                        type: 'string',
                                        useNull: true
                                    });
                                }
                                row[gridColumns[colIdx].name] = summaryValue;
                            }
                            colIdx++;
                        });
                    }
                });
                rowIdx++;
            }, grid.extGrid);

            cswPrivate.gridTbl.cell(2, 1).empty();
            var summaryGrid = cswPrivate.gridTbl.cell(2, 1).grid({
                name: 'SummaryGrid',
                storeId: 'SummaryGrid',
                title: 'HMIS Report for ' + cswPrivate.controlZoneSelect.selectedName(),
                stateId: 'SummaryGrid',
                usePaging: false,
                showActionColumn: false,
                fields: SummaryGridFields,
                columns: SummaryGridColumns,
                data: SummaryGridData,
                onPrintSuccess: function () {
                    cswPrivate.gridTbl.cell(2, 1).empty();
                    cswPrivate.gridTbl.cell(1, 1).empty();
                    cswPrivate.HMISGrid = cswPrivate.gridTbl.cell(1, 1).grid(cswPrivate.gridOptions);
                }
            }).hide();
            return summaryGrid;
        };
        
        cswPrivate.buildHazardClassEmptyDataset = function () {
            var EmptyHazardClasses = [];
            Csw.each(cswPrivate.Materials, function (row) {
                var exists = false;
                Csw.each(EmptyHazardClasses, function(hazardClass) {
                    if (hazardClass.hazardclass === EmptyHazardClasses.hazardclass) {
                        exists = true;
                    }
                });
                if (false === exists) {
                    EmptyHazardClasses.push({
                        material: row.Material,
                        hazardclass: row.HazardCategory + ' ' + row.Class,
                        storagesolidmaq: row.Storage.Solid.MAQ,
                        storagesolidqty: row.Storage.Solid.Qty,
                        storageliquidmaq: row.Storage.Liquid.MAQ,
                        storageliquidqty: row.Storage.Liquid.Qty,
                        storagegasmaq: row.Storage.Gas.MAQ,
                        storagegasqty: row.Storage.Gas.Qty,
                        closedsolidmaq: row.Closed.Solid.MAQ,
                        closedsolidqty: row.Closed.Solid.Qty,
                        closedliquidmaq: row.Closed.Liquid.MAQ,
                        closedliquidqty: row.Closed.Liquid.Qty,
                        closedgasmaq: row.Closed.Gas.MAQ,
                        closedgasqty: row.Closed.Gas.Qty,
                        opensolidmaq: row.Open.Solid.MAQ,
                        opensolidqty: row.Open.Solid.Qty,
                        openliquidmaq: row.Open.Liquid.MAQ,
                        openliquidqty: row.Open.Liquid.Qty
                    });
                }
            });
            return EmptyHazardClasses;
        };
        
        //#endregion Summary Grid

        //#region _postCtor
        (function _postCtor() {
            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: 'HMIS Reporting',
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

            cswPrivate.makeControlZoneControl();
        }());
        //#endregion _postCtor
        
        return cswPublic;
    });
}());