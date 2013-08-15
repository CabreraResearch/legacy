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
            };
            if (options) Csw.extend(cswPrivate, options);
            var cswPublic = {};


            // Init
            (function() {
                var div = cswParent.div({
                    suffix: 'div'
                });
                cswPublic.table = div.table({
                    cellpadding: 2
                });

                cswPublic.table.cell(1, 1)
                    .propDom('colspan', 2)
                    .addClass('LoginTitle')
                    .text(cswPrivate.title);

                cswPublic.table.cell(2, 1).br();

                // 'Current Stats' table
                cswPublic.table.cell(3, 1)
                    .text('Current Status')
                    .css({ textAlign: 'center', fontWeight: 'bold' });

                cswPublic.statusTable = cswPublic.table.cell(4, 1).table({
                    FirstCellRightAlign: true,
                    cellpadding: 2
                });
                
                
                Csw.ajaxWcf.get({
                    urlMethod:'Import/getImportStatus',
                    success: function(data) {
                        cswPublic.statusTable.cell(1, 1).text('Pending Rows:');
                        cswPublic.statusTable.cell(1, 2)
                            .css({ fontWeight: 'bold' })
                            .text(data.PendingCount);

                        cswPublic.statusTable.cell(2, 1).text('Error Rows:');
                        cswPublic.statusTable.cell(2, 2)
                            .css({ fontWeight: 'bold' })
                            .text(data.ErrorCount);
                    }
                });


                
                // 'Upload New' table
                cswPublic.table.cell(3, 2)
                    .text('Upload a New Data File')
                    .css({ textAlign: 'center', fontWeight: 'bold' });

                cswPublic.uploadTable = cswPublic.table.cell(4, 2).table({
                    FirstCellRightAlign: true,
                    cellpadding: 2
                });

                cswPublic.uploadTable.cell(1, 1).text('Import Definition:');
                cswPrivate.selDefName = cswPublic.uploadTable.cell(1, 2).select({ name: 'selDefName' });

                Csw.ajaxWcf.get({
                    urlMethod:'Import/getImportDefs',
                    success: function(data) {
                        Csw.iterate(data.split(','), function(defname) {
                            cswPrivate.selDefName.addOption({ value: defname, display: defname });
                        });
                    }
                });

                cswPublic.uploadTable.cell(2, 1).text( 'Overwrite:' );
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
                            onSuccess: function (response) {}
                        });
                    }
                });

            })(); // init

            return cswPublic;
        });

})();
