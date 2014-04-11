/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.register('importExcel', function (cswParent, options) {
        'use strict';

        var cswPrivate = {
            name: 'CswImportExcelWizard',
            title: 'Import Data From Excel',
            $parent: null,
            onClose: null,
            selectedJobId: 0
        };
        if (options) Csw.extend(cswPrivate, options);
        var cswPublic = {};


        cswPrivate.loadStatus = function (job) {
            cswPrivate.selectedJobId = job.ImportDataJobId;
            cswPublic.statusTable.cell(2, 1).empty();

            Csw.ajaxWcf.post({
                urlMethod: 'Import/getImportStatus',
                data: {
                    JobId: job.ImportDataJobId
                },
                success: function (data) {

                    var jobTable = cswPublic.statusTable.cell(2, 1)
                        .propDom('colspan', 2)
                        .table({
                            FirstCellRightAlign: true,
                            cellpadding: 2
                        });

                    var itemPercent = Math.round(Csw.number(data.ItemsDone) / Csw.number(data.ItemsTotal) * 100, 2);
                    var rowPercent = Math.round(Csw.number(data.RowsDone) / Csw.number(data.RowsTotal) * 100, 2);
                    var jobrow = 1;

                    // TODO kludge until we have a general way to solve this (case 30497)
                    function DateWCF(datestr) {
                        if (false === Csw.isNullOrEmpty(datestr)) {
                            var m = datestr.match(/\/Date\(([0-9]+)(?:.*)\)\//);
                            if (null !== m) {
                                var d = new Date(parseInt(m[1]));
                                return (d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear() + " " +
                                       d.getHours() + ':' + ('0' + d.getMinutes()).slice(-2) + ':' + ('0' + d.getSeconds()).slice(-2);
                            }
                        } else {
                            return "";
                        }
                    };

                    jobTable.cell(jobrow, 1).text('Uploaded By:');
                    jobTable.cell(jobrow, 2)
                        .css({ fontWeight: 'bold' })
                        .text(job.UserName);
                    jobrow++;


                    jobTable.cell(jobrow, 1).text('Date Started:');
                    jobTable.cell(jobrow, 2)
                        .css({ fontWeight: 'bold' })
                        .text(DateWCF(job.DateStarted));
                    jobrow++;

                    jobTable.cell(jobrow, 1).text('Date Ended:');
                    jobTable.cell(jobrow, 2)
                        .css({ fontWeight: 'bold' })
                        .text(DateWCF(data.DateEnded));
                    jobrow++;

                    jobTable.cell(jobrow, 1).text('Items Finished:');
                    jobTable.cell(jobrow, 2)
                        .css({ fontWeight: 'bold' })
                        .text(data.ItemsDone + " of " + data.ItemsTotal + " (" + itemPercent + "%)");
                    jobrow++;

                    jobTable.cell(jobrow, 1).text('Rows Finished:');
                    jobTable.cell(jobrow, 2)
                        .css({ fontWeight: 'bold' })
                        .text(data.RowsDone + " of " + data.RowsTotal + " (" + rowPercent + "%)");
                    jobrow++;

                    jobTable.cell(jobrow, 1).text('Error Rows:');
                    jobTable.cell(jobrow, 2)
                        .css({ fontWeight: 'bold' })
                        .text(data.RowsError);
                    jobrow++;

                    jobTable.cell(jobrow, 2).buttonExt({
                        name: 'refreshBtn',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.refresh),
                        enabledText: 'Refresh',
                        disabledText: 'Refresh',
                        disableOnClick: false,
                        onClick: function () {
                            cswPrivate.loadStatus(job);
                        }
                    });
                    jobrow++;
                    
                    jobTable.cell(jobrow, 2).buttonExt({
                        name: 'downloadBtn',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docexport),
                        enabledText: 'Download Data File',
                        disabledText: 'Fetching Data...',
                        disableOnClick: false,
                        onClick: function () {
                            var action = 'Services/Import/downloadImportData';

                            var $form = $('<form method="POST" action="' + action + '"></form>').appendTo($('body'));
                            var form = Csw.literals.factory($form);

                            form.input({
                                name: 'filename',
                                value: job.FileName,
                            });

                            form.$.submit();
                            form.remove();
                        }
                    });
                    jobrow++;
                    if (false === Csw.bool(data.Completed)){
                        jobTable.cell(jobrow, 2).buttonExt({
                            name: 'cancelBtn',
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.cancel),
                            enabledText: 'Cancel Import',
                            disabledText: 'Cancelling...',
                            disableOnClick: false,
                            onClick: function() {
                                cswPrivate.cancelJob(job);
                            }
                        });
                    }
                } // success()
            }); // get()
        }; // loadStatus()


        cswPrivate.cancelJob = function(job) {
            if (confirm("Are you sure you want to cancel this import?")) {
                Csw.ajaxWcf.post({
                    urlMethod: 'Import/cancelJob',
                    data: {
                        JobId: job.ImportDataJobId
                    },
                    success: function(data) {
                        cswPrivate.loadStatus(job);
                    }
                });
            }
        }; // cancelJob()

        cswPrivate.makeStatusTable = function () {
            cswPublic.table.cell(3, 1)
                .text('Current Status')
                .css({ textAlign: 'center', fontWeight: 'bold' });

            cswPublic.statusTable = cswPublic.table.cell(4, 1)
                .empty()
                .table({
                    FirstCellRightAlign: true,
                    cellpadding: 2
                });

            cswPublic.statusTable.cell(1, 1).text('Job:');
            cswPrivate.selJob = cswPublic.statusTable.cell(1, 2).select({
                name: 'selJob',
                onChange: function (newval) {
                    Csw.iterate(cswPrivate.jobs, function (job) {
                        if (Csw.number(job.ImportDataJobId) === Csw.number(newval)) {
                            cswPrivate.loadStatus(job);
                        }
                    });
                }
            }); // select()

            Csw.ajaxWcf.get({
                urlMethod: 'Import/getImportJobs',
                success: function (data) {
                    var first = true;
                    cswPrivate.jobs = data;
                    if (data.length > 0) {
                        Csw.iterate(data, function (job) {
                            var isSelected = ((cswPrivate.selectedJobId === 0 && first) ||
                                              (Csw.number(job.ImportDataJobId) === Csw.number(cswPrivate.selectedJobId)));
                            cswPrivate.selJob.addOption({
                                value: job.ImportDataJobId,
                                display: job.FileName,
                                isSelected: isSelected
                            });
                            if (isSelected) {
                                cswPrivate.selJob.val(job.ImportDataJobId);
                                cswPrivate.loadStatus(job);
                            }
                            first = false;
                        });
                    } else {
                        cswPublic.statusTable.cell(1, 2).text('No Jobs').css({ fontStyle: 'italic' });
                    }
                }
            });
        }; // makeStatusTable()


        cswPrivate.makeUploadDataTable = function () {
            cswPublic.table.cell(3, 2)
                .css({ paddingLeft: '100px' })
                    .text('Start a new import')
                    .css({
                        textAlign: 'center',
                        fontWeight: 'bold'
                    });

            //cswPrivate.excelDataFileText = null;
            //cswPrivate.uploadButton = null;
            
            cswPublic.uploadDataTable = cswPublic.table.cell(4, 2)
            .empty()
            .css({ paddingLeft: '100px' })
            .table({
                FirstCellRightAlign: true,
                cellpadding: 2
            });

            cswPublic.uploadDataTable.cell(1, 1).text('Import Definition:');
            cswPrivate.selDefName = cswPublic.uploadDataTable.cell(1, 2).select({
                name: 'selDefName',
                onChange: function () {
                    if (cswPrivate.selDefName.selectedText() === "CAF") {
                        cswPrivate.makeUploadDataProps(false);
                        cswPrivate.makeStartImportProps(true);
                    } else {
                        cswPrivate.makeUploadDataProps(true);
                        cswPrivate.makeStartImportProps(false);
                    }

                    cswPrivate.makeBindingsGrid(cswPrivate.selDefName.val());
                }
            });


            Csw.ajaxWcf.get({
                urlMethod: 'Import/getImportDefs',
                success: function (data) {
                    Csw.iterate(data.split(','), function (defname) {
                        cswPrivate.selDefName.addOption({ value: defname, display: defname });
                    });
                }
            });

            cswPublic.uploadDataTable.cell(2, 1).text('Overwrite:');
            cswPrivate.cbOverwrite = cswPublic.uploadDataTable.cell(2, 2).input({
                name: 'cbOverwrite',
                type: Csw.enums.inputTypes.checkbox,
                checked: true
            });

            cswPrivate.makeUploadDataProps = function (visible) {
                if (!cswPrivate.excelDataFileText) {
                    cswPrivate.excelDataFileText = cswPublic.uploadDataTable.cell(3, 1).text('Excel Data File (.xlsx):');
                }
                if (!cswPrivate.downloadButton) {
                    cswPrivate.downloadButton = cswPublic.uploadDataTable.cell(3, 2).buttonExt({
                        name: 'downloadDataBtn',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docexport),
                        enabledText: 'Download',
                        disabledText: 'Download',
                        disableOnClick: false,
                        onClick: function() {
                            if (cswPrivate.selDefName.val() == "IMCS") {
                                window.open("etc/imcs_import_template.xlsx");
                            } else if (cswPrivate.selDefName.val() == "CISPro") {
                                window.open("etc/cispro_import_template.xlsx");
                            }
                        }
                    });
                }
                    
                if(!cswPrivate.uploadButton) {
                    cswPrivate.uploadButton = cswPublic.uploadDataTable.cell(3, 3).buttonExt({
                        name: 'uploadnewDataBtn',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docimport),
                        enabledText: 'Upload',
                        disabledText: 'Upload',
                        disableOnClick: false,
                        onClick: function () {
                            Csw.dialogs.fileUpload({
                                urlMethod: 'Services/Import/uploadImportData',
                                params: {
                                    defname: cswPrivate.selDefName.val(),
                                    overwrite: cswPrivate.cbOverwrite.checked
                                },
                                forceIframeTransport: true,
                                dataType: 'iframe',
                                onSuccess: function (response) {
                                    cswPrivate.selectedJobId = Csw.number(Csw.getPropFromIFrame(response, 'jobid', false), Csw.int32MinVal);
                                    cswPrivate.makeStatusTable();
                                }
                            }).open();
                        }
                    });
                }
                if (visible) {
                    cswPrivate.excelDataFileText.show();
                    cswPrivate.downloadButton.show();
                    cswPrivate.uploadButton.show();
                } else {
                    cswPrivate.excelDataFileText.hide();
                    cswPrivate.downloadButton.hide();
                    cswPrivate.uploadButton.hide();
                }
            };//cswPrivate.makeUploadDataProps()

            cswPrivate.renderInputDialog = function() {
                cswPrivate.inputDialog = Csw.dialogs.inputDialog({
                                name: 'StartCAFDialog',
                                title: 'CAF Connection Details',
                                message: 'Enter the database connection information for this customer',
                                fields: {
                                    'CAF Database Name': Csw.enums.inputTypes.text,
                                    'CAF Database Password': Csw.enums.inputTypes.text,
                                    'CAF Server': Csw.enums.inputTypes.text,
                                },
                                onOk: function (fields) {
                                    Csw.ajaxWcf.post({
                                        urlMethod: 'Import/startImport',
                                        data: {
                                            CAFSchema: fields['CAF Database Name'].val(),
                                            CAFPassword: fields['CAF Database Password'].val(),
                                            CAFDatabase: fields['CAF Server'].val(),
                                        },
                                        success: function (data) {
                                            // show success or show progress
                                            inputDialog.close();
                                        }
                                    });
                                },
                            });
            };

            cswPrivate.makeStartImportProps = function (visible) {
                if (!cswPrivate.startImportBtn) {
                    cswPrivate.startImportBtn = cswPublic.uploadDataTable.cell(4, 2).buttonExt({
                        name: 'startCAFImportBtn',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.star),
                        enabledText: 'Start',
                        disabledText: 'Start',
                        disableOnClick: false,
                        onClick: function () {

                            // Ask the user if they want to delete all existing nodes from the system
                            Csw.ajaxWcf.post({
                                urlMethod: 'Import/getExistingNodes',
                                success: function (data) {
                                    cswPrivate.confirmDialog = Csw.dialogs.confirmDialog({
                                        name: 'ConfirmDeleteNodes',
                                        title: 'Delete Existing Nodes?',
                                        message: 'The following nodes will be deleted if you accept. This is a permanent and irrevocable action.',
                                        gridData: {
                                            fields: [
                                                { name: 'nodeid', type: 'number' },
                                                { name: 'nodename', type: 'string' },
                                                { name: 'nodetype', type: 'string' }],
                                            columns: [
                                                { header: 'NodeId', dataIndex: 'nodeid', width: 70 },
                                                { header: 'Name', dataIndex: 'nodename', flex: 1 },
                                                { header: 'NodeType', dataIndex: 'nodetype', width: 200 }
                                            ],
                                            items: data.NodesToDelete
                                        },
                                        onYes: function () {
                                            // Delete existing nodes
                                            Csw.ajaxWcf.post({
                                                urlMethod: 'Import/deleteExistingNodes',
                                                success: function (data) {
                                                    cswPrivate.confirmDialog.close();
                                                    // Continue with CAF import process
                                                    cswPrivate.renderInputDialog();
                                                },error: function(data) {
                                                    // what to do on error?
                                                }
                                            });
                                        },
                                        onNo: function () {
                                            cswPrivate.confirmDialog.close();
                                            cswPrivate.renderInputDialog();
                                        }
                                    });
                                }//success
                            });
                            
                        }//onClick
                    });

                }
                if (visible) {
                    cswPrivate.startImportBtn.show();
                } else {
                    cswPrivate.startImportBtn.hide();
                }

            };//cswPrivate.makeStartImportProps()

            if (cswPrivate.selDefName.selectedText() != "CAF") {
                cswPrivate.makeUploadDataProps(true);
            } else {
                cswPrivate.makeStartImportProps(true);
            }

                cswPublic.uploadDataTable.cell(1, 3).buttonExt({
                    name: 'downloadBindingsBtn',
                    enabledText: 'Download this Definition',
                    disabledText: 'Generating File...',
                    disableOnClick: true,
                    onClick: function () {
                        var action = 'Services/Import/downloadImportDefinition';

                        var $form = $('<form method="POST" action="' + action + '"></form>').appendTo($('body'));
                        var form = Csw.literals.factory($form);

                        form.input({
                            name: 'importdefname',
                            value: cswPrivate.selDefName.val(),
                                });

                        form.$.submit();
                        form.remove();
                    }//onClick
                });
        }; // makeUploadTable()

        cswPrivate.makeUploadBindingsTable = function () {
            cswPublic.table.cell(3, 3)
                .css({ paddingLeft: '100px' })
                .text('Upload a New Bindings Definition')
                    .css({
                        textAlign: 'center',
                        fontWeight: 'bold'
                    });

            cswPublic.uploadBindingsTable = cswPublic.table.cell(4, 3)
            .empty()
            .css({ paddingLeft: '100px' })
            .table({
                FirstCellRightAlign: true,
                cellpadding: 2
            });

            cswPublic.uploadBindingsTable.cell(1, 1).text('Import Definition Name:');
            cswPrivate.txtDefName = cswPublic.uploadBindingsTable.cell(1, 2).input({ name: 'txtDefName' });

            cswPublic.uploadBindingsTable.cell(3, 1).text('Excel Bindings File (.xlsx):');
            cswPublic.uploadBindingsTable.cell(3, 2).buttonExt({
                name: 'uploadnewBindingsBtn',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docimport),
                enabledText: 'Upload',
                disabledText: 'Upload',
                disableOnClick: false,
                onClick: function () {
                    var name = cswPrivate.txtDefName.val();
                    if (false === Csw.isNullOrEmpty(name)) {
                        Csw.dialogs.fileUpload({
                            urlMethod: 'Services/Import/uploadImportDefinition',
                            params: {
                                defname: name
                            },
                            forceIframeTransport: true,
                            dataType: 'iframe',
                            onSuccess: function(response) {
                                cswPrivate.makeUploadDataTable();
                                cswPrivate.txtDefName.val('');
                            }
                        }).open();
                    } else {
                        alert('Import Definition Name is required.');
                    }
                }
                });


        }; // makeUploadBindingsTable()

        cswPrivate.makeBindingsGrid = function (importDefName) {
            //the grid does not actually get created until after we have received its bindings from the server
            Csw.ajaxWcf.post({
                urlMethod: 'Import/getBindingsForDefinition',
                data: cswPrivate.selDefName.val(),
                success: function (data) {

                    //maintain a cache of just the updated rows so we do not have to send the whole table to the server
                    cswPrivate.gridModifiedRows = [];
                        
                    //prepare a data structure for holding the ext grid data
                    cswPrivate.gridData = {};
                    cswPrivate.gridData.Order = data.Order || { fields: [], columns: [], data: { items: [] } };
                    cswPrivate.gridData.Bindings = data.Bindings || { fields: [], columns: [], data: { items: [] } };
                    cswPrivate.gridData.Relationships = data.Relationships || { fields: [], columns: [], data: { items: [] } };

                    //because the CswExtJsGrid object is a bit hard coded for scheduledRules, we must massage it a bit to display properly here
                    ["Order", "Bindings", "Relationships"].forEach(function (tableName) {
                        for (var itemNo = 0; itemNo < cswPrivate.gridData[tableName].data.items.length; itemNo++) {
                            cswPrivate.gridData[tableName].data.items[itemNo] = cswPrivate.gridData[tableName].data.items[itemNo].Row;
                        }
                        
                        //hide the pk column name so people can't mess with it, otherwise make the column editable
                        cswPrivate.gridData[tableName].columns.forEach(function (column) {
                            var pkcolumnname = "IMPORTDEF" + (tableName.endsWith('s') ? tableName.substr(0, tableName.length - 1) : tableName) + "ID";
                            if (column.header == pkcolumnname.toUpperCase()) {
                                column.hidden = true;
                            }
                            column.sortable = false; //case 31144: sorting/filtering breaks editable grids for now
                            column.filterable = false;
                            column.editable = true;
                            Object.defineProperty(column, 'editor', {
                                writable: true,
                                configurable: true,
                                enumerable: true,
                                value: {
                                    allowBlank: true
                                }
                            });
                        });
                    });

                    //prepare the tabbed container for the grid sheets
                    cswPrivate.gridContentArea = cswPrivate.gridContentArea || cswPublic.table.cell(6, 2).propDom('colspan', 4).div().css('margin', '20px');
                    cswPrivate.gridContentArea.empty();
                    cswPrivate.gridTabstrip = cswPrivate.gridContentArea.tabStrip({
                        onTabSelect: cswPrivate.updateDisplayedTab,
                    });
                    cswPrivate.gridTabstrip.setSize({ width: 800, height: 600 });

                    cswPrivate.gridTabstrip.setTitle('Import Definition for ' + importDefName);

                    //store the tabs so that we can manipulate them when we update the displayed tab
                    cswPrivate.gridTabs = {};
                    cswPrivate.gridTabs.Order = cswPrivate.gridTabstrip.addTab({ title: 'Order' });
                    cswPrivate.gridTabs.Bindings = cswPrivate.gridTabstrip.addTab({ title: 'Bindings' });
                    cswPrivate.gridTabs.Relationships = cswPrivate.gridTabstrip.addTab({ title: 'Relationships' });
                    
                    //set the active tab so everything works before the user picks one for the first time
                    cswPrivate.currentGridTab = 'Order';
                    cswPrivate.gridTabstrip.setActiveTab(0);
                    cswPrivate.updateDisplayedTab(cswPrivate.currentGridTab);

                    //only show the CAF-specific buttons below the grid if we are displaying CAF
                    cswPublic.table.cell(7, 2).empty();
                    cswPublic.table.cell(7, 3).empty();
                    
                    if (importDefName == "CAF") {
                        cswPublic.table.cell(7, 2).buttonExt({
                            enabledText: 'Save Changes',
                            disabledText: 'Sending Updates...',
                            disableOnClick: true,
                            onClick: function() {

                                //WCF requires dictionaries to be arrays of {Key: XXX, Value: YYY} tuples instead of, y'know... dictionaries. So we need to copy the data over to this format
                                var dataToSend = [];

                                cswPrivate.gridModifiedRows.forEach(function(row, index) {
                                    dataToSend[index] = {};
                                    dataToSend[index].editMode = row.editMode;
                                    dataToSend[index].definitionType = row.definitionType;
                                    dataToSend[index].row = [];
                                    Object.keys(row.row).forEach(function(cellName) {
                                        dataToSend[index].row.push({ Key: cellName, Value: row.row[cellName] });
                                    });
                                });
                                
                                //send the data to the server, then clear our list of pending modifications on success
                                Csw.ajaxWcf.post({
                                    urlMethod: 'Import/updateImportDefinition',
                                    data: dataToSend,
                                    success: function() {
                                        cswPrivate.gridModifiedRows = [];
                                    }
                                });
                            }
                        });

                        cswPublic.table.cell(7, 3).buttonExt({
                            enabledText: 'Add Row',
                            disableOnClick: false,
                            onClick: function () {
                                //add a new row to the data store, then refresh the grid so that it appears in the UI
                                var tabName = cswPrivate.currentGridTab;

                                var rowNum = cswPrivate.gridData[tabName].data.items.push(new Object()) - 1;
                                var newRow = cswPrivate.gridData[tabName].data.items[rowNum];
                                cswPrivate.gridData[tabName].columns.forEach(function(column) {
                                    newRow[column.dataIndex] = "";
                                });
                                cswPrivate.indexModifiedRow("add", tabName, newRow);
                                cswPrivate.updateDisplayedTab(tabName);
                            }
                        });
                    }

                }//success()
            });//ajaxWcf.post
        };//make binding grid


        cswPrivate.indexModifiedRow = function (editMode, sheetName, rowData) {
            //NOTE: Rows are stored by REFERENCE, and the Ext data store and modifiedRows column both reference the SAME row object
            var matchingRow = null;
            
            //figure out if the user has already modified this row, so we can re-use the same record in the cache
            cswPrivate.gridModifiedRows.forEach(function (modifiedRow) {
                if ( rowData == modifiedRow.row ) {
                    matchingRow = modifiedRow;
                }
            });

            //if this row does not exist in our cache of modified rows yet, add it
            if (matchingRow == null) {
                cswPrivate.gridModifiedRows.push({
                    editMode: editMode,
                    definitionType: sheetName,
                    row: rowData
                });
            } else {
                //otherwise, only update the reference to the row data on the off chance its changed
                //TODO: this is probably broken if a user modifies a binding, and then deletes it afterward
                matchingRow.row = rowData;
            }
        };


        cswPrivate.updateDisplayedTab = function(tabName) {
            //clear the old grid out of the tab
            cswPrivate.gridTabs[tabName].csw.empty();
            

            var gridOptions = {
                fields: cswPrivate.gridData[tabName].fields,
                columns: cswPrivate.gridData[tabName].columns,
                data: cswPrivate.gridData[tabName].data,
                showActionColumn: false,
                width: 800 - 2,
                height: 600 - 52,
                usePaging: false,
                selModel: {
                    selType: 'cellmodel'
                },
                canSelectRow: false,
            };


            if (cswPrivate.selDefName.val() == "CAF") {
                //only make the grid editable if the definition is CAF
                gridOptions["plugins"] = [
                    Ext.create('Ext.grid.plugin.CellEditing', {
                        clicksToEdit: 1,
                        listeners: {
                            edit: function (grid, row) {
                                if (cswPrivate.gridData[tabName].data.items[row.rowIdx][row.field] != row.value) {
                                    cswPrivate.gridData[tabName].data.items[row.rowIdx][row.field] = row.value;
                                    cswPrivate.indexModifiedRow("modify", tabName, cswPrivate.gridData[tabName].data.items[row.rowIdx]);
                                }
                            }
                        }
                    })
                ];
                gridOptions["showActionColumn"] = true;
                gridOptions["showView"] = false;
                gridOptions["showPreview"] = false;
                gridOptions["showLock"] = false;
                gridOptions["showEdit"] = false;
                gridOptions["showFavorites"] = false;
                gridOptions["onDelete"] = function (rows, rowdata) {
                    //when deleting a row from the grid, splice it out of the data store then reload the grid so it disapears from the UI
                    cswPrivate.gridData[tabName].data.items.splice(cswPrivate.bindingsGrid.getSelectedRowId(), 1);
                    cswPrivate.updateDisplayedTab(tabName);
                    cswPrivate.indexModifiedRow("delete", tabName, rowdata);
                    
                };
            }//if import def name == CAF
            
            //build the grid with the options supplied above, then set the active tab
            cswPrivate.bindingsGrid = cswPrivate.gridTabs[tabName].csw.grid(gridOptions);
            cswPrivate.currentGridTab = tabName;
        };
        



        // Init
        (function () {
            var div = cswParent.div({
                suffix: 'div'
            });
            cswPublic.table = div.table({
                cellpadding: 2
            });

            cswPublic.table.cell(1, 1)
                .propDom('colspan', 3)
                .addClass('LoginTitle')
                .text(cswPrivate.title);

            cswPublic.table.cell(2, 1).br();

            cswPrivate.makeStatusTable();
            cswPrivate.makeUploadDataTable();
            cswPrivate.makeUploadBindingsTable();

        })(); // init

        return cswPublic;
    });

})();
