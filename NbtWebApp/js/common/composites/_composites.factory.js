/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.composites.factory = Csw.composites.factory ||
        Csw.composites.register('factory',
            function (cswParent) {
                /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
                /// <param name="cswParent" type="Csw.literals">An Csw Control to bind to.</param>
                /// <returns type="Csw.composites">The options object with DOM methods attached.</returns> 
                'use strict';

                var internal = {};
                if (Csw.isNullOrEmpty(cswParent)) {
                    throw new Error('Cannot create a Csw component without a Csw control');
                }

                internal.controlPreProcessing = function (opts, controlName) {
                    opts = opts || {};
                    return opts;
                };

                cswParent.grid = function (opts) {
                    /// <summary> Creates a Csw.grid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the grid.</param>
                    /// <returns type="Csw.composites.grid">A Csw.composites.grid</returns>
                    opts = internal.controlPreProcessing(opts, 'grid');
                    return Csw.composites.grid(cswParent, opts);
                };

                cswParent.layoutTable = function (opts) {
                    /// <summary> Creates a Csw.layoutTable on this element</summary>
                    /// <param name="opts" type="Object">Options to define the table.</param>
                    /// <returns type="Csw.composites.layoutTable">A Csw.composites.layoutTable</returns> 
                    opts = internal.controlPreProcessing(opts, 'layoutTable');
                    return Csw.composites.layoutTable(cswParent, opts);
                };
                
//                cswParent.table = function (opts) {
//                    /// <summary> Creates a Csw.table on this element</summary>
//                    /// <param name="opts" type="Object">Options to define the table.</param>
//                    /// <returns type="Csw.composites.table">A Csw.composites.table</returns> 
//                    opts = internal.controlPreProcessing(opts, 'table');
//                    return Csw.composites.table(cswParent, opts);
//                };


                cswParent.thinGrid = function (opts) {
                    /// <summary> Creates a Csw.thinGrid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the thinGrid.</param>
                    /// <returns type="Csw.composites.thinGrid">A Csw.composites.thinGrid</returns>
                    opts = internal.controlPreProcessing(opts, 'thinGrid');
                    return Csw.composites.thinGrid(cswParent, opts);
                };
                
                return cswParent;
            });


} ());


