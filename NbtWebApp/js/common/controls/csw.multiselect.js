/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

//See http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos for fancy options

(function () {
    'use strict';

    Csw.controls.multiSelect = Csw.controls.multiSelect ||
        Csw.controls.register('multiSelect', function (cswParent, options) {

            var cswPrivate = {
                $parent: '',
                ID: '',
                values: [],
                multiple: true,
                cssclass: '',
                isMultiEdit: false,
                onChange: null, //function () {}
                isControl: false,
                hidethreshold: 5,
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
                    parentDiv = cswParent.div({ ID: Csw.makeId(cswPrivate.ID, 'multiListDiv') }),
                    table = parentDiv.table({ ID: Csw.makeId(cswPrivate.ID, 'tbl') }),
                    moreDivCell = table.cell(1, 1),
                    editBtnCell = table.cell(1, 2),
                    multiSelectCell = table.cell(2, 1),
                    morediv = moreDivCell.moreDiv({ ID: Csw.makeId(cswPrivate.ID, cswPrivate.ID + '_morediv') }),
                    showntbl = morediv.shownDiv.table({ cellpadding: '2px', width: '100%' }),
                    hiddentbl = morediv.hiddenDiv.table({ cellpadding: '2px', width: '100%' }),
                    moreDivTbl = showntbl,
                    row = 1,
                    optionSubject = '';

                delete cswPrivate.values;
                morediv.moreLink.hide();
                cswPrivate.select = multiSelectCell.select(cswPrivate);
                multiSelectCell.hide();
                cswPublic = Csw.dom({}, cswPrivate.select);

                if (Csw.isFunction(cswPrivate.onChange)) {
                    cswPrivate.select.bind('change', function () {
                        cswPrivate.onChange(cswPrivate.select);
                    });
                }

                Csw.each(values, function (opt) {                    
                    var value = Csw.string(opt.value, opt.text),
                        text = Csw.string(opt.text, value),
                        isSelected;
                    if (false === Csw.isNullOrEmpty(value)) {
                        isSelected = (Csw.bool(opt.selected) && false === isMultiEdit);
                        cswPrivate.select.option({ value: value, display: text, isSelected: isSelected, isDisabled: opt.disabled });
                        optionCount += 1;
                        if (isSelected) {
                            var splitOption = text.split(": ");
                            if (splitOption[0] === optionSubject) {
                                moreDivTbl.cell(row, 1).span({ text: ', ' + splitOption[1] });
                            } else {
                                row += 1;
                                if (row > cswPrivate.hidethreshold && moreDivTbl === showntbl) {
                                    row = 1;
                                    moreDivTbl = hiddentbl;
                                    morediv.moreLink.show();
                                }
                                optionSubject = splitOption[0];
                                moreDivTbl.cell(row, 1).span({ text: text });                                
                            }
                        }
                    }
                });

                var makeMultiSelect = function () {
                    moreDivCell.hide();
                    editBtnCell.hide();
                    if (optionCount > 20) {
                        cswPrivate.select.$.multiselect().multiselectfilter();
                    } else {
                        cswPrivate.select.$.multiselect();
                    }
                    multiSelectCell.show();
                }

                if (cswPrivate.EditMode === Csw.enums.editMode.Add) {
                    makeMultiSelect();
                } else {
                    editBtnCell.imageButton({
                        ButtonType: Csw.enums.imageButton_ButtonType.Edit,
                        AlternateText: 'Edit',
                        ID: Csw.makeId(cswPrivate.ID, 'toggle'),
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
