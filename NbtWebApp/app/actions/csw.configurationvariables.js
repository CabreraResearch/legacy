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

       cswPrivate.renderSection = function(ConfigVarsForModule, ModuleName, parentTable, tableRow) {
                var headerRowCell = parentTable.cell(tableRow, 1);
                headerRowCell.text(ModuleName);
                headerRowCell.propDom('colspan', 3);
                headerRowCell.css('background', "#D1DBE6");                    
                tableRow += 1;
                //generate the subtable showing the config vars for this
                //module
                Csw.each(ConfigVarsForModule, function(ConfigVarObject) {

                    parentTable.cell(tableRow, 1).text(ConfigVarObject.variableName)
                                                 .css({
                                                    textAlign:'right',
                                                    verticalAlign: 'middle'
                                                 });
                    var thisControl = parentTable.cell(tableRow, 2)
                                                 .input({
                        size: 35,
                        name: ConfigVarObject.variableName,
                        value: ConfigVarObject.variableValue
                    });
                    parentTable.cell(tableRow, 3).text(ConfigVarObject.description)
                                                 .css({
                                                    textAlign : 'left',
                                                    verticalAlign : 'middle'
                                                 });

                    cswPrivate.configVarControls.push(thisControl);

                    tableRow += 1;
                });

           return tableRow;
       };

       cswPrivate.render = function(response, actionDiv) {
            var parentTable = actionDiv.table({
                suffix: 'tbl',
                margin: 0,
                cellpadding: 5, 
                width: '100%'
            });

            //array of controls, used to get the config values from
            //when applying changes
            cswPrivate.configVarControls = [];

            var tableRow = 1;
            
           //render each section, making sure common is at the top
           //and that system is at the bottom
            var CommonVars = response.ConfigVarsByModule.Common;
            tableRow = cswPrivate.renderSection(CommonVars, "Common", parentTable, tableRow);

            Csw.each(response.ConfigVarsByModule, function(ConfigVarsForModule, ModuleName) {
                if (ModuleName !== "Common" && ModuleName !== "System") {
                    tableRow = cswPrivate.renderSection(ConfigVarsForModule, ModuleName, parentTable, tableRow);
                } 

            });

           if (response.ConfigVarsByModule.System) {
               var systemVars = response.ConfigVarsByModule.System;
               tableRow = cswPrivate.renderSection(systemVars, "System", parentTable, tableRow);
           }

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
               }
            });

            layout.cancel.hide();

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
