/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.quotas = Csw.actions.quotas ||
        Csw.actions.register('quotas', function (cswParent, options) {
            'use strict';
            var o = {
                urlMethod: 'getQuotas',
                saveUrlMethod: 'saveQuotas',
                ID: 'action_quotas',
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
                table.cell(row, 1).b({ text: 'Object Class' });
                table.cell(row, 2).b({ text: 'Node Types' });
                table.cell(row, 3).b({ text: 'Current Usage' });
                table.cell(row, 4).b({ text: 'Quota' });
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
                                makeQuotaRow(row, canedit, 'OC_' + childObj.objectclassid, childObj.objectclass, '', childObj.currentusage, childObj.quota);
                                row += 1;

                                // several nodetype rows
                                Csw.crawlObject(childObj.nodetypes, function (childObjNt) {
                                    makeQuotaRow(row, canedit, 'NT_' + childObjNt.nodetypeid, '', childObjNt.nodetypename, childObjNt.currentusage, childObjNt.quota);
                                    row += 1;
                                }, false);
                            }
                        }, false); // Csw.crawlObject()

                        if (canedit) {
                            div.button({
                                ID: o.ID + '_save',
                                enabledText: 'Save',
                                disabledText: 'Saving',
                                onClick: handleSave
                            });
                        }
                    } // success
                }); // ajax()
            }

            // initTable()

            function makeQuotaRow(qRow, canedit, id, objectclass, nodetype, currentusage, quota) {
                // one object class row                                
                var cell4;
                table.cell(qRow, 1).text(objectclass);
                table.cell(qRow, 2).text(nodetype);
                table.cell(qRow, 3).text(currentusage);

                if (canedit) {
                    cell4 = table.cell(qRow, 4);
                    cell4.input({
                        ID: o.ID + '_' + id + '_quota',
                        name: o.ID + '_' + id + '_quota',
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
                    childObj.quota = $('#' + o.ID + '_OC_' + childObj.objectclassid + '_quota').val();
                    Csw.crawlObject(childObj.nodetypes, function (childObjNt) {
                        childObjNt.quota = $('#' + o.ID + '_NT_' + childObjNt.nodetypeid + '_quota').val();
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
