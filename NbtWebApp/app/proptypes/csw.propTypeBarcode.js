/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.barcode = Csw.properties.barcode ||
        Csw.properties.register('barcode',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPrivate = { };
                var cswPublic = {
                    data: propertyOption
                };
                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);
                    
                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.value = (false === cswPublic.data.isMulti()) ? Csw.string(cswPrivate.propVals.barcode).trim() : Csw.enums.multiEditDefaultValue;

                    cswPublic.control = cswPublic.data.propDiv.table({
                        ID: Csw.makeId(cswPublic.data.ID, 'tbl')
                    });

                    cswPrivate.cell1 = cswPublic.control.cell(1, 1);

                    if (cswPublic.data.isReadOnly()) {
                        cswPrivate.cell1.text(cswPrivate.value);
                    } else {

                        cswPrivate.input = cswPrivate.cell1.input({
                            ID: cswPublic.data.ID,
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
                        cswPublic.control.cell(1, 2).div({ ID: Csw.makeId(cswPublic.data.ID, 'parent', window.Ext.id()) })
                            .buttonExt({
                                ID: Csw.makeId(cswPublic.data.ID, 'print', window.Ext.id()),
                                enabledText: 'Print',
                                size: 'small',
                                tooltip: { title: 'Print Barcode Label' },
                                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.barcode),
                                onClick: function () {
                            $.CswDialog('PrintLabelDialog', {
                                nodeids: [cswPublic.data.tabState.nodeid],
                                nodetypeid: cswPublic.data.tabState.nodetypeid
                            });
                                },
                                editMode: cswPublic.data.tabState.EditMode
                            });
                    }
                };
                cswPublic.data.bindRender(render);

                return cswPublic;

            }));
}());
