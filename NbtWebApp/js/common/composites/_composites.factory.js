/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.composites.factory = Csw.composites.factory ||
        Csw.composites.register('factory',
            function (cswParent) {
                /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
                /// <param name="cswParent" type="Csw.literals">An Csw Control to bind to.</param>
                /// <returns type="Csw.composites">The options object with DOM methods attached.</returns> 
                'use strict';
                //#region internal

                var internal = {};
                if (Csw.isNullOrEmpty(cswParent)) {
                    throw new Error('Cannot create a Csw component without a Csw control');
                }

                internal.controlPreProcessing = function (opts, controlName) {
                    opts = opts || {};
                    return opts;
                };

                //#endregion internal

                //#region Csw DOM classes

                cswParent.checkBoxArray = function (opts) {
                    /// <summary> Creates a Csw.checkBoxArray on this element</summary>
                    /// <param name="opts" type="Object">Options to define the checkBoxArray.</param>
                    /// <returns type="Csw.composites.checkBoxArray">A Csw.composites.checkBoxArray</returns>
                    opts = internal.controlPreProcessing(opts, 'checkBoxArray');
                    return Csw.composites.checkBoxArray(cswParent, opts);
                };

                cswParent.comboBox = function (opts) {
                    /// <summary> Creates a Csw.comboBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the comboBox.</param>
                    /// <returns type="Csw.composites.comboBox">A Csw.composites.comboBox</returns>
                    opts = internal.controlPreProcessing(opts, 'comboBox');
                    return Csw.composites.comboBox(cswParent, opts);
                };

                cswParent.dateTimePicker = function (opts) {
                    /// <summary> Creates a Csw.dateTimePicker on this element</summary>
                    /// <param name="opts" type="Object">Options to define the dateTimePicker.</param>
                    /// <returns type="Csw.composites.dateTimePicker">A Csw.composites.dateTimePicker</returns>
                    opts = internal.controlPreProcessing(opts, 'dateTimePicker');
                    return Csw.composites.dateTimePicker(cswParent, opts);
                };

                cswParent.grid = function (opts) {
                    /// <summary> Creates a Csw.grid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the grid.</param>
                    /// <returns type="Csw.composites.grid">A Csw.composites.grid</returns>
                    opts = internal.controlPreProcessing(opts, 'grid');
                    return Csw.composites.grid(cswParent, opts);
                };

                cswParent.imageButton = function (opts) {
                    /// <summary> Creates a Csw.imageButton on this element</summary>
                    /// <param name="opts" type="Object">Options to define the imageButton.</param>
                    /// <returns type="Csw.composites.imageButton">A Csw.composites.imageButton</returns>
                    opts = internal.controlPreProcessing(opts, 'imageButton');
                    return Csw.composites.imageButton(cswParent, opts);
                };

                cswParent.layoutTable = function (opts) {
                    /// <summary> Creates a Csw.layoutTable on this element</summary>
                    /// <param name="opts" type="Object">Options to define the table.</param>
                    /// <returns type="Csw.composites.layoutTable">A Csw.composites.layoutTable</returns> 
                    opts = internal.controlPreProcessing(opts, 'layoutTable');
                    return Csw.composites.layoutTable(cswParent, opts);
                };

                cswParent.multiSelect = function (opts) {
                    /// <summary> Creates a Csw.multiSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the multiSelect.</param>
                    /// <returns type="Csw.composites.multiSelect">A Csw.composites.multiSelect</returns>
                    opts = internal.controlPreProcessing(opts, 'multiSelect');
                    return Csw.composites.multiSelect(cswParent, opts);
                };

                cswParent.nodeTypeSelect = function (opts) {
                    /// <summary> Creates a Csw.numberTextBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the numberTextBox.</param>
                    /// <returns type="Csw.composites.nodeTypeSelect">A Csw.composites.nodeTypeSelect</returns>
                    opts = internal.controlPreProcessing(opts, 'nodeTypeSelect');
                    return Csw.composites.nodeTypeSelect(cswParent, opts);
                };

                cswParent.thinGrid = function (opts) {
                    /// <summary> Creates a Csw.thinGrid on this element</summary>
                    /// <param name="opts" type="Object">Options to define the thinGrid.</param>
                    /// <returns type="Csw.composites.thinGrid">A Csw.composites.thinGrid</returns>
                    opts = internal.controlPreProcessing(opts, 'thinGrid');
                    return Csw.composites.thinGrid(cswParent, opts);
                };

                cswParent.timeInterval = function (opts) {
                    /// <summary> Creates a Csw.timeInterval on this element</summary>
                    /// <param name="opts" type="Object">Options to define the timeInterval.</param>
                    /// <returns type="Csw.composites.timeInterval">A Csw.composites.timeInterval</returns>
                    opts = internal.controlPreProcessing(opts, 'timeInterval');
                    return Csw.composites.timeInterval(cswParent, opts);
                };

                cswParent.triStateCheckBox = function (opts) {
                    /// <summary> Creates a Csw.triStateCheckBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the triStateCheckBox.</param>
                    /// <returns type="Csw.composites.triStateCheckBox">A Csw.composites.triStateCheckBox</returns>
                    opts = internal.controlPreProcessing(opts, 'triStateCheckBox');
                    return Csw.composites.triStateCheckBox(cswParent, opts);
                };

                cswParent.viewSelect = function (opts) {
                    /// <summary> Creates a Csw.viewSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the viewSelect.</param>
                    /// <returns type="Csw.composites.viewSelect">A Csw.composites.viewSelect</returns>
                    opts = internal.controlPreProcessing(opts, 'viewSelect');
                    return Csw.composites.viewSelect(cswParent, opts);
                };

                //#endregion Csw DOM classes

                return cswParent;
            });


} ());


