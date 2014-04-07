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

        cswPrivate.update = function (configVarControls) {

            //make array of objects whose key: name of config var
            //and value = value of config var, then send to
            //server
            var responseObject = {
              Data : []                  
            };

            Csw.each(cswPrivate.configVarControls, function(thisControlReference) {
                var thisControl = thisControlReference[0];
                var thisConfigVarName = thisControl.name;
                var thisControlValue = thisControl.value;
                //disable each control 

                var thisResponseObject = {
                    variableName: thisConfigVarName,
                    variableValue: thisControlValue
                };
                responseObject.Data.push(thisResponseObject);
            });

            Csw.ajaxWcf.post({
                urlMethod: 'ConfigurationVariables/Update',
                data: responseObject,
                success: function (response) {
                    cswPrivate.init();
                }
            });
        };

        cswPrivate.render = function(response, actionDiv) {
            // this action is laid out using 3 tables embedded in each other
            // the outer table contains each section (e.g: Common)
            // the middle table contains each section title + section table
            // the inner table lists out each config var in that section
            var parentTable = actionDiv.table({
                suffix: 'tbl',
                width: '90%'
            });

            //array of controls, used to get the config values from
            //when applying changes
            cswPrivate.configVarControls = [];

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
                    var thisControl = cellTable.cell(subTableRow, 2).input({
                       size : 30,
                       name: ConfigVarObject.variableName,
                       value: ConfigVarObject.variableValue
                    });
                    cellTable.cell(subTableRow, 3).text(ConfigVarObject.description);

                    cswPrivate.configVarControls.push(thisControl);

                    subTableRow += 1;
                });
                superTableRow += 1;
            });

        };

        // constructor
        cswPrivate.init = function () {

            //clear the parent div
            cswParent.empty();
            
            var layout = Csw.layouts.action(cswParent, {
               title: 'Configuration Variables',
               finishText: 'Apply',
               onFinish: function() {
                   cswPrivate.update(cswPrivate.configVarControls);
               },
               onCancel: function() {
                   cswPrivate.onCancel();
               }
            });

            return Csw.ajaxWcf.post({
                urlMethod: 'ConfigurationVariables/Initialize',
                success: function (response) {
                    cswPrivate.render(response, layout.actionDiv);
                }
            });
        }; // cswPrivate.init()

        return cswPrivate.init();
   }); // register()
}());
