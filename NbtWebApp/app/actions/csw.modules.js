/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.modules = Csw.actions.modules ||
        Csw.actions.register('modules', function (cswParent, options) {
            'use strict';
            var cswPrivate = {
                name: 'action_modules',
                modules: {},
                onModuleChange: null // function() {}
            };
            if (options) Csw.extend(cswPrivate, options);

            cswPrivate.update = function (module) {
                module.Enabled = cswPrivate.modules[module.Id].checked();
                
                //until we're done handling a module, prevent the user from clicking more modules (Masotti)
                Csw.iterate(cswPrivate.modules, function(chckBox) {
                    chckBox.disable();
                });

                Csw.ajaxWcf.post({
                    urlMethod: 'Modules/HandleModule',
                    data: module,
                    success: function (response) {
                        Csw.tryExec(cswPrivate.onModuleChange);
                        cswPrivate.render(response);
                    }
                });
            };

            cswPrivate.render = function (response) {
                cswPrivate.modules = {};
                cswPrivate.table.empty();
                
                var row = 1;
                cswPrivate.table.cell(row, 1).css({ 'font-weight': 'bold' }).append('Enabled');
                cswPrivate.table.cell(row, 2).css({ 'font-weight': 'bold' }).append('Module');
                row++;

                Csw.iterate(response.Modules, function (module) {
                    var moduleCheckBox = cswPrivate.table.cell(row, 1).input({
                        name: module.Name,
                        type: Csw.enums.inputTypes.checkbox,
                        checked: module.Enabled,
                        onClick: function () {
                            cswPrivate.update(module);
                        }
                    });

                    if (false == Csw.isNullOrEmpty(module.StatusMsg)) {
                        moduleCheckBox.disable();
                    }

                    cswPrivate.modules[module.Id] = moduleCheckBox;

                    cswPrivate.table.cell(row, 2).text(module.Name);
                    cswPrivate.table.cell(row, 3).span({ text: module.StatusMsg }).css({ 'font-style': 'italic', 'color': '#787878' });
                    row++;
                });
            };

            // constructor
            cswPrivate.init = function () {

                cswParent.$.empty();

                cswPrivate.table = cswParent.table({
                    name: cswPrivate.name,
                    cellpadding: '3px',
                    FirstCellRightAlign: true
                }).css({ 'padding-top': '5px' });

                        var row = 1;
                        cswPrivate.table.cell(row, 1).css({ 'font-weight': 'bold' }).append('Enabled');
                        cswPrivate.table.cell(row, 2).css({ 'font-weight': 'bold' }).append('Module');
                        row++;

                Csw.ajaxWcf.post({
                    urlMethod: 'Modules/Initialize',
                    success: function (response) {
                        Csw.iterate(response.Modules, function (module) {
                            var moduleCheckBox = cswPrivate.table.cell(row, 1).input({
                                name: module.Name,
                                type: Csw.enums.inputTypes.checkbox,
                                checked: module.Enabled,
                                onClick: function () {
                                    //TODO: update
                                }
                            });

                            if (false == Csw.isNullOrEmpty(module.StatusMsg)) {
                                moduleCheckBox.disable();
                            }

                            module.chckBox = moduleCheckBox;
                            cswPrivate.modules[module.Id] = module;

                            cswPrivate.table.cell(row, 2).text(module.Name);
                            cswPrivate.table.cell(row, 3).span({ text: module.StatusMsg }).css({ 'font-style': 'italic', 'color': '#787878' });
                            row++;
                            });
                    }
                });

                //Csw.ajax.post({
                //    urlMethod: cswPrivate.urlMethod,
                //    data: {},
                //    success: function (result) {
                //        var row = 1;
                //        cswPrivate.table.cell(row, 1).css({ 'font-weight': 'bold' }).append('Enabled');
                //        cswPrivate.table.cell(row, 2).css({ 'font-weight': 'bold' }).append('Module');
                //        row++;
                //
                //        var checkboxes = {};
                //
                //        var resetEnabled = function () {
                //            Csw.each(checkboxes, function (thisCheckbox, thisModule) {
                //                var prereq = thisCheckbox.data('prereq');
                //                if (prereq !== '' && false === checkboxes[prereq].checked()) {
                //                    thisCheckbox.disable();
                //                    thisCheckbox.checked(false);
                //                    var row = thisCheckbox.data('row');
                //                    cswPrivate.table.cell(row, 3).span({ text: 'Requires: ' + prereq }).css({ 'font-style': 'italic', 'color': '#787878' });
                //                }
                //            });
                //        };
                //
                //        Csw.each(result, function (thisModuleInfo, thisModule) {
                //            var input = cswPrivate.table.cell(row, 1).input({
                //                name: thisModule,
                //                type: Csw.enums.inputTypes.checkbox,
                //                checked: Csw.bool(thisModuleInfo.enabled)
                //            });
                //            input.data('prereq', thisModuleInfo.prereq);
                //            input.data('row', row);
                //
                //            checkboxes[thisModule] = input;
                //
                //            cswPrivate.table.cell(row, 2).append(thisModule);
                //            row++;
                //        }); //each()
                //
                //        resetEnabled();
                //
                //        cswPrivate.table.cell(row, 2).button({
                //            name: 'savebtn',
                //            enabledText: 'Save',
                //            disabledText: 'Saving...',
                //            onClick: function () {
                //                var changes = result;
                //                Csw.each(checkboxes, function (thisCheckbox, thisModule) {
                //                    changes[thisCheckbox.$.attr('name')] = thisCheckbox.checked();
                //                }); // each
                //
                //                Csw.ajax.post({
                //                    urlMethod: cswPrivate.saveUrlMethod,
                //                    data: { Modules: JSON.stringify(changes) },
                //                    success: function (result) {
                //                        Csw.tryExec(cswPrivate.onModuleChange);
                //                        cswPrivate.init();
                //                    }, // success
                //                    error: function () {
                //                        cswPrivate.init();
                //                    } // error
                //                }); // ajax
                //
                //            } // onClick()
                //        }); // button()
                //
                //    } // success
                //}); // ajax()

            }; // cswPrivate.init()

            cswPrivate.init();

        }); // register()
} ());
