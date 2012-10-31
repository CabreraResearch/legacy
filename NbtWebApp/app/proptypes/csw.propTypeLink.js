/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.link = Csw.properties.link ||
        Csw.properties.register('link',
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
                    cswPrivate.parent = cswPublic.data.propDiv;

                    cswPrivate.text = Csw.string(cswPrivate.propVals.text).trim();
                    cswPrivate.href = Csw.string(cswPrivate.propVals.href).trim();

                    if (cswPublic.data.isReadOnly()) {
                        cswPublic.control = cswPrivate.parent.a({
                            href: cswPrivate.href,
                            text: cswPrivate.text
                        });
                    } else {
                        cswPublic.control = cswPrivate.parent.table();

                        if (cswPublic.data.isDisabled()) {
                            cswPublic.control.cell(1, 1).p({
                                text: cswPrivate.text
                            });
                        } else {
                            cswPublic.control.cell(1, 1).a({
                                href: cswPrivate.href,
                                text: cswPrivate.text
                            });
                        }
                        cswPrivate.cell12 = cswPublic.control.cell(1, 2).div();

                        cswPrivate.cell12.icon({
                            iconType: Csw.enums.iconType.pencil,
                            hovertext: 'Edit',
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                cswPrivate.editTable.show();
                            }
                        });

                        cswPrivate.editTable = cswPrivate.parent.table().hide();

                        cswPrivate.editTable.cell(1, 1).span({ text: 'Text' });

                        cswPrivate.editTextCell = cswPrivate.editTable.cell(1, 2);
                        cswPrivate.editText = cswPrivate.editTextCell.input({
                            name: cswPublic.data.name + '_text',
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.text,
                            onChange: function () {
                                var val = cswPrivate.editText.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ text: val });
                            }
                        });

                        cswPrivate.editTable.cell(2, 1).span({ text: 'URL' });

                        cswPrivate.editHrefCell = cswPrivate.editTable.cell(2, 2);
                        cswPrivate.editHref = cswPrivate.editHrefCell.input({
                            name: cswPublic.data.name + '_href',
                            type: Csw.enums.inputTypes.text,
                            value: cswPrivate.href,
                            onChange: function () {
                                var val = cswPrivate.editHref.val();
                                Csw.tryExec(cswPublic.data.onChange, val);
                                cswPublic.data.onPropChange({ href: val });
                            }
                        });

                        cswPrivate.editText.required(cswPublic.data.isRequired());
                        cswPrivate.editHref.required(cswPublic.data.isRequired());
                        if (cswPublic.data.isRequired() && cswPrivate.href === '') {
                            cswPrivate.editTable.show();
                        }
                        cswPrivate.editText.clickOnEnter(cswPublic.data.saveBtn);
                        cswPrivate.editHref.clickOnEnter(cswPublic.data.saveBtn);
                    }

                };

                //Bind the callback to the render event
                cswPublic.data.bindRender(render);
                
                //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
                //cswPublic.data.unBindRender();

                return cswPublic;
            }));

} ());
  
