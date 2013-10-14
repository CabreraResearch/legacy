/// <reference path="~/app/CswApp-vsdoc.js" />
(function () {

    Csw.dialogs.register('multiselectedit', function (cswPrivate) {
        'use strict';

        var cswPublic = {};

        (function _preCtor() {
            cswPrivate.title = cswPrivate.title || 'Edit Property';
            cswPrivate.opts = cswPrivate.opts || [];
            cswPrivate.onSave = cswPrivate.onSave || function () { };
        }());

        cswPrivate.multiSelectEditDialog = (function () {
            'use strict';
            var selected = [];

            var editDialog = Csw.layouts.dialog({
                title: cswPrivate.title,
                width: 800,
                height: 600,
                onOpen: function () {
                    var ctrlOpts = [];
                    var filterInput = editDialog.div.input({
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

                    var btnTbl = editDialog.div.table({
                        cellpadding: 5
                    });
                    btnTbl.cell(1, 1).div().buttonExt({
                        enabledText: 'Uncheck All',
                        onClick: function () {
                            Csw.iterate(ctrlOpts, function (opt) {
                                opt.ctrl.checkbox.checked(false);
                            });
                        }
                    });
                    btnTbl.cell(1, 2).div().buttonExt({
                        enabledText: 'Check All',
                        onClick: function () {
                            Csw.iterate(ctrlOpts, function (opt) {
                                opt.ctrl.checkbox.checked(true);
                            });
                        }
                    });

                    var optsDiv = editDialog.div.div();
                    optsDiv.css({ 'height': '400px', 'width': '750px', 'overflow': 'auto', 'border': '1px solid #AED0EA' });

                    var optsTbl = optsDiv.table().css('padding', '10px');

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
                                var selectedVal = ctrlOpts[thisRowIdx - 1].val;
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
                            }
                        });
                        cell.span({ text: opt.text, value: opt.text });
                        ctrlOpts.push({
                            text: opt.text,
                            val: opt.value,
                            ctrl: { ctrlDiv: cell, checkbox: checkBox }
                        });
                        rowIdx++;
                    });

                    editDialog.div.buttonExt({
                        enabledText: 'Save Changes',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.save),
                        onClick: function () {
                            Csw.tryExec(cswPrivate.onSave, selected);
                            editDialog.close();
                        }
                    }).css('margin-top', '20px');

                }
            });

            return editDialog;
        }());

        (function _postCtor() {
            cswPrivate.multiSelectEditDialog.open();
        }());

        return cswPublic;
    });
}());