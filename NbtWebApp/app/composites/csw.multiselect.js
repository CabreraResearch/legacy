/// <reference path="~/app/CswApp-vsdoc.js" />


//See http://www.erichynds.com/examples/jquery-ui-multiselect-widget/demos for fancy options

(function () {
    'use strict';


    Csw.composites.register('multiSelect', function (cswParent, options) {

        var cswPrivate = {
            $parent: '',
            name: '',
            values: [],
            valStr: '',
            multiple: true,
            cssclass: '',
            readonlyless: '',
            readonlymore: '',
            isMultiEdit: false,
            onChange: null, //function () {}
            onValidate: null,
            isControl: false,
            EditMode: '',
            required: false,
        };

        var cswPublic = {};
        var multiSelectCtrl;
        var valStr;
        var valArr;
        
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
            valStr = cswPrivate.valStr;

            delete cswPrivate.values;
            morediv.moreLink.hide();
            cswPrivate.select = multiSelectCell.select(cswPrivate);
            multiSelectCell.hide();
            cswPublic = Csw.dom({}, cswPrivate.select);

            if (false === Csw.isNullOrEmpty(cswPrivate.readonlyless)) {
                morediv.shownDiv.span({ text: cswPrivate.readonlyless });
                if (false === Csw.isNullOrEmpty(cswPrivate.readonlymore)) {
                    morediv.hiddenDiv.span({ text: cswPrivate.readonlymore });
                    morediv.moreLink.show();
                }
            }

            var makeMultiSelect = function (inDialog, div, height, width) {
                Csw.dialogs.multiselectedit({
                    opts: values,
                    title: 'Edit Prop',
                    required: cswPrivate.required,
                    inDialog: inDialog,
                    parent: div,
                    height: height,
                    width: width,
                    onChange: function (updatedValues) {
                        valArr = updatedValues;
                        Csw.tryExec(cswPrivate.onChange, updatedValues);
                    },
                    onSave: function (updatedValues) {
                        valArr = updatedValues;
                        var isValid = Csw.tryExec(cswPrivate.onChange, updatedValues);
                        if (isValid) {
                            Csw.publish('triggerSave');
                        }
                        return isValid;
                    },
                });
            };

            if (cswPrivate.EditMode === Csw.enums.editMode.Add) {
                morediv.hide();
                multiSelectCtrl = makeMultiSelect(false, parentDiv, '240px', '360px');
            } else {
                editBtnCell.icon({
                    name: cswPrivate.name + '_toggle',
                    iconType: Csw.enums.iconType.pencil,
                    hovertext: 'Edit',
                    size: 16,
                    isButton: true,
                    onClick: function() {
                        makeMultiSelect(true);
                    }
                });
            }

        }());

        cswPublic.val = function () {
            var ret = valStr;
            if (valArr) {
                valStr = valArr.join(',');
                ret = valStr;
            }
            return ret;
        };

        cswPublic.getValue = function () { //need func with this name for the Csw.validator
            return cswPublic.val(); 
        };

        return cswPublic;
    });


}());
