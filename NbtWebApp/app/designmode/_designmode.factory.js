/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    Csw.designmode.factory = Csw.designmode.factory ||
        Csw.designmode.register('factory',
            function (cswParent) {
                /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
                /// <param name="cswParent" type="Csw.literals">An Csw Control to bind to.</param>
                /// <returns type="Csw.designmode">The options object with DOM methods attached.</returns> 
                'use strict';

                var cswPrivate = {
                    count: 0
                };
                if (Csw.isNullOrEmpty(cswParent)) {
                    throw new Error('Cannot create a Csw designmode without a Csw control');
                }

                cswPrivate.controlPreProcessing = function (opts, controlName) {
                    opts = opts || {};
                    cswPrivate.count += 1;
                    opts.suffix = controlName + cswPrivate.count;
                    opts.ID = cswParent.getId() + opts.suffix;
                    return opts;
                };

                //Csw.each is EXPENSIVE. Do !not! do this until each() is fixed.
                //Csw.each(Csw.composites, function (literal, name) {
                //    if (false === Csw.contains(Csw, name) &&
                //        name !== 'factory') {
                //        cswParent[name] = function (opts) {
                //            opts = cswPrivate.controlPreProcessing(opts, name);
                //            return Csw.composites[name](cswParent, opts);
                //};
                //    }
                //});

                cswParent.sidebar = function (opts) {
                    /// <summary> Creates a Csw.sidebar on this element</summary>
                    /// <param name="opts" type="Object">Options to define the sidebar.</param>
                    /// <returns type="Csw.designmode.sidebar">A Csw.designmode.sidebar</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'sidebar');
                    return Csw.designmode.sidebar(cswParent, opts);
                };

                return cswParent;
            });


}());


