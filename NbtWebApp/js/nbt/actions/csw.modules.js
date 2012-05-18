/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.actions.modules = Csw.actions.modules ||
        Csw.actions.register('modules', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
                urlMethod: 'getModules',
                saveUrlMethod: 'saveModules',
                ID: 'action_modules',
                onModuleChange: null // function() {}
            };
            if (options) $.extend(cswPrivateVar, options);

            // constructor
            cswPrivateVar.init = function () {

                cswParent.$.empty();

                cswPrivateVar.table = cswParent.table({
                    ID: cswPrivateVar.ID,
                    cellpadding: '3px',
                    FirstCellRightAlign: true
                }).css({ 'padding-top': '5px' });

                Csw.ajax.post({
                    urlMethod: cswPrivateVar.urlMethod,
                    data: {},
                    success: function (result) {
                        var row = 1;
                        cswPrivateVar.table.cell(row, 1).css({ 'font-weight': 'bold' }).append('Enabled');
                        cswPrivateVar.table.cell(row, 2).css({ 'font-weight': 'bold' }).append('Module');
                        row++;

                        var checkboxes = [];

                        Csw.each(result, function (thisValue, thisModule) {
                            checkboxes.push(cswPrivateVar.table.cell(row, 1).input({
                                ID: thisModule,
                                name: thisModule,
                                type: Csw.enums.inputTypes.checkbox,
                                checked: Csw.bool(thisValue)
                            }));
                            cswPrivateVar.table.cell(row, 2).append(thisModule);
                            row++;
                        }); //each()

                        cswPrivateVar.table.cell(row, 2).button({
                            ID: 'savebtn',
                            enabledText: 'Save',
                            disabledText: 'Saving...',
                            onClick: function () {
                                var changes = result;
                                Csw.each(checkboxes, function (thisCheckbox) {
                                    changes[thisCheckbox.propDom('name')] = thisCheckbox.checked();
                                }); // each

                                Csw.ajax.post({
                                    urlMethod: cswPrivateVar.saveUrlMethod,
                                    data: { Modules: JSON.stringify(changes) },
                                    success: function (result) {
                                        Csw.tryExec(cswPrivateVar.onModuleChange);
                                        cswPrivateVar.init();
                                    } // success
                                }); // ajax

                            } // onClick()
                        }); // button()

                    } // success
                }); // ajax()

            } // cswPrivateVar.init()

            cswPrivateVar.init();

        }); // register()
} ());
