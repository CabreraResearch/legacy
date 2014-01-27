/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.register('imageList', function (nodeProperty) {
        'use strict';
        var cswPrivate = {};

        //The render function to be executed as a callback
        var render = function () {
            'use strict';

            cswPrivate.value = nodeProperty.propData.values.value;
            cswPrivate.options = nodeProperty.propData.values.options;
            cswPrivate.width = nodeProperty.propData.values.width;
            cswPrivate.height = nodeProperty.propData.values.height;
            cswPrivate.allowMultiple = nodeProperty.propData.values.allowmultiple;
            cswPrivate.imageprefix = nodeProperty.propData.values.imageprefix;

            var table = nodeProperty.propDiv.table({
                cellvalign: 'top'
            });
            var cell = table.cell(1, 2);
            var errorDiv = table.cell(1, 3).div();

            cswPrivate.imageTable = table.cell(1, 1).table();
            cswPrivate.imgTblCol = 1;
            cswPrivate.selectedValues = [];

            cswPrivate.addImage = function (name, href, doAnimation) {
                var imageCell = cswPrivate.imageTable.cell(1, cswPrivate.imgTblCol)
                    .css({
                        'text-align': 'center',
                        'padding-left': '10px',
                        'padding-right': '10px'
                    });
                imageCell.a({
                    href: cswPrivate.imageprefix + href,
                    target: '_blank'
                })
                    .img({
                        src: cswPrivate.imageprefix + href,
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
                    nameCell.a({ href: cswPrivate.imageprefix + href, target: '_blank', text: name });
                }
                if (false === nodeProperty.isReadOnly() && (false === nodeProperty.isRequired() || cswPrivate.allowMultiple)) {
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
                            //Csw.tryExec(nodeProperty.onChange);
                        } // onClick
                    }); //
                } // if(!o.ReadOnly)

                if (doAnimation) {
                    imageCell.$.fadeIn('fast');
                    nameCell.$.fadeIn('fast');
                }
            }; // addImage()

            cswPrivate.saveProp = function () {
                //Case 29390: No sync for Image List
                nodeProperty.propData.values.value = cswPrivate.selectedValues.join('\n');
                nodeProperty.broadcastPropChange(nodeProperty.propData.values.value);
                cswPrivate.doValidation(nodeProperty.propData.values.value);
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
            
            (function () {
                
                cswPrivate.imageSelectList = cell.imageSelect({
                    comboImgHeight: cswPrivate.height,
                    comboImgWidth: cswPrivate.width,
                    imageprefix: cswPrivate.imageprefix,
                    onSelect: function (name, href, id, imageCell, nameCell) {
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
                        Csw.tryExec(nodeProperty.onChange, cswPrivate.selectedValues);
                        return false;
                    }
                });

                cswPrivate.imageTable.empty();
                Csw.iterate(cswPrivate.options, function (thisOpt) {
                    if (Csw.bool(thisOpt.selected)) {
                        cswPrivate.selectedValues.push(thisOpt.value);
                        cswPrivate.addImage(thisOpt.text, thisOpt.value, false);
                    } else {
                        if (false === nodeProperty.isReadOnly()) {
                            cswPrivate.imageSelectList.addOption(thisOpt.text, thisOpt.value);
                        }
                    }
                }, true);
                
                if (nodeProperty.isReadOnly()) {
                    cswPrivate.imageSelectList.hide();
                }
                cswPrivate.imageSelectList.required(nodeProperty.isRequired());
                
                cswPrivate.doValidation = function (val) {
                    var ret = true;
                    if (nodeProperty.isRequired()) {
                        if (Csw.isNullOrEmpty(val)) {
                            cswPrivate.validatorCheckBox.val(false);
                            ret = false;
                        } else {
                            cswPrivate.validatorCheckBox.val(true);
                            ret = true;
                        }
                        cswPrivate.validatorCheckBox.$.valid();
                    }
                    return ret;
                };

                var validator = Csw.validator(errorDiv, cswPrivate.imageSelectList, {
                    cssOptions: { 'visibility': 'hidden', 'width': '20px' },
                    errorMsg: 'At least one value must be selected',
                    className: 'imageListValidator' + nodeProperty.propData.id,
                    onValidation: function (isValid) {
                        if (isValid || Csw.enums.editMode.Edit === nodeProperty.tabState.EditMode) {
                            errorDiv.show();
                        } else {
                            errorDiv.show();
                        }
                    }
                });

                cswPrivate.validatorCheckBox = validator.input;
                var isInitiallyValid = ((false === nodeProperty.isRequired()) || false === Csw.isNullOrEmpty(nodeProperty.propData.values.value));
                cswPrivate.validatorCheckBox.val(isInitiallyValid);
                if (false === nodeProperty.isReadOnly()) {
                    cswPrivate.doValidation(nodeProperty.propData.values.value);
                }

            })();
        }; // render()

        //Bind the callback to the render event
        nodeProperty.bindRender(render);

        //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
        //nodeProperty.unBindRender();

        return true;
    });

}());

