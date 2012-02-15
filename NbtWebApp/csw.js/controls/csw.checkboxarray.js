/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswCheckBoxArray() {
    "use strict";

    function checkBoxArray(options) {

        var internal = {
            storedDataSuffix: 'cswCbaArrayDataStore',
            cbaPrevSelectedSuffix: 'cswCba_prevSelected',
            ID: '',
            cols: [],
            data: [],
            HeightInRows: 4,
            UseRadios: false,
            Required: false,
            ReadOnly: false,
            Multi: false,
            MultiIsUnchanged: true,
            onChange: null,
            dataAry: [],
            nameCol: '',
            keyCol: '',
            valCol: '',
            valColName: ''
        };
        var external = {

        };


        (function () {
            if (options) {
                $.extend(internal, options);
            }

            var storeDataId = Csw.controls.dom.makeId(internal.ID, internal.storedDataSuffix);
            internal.cbaPrevSelected = Csw.controls.dom.makeId(storeDataId, internal.cbaPrevSelectedSuffix);

            Csw.clientDb.removeItem(storeDataId);
            Csw.clientDb.removeItem(internal.cbaPrevSelected);

            Csw.controls.factory(internal.$parent, external);
            var cbaData = transmogrify({
                storeDataId: storeDataId,
                dataAry: internal.dataAry,
                nameCol: internal.nameCol,
                keyCol: internal.keyCol,
                valCol: internal.valCol,
                cols: internal.cols
            });
            if (false === Csw.isNullOrEmpty(cbaData)) {
                $.extend(internal, cbaData);
            }
            internal.MultiIsUnchanged = internal.Multi;

            var checkType = Csw.enums.inputTypes.checkbox.name;
            if (internal.UseRadios) {
                checkType = Csw.enums.inputTypes.radio.name;
            }

            var outerDiv = external.div({ ID: storeDataId });

            Csw.clientDb.setItem(storeDataId, { columns: internal.cols, data: internal.data });

            if (internal.ReadOnly) {
                for (var r = 0; r < internal.data.length; r += 1) {
                    var rRow = internal.data[r];
                    var rowlabeled = false;
                    var first = true;
                    for (var c = 0; c < internal.cols.length; c += 1) {
                        if (Csw.bool(rRow.values[c])) {
                            if (false === internal.Multi) {
                                if (false === rowlabeled) {
                                    outerDiv.append(rRow.label + ": ");
                                    rowlabeled = true;
                                }
                                if (false === first) {
                                    outerDiv.append(", ");
                                }
                                if (false === internal.UseRadios) {
                                    outerDiv.append(internal.cols[c]);
                                }
                                first = false;
                            }
                        }
                    }
                    if (rowlabeled) {
                        outerDiv.br();
                    }
                }
            } else {
                var table = outerDiv.table({
                    ID: Csw.controls.dom.makeId(internal.ID, 'tbl')
                });

                outerDiv.css('height', (25 * internal.HeightInRows) + 'px');
                outerDiv.addClass('cbarraydiv');
                table.addClass(table.$, 'cbarraytable');

                // Header
                var tablerow = 1;
                for (var d = 0; d < internal.cols.length; d++) {
                    var dCell = table.cell(tablerow, d + 2);
                    dCell.addClass('cbarraycell');
                    var colName = internal.cols[d];
                    if (colName === internal.valCol && false === Csw.isNullOrEmpty(internal.valColName)) {
                        colName = internal.valColName;
                    }
                    if ((colName !== internal.keyCol && colName !== internal.nameCol)) {
                        dCell.append(colName);
                    }
                }
                tablerow += 1;

                //[none] row
                if (internal.UseRadios && false === internal.Required) {
                    // Row label
                    var labelCell = table.add(tablerow, 1, '[none]');
                    labelCell.addClass('cbarraycell');

                    for (var e = 0; e < internal.cols.length; e++) {
                        var eCell = table.cell(tablerow, e + 2);
                        eCell.addClass('cbarraycell');
                        var eCheckid = internal.ID + '_none';
                        var eCheck = eCell.input({
                            type: checkType,
                            cssclass: 'CBACheckBox_' + internal.ID,
                            id: eCheckid,
                            name: internal.ID
                        }).propNonDom({ 'key': '', rowlabel: '[none]', collabel: internal.cols[e], row: -1, col: e });
                        var delClick = Csw.makeDelegate(internal.onChange, eCheck);
                        eCheck.click(function () {
                            internal.MultiIsUnchanged = false;
                            delClick();
                        });
                        eCheck.change(delClick);

                        if (false === internal.Multi) {
                            eCheck.propDom('checked', true); // the browser will override this if another one is checked
                        }
                    } // for(var c = 0; c < internal.cols.length; c++)
                } // if(internal.UseRadios && ! internal.Required)
                tablerow += 1;

                var onChange = function (cB) {
                    //var cB = this;
                    var col = cB.propNonDom['col'];
                    var row = cB.propNonDom['row'];
                    var cache = Csw.clientDb.getItem(storeDataId);
                    cache.MultiIsUnchanged = false;
                    if (Csw.contains(cache.data, row) && Csw.contains(cache.data[row], 'values')) {
                        cache.data[row].values[col] = cB.checked;
                    }
                    if (internal.UseRadios) { //we're toggling--cache the prev selected row/col to deselect on later change
                        var data = Csw.clientDb.getItem(internal.cbaPrevSelected);
                        if (Csw.contains(data, 'row') && Csw.contains(data, 'col')) {
                            if (Csw.contains(cache.data, data.row) && Csw.contains(cache.data[data.row], 'values')) {
                                cache.data[data.row].values[data.col] = false;
                            }
                        }
                        Csw.clientDb.setItem(internal.cbaPrevSelected, { row: row, col: col });
                    }
                    Csw.clientDb.setItem(storeDataId, cache);
                };

                // Data
                for (var s = 0; s < internal.data.length; s++) {
                    var sRow = internal.data[s];
                    // Row label
                    var sLabelcell = table.add(tablerow + s, 1, sRow.label);
                    sLabelcell.addClass('cbarraycell');

                    for (var f = 0; f < internal.cols.length; f++) {
                        var fCell = table.cell(tablerow + s, f + 2);
                        fCell.addClass('cbarraycell');
                        var fCheckid = internal.ID + '_' + s + '_' + f;
                        var fCheck = fCell.input({
                            type: checkType,
                            cssclass: 'CBACheckBox_' + internal.ID,
                            ID: fCheckid,
                            name: internal.ID,
                            onClick: internal.onChange
                        }).propNonDom({ key: sRow.key, rowlabel: sRow.label, collabel: internal.cols[f], row: s, col: f });
                        var delChange = Csw.makeDelegate(onChange, fCheck);
                        fCheck.change(delChange);
                        fCheck.data('thisRow', sRow);

                        if (sRow.values[f]) {
                            if (internal.UseRadios) {
                                Csw.clientDb.setItem(internal.cbaPrevSelected, { col: f, row: s });
                            }
                            fCheck.propDom('checked', 'true');
                        }
                    } // for(var c = 0; c < internal.cols.length; c++)
                } // for(var r = 0; r < internal.data.length; r++)

                if (false === internal.UseRadios) {
                    var checkAllLinkText = "Check All";
                    if ($('.CBACheckBox_' + internal.ID).not(':checked').length === 0) {
                        checkAllLinkText = "Uncheck All";
                    }

                    internal.checkAllLink = external.div({
                        align: 'right'
                    })
                        .link({
                            href: '#',
                            text: checkAllLinkText,
                            onClick: function () {
                                external.toggleCheckAll(checkAllLink, internal.ID);
                                return false;
                            }
                        });
                }

            } // if-else(internal.ReadOnly)

        } ());
        
        external.getdata = function (opts) {
            var _internal = {
                ID: ''
            };

            if (opts) {
                $.extend(_internal, opts);
            }
            var storeDataId = Csw.controls.dom.makeId(internal.ID, internal.storedDataSuffix);
            var data = Csw.clientDb.getItem(storeDataId);
            return data;
        };
        
        internal.transmogrify = function (opts) {
            var dataStore = {
                cols: [],
                data: []
            };
            var o = {
                storeDataId: '',
                dataAry: [],
                nameCol: '',
                keyCol: '',
                valCol: '',
                cols: []
            };
            if (opts) {
                $.extend(o, opts);
            }

            if (false === Csw.isNullOrEmpty(internal.dataAry) && internal.dataAry.length > 0) {
                // get columns
                var cols = internal.cols;
                if (Csw.hasLength(cols) && cols.length === 0) {
                    var firstProp = internal.dataAry[0];
                    for (var column in firstProp) {

                        if (Csw.contains(firstProp, column)) {
                            var fieldname = column;
                            if (fieldname !== internal.nameCol && fieldname !== internal.keyCol) {
                                cols.push(fieldname);
                            }
                        }
                    }
                }
                if (false === Csw.isNullOrEmpty(internal.valCol) && false === Csw.contains(cols, internal.valCol)) {
                    cols.push(internal.valCol);
                }

                // get data
                var data = [];

                for (var i = 0; i < internal.dataAry.length; i++) {
                    var thisSet = internal.dataAry[i];

                    if (Csw.contains(thisSet, internal.keyCol) && Csw.contains(thisSet, internal.nameCol)) {
                        var values = [];
                        for (var v = 0; v < cols.length; v++) {
                            if (Csw.contains(thisSet, cols[v])) {
                                values.push(Csw.bool(thisSet[cols[v]]));
                            }
                        }
                        var dataOpts = {
                            'label': thisSet[internal.nameCol],
                            'key': thisSet[internal.keyCol],
                            'values': values
                        };
                        data.push(dataOpts);
                    }
                }

                dataStore.cols = cols;
                dataStore.data = data;
                Csw.clientDb.setItem(internal.storeDataId, dataStore);
            }
            return dataStore;
        };

        external.toggleCheckAll = function () {
            // Are there any unchecked checkboxes?
            if ($('.CBACheckBox_' + id).not(':checked').length > 0) {
                checkAll();
            } else {
                uncheckAll();
            }
        }; // ToggleCheckAll()

        external.checkAll = function () {
            $('.CBACheckBox_' + id).prop('checked', 'checked').click();
            internal.checkAllLink.text('Uncheck all');
        };

        external.uncheckAll = function () {
            $('.CBACheckBox_' + id).removeAttr('checked').click();
            internal.checkAllLink.text('Check all');
        };
        return external;
    }

    Csw.controls.register('checkBoxArray', checkBoxArray);
    Csw.controls.checkBoxArray = Csw.controls.checkBoxArray || checkBoxArray;

} ());