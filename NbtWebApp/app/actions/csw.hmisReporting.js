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
            cswPrivate.onSubmit = cswPrivate.onSubmit || function () {};
            cswPrivate.onCancel = cswPrivate.onCancel || function () {};
            
            cswPrivate.controlZoneId = cswPrivate.controlZoneId || '';
            cswPrivate.FireClassExemptAmountSet = cswPrivate.FireClassExemptAmountSet || '';
            cswPrivate.Materials = cswPrivate.Materials || [];
            cswPrivate.CachedHMISData = [];

            cswParent.empty();
        }());
        //#endregion _preCtor

        //#region Action Functions
        cswPrivate.onSubmitClick = function() {
            Csw.tryExec(cswPrivate.onSubmit);
        };

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
                    Csw.each(cswPrivate.Materials, function(row) {
                        HMISGridData.push({
                            material: row.Material,
                            hazardclass: row.HazardClass,
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
                        var state = headerDisplay === 'Solid' ? ' lbs' : '';
                        state = headerDisplay === 'Liquid' ? ' gal' : state;
                        state = headerDisplay === 'Gas' ? ' cu.ft.' : state;
                        Columns.push({
                            text: headerDisplay,
                            columns: [{
                                    dataIndex: maqColName,
                                    filterable: false,
                                    header: 'MAQ',
                                    id: 'hmis_' + maqColName,
                                    width: 80,
                                    summaryType: 'max',
                                    renderer: function(value, metaData, record, rowIdx, colIdx, store, view) {
                                        return value === 0 ? '' : value + state;
                                    },
                                    summaryRenderer: function(value, summaryData, dataIndex) {
                                        return value === 0 ? '' : value + state;
                                    }
                                }, {
                                    dataIndex: qtyColName,
                                    filterable: false,
                                    header: 'Qty',
                                    id: 'hmis_' + qtyColName,
                                    width: 80,
                                    summaryType: 'sum',
                                    renderer: function(value, metaData, record, rowIdx, colIdx, store, view) {
                                        return value === 0 ? '' : value + state;
                                    },
                                    summaryRenderer: function(value, summaryData, dataIndex) {
                                        return value === 0 ? '' : value + state;
                                    }
                                }]
                        });
                        HMISGridFields.push({
                            name: maqColName,
                            type: 'float',
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
                        addQtyColumn(Columns, useType + 'solidmaq', useType + 'solidqty', 'Solid');
                        addQtyColumn(Columns, useType + 'liquidmaq', useType + 'liquidqty', 'Liquid');
                        if (useGas) {
                            addQtyColumn(Columns, useType + 'gasmaq', useType + 'gasqty', 'Gas');
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
            var SummaryGridData = [];
            var gridColumns = [
                { name: 'material', display: '&nbsp;<br/>&nbsp;<br/>QtyMaterial' },
                { name: 'hazardclass', display: '&nbsp;<br/>&nbsp;<br/>HazardClass' },
                { name: 'storagesolidmaq', display: 'Storage<br/>Solid<br/>MAQ' },
                { name: 'storagesolidqty', display: '&nbsp;<br/>&nbsp;<br/>Qty' },
                { name: 'storageliquidmaq', display: '&nbsp;<br/>Liquid<br/>MAQ' },
                { name: 'storageliquidqty', display: '&nbsp;<br/>&nbsp;<br/>Qty' },
                { name: 'storagegasmaq', display: '&nbsp;<br/>Gas<br/>MAQ' },
                { name: 'storagegasqty', display: '&nbsp;<br/>&nbsp;<br/>Qty' },
                { name: 'closedsolidmaq', display: 'Closed<br/>Solid<br/>MAQ' },
                { name: 'closedsolidqty', display: '&nbsp;<br/>&nbsp;<br/>Qty' },
                { name: 'closedliquidmaq', display: '&nbsp;<br/>Liquid<br/>MAQ' },
                { name: 'closedliquidqty', display: '&nbsp;<br/>&nbsp;<br/>Qty' },
                { name: 'closedgasmaq', display: '&nbsp;<br/>Gas<br/>MAQ' },
                { name: 'closedgasqty', display: '&nbsp;<br/>&nbsp;<br/>Qty' },
                { name: 'opensolidmaq', display: 'Open<br/>Solid<br/>MAQ' },
                { name: 'opensolidqty', display: '&nbsp;<br/>&nbsp;<br/>Qty' },
                { name: 'openliquidmaq', display: '&nbsp;<br/>Liquid<br/>MAQ' },
                { name: 'openliquidqty', display: '&nbsp;<br/>&nbsp;<br/>Qty' }
            ];
            SummaryGridColumns.push({
                dataIndex: gridColumns[1].name,
                header: gridColumns[1].display,
                id: 'hmis_' + gridColumns[1].name
            });
            SummaryGridFields.push({
                name: gridColumns[1].name,
                type: 'string',
                useNull: true
            });
            var i = 0;
            window.Ext.Array.each(grid.extGrid.view.el.query('tr.x-grid-row-summary'), function (group) {
                var GroupName = grid.extGrid.getStore().getGroups()[i].name;
                var HazardData = {};
                HazardData.hazardclass = GroupName;
                var j = 0;
                window.Ext.Array.each(group.childNodes, function (cell) {
                    if (j > 1) {
                        var summaryValue = cell.textContent;
                        if (i === 0) {
                            SummaryGridColumns.push({
                                dataIndex: gridColumns[j].name,
                                header: gridColumns[j].display,
                                id: 'hmis_' + gridColumns[j].name
                            });
                            SummaryGridFields.push({
                                name: gridColumns[j].name,
                                type: 'string',
                                useNull: true
                            });
                        }
                        HazardData[gridColumns[j].name] = summaryValue;
                    }
                    j++;
                });
                SummaryGridData.push(HazardData);
                i++;
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
        
        //#endregion Summary Grid

        //#region _postCtor
        (function _postCtor() {
            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: 'HMIS Reporting',
                finishText: 'Finish',
                onFinish: cswPrivate.onSubmitClick,
                onCancel: cswPrivate.onCancelClick
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