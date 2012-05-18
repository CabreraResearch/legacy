/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.literals.factory = Csw.literals.factory ||
        Csw.literals.register('factory', function ($element, cswPublicRet) {
            /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
            /// <param name="$element" type="jQuery">An element to bind to.</param>
            /// <param name="options" type="Object">An options collection to extend.</param>
            /// <returns type="Object">The options object with DOM methods attached.</returns> 
            'use strict';
            //#region cswPrivateVar

            var cswPrivateVar = {};
            cswPublicRet = cswPublicRet || {};

            cswPrivateVar.controlPreProcessing = function (opts, controlName) {
                /* 
                This is our last chance to capture context for chaining. 
                Reference useful relationships. 
                TODO: We must fix this to allow Csw style children() as well as parent()
                */
                opts = opts || {};
                opts.controlName = controlName;
                opts.$parent = $element;
                opts.root = cswPublicRet.root;
                if (opts.suffix) {
                    opts.ID = Csw.makeId(cswPublicRet.getId(), opts.suffix);
                } else if (Csw.isNullOrEmpty(opts.ID) && false === Csw.isNullOrEmpty(cswPublicRet.getId())) {
                    opts.ID = Csw.makeId(cswPublicRet.getId(), controlName);
                }
                if (false === Csw.isNullOrEmpty(opts.labelText)) {
                    cswPublicRet.label({ forAttr: opts.ID, text: opts.labelText, useWide: opts.useWide });
                }
                opts.parent = function () {
                    return cswPublicRet;
                };
                return opts;
            };

            cswPrivateVar.controlPostProcessing = function (componentParent, controlName) {
                /* If it's possible/desirable to attach a complex component to a simple control, then extend */
                if (Csw.isNullOrEmpty(controlName) ||
                    controlName === 'div' ||
                    controlName === 'span' ||
                    controlName === 'p' ||
                    controlName === 'form' ||
                    controlName === 'jquery' ||
                    controlName === 'div' ||
                    controlName === 'ol' ||
                    controlName === 'ul' ||
                    controlName === 'table' ||
                    controlName === 'label' ||
                    controlName === 'tabDiv') {
                    Csw.controls.factory(componentParent, controlName);
                    Csw.composites.factory(componentParent, controlName);
                }
                return componentParent;
            };

            cswPrivateVar.makeControlForChaining = function (opts, controlName, $jqElement) {
                /* To support faux children(), find(), first() conventions from the jQuery-verse. The return is not a true Csw control object. Beware! */
                var ret = {};
                opts = cswPrivateVar.controlPreProcessing(opts, controlName);
                if (controlName === 'jquery') {
                    ret = Csw.literals.factory($jqElement, opts);
                } else {
                    ret = Csw.literals[controlName](opts);
                    ret.controlName = controlName;
                }
                cswPrivateVar.controlPostProcessing(ret, controlName);
                return ret;
            };

            if (Csw.isJQuery($element)) {
                cswPublicRet = Csw.dom(cswPublicRet, $element);
                cswPrivateVar.controlPostProcessing(cswPublicRet);
            } else {
                throw new Error('Cannot directly instance a literals factory without a jQuery element.');
            }

            //#endregion cswPrivateVar

            //#region Csw DOM classes

            cswPublicRet.a = function (opts) {
                /// <summary> Creates a Csw.a on this element</summary>
                /// <param name="opts" type="Object">Options to define the a.</param>
                /// <returns type="Csw.literals.a">A Csw.literals.a</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'a');
            };

            cswPublicRet.b = function (opts) {
                /// <summary> Creates a Csw.b on this element</summary>
                /// <param name="opts" type="Object">Options to define the b.</param>
                /// <returns type="Csw.literals.b">A Csw.literals.b</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'b');
            };

            cswPublicRet.br = function (opts) {
                /// <summary> Creates a Csw.br on this element</summary>
                /// <param name="opts" type="Object">Options to define the br.</param>
                /// <returns type="Csw.literals.br">A Csw.literals.br</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'br');
            };

            cswPublicRet.button = function (opts) {
                /// <summary> Creates a Csw.button on this element</summary>
                /// <param name="opts" type="Object">Options to define the button.</param>
                /// <returns type="Csw.literals.button">A Csw.literals.button</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'button');
            };

            cswPublicRet.div = function (opts) {
                /// <summary> Creates a Csw.div on this element</summary>
                /// <param name="opts" type="Object">Options to define the div.</param>
                /// <returns type="Csw.literals.div">A Csw.literals.div</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'div');
            };

            cswPublicRet.form = function (opts) {
                /// <summary> Creates a Csw.form on this element</summary>
                /// <param name="opts" type="Object">Options to define the form.</param>
                /// <returns type="Csw.literals.form">A Csw.literals.form</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'form');
            };

            cswPublicRet.img = function (opts) {
                /// <summary> Creates a Csw.img on this element</summary>
                /// <param name="opts" type="Object">Options to define the img.</param>
                /// <returns type="Csw.literals.img">A Csw.literals.img</returns>
                return cswPrivateVar.makeControlForChaining(opts, 'img');
            };

            cswPublicRet.jquery = function ($jqElement, opts) {
                /// <summary> Extend a jQuery object with Csw methods.</summary>
                /// <param name="$element" type="jQuery">Element to extend.</param>
                /// <returns type="Csw.literals.jquery">A Csw.literals.jquery object</returns>
                return cswPrivateVar.makeControlForChaining(opts, 'jquery', $jqElement);
            };

            cswPublicRet.input = function (opts) {
                /// <summary> Creates a Csw.input on this element</summary>
                /// <param name="opts" type="Object">Options to define the input.</param>
                /// <returns type="Csw.literals.input">A Csw.literals.input</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'input');
            };

            cswPublicRet.label = function (opts) {
                /// <summary> Creates a Csw.label on this element</summary>
                /// <param name="opts" type="Object">Options to define the label.</param>
                /// <returns type="Csw.literals.label">A Csw.literals.label</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'label');
            };

            /* Case 25125: This literal is deprecated. Use composite instead. */
            cswPublicRet.moreDiv = function (opts) {
                /// <summary> (Deprecated) Creates a Csw.moreDiv on this element</summary>
                /// <param name="opts" type="Object">Options to define the moreDiv.</param>
                /// <returns type="Csw.literals.moreDiv">A Csw.literals.moreDiv</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'moreDiv');
            };

            cswPublicRet.ol = function (opts) {
                /// <summary> Creates a Csw.ol on this element</summary>
                /// <param name="opts" type="Object">Options to define the ol.</param>
                /// <returns type="Csw.literals.ol">A Csw.literals.ol</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'ol');
            };

            cswPublicRet.p = function (opts) {
                /// <summary> Creates a Csw.p on this element</summary>
                /// <param name="opts" type="Object">Options to define the p.</param>
                /// <returns type="Csw.literals.p">A Csw.literals.p</returns>
                return cswPrivateVar.makeControlForChaining(opts, 'p');
            };

            cswPublicRet.select = function (opts) {
                /// <summary> Creates a Csw.select on this element</summary>
                /// <param name="opts" type="Object">Options to define the select.</param>
                /// <returns type="Csw.literals.select">A Csw.literals.select</returns>
                return cswPrivateVar.makeControlForChaining(opts, 'select');
            };

            cswPublicRet.span = function (opts) {
                /// <summary> Creates a Csw.span on this element</summary>
                /// <param name="opts" type="Object">Options to define the span.</param>
                /// <returns type="Csw.literals.span">A Csw.literals.span</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'span');
            };

            /* Case 25125: This literal is deprecated. Use composite instead. */
            cswPublicRet.table = function (opts) {
                /// <summary> (Deprecated) Creates a Csw.table on this element</summary>
                /// <param name="opts" type="Object">Options to define the table.</param>
                /// <returns type="Csw.literals.table">A Csw.literals.table</returns>
                return cswPrivateVar.makeControlForChaining(opts, 'table');
            };

            cswPublicRet.textArea = function (opts) {
                /// <summary> Creates a Csw.textArea on this element</summary>
                /// <param name="opts" type="Object">Options to define the textArea.</param>
                /// <returns type="Csw.literals.textArea">A Csw.literals.textArea</returns>
                return cswPrivateVar.makeControlForChaining(opts, 'textArea');
            };

            cswPublicRet.ul = function (opts) {
                /// <summary> Creates a Csw.ul on this element</summary>
                /// <param name="opts" type="Object">Options to define the ul.</param>
                /// <returns type="Csw.literals.ul">A Csw.literals.ul</returns> 
                return cswPrivateVar.makeControlForChaining(opts, 'ul');
            };

            //#endregion Csw DOM classes

            return cswPublicRet;
        });
} ());


