
/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function () {
    'use strict';


    Csw.composites.register('formula', function (cswParent, options) {
        //#region variables
        var cswPrivate = {
            name: 'csw_formula',
            nodeid: '',
            viewid: '',
            nodeKey: '',
            ReadOnly: false,
            isRequired: false,
            EditMode: Csw.enums.editMode.Edit,
            value: '',
            onChange: null,
            formattedText: ''
        };

        var cswPublic = {};
        //#endregion variables

        //#region safety net
        Csw.tryExec(function () {

            //#region init ctor
            (function _pre() {

                Csw.extend(cswPrivate, options, true);
                cswPrivate.ready = Csw.promises.all();

                cswPublic.table = cswParent.table({ TableCssClass: 'cswInline' });

                cswPrivate.spanCell = cswPublic.table.cell(1, 1);
                cswPrivate.inputCell = cswPublic.table.cell(1, 2);
                cswPrivate.editCell = cswPublic.table.cell(1, 3);

                cswPrivate.inputDiv = cswPrivate.inputCell.div({
                    value: cswPrivate.value,
                    onChange: function () {
                        cswPrivate.inputDiv.val();
                    }
                });

                cswPrivate.formattedSpan = cswPrivate.spanCell.span({
                    value: cswPrivate.formattedText,
                    onChange: function () {
                        cswPrivate.formattedSpan.val();
                    }
                });

                cswPrivate.inputCell.hide();

            }());
            //#endregion init ctor

            //#region cswPrivate/cswPublic methods and props

            cswPublic.val = function () {
                return cswPrivate.value;
            };

            cswPublic.valFormatted = function () {
                return cswPrivate.formattedText;
            };

            cswPrivate.makeFormulaInput = function () {
                cswPrivate.spanCell.hide();
                cswPrivate.editCell.hide();
                cswPrivate.inputCell.show();

                cswPublic.input = cswPrivate.inputDiv.input({
                    name: cswPrivate.name + '_input',
                    value: cswPrivate.value,
                    onChange: cswPrivate.onChange,
                    width: '290px',
                });
            }; // makeLocationCombo()

            //#endregion cswPrivate/cswPublic methods and props

            //#region final ctor
            (function _post() {
                cswPrivate.ready.then(function () {
                    if (false === cswPrivate.ReadOnly) {
                        if (cswPrivate.EditMode === Csw.enums.editMode.Add) {
                            cswPrivate.makeFormulaInput();
                        } else {
                            cswPrivate.editCell.icon({
                                name: cswPrivate.name + '_toggle',
                                iconType: Csw.enums.iconType.pencil,
                                hovertext: 'Edit',
                                size: 16,
                                isButton: true,
                                onClick: cswPrivate.makeFormulaInput
                            }); // imageButton
                        }
                    }
                });
            }());
            //#region final ctor

        });
        //#endregion safety net

        return cswPublic;
    });

}());


