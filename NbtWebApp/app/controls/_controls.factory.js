/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.controls.factory = Csw.controls.factory ||
        Csw.controls.register('factory',
            function (cswParent) {
                /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
                /// <param name="cswParent" type="Csw.literals">An Csw Control to bind to.</param>
                /// <returns type="Csw.controls">The options object with DOM methods attached.</returns> 
                'use strict';
                var cswPrivate = {};
                if (Csw.isNullOrEmpty(cswParent)) {
                    Csw.error.throwException('Cannot create a Csw control without a Csw parent', '_controls.factory', '_controls.factory.js', 14);
                }

                cswPrivate.controlPreProcessing = function (opts, controlName) {
                    opts = opts || {};
                    opts.suffix = controlName;
                    return opts;
                };

                Csw.each(Csw.controls, function (literal, name) {
                    if (false === Csw.contains(Csw, name) &&
                        name !== 'factory') {
                        cswParent[name] = function (opts) {
                            opts = cswPrivate.controlPreProcessing(opts, name);
                            return Csw.controls[name](cswParent, opts);
                        };
                    }
                });


                return cswParent;
            });


}());


