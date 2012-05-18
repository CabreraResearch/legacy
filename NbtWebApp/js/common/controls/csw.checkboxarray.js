/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function _cswCheckBoxArray() {
    "use strict";

    Csw.controls.checkBoxArray = Csw.controls.checkBoxArray ||
        Csw.controls.register('checkBoxArray', function (cswParent, options) {

            if (Csw.isNullOrEmpty(cswParent)) {
                throw new Error('Cannot instance a Csw component without a Csw control');
            }

            var cswPrivateVar = {
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
                checked: 0,
                nameCol: '',
                keyCol: '',
                valCol: '',
                valColName: '',
                storeDataId: ''
            };
            var cswPublicRet = {};

            cswPrivateVar.transmogrify = function () {
                var dataStore = {
                    cols: [],
                    data: []
                };
                var data = [],
                    values;
                var cols, i, v, thisSet, firstProp, column, fieldname;


                if (false === Csw.isNullOrEmpty(cswPrivateVar.dataAry) && cswPrivateVar.dataAry.length > 0) {
                    // get columns
                    cols = cswPrivateVar.cols;
                    if (Csw.hasLength(cols) && cols.length === 0) {
                        firstProp = cswPrivateVar.dataAry[0];
                        for (column in firstProp) {
                            if (Csw.contains(firstProp, column)) {
                                fieldname = column;
                                if (fieldname !== cswPrivateVar.nameCol && fieldname !== cswPrivateVar.keyCol) {
                                    cols.push(fieldname);
                                }
                            }
                        }
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivateVar.valCol) && false === Csw.contains(cols, cswPrivateVar.valCol)) {
                        cols.push(cswPrivateVar.valCol);
                    }

                    // get data
                    for (i = 0; i < cswPrivateVar.dataAry.length; i += 1) {
                        thisSet = cswPrivateVar.dataAry[i];
                        values = [];
                        if (Csw.contains(thisSet, cswPrivateVar.keyCol) && Csw.contains(thisSet, cswPrivateVar.nameCol)) {
                            for (v = 0; v < cols.length; v += 1) {
                                if (Csw.contains(thisSet, cols[v])) {
                                    values.push(Csw.bool(thisSet[cols[v]]));
                                }
                            }
                            var dataOpts = {
                                'label': thisSet[cswPrivateVar.nameCol],
                                'key': thisSet[cswPrivateVar.keyCol],
                                'values': values
                            };
                            data.push(dataOpts);
                        }
                    }

                    dataStore.cols = cols;
                    dataStore.data = data;
                    Csw.clientDb.setItem(cswPrivateVar.storeDataId, dataStore);
                }
                return dataStore;
            };

            (function () {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }

                cswPrivateVar.storeDataId = Csw.makeId(cswPrivateVar.ID, cswPrivateVar.storedDataSuffix, '', '', false);
                cswPrivateVar.cbaPrevSelected = Csw.makeId(cswPrivateVar.storeDataId, cswPrivateVar.cbaPrevSelectedSuffix, '', '', false);

                Csw.clientDb.removeItem(cswPrivateVar.storeDataId);
                Csw.clientDb.removeItem(cswPrivateVar.cbaPrevSelected);

                cswPrivateVar.cbaDiv = cswParent.div({
                    ID: cswPrivateVar.storeDataId,
                    height: (25 * cswPrivateVar.HeightInRows) + 'px'
                });
                cswPublicRet = Csw.dom({ }, cswPrivateVar.cbaDiv);
                //Csw.controls.factory(cswPrivateVar.$parent, cswPublicRet);

                var cbaData = cswPrivateVar.transmogrify({
                    dataAry: cswPrivateVar.dataAry,
                    nameCol: cswPrivateVar.nameCol,
                    keyCol: cswPrivateVar.keyCol,
                    valCol: cswPrivateVar.valCol,
                    cols: cswPrivateVar.cols
                });
                if (false === Csw.isNullOrEmpty(cbaData)) {
                    $.extend(cswPrivateVar, cbaData);
                }
                cswPrivateVar.MultiIsUnchanged = cswPrivateVar.Multi;

                var checkType = Csw.enums.inputTypes.checkbox;
                if (cswPrivateVar.UseRadios) {
                    checkType = Csw.enums.inputTypes.radio;
                }

                Csw.clientDb.setItem(cswPrivateVar.storeDataId, { columns: cswPrivateVar.cols, data: cswPrivateVar.data });

                if (cswPrivateVar.ReadOnly) {
                    for (var r = 0; r < cswPrivateVar.data.length; r += 1) {
                        var rRow = cswPrivateVar.data[r];
                        var rowlabeled = false;
                        var first = true;
                        for (var c = 0; c < cswPrivateVar.cols.length; c += 1) {
                            if (Csw.bool(rRow.values[c])) {
                                if (false === cswPrivateVar.Multi) {
                                    if (false === rowlabeled) {
                                        cswPrivateVar.cbaDiv.append(rRow.label + ": ");
                                        rowlabeled = true;
                                    }
                                    if (false === first) {
                                        cswPrivateVar.cbaDiv.append(", ");
                                    }
                                    if (false === cswPrivateVar.UseRadios) {
                                        cswPrivateVar.cbaDiv.append(cswPrivateVar.cols[c]);
                                    }
                                    first = false;
                                }
                            }
                        }
                        if (rowlabeled) {
                            cswPrivateVar.cbaDiv.br();
                        }
                    }
                } else {
                    var table = cswPrivateVar.cbaDiv.table({
                        ID: Csw.makeId(cswPrivateVar.ID, 'tbl')
                    });

                    cswPrivateVar.cbaDiv.addClass('cbarraydiv');
                    table.addClass('cbarraytable');

                    // Header
                    var tablerow = 1;
                    for (var d = 0; d < cswPrivateVar.cols.length; d++) {
                        var dCell = table.cell(tablerow, d + 2);
                        dCell.addClass('cbarraycell');
                        var colName = cswPrivateVar.cols[d];
                        if (colName === cswPrivateVar.valCol && false === Csw.isNullOrEmpty(cswPrivateVar.valColName)) {
                            colName = cswPrivateVar.valColName;
                        }
                        if ((colName !== cswPrivateVar.keyCol && colName !== cswPrivateVar.nameCol)) {
                            dCell.append(colName);
                        }
                    }
                    tablerow += 1;

                    //[none] row
                    if (cswPrivateVar.UseRadios && false === cswPrivateVar.Required) {
                        // Row label
                        var labelCell = table.cell(tablerow, 1).text('[none]');
                        labelCell.addClass('cbarraycell');

                        for (var e = 0; e < cswPrivateVar.cols.length; e += 1) {
                            var eCell = table.cell(tablerow, e + 2);
                            eCell.addClass('cbarraycell');
                            var eCheckid = cswPrivateVar.ID + '_none';
                            var eCheck = eCell.input({
                                type: checkType,
                                cssclass: 'CBACheckBox_' + cswPrivateVar.ID,
                                id: eCheckid,
                                name: cswPrivateVar.ID,
                                checked: false === cswPrivateVar.Multi
                            });
                            eCheck.propNonDom({ 'key': '', rowlabel: '[none]', collabel: cswPrivateVar.cols[e], row: -1, col: e });
                            var delClick = Csw.makeDelegate(cswPrivateVar.onChange, eCheck);
                            eCheck.click(function () {
                                cswPrivateVar.MultiIsUnchanged = false;
                                delClick();
                            });
                            eCheck.change(delClick);
                        } // for(var c = 0; c < cswPrivateVar.cols.length; c++)
                    } // if(cswPrivateVar.UseRadios && ! cswPrivateVar.Required)
                    tablerow += 1;

                    var onChange = function (cB) {
                        //var cB = this;
                        var col = cB.propNonDom('col');
                        var row = cB.propNonDom('row');
                        var isChecked = Csw.bool(cB.propDom('checked'));
                        //                    if (false === isChecked) {
                        //                        if (cswPrivateVar.checked > 0) {
                        //                            cswPrivateVar.checked -= 1;
                        //                        }
                        //                    } else {
                        //                        cswPrivateVar.checked += 1;
                        //                    }
                        var cache = Csw.clientDb.getItem(cswPrivateVar.storeDataId);
                        cache.MultiIsUnchanged = false;
                        if (Csw.contains(cache.data, row) && Csw.contains(cache.data[row], 'values')) {
                            cache.data[row].values[col] = cB.$.is(':checked');
                        }
                        if (cswPrivateVar.UseRadios) { //we're toggling--cache the prev selected row/col to deselect on later change
                            var data = Csw.clientDb.getItem(cswPrivateVar.cbaPrevSelected);
                            if (Csw.contains(data, 'row') && Csw.contains(data, 'col')) {
                                if (Csw.contains(cache.data, data.row) && Csw.contains(cache.data[data.row], 'values')) {
                                    cache.data[data.row].values[data.col] = false;
                                }
                            }
                            Csw.clientDb.setItem(cswPrivateVar.cbaPrevSelected, { row: row, col: col });
                        }
                        Csw.clientDb.setItem(cswPrivateVar.storeDataId, cache);
                    };

                    // Data
                    for (var s = 0; s < cswPrivateVar.data.length; s += 1) {
                        var sRow = cswPrivateVar.data[s];
                        // Row label
                        var sLabelcell = table.cell(tablerow + s, 1).text(sRow.label);
                        sLabelcell.addClass('cbarraycell');

                        for (var f = 0; f < cswPrivateVar.cols.length; f += 1) {
                            var fCell = table.cell(tablerow + s, f + 2);
                            fCell.addClass('cbarraycell');
                            var fCheckid = cswPrivateVar.ID + '_' + s + '_' + f;
                            var fCheck = fCell.input({
                                type: checkType,
                                cssclass: 'CBACheckBox_' + cswPrivateVar.ID,
                                ID: fCheckid,
                                name: cswPrivateVar.ID,
                                onClick: cswPrivateVar.onChange,
                                checked: sRow.values[f]
                            });

                            fCheck.propNonDom({ key: sRow.key, rowlabel: sRow.label, collabel: cswPrivateVar.cols[f], row: s, col: f });
                            var delChange = Csw.makeDelegate(onChange, fCheck);
                            fCheck.change(delChange);
                            //fCheck.data('thisRow', sRow);

                            if (sRow.values[f]) {
                                if (cswPrivateVar.UseRadios) {
                                    Csw.clientDb.setItem(cswPrivateVar.cbaPrevSelected, { col: f, row: s });
                                }
                            }
                        } // for(var c = 0; c < cswPrivateVar.cols.length; c++)
                    } // for(var r = 0; r < cswPrivateVar.data.length; r++)

                    if (false === cswPrivateVar.UseRadios && cswPrivateVar.data.length > 0) {
                        var checkAllLinkText = 'Check All';
                        if ($('.CBACheckBox_' + cswPrivateVar.ID).not(':checked').length === 0) {
                            checkAllLinkText = 'Uncheck All';
                        }

                        cswPrivateVar.checkAllLink = cswParent.div({
                            isControl: cswPrivateVar.isControl,
                            align: 'right'
                        })
                            .a({
                                href: 'javascript:void(0)',
                                text: checkAllLinkText,
                                onClick: function () {
                                    cswPublicRet.toggleCheckAll();
                                    return false;
                                }
                            });
                    }

                } // if-else(cswPrivateVar.ReadOnly)

            } ());

            cswPublicRet.getdata = function (opts) {
                var _internal = {
                    ID: ''
                };

                if (opts) {
                    $.extend(_internal, opts);
                }

                var data = Csw.clientDb.getItem(cswPrivateVar.storeDataId);
                return data;
            };

            cswPublicRet.toggleCheckAll = function () {
                var checkBoxes = cswPublicRet.find('.CBACheckBox_' + cswPrivateVar.ID);
                if (checkBoxes.isValid) {
                    if (cswPrivateVar.checkAllLink.text() === 'Uncheck All') {
                        //if (cswPrivateVar.checked <= 0) {
                        //    cswPrivateVar.checked = cswPrivateVar.data.length;
                        checkBoxes.propDom('checked', 'checked'); // Yes, this checks.  But click below unchecks again.
                        cswPrivateVar.checkAllLink.text('Check All');
                    } else {
                        //    cswPrivateVar.checked = 0;
                        checkBoxes.$.removeAttr('checked'); // Yes, this unchecks.  But click below checks again.
                        cswPrivateVar.checkAllLink.text('Uncheck All');
                    }
                    checkBoxes.trigger('click'); // this toggles again
                }
            }; // ToggleCheckAll()

            return cswPublicRet;
        });

} ());