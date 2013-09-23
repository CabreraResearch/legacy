
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {


    Csw.wizard.register('sizesGrid', function (cswParent, options) {
        'use strict';
        ///<summary>Creates an amounts thin grid with an Add form.</summary>
        var cswPublic = {
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
            deletedRows: [],
            sizes: function () {
                return cswPublic.rows;
            },
            deletedSizes: function () {
                return cswPublic.deletedRows;
            },
            thinGrid: null,
        };

        Csw.tryExec(function () {

            var cswPrivate = {
                name: 'wizardSizesThinGrid',
                sizeNodeTypeId: null,
                sizeRowsToAdd: [],
                physicalState: null,
                unitsOfMeasure: [],
                showQuantityEditable: false,
                showDispensable: false,
                showOriginalUoM: true,
                containerlimit: 25,
                config: {
                    quantityName: 'Initial Quantity',
                    numberName: 'Catalog No.',
                    dispensibleName: 'Dispensible',
                    quantityEditableName: 'Quantity Editable',
                    unitCountName: 'Unit Count',
                    newUomName: 'New Unit',
                    origUomName: 'Original Unit'
                }
            };
            Csw.extend(cswPrivate, options);

            //#region: Grid header (MUST BE IN ORDER)
            cswPrivate.header = [
                { "value": cswPrivate.config.unitCountName, "isRequired": false },
                { "value": cswPrivate.config.quantityName, "isRequired": false },
                { "value": cswPrivate.config.newUomName, "isRequired": false }
            ];
            if (cswPrivate.showOriginalUoM) {
                cswPrivate.header = cswPrivate.header.concat([
                    { "value": cswPrivate.config.origUomName, "isRequired": false }
                ]);
            }
            cswPrivate.header = cswPrivate.header.concat([
                { "value": cswPrivate.config.numberName, "isRequired": false }
            ]);
            if (cswPrivate.showQuantityEditable) {
                cswPrivate.header = cswPrivate.header.concat([
                    { "value": cswPrivate.config.quantityEditableName, "isRequired": false }
                ]);
            }
            if (cswPrivate.showDispensable) {
                cswPrivate.header = cswPrivate.header.concat([
                    { "value": cswPrivate.config.dispensibleName, "isRequired": false }
                ]);
            }
            //#region: Grid header

            cswPrivate.addPreExistingRows = function () {
                var currentRowCount = cswPublic.thinGrid.getRowCount();

                var extractNewAmount = function (object) {
                    var ret = Csw.extend({}, object, true);
                    return ret;
                };

                cswPrivate.sizeRowsToAdd.forEach(function (element, index, array) {
                    cswPublic.rows[currentRowCount] = { sizeValues: extractNewAmount(element) };
                    Csw.tryExec(cswPublic.thinGrid.makeAddRow, function (cswCell, columnName, rowid) {
                        'use strict';

                        var sizeValues = cswPublic.rows[rowid]["sizeValues"];

                        switch (columnName) {
                            case cswPrivate.config.unitCountName:
                                if (sizeValues.unitCount.readOnly) {
                                    cswPublic.rows[rowid].unitCountCtrl = cswCell.label({
                                        name: 'sizeUnitCount',
                                        text: sizeValues.unitCount.value
                                    });
                                } else {
                                    cswPublic.rows[rowid].unitCountCtrl = cswCell.numberTextBox({
                                        name: 'sizeUnitCount',
                                        MinValue: 1,
                                        Precision: 0,
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.unitCount.value = cswPublic.rows[rowid].unitCountCtrl.val();
                                        }
                                    });
                                }
                                break;
                            case cswPrivate.config.quantityName:
                                if (sizeValues.quantity.readOnly) {
                                    cswPublic.rows[rowid].quantityCtrl = cswCell.label({
                                        name: 'quantityNumberBox',
                                        text: sizeValues.quantity.value
                                    });
                                } else {
                                    cswPublic.rows[rowid].quantityCtrl = cswCell.numberTextBox({
                                        name: 'quantityNumberBox',
                                        MinValue: 0,
                                        Precision: '',
                                        excludeRangeLimits: true,
                                        width: '60px',
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.quantity.value = cswPublic.rows[rowid].quantityCtrl.val();
                                        }
                                    });
                                }
                                break;
                            case cswPrivate.config.newUomName:
                                if (sizeValues.uom.readOnly) {
                                    cswPublic.rows[rowid].unitsCtrl = cswCell.label({
                                        name: 'unitsOfMeasureSelect',
                                        text: sizeValues.uom.value
                                    });
                                    cswPublic.rows[rowid].sizeValues.uom.id = cswPrivate.getID(cswPublic.rows[rowid].sizeValues.uom.value);
                                } else {
                                    cswPublic.rows[rowid].unitsCtrl = cswCell.select({
                                        name: 'unitsOfMeasureSelect',
                                        values: cswPrivate.unitsOfMeasure,
                                            selected: '',
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.uom.value = cswPublic.rows[rowid].unitsCtrl.val();
                                            cswPublic.rows[rowid].sizeValues.uom.id = cswPrivate.getID(cswPublic.rows[rowid].sizeValues.uom.value);
                                        }
                                    });
                                    cswPublic.rows[rowid].sizeValues.uom.value = cswPublic.rows[rowid].unitsCtrl.val();
                                    cswPublic.rows[rowid].sizeValues.uom.id = cswPrivate.getID(cswPublic.rows[rowid].sizeValues.uom.value);
                                }
                                break;
                            case cswPrivate.config.origUomName:
                                if (sizeValues.origUom.readOnly) {
                                    var text = sizeValues.origUom.value;
                                    if (Csw.isNullOrEmpty(text)) {
                                        text = 'None Provided';
                                    }
                                    cswPublic.rows[rowid].origUnitsCtrl = cswCell.label({
                                        name: 'originalUnitOfMeasure',
                                        text: text
                                    });
                                }
                                break;
                            case cswPrivate.config.numberName:
                                if (sizeValues.catalogNo.readOnly) {
                                    cswPublic.rows[rowid].catalogNoCtrl = cswCell.label({
                                        name: 'sizeCatalogNo',
                                        text: sizeValues.catalogNo.value
                                    });
                                } else {
                                    cswPublic.rows[rowid].catalogNoCtrl = cswCell.input({
                                        name: 'sizeCatalogNo',
                                        width: '80px',
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.catalogNo.value = value;
                                        }
                                    });
                                }
                                break;
                            case cswPrivate.config.quantityEditableName:
                                cswPublic.rows[rowid].quantEditableCtrl = cswCell.checkBox({
                                    name: 'sizeQuantEditable',
                                    checked: true,
                                    onChange: function (value) {
                                        cswPublic.rows[rowid].sizeValues.quantityEditable.value = cswPublic.rows[rowid].quantEditableCtrl.val();
                                    }
                                });
                                break;
                            case cswPrivate.config.dispensibleName:
                                cswPublic.rows[rowid].dispensibleCtrl = cswCell.checkBox({
                                    name: 'sizeDispensible',
                                    checked: true,
                                    onChange: function (value) {
                                        cswPublic.rows[rowid].sizeValues.dispensible.value = cswPublic.rows[rowid].dispensibleCtrl.val();
                                    }
                                });
                                break;
                        } //switch()
                    });

                    currentRowCount = cswPublic.thinGrid.getRowCount();
                });

            }; //cswPrivate.addPreExistingRows

            cswPrivate.getID = function (unitType) {
                var ret = '';
                Csw.each(cswPrivate.unitsOfMeasure, function (obj, key) {
                    if (cswPrivate.unitsOfMeasure[key] === unitType) {
                        ret = key;
                    }
                });
                return ret;
            };

            (function _pre() {

                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException(Csw.error.exception(
                        'Cannot create a Wizard sizes grid without a parent.',
                        '',
                        'csw.wizard.sizesgrid.js', 22));
                }

                var ajax = Csw.ajax.deprecatedWsNbt({
                    urlMethod: 'getMaterialUnitsOfMeasure',
                    data: {
                        PhysicalStateValue: cswPrivate.physicalState
                    },
                    success: function (data) {
                        cswPrivate.unitsOfMeasure = data;
                    }
                });

                ajax.then(function () {

                    var showEmptyRow = true;
                    if (cswPrivate.sizeRowsToAdd.length > 0) {
                        showEmptyRow = false;
                    }

                    cswPublic.thinGrid = cswParent.thinGrid({
                        linkText: '',
                        hasHeader: true,
                        rows: [cswPrivate.header],
                        showEmptyRow: showEmptyRow,
                        allowDelete: true,
                        allowAdd: true,
                        makeAddRow: function (cswCell, columnName, rowid) {
                            'use strict';
                            switch (columnName) {
                                case cswPrivate.config.unitCountName:
                                    cswPublic.rows[rowid].unitCountCtrl = cswCell.numberTextBox({
                                        name: 'sizeUnitCount',
                                        MinValue: 1,
                                        MaxValue: cswPrivate.containerlimit,
                                        Precision: 0,
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.unitCount.value = cswPublic.rows[rowid].unitCountCtrl.val();
                                        }
                                    });
                                    cswCell.span({ text: ' x' });
                                    break;
                                case cswPrivate.config.quantityName:
                                    cswPublic.rows[rowid].quantityCtrl = cswCell.numberTextBox({
                                        name: 'quantityNumberBox',
                                        MinValue: 0,
                                        Precision: 6,
                                        excludeRangeLimits: true,
                                        width: '60px',
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.quantity.value = cswPublic.rows[rowid].quantityCtrl.val();
                                        }
                                    });
                                    break;
                                case cswPrivate.config.newUomName:
                                    cswPublic.rows[rowid].unitsCtrl = cswCell.select({
                                        name: 'unitsOfMeasureSelect',
                                        values: cswPrivate.unitsOfMeasure,
                                        selected: '',
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.uom.value = cswPublic.rows[rowid].unitsCtrl.val();
                                            cswPublic.rows[rowid].sizeValues.uom.id = cswPrivate.getID(cswPublic.rows[rowid].sizeValues.uom.value);
                                        }
                                    });
                                    cswPublic.rows[rowid].sizeValues.uom.value = cswPublic.rows[rowid].unitsCtrl.val();
                                    cswPublic.rows[rowid].sizeValues.uom.id = cswPrivate.getID(cswPublic.rows[rowid].sizeValues.uom.value);
                                    break;
                                case cswPrivate.config.origUomName:
                                    cswPublic.rows[rowid].origUnitsCtrl = cswCell.label({
                                        name: 'originalUnitOfMeasure',
                                        text: ''
                                    });
                                    break;
                                case cswPrivate.config.numberName:
                                    cswPublic.rows[rowid].catalogNoCtrl = cswCell.input({
                                        name: 'sizeCatalogNo',
                                        width: '80px',
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.catalogNo.value = value;
                                        }
                                    });
                                    break;
                                case cswPrivate.config.quantityEditableName:
                                    cswPublic.rows[rowid].quantEditableCtrl = cswCell.checkBox({
                                        name: 'sizeQuantEditable',
                                        checked: true,
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.quantityEditable.value = cswPublic.rows[rowid].quantEditableCtrl.val();
                                        }
                                    });
                                    break;
                                case cswPrivate.configc.dispensibleName:
                                    cswPublic.rows[rowid].dispensibleCtrl = cswCell.checkBox({
                                        name: 'sizeDispensible',
                                        checked: true,
                                        onChange: function (value) {
                                            cswPublic.rows[rowid].sizeValues.dispensible.value = cswPublic.rows[rowid].dispensibleCtrl.val();
                                        }
                                    });
                                    break;
                            }
                        },
                        onAdd: function (newRowid) {
                            var newSize = {};
                            //This while loop serves as a buffer to remove the +1/-1 issues when comparing 
                            //the data with the table cell rows in the thingrid.
                            //This puts the burden on the user of thingrid to ensure their data lines up 
                            //with the table cells. Also, undefined size values break the serverside foreach
                            // loop, so an empty one is inserted in each element (including deleted elements).
                            while (cswPublic.rows.length < newRowid) {
                                cswPublic.rows[newRowid] = { sizeValues: {} };
                            }

                            newSize = {
                                nodeId: {
                                    value: ''
                                },
                                nodeTypeId: {
                                    value: cswPrivate.sizeNodeTypeId
                                },
                                unitCount: {
                                    value: ''
                                },
                                quantity: {
                                    value: ''
                                },
                                uom: {
                                    value: '',
                                    id: ''
                                },
                                origUom: {
                                    value: ''
                                },
                                catalogNo: {
                                    value: ''
                                },
                                quantityEditable: {
                                    value: 'true'
                                },
                                dispensible: {
                                    value: 'true'
                                }
                            };

                            var extractNewAmount = function (object) {
                                var ret = Csw.extend({}, object, true);
                                return ret;
                            };
                            cswPublic.rows[newRowid] = { sizeValues: extractNewAmount(newSize) };
                        },
                        onDelete: function (rowid) {
                            cswPublic.deletedRows.push(cswPublic.rows[rowid].sizeValues.nodeId.value);
                            delete cswPublic.rows[rowid];
                            cswPublic.rows[rowid] = { sizeValues: {} };
                        }
                    });

                    cswParent.br();

                    if (cswPrivate.sizeRowsToAdd.length > 0) {
                        cswPrivate.addPreExistingRows();
                    }

                });

            }()); //_pre()

        });

        return cswPublic;

    });
}());

