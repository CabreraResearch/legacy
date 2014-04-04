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
            cswPrivate.modules = {};
            cswPrivate.div.empty().text('Configuration Variables');
            cswPrivate.div.css({
                width: '500px'
            });

            var parentTable = cswParent.table({
                suffix: 'tbl',
                name: 'ConfigVarParentTable'
            });

            var superTableRow = 1;

            //generate the supertable showing the module name
            Csw.each(response.ConfigVarsByModule, function(ConfigVarsForModule, ModuleName) {

                var cellDiv = cswParent.div();
                var cellTable = cswParent.table({
                    suffix: 'tbl',
                    border: 1,
                    margin: 0,
                    cellpadding: 5,
                    name: 'ConfigVarChildTable'
                });

                cellDiv.text(ModuleName).css({
                                        
                });                 

                cellDiv.div(cellTable);

                var subTableRow = 1;

                //generate the subtable showing the config vars for this
                //module
                Csw.each(ConfigVarsForModule, function(ConfigVarObject) {

                    cellTable.cell(subTableRow, 1).text(ConfigVarObject.variableName);
                    cellTable.cell(subTableRow, 2).text(ConfigVarObject.variableName);
                    cellTable.cell(subTableRow, 3).text(ConfigVarObject.description);
                    subTableRow += 1;
                });


                parentTable.cell(superTableRow, 1);

                superTableRow += 1;

            });
        };

        // constructor
        cswPrivate.init = function () {

            cswParent.$.empty();

            cswPrivate.div = cswParent.div();

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
