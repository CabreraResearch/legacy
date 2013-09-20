/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.register('barcode',
        function (nodeProperty) {
            'use strict';

            var eventName = 'onChangeBarcode_' + nodeProperty.propid;
            //The render function to be executed as a callback
            var render = function () {
                'use strict';

                var cswPrivate = Csw.object();

                cswPrivate.value = nodeProperty.propData.values.barcode;
                //cswPrivate.value = Csw.string(nodeProperty.propData.values.barcode).trim();

                var table = nodeProperty.propDiv.table({
                    name: 'tbl',
                    TableCssClass: 'cswInline'
                });

                cswPrivate.cell1 = table.cell(1, 1);

                nodeProperty.onPropChangeBroadcast(function (barcode) {
                    if (barcode !== cswPrivate.value) {
                        cswPrivate.value = barcode;

                        if (cswPrivate.input) {
                            cswPrivate.input.val(barcode);
                        }
                        if (span) {
                            span.remove();
                            span = cswPrivate.cell1.span({ text: cswPrivate.value });
                        }
                    }
                });

                if (nodeProperty.isReadOnly()) {
                    var span = cswPrivate.cell1.span({ text: cswPrivate.value });
                } else {

                    cswPrivate.input = cswPrivate.cell1.input({
                        name: nodeProperty.name,
                        type: Csw.enums.inputTypes.text,
                        cssclass: 'textinput',
                        onChange: function (barcode) {
                            cswPrivate.value = barcode;
                            nodeProperty.propData.values.barcode = barcode;
                            nodeProperty.broadcastPropChange(barcode);
                        },
                        value: cswPrivate.value
                    });
                    cswPrivate.input.required(nodeProperty.isRequired());
                }
                if (false === nodeProperty.isMulti() && false === nodeProperty.isDisabled()) {
                    table.cell(1, 2).div({ name: 'parent' })
                        .buttonExt({
                            name: 'print',
                            enabledText: 'Print',
                            size: 'small',
                            tooltip: { title: 'Print Barcode Label' },
                            icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.barcode),
                            onClick: function () {
                                $.CswDialog('PrintLabelDialog', {
                                    nodes: [{
                                        nodeid: nodeProperty.tabState.nodeid,
                                        nodename: nodeProperty.tabState.nodename || cswPrivate.value
                                    }],
                                    nodetypeid: Csw.number(nodeProperty.tabState.nodetypeid, 0)
                                });
                            }
                        });
                }
            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender(function() {

            return true;

        });
}());
