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

                cswParent.buttonExt = function (opts) {
                    /// <summary> Creates a Csw.buttonExt on this element</summary>
                    /// <param name="opts" type="Object">Options to define the buttonExt.</param>
                    /// <returns type="Csw.controls.buttonExt">A Csw.controls.buttonExt</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'buttonExt');
                    return Csw.controls.buttonExt(cswParent, opts);
                };

                cswParent.buttonGroup = function (opts) {
                    /// <summary> Creates a Csw.buttonGroup on this element</summary>
                    /// <param name="opts" type="Object">Options to define the buttonGroup.</param>
                    /// <returns type="Csw.controls.buttonGroup">A Csw.controls.buttonGroup</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'buttonGroup');
                    return Csw.controls.buttonGroup(cswParent, opts);
                };

                cswParent.CASNoTextBox = function (opts) {
                    /// <summary> Creates a Csw.CASNoTextBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the CASNoTextBox.</param>
                    /// <returns type="Csw.controls.CASNoTextBox">A Csw.controls.CASNoTextBox</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'CASNoTextBox');
                    return Csw.controls.CASNoTextBox(cswParent, opts);
                };

                cswParent.checkBox = function (opts) {
                    /// <summary> Creates a Csw.checkBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the checkBox.</param>
                    /// <returns type="Csw.controls.checkBox">A Csw.controls.checkBox</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'checkBox');
                    return Csw.controls.checkBox(cswParent, opts);
                };

                cswParent.checkBoxArray = function (opts) {
                    /// <summary> Creates a Csw.checkBoxArray on this element</summary>
                    /// <param name="opts" type="Object">Options to define the checkBoxArray.</param>
                    /// <returns type="Csw.controls.checkBoxArray">A Csw.controls.checkBoxArray</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'checkBoxArray');
                    return Csw.controls.checkBoxArray(cswParent, opts);
                };

                cswParent.comboBox = function (opts) {
                    /// <summary> Creates a Csw.comboBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the comboBox.</param>
                    /// <returns type="Csw.controls.comboBox">A Csw.controls.comboBox</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'comboBox');
                    return Csw.controls.comboBox(cswParent, opts);
                };

                cswParent.dateTimePicker = function (opts) {
                    /// <summary> Creates a Csw.dateTimePicker on this element</summary>
                    /// <param name="opts" type="Object">Options to define the dateTimePicker.</param>
                    /// <returns type="Csw.controls.dateTimePicker">A Csw.controls.dateTimePicker</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'dateTimePicker');
                    return Csw.controls.dateTimePicker(cswParent, opts);
                };

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

                cswParent.icon = function (opts) {
                    /// <summary> Creates a Csw.icon on this element</summary>
                    /// <param name="opts" type="Object">Options to define the icon.</param>
                    /// <returns type="Csw.controls.icon">A Csw.controls.icon</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'icon');
                    return Csw.controls.icon(cswParent, opts);
                };

                cswParent.imageButton = function (opts) {
                    /// <summary> Creates a Csw.imageButton on this element</summary>
                    /// <param name="opts" type="Object">Options to define the imageButton.</param>
                    /// <returns type="Csw.controls.imageButton">A Csw.controls.imageButton</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'imageButton');
                    return Csw.controls.imageButton(cswParent, opts);
                };

                cswParent.imageSelect = function (opts) {
                    /// <summary> Creates a Csw.imageSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the imageSelect.</param>
                    /// <returns type="Csw.controls.imageButton">A Csw.controls.imageSelect</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'imageSelect');
                    return Csw.controls.imageSelect(cswParent, opts);
                };
                
                cswParent.layoutTable = function (opts) {
                    /// <summary> Creates a Csw.layoutTable on this element</summary>
                    /// <param name="opts" type="Object">Options to define the table.</param>
                    /// <returns type="Csw.composites.layoutTable">A Csw.composites.layoutTable</returns> 
                    opts = cswPrivate.controlPreProcessing(opts, 'layoutTable');
                    return Csw.composites.layoutTable(cswParent, opts);
                };

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
                
                cswParent.moreDiv = function (opts) {
                    /// <summary> Creates a Csw.multiSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the moreDiv.</param>
                    /// <returns type="Csw.controls.moreDiv">A Csw.controls.moreDiv</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'moreDiv');
                    return Csw.controls.moreDiv(cswParent, opts);
                };

                cswParent.multiSelect = function (opts) {
                    /// <summary> Creates a Csw.multiSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the multiSelect.</param>
                    /// <returns type="Csw.controls.multiSelect">A Csw.controls.multiSelect</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'multiSelect');
                    return Csw.controls.multiSelect(cswParent, opts);
                };

                cswParent.nodeButton = function (opts) {
                    /// <summary> Creates a Csw.nodeButton on this element</summary>
                    /// <param name="opts" type="Object">Options to define the nodeButton.</param>
                    /// <returns type="Csw.controls.nodeButton">A Csw.controls.nodeButton</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'nodeButton');
                    return Csw.controls.nodeButton(cswParent, opts);
                };

                cswParent.nodeSelect = function (opts) {
                    /// <summary> Creates a Csw.nodeSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the nodeSelect.</param>
                    /// <returns type="Csw.controls.nodeSelect">A Csw.controls.nodeSelect</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'nodeSelect');
                    return Csw.controls.nodeSelect(cswParent, opts);
                };

                cswParent.nodeLink = function (opts) {
                    /// <summary> Creates a Csw.nodeSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the nodeSelect.</param>
                    /// <returns type="Csw.controls.nodeSelect">A Csw.controls.nodeSelect</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'nodeLink');
                    return Csw.controls.nodeLink(cswParent, opts);
                };

                cswParent.nodeTypeSelect = function (opts) {
                    /// <summary> Creates a Csw.nodeTypeSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the nodeTypeSelect.</param>
                    /// <returns type="Csw.controls.nodeTypeSelect">A Csw.controls.nodeTypeSelect</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'nodeTypeSelect');
                    return Csw.controls.nodeTypeSelect(cswParent, opts);
                };

                cswParent.numberTextBox = function (opts) {
                    /// <summary> Creates a Csw.numberTextBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the numberTextBox.</param>
                    /// <returns type="Csw.controls.numberTextBox">A Csw.controls.numberTextBox</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'numberTextBox');
                    return Csw.controls.numberTextBox(cswParent, opts);
                };

                cswParent.quantity = function (opts) {
                    /// <summary> Creates a Csw.quantity on this element</summary>
                    /// <param name="opts" type="Object">Options to define the quantity.</param>
                    /// <returns type="Csw.composites.quantity">A Csw.composites.quantity</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'quantity');
                    return Csw.composites.quantity(cswParent, opts);
                };

                cswParent.quickTip = function (opts) {
                    /// <summary> Creates a Csw.quickTip on this element</summary>
                    /// <param name="opts" type="Object">Options to define the quickTip.</param>
                    /// <returns type="Csw.controls.quickTip">A Csw.controls.quickTip</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'quickTip');
                    return Csw.controls.quickTip(cswParent, opts);
                };

                cswParent.radiobutton = function (opts) {
                    /// <summary> Creates a Csw.radiobutton on this element</summary>
                    /// <param name="opts" type="Object">Options to define the radiobutton.</param>
                    /// <returns type="Csw.controls.radiobutton">A Csw.controls.radiobutton</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'radiobutton');
                    return Csw.controls.radiobutton(cswParent, opts);
                };
                
                cswParent.schedRulesTimeline = function (opts) {
                    /// <summary> Creates a Csw.schedRulesTimeline on this element</summary>
                    /// <param name="opts" type="Object">Options to define the graph.</param>
                    /// <returns type="Csw.composites.schedRulesTimeline">A Csw.composites.schedRulesTimeline</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'schedRulesTimeline');
                    return Csw.composites.schedRulesTimeline(cswParent, opts);
                };

                cswParent.tabDiv = function (opts) {
                    /// <summary> Creates a Csw.tabDiv on this element</summary>
                    /// <param name="opts" type="Object">Options to define the div.</param>
                    /// <returns type="Csw.controls.tabDiv">A Csw.literals.tabDiv</returns> 
                    opts = cswPrivate.controlPreProcessing(opts, 'tabDiv');
                    return Csw.controls.tabDiv(cswParent, opts);
                };

                cswParent.tabSelect = function (opts) {
                    /// <summary> Creates a Csw.tabSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the tabSelect.</param>
                    /// <returns type="Csw.controls.tabSelect">A Csw.controls.tabSelect</returns> 
                    opts = cswPrivate.controlPreProcessing(opts, 'tabSelect');
                    return Csw.controls.tabSelect(cswParent, opts);
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

                cswParent.timeInterval = function (opts) {
                    /// <summary> Creates a Csw.timeInterval on this element</summary>
                    /// <param name="opts" type="Object">Options to define the timeInterval.</param>
                    /// <returns type="Csw.controls.timeInterval">A Csw.controls.timeInterval</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'timeInterval');
                    return Csw.controls.timeInterval(cswParent, opts);
                };

                cswParent.tree = function (opts) {
                    /// <summary> Creates a Csw.tree on this element</summary>
                    /// <param name="opts" type="Object">Options to define the tree.</param>
                    /// <returns type="Csw.composites.tree">A Csw.composites.tree</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'tree');
                    return Csw.composites.tree(cswParent, opts);
                };

                cswParent.triStateCheckBox = function (opts) {
                    /// <summary> Creates a Csw.triStateCheckBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the triStateCheckBox.</param>
                    /// <returns type="Csw.controls.triStateCheckBox">A Csw.controls.triStateCheckBox</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'triStateCheckBox');
                    return Csw.controls.triStateCheckBox(cswParent, opts);
                };
                
                cswParent.universalSearch = function (opts) {
                    /// <summary> Creates a Csw.universalSearch on this element</summary>
                    /// <param name="opts" type="Object">Options to define the universalSearch.</param>
                    /// <returns type="Csw.composites.universalSearch">A Csw.composites.universalSearch</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'universalSearch');
                    return Csw.composites.universalSearch(cswParent, opts);
                };

                cswParent.viewSelect = function (opts) {
                    /// <summary> Creates a Csw.viewSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the viewSelect.</param>
                    /// <returns type="Csw.controls.viewSelect">A Csw.controls.viewSelect</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'viewSelect');
                    return Csw.controls.viewSelect(cswParent, opts);
                };
                

                //                cswParent.table = function (opts) {
                //                    /// <summary> Creates a Csw.table on this element</summary>
                //                    /// <param name="opts" type="Object">Options to define the table.</param>
                //                    /// <returns type="Csw.composites.table">A Csw.composites.table</returns> 
                //                    opts = cswPrivate.controlPreProcessing(opts, 'table');
                //                    return Csw.composites.table(cswParent, opts);
                //                };
                
                return cswParent;
            });


}());


