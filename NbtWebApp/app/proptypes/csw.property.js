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
                var cswPrivate = Csw.nbt.propertyOption();
                
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException('Cannot create a Csw property without a Csw control', 'csw.property', 'csw.property.js', 15);
                }
                                             
                cswPrivate.controlPreProcessing = function (opts, controlName) {
                    var id = '';
                    opts = opts || {};
                    if (opts.getId) {
                        id = opts.getId();
                    }
                    if (opts.suffix) {
                        opts.ID = Csw.makeId(id, opts.suffix);
                    } else if (Csw.isNullOrEmpty(opts.ID) && false === Csw.isNullOrEmpty(cswParent.getId())) {
                        opts.ID = Csw.makeId(id, controlName);
                    }
                    return opts;
                };
                
                cswParent.buttonExt = function (opts) {
                    /// <summary> Creates a Csw.buttonExt on this element</summary>
                    /// <param name="opts" type="Object">Options to define the buttonExt.</param>
                    /// <returns type="Csw.controls.buttonExt">A Csw.controls.buttonExt</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'buttonExt');
                    return Csw.controls.buttonExt(cswParent, opts);
                };

                
            }));


} ());


