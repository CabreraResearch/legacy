
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {

    Csw.wizard.nodeThinGrid = Csw.wizard.nodeThinGrid ||
        Csw.wizard.register('nodeThinGrid', function (cswParent, options) {
            'use strict';

            ///<summary></summary>
            //Csw.error.throwException(Csw.error.exception('Csw.wizard.nodeThinGrid probably (possibly [maybe {dubiously} ] ) works, but it hasn\'t been tested. At all. Not even a little. You could start by uncommenting this line.', '', 'csw.wizard.nodethingrid.js', 22));

            var cswPrivate = {
                name: 'wizardNodeThinGrid',
                header: [],
                headerColumnNames: null,
                allowAdd: true,
                rowsToAdd: null,
                physicalState: null,
                sizeNodeTypeId: null,
                sizes: {
                    rows: {
                        rowid: {
                            catalogNoCtrl: {},
                            quantityCtrl: {},
                            unitsCtrl: {},
                            dispensibleCtrl: {},
                            quantEditableCtrl: {},
                            unitCountCtrl: {},
                            sizeValues: {}
                        }
                    },
                    sizesForm: null,
                    sizeGrid: null,
                }
            };
            var cswPublic = {};

            (function _pre() {
                Csw.extend(cswPrivate, options);

                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception('Cannot create a Wizard thin grid without a parent.', '', 'csw.wizard.nodethingrid.js', 22));
                }

            }());

            cswPrivate.makeGrid = function () {
                'use strict';
                cswPublic.rootDiv = cswPublic.rootDiv || cswParent.div();
                cswPublic.rootDiv.empty();

                cswPrivate.sizeGrid = cswPublic.rootDiv.thinGrid({
                    linkText: '',
                    hasHeader: true,
                    rows: [cswPrivate.header],
                    allowDelete: true,
                    allowAdd: true,
                    makeAddRow: function (cswCell, columnName, rowid) {
                        makeNewRow(cswCell, columnName, rowid);
                    },
                    onAdd: function (newRowid) {
                        onAddNewRow(newRowid);
                    },
                    onDelete: function (rowid) {
                        onDelete(rowid);
                    }
                });

                cswPublic.rootDiv.br();
            };

            //get Units of Measure for this Material
            var unitsOfMeasure = [];
            Csw.ajax.post({
                urlMethod: 'getMaterialUnitsOfMeasure',
                data: {
                    PhysicalStateValue: cswPrivate.physicalState
                },
                async: false, //wait for this request to finish
                success: function (data) {
                    unitsOfMeasure = data;
                }
            });

            var getID = function (unitType) {
                var ret = '';
                Csw.each(unitsOfMeasure, function (obj, key) {
                    if (unitsOfMeasure[key] === unitType) {
                        ret = key;
                    }
                });
                return ret;
            };

            var makeNewRow = function (cswCell, columnName, rowid) {
                'use strict';
                switch (columnName) {
                    case cswPrivate.headerColumnNames.unitCountName:
                        cswPrivate.sizes.rows[rowid].unitCountCtrl = cswCell.numberTextBox({
                            name: 'sizeUnitCount',
                            MinValue: 1,
                            Precision: 0,
                            onChange: function (value) {
                                cswPrivate.sizes.rows[rowid].sizeValues.unitCount = cswPrivate.sizes.rows[rowid].unitCountCtrl.val();
                            }
                        });
                        cswCell.span({ text: ' x' });
                        break;
                    case cswPrivate.headerColumnNames.quantityName:
                        cswPrivate.sizes.rows[rowid].quantityCtrl = cswCell.numberTextBox({
                            name: 'quantityNumberBox',
                            MinValue: 0,
                            Precision: '',
                            excludeRangeLimits: true,
                            width: '60px',
                            onChange: function (value) {
                                cswPrivate.sizes.rows[rowid].sizeValues.quantity = cswPrivate.sizes.rows[rowid].quantityCtrl.val();
                            }
                        });
                        break;
                    case cswPrivate.headerColumnNames.newUomName:
                        cswPrivate.sizes.rows[rowid].unitsCtrl = cswCell.select({
                            name: 'unitsOfMeasureSelect',
                            values: unitsOfMeasure,
                            selected: cswPrivate.sizes.rows[rowid].sizeValues.unit || '',
                            onChange: function (value) {
                                cswPrivate.sizes.rows[rowid].sizeValues.unit = cswPrivate.sizes.rows[rowid].unitsCtrl.val();
                                cswPrivate.sizes.rows[rowid].sizeValues.unitid = getID(cswPrivate.sizes.rows[rowid].sizeValues.unit);
                            }
                        });
                        break;
                    case cswPrivate.headerColumnNames.origUomName:
                        cswPrivate.sizes.rows[rowid].origUnitsCtrl = cswCell.label({
                            name: 'originalUnitOfMeasure',
                            text: cswPrivate.sizes.rows[rowid].sizeValues.origUnit || ''
                        });
                        break;
                    case cswPrivate.headerColumnNames.numberName:
                        cswPrivate.sizes.rows[rowid].catalogNoCtrl = cswCell.input({
                            name: 'sizeCatalogNo',
                            width: '80px',
                            onChange: function (value) {
                                cswPrivate.sizes.rows[rowid].sizeValues.catalogNo = value;
                            }
                        });
                        break;
                    case cswPrivate.headerColumnNames.quantityEditableName:
                        cswPrivate.sizes.rows[rowid].quantEditableCtrl = cswCell.checkBox({
                            name: 'sizeQuantEditable',
                            checked: true,
                            onChange: function (value) {
                                cswPrivate.sizes.rows[rowid].sizeValues.quantEditableChecked = cswPrivate.sizes.rows[rowid].quantEditableCtrl.val();
                            }
                        });
                        break;
                    case cswPrivate.headerColumnNamesc.dispensibleName:
                        cswPrivate.sizes.rows[rowid].dispensibleCtrl = cswCell.checkBox({
                            name: 'sizeDispensible',
                            checked: true,
                            onChange: function (value) {
                                cswPrivate.sizes.rows[rowid].sizeValues.dispensibleChecked = cswPrivate.sizes.rows[rowid].dispensibleCtrl.val();
                            }
                        });
                        break;
                }
            };

            var onAddNewRow = function (newRowid) {
                var newSize = {};
                //This while loop serves as a buffer to remove the +1/-1 issues when comparing the data with the table cell rows in the thingrid.
                //This puts the burden on the user of thingrid to ensure their data lines up with the table cells.
                //Also, undefined size values break the serverside foreach loop, so an empty one is inserted in each element (including deleted elements).
                while (cswPrivate.sizes.rows.length < newRowid) {
                    cswPrivate.sizes.rows[newRowid] = { sizeValues: {} };
                }
                
                newSize = {
                    catalogNo: '',
                    quantity: '',
                    unit: '',
                    unitid: '',
                    unitCount: '',
                    origUnit: '',
                    quantEditableChecked: 'true',
                    dispensibleChecked: 'true',
                    nodetypeid: cswPrivate.sizeNodeTypeId
                };

                var extractNewAmount = function (object) {
                    var ret = Csw.extend({}, object, true);
                    return ret;
                };
                cswPrivate.sizes.rows[newRowid] = { sizeValues: extractNewAmount(newSize) };
            };

            var onDelete = function (rowid) {
                delete cswPrivate.sizes.rows[rowid];
                cswPrivate.sizes.rows[rowid] = { sizeValues: {} };
            };

            var makeExistingSizeRow = function (cswCell, columnName, rowid) {
                'use strict';

                var sizeValues = cswPrivate.sizes.rows[rowid]["sizeValues"];

                switch (columnName) {
                    case cswPrivate.headerColumnNames.unitCountName:
                        if (sizeValues["unitCount"]["readOnly"]) {
                            cswPrivate.sizes.rows[rowid].unitCountCtrl = cswCell.label({
                                name: 'sizeUnitCount',
                                text: sizeValues["unitCount"]["value"]
                            });
                        } else {
                            cswPrivate.sizes.rows[rowid].unitCountCtrl = cswCell.numberTextBox({
                                name: 'sizeUnitCount',
                                MinValue: 1,
                                Precision: 0,
                                onChange: function (value) {
                                    cswPrivate.sizes.rows[rowid].sizeValues.unitCount = cswPrivate.sizes.rows[rowid].unitCountCtrl.val();
                                }
                            });
                        }
                        break;
                    case cswPrivate.headerColumnNames.quantityName:
                        if (sizeValues["initialQuantity"]["readOnly"]) {
                            cswPrivate.sizes.rows[rowid].quantityCtrl = cswCell.label({
                                name: 'quantityNumberBox',
                                text: sizeValues["initialQuantity"]["value"]
                            });
                        } else {
                            cswPrivate.sizes.rows[rowid].quantityCtrl = cswCell.numberTextBox({
                                name: 'quantityNumberBox',
                                MinValue: 0,
                                Precision: '',
                                excludeRangeLimits: true,
                                width: '60px',
                                onChange: function (value) {
                                    cswPrivate.sizes.rows[rowid].sizeValues.quantity = cswPrivate.sizes.rows[rowid].quantityCtrl.val();
                                }
                            });
                        }
                        break;
                    case cswPrivate.headerColumnNames.newUomName:
                        if (sizeValues["newUoM"]["readOnly"]) {
                            cswPrivate.sizes.rows[rowid].unitsCtrl = cswCell.label({
                                name: 'unitsOfMeasureSelect',
                                text: sizeValues["newUoM"]["value"]
                            });
                        } else {
                            cswPrivate.sizes.rows[rowid].unitsCtrl = cswCell.select({
                                name: 'unitsOfMeasureSelect',
                                values: unitsOfMeasure,
                                selected: '',
                                onChange: function (value) {
                                    cswPrivate.sizes.rows[rowid].sizeValues.unit = cswPrivate.sizes.rows[rowid].unitsCtrl.val();
                                    cswPrivate.sizes.rows[rowid].sizeValues.unitid = getID(cswPrivate.sizes.rows[rowid].sizeValues.unit);
                                }
                            });
                        }
                        break;
                    case cswPrivate.headerColumnNames.origUomName:
                        if (sizeValues["originalUoM"]["readOnly"]) {
                            cswPrivate.sizes.rows[rowid].origUnitsCtrl = cswCell.label({
                                name: 'originalUnitOfMeasure',
                                text: sizeValues["originalUoM"]["value"]
                            });
                        } else {
                            cswPrivate.sizes.rows[rowid].origUnitsCtrl = cswCell.label({
                                name: 'originalUnitOfMeasure',
                                text: cswPrivate.sizes.rows[rowid].sizeValues.origUnit || ''
                            });
                        }
                        break;
                    case cswPrivate.headerColumnNames.numberName:
                        if (sizeValues["catalogNo"]["readOnly"]) {
                            cswPrivate.sizes.rows[rowid].catalogNoCtrl = cswCell.label({
                                name: 'sizeCatalogNo',
                                text: sizeValues["catalogNo"]["value"]
                            });
                        } else {
                            cswPrivate.sizes.rows[rowid].catalogNoCtrl = cswCell.input({
                                name: 'sizeCatalogNo',
                                width: '80px',
                                onChange: function (value) {
                                    cswPrivate.sizes.rows[rowid].sizeValues.catalogNo = value;
                                }
                            });
                        }
                        break;
                    case cswPrivate.headerColumnNames.quantityEditableName:
                        cswPrivate.sizes.rows[rowid].quantEditableCtrl = cswCell.checkBox({
                            name: 'sizeQuantEditable',
                            checked: true,
                            onChange: function (value) {
                                cswPrivate.sizes.rows[rowid].sizeValues.quantEditableChecked = cswPrivate.sizes.rows[rowid].quantEditableCtrl.val();
                            }
                        });
                        break;
                    case cswPrivate.headerColumnNames.dispensibleName:
                        cswPrivate.sizes.rows[rowid].dispensibleCtrl = cswCell.checkBox({
                            name: 'sizeDispensible',
                            checked: true,
                            onChange: function (value) {
                                cswPrivate.sizes.rows[rowid].sizeValues.dispensibleChecked = cswPrivate.sizes.rows[rowid].dispensibleCtrl.val();
                            }
                        });
                        break;
                }
            };

            cswPrivate.addPreExistingRows = function () {

                if (false === Csw.isNullOrEmpty(cswPrivate.rowsToAdd)) {

                    var currentRowCount = cswPrivate.sizeGrid.getRowCount();

                    var extractNewAmount = function (object) {
                        var ret = Csw.extend({}, object, true);
                        return ret;
                    };

                    cswPrivate.rowsToAdd.forEach(function (element, index, array) {
                        cswPrivate.sizes.rows[currentRowCount] = { sizeValues: extractNewAmount(element) };
                        Csw.tryExec(cswPrivate.sizeGrid.makeAddRow, makeExistingSizeRow);
                        currentRowCount = cswPrivate.sizeGrid.getRowCount();
                    });
                }
            };//cswPrivate.addPreExistingRows

            cswPublic.getSizes = Csw.method(function () {
                /// <summary>
                /// Provides the caller with the cswPrivate.sizes object.
                /// </summary>
                var sizes = cswPrivate.sizes;
                return sizes;
            });

            (function _post() {
                cswPrivate.makeGrid();
                cswPrivate.addPreExistingRows();
            }());

            return cswPublic;

        });
}());

