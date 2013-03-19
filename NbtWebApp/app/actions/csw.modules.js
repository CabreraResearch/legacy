/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.modules = Csw.actions.modules ||
        Csw.actions.register('modules', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                urlMethod: 'getModules',
                saveUrlMethod: 'saveModules',
                name: 'action_modules',
                onModuleChange: null // function() {}
            };
            if (options) Csw.extend(cswPrivate, options);

            // constructor
            cswPrivate.init = function () {

                cswParent.$.empty();

                cswPrivate.table = cswParent.table({
                    name: cswPrivate.name,
                    cellpadding: '3px',
                    FirstCellRightAlign: true
                }).css({ 'padding-top': '5px' });

                Csw.ajax.post({
                    urlMethod: cswPrivate.urlMethod,
                    data: {},
                    success: function (result) {
                        var row = 1;
                        cswPrivate.table.cell(row, 1).css({ 'font-weight': 'bold' }).append('Enabled');
                        cswPrivate.table.cell(row, 2).css({ 'font-weight': 'bold' }).append('Module');
                        row++;

                        var checkboxes = {};

                        var resetEnabled = function () {
                            Csw.each(checkboxes, function (thisCheckbox, thisModule) {
                                var prereq = thisCheckbox.data('prereq');
                                if (prereq !== '' && false === checkboxes[prereq].checked()) {
                                    thisCheckbox.disable();
                                    thisCheckbox.checked(false);
                                    var row = thisCheckbox.data('row');
                                    cswPrivate.table.cell(row, 3).span({ text: 'Requires: ' + prereq }).css({ 'font-style': 'italic', 'color': '#787878' });
                                }
                            });
                        };

                        Csw.each(result, function (thisModuleInfo, thisModule) {
                            var input = cswPrivate.table.cell(row, 1).input({
                                name: thisModule,
                                type: Csw.enums.inputTypes.checkbox,
                                checked: Csw.bool(thisModuleInfo.enabled)
                            });
                            input.data('prereq', thisModuleInfo.prereq);
                            input.data('row', row);

                            checkboxes[thisModule] = input;

                            cswPrivate.table.cell(row, 2).append(thisModule);
                            row++;
                        }); //each()

                        resetEnabled();

                        cswPrivate.table.cell(row, 2).button({
                            name: 'savebtn',
                            enabledText: 'Save',
                            disabledText: 'Saving...',
                            onClick: function () {
                                var changes = result;
                                Csw.each(checkboxes, function (thisCheckbox, thisModule) {
                                    changes[thisCheckbox.$.attr('name')] = thisCheckbox.checked();
                                }); // each

                                Csw.ajax.post({
                                    urlMethod: cswPrivate.saveUrlMethod,
                                    data: { Modules: JSON.stringify(changes) },
                                    success: function (result) {
                                        Csw.tryExec(cswPrivate.onModuleChange);
                                        cswPrivate.init();
                                    }, // success
                                    error: function () {
                                        cswPrivate.init();
                                    } // error
                                }); // ajax

                            } // onClick()
                        }); // button()

                    } // success
                }); // ajax()

            } // cswPrivate.init()

            cswPrivate.init();

        }); // register()
} ());
