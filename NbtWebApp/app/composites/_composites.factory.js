/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.composites.factory = Csw.composites.factory ||
        Csw.composites.register('factory',
            function (cswParent) {
                /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
                /// <param name="cswParent" type="Csw.literals">An Csw Control to bind to.</param>
                /// <returns type="Csw.composites">The options object with DOM methods attached.</returns> 
                'use strict';

                var cswPrivate = {};
                if (Csw.isNullOrEmpty(cswParent)) {
                    throw new Error('Cannot create a Csw component without a Csw control');
                }

                cswPrivate.controlPreProcessing = function (opts, controlName) {
                    opts = opts || {};
                    opts.ID = cswParent.getId() + '_' + controlName;
                    return opts;
                };

                Csw.each(Csw.composites, function (literal, name) {
                    if (false === Csw.contains(Csw, name) &&
                        name !== 'factory') {
                        cswParent[name] = function (opts) {
                            opts = cswPrivate.controlPreProcessing(opts, name);
                            return Csw.composites[name](cswParent, opts);
                };
                    }
                });
                
                return cswParent;
            });


} ());


