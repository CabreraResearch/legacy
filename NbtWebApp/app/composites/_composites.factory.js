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

                var cswPrivate = {
                    count: 0
                };
                if (Csw.isNullOrEmpty(cswParent)) {
                    throw new Error('Cannot create a Csw component without a Csw control');
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

                cswParent.fileUpload = function (opts) {
                    /// <summary> Creates a Csw.fileUpload on this element</summary>
                    /// <param name="opts" type="Object">Options to define the fileUpload.</param>
                    /// <returns type="Csw.composites.grid">A Csw.composites.grid</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'fileUpload');
                    return Csw.composites.fileUpload(cswParent, opts);

                };
                
                cswParent.grid = function (opts) {
                    /// <summary> Creates a Csw.grid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the grid.</param>
                    /// <returns type="Csw.composites.grid">A Csw.composites.grid</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'grid');
                    return Csw.composites.grid(cswParent, opts);

                };

                cswParent.jZebra = function (opts) {
                    /// <summary> Creates a Csw.jZebra on this element</summary>
                    /// <param name="opts" type="Object">Options to define the jZebra.</param>
                    /// <returns type="Csw.composites.jZebra">A Csw.composites.jZebra</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'jZebra');
                    return Csw.composites.jZebra(cswParent, opts);
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

                cswParent.linkGrid = function (opts) {
                    /// <summary> Creates a Csw.linkGrid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the linkGrid.</param>
                    /// <returns type="Csw.composites.linkGrid">A Csw.composites.linkGrid</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'linkGrid');
                    return Csw.composites.linkGrid(cswParent, opts);
                };

                cswParent.location = function (opts) {
                    /// <summary> Creates a Csw.location on this element</summary>
                    /// <param name="opts" type="Object">Options to define the location.</param>
                    /// <returns type="Csw.composites.location">A Csw.composites.location</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'location');
                    return Csw.composites.location(cswParent, opts);
                };

                cswParent.menu = function (opts) {
                    /// <summary> Creates a Csw.menu on this element</summary>
                    /// <param name="opts" type="Object">Options to define the menu.</param>
                    /// <returns type="Csw.composites.menu">A Csw.composites.menu</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'menu');
                    return Csw.composites.menu(cswParent, opts);
                };

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

                cswParent.tabStrip = function (opts) {
                    /// <summary> Creates a Csw.tabStrip on this element</summary>
                    /// <param name="opts" type="Object">Options to define the tabStrip.</param>
                    /// <returns type="Csw.composites.tabStrip">A Csw.composites.tabStrip</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'tabStrip');
                    return Csw.composites.tabStrip(cswParent, opts);
                };


                cswParent.thinGrid = function (opts) {
                    /// <summary> Creates a Csw.thinGrid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the thinGrid.</param>
                    /// <returns type="Csw.composites.thinGrid">A Csw.composites.thinGrid</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'thinGrid');
                    return Csw.composites.thinGrid(cswParent, opts);
                };
                
                cswParent.tree = function (opts) {
                    /// <summary> Creates a Csw.tree on this element</summary>
                    /// <param name="opts" type="Object">Options to define the tree.</param>
                    /// <returns type="Csw.composites.tree">A Csw.composites.tree</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'tree');
                    return Csw.composites.tree(cswParent, opts);
                };

                cswParent.universalSearch = function (opts) {
                    /// <summary> Creates a Csw.universalSearch on this element</summary>
                    /// <param name="opts" type="Object">Options to define the universalSearch.</param>
                    /// <returns type="Csw.composites.universalSearch">A Csw.composites.universalSearch</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'universalSearch');
                    return Csw.composites.universalSearch(cswParent, opts);
                };

                return cswParent;
            });


}());


