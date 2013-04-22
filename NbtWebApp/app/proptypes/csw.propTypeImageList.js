/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.properties.imageList = Csw.properties.register('imageList',
        function(nodeProperty) {
            'use strict';
            var cswPrivate = {};
            
            //The render function to be executed as a callback
            var render = function() {
                'use strict';

                cswPrivate.value = nodeProperty.propData.values.value;
                cswPrivate.options = nodeProperty.propData.values.options;
                cswPrivate.width = nodeProperty.propData.values.width;
                cswPrivate.height = nodeProperty.propData.values.height;
                cswPrivate.allowMultiple = nodeProperty.propData.values.allowmultiple;

                var table = nodeProperty.propDiv.table({
                    cellvalign: 'top'
                });

                cswPrivate.imageTable = table.cell(1, 1).table();
                cswPrivate.imgTblCol = 1;
                cswPrivate.selectedValues = [];

                cswPrivate.addImage = function(name, href, doAnimation) {
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
                    if (false === nodeProperty.isReadOnly() && (false === nodeProperty.isRequired() || cswPrivate.allowMultiple)) {
                        nameCell.icon({
                            name: 'rembtn',
                            iconType: Csw.enums.iconType.trash,
                            hovertext: 'Remove',
                            size: 16,
                            isButton: true,
                            onClick: function() {
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

                cswPrivate.saveProp = function() {
                    //Case 29390: No sync for Image List
                    nodeProperty.propData.values.value = cswPrivate.selectedValues.join('\n');
                    nodeProperty.onPropChangeBroadcast(nodeProperty.propData.values.value);
                };

                cswPrivate.addValue = function(valueToAdd) {
                    if (false === Csw.contains(cswPrivate.selectedValues, valueToAdd) &&
                        false === Csw.isNullOrEmpty(valueToAdd)) {
                        cswPrivate.selectedValues.push(valueToAdd);
                    }
                    cswPrivate.saveProp();
                };

                cswPrivate.removeValue = function(valueToRemove) {
                    var idx = cswPrivate.selectedValues.indexOf(valueToRemove);
                    if (idx !== -1) {
                        cswPrivate.selectedValues.splice(idx, 1);
                    }
                    cswPrivate.saveProp();
                };

                cswPrivate.imageSelectList = table.cell(1, 2).imageSelect({
                    onSelect: function(name, href, id, imageCell, nameCell) {
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

                Csw.iterate(cswPrivate.options, function(thisOpt) {
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
                if (nodeProperty.isRequired()) {
                    $.validator.addMethod('imageRequired', function(value, element) {
                        return (cswPrivate.selectedValues.length > 0);
                    }, 'An image is required.');
                    cswPrivate.imageSelectList.addClass('imageRequired');
                }

            };

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });

} ());

