/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.barcode = Csw.properties.register('barcode',
            function (nodeProperty) {
                'use strict';

                var eventName = 'onChangeBarcode_' + nodeProperty.propid;
                //The render function to be executed as a callback
                var render = function () {
                    'use strict';

                    var cswPrivate = Csw.object();

                    cswPrivate.propVals = nodeProperty.propData.values;
                    cswPrivate.value = cswPrivate.propVals.barcode;
                    //cswPrivate.value = Csw.string(cswPrivate.propVals.barcode).trim();

                    var table = nodeProperty.propDiv.table({
                        name: 'tbl',
                        TableCssClass: 'cswInline'
                    });

                    cswPrivate.cell1 = table.cell(1, 1);

                    if (nodeProperty.isReadOnly()) {
                        cswPrivate.cell1.text(cswPrivate.value);
                    } else {

                        Csw.properties.subscribe(eventName, function(eventObj, barcode) {
                            if(barcode !== cswPrivate.value) {
                                cswPrivate.input.val(barcode);
                            }
                        });

                        cswPrivate.input = cswPrivate.cell1.input({
                            name: nodeProperty.name,
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: function (barcode) {
                                nodeProperty.propData.values.barcode = barcode;
                                Csw.properties.publish(eventName, barcode);
                                //Csw.tryExec(nodeProperty.onChange, barcode);
                                //nodeProperty.onPropChange({ barcode: barcode });
                            },
                            value: cswPrivate.value
                        });
                        cswPrivate.input.required(nodeProperty.isRequired());
                        cswPrivate.input.clickOnEnter(function () {
                            cswPrivate.publish('CswSaveTabsAndProp_tab' + nodeProperty.tabState.tabid + '_' + nodeProperty.tabState.nodeid);
                        });
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
                nodeProperty.unBindRender(function() {
                    Csw.properties.unsubscribe(eventName);
                });

                return true;

            });
} ());
