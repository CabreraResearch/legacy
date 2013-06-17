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
                Csw.iterate(cswPrivate.modules, function (chckBox) {
                    chckBox.disable();
                });

                Csw.ajaxWcf.post({
                    urlMethod: 'Modules/HandleModule',
                    data: module,
                    success: function (response) {
                        cswPrivate.render(response);
                        Csw.tryExec(cswPrivate.onModuleChange);
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
                        canCheck: true,
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
                    success: function(response) {
                        cswPrivate.render(response);
                    }
                });
            }; // cswPrivate.init()

            cswPrivate.init();

        }); // register()
}());
