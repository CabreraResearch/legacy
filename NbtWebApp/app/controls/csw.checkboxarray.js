/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswCheckBoxArray() {
    "use strict";

    Csw.controls.checkBoxArray = Csw.controls.checkBoxArray ||
        Csw.controls.register('checkBoxArray', function (cswParent, options) {

            if (Csw.isNullOrEmpty(cswParent)) {
                throw new Error('Cannot instance a Csw component without a Csw control');
            }

            var cswPrivate = {
                ID: '',
                cols: [], // [ 'Included', ... ]
                data: [], // [{ label: 'Option1', key: '1', values: [ true, ... ] }, ... ]
                HeightInRows: 4,
                UseRadios: false,
                Required: false,
                ReadOnly: false,
                Multi: false,
                onChange: null,
                nameCol: 'label',
                keyCol: 'key',
                valCol: 'values',
                useEditButton: true
            };
            var cswPublic = {};




            cswPrivate.makeTextValue = function () {
                var ret = '', r, c;

                if(cswPrivate.cols.length === 1) {
                    for (r = 0; r < cswPrivate.data.length; r += 1) {
                        var row = cswPrivate.data[r];
                        if (Csw.bool(row[cswPrivate.valCol][0])) {
                            if(false === Csw.isNullOrEmpty(ret)) {
                                ret += ', ';
                            }
                            ret += row[cswPrivate.nameCol];
                        }
                    }
                } else {
                    for (r = 0; r < cswPrivate.data.length; r += 1) {
                        var row = cswPrivate.data[r];
                        var rowlabeled = false;
                        var first = true;
                        for (c = 0; c < cswPrivate.cols.length; c += 1) {
                            if (Csw.bool(row[cswPrivate.valCol][c])) {
                                if (false === rowlabeled) {
                                    ret += row[cswPrivate.nameCol];
                                    rowlabeled = true;
                                }
                                if (first) {
                                    ret += ": ";
                                } else {
                                    ret += ", ";
                                }
                                ret += cswPrivate.cols[c];
                                first = false;
                            }
                        }
                        if (rowlabeled) {
                            ret += '<br>';
                        }
                    } // for (r = 0; r < cswPrivate.data.length; r += 1) {
                }
                return ret;
            }; // makeTextValue


            cswPrivate.makeCheckboxRow = function (table, tablerownum, rowdata, rownum) {
                // Row label
                var labelcell = table.cell(tablerownum, 1);
                labelcell.text(rowdata[cswPrivate.nameCol]);
                labelcell.addClass('cbarraycell');

                for (var c = 0; c < cswPrivate.cols.length; c += 1) {
                    var cell = table.cell(tablerownum, c + 2);
                    cell.addClass('cbarraycell');

                    var checkid = cswPrivate.ID + '_' + rownum + '_' + c;
                    var checkType = Csw.enums.inputTypes.checkbox;
                    if (cswPrivate.UseRadios) {
                        checkType = Csw.enums.inputTypes.radio;
                    }

                    var check = cell.input({
                        type: checkType,
                        cssclass: 'CBACheckBox_' + cswPrivate.ID,
                        ID: checkid,
                        name: cswPrivate.ID,
                        checked: rowdata[cswPrivate.valCol][c]
                    });

                    check.propNonDom({
                        key: rowdata[cswPrivate.keyCol],
                        rowlabel: rowdata[cswPrivate.nameCol],
                        collabel: cswPrivate.cols[c],
                        row: rownum,
                        col: c
                    });

                    var onChange = function (cB) {
                        var col = cB.propNonDom('col');
                        var row = cB.propNonDom('row');
                        if(cswPrivate.UseRadios) {
                            // clear all other values
                            for (var r = 0; r < cswPrivate.data.length; r += 1) {
                                cswPrivate.data[r][cswPrivate.valCol][col] = false;
                            }
                        }
                        if (row >= 0) {
                            cswPrivate.data[row][cswPrivate.valCol][col] = cB.$.is(':checked');
                        }

                        Csw.tryExec(cswPrivate.onChange);
                    };
                    var delChange = Csw.makeDelegate(onChange, check);
                    check.change(delChange);
                } // for(var c = 0; c < cswPrivate.cols.length; c++)

            }; // makeCheckboxRow()


            cswPrivate.makeTable = function (parent) {
                parent.empty();

                var table = parent.table({
                    ID: Csw.makeId(cswPrivate.ID, 'tbl')
                });

                parent.addClass('cbarraydiv');
                table.addClass('cbarraytable');

                // Header
                var tablerow = 1;
                for (var d = 0; d < cswPrivate.cols.length; d++) {
                    var dCell = table.cell(tablerow, d + 2);
                    dCell.addClass('cbarraycell');
                    var colName = cswPrivate.cols[d];
                    if ((colName !== cswPrivate.keyCol && colName !== cswPrivate.nameCol)) {
                        dCell.append(colName);
                    }
                }
                tablerow += 1;

                //[none] row
                if (cswPrivate.UseRadios && false === cswPrivate.Required) {
                    var noneRowData = {};
                    noneRowData[cswPrivate.nameCol] = '[none]';
                    noneRowData[cswPrivate.keyCol] = '';
                    noneRowData[cswPrivate.valCol] = [];
                    for (var e = 0; e < cswPrivate.cols.length; e += 1) {
                        noneRowData[cswPrivate.valCol][e] = true;
                    }
                    cswPrivate.makeCheckboxRow(table, tablerow, noneRowData, -1);
                } // if(cswPrivate.UseRadios && ! cswPrivate.Required)
                tablerow += 1;


                // Data
                for (var s = 0; s < cswPrivate.data.length; s += 1) {
                    cswPrivate.makeCheckboxRow(table, tablerow + s, cswPrivate.data[s], s);
                }

                if (false === cswPrivate.UseRadios && cswPrivate.data.length > 0) {
                    var checkAllLinkText = 'Check All';
                    if ($('.CBACheckBox_' + cswPrivate.ID).not(':checked').length === 0) {
                        checkAllLinkText = 'Uncheck All';
                    }

                    cswPrivate.checkAllLink = cswParent.div({
                        isControl: cswPrivate.isControl,
                        align: 'right'
                    }).a({
                        href: 'javascript:void(0)',
                        text: checkAllLinkText,
                        onClick: function () {
                            cswPublic.toggleCheckAll();
                            return false;
                        }
                    });
                } // if (false === cswPrivate.UseRadios && cswPrivate.data.length > 0) {
            }; // makeTable()


            // Constructor
            (function () {
                Csw.extend(cswPrivate, options);

                // link data directly (our version of databinding)
                cswPrivate.data = options.data;

                cswPrivate.cbaDiv = cswParent.div({
                    ID: cswPrivate.ID
                });
                cswPublic = Csw.dom({}, cswPrivate.cbaDiv);

                if (cswPrivate.useEditButton || cswPrivate.ReadOnly) {
                    if (cswPrivate.Multi) {
                        cswPrivate.cbaDiv.append(Csw.enums.multiEditDefaultValue);
                    } else {
                        cswPrivate.cbaDiv.append(cswPrivate.makeTextValue());
                    }

                    if (false === cswPrivate.ReadOnly) {
                        cswPrivate.editButton = cswPrivate.cbaDiv.icon({
                            iconType: Csw.enums.iconType.pencil,
                            isButton: true,
                            ID: Csw.makeId(cswPrivate.ID, 'edit'),
                            onClick: function () {
                                cswPrivate.cbaDiv.css({
                                    height: (25 * cswPrivate.HeightInRows) + 'px'
                                });
                                cswPrivate.makeTable(cswPrivate.cbaDiv);
                            } // onClick
                        }); // editButton
                    }
                } else {
                    cswPrivate.makeTable(cswPrivate.cbaDiv);
                };
            } ()); // constructor


            cswPublic.toggleCheckAll = function () {
                var checkBoxes = cswPublic.find('.CBACheckBox_' + cswPrivate.ID);
                if (checkBoxes.isValid) {
                    if (cswPrivate.checkAllLink.text() === 'Uncheck All') {
                        checkBoxes.propDom('checked', 'checked'); // Yes, this checks.  But click below unchecks again.
                        cswPrivate.checkAllLink.text('Check All');
                    } else {
                        checkBoxes.$.removeAttr('checked'); // Yes, this unchecks.  But click below checks again.
                        cswPrivate.checkAllLink.text('Uncheck All');
                    }
                    checkBoxes.trigger('click'); // this toggles again
                }
            }; // ToggleCheckAll()


            cswPublic.val = function () {
                return cswPrivate.data;
            };

            return cswPublic;
        });

} ());