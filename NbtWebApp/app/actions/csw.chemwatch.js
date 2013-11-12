﻿/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.register('chemwatch', function (cswParent, options) {
        'use strict';

        var cswPrivate = {
            name: 'CswChemWatchAction',
            title: 'ChemWatch'
        };
        cswPrivate.OperationData = {
            NbtMaterialId: options.materialid,
            Countries: [],
            Languages: [],
            Supplier: "",
            Suppliers: [],
            PartNo: "",
            MaterialName: "",
            ChemWatchMaterialId: 1,
            Materials: [],
            SDSDocuments: []
        };
        var cswPublic = {};

        cswPrivate.makeSupplierSelect = function(tbl) {
            var supplierList = cswPrivate.OperationData.Suppliers;
            cswPrivate.supplierSelect = tbl.cell(1, 3);
            cswPrivate.supplierSelect.select({
                values: supplierList
            });

            if (supplierList.length === 0) {
                tbl.cell(1, 4).span({
                    text: 'No Suppliers matched the one provided. Try using a broader search term.'
                }).css({ 'color': 'red' });
            } else {
                tbl.cell(1, 4).empty();
            }
        };

        cswPrivate.makeMatSearchTable = function () {
            var tbl = cswPublic.table.cell(1, 1).table({
                cellPadding: 5
            });

            //#region Supplier
            cswPrivate.supplierInput = tbl.cell(1, 1).input({
                labelText: 'Supplier',
                value: cswPrivate.OperationData.Supplier,
                onChange: function (value) {
                    cswPrivate.OperationData.Supplier = value;

                    Csw.ajaxWcf.post({
                        urlMethod: 'ChemWatch/GetMatchingSuppliers',
                        data: cswPrivate.OperationData,
                        success: function (data) {
                            cswPrivate.OperationData = data;

                            if (cswPrivate.supplierSelect) {
                                cswPrivate.supplierSelect.empty();
                                cswPrivate.makeSupplierSelect(tbl);
                            }
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });

                }
            });

            tbl.cell(1, 2).span({
                text: 'AS'
            });

            cswPrivate.makeSupplierSelect(tbl);
            //#endregion Supplier

            tbl.cell(2, 1).input({
                labelText: 'Material',
                value: cswPrivate.OperationData.MaterialName
            });

            tbl.cell(3, 1).input({
                labelText: 'Part No',
                value: cswPrivate.OperationData.PartNo
            });

            tbl.cell(4, 1).buttonExt({
                name: 'searchChemWatchBtn',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.magglass),
                enabledText: 'Search',
                disabledText: 'Searching...',
                disableOnClick: false,
                onClick: function () {

                    // todo: call the method that will search chemwatch

                    Csw.ajaxWcf.post({
                        urlMethod: 'ChemWatch/MaterialSearch', //todo: Pending discussion to call this 'MaterialSearch'
                        data: cswPrivate.OperationData,
                        success: function (data) {
                            cswPrivate.OperationData = data;

                            // Fill the table with the returned data
                            if (cswPrivate.cswPrivate.materialListGrid && cswPrivate.cswPrivate.materialListGrid.destroy) {
                                cswPrivate.cswPrivate.materialListGrid.destroy();
                            }
                            cswPrivate.makeMatListGrid();
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                }
            });
        };

        cswPrivate.makeMatListGrid = function () {
            cswPrivate.matListGridDiv = cswPublic.table.cell(2, 1).table({
                cellPadding: 5
            });
            cswPrivate.matListGridDiv.empty();

            //todo: span single column the length of the entire grid

            cswPrivate.materialListGrid = cswPrivate.matListGridDiv.cell(1, 1).grid({
                name: 'chemwatchmatlistgrid',
                fields: ['material'],
                columns: [
                    {
                        header: 'Material',
                        dataIndex: 'material'
                    }],
                //data: cswPrivate.Materials,
                data: {
                    'items': [
                        {
                            'material': 'Material 1'
                        },
                        {
                            'material': 'Material 2'
                        },
                        {
                            'material': 'Material 3'
                        }
                    ]
                },
                height: 200,
                width: 400,
                showActionColumn: false,
                onSelect: function (rows) {
                    //todo: fill the SDS grid with data
                    var data = rows;
                    console.log(data);

                    cswPrivate.makeSDSListGrid();

                }
            });
        };

        cswPrivate.makeLngCntrySelects = function () {
            cswPrivate.sdsInfoTbl = cswPublic.table.cell(2, 2).table({
                cellPadding: 5
            });

            //todo: onSelect for either control below, reload the sdsgrid or filter it

            cswPrivate.lngSelect = cswPrivate.sdsInfoTbl.cell(1, 1).select({
                ID: 'chemwatchLngSelect',
                name: 'chemwatchLngSelect',
                selected: '',
                //values: cswPrivate.Languages,
                values: ['language1', 'language1', 'language1', 'language1'],
                width: '',
                onChange: null
            });

            cswPrivate.makeLngCntrySelects = cswPrivate.sdsInfoTbl.cell(1, 2).select({
                ID: 'chemwatchCntrySelect',
                name: 'chemwatchCntrySelect',
                selected: '',
                //values: cswPrivate.Countries,
                values: ['country', 'country', 'country', 'country', 'country', 'country'],
                width: '',
                onChange: null
            });
        };

        cswPrivate.makeSDSListGrid = function () {
            cswPrivate.sdsListGridDiv = cswPrivate.sdsInfoTbl.cell(2, 1).div();
            cswPrivate.sdsListGridDiv.empty();

            cswPrivate.sdsListGridDiv = cswPrivate.sdsListGridDiv.grid({
                name: 'chemwatchsdslistgrid',
                fields: ['view', 'language', 'country', 'select'],
                columns: [
                    { header: 'View', dataIndex: 'view' },
                    { header: 'Language', dataIndex: 'language' },
                    { header: 'Country', dataIndex: 'country' },
                    { header: 'Select', dataIndex: 'select' }
                ],
                //data: cswPrivate.SDSDocuments,
                data: {
                    'items': [
                        {
                            'view': '1111',
                            'language': '2222',
                            'country': '3333',
                            'select': '4444'
                        }]
                },
                height: 200,
                width: 400,
                showActionColumn: false
            });
        };

        cswPrivate.makeCreateSDSLinksBtn = function () {
            cswPrivate.createSDSLinksBtn = cswPublic.table.cell(3, 1).buttonExt({
                name: 'createSDSLinksBtn',
                enabledText: 'Create SDS Links',
                disabledText: 'Creating...',
                disableOnClick: true,
                onClick: function () {
                    // todo: call the method that will create the SDS documents
                    // Return to the material view
                }
            });
        };

        // Init
        (function () {

            // Set up action layout
            var layout = Csw.layouts.action(cswParent, {
                title: 'ChemWatch',
                useFinish: false,
                useCancel: cswPrivate.onCancel
            });

            cswPublic.table = layout.actionDiv.table();

            // call the Initialize web service met
            Csw.ajaxWcf.post({
                urlMethod: 'ChemWatch/Initialize',
                data: cswPrivate.OperationData,
                success: function (data) {
                    cswPrivate.OperationData = data;

                    // call functions to make the controls
                    cswPrivate.makeMatSearchTable();
                    cswPrivate.makeMatListGrid();
                    cswPrivate.makeLngCntrySelects();
                    cswPrivate.makeSDSListGrid();
                    cswPrivate.makeCreateSDSLinksBtn();
                },
                error: function (data) {
                    console.log(data);
                }
            });


        })(); // init
    });

})();