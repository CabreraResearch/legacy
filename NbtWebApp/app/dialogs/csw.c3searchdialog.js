/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('c3SearchDialog', function (options) {
        'use strict';

        var cswPrivate = {};

        (function _preCtor() {
            cswPrivate.title = 'ChemCatCentral Search';
            cswPrivate.height = options.height || 300;
            cswPrivate.width = options.width || 750;
            cswPrivate.c3searchterm = options.c3searchterm || '';
            cswPrivate.c3handleresults = options.c3handleresults; //is this a function?
            cswPrivate.c3dataservice = '';
            cswPrivate.clearview = options.clearview; //is this a function?
            cswPrivate.loadView = null; // function() { }
            cswPrivate.preferredSuppliers = '';
        }());
        
        function setVendorOpts()
        {
            Csw.ajaxWcf.post({
                urlMethod: 'ChemCatCentral/GetVendorOptions',
                success: function (data) {
                    cswPrivate.vendorOptions.empty();
                    cswPrivate.vendorOptions.setOptions(cswPrivate.vendorOptions.makeOptions(data.VendorListOptions));
                }
            });
        }
        
        function createSearchParamsObj() {
            var CswC3SearchParams = {
                Field: cswPrivate.searchTypeSelect.selectedVal(),
                Query: $.trim(cswPrivate.searchTermField.val()),
                SearchOperator: cswPrivate.searchOperatorSelect.selectedVal()
            };
            
            if (cswPrivate.searchTypeSelect.selectedText() == "Structure") {
                CswC3SearchParams.Query = $.trim(cswPrivate.molSearchField.val());
            }

            if (cswPrivate.c3dataservice === 'ACD') {
                CswC3SearchParams.ACDSearchParams = {};
                CswC3SearchParams["ACDSearchParams"]["CompanyIds"] = cswPrivate.vendorOptions.selectedVal();
            }
            if (cswPrivate.c3dataservice === 'C3') {
                CswC3SearchParams.C3SearchParams = {};
                CswC3SearchParams["C3SearchParams"]["DataSources"] = cswPrivate.vendorOptions.selectedVal();
            }

            return CswC3SearchParams;
        }

        return (function () {
            'use strict';

            // We need to FIRST determine what type of dataservice (C3 or ACD)
            Csw.ajaxWcf.post({
                urlMethod: 'ChemCatCentral/GetC3DataService',
                success: function (data) {
                    cswPrivate.c3dataservice = data.DataService;
                    
                    function onOpen() {

                        // Vendor Options Picklist
                        cswPrivate.vendorOptions = tableInner.cell(1, 1).select({
                            name: 'C3Search_vendorOptionSelect',
                        });

                        //SearchTypes Picklist
                        cswPrivate.searchTypeSelect = tableInner.cell(1, 2).select({
                            name: 'C3Search_searchTypeSelect',
                            selected: 'Name',
                            onChange: function (event) {
                                if (cswPrivate.searchTypeSelect.selectedText() == "Structure") {
                                    cswPrivate.searchOperatorSelect.removeOption('begins');
                                    cswPrivate.searchTermField.hide();
                                    molSearchText.show();
                                    cswPrivate.molSearchField.show();
                                    molImageCell.show();
                                } else if (cswPrivate.searchOperatorSelect[0].length == 2) {
                                    cswPrivate.searchOperatorSelect.addOption({ display: 'Begins', value: 'begins' });
                                    cswPrivate.searchTermField.show();
                                    molSearchText.hide();
                                    cswPrivate.molSearchField.hide();
                                    molImageCell.hide();
                                }
                            }// onChange
                        }); //searchTypeSelect

                        Csw.ajaxWcf.post({
                            urlMethod: 'ChemCatCentral/GetSearchProperties',
                            success: function (data) {
                                cswPrivate.searchTypeSelect.setOptions(cswPrivate.searchTypeSelect.makeOptions(data.SearchProperties));
                            }
                        });

                        setVendorOpts();

                    };//function onOpen() 
                    

                    
                    // Create the actual dialog
                    cswPrivate.c3SearchDialog = Csw.layouts.dialog({
                        title: cswPrivate.title,
                        width: cswPrivate.width,
                        height: cswPrivate.height,
                        onOpen: function () {
                            onOpen();
                        },
                        //onClose: cswPrivate.onClose
                    });

                    var div = cswPrivate.c3SearchDialog.div;

                    // Outer table
                    var tableOuter = div.table({ cellpadding: '2px', align: 'left', width: '700px' });
                    tableOuter.cell(1, 1).p({ text: '' });

                    // Inner table
                    var tableInner = tableOuter.cell(2,1).table({ cellpadding: '2px' });

                    // Pick-lists
                    cswPrivate.vendorOptions = null;
                    cswPrivate.searchTypeSelect = null;

                    cswPrivate.searchOperatorSelect = tableInner.cell(1, 3).select({
                        name: 'C3Search_searchOperatorSelect'
                    });
                    cswPrivate.searchOperatorSelect.option({ display: 'Begins', value: 'begins' });
                    cswPrivate.searchOperatorSelect.option({ display: 'Contains', value: 'contains' });
                    cswPrivate.searchOperatorSelect.option({ display: 'Exact', value: 'exact' });

                    cswPrivate.searchTermField = tableInner.cell(1, 4).input({
                        value: cswPrivate.c3searchterm,
                        onKeyUp: function (keyCode) {
                            // If the key pressed is NOT the 'Enter' key
                            if (keyCode != 13) {
                                if (Csw.isNullOrEmpty(cswPrivate.searchTermField.val())) {
                                    searchButton.disable();
                                } else {
                                    searchButton.enable();
                                }
                            }
                        }
                    });

                    //#region MOL
                    var molSearchText = tableInner.cell(2, 1).div({
                        text: "<b>Paste MOL data from clipboard:</b>"
                    });
                    cswPrivate.molSearchField = tableInner.cell(2, 1).textArea({
                        rows: 8,
                        cols: 35
                    });

                    var molImageCell = tableInner.cell(2, 2);

                    var displayMolThumbnail = function (data) {
                        molImageCell.empty();
                        if (data.molImgAsBase64String) {
                            molImageCell.img({
                                src: "data:image/jpeg;base64," + data.molImgAsBase64String
                            });
                        }
                    };
                    
                    cswPrivate.molSearchField.bind('keyup', function () {
                        if (Csw.isNullOrEmpty(cswPrivate.molSearchField.val())) {
                            searchButton.disable();
                        } else {
                            searchButton.enable();
                            Csw.getMolImgFromText('', cswPrivate.molSearchField.val(), displayMolThumbnail);
                        }
                    });
                    
                    tableInner.cell(2, 1).propDom('colspan', 3);
                    molImageCell.propDom('colspan', 2);
                    molSearchText.hide();
                    cswPrivate.molSearchField.hide();
                    molImageCell.hide();

                    //#endregion MOL

                    var enableSearchButton = !(Csw.isNullOrEmpty(cswPrivate.searchTermField.val()));

                    var searchButton = tableInner.cell(1, 5).button({
                        name: 'c3SearchBtn',
                        enabledText: 'Search',
                        bindOnEnter: div,
                        isEnabled: enableSearchButton,
                        onClick: function () {

                            //var CswC3SearchParams = {
                            //    Field: cswPrivate.searchTypeSelect.selectedVal(),
                            //    Query: $.trim(cswPrivate.searchTermField.val()),
                            //    SearchOperator: cswPrivate.searchOperatorSelect.selectedVal(),
                            //    SourceName: cswPrivate.vendorOptions.selectedVal(),
                            //    ACDCompanyIds: cswPrivate.vendorOptions.selectedVal()
                            //};

                            //if (cswPrivate.searchTypeSelect.selectedText() == "Structure") {
                            //    CswC3SearchParams.Query = $.trim(cswPrivate.molSearchField.val());
                            //}

                            var CswC3SearchParams = createSearchParamsObj();

                            Csw.ajaxWcf.post({
                                urlMethod: 'ChemCatCentral/Search',
                                data: CswC3SearchParams,
                                success: function (data) {
                                    //Convert to object from string
                                    var obj = JSON.parse(data.SearchResults);
                                    Csw.tryExec(cswPrivate.clearview);
                                    Csw.tryExec(cswPrivate.c3handleresults(obj));
                                    div.$.dialog('close');
                                }
                            });
                        }//onClick
                    }); //var searchButton = tableInner.cell(1, 5).button

                    //tableOuter.cell(2, 1).div(tableInner);

                    if (cswPrivate.c3dataservice === "ACD") {
                        cswPrivate.setPreferredSuppliersLink = tableOuter.cell(3, 1).a({
                            text: 'Set Preferred Suppliers',
                            onClick: function() {
                                //open the 'set preferred suppliers' dialog
                                Csw.dialogs.c3PrefSuppliersDialog({
                                    onSave: function(selected) {
                                        //reload the source select
                                        setVendorOpts();
                                    }
                                });
                            }
                        });
                    }

                    // We need to open the dialog at the end
                    cswPrivate.c3SearchDialog.open();
                },
                error: function(error) {
                    //Display error in dialog?
                }
            });

            return cswPrivate.c3SearchDialog;
        }());
    });
}());