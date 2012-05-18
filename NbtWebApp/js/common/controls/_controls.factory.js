/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />
'use strict';
(function () {

    Csw.controls.factory = Csw.controls.factory ||
        Csw.controls.register('factory',
            function (cswParent) {
                /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
                /// <param name="cswParent" type="Csw.literals">An Csw Control to bind to.</param>
                /// <returns type="Csw.controls">The options object with DOM methods attached.</returns> 
                var cswPrivateVar = {};
                if (Csw.isNullOrEmpty(cswParent)) {
                    throw new Error('Cannot create a Csw component without a Csw control');
                }

                cswPrivateVar.controlPreProcessing = function (opts, controlName) {
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

                cswParent.checkBoxArray = function (opts) {
                    /// <summary> Creates a Csw.checkBoxArray on this element</summary>
                    /// <param name="opts" type="Object">Options to define the checkBoxArray.</param>
                    /// <returns type="Csw.controls.checkBoxArray">A Csw.controls.checkBoxArray</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'checkBoxArray');
                    return Csw.controls.checkBoxArray(cswParent, opts);
                };

                cswParent.comboBox = function (opts) {
                    /// <summary> Creates a Csw.comboBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the comboBox.</param>
                    /// <returns type="Csw.controls.comboBox">A Csw.controls.comboBox</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'comboBox');
                    return Csw.controls.comboBox(cswParent, opts);
                };

                cswParent.dateTimePicker = function (opts) {
                    /// <summary> Creates a Csw.dateTimePicker on this element</summary>
                    /// <param name="opts" type="Object">Options to define the dateTimePicker.</param>
                    /// <returns type="Csw.controls.dateTimePicker">A Csw.controls.dateTimePicker</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'dateTimePicker');
                    return Csw.controls.dateTimePicker(cswParent, opts);
                };

                cswParent.imageButton = function (opts) {
                    /// <summary> Creates a Csw.imageButton on this element</summary>
                    /// <param name="opts" type="Object">Options to define the imageButton.</param>
                    /// <returns type="Csw.controls.imageButton">A Csw.controls.imageButton</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'imageButton');
                    return Csw.controls.imageButton(cswParent, opts);
                };

                cswParent.moreDiv = function (opts) {
                    /// <summary> Creates a Csw.multiSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the moreDiv.</param>
                    /// <returns type="Csw.controls.moreDiv">A Csw.controls.moreDiv</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'moreDiv');
                    return Csw.controls.moreDiv(cswParent, opts);
                };

                cswParent.multiSelect = function (opts) {
                    /// <summary> Creates a Csw.multiSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the multiSelect.</param>
                    /// <returns type="Csw.controls.multiSelect">A Csw.controls.multiSelect</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'multiSelect');
                    return Csw.controls.multiSelect(cswParent, opts);
                };

                cswParent.nodeSelect = function (opts) {
                    /// <summary> Creates a Csw.nodeSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the nodeSelect.</param>
                    /// <returns type="Csw.controls.nodeSelect">A Csw.controls.nodeSelect</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'nodeSelect');
                    return Csw.controls.nodeSelect(cswParent, opts);
                };

                cswParent.nodeTypeSelect = function (opts) {
                    /// <summary> Creates a Csw.nodeTypeSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the nodeTypeSelect.</param>
                    /// <returns type="Csw.controls.nodeTypeSelect">A Csw.controls.nodeTypeSelect</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'nodeTypeSelect');
                    return Csw.controls.nodeTypeSelect(cswParent, opts);
                };

                cswParent.numberTextBox = function (opts) {
                    /// <summary> Creates a Csw.numberTextBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the numberTextBox.</param>
                    /// <returns type="Csw.controls.numberTextBox">A Csw.controls.numberTextBox</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'numberTextBox');
                    return Csw.controls.numberTextBox(cswParent, opts);
                };

                cswParent.tabDiv = function (opts) {
                    /// <summary> Creates a Csw.tabDiv on this element</summary>
                    /// <param name="opts" type="Object">Options to define the div.</param>
                    /// <returns type="Csw.controls.tabDiv">A Csw.literals.tabDiv</returns> 
                    opts = cswPrivateVar.controlPreProcessing(opts, 'tabDiv');
                    return Csw.controls.tabDiv(cswParent, opts);
                };

                cswParent.timeInterval = function (opts) {
                    /// <summary> Creates a Csw.timeInterval on this element</summary>
                    /// <param name="opts" type="Object">Options to define the timeInterval.</param>
                    /// <returns type="Csw.controls.timeInterval">A Csw.controls.timeInterval</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'timeInterval');
                    return Csw.controls.timeInterval(cswParent, opts);
                };

                cswParent.triStateCheckBox = function (opts) {
                    /// <summary> Creates a Csw.triStateCheckBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the triStateCheckBox.</param>
                    /// <returns type="Csw.controls.triStateCheckBox">A Csw.controls.triStateCheckBox</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'triStateCheckBox');
                    return Csw.controls.triStateCheckBox(cswParent, opts);
                };

                cswParent.viewSelect = function (opts) {
                    /// <summary> Creates a Csw.viewSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the viewSelect.</param>
                    /// <returns type="Csw.controls.viewSelect">A Csw.controls.viewSelect</returns>
                    opts = cswPrivateVar.controlPreProcessing(opts, 'viewSelect');
                    return Csw.controls.viewSelect(cswParent, opts);
                };

                return cswParent;
            });


} ());


