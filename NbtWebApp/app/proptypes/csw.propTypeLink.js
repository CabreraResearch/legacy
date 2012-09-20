/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.link = Csw.properties.link ||
        Csw.properties.register('link',
            Csw.method(function(propertyOption) {
                'use strict';
                var cswPrivate = { };
                var cswPublic = {
                    data: propertyOption
                };
                
                var render = function() {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.text = (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.text).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.href = (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.href).trim() : Csw.enums.multiEditDefaultValue;

                    if (cswPublic.data.ReadOnly) {
                        cswPublic.control = cswPrivate.parent.a({
                            href: cswPrivate.href,
                            text: cswPrivate.text
                        });
                    } else {
                        cswPublic.control = cswPrivate.parent.table({
                            ID: Csw.makeId(cswPublic.data.ID, 'tbl')
                        });

                        cswPublic.control.cell(1, 1).a({
                            href: cswPrivate.href,
                            text: cswPrivate.text
                        });
                        cswPrivate.cell12 = cswPublic.control.cell(1, 2).div();

                        cswPrivate.cell12.icon({
                            ID: cswPublic.data.ID + '_edit',
                            iconType: Csw.enums.iconType.pencil,
                            hovertext: 'Edit',
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                cswPrivate.editTable.show();
                            }
                        });

                        cswPrivate.editTable = cswPrivate.parent.table({
                            ID: Csw.makeId(cswPublic.data.ID, 'edittbl')
                        }).hide();

                        cswPrivate.editTable.cell(1, 1).span({ text: 'Text' });

                        cswPrivate.editTextCell = cswPrivate.editTable.cell(1, 2);
                        cswPrivate.editText = cswPrivate.editTextCell.input({
                            ID: cswPublic.data.ID + '_text',
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.text,
                            onChange: function() {
                                var val = cswPrivate.editText.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ text: val });
                            }
                        });

                        cswPrivate.editTable.cell(2, 1).span({ text: 'URL' });

                        cswPrivate.editHrefCell = cswPrivate.editTable.cell(2, 2);
                        cswPrivate.editHref = cswPrivate.editHrefCell.input({
                            ID: cswPublic.data.ID + '_href',
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.href,
                            onChange: function () {
                                var val = cswPrivate.editHref.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ href: val });
                            }
                        });

                        cswPrivate.editText.required(cswPublic.data.Required);
                        cswPrivate.editHref.required(cswPublic.data.Required);
                        if (cswPublic.data.Required && cswPrivate.href === '') {
                            cswPrivate.editTable.show();
                        }
                        cswPrivate.editText.clickOnEnter(cswPublic.data.saveBtn);
                        cswPrivate.editHref.clickOnEnter(cswPublic.data.saveBtn);
                    }

                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));
    
}());
  
