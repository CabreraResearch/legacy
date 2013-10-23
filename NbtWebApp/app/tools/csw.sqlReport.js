/// <reference path="~/app/CswApp-vsdoc.js" />

/* globals Csw:false, $:false */
(function () {
    'use strict';

    function init() {

        $(document).ajaxStop(function stopSpinner() {
            $('#spinner').hide();
        });

        var qs = Csw.queryString();

        Csw.ajaxWcf.post({
            urlMethod: 'Reports/getReportInfo',
            async: false,
            data: {
                nodeId: qs.reportid,
                sourceId: qs.sourceid
            },
            success: function (data) {
                var params = [];
                if (data.doesSupportCrystal) {
                    $('#btnCrystal').css('display', '');
                }

                var parent = Csw.domNode({ ID: 'paramsDiv' });
                var table = parent.table().css('margin', '5px');
                var i = 0;
                Csw.iterate(data.reportParams, function (param) {
                    (function () {
                        var paramName = param.name;
                        params[paramName] = param.value;

                        //If the param is internally managed, don't expose it.
                        if (data.controlledParams.indexOf(paramName) === -1) {
                            var spanCell = table.cell(i + 1, 1).span({ text: paramName }).css('margin', '5px');

                            var OnChange = function (text) {
                                params[paramName] = text;
                            };

                            var inputCell = table.cell(i + 1, 2).input({
                                value: param.value,
                                onChange: OnChange,
                                isRequired: true
                            }).css('margin', '5px');
                            i++;
                        }
                    })();
                });

                if (false == Csw.isNullOrEmpty(qs.reportid)) {
                    $('#btnGrid').click(function () {
                        runReport(qs.reportid, 'grid', params);
                    });
                    $('#btnCsv').click(function () { runReport(qs.reportid, 'csv', params); });
                    $('#btnCrystal').click(function () { runReport(qs.reportid, 'crystal', params) });
                }
            }
        });

        var postForm = function (reportid, params, action) {

            var $form = $('<form method="POST" action="' + action + '"></form>').appendTo($('body'));
            var form = Csw.literals.factory($form);

            for (var param in params) {
                form.input({
                    name: param,
                    value: params[param]
                });
            }

            form.input({
                name: 'reportid',
                value: reportid
            });
            form.$.submit();
            form.remove();
        };

        var runReport = function (reportid, rformat, params) {

            $(document).ajaxSend(function startSpinner() {
                $('#spinner').show();
            });

            var reportParams = [];
            for (var item in params) {
                reportParams.push({ name: item, value: params[item] });
            }

            if ('csv' == rformat.toLowerCase()) {
                postForm(reportid, params, 'Services/Reports/reportCSV');
            } else if ('crystal' == rformat.toLowerCase()) {
                //postForm(reportid, params, 'report.aspx');
                Csw.ajaxWcf.post({
                    urlMethod: 'Reports/reportCrystal',
                    data: {
                        reportFormat: rformat,
                        nodeId: reportid,
                        reportParams: reportParams
                    },
                    success: function (data) {
                        var parent = Csw.domNode({ ID: 'rptDiv' });
                        parent.empty();
                        if (data.hasResults) {
                            Csw.window.location(data.reportUrl);
                        } else {
                            parent.span('There are no rows to display.');
                        }
                    }
                });

            } else {

                Csw.ajaxWcf.post({
                    urlMethod: 'Reports/report',
                    data: {
                        reportFormat: rformat,
                        nodeId: reportid,
                        reportParams: reportParams
                    },
                    success: function (data) {
                        var parent = Csw.domNode({ ID: 'rptDiv' });
                        parent.empty();

                        if (true === data.Truncated) {
                            parent.br();
                            parent.div({ text: 'Results were truncated at ' + data.RowCount + ' rows.' });
                        }

                        var gridDiv = parent.div();

                        var gridData = JSON.parse(data.gridJSON);
                        gridData.grid.showActionColumn = false;
                        gridData.grid.storeid = reportid;
                        gridData.grid.usePaging = false;
                        gridDiv.grid(gridData.grid).css('margin', '20px');
                    }
                });
            }
        };
    }

    Csw.reports.register('init', init);

} ());