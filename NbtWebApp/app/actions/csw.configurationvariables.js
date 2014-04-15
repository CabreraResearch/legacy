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

        cswPrivate.update = function (responseData) {

            //convert the responseData dictionary into an array
            var responseDataArray = [];
            $.each(responseData, function(key) {
                responseDataArray.push({
                   variableName: key,
                   variableValue: responseData[key]
                });
            });

            var responseDataObject = {
                Data : responseDataArray
            }

            //clear responseData after posting it to prevent the 
            //data from being resent 
            Csw.ajaxWcf.post({
                urlMethod: 'ConfigurationVariables/Update',
                data: responseDataObject,
                success: function (response) {
                    cswPrivate.init();
                    cswPrivate.responseData = [];
                },
                error: function(response) {
                    cswPrivate.responseData = [];
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
                        value: ConfigVarObject.variableValue,
                        onChange: function() {
                            var thisControlName = thisControl[0].name;
                            cswPrivate.responseData[thisControlName] = thisControl.val();
                        }
                    });
                    parentTable.cell(tableRow, 3).text(ConfigVarObject.description)
                                                 .css({
                                                    textAlign : 'left',
                                                    verticalAlign : 'middle'
                                                 });

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

            //array of objects, where each object represents a 
            //config var that has been modified
            cswPrivate.responseData = {};

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
                   cswPrivate.update(cswPrivate.responseData);
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
