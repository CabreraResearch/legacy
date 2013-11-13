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

        //Once these are set by Init, they shouldn't be touched
        var LanguageOptions = [];
        var CountryOptions = [];
        var LanguageLookup = {};
        var CountryLookup = {};

        var selectedLanguages = [];
        var selectedCountries = [];

        cswPrivate.makeSupplierSelect = function (tbl) {
            var supplierList = cswPrivate.OperationData.Suppliers;
            cswPrivate.supplierSelectCell = tbl.cell(1, 3);
            cswPrivate.supplierSelect = cswPrivate.supplierSelectCell.select({
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
                                cswPrivate.supplierSelectCell.empty();
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

            cswPrivate.materialInput = tbl.cell(2, 1).input({
                labelText: 'Material',
                value: cswPrivate.OperationData.MaterialName
            });

            cswPrivate.partNoInput = tbl.cell(3, 1).input({
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
                    Csw.ajaxWcf.post({
                        urlMethod: 'ChemWatch/MaterialSearch',
                        data: {
                            Supplier: cswPrivate.supplierSelect.selectedVal(),
                            MaterialName: cswPrivate.materialInput.val(),
                            PartNo: cswPrivate.partNoInput.val()
                        },
                        success: function (data) {
                            cswPrivate.OperationData = data;

                            var gridData = { 'items': [] };
                            Csw.iterate(cswPrivate.OperationData.Materials, function (material) {
                                gridData.items.push({ 'material': material.text, 'materialid': material.value });
                            });

                            // Fill the table with the returned data
                            if (cswPrivate.materialListGrid && cswPrivate.materialListGrid.destroy) {
                                cswPrivate.materialListGrid.destroy();
                            }

                            cswPrivate.makeMatListGrid(gridData);
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                }
            });
        };

        cswPrivate.makeMatListGrid = function (gridData) {
            cswPrivate.matListGridDiv = cswPublic.table.cell(2, 1).table({
                cellPadding: 5
            });
            cswPrivate.matListGridDiv.empty();

            cswPrivate.materialListGrid = cswPrivate.matListGridDiv.cell(1, 1).grid({
                name: 'chemwatchmatlistgrid',
                fields: ['material', 'materialid'],
                columns: [
                    {
                        header: 'Material',
                        dataIndex: 'material',
                        width: 400
                    },
                    {
                        header: 'Material Id',
                        dataIndex: 'materialid',
                        hidden: true
                    }],
                data: gridData || {
                    'items': [] //ExtGrids won't show without data
                },
                height: 200,
                width: 400,
                showActionColumn: false,
                canSelectRow: true,
                onSelect: function (rows) {
                    // Search for documents related to selected material
                    Csw.ajaxWcf.post({
                        urlMethod: 'ChemWatch/SDSDocumentSearch',
                        data: {
                            ChemWatchMaterialId: rows.materialid,
                            Languages: selectedLanguages,
                            Countries: selectedCountries,
                            Supplier: cswPrivate.supplierSelect.val()
                        },
                        success: function (data) {
                            cswPrivate.OperationData = data;

                            var gridData = { 'items': [] };
                            Csw.iterate(cswPrivate.OperationData.SDSDocuments, function (sdsdoc) {
                                gridData.items.push({ 'language': sdsdoc.language, 'country': sdsdoc.country, 'file': sdsdoc.file });
                            });

                            // Destroy current grid
                            if (cswPrivate.sdsListGrid && cswPrivate.sdsListGrid.destroy) {
                                cswPrivate.sdsListGrid.destroy();
                            }
                            // Remake grid
                            cswPrivate.makeSDSListGrid(gridData);
                        },
                        error: function (data) {
                            //todo: implement error condition
                            console.log(data);
                        }
                    });
                }
            });
        };

        cswPrivate.makeLngCntrySelects = function () {
            cswPrivate.sdsInfoTbl = cswPublic.table.cell(2, 2).table({
                cellPadding: 5
            });

            var langDiv = cswPrivate.sdsInfoTbl.cell(1, 1).div();
            langDiv.span({ text: 'Edit Languages: ' });
            langDiv.icon({
                iconType: Csw.enums.iconType.pencil,
                isButton: true,
                onClick: function () {
                    Csw.dialogs.multiselectedit({
                        opts: LanguageOptions,
                        title: 'Edit Language Filters',
                        inDialog: true,
                        onSave: function (updatedValues) {
                            Csw.iterate(LanguageOptions, function (lang) {
                                    lang.selected = false;
                            });

                            selectedLanguages = [];
                            Csw.iterate(updatedValues, function (val) {
                                selectedLanguages.push({ 'text': '', 'value': val });
                                LanguageLookup[val].selected = true;
                            });
                        },
                    });
                }
            });

            var countryDiv = cswPrivate.sdsInfoTbl.cell(1, 2).div();
            countryDiv.span({ text: 'Edit Countries: ' });
            countryDiv.icon({
                iconType: Csw.enums.iconType.pencil,
                isButton: true,
                onClick: function () {
                    Csw.dialogs.multiselectedit({
                        opts: CountryOptions,
                        title: 'Edit Country Filters',
                        inDialog: true,
                        onSave: function (updatedValues) {
                            Csw.iterate(CountryOptions, function (country) {
                                    country.selected = false;
                            });

                            selectedCountries = [];
                            Csw.iterate(updatedValues, function (val) {
                                selectedCountries.push({ 'text': '', 'value': val });
                                CountryLookup[val].selected = true;
                            });
                        },
                    });
                }
            });
        };

        cswPrivate.makeSDSListGrid = function (gridData) {
            cswPrivate.sdsListGridCell = cswPrivate.sdsInfoTbl.cell(2, 1);
            cswPrivate.sdsListGridCell.empty();

            cswPrivate.sdsListGrid = cswPrivate.sdsListGridCell.grid({
                name: 'chemwatchsdslistgrid',
                fields: ['view', 'language', 'country', 'select', 'file'],
                columns: [
                    { header: 'View', dataIndex: 'view' },
                    { header: 'Language', dataIndex: 'language' },
                    { header: 'Country', dataIndex: 'country' },
                    { header: 'Select', dataIndex: 'select' },
                    { header: 'FileInfo', dataIndex: 'file', hidden: true }
                ],
                data: gridData || {
                    'items': [] //ExtGrids won't show without data
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

                    LanguageOptions = cswPrivate.OperationData.Languages;
                    CountryOptions = cswPrivate.OperationData.Countries;

                    Csw.iterate(LanguageOptions, function(lang) {
                        LanguageLookup[lang.value] = lang;
                    });
                    Csw.iterate(CountryOptions, function (country) {
                        CountryLookup[country.value] = country;
                    });

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