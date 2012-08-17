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
                editMode: ''
            };

            var cswPublic = {};

            (function () {

                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                var optionCount = 0,
                    isMultiEdit = Csw.bool(cswPrivate.isMultiEdit),
                    values = cswPrivate.values;
                delete cswPrivate.values;

                var parentDiv = cswParent.div({
                    ID: Csw.makeId(cswPrivate.ID, 'multiListDiv')
                });

                var table = parentDiv.table({
                    ID: Csw.makeId(cswPrivate.ID, 'tbl')
                });

                var morediv = table.cell(1, 1).moreDiv({
                    ID: Csw.makeId(cswPrivate.ID, cswPrivate.ID + '_morediv')
                });

                var showntbl = morediv.shownDiv.table({ cellpadding: '2px', width: '100%' });
                var hiddentbl = morediv.hiddenDiv.table({ cellpadding: '2px', width: '100%' });
                var row = 1;
                var tbl = showntbl;
                morediv.moreLink.hide();

                cswPrivate.select = table.cell(2, 1).select(cswPrivate);
                table.cell(2, 1).hide();
                cswPublic = Csw.dom({}, cswPrivate.select);
                //Csw.extend(cswPublic, Csw.literals.select(cswPrivate));

                if (Csw.isFunction(cswPrivate.onChange)) {
                    cswPrivate.select.bind('change', function () {
                        cswPrivate.onChange(cswPrivate.select);
                    });
                }

                Csw.each(values, function (opt) {
                    //TODO - add to moreDiv
                    var value = Csw.string(opt.value, opt.text),
                        text = Csw.string(opt.text, value),
                        isSelected;
                    if (false === Csw.isNullOrEmpty(value)) {
                        isSelected = (Csw.bool(opt.selected) && false === isMultiEdit);
                        cswPrivate.select.option({ value: value, display: text, isSelected: isSelected, isDisabled: opt.disabled });
                        optionCount += 1;
                    }
                });

                var makeMultiSelect = function () {
                    table.cell(1, 1).hide();
                    table.cell(1, 2).hide();
                    if (optionCount > 20) {
                        cswPrivate.select.$.multiselect().multiselectfilter();
                    } else {
                        cswPrivate.select.$.multiselect();
                    }
                    table.cell(2, 1).show();
                }

                if (cswPrivate.EditMode === Csw.enums.editMode.Add) {
                    makeMultiSelect();
                } else {
                    table.cell(1, 2).imageButton({
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
