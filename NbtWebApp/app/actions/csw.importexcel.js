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
                if (!cswPrivate.excelDataFileText && !cswPrivate.uploadButton) {
                    cswPrivate.excelDataFileText = cswPublic.uploadDataTable.cell(3, 1).text('Excel Data File (.xlsx):');
                    cswPrivate.uploadButton = cswPublic.uploadDataTable.cell(3, 2).buttonExt({
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
                                    cswPrivate.selectedJobId = Csw.number(Csw.getPropFromIFrame(response, 'jobid', false), Csw.int32MinVal);
                                    cswPrivate.makeStatusTable();
                                }
                            });
                        }
                    });
                }

                if (visible) {
                    cswPrivate.excelDataFileText.show();
                    cswPrivate.uploadButton.show();
                } else {
                    cswPrivate.excelDataFileText.hide();
                    cswPrivate.uploadButton.hide();
                }
            };//cswPrivate.makeUploadDataProps()

            cswPrivate.makeStartImportProps = function (visible) {
                if (!cswPrivate.startImportBtn) {
                    cswPrivate.startImportBtn = cswPublic.uploadDataTable.cell(3, 3).buttonExt({
                        name: 'startCAFImportBtn',
                        icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.star),
                        enabledText: 'Start',
                        disabledText: 'Start',
                        disableOnClick: false,
                        onClick: function () {
                            Csw.ajaxWcf.post({
                                urlMethod: 'Import/startImport',
                                data: {
                                    ImportDefName: cswPrivate.selDefName.val(),
                                    Overwrite: cswPrivate.cbOverwrite.checked()
                                },
                                success: function (data) {
                                    // show success or show progress
                                }
                            });
                        }
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
                        $.CswDialog('FileUploadDialog', {
                            urlMethod: 'Services/Import/uploadImportDefinition',
                            params: {
                                defname: name
                            },
                            forceIframeTransport: true,
                            dataType: 'iframe',
                            onSuccess: function (response) {
                                cswPrivate.makeUploadDataTable();
                                cswPrivate.txtDefName.val('');
                            }
                        });
                    }
                }
            });
        }; // makeUploadBindingsTable()

        cswPrivate.makeGenerateSqlTable = function () {
            cswPublic.table.cell(3, 4)
                .css({ paddingLeft: '100px' })
                .text('Generate CAF SQL')
                    .css({
                        textAlign: 'center',
                        fontWeight: 'bold'
                    });

            cswPublic.generateSqlTable = cswPublic.table.cell(4, 4)
            .empty()
            .css({ paddingLeft: '100px' })
            .table({
                FirstCellRightAlign: true,
                cellpadding: 2
            });

            cswPublic.generateSqlTable.cell(1, 1).buttonExt({
                name: 'generateSqlBtn',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.docimport),
                enabledText: 'Generate',
                disabledText: 'Generate',
                disableOnClick: false,
                onClick: function () {
                    // Return a .sql file that the User can save on their system
                    var action = 'Services/Import/generateCAFSql';
                    
                    var $form = $('<form method="POST" action="' + action + '"></form>').appendTo($('body'));
                    var form = Csw.literals.factory($form);

                    form.input({
                        name: 'importdefname',
                        value: 'CAF'
                    });

                    form.$.submit();
                    form.remove();
                }
            });
        }; // makeGenerateSqlTable()

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
            cswPrivate.makeGenerateSqlTable();

        })(); // init

        return cswPublic;
    });

})();
