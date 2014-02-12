/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('c3ProductDetails', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || "Product Details";
            cswPrivate.node = cswPrivate.nodeObj;
            cswPrivate.c3dataservice = cswPrivate.c3dataservice || 'C3';
        }());
        
        cswPrivate.c3DetailsDialog = (function () {
            'use strict';
            
            // Create the actual dialog
            var c3DetailsDialog =  Csw.layouts.dialog({
                title: cswPrivate.title,
                width: 500,
                height: 500,
                onOpen: function () {
                    var div = cswPrivate.c3DetailsDialog.div;

                    function createSearchParamsObj() {
                        var CswC3SearchParams = {};

                        if (cswPrivate.c3dataservice === 'ACD') {
                            CswC3SearchParams.ACDSearchParams = {};
                            CswC3SearchParams["ACDSearchParams"]["Cdbregno"] = cswPrivate.node.acdcdbregno;
                            CswC3SearchParams["ACDSearchParams"]["ProductId"] = cswPrivate.node.c3productid;

                        }
                        if (cswPrivate.c3dataservice === 'C3') {
                            CswC3SearchParams.C3SearchParams = {};
                            CswC3SearchParams["C3SearchParams"]["ProductId"] = cswPrivate.node.c3productid;
                        }

                        return CswC3SearchParams;
                    }


                    var getProductDetails = function () {

                        var CswC3SearchParams = createSearchParamsObj();

                        Csw.ajaxWcf.post({
                            urlMethod: 'ChemCatCentral/GetProductDetails',
                            data: CswC3SearchParams,
                            success: function (data) {

                                // Before rendering the size(s) grid, remove any sizes that vary only by unit count
                                // since they violate the uniqueness of sizes in NBT and wouldn't be imported anyways
                                var OriginalProductSizes = data.ProductDetails.ProductSize;

                                var UniqueProductSizes = [];
                                var unique = {};

                                for (var i = 0; i < OriginalProductSizes.length; i++) {
                                    if (!unique[(OriginalProductSizes[i].pkg_qty + OriginalProductSizes[i].pkg_qty_uom + OriginalProductSizes[i].catalog_no)]) {
                                        UniqueProductSizes.push(OriginalProductSizes[i]);
                                        unique[(OriginalProductSizes[i].pkg_qty + OriginalProductSizes[i].pkg_qty_uom + OriginalProductSizes[i].catalog_no)] = OriginalProductSizes[i];
                                    }
                                }

                                //Create the table
                                var table1 = div.table({ cellspacing: '5px', align: 'left', width: '100%' });

                                table1.cell(1, 1).div({
                                    text: cswPrivate.node.nodename,
                                }).css({ 'font-size': '18px', 'font-weight': 'bold' });
                                table1.cell(1, 1).propDom('colspan', 2);

                                table1.cell(2, 1).div({
                                    text: 'Supplier: ' + data.ProductDetails.SupplierName
                                });

                                if ('ACD' === cswPrivate.c3dataservice) {
                                    var catalogNumbers = '';
                                    Csw.iterate(cswPrivate.node.props, function(prop) {
                                        if (prop.propname === "CatalogNumbers") {
                                            catalogNumbers = prop.gestalt;
                                        }
                                    });
                                    table1.cell(3, 1).div({
                                        text: 'Catalog Numbers: ' + catalogNumbers
                                    });
                                } else {
                                    table1.cell(3, 1).div({
                                        text: 'Catalog No: ' + data.ProductDetails.CatalogNo
                                    });
                                }

                                // CAS Number
                                var casnodiv = table1.cell(4, 1).div({
                                    text: 'CAS No: ' + data.ProductDetails.CasNo
                                });
                                if (Csw.isNullOrEmpty(data.ProductDetails.CasNo)) {
                                    casnodiv.hide();
                                }

                                // Formula
                                var formuladiv = table1.cell(5, 1).div({
                                    text: 'Formula: ' + data.ProductDetails.Formula
                                });
                                if (Csw.isNullOrEmpty(data.ProductDetails.Formula)) {
                                    formuladiv.hide();
                                }

                                // Product Website
                                var producturldiv = table1.cell(6, 1).div({
                                    text: '<a href=' + data.ProductDetails.ProductUrl + ' target="_blank">Product Website</a>'
                                });
                                if (Csw.isNullOrEmpty(data.ProductDetails.ProductUrl)) {
                                    producturldiv.hide();
                                }

                                // SDS URL
                                var sdsurldiv = table1.cell(7, 1).div({
                                    text: '<a href=' + data.ProductDetails.MsdsUrl + ' target="_blank">SDS</a>'
                                });
                                if (Csw.isNullOrEmpty(data.ProductDetails.MsdsUrl)) {
                                    sdsurldiv.hide();
                                }

                                var molImageHeight = 0;
                                if ("" != data.ProductDetails.MolImage) {
                                    molImageHeight = 120;
                                }
                                table1.cell(2, 2).img({
                                    src: 'data:image/jpeg;base64,' + data.ProductDetails.MolImage,
                                    height: molImageHeight
                                });
                                table1.cell(2, 2).propDom('rowspan', 6);

                                var fields = [];
                                var columns = [];

                                fields = [
                                    { name: 'case_qty', type: 'string' },
                                    { name: 'pkg_qty', type: 'string' },
                                    { name: 'pkg_qty_uom', type: 'string' },
                                    { name: 'c3_uom', type: 'string' },
                                    { name: 'catalog_no', type: 'string' }
                                ];

                                columns = [
                                    { header: 'Unit Count', dataIndex: 'case_qty' },
                                    { header: 'Initial Quantity', dataIndex: 'pkg_qty' },
                                    { header: 'UOM', dataIndex: 'c3_uom' },
                                    { header: 'Catalog No', dataIndex: 'catalog_no' }
                                ];

                                var sizeGridId = 'c3detailsgrid_size';
                                table1.cell(8, 1).grid({
                                    name: sizeGridId,
                                    stateId: sizeGridId,
                                    title: 'Sizes',
                                    height: 100,
                                    width: 300,
                                    fields: fields,
                                    columns: columns,
                                    data: {
                                        items: UniqueProductSizes,
                                        buttons: []
                                    },
                                    usePaging: false,
                                    showActionColumn: false
                                });
                                table1.cell(8, 1).propDom('colspan', 2);

                                var extraDataGridId = 'c3detailsgrid_extradata';
                                table1.cell(9, 1).grid({
                                    name: extraDataGridId,
                                    stateId: extraDataGridId,
                                    title: 'Extra Attributes',
                                    height: 150,
                                    width: 300,
                                    fields: [{ name: 'attribute', type: 'string' }, { name: 'value', type: 'string' }],
                                    columns: [{ header: 'Attribute', dataIndex: 'attribute' }, { header: 'Value', dataIndex: 'value' }],
                                    data: {
                                        items: data.ProductDetails.TemplateSelectedExtensionData,
                                        buttons: []
                                    },
                                    usePaging: false,
                                    showActionColumn: false
                                });
                                table1.cell(9, 1).propDom('colspan', 2);


                            }
                        });
                    };

                    getProductDetails();
                }
            });

            return c3DetailsDialog;
        }());
        
        (function _postCtor() {
            cswPrivate.c3DetailsDialog.open();
        }());

        return cswPublic;
    });
}());