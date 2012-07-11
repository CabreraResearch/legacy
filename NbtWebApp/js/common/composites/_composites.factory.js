/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
                    return opts;
                };

                cswParent.grid = function (opts) {
                    /// <summary> Creates a Csw.grid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the grid.</param>
                    /// <returns type="Csw.composites.grid">A Csw.composites.grid</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'grid');
                    return Csw.composites.grid(cswParent, opts);
                };

                cswParent.layoutTable = function (opts) {
                    /// <summary> Creates a Csw.layoutTable on this element</summary>
                    /// <param name="opts" type="Object">Options to define the table.</param>
                    /// <returns type="Csw.composites.layoutTable">A Csw.composites.layoutTable</returns> 
                    opts = cswPrivate.controlPreProcessing(opts, 'layoutTable');
                    return Csw.composites.layoutTable(cswParent, opts);
                };
                
//                cswParent.table = function (opts) {
//                    /// <summary> Creates a Csw.table on this element</summary>
//                    /// <param name="opts" type="Object">Options to define the table.</param>
//                    /// <returns type="Csw.composites.table">A Csw.composites.table</returns> 
//                    opts = cswPrivate.controlPreProcessing(opts, 'table');
//                    return Csw.composites.table(cswParent, opts);
//                };
                
                cswParent.menuButton = function (opts) {
                    /// <summary> Creates a Csw.menuButton on this element</summary>
                    /// <param name="opts" type="Object">Options to define the table.</param>
                    /// <returns type="Csw.composites.menuButton">A Csw.composites.menuButton</returns> 
                    opts = cswPrivate.controlPreProcessing(opts, 'menuButton');
                    return Csw.composites.menuButton(cswParent, opts);
                };

                cswParent.quantity = function (opts) {
                    /// <summary> Creates a Csw.quantity on this element</summary>
                    /// <param name="opts" type="Object">Options to define the quantity.</param>
                    /// <returns type="Csw.composites.quantity">A Csw.composites.quantity</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'quantity');
                    return Csw.composites.quantity(cswParent, opts);
                };

                cswParent.thinGrid = function (opts) {
                    /// <summary> Creates a Csw.thinGrid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the thinGrid.</param>
                    /// <returns type="Csw.composites.thinGrid">A Csw.composites.thinGrid</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'thinGrid');
                    return Csw.composites.thinGrid(cswParent, opts);
                };

                cswParent.linkGrid = function (opts) {
                    /// <summary> Creates a Csw.linkGrid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the linkGrid.</param>
                    /// <returns type="Csw.composites.linkGrid">A Csw.composites.linkGrid</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'linkGrid');
                    return Csw.composites.linkGrid(cswParent, opts);
                };

                cswParent.menu = function (opts) {
                    /// <summary> Creates a Csw.menu on this element</summary>
                    /// <param name="opts" type="Object">Options to define the menu.</param>
                    /// <returns type="Csw.composites.menu">A Csw.composites.menu</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'menu');
                    return Csw.composites.menu(cswParent, opts);
                };
                
                return cswParent;
            });


} ());


