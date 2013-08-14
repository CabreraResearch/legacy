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

                cswPublic.uploadTable = cswPublic.table.cell(2, 2).table({
                    FirstCellRightAlign: true,
                    cellpadding: 2
                }).hide();

                /* Title Cell */
                cswPublic.table.cell(1, 1)
                    .propDom('colspan', 2)
                    .addClass('LoginTitle')
                    .text(cswPrivate.title);

                cswPublic.table.cell(2, 1)
                    .propDom('colspan', 2)
                    .a({
                        text: 'Upload a New Data File',
                        onClick: function() {
                            cswPublic.uploadTable.show();
                    }});

                cswPublic.uploadTable.cell(1, 1).span({ text: 'Import Definition:' });
                cswPrivate.selDefName = cswPublic.uploadTable.cell(1, 2).select({ name: 'selDefName' });

                Csw.ajaxWcf.get({
                    urlMethod:'Import/getImportDefs',
                    success: function(data) {
                        Csw.iterate(data.split(','), function(defname) {
                            cswPrivate.selDefName.addOption({ value: defname, display: defname });
                        });
                    }
                });

                cswPublic.uploadTable.cell(2, 1).span({ text: 'Overwrite:' });
                cswPrivate.cbOverwrite = cswPublic.uploadTable.cell(2, 2).input({
                        name: 'cbOverwrite',
                        type: Csw.enums.inputTypes.checkbox,
                        checked: true
                    });
                
                cswPublic.uploadTable.cell(3, 1).span({ text: 'Excel Data File (.xlsx):' });
                cswPublic.uploadTable.cell(3, 2).icon({
                    name: 'uploadnewDataBtn',
                    iconType: Csw.enums.iconType.save,
                    hovertext: 'Upload New Data',
                    isButton: true,
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
