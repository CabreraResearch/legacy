/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.barcode = Csw.properties.barcode ||
        Csw.properties.register('barcode',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };
                
                //The render function to be executed as a callback
                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.value = Csw.string(cswPrivate.propVals.barcode).trim();

                    cswPublic.control = cswPublic.data.propDiv.table({
                        name: 'tbl'
                    });

                    cswPrivate.cell1 = cswPublic.control.cell(1, 1);

                    if (cswPublic.data.isReadOnly()) {
                        cswPrivate.cell1.text(cswPrivate.value);
                    } else {

                        cswPrivate.input = cswPrivate.cell1.input({
                            name: cswPublic.data.name,
                            type: Csw.enums.inputTypes.text,
                            cssclass: 'textinput',
                            onChange: function () {
                                var barcode = cswPrivate.input.val();
                                Csw.tryExec(cswPublic.data.onChange, barcode);
                                cswPublic.data.onPropChange({ barcode: barcode });
                            },
                            value: cswPrivate.value
                        });
                        cswPrivate.input.required(cswPublic.data.isRequired());
                        cswPrivate.input.clickOnEnter(cswPublic.data.saveBtn);
                    }
                    if (false === cswPublic.data.isMulti()) {
                        var nodeObj = {};
                        nodeObj[cswPublic.data.tabState.nodeid] = {};
                        nodeObj[cswPublic.data.tabState.nodeid].nodeid = cswPublic.data.tabState.nodeid;
                        nodeObj[cswPublic.data.tabState.nodeid].nodename = cswPublic.data.tabState.nodename;
                        cswPublic.control.cell(1, 2).div({ name: 'parent' })
                            .buttonExt({
                                name: 'print',
                                enabledText: 'Print',
                                size: 'small',
                                tooltip: { title: 'Print Barcode Label' },
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.barcode),
                                onClick: function () {
                                    $.CswDialog('PrintLabelDialog', {
                                        nodes: nodeObj,
                                        nodetypeid: cswPublic.data.tabState.nodetypeid
                                    });
                                },
                                disabled: cswPublic.data.isDisabled()
                            });
                    }
                };
                
                //Bind the callback to the render event
                cswPublic.data.bindRender(render);

                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;

            }));
} ());
