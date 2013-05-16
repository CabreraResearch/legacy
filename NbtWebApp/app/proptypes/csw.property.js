/// <reference  path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.nbt.property = Csw.nbt.register('property',
        function(cswPublic, tabsAndProps) {

            'use strict';
            
            (function _preCtor() {
                switch (cswPublic.fieldtype) {
                case Csw.enums.subFieldsMap.AuditHistoryGrid.name:
                    Csw.properties.auditHistoryGrid(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Barcode.name:
                    Csw.properties.barcode(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Button.name:
                    //Buttons, and only buttons, require access to the tabsAndProps instance
                    Csw.properties.button(cswPublic, tabsAndProps);
                    break;
                case Csw.enums.subFieldsMap.CASNo.name:
                    Csw.properties.CASNo(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.ChildContents.name:
                    Csw.properties.childContents(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Comments.name:
                    Csw.properties.comments(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Composite.name:
                    Csw.properties.composite(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.DateTime.name:
                    Csw.properties.dateTime(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.File.name:
                    Csw.properties.file(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Grid.name:
                    Csw.properties.grid(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Image.name:
                    Csw.properties.image(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.ImageList.name:
                    Csw.properties.imageList(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Link.name:
                    Csw.properties.link(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.List.name:
                    Csw.properties.list(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Location.name:
                    Csw.properties.location(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.LocationContents.name:
                    //TODO: Remove this class
                    break;
                case Csw.enums.subFieldsMap.Logical.name:
                    Csw.properties.logical(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.LogicalSet.name:
                    Csw.properties.logicalSet(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.MetaDataList.name:
                    Csw.properties.metaDataList(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Memo.name:
                    Csw.properties.memo(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.MOL.name:
                    Csw.properties.mol(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.MTBF.name:
                    Csw.properties.mtbf(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.MultiList.name:
                    Csw.properties.multiList(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.NFPA.name:
                    Csw.properties.nfpa(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.NodeTypeSelect.name:
                    Csw.properties.nodeTypeSelect(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Number.name:
                    Csw.properties.number(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Password.name:
                    Csw.properties.password(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.PropertyReference.name:
                    Csw.properties.propertyReference(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Quantity.name:
                    Csw.properties.quantity(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Question.name:
                    Csw.properties.question(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Relationship.name:
                    Csw.properties.relationship(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Scientific.name:
                    Csw.properties.scientific(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Sequence.name:
                    Csw.properties.sequence(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Static.name:
                    Csw.properties['static'](cswPublic);
                    break;
                case Csw.enums.subFieldsMap.Text.name:
                    Csw.properties.text(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.TimeInterval.name:
                    Csw.properties.timeInterval(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.UserSelect.name:
                    Csw.properties.userSelect(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.ViewPickList.name:
                    Csw.properties.viewPickList(cswPublic);
                    break;
                case Csw.enums.subFieldsMap.ViewReference.name:
                    Csw.properties.viewReference(cswPublic);
                    break;
                default:
                    Csw.error.throwException('No matching property type for "' + cswPublic.fieldtype + '" could be found.', 'Csw.property', 'csw.property.js', 130);
                }
            }());

            (function _postCtor() {

            }());

            return cswPublic;
        });
} ());


