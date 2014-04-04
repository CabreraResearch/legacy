/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {


   Csw.actions.register('configurationvariables', function (cswParent, options) {
        'use strict';
        var cswPrivate = {
            name: 'action_config_vars',
            modules: {},
            onConfigVarChange: null // function() {}
        };
        if (options) Csw.extend(cswPrivate, options);

        cswPrivate.update = function (module) {
            Csw.ajaxWcf.post({
                urlMethod: 'Modules/HandleModule',
                data: module,
                success: function (response) {
                    cswPrivate.render(response);
                    Csw.tryExec(cswPrivate.onConfigVarChange);
                }
            });
        };

        cswPrivate.render = function(response) {
            // this action is laid out using 3 tables embedded in each other
            // the outer table contains each section (e.g: Common)
            // the middle table contains each section title + section table
            // the inner table lists out each config var in that section
            var parentTable = cswPrivate.div.table({
                suffix: 'tbl',
                width: '90%'
            });

            var superTableRow = 1;

            //generate the supertable showing the module name
            Csw.each(response.ConfigVarsByModule, function(ConfigVarsForModule, ModuleName) {

                var cellDivTable = parentTable.cell(superTableRow,1).table();
                cellDivTable.cell(1, 1).text(ModuleName);

                var cellTable = cellDivTable.cell(2,1).table({
                    suffix: 'tbl',
                    border: 1,
                    margin: 0,
                    cellpadding: 5,
                    name: 'ConfigVarChildTable'
                });

                var subTableRow = 1;

                //generate the subtable showing the config vars for this
                //module
                Csw.each(ConfigVarsForModule, function(ConfigVarObject) {

                    cellTable.cell(subTableRow, 1).text(ConfigVarObject.variableName);
                    cellTable.cell(subTableRow, 2).input({
                       size : 30,
                       name: ConfigVarObject.variableName + 'field',
                       value: ConfigVarObject.variableValue
                    });
                    cellTable.cell(subTableRow, 3).text(ConfigVarObject.description);
                    subTableRow += 1;
                });
                superTableRow += 1;
            });

            //close Button
            
        };

        // constructor
        cswPrivate.init = function () {
            var layout = Csw.layouts.action(cswParent, {
               title: 'Configuration Variables',
               finishText: 'Apply',
               onFinish: function() {
                   console.log("finish");
               }
            });

//            cswParent.$.empty();

            //cswParent.div = layout.actionDiv;

//            cswPrivate.div = cswParent.div();
            cswPrivate.div = layout.actionDiv;

            return Csw.ajaxWcf.post({
                urlMethod: 'ConfigurationVariables/Initialize',
                success: function (response) {
                    cswPrivate.render(response);
                }
            });
        }; // cswPrivate.init()

        return cswPrivate.init();
   }); // register()
}());
