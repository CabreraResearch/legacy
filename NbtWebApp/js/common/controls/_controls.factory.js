/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
                    Csw.error.throwException('Cannot create a Csw component without a Csw control', '_controls.factory', '_controls.factory.js', 14);
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

                cswParent.buttonGroup = function (opts) {
                    /// <summary> Creates a Csw.buttonGroup on this element</summary>
                    /// <param name="opts" type="Object">Options to define the buttonGroup.</param>
                    /// <returns type="Csw.controls.buttonGroup">A Csw.controls.buttonGroup</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'buttonGroup');
                    return Csw.controls.buttonGroup(cswParent, opts);
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

                cswParent.radiobutton = function (opts) {
                    /// <summary> Creates a Csw.radiobutton on this element</summary>
                    /// <param name="opts" type="Object">Options to define the radiobutton.</param>
                    /// <returns type="Csw.controls.radiobutton">A Csw.controls.radiobutton</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'radiobutton');
                    return Csw.controls.radiobutton(cswParent, opts);
                };

                cswParent.tabDiv = function (opts) {
                    /// <summary> Creates a Csw.tabDiv on this element</summary>
                    /// <param name="opts" type="Object">Options to define the div.</param>
                    /// <returns type="Csw.controls.tabDiv">A Csw.literals.tabDiv</returns> 
                    opts = cswPrivate.controlPreProcessing(opts, 'tabDiv');
                    return Csw.controls.tabDiv(cswParent, opts);
                };

                cswParent.timeInterval = function (opts) {
                    /// <summary> Creates a Csw.timeInterval on this element</summary>
                    /// <param name="opts" type="Object">Options to define the timeInterval.</param>
                    /// <returns type="Csw.controls.timeInterval">A Csw.controls.timeInterval</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'timeInterval');
                    return Csw.controls.timeInterval(cswParent, opts);
                };

                cswParent.triStateCheckBox = function (opts) {
                    /// <summary> Creates a Csw.triStateCheckBox on this element</summary>
                    /// <param name="opts" type="Object">Options to define the triStateCheckBox.</param>
                    /// <returns type="Csw.controls.triStateCheckBox">A Csw.controls.triStateCheckBox</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'triStateCheckBox');
                    return Csw.controls.triStateCheckBox(cswParent, opts);
                };

                cswParent.viewSelect = function (opts) {
                    /// <summary> Creates a Csw.viewSelect on this element</summary>
                    /// <param name="opts" type="Object">Options to define the viewSelect.</param>
                    /// <returns type="Csw.controls.viewSelect">A Csw.controls.viewSelect</returns>
                    opts = cswPrivate.controlPreProcessing(opts, 'viewSelect');
                    return Csw.controls.viewSelect(cswParent, opts);
                };

                return cswParent;
            });


} ());


