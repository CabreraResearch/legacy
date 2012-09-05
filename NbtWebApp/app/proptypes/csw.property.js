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
                var cswPrivate = Csw.nbt.propertyOption(options);
                var cswPublic = {};
                
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException('Cannot create a Csw property without a Csw control', 'csw.property', 'csw.property.js', 15);
                }

                (function _preCtor() {
                    switch (cswPrivate.fieldtype) {
                        case Csw.enums.subFieldsMap.AuditHistoryGrid.name:
                            cswPublic = Csw.properties.auditHistoryGrid(cswParent, cswPrivate);
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


