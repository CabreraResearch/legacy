/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    var cswChemWatchActionStateName = 'cswChemWatchActionStateName';
    Csw.actions.register('chemwatch', function (cswParent, options) {
        'use strict';

        var cswPrivate = {
            name: 'CswChemWatchAction',
            title: 'ChemWatch',
            onCancel: options.onCancel || null,
            onFinish: options.onFinish || null,
            state: {
                materialId: ''
            }
        };
        cswPrivate.OperationData = {
            NbtMaterialId: '',
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

        cswPrivate.makeStepOne = function () {
            var stepOneTable = cswPublic.table.cell(1, 1).table({
                width: '100%',
                cellpadding: 3
            });
            cswPrivate.stepOneTable = stepOneTable;

            // Supplier
            cswPrivate.supplierInput = stepOneTable.cell(1, 1).empty().css({ width: '350px' });
            cswPrivate.supplierInput.input({
                labelText: 'Supplier Name contains: ',
                value: cswPrivate.OperationData.Supplier,
                onChange: function (value) {

                    // Clear steps two and three if they exist
                    if (cswPrivate.stepTwoTable) {
                        cswPrivate.stepTwoTable.empty();
                    }
                    if (cswPrivate.stepThreeTable) {
                        cswPrivate.stepThreeTable.empty();
                    }

                    Csw.ajaxWcf.post({
                        urlMethod: 'ChemWatch/GetMatchingSuppliers',
                        data: {
                            Supplier: value
                        },
                        success: function (data) {
                            cswPrivate.OperationData.Suppliers = data.Suppliers;

                            if (cswPrivate.supplierSelect) {
                                makeSupplierSelect(stepOneTable);
                            }
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });

                }
            });

            var cell12 = stepOneTable.cell(1, 2).empty().css({ width: '30px' });
            cell12.span({ text: 'AS' });

            makeSupplierSelect(stepOneTable);

            // Material name
            cswPrivate.materialInput = stepOneTable.cell(2, 1).input({
                labelText: 'Material Name begins: ',
                value: cswPrivate.OperationData.MaterialName
            });

            // Part number
            cswPrivate.partNoInput = stepOneTable.cell(3, 1).input({
                labelText: 'Part Number is: ',
                value: cswPrivate.OperationData.PartNo
            });

            // Search button
            var searchBtnDisabled = true;
            if (cswPrivate.supplierSelect.length() > 0) {
                searchBtnDisabled = false;
            }
            cswPrivate.searchButton = stepOneTable.cell(4, 1).buttonExt({
                name: 'searchChemWatchBtn',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.magglass),
                enabledText: 'Search Materials',
                disabled: searchBtnDisabled,
                disableOnClick: false,
                onClick: function () {

                    // Clear Step 3 (if it has been rendered)
                    if (cswPrivate.stepThreeTable) {
                        cswPrivate.stepThreeTable.empty();
                    }

                    Csw.ajaxWcf.post({
                        urlMethod: 'ChemWatch/MaterialSearch',
                        data: {
                            Supplier: cswPrivate.supplierSelect.selectedVal(),
                            MaterialName: cswPrivate.materialInput.val(),
                            PartNo: cswPrivate.partNoInput.val()
                        },
                        success: function (data) {
                            cswPrivate.OperationData.Materials = data.Materials;
                            cswPrivate.makeStepTwo();
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                }
            });

            stepOneTable.cell(5, 1).div({
                text: '<br/>'
            });
        };//makeStepOne()

        cswPrivate.makeStepTwo = function () {
            var stepTwoTable = cswPublic.table.cell(2, 1).empty().table({
                width: '80%',
                cellpadding: 5
            });
            cswPrivate.stepTwoTable = stepTwoTable;

            stepTwoTable.cell(1, 1).div({
                text: 'Matching Materials',
                styles: {
                    'font-weight': 'bold',
                    'font-size': '14px'
                }
            });

            if (cswPrivate.OperationData.Materials.length > 0) {
                // Material select
                makeMaterialSelect(stepTwoTable);

                // Language multi-select
                var langDiv = stepTwoTable.cell(2, 2).empty().css({ width: '100px' });
                langDiv.div();
                langDiv.span({ text: 'Languages: ' });
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

                // Country multi-select
                var countryDiv = stepTwoTable.cell(2, 3).empty().css({ width: '100px' });
                countryDiv.div();
                countryDiv.span({ text: 'Countries: ' });
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

                // Document Search button
                cswPrivate.documentSrchBtn = stepTwoTable.cell(2, 4).buttonExt({
                    name: 'searchDocumentsBtn',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.magglass),
                    enabledText: 'Search Documents',
                    disableOnClick: false,
                    onClick: function () {
                        // Search for documents related to selected material
                        Csw.ajaxWcf.post({
                            urlMethod: 'ChemWatch/SDSDocumentSearch',
                            data: {
                                ChemWatchMaterialId: cswPrivate.materialSelect.val(),
                                Languages: selectedLanguages,
                                Countries: selectedCountries,
                                Supplier: cswPrivate.supplierSelect.val()
                            },
                            success: function (data) {
                                cswPrivate.OperationData.SDSDocuments = data.SDSDocuments;

                                var gridData = { 'items': [] };
                                Csw.iterate(cswPrivate.OperationData.SDSDocuments, function (sdsdoc) {
                                    gridData.items.push(
                                        {
                                            'language': sdsdoc.language,
                                            'country': sdsdoc.country,
                                            'filename': sdsdoc.filename,
                                            'externalurl': sdsdoc.externalurl
                                        });
                                });
                                cswPrivate.makeStepThree(gridData);
                            },
                            error: function (data) {
                                //todo: implement error condition
                                console.log(data);
                            }
                        });
                    }
                });

                stepTwoTable.cell(3, 1).div({
                    text: '<br/>'
                });
            } else {
                stepTwoTable.cell(2, 1).div({
                    text: 'No matching materials were found. Try using broader search terms.',
                    styles: {
                        'color': 'red'
                    }
                });
            }

        };//makeStepTwo()

        cswPrivate.makeStepThree = function (gridData) {
            var stepThreeTable = cswPublic.table.cell(3, 1).empty().table({
                width: '80%',
                cellpadding: 5
            });
            cswPrivate.stepThreeTable = stepThreeTable;

            stepThreeTable.cell(1, 1).div({
                text: 'Matching SDS Documents',
                styles: {
                    'font-weight': 'bold',
                    'font-size': '14px'
                }
            });

            if (gridData.items.length > 0) {
                makeSDSListGrid(stepThreeTable, gridData);
            } else {
                stepThreeTable.cell(2, 1).div({
                    text: 'No documents were found. Choose another material.',
                    styles: {
                        'color': 'red'
                    }
                });
            }


        };//makeStepThree()

        cswPrivate.createSDSLinks = function () {
            var rawRows = [];
            if (cswPrivate.sdsDocumentGrid) {
                rawRows = cswPrivate.sdsDocumentGrid.getGridItems();
            }
            var sdsdocs = [];
            if (rawRows && rawRows.length > 0) {
                rawRows.forEach(function (item) {
                    if (item && item.select === true) {
                        sdsdocs.push(item);
                    }
                });
            }

            // Create the SDS Documents
            Csw.ajaxWcf.post({
                urlMethod: 'ChemWatch/CreateSDSDocuments',
                data: {
                    SDSDocuments: sdsdocs,
                    NbtMaterialId: cswPrivate.OperationData.NbtMaterialId
                },
                success: function (data) {
                    Csw.tryExec(cswPrivate.onFinish);
                },
                error: function (data) {
                    //TODO: implement error condition
                    console.log(data);
                }
            });
        };

        //#region Helper Functions
        function makeSupplierSelect(table) {
            var supplierList = cswPrivate.OperationData.Suppliers;
            cswPrivate.supplierSelectCell = table.cell(1, 3).empty().css({ width: '400px' });
            cswPrivate.supplierSelect = cswPrivate.supplierSelectCell.select({
                values: supplierList
            });

            if (supplierList.length === 0) {
                var cell14 = table.cell(1, 4).empty().css({ width: '300px' });
                cell14.span({
                    text: 'No Suppliers matched the one provided. Try using a broader search term.'
                }).css({ 'color': 'red' });
                if (cswPrivate.searchButton) {
                    cswPrivate.searchButton.disable();
                }
            } else {
                table.cell(1, 4).empty();
                if (cswPrivate.searchButton) {
                    cswPrivate.searchButton.enable();
                }
            }
        }//makeSupplierSelect()

        function onView(recordData) {
            //TODO: What if the URL doesn't work? Should we show an error?
            var url = 'Services/ChemWatch/GetSDSDocument' + '?' + 'filename=' + recordData.filename;
            Csw.openPopup(url);
        };//onView()

        function makeMaterialSelect(table) {
            var materialList = cswPrivate.OperationData.Materials;
            cswPrivate.materialSelectCell = table.cell(2, 1).empty().css({ width: '500px' });
            cswPrivate.materialSelect = cswPrivate.materialSelectCell.select({
                values: materialList
            });
        }//makeMaterialSelect()

        function makeSDSListGrid(table, gridData) {
            cswPrivate.sdsDocumentGridCell = table.cell(2, 1).empty();

            // Destroy the grid first if it has been made previously
            if (cswPrivate.sdsDocumentGrid && cswPrivate.sdsDocumentGrid.destroy) {
                cswPrivate.sdsDocumentGrid.destroy();
            }

            cswPrivate.sdsDocumentGrid = cswPrivate.sdsDocumentGridCell.grid({
                name: 'chemwatchsdslistgrid',
                stateId: 'chemwatchsdslistgrid',
                fields: [
                    'view',
                    'language',
                    'country',
                    { name: 'select', type: 'bool' },
                    'filename',
                    'externalurl'
                ],
                columns: [
                    {
                        header: 'View',
                        dataIndex: 'view',
                        menuDisabled: true,
                        sortable: false,
                        width: 40,
                        flex: false,
                        resizable: false,
                        xtype: 'actioncolumn',
                        renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                            var divId = 'view' + rowIndex + colIndex;
                            Csw.defer(Csw.method(function () {

                                var div = Csw.domNode({
                                    ID: divId,
                                    tagName: 'DIV'
                                });
                                div.empty();

                                var table = div.table({ cellpadding: 0 });
                                var previewCell = table.cell(1, 1).css({ width: '40px', 'text-align': 'center' });

                                var iconopts = {
                                    name: 'View',
                                    hovertext: 'View',
                                    iconType: Csw.enums.iconType.magglass,
                                    state: Csw.enums.iconState.normal,
                                    isButton: true,
                                    size: 18,
                                    onClick: function (event) {
                                        Csw.tryExec(onView, record.data);
                                    }
                                };
                                previewCell.icon(iconopts);

                            }), 100);
                            return '<div id="' + divId + '" style="height:18px;"></div>';
                        } // renderer()
                    },
                    { header: 'Language', dataIndex: 'language' },
                    { header: 'Country', dataIndex: 'country' },
                    { header: 'Select', dataIndex: 'select', xtype: 'checkcolumn' },
                    { header: 'FileName', dataIndex: 'filename', hidden: true },
                    { header: 'ExternalUrl', dataIndex: 'externalurl', hidden: true }
                ],
                data: gridData || {
                    'items': [] //ExtGrids won't show without data
                },
                viewConfig: {
                    markDirty: false
                },
                height: 200,
                width: 400,
                showActionColumn: false
            });
        }
        //#endregion Helper Functions


        //#region State Functions

        cswPrivate.setState = function () {
            var state;
            if (Csw.isNullOrEmpty(cswPrivate.state.materialId)) {
                state = cswPrivate.getState();
                Csw.extend(cswPrivate.state, state);
            }
            Csw.clientDb.setItem(cswPrivate.name + '_' + cswChemWatchActionStateName, cswPrivate.state);
        };
        
        cswPrivate.getState = function () {
            var ret = Csw.clientDb.getItem(cswPrivate.name + '_' + cswChemWatchActionStateName);
            return ret;
        };

        cswPrivate.clearState = function () {
            Csw.clientDb.removeItem(cswPrivate.name + '_' + cswChemWatchActionStateName);
        };

        //#endregion State Functions

        //#region Initialize
        (function () {
            // Extend cswPrivate
            Csw.extend(cswPrivate, options);
            
            // We need to preserve the nbtmaterialid
            cswPrivate.setState();
            cswPrivate.OperationData.NbtMaterialId = cswPrivate.state.materialId;
            
            // Set up action layout
            var layout = Csw.layouts.action(cswParent, {
                title: 'ChemWatch',
                finishText: 'Create SDS Links',
                onFinish: function () {
                    cswPrivate.clearState();
                    cswPrivate.createSDSLinks();
                },
                onCancel: function () {
                    cswPrivate.clearState();
                    Csw.tryExec(cswPrivate.onCancel);
                },
            });

            cswPublic.table = layout.actionDiv.table({
                width: '90%'
            }).css({ padding: '20px' });

            // call the Initialize web service met
            Csw.ajaxWcf.post({
                urlMethod: 'ChemWatch/Initialize',
                data: cswPrivate.OperationData,
                success: function (data) {
                    cswPrivate.OperationData = data;

                    LanguageOptions = cswPrivate.OperationData.Languages;
                    CountryOptions = cswPrivate.OperationData.Countries;

                    Csw.iterate(LanguageOptions, function (lang) {
                        LanguageLookup[lang.value] = lang;
                    });
                    Csw.iterate(CountryOptions, function (country) {
                        CountryLookup[country.value] = country;
                    });

                    // Make step 1
                    cswPrivate.makeStepOne();
                },
                error: function (data) {
                    console.log(data);
                }
            });

        })();// init
        //#endregion Initialize

    });

})();