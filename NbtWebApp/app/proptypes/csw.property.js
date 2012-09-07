/// <reference  path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.nbt.property = Csw.nbt.property ||
        Csw.nbt.register('property',
            Csw.method(function (cswParent, options) {
                /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
                /// <param name="cswParent" type="Csw.literals">An Csw Control to bind to.</param>
                /// <returns type="Csw.controls">The options object with DOM methods attached.</returns> 
                'use strict';
                var cswPrivate = Csw.nbt.propertyOption(options, cswParent);
                var cswPublic = {};
                
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException('Cannot create a Csw property without a Csw control', 'csw.property', 'csw.property.js', 15);
                }

                (function _preCtor() {
                    switch (cswPrivate.fieldtype) {
                        case Csw.enums.subFieldsMap.AuditHistoryGrid.name:
                            cswPublic = Csw.properties.auditHistoryGrid(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Barcode.name:
                            cswPublic = Csw.properties.barcode(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Button.name:
                            cswPublic = Csw.properties.button(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Comments.name:
                            cswPublic = Csw.properties.comments(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Composite.name:
                            cswPublic = Csw.properties.composite(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.DateTime.name:
                            cswPublic = Csw.properties.dateTime(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.File.name:
                            cswPublic = Csw.properties.file(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Grid.name:
                            cswPublic = Csw.properties.grid(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Image.name:
                            cswPublic = Csw.properties.image(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.ImageList.name:
                            cswPublic = Csw.properties.imageList(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Link.name:
                            cswPublic = Csw.properties.link(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.List.name:
                            cswPublic = Csw.properties.list(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Location.name:
                            cswPublic = Csw.properties.location(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.LocationContents.name:
                            cswPublic = Csw.properties.locationContents(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Logical.name:
                            cswPublic = Csw.properties.logical(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.LogicalSet.name:
                            cswPublic = Csw.properties.logicalSet(cswPrivate);
                            break;
                        case Csw.enums.subFieldsMap.Memo.name:
                            cswPublic = Csw.properties.memo(cswPrivate);
                            break;
                            
                        case Csw.enums.subFieldsMap.MultiList.name:
                            cswPublic = Csw.properties.multiList(cswPrivate);
                            break;
                            
                        case Csw.enums.subFieldsMap.NodeTypeSelect.name:
                            cswPublic = Csw.properties.nodeTypeSelect(cswPrivate);
                            break;    
                        case Csw.enums.subFieldsMap.Number.name:
                            cswPublic = Csw.properties.number(cswPrivate);
                            break;   

                        case Csw.enums.subFieldsMap.Sequence.name:
                            cswPublic = Csw.properties.sequence(cswPrivate);
                            break;
                            
                        case Csw.enums.subFieldsMap.UserSelect.name:
                            cswPublic = Csw.properties.userSelect(cswPrivate);
                            break;
                            
                        case Csw.enums.subFieldsMap.ViewPickList.name:
                            cswPublic = Csw.properties.viewPickList(cswPrivate);
                            break;
                        default:
                            cswPublic = $.CswFieldTypeFactory('make', cswPrivate);
                    }
                }());

                (function _postCtor() {

                }());
                
                return cswPublic;
            }));


} ());


