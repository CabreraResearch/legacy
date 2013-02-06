/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.imageList = Csw.properties.imageList ||
        Csw.properties.register('imageList', Csw.method(function (propertyOption) {
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
                cswPrivate.value = Csw.string(cswPrivate.propVals.value).trim();
                cswPrivate.options = cswPrivate.propVals.options;
                cswPrivate.width = Csw.string(cswPrivate.propVals.width);
                cswPrivate.height = Csw.string(cswPrivate.propVals.height);
                cswPrivate.allowMultiple = Csw.bool(cswPrivate.propVals.allowmultiple);

                cswPublic.control = cswPrivate.parent.table({
                    cellvalign: 'top'
                });

                cswPrivate.imageTable = cswPublic.control.cell(1, 1).table();
                cswPrivate.imgTblCol = 1;
                cswPrivate.selectedValues = [];

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
                    if (false === cswPublic.data.isReadOnly() && (false === cswPublic.data.isRequired() || cswPrivate.allowMultiple)) {
                        nameCell.icon({
                            name: 'rembtn',
                            iconType: Csw.enums.iconType.trash,
                            hovertext: 'Remove',
                            size: 16,
                            isButton: true,
                            onClick: function () {
                                nameCell.$.fadeOut('fast');
                                imageCell.$.fadeOut('fast');
                                cswPrivate.removeValue(href);
                                if (cswPrivate.allowMultiple) {
                                    cswPrivate.imageSelectList.addOption(name, href);
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

                cswPrivate.saveProp = function () {
                    cswPublic.data.onPropChange({ value: cswPrivate.selectedValues.join('\n') });
                };

                cswPrivate.addValue = function (valueToAdd) {
                    if (false === Csw.contains(cswPrivate.selectedValues, valueToAdd) &&
                            false === Csw.isNullOrEmpty(valueToAdd)) {
                        cswPrivate.selectedValues.push(valueToAdd);
                    }
                    cswPrivate.saveProp();
                };

                cswPrivate.removeValue = function (valueToRemove) {
                    var idx = cswPrivate.selectedValues.indexOf(valueToRemove);
                    if (idx !== -1) {
                        cswPrivate.selectedValues.splice(idx, 1);
                    }
                    cswPrivate.saveProp();
                };

                cswPrivate.imageSelectList = cswPublic.control.cell(1, 2).imageSelect({
                    onSelect: function (name, href, imageCell, nameCell) {
                        if (false === cswPrivate.allowMultiple) {
                            cswPrivate.imageTable.empty();
                            cswPrivate.selectedValues = [];
                        }
                        cswPrivate.addValue(href);
                        if (cswPrivate.allowMultiple) {
                            imageCell.hide();
                            nameCell.hide();
                        }
                        cswPrivate.addImage(name, href, true);
                        Csw.tryExec(cswPublic.data.onChange, cswPrivate.selectedValues);
                    }
                });

                Csw.eachRecursive(cswPrivate.options, function (thisOpt) {
                    if (Csw.bool(thisOpt.selected)) {
                        cswPrivate.selectedValues.push(thisOpt.value);
                        cswPrivate.addImage(thisOpt.text, thisOpt.value, false);
                    } else {
                        if (false === cswPublic.data.isReadOnly()) {
                            cswPrivate.imageSelectList.addOption(thisOpt.text, thisOpt.value);
                        }
                    }
                }, false);

                if (cswPublic.data.isReadOnly()) {
                    cswPrivate.imageSelectList.hide();
                }
                cswPrivate.imageSelectList.required(cswPublic.data.isRequired());
                if (cswPublic.data.isRequired()) {
                    $.validator.addMethod('imageRequired', function (value, element) {
                        return (cswPrivate.selectedValues.length > 0);
                    }, 'An image is required.');
                    cswPrivate.imageSelectList.addClass('imageRequired');
                }

            };

            //Bind the callback to the render event
            cswPublic.data.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //cswPublic.data.unBindRender();

            return cswPublic;
        }));

} ());

