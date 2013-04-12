/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.composites.schedRulesTimeline = Csw.composites.schedRulesTimeline ||
        Csw.composites.register('schedRulesTimeline', function (cswParent, options) {
            /// <summary>
            /// 
            ///</summary>
            'use strict';
            var cswPrivate = {
                optsPopulated: false,
                FilterSchemaTo: '',
                FilterOpTo: '',
                FilterStartTo: '',
                FilterEndTo: ''
            };
            var cswPublic = {
                plot: null
            };

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }

                cswPrivate.mainDiv = cswParent.div();
                cswPrivate.filterTbl = cswPrivate.mainDiv.table();

                cswPrivate.mainDiv.br({ number: 2 });

                cswPrivate.tbl = cswPrivate.mainDiv.table({
                    cellspacing: 15,
                    cellpadding: 15
                });

                cswPrivate.legendCell = cswPrivate.tbl.cell(3, 1);
                cswPrivate.chartCell = cswPrivate.tbl.cell(3, 2).css({ height: '450px', width: '1375px' }).text('Fetching timeline data...');
                cswPrivate.xAxisCell = cswPrivate.tbl.cell(4, 2).text('Time (S)').css('text-align', 'center');

            }());

            cswPrivate.toFlot = function (series) {
                var plotData = [];
                Csw.iterate(series, function (CurSeries) {
                    var LegendName = CurSeries.SchemaName + " " + CurSeries.OpName;
                    var thisSeries = { label: LegendName, data: [] };
                    Csw.iterate(CurSeries.DataPoints, function (Point) {
                        if (false === Point.IsNull) {
                            thisSeries.data.push([Point.Start, Point.End]);
                        } else {
                            thisSeries.data.push(null);
                        }
                    });
                    plotData.push(thisSeries);
                });
                return plotData;
            };

            cswPrivate.makeFilters = function (opts) {
                if (false == cswPrivate.optsPopulated) {

                    var onFilterChange = function () {
                        cswPrivate.FilterOpTo = cswPrivate.filterOperation.val();
                        cswPrivate.FilterSchemaTo = cswPrivate.filterSchema.val();

                        var start = cswPrivate.filterStartDate.val();
                        var end = cswPrivate.filterEndDate.val();

                        cswPrivate.FilterStartTo = start.date + ' ' + start.time;
                        cswPrivate.FilterEndTo = end.date + ' ' + end.time;
                    };

                    cswPrivate.filterTbl.cell(1, 1).setLabelText('Schema: ', false, false);
                    cswPrivate.filterSchema = cswPrivate.filterTbl.cell(1, 2).multiSelect({
                        name: 'schemaFilter',
                        values: opts.Schema,
                        onChange: onFilterChange
                    });

                    cswPrivate.filterTbl.cell(2, 1).setLabelText('Operation: ', false, false);
                    cswPrivate.filterOperation = cswPrivate.filterTbl.cell(2, 2).multiSelect({
                        name: 'opFilter',
                        values: opts.Operations,
                        onChange: onFilterChange
                    });

                    cswPrivate.filterTbl.cell(1, 3).css({ 'width': '35px' }); //for UI prettyness

                    cswPrivate.filterTbl.cell(1, 4).setLabelText('Start Date: ', false, false);
                    cswPrivate.filterStartDate = cswPrivate.filterTbl.cell(1, 5).dateTimePicker({
                        DisplayMode: 'DateTime',
                        onChange: onFilterChange
                    });

                    cswPrivate.filterTbl.cell(2, 4).setLabelText('End Date: ', false, false);
                    cswPrivate.filterEndDate = cswPrivate.filterTbl.cell(2, 5).dateTimePicker({
                        DisplayMode: 'DateTime',
                        onChange: onFilterChange
                    });
                    cswPrivate.applyFilterBtn = cswPrivate.filterTbl.cell(3, 1).buttonExt({
                        enabledText: 'Apply Filters',
                        disabledText: 'Applying...',
                        onClick: function () {
                            cswPrivate.init();
                            cswPrivate.applyFilterBtn.enable();
                        }
                    });

                    cswPrivate.optsPopulated = true;
                }
            };

            cswPrivate.plot = function (data) {
                cswPrivate.chartCell.empty();
                var plotData = cswPrivate.toFlot(data);
                cswPublic.plot = $.plot($('#' + cswPrivate.chartCell.getId()), plotData,
                    {
                        legend: { container: $('#' + cswPrivate.legendCell.getId()) },
                        yaxis: {
                            show: false,
                            autoscaleMargin: .5,
                        },
                        series: {
                            lines: {
                                lineWidth: 10,
                                show: true
                            },
                            bars: {
                                show: false,
                                horizontal: true,
                                fill: false
                            },
                            points: {
                                show: true
                            }
                        },
                        grid: {
                            clickable: false,
                            hoverable: false
                        }
                    });
            };

            cswPrivate.init = function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Scheduler/getTimeline',
                    data: {
                        FilterSchemaTo: cswPrivate.FilterSchemaTo,
                        FilterOpTo: cswPrivate.FilterOpTo,
                        FilterStartTimeTo: cswPrivate.FilterStartTo,
                        FilterEndTimeTo: cswPrivate.FilterEndTo,
                    },
                    success: function (data) {
                        cswPrivate.makeFilters(data.FilterData);
                        cswPrivate.plot(data.Series);
                    }
                });
            };

            (function () {
                cswPrivate.init();
            }());

            return cswPublic;
        });

}());
