/// <reference  path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.nbt.property = Csw.nbt.register('property',
        function(cswPrivate) {

            'use strict';
            cswPrivate = cswPrivate || Csw.nbt.propertyOption({});
            var cswPublic = {};

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
                case Csw.enums.subFieldsMap.CASNo.name:
                    cswPublic = Csw.properties.CASNo(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.ChildContents.name:
                    cswPublic = Csw.properties.childContents(cswPrivate);
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
                    //TODO: Remove this class
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
                case Csw.enums.subFieldsMap.MOL.name:
                    cswPublic = Csw.properties.mol(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.MTBF.name:
                    cswPublic = Csw.properties.mtbf(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.MultiList.name:
                    cswPublic = Csw.properties.multiList(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.NFPA.name:
                    cswPublic = Csw.properties.nfpa(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.NodeTypeSelect.name:
                    cswPublic = Csw.properties.nodeTypeSelect(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.Number.name:
                    cswPublic = Csw.properties.number(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.Password.name:
                    cswPublic = Csw.properties.password(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.PropertyReference.name:
                    cswPublic = Csw.properties.propertyReference(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.Quantity.name:
                    cswPublic = Csw.properties.quantity(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.Question.name:
                    cswPublic = Csw.properties.question(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.Relationship.name:
                    cswPublic = Csw.properties.relationship(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.Scientific.name:
                    cswPublic = Csw.properties.scientific(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.Sequence.name:
                    cswPublic = Csw.properties.sequence(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.Static.name:
                    cswPublic = Csw.properties['static'](cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.Text.name:
                    cswPublic = Csw.properties.text(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.TimeInterval.name:
                    cswPublic = Csw.properties.timeInterval(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.UserSelect.name:
                    cswPublic = Csw.properties.userSelect(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.ViewPickList.name:
                    cswPublic = Csw.properties.viewPickList(cswPrivate);
                    break;
                case Csw.enums.subFieldsMap.ViewReference.name:
                    cswPublic = Csw.properties.viewReference(cswPrivate);
                    break;
                default:
                    Csw.error.throwException('No matching property type for "' + cswPrivate.fieldtype + '" could be found.', 'Csw.property', 'csw.property.js', 130);
                }
            }());

            (function _postCtor() {

            }());

            return cswPublic;
        });
} ());


