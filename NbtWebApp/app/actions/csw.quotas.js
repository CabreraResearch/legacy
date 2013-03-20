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
                table.cell(row, 5).span({ cssclass: 'CswThinGridHeaderShow', text: 'Exclude In Meter' });
                row += 1;

                // Quota table
                Csw.ajax.post({
                    urlMethod: o.urlMethod,
                    data: {},
                    success: function (result) {
                        quotaJson = result;
                        var canedit = Csw.bool(quotaJson.canedit);

                        Csw.eachRecursive(quotaJson.objectclasses, function (childObj) {
                            if (Csw.number(childObj.nodetypecount) > 0) {

                                // one object class row                                
                                makeQuotaRow(row, canedit, 'OC' + childObj.objectclassid, childObj.objectclass, '', childObj.currentusage, childObj.quota, childObj.excludeinquotabar);
                                row += 1;

                                // several nodetype rows
                                Csw.eachRecursive(childObj.nodetypes, function (childObjNt) {
                                    makeQuotaRow(row, canedit, 'NT' + childObjNt.nodetypeid, '', childObjNt.nodetypename, childObjNt.currentusage, childObjNt.quota, childObjNt.excludeinquotabar);
                                    row += 1;
                                }, false);
                            }
                        }, false); // Csw.eachRecursive()

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

            function makeQuotaRow(qRow, canedit, rowid, objectclass, nodetype, currentusage, quota, excludeinquotabar) {
                // one object class row                                
                var cell4;
                var cell5;
                table.cell(qRow, 1).text(objectclass);
                table.cell(qRow, 2).text(nodetype);
                table.cell(qRow, 3).text(currentusage);

                if (canedit) {
                    cell4 = table.cell(qRow, 4);
                    cell4.input({
                        name: o.name + rowid + 'quota',
                        type: Csw.enums.inputTypes.text,
                        value: quota,
                        size: '15px'
                    });

                    cell5 = table.cell(qRow, 5);
                    cell5.input({
                        name: o.name + rowid + 'excludeinquotabar',
                        type: Csw.enums.inputTypes.checkbox,
                        checked: excludeinquotabar
                    });
                } else {
                    table.cell(qRow, 4).text(quota);
                    table.cell(qRow, 5).text(excludeinquotabar);
                }
            }

            // makeQuotaRow()

            function handleSave() {
                Csw.eachRecursive(quotaJson.objectclasses, function (childObj) {
                    childObj.quota = $('[name="' + o.name + 'OC' + childObj.objectclassid + 'quota"]').val();
                    childObj.excludeinquotabar = $('[name="' + o.name + 'OC' + childObj.objectclassid + 'excludeinquotabar"]').is(':checked');
                    Csw.eachRecursive(childObj.nodetypes, function (childObjNt) {
                        childObjNt.quota = $('[name="' + o.name + 'NT' + childObjNt.nodetypeid + 'quota"]').val();
                        childObjNt.excludeinquotabar = $('[name="' + o.name + 'NT' + childObjNt.nodetypeid + 'excludeinquotabar"]').is(':checked');
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
