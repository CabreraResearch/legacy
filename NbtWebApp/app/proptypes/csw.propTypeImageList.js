/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.imageList = Csw.properties.imageList ||
        Csw.properties.register('imageList',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.value = (false === cswPublic.data.Multi) ? Csw.string(cswPrivate.propVals.value).trim() : Csw.enums.multiEditDefaultValue;
                    cswPrivate.options = cswPrivate.propVals.options;
                    cswPrivate.width = Csw.string(cswPrivate.propVals.width);
                    cswPrivate.height = Csw.string(cswPrivate.propVals.height);
                    cswPrivate.allowMultiple = Csw.bool(cswPrivate.propVals.allowmultiple);

                    cswPublic.control = cswPrivate.parent.table({
                        ID: Csw.makeId(cswPublic.data.ID, 'tbl'),
                        cellvalign: 'top'
                    });

                    cswPrivate.imageTable = cswPublic.control.cell(1, 1).table({
                        ID: Csw.makeId(cswPublic.data.ID, 'tbl')
                    });
                    cswPrivate.imgTblCol = 1;
                    cswPrivate.selectedValues = [];

                    cswPrivate.changeImage = function (name, href, doAnimation, selected) {
                        if (false === cswPrivate.allowMultiple) {
                            cswPrivate.imageTable.empty();
                            cswPrivate.selectedValues = [];
                        }
                        cswPrivate.addValue(selected.val());
                        if (cswPrivate.allowMultiple) {
                            selected.remove();
                        }
                        if (name != 'Select...') {
                            cswPrivate.addImage(name, href, doAnimation);
                        }
                    };

                    cswPrivate.addImage = function (name, href, doAnimation) {
                        var imageCell = cswPrivate.imageTable.cell(1, cswPrivate.imgTblCol)
                            .css({
                                'text-align': 'center',
                                'padding-left': '10px'
                            });
                        imageCell.a({
                            href: href,
                            target: '_blank'
                        })
                            .img({
                                src: href,
                                alt: name,
                                width: cswPrivate.width,
                                height: cswPrivate.height
                            });
                        var nameCell = cswPrivate.imageTable.cell(2, cswPrivate.imgTblCol)
                            .css({
                                'text-align': 'center',
                                'padding-left': '10px'
                            });
                        cswPrivate.imgTblCol += 1;

                        if (doAnimation) {
                            imageCell.hide();
                            nameCell.hide();
                        }

                        if (name !== href) {
                            nameCell.a({ href: href, target: '_blank', text: name });
                        }
                        if (false === cswPublic.data.ReadOnly && (false === cswPublic.data.Required || cswPrivate.allowMultiple)) {
                            nameCell.icon({
                                ID: Csw.makeId('image', cswPrivate.imgTblCol, 'rembtn'),
                                iconType: Csw.enums.iconType.trash,
                                hovertext: 'Remove',
                                size: 16,
                                isButton: true,
                                onClick: function () {
                                    nameCell.$.fadeOut('fast');
                                    imageCell.$.fadeOut('fast');
                                    cswPrivate.removeValue(href);
                                    if (cswPrivate.allowMultiple) {
                                        cswPrivate.imageSelectList.option({ value: href, display: name });
                                    }
                                    Csw.tryExec(cswPublic.data.onChange);
                                } // onClick
                            }); //
                        } // if(!o.ReadOnly)

                        if (doAnimation) {
                            imageCell.$.fadeIn('fast');
                            nameCell.$.fadeIn('fast');
                        }
                    }; // addImage()

                    cswPrivate.addValue = function (valueToAdd) {
                        if (false === Csw.contains(cswPrivate.selectedValues, valueToAdd) &&
                            false === Csw.isNullOrEmpty(valueToAdd)) {
                            cswPrivate.selectedValues.push(valueToAdd);
                        }
                        cswPrivate.hiddenValue.text(cswPrivate.selectedValues.join('\n'));
                    };

                    cswPrivate.removeValue = function (valueToRemove) {
                        var idx = cswPrivate.selectedValues.indexOf(valueToRemove);
                        if (idx !== -1) {
                            cswPrivate.selectedValues.splice(idx, 1);
                        }
                        cswPrivate.hiddenValue.text(cswPrivate.selectedValues.join('\n'));
                    };

                    if (false === cswPublic.data.ReadOnly) {
                        cswPrivate.imageSelectList = cswPublic.control.cell(1, 2).select({ id: cswPublic.data.ID });
                        cswPrivate.selectOption = cswPrivate.imageSelectList.option({ value: '', display: 'Select...' });
                        if (cswPublic.data.Multi) {
                            cswPrivate.imageSelectList.option({ value: Csw.enums.multiEditDefaultValue, display: Csw.enums.multiEditDefaultValue, isSelected: true });
                        }
                        cswPrivate.hiddenValue = cswPrivate.parent.textArea({
                            ID: cswPublic.data.ID + '_value',
                            value: cswPrivate.value,
                            onChange: function() {
                                var val = cswPrivate.hiddenValue.val();
                                cswPublic.data.onPropChange({ value: val });
                            }
                        }).hide();

                        cswPrivate.imageSelectList.bind('change', function () {
                            var selected = cswPrivate.imageSelectList.children(':selected');
                            cswPrivate.changeImage(selected.text(), selected.val(), true, selected);
                            if (cswPublic.data.Required && false === cswPrivate.allowMultiple) {
                                cswPrivate.selectOption.remove();
                            }
                            cswPublic.data.onChange();
                        });

                        Csw.crawlObject(cswPrivate.options,
                            function (thisOpt) {
                                if (Csw.bool(thisOpt.selected)) {
                                    cswPrivate.selectedValues.push(thisOpt.value);
                                    cswPrivate.addImage(thisOpt.text, thisOpt.value, false);
                                } else {
                                    if (false === cswPublic.data.ReadOnly) {
                                        cswPrivate.imageSelectList.option({ value: thisOpt.value, display: thisOpt.text });
                                    }
                                }
                            },
                            false
                        );

                        if (cswPublic.data.Required) {
                            $.validator.addMethod('imageRequired', function (value, element) {
                                return (cswPrivate.selectedValues.length > 0);
                            }, 'An image is required.');
                            cswPrivate.imageSelectList.addClass('imageRequired');
                        }
                    }
                    
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

}());

