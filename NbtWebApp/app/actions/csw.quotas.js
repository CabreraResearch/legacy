/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.quotas = Csw.actions.quotas ||
        Csw.actions.register('quotas', function (cswParent, options) {
            'use strict';
            var o = {
                urlMethod: 'getQuotas',
                saveUrlMethod: 'saveQuotas',
                name: 'action_quotas',
                onQuotaChange: null // function () { }
            };
            if (options) {
                Csw.extend(o, options);
            }
            
            var div = cswParent.div();
            var table;
            var row;
            var quotaJson;

            function initTable() {
                div.empty();
                table = div.table({
                    suffix: 'tbl',
                    border: 1,
                    cellpadding: 5
                });
                row = 1;

                // Header row
                table.cell(row, 1).span({ cssclass: 'CswThinGridHeaderShow', text: 'Object Class' });
                table.cell(row, 2).span({ cssclass: 'CswThinGridHeaderShow', text: 'Node Types' });
                table.cell(row, 3).span({ cssclass: 'CswThinGridHeaderShow', text: 'Current Usage' });
                table.cell(row, 4).span({ cssclass: 'CswThinGridHeaderShow', text: 'Quota' });
                row += 1;

                // Quota table
                Csw.ajax.post({
                    urlMethod: o.urlMethod,
                    data: {},
                    success: function (result) {
                        quotaJson = result;
                        var canedit = Csw.bool(quotaJson.canedit);

                        Csw.crawlObject(quotaJson.objectclasses, function (childObj) {
                            if (Csw.number(childObj.nodetypecount) > 0) {

                                // one object class row                                
                                makeQuotaRow(row, canedit, 'OC' + childObj.objectclassid, childObj.objectclass, '', childObj.currentusage, childObj.quota);
                                row += 1;

                                // several nodetype rows
                                Csw.crawlObject(childObj.nodetypes, function (childObjNt) {
                                    makeQuotaRow(row, canedit, 'NT' + childObjNt.nodetypeid, '', childObjNt.nodetypename, childObjNt.currentusage, childObjNt.quota);
                                    row += 1;
                                }, false);
                            }
                        }, false); // Csw.crawlObject()

                        if (canedit) {
                            div.button({
                                name: 'Save',
                                enabledText: 'Save',
                                disabledText: 'Saving',
                                onClick: handleSave
                            });
                        }
                    } // success
                }); // ajax()
            }

            // initTable()

            function makeQuotaRow(qRow, canedit, objectclass, nodetype, currentusage, quota) {
                // one object class row                                
                var cell4;
                table.cell(qRow, 1).text(objectclass);
                table.cell(qRow, 2).text(nodetype);
                table.cell(qRow, 3).text(currentusage);

                if (canedit) {
                    cell4 = table.cell(qRow, 4);
                    cell4.input({
                        name: o.name + id + 'quota',
                        type: Csw.enums.inputTypes.text,
                        value: quota,
                        width: '50px'
                    });
                } else {
                    table.cell(qRow, 4).text(quota);
                }
            }

            // makeQuotaRow()

            function handleSave() {
                Csw.crawlObject(quotaJson.objectclasses, function (childObj) {
                    childObj.quota = $('[name="' + o.name + 'OC' + childObj.objectclassid + 'quota"]').val();
                    Csw.crawlObject(childObj.nodetypes, function (childObjNt) {
                        childObjNt.quota = $('[name="' + o.name + 'NT' + childObjNt.nodetypeid + 'quota"]').val();
                    }, false);
                }, false);

                Csw.ajax.post({
                    urlMethod: o.saveUrlMethod,
                    data: { Quotas: JSON.stringify(quotaJson) },
                    success: function () {
                        initTable();
                        Csw.tryExec(o.onQuotaChange);
                    }
                });
            }

            // handleSave()

            initTable();
        }); // methods

} ());
