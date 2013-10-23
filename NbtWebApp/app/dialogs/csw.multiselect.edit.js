/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('multiselectedit', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || 'Edit Property';
            cswPrivate.opts = cswPrivate.opts || [];
            cswPrivate.onSave = cswPrivate.onSave || function () { };
            cswPrivate.required = cswPrivate.required || false;
            cswPrivate.inDialog = cswPrivate.inDialog || false;
            cswPrivate.parent = cswPrivate.parent; //for when we don't render in a dialog,
            cswPrivate.height = cswPrivate.height || '400px';
            cswPrivate.width = cswPrivate.width || '750px';
            cswPrivate.onChange = cswPrivate.onChange || function () { };
        }());

        cswPrivate.make = function () {

            cswPublic.val = function () {
                return selected;
            };

            var makeCtrl = function (multiSelectDiv) {
                var ctrlOpts = [];
                var filterInput = multiSelectDiv.input({
                    labelText: 'Filter:',
                    placeholder: 'Enter keywords',
                    onKeyUp: function () {
                        Csw.iterate(ctrlOpts, function (item) {
                            if (false === item.text.toLowerCase().contains(filterInput.val().toLowerCase())) {
                                item.ctrl.ctrlDiv.hide();
                            } else {
                                item.ctrl.ctrlDiv.show();
                            }
                        });
                    }
                });

                var btnTbl = multiSelectDiv.table({
                    cellpadding: 5
                });
                btnTbl.cell(1, 1).div().buttonExt({
                    enabledText: 'Uncheck All',
                    onClick: function () {
                        Csw.iterate(ctrlOpts, function (opt) {
                            opt.ctrl.checkbox.checked(false);
                            onCheck(opt.ctrl.checkbox, opt.rowIdx);
                        });
                    }
                });
                btnTbl.cell(1, 2).div().buttonExt({
                    enabledText: 'Check All',
                    onClick: function () {
                        Csw.iterate(ctrlOpts, function (opt) {
                            document.getElementById(opt.ctrl.checkbox.getId()).checked = true;
                            onCheck(opt.ctrl.checkbox, opt.rowIdx);
                        });
                    }
                });

                errorDiv = multiSelectDiv.div().span({ text: 'At least one value must be selected' }).css('color', 'red');
                errorDiv.hide();

                optsDiv = multiSelectDiv.div();
                optsDiv.css({ 'height': cswPrivate.height, 'width': cswPrivate.width, 'overflow': 'auto', 'border': '1px solid #AED0EA' });

                var optsTbl = optsDiv.table().css('padding', '10px');

                var onCheck = function (checkBox, idx) {
                    Csw.clientChanges.setChanged();
                    var selectedVal = ctrlOpts[idx - 1].val;
                    var selectedIdx = selected.indexOf(selectedVal);
                    if (checkBox.checked()) {
                        if (false == (selectedIdx > -1)) {
                            selected.push(selectedVal);
                        }
                    } else {
                        if (selectedIdx > -1) {
                            selected.splice(selectedIdx, 1);
                        }
                    }
                    cswPrivate.onChange(selected);
                };

                var rowIdx = 1;
                Csw.iterate(cswPrivate.opts, function (opt) {
                    var thisRowIdx = rowIdx;
                    if (Csw.bool(opt.selected)) {
                        selected.push(opt.value);
                    }

                    var cell = optsTbl.cell(thisRowIdx, 1).div();
                    var checkBox = cell.input({
                        name: 'chkbx' + rowIdx,
                        type: Csw.enums.inputTypes.checkbox,
                        canCheck: true,
                        checked: Csw.bool(opt.selected),
                        onChange: function () {
                            onCheck(checkBox, thisRowIdx);
                        }
                    });
                    cell.span({ text: opt.text, value: opt.text });
                    ctrlOpts.push({
                        text: opt.text,
                        val: opt.value,
                        rowIdx: thisRowIdx,
                        ctrl: { ctrlDiv: cell, checkbox: checkBox }
                    });
                    rowIdx++;
                });
            };

            var selected = [];
            var saveBtnClicked = false;
            var optsDiv, errorDiv;

            if (cswPrivate.inDialog) {
                var editDialog = Csw.layouts.dialog({
                    title: cswPrivate.title,
                    width: 800,
                    height: 600,
                    onOpen: function () {
                        makeCtrl(editDialog.div);

                        editDialog.div.buttonExt({
                            enabledText: 'Save Changes',
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                            onClick: function () {
                                errorDiv.hide();
                                saveBtnClicked = true;
                                Csw.clientChanges.unsetChanged(); //closing a csw.dialog fires manual validation, which we don't want here
                                var isValid = Csw.tryExec(cswPrivate.onSave, selected);
                                if (isValid) {
                                    editDialog.close();
                                } else {
                                    errorDiv.show();
                                }
                            }
                        }).css('margin-top', '20px');

                    }
                });
                editDialog.open();
            } else {
                makeCtrl(cswPrivate.parent);
            }
        };

        (function _postCtor() {
            cswPrivate.make();
        }());

        return cswPublic;
    });
}());