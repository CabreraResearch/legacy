/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    Csw.actions.importExcel = Csw.actions.importExcel ||
        Csw.nbt.register('importExcel', function (cswParent, options) {
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

                Csw.ajaxWcf.post({
                    urlMethod: 'Import/getImportStatus',
                    data: {
                        JobId: job.ImportDataJobId
                    },
                    success: function (data) {

                        var jobTable = cswPublic.statusTable.cell(2, 1)
                            .empty()
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
                            .text(DateWCF(job.DateEnded));
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
                        
                    } // success()
                }); // get()
            }; // loadStatus()


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
                                if(isSelected) {
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
                    .text('Upload a New Data File')
                    .css({ textAlign: 'center', 
                           fontWeight: 'bold' });

                cswPublic.uploadTable = cswPublic.table.cell(4, 2)
                    .empty()
                    .css({ paddingLeft: '100px' })
                    .table({
                        FirstCellRightAlign: true,
                        cellpadding: 2
                    });

                cswPublic.uploadTable.cell(1, 1).text('Import Definition:');
                cswPrivate.selDefName = cswPublic.uploadTable.cell(1, 2).select({ name: 'selDefName' });

                Csw.ajaxWcf.get({
                    urlMethod: 'Import/getImportDefs',
                    success: function (data) {
                        Csw.iterate(data.split(','), function (defname) {
                            cswPrivate.selDefName.addOption({ value: defname, display: defname });
                        });
                    }
                });

                cswPublic.uploadTable.cell(2, 1).text('Overwrite:');
                cswPrivate.cbOverwrite = cswPublic.uploadTable.cell(2, 2).input({
                    name: 'cbOverwrite',
                    type: Csw.enums.inputTypes.checkbox,
                    checked: true
                });

                cswPublic.uploadTable.cell(3, 1).text('Excel Data File (.xlsx):');
                cswPublic.uploadTable.cell(3, 2).buttonExt({
                    name: 'uploadnewDataBtn',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docimport),
                    enabledText: 'Upload',
                    disabledText: 'Upload',
                    disableOnClick: false,
                    onClick: function () {
                        $.CswDialog('FileUploadDialog', {
                            urlMethod: 'Services/Import/uploadImportData',
                            params: {
                                defname: cswPrivate.selDefName.val(),
                                overwrite: cswPrivate.cbOverwrite.checked
                            },
                            forceIframeTransport: true,
                            dataType: 'iframe',
                            onSuccess: function (response) {
                                cswPrivate.selectedJobId = response.jobid;
                                cswPrivate.makeStatusTable();
                            }
                        });
                    }
                });
            }; // makeUploadTable()

            
            cswPrivate.makeUploadBindingsTable = function () {
                cswPublic.table.cell(3, 3)
                    .css({ paddingLeft: '100px' })
                    .text('Upload a New Bindings Definition')
                    .css({ textAlign: 'center', 
                           fontWeight: 'bold' });

                cswPublic.uploadTable = cswPublic.table.cell(4, 3)
                    .empty()
                    .css({ paddingLeft: '100px' })
                    .table({
                        FirstCellRightAlign: true,
                        cellpadding: 2
                    });

                cswPublic.uploadTable.cell(1, 1).text('Import Definition Name:');
                cswPrivate.txtDefName = cswPublic.uploadTable.cell(1, 2).input({ name: 'txtDefName' });

                cswPublic.uploadTable.cell(3, 1).text('Excel Bindings File (.xlsx):');
                cswPublic.uploadTable.cell(3, 2).buttonExt({
                    name: 'uploadnewBindingsBtn',
                    icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docimport),
                    enabledText: 'Upload',
                    disabledText: 'Upload',
                    disableOnClick: false,
                    onClick: function () {
                        var name = cswPrivate.txtDefName.val();
                        if (false === Csw.isNullOrEmpty(name)) {
                            $.CswDialog('FileUploadDialog', {
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
                            });
                        }
                    }
                });
            }; // makeUploadBindingsTable()

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
