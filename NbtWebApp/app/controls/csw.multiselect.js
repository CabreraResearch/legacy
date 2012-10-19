/// <reference path="~/app/CswApp-vsdoc.js" />


//See http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos for fancy options

(function () {
    'use strict';

    Csw.controls.multiSelect = Csw.controls.multiSelect ||
        Csw.controls.register('multiSelect', function (cswParent, options) {

            var cswPrivate = {
                $parent: '',
                name: '',
                values: [],
                multiple: true,
                cssclass: '',
                readonlyless: '',
                readonlymore: '',
                isMultiEdit: false,
                onChange: null, //function () {}
                isControl: false,
                EditMode: ''
            };

            var cswPublic = {};

            (function () {

                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                var optionCount = 0,
                    isMultiEdit = Csw.bool(cswPrivate.isMultiEdit),
                    values = cswPrivate.values,
                    parentDiv = cswParent.div({ name: 'multiListDiv' }),
                    table = parentDiv.table({ name: 'tbl' }),
                    moreDivCell = table.cell(1, 1),
                    editBtnCell = table.cell(1, 2),
                    multiSelectCell = table.cell(2, 1),
                    morediv = moreDivCell.moreDiv({ name: cswPrivate.name + '_morediv' });

                delete cswPrivate.values;
                morediv.moreLink.hide();
                cswPrivate.select = multiSelectCell.select(cswPrivate);
                multiSelectCell.hide();
                cswPublic = Csw.dom({}, cswPrivate.select);

                Csw.each(values, function (opt) {
                    var value = Csw.string(opt.value, opt.text),
                        text = Csw.string(opt.text, value),
                        isSelected;
                    if (false === Csw.isNullOrEmpty(value)) {
                        isSelected = (Csw.bool(opt.selected) && false === isMultiEdit);
                        cswPrivate.select.option({ value: value, display: text, isSelected: isSelected, isDisabled: opt.disabled });
                        optionCount += 1;
                    }
                });

                if (false === Csw.isNullOrEmpty(cswPrivate.readonlyless)) {
                    morediv.shownDiv.span({ text: cswPrivate.readonlyless });
                    if (false === Csw.isNullOrEmpty(cswPrivate.readonlymore)) {
                        morediv.hiddenDiv.span({ text: cswPrivate.readonlymore });
                        morediv.moreLink.show();
                    }
                }

                cswPrivate.multiSelect = { };
                var makeMultiSelect = function() {
                    moreDivCell.hide();
                    editBtnCell.hide();
                    cswPrivate.multiSelect = cswPrivate.select.$.multiselect({
                        click: Csw.method(cswPrivate.onChange, cswPrivate.select),
                        checkall: Csw.method(cswPrivate.onChange, cswPrivate.select),
                        uncheckall: Csw.method(cswPrivate.onChange, cswPrivate.select),
                        optgrouptoggle: Csw.method(cswPrivate.onChange, cswPrivate.select),
                        close: Csw.method(cswPrivate.onChange, cswPrivate.select)
                    });
                    if (optionCount > 20) {
                        cswPrivate.multiSelect.multiselectfilter();
                    } 
                    
                    multiSelectCell.show();
                };

                if (cswPrivate.EditMode === Csw.enums.editMode.Add) {
                    makeMultiSelect();
                } else {
                    editBtnCell.icon({
                        name: cswPrivate.name + '_toggle',
                        iconType: Csw.enums.iconType.pencil,
                        hovertext: 'Edit',
                        size: 16,
                        isButton: true,
                        onClick: makeMultiSelect
                    });
                }

            } ());

            cswPublic.val = function () {
                //In IE an empty array is frequently !== [], rather === null
                var values = cswPrivate.select.$.val() || [],
                    valArray = values.sort();
                return valArray.join(',');
            };

            return cswPublic;
        });


} ());
