/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    Csw.nbt.importExcel = Csw.nbt.importExcel ||
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
            (function () {
                var div = cswParent.div({
                    suffix: 'div'
                });
                cswPublic.table = div.table({
                    width: '100%',
                    cellpadding: 2
                });

                /* Title Cell */
                cswPublic.table.cell(1, 1).text(cswPrivate.Title)
                    .propDom('colspan', 2)
                    .addClass('CswWizard_TitleCell');

                cswPublic.table.cell(2, 1).text('Upload a New Data File')
                    .propDom('colspan', 2)

                cswPublic.table.cell(3, 1).span({ text: 'Import Definition:' });
                cswPrivate.selDefName = cswPublic.table.cell(3, 2).select({ name: 'selDefName' });
                
                Csw.ajax.post({
                    urlMethod:'Services/Import/getImportDefs',
                    success: function(result) {
                        Csw.iterate(result, function(defname) {
                            cswPrivate.defNameSel.addOption(defname);
                        });
                    }
                });
                
                cswPublic.table.cell(4, 1).span({ text: 'Overwrite:' });
                cswPrivate.cbOverwrite = cswPublic.table.cell(4, 2).input({
                        name: 'cbOverwrite',
                        type: Csw.enums.inputTypes.checkbox,
                        checked: true
                    });
                
                cswPublic.table.cell(5, 1).span({ text: 'Excel Data File (.xlsx):' });
                cswPublic.table.cell(5, 2).icon({
                    name: 'uploadnewDataBtn',
                    iconType: Csw.enums.iconType.save,
                    hovertext: 'Upload New Data',
                    isButton: true,
                    onClick: function () {
                        $.CswDialog('FileUploadDialog', {
                            urlMethod: 'Services/Import/uploadImportData',
                            params: {
                                defname: '',
                                overwrite: ''
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
