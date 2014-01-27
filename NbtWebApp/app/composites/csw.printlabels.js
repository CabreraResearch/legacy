/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {

    ///<Summary>
    ///Renders the content for printing labels 
    ///</summary>
    Csw.composites.register('printLabels', function (cswParent, options, targetTypeId, targetId) {
        'use strict';
        var cswPrivate = {
            name: '',
            showButton: true,
            onBtnClick: function () { },
            barcodes: [],
            nodeIds: [],
            nodes: [],
            handlePrint: null, //for custom handling of print jobs (see Receiving wizard)
            selectedLabel: null,
        };
        var cswPublic = {};

        (function () {
            if (options) {
                Csw.extend(cswPrivate, options);
            }

            var labelText = 'Print labels for the following: ';
            if (cswPrivate.nodes.length === 0) {
                labelText = '';
            }

            var labelsDiv = cswParent.div({ text: labelText });
            labelsDiv.br();

            Csw.iterate(cswPrivate.nodes, function (nodeObj) {
                cswPrivate.nodeIds.push(nodeObj.nodeid);
                labelsDiv.span({ text: nodeObj.nodename }).css({ 'padding-left': '10px' }).br();
            });

            var defaultHandlePrint = function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Labels/newPrintJob',
                    data: cswPublic.getPrintData(),
                    success: function (data) {
                        labelsDiv.empty();
                        labelsDiv.nodeLink({ text: 'Label(s) will be printed in Job: ' + data.JobLink });
                    } // success
                }); // ajax
            }; // handlePrint()
            if (false == Csw.isFunction(cswPrivate.handlePrint)) {
                cswPrivate.handlePrint = defaultHandlePrint;
            }

            labelsDiv.br();
            labelsDiv.div({ text: 'Select a label to Print:' });
            var labelSelDiv = labelsDiv.div();
            var labelSel = labelSelDiv.select({
                name: cswPrivate.name + '_labelsel'
            });
            var labelSelError = labelSelDiv.div();

            Csw.ajaxWcf.post({
                urlMethod: 'Labels/list',
                data: {
                    TargetTypeId: Csw.number(targetTypeId, 0),
                    TargetId: targetId
                },
                success: function (data) {
                    if (data.Labels && data.Labels.length > 0) {

                        //if a label to display by default was passed in, use that. Otherwise take the result of the web request
                        var labelSelectionTarget = Csw.isNullOrEmpty(cswPrivate.selectedLabel) ? data.SelectedLabelId : cswPrivate.selectedLabel;

                        //iterate through the list of labels. If the label's nodeid matches our target, set it to be selected
                        for (var i = 0; i < data.Labels.length; i += 1) {
                            var label = data.Labels[i];
                            var isSelected = Csw.bool(label.Id === labelSelectionTarget);
                            labelSel.option({ value: label.Id, display: label.Name, isSelected: isSelected });
                        }
                    } else {
                        labelSelError.span({ cssclass: 'warning', text: 'No labels have been assigned!' });
                    }
                } // success
            }); // ajax

            labelSelDiv.br();
            labelSelDiv.div({ text: 'Select a Printer:' });

            var printerSel = labelSelDiv.nodeSelect({
                name: cswPrivate.name + '_printersel',
                objectClassName: 'PrinterClass',
                allowAdd: false,
                isRequired: true,
                showSelectOnLoad: true,
                isMulti: false,
                selectedNodeId: Csw.currentUser.defaults().DefaultPrinterId,
                onSuccess: function () {
                    if (printerSel.optionsCount() === 0) {
                        printerSel.hide();
                        printBtn.hide();
                        prinerSelErr.span({ cssclass: 'warning', text: 'No printers have been registered!' });
                    }
                }
            });
            var prinerSelErr = labelSelDiv.div();

            var printBtn = labelsDiv.button({
                name: 'print_label_print',
                enabledText: 'Print',
                disableOnClick: false,
                onClick: function () {
                    cswPrivate.onBtnClick();
                    cswPrivate.handlePrint(); //getEplContext
                }
            });
            if (false == cswPrivate.showButton) {
                printBtn.hide();
            }

            cswPublic.getPrintData = function () {
                var ret = {
                    LabelId: labelSel.val(),
                    PrinterId: printerSel.selectedNodeId(),
                    TargetIds: cswPrivate.nodeIds.join(',')
                };

                return ret;
            };

        }());
        return cswPublic;
    });

})(jQuery);
