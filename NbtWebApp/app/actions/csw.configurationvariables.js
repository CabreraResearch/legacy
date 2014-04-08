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
                margin: 0,
                cellpadding: 5, 
                width: '80%'
            });

            //array of controls, used to get the config values from
            //when applying changes
            cswPrivate.configVarControls = [];

            var tableRow = 1;

            //generate the supertable showing the module name
            Csw.each(response.ConfigVarsByModule, function(ConfigVarsForModule, ModuleName) {

                var headerRowCell = parentTable.cell(tableRow, 1);
                headerRowCell.text(ModuleName);
                headerRowCell.propDom('colspan', 3);
                headerRowCell.css('background', "#D1DBE6");                    
                tableRow += 1;
                //generate the subtable showing the config vars for this
                //module
                Csw.each(ConfigVarsForModule, function(ConfigVarObject) {

                    parentTable.cell(tableRow, 1).text(ConfigVarObject.variableName)
                                                 .css('text-align', 'right');
                    var thisControl = parentTable.cell(tableRow, 2)
                                                 .css('width', 70)
                                                 .input({
                        size: "95%",
                        name: ConfigVarObject.variableName,
                        value: ConfigVarObject.variableValue
                    });
                    parentTable.cell(tableRow, 3).text(ConfigVarObject.description)
                                                 .css('text-align', 'left');

                    cswPrivate.configVarControls.push(thisControl);

                    tableRow += 1;
                });
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
