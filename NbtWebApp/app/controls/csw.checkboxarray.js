/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswCheckBoxArray() {
    "use strict";

    Csw.controls.checkBoxArray = Csw.controls.checkBoxArray ||
        Csw.controls.register('checkBoxArray', function (cswParent, options) {

            if (Csw.isNullOrEmpty(cswParent)) {
                throw new Error('Cannot instance a Csw component without a Csw control');
            }

            var cswPrivate = {
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
                storeDataId: '',
                dataStore: {
                    cols: [],
                    data: []
                },
                oldDataStore: {
                    col: '',
                    row: ''
                }
            };
            var cswPublic = {};

            cswPrivate.transmogrify = function () {
                var data = [],
                    values;
                var cols, i, v, thisSet, firstProp, column, fieldname;

                if (false === Csw.isNullOrEmpty(cswPrivate.dataAry) && cswPrivate.dataAry.length > 0) {
                    // get columns
                    cols = cswPrivate.cols;
                    if (Csw.hasLength(cols) && cols.length === 0) {
                        firstProp = cswPrivate.dataAry[0];
                        for (column in firstProp) {
                            if (Csw.contains(firstProp, column)) {
                                fieldname = column;
                                if (fieldname !== cswPrivate.nameCol && fieldname !== cswPrivate.keyCol) {
                                    cols.push(fieldname);
                                }
                            }
                        }
                    }
                    if (false === Csw.isNullOrEmpty(cswPrivate.valCol) && false === Csw.contains(cols, cswPrivate.valCol)) {
                        cols.push(cswPrivate.valCol);
                    }

                    // get data
                    for (i = 0; i < cswPrivate.dataAry.length; i += 1) {
                        thisSet = cswPrivate.dataAry[i];
                        values = [];
                        if (Csw.contains(thisSet, cswPrivate.keyCol) && Csw.contains(thisSet, cswPrivate.nameCol)) {
                            for (v = 0; v < cols.length; v += 1) {
                                if (Csw.contains(thisSet, cols[v])) {
                                    values.push(Csw.bool(thisSet[cols[v]]));
                                }
                            }
                            var dataOpts = {
                                'label': thisSet[cswPrivate.nameCol],
                                'key': thisSet[cswPrivate.keyCol],
                                'values': values
                            };
                            data.push(dataOpts);
                        }
                    }

                    cswPrivate.dataStore.cols = cols;
                    cswPrivate.dataStore.data = data;
                }
                return cswPrivate.dataStore;
            };

            (function () {
                Csw.extend(cswPrivate, options);

                cswPrivate.cbaDiv = cswParent.div({
                    ID: window.Ext.id(),
                    height: (25 * cswPrivate.HeightInRows) + 'px'
                });
                cswPublic = Csw.dom({ }, cswPrivate.cbaDiv);

                var cbaData = cswPrivate.transmogrify({
                    dataAry: cswPrivate.dataAry,
                    nameCol: cswPrivate.nameCol,
                    keyCol: cswPrivate.keyCol,
                    valCol: cswPrivate.valCol,
                    cols: cswPrivate.cols
                });
                if (false === Csw.isNullOrEmpty(cbaData)) {
                    Csw.extend(cswPrivate, cbaData);
                }
                cswPrivate.MultiIsUnchanged = cswPrivate.Multi;

                var checkType = Csw.enums.inputTypes.checkbox;
                if (cswPrivate.UseRadios) {
                    checkType = Csw.enums.inputTypes.radio;
                }

                cswPrivate.dataStore.cols = cswPrivate.cols;
                cswPrivate.dataStore.data = cswPrivate.data;

                if (cswPrivate.ReadOnly) {
                    for (var r = 0; r < cswPrivate.data.length; r += 1) {
                        var rRow = cswPrivate.data[r];
                        var rowlabeled = false;
                        var first = true;
                        for (var c = 0; c < cswPrivate.cols.length; c += 1) {
                            if (Csw.bool(rRow.values[c])) {
                                if (false === cswPrivate.Multi) {
                                    if (false === rowlabeled) {
                                        cswPrivate.cbaDiv.append(rRow.label + ": ");
                                        rowlabeled = true;
                                    }
                                    if (false === first) {
                                        cswPrivate.cbaDiv.append(", ");
                                    }
                                    if (false === cswPrivate.UseRadios) {
                                        cswPrivate.cbaDiv.append(cswPrivate.cols[c]);
                                    }
                                    first = false;
                                }
                            }
                        }
                        if (rowlabeled) {
                            cswPrivate.cbaDiv.br();
                        }
                    }
                } else {
                    var table = cswPrivate.cbaDiv.table({
                        ID: Csw.makeId(cswPrivate.ID, 'tbl')
                    });

                    cswPrivate.cbaDiv.addClass('cbarraydiv');
                    table.addClass('cbarraytable');

                    // Header
                    var tablerow = 1;
                    for (var d = 0; d < cswPrivate.cols.length; d++) {
                        var dCell = table.cell(tablerow, d + 2);
                        dCell.addClass('cbarraycell');
                        var colName = cswPrivate.cols[d];
                        if (colName === cswPrivate.valCol && false === Csw.isNullOrEmpty(cswPrivate.valColName)) {
                            colName = cswPrivate.valColName;
                        }
                        if ((colName !== cswPrivate.keyCol && colName !== cswPrivate.nameCol)) {
                            dCell.append(colName);
                        }
                    }
                    tablerow += 1;

                    //[none] row
                    if (cswPrivate.UseRadios && false === cswPrivate.Required) {
                        // Row label
                        var labelCell = table.cell(tablerow, 1).text('[none]');
                        labelCell.addClass('cbarraycell');

                        for (var e = 0; e < cswPrivate.cols.length; e += 1) {
                            var eCell = table.cell(tablerow, e + 2);
                            eCell.addClass('cbarraycell');
                            var eCheckid = cswPrivate.ID + '_none';
                            var eCheck = eCell.input({
                                type: checkType,
                                cssclass: 'CBACheckBox_' + cswPrivate.ID,
                                id: eCheckid,
                                name: cswPrivate.ID,
                                checked: false === cswPrivate.Multi
                            });
                            eCheck.propNonDom({ 'key': '', rowlabel: '[none]', collabel: cswPrivate.cols[e], row: -1, col: e });
                            var delClick = Csw.makeDelegate(cswPrivate.onChange, eCheck);
                            eCheck.click(function () {
                                cswPrivate.MultiIsUnchanged = false;
                                delClick();
                            });
                            eCheck.change(delClick);
                        } // for(var c = 0; c < cswPrivate.cols.length; c++)
                    } // if(cswPrivate.UseRadios && ! cswPrivate.Required)
                    tablerow += 1;

                    var onChange = function (cB) {
                        //var cB = this;
                        var col = cB.propNonDom('col');
                        var row = cB.propNonDom('row');
                        
                        cswPrivate.dataStore.MultiIsUnchanged = false;
                        if (Csw.contains(cswPrivate.dataStore.data, row) && Csw.contains(cswPrivate.dataStore.data[row], 'values')) {
                            cswPrivate.dataStore.data[row].values[col] = cB.$.is(':checked');
                        }
                        if (cswPrivate.UseRadios) { //we're toggling--cache the prev selected row/col to deselect on later change
                            if (Csw.contains(cswPrivate.oldDataStore, 'row') && Csw.contains(cswPrivate.oldDataStore, 'col')) {
                                if (Csw.contains(cswPrivate.dataStore.data, cswPrivate.oldDataStore.row) && Csw.contains(cswPrivate.dataStore.data[cswPrivate.oldDataStore.row], 'values')) {
                                    cswPrivate.dataStore.data[cswPrivate.oldDataStore.row].values[cswPrivate.oldDataStore.col] = false;
                                }
                            }
                            cswPrivate.oldDataStore.row = row; 
                            cswPrivate.oldDataStore.col = col;
                        }
                    };

                    // Data
                    for (var s = 0; s < cswPrivate.data.length; s += 1) {
                        var sRow = cswPrivate.data[s];
                        // Row label
                        var sLabelcell = table.cell(tablerow + s, 1).text(sRow.label);
                        sLabelcell.addClass('cbarraycell');

                        for (var f = 0; f < cswPrivate.cols.length; f += 1) {
                            var fCell = table.cell(tablerow + s, f + 2);
                            fCell.addClass('cbarraycell');
                            var fCheckid = cswPrivate.ID + '_' + s + '_' + f;
                            var fCheck = fCell.input({
                                type: checkType,
                                cssclass: 'CBACheckBox_' + cswPrivate.ID,
                                ID: fCheckid,
                                name: cswPrivate.ID,
                                onClick: cswPrivate.onChange,
                                checked: sRow.values[f]
                            });

                            fCheck.propNonDom({ key: sRow.key, rowlabel: sRow.label, collabel: cswPrivate.cols[f], row: s, col: f });
                            var delChange = Csw.makeDelegate(onChange, fCheck);
                            fCheck.change(delChange);
                            //fCheck.data('thisRow', sRow);

                            if (sRow.values[f]) {
                                if (cswPrivate.UseRadios) {
                                    cswPrivate.oldDataStore.col = f; 
                                    cswPrivate.oldDataStore.row = s;
                                }
                            }
                        } // for(var c = 0; c < cswPrivate.cols.length; c++)
                    } // for(var r = 0; r < cswPrivate.data.length; r++)

                    if (false === cswPrivate.UseRadios && cswPrivate.data.length > 0) {
                        var checkAllLinkText = 'Check All';
                        if ($('.CBACheckBox_' + cswPrivate.ID).not(':checked').length === 0) {
                            checkAllLinkText = 'Uncheck All';
                        }

                        cswPrivate.checkAllLink = cswParent.div({
                            isControl: cswPrivate.isControl,
                            align: 'right'
                        })
                            .a({
                                href: 'javascript:void(0)',
                                text: checkAllLinkText,
                                onClick: function () {
                                    cswPublic.toggleCheckAll();
                                    return false;
                                }
                            });
                    }

                } // if-else(cswPrivate.ReadOnly)

            } ());

            cswPublic.toggleCheckAll = function () {
                var checkBoxes = cswPublic.find('.CBACheckBox_' + cswPrivate.ID);
                if (checkBoxes.isValid) {
                    if (cswPrivate.checkAllLink.text() === 'Uncheck All') {
                        //if (cswPrivate.checked <= 0) {
                        //    cswPrivate.checked = cswPrivate.data.length;
                        checkBoxes.propDom('checked', 'checked'); // Yes, this checks.  But click below unchecks again.
                        cswPrivate.checkAllLink.text('Check All');
                    } else {
                        //    cswPrivate.checked = 0;
                        checkBoxes.$.removeAttr('checked'); // Yes, this unchecks.  But click below checks again.
                        cswPrivate.checkAllLink.text('Uncheck All');
                    }
                    checkBoxes.trigger('click'); // this toggles again
                }
            }; // ToggleCheckAll()

            cswPublic.val = function() {
                return cswPrivate.dataStore;
            };

            return cswPublic;
        });

} ());