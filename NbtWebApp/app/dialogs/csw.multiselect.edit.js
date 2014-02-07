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
            cswPrivate.usePaging = cswPrivate.usePaging || false;
            cswPrivate.itemsPerPage = cswPrivate.itemsPerPage || 100;
            cswPrivate.currentPage = 1;
            cswPrivate.totalPages = calculateTotalPages();
        }());
        
        function calculateTotalPages() {
            var total = Math.round(Object.keys(cswPrivate.opts).length / cswPrivate.itemsPerPage);
            if (total === 0) {
                return 1;
            } else {
                return total;
            }
        }

        cswPrivate.pageInfo = function () {
            return "Page " + cswPrivate.currentPage + " of " + cswPrivate.totalPages;
        };

        cswPrivate.make = function () {

            cswPublic.val = function () {
                return cswPrivate.selected;
            };

            var makeCtrl = function (multiSelectDiv) {
                cswPrivate.ctrlOpts = {};
                var filterInput = multiSelectDiv.input({
                    labelText: 'Filter:',
                    placeholder: 'Enter keywords',
                    onKeyUp: function () {
                        Csw.iterate(cswPrivate.ctrlOpts, function (item) {
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
                        Csw.iterate(cswPrivate.ctrlOpts, function (opt) {
                            onCheck(opt.ctrl.checkbox, opt.rowIdx, opt.page, false);
                        });
                    }
                });
                btnTbl.cell(1, 2).div().buttonExt({
                    enabledText: 'Check All',
                    onClick: function () {
                        Csw.iterate(cswPrivate.ctrlOpts, function (opt) {
                            onCheck(opt.ctrl.checkbox, opt.rowIdx, opt.page, true);
                        });
                    }
                });

                errorDiv = multiSelectDiv.div().span({ text: 'At least one value must be selected' }).css('color', 'red');
                errorDiv.hide();

                optsDiv = multiSelectDiv.div();
                optsDiv.css({ 'height': cswPrivate.height, 'width': cswPrivate.width, 'overflow': 'auto', 'border': '1px solid #AED0EA' });

                cswPrivate.optsTbl = optsDiv.table().css('padding', '10px');

                if (cswPrivate.usePaging) {

                    pagingDiv = multiSelectDiv.div();
                    cswPrivate.pagingTbl = pagingDiv.table().css('padding', '10px');

                    cswPrivate.pagingTbl.cell(1, 1).buttonExt({
                        enabledText: 'Previous',
                        onClick: function() {
                            onPrevious();
                        }
                    });

                    cswPrivate.pagingTbl.cell(1, 2).span({
                        text: '&nbsp;&nbsp;&nbsp;'
                    });

                    cswPrivate.pagingTbl.cell(1, 3).buttonExt({
                        enabledText: 'Next',
                        onClick: function() {
                            onNext();
                        }
                    });

                    cswPrivate.pagingTbl.cell(1, 3).span({
                        text: '&nbsp;&nbsp;&nbsp;'
                    });

                    cswPrivate.pageDisplay = cswPrivate.pagingTbl.cell(1, 5).span({ text: cswPrivate.pageInfo() });
                }//if (cswPrivate.usePaging)

            };//makeCtrl()

            cswPrivate.selected = [];
            var saveBtnClicked = false;
            var optsDiv, errorDiv, pagingDiv;

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
                                var isValid = Csw.tryExec(cswPrivate.onSave, cswPrivate.selected);
                                if (isValid || false === cswPrivate.required) {
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

        function onPrevious() {
            var currentPage = cswPrivate.currentPage;
            var previousPage = cswPrivate.currentPage - 1;

            if (currentPage != 1) {

                // Hide all of the options from last page
                Csw.iterate(cswPrivate.ctrlOpts, function (item) {
                    if (item.page = currentPage) {
                        item.ctrl.ctrlDiv.hide();
                    }
                });

                // Create new ones (if necessary) and show them
                cswPrivate.currentPage = previousPage;
                addOptions();

                cswPrivate.pageDisplay.text(cswPrivate.pageInfo());
            }
        }//onPrevious()

        function onNext() {
            var currentPage = cswPrivate.currentPage;
            var nextPage = cswPrivate.currentPage + 1;

            if (false === (nextPage > cswPrivate.totalPages)) {

                // Hide all of the options from last page
                Csw.iterate(cswPrivate.ctrlOpts, function (item) {
                    if (item.page = currentPage) {
                        item.ctrl.ctrlDiv.hide();
                    }
                });

                // Create new ones (if necessary) and show them
                cswPrivate.currentPage = nextPage;
                addOptions();

                cswPrivate.pageDisplay.text(cswPrivate.pageInfo());
            }
        }//onNext()

        function onCheck(checkBox, idx, page, isChecked) {
            if (false === checkBox.$.is(':hidden')) {
                var selectedVal = cswPrivate.ctrlOpts[page + "_" + idx].val;
                var selectedIdx = cswPrivate.selected.indexOf(selectedVal);
                document.getElementById(checkBox.getId()).checked = isChecked;
                Csw.clientChanges.setChanged();
                if (checkBox.checked()) {
                    if (false == (selectedIdx > -1)) {
                        cswPrivate.selected.push(selectedVal);
                    }
                } else {
                    if (selectedIdx > -1) {
                        cswPrivate.selected.splice(selectedIdx, 1);
                    }
                }
            }
            cswPrivate.onChange(cswPrivate.selected);
        };

        function addOptions() {

            var opts = [];

            if (cswPrivate.usePaging) {
                var start = (cswPrivate.currentPage - 1) * (cswPrivate.itemsPerPage);
                var end = start + cswPrivate.itemsPerPage - 1;

                if (cswPrivate.currentPage === cswPrivate.totalPages) {
                    end = cswPrivate.opts.length - 1;
                }

                for (var i = start; i < end + 1; i++) {
                    opts.push(cswPrivate.opts[i]);
                }
            } else {
                opts = cswPrivate.opts;
            }

            var page = cswPrivate.currentPage;
            var rowIdx = 1;
            Csw.iterate(opts, function (opt) {
                var thisRowIdx = rowIdx;

                if (Csw.isNullOrUndefined(cswPrivate.ctrlOpts[page + '_' + thisRowIdx])) {

                    if (Csw.bool(opt.selected)) {
                        cswPrivate.selected.push(opt.value);
                    }

                    var cell = cswPrivate.optsTbl.cell(thisRowIdx, 1).div();
                    var checkBox = cell.input({
                        name: 'chkbx' + rowIdx,
                        type: Csw.enums.inputTypes.checkbox,
                        canCheck: true,
                        checked: Csw.bool(opt.selected),
                        onChange: function () {
                            onCheck(checkBox, thisRowIdx, page, checkBox.checked());
                        }
                    });
                    cell.span({ text: opt.text, value: opt.text });
                    cswPrivate.ctrlOpts[page + '_' + thisRowIdx] =
                    {
                        text: opt.text,
                        val: opt.value,
                        rowIdx: thisRowIdx,
                        page: page,
                        ctrl: { ctrlDiv: cell, checkbox: checkBox }
                    };
                } else {
                    cswPrivate.ctrlOpts[page + '_' + thisRowIdx].ctrl.ctrlDiv.show();
                }
                rowIdx++;
            });
        }

        (function _postCtor() {
            cswPrivate.make();
            addOptions();
        }());

        return cswPublic;
    });
}());