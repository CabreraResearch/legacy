/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.literals.factory = Csw.literals.factory ||
        Csw.literals.register('factory', function ($element, external) {
            /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
            /// <param name="$element" type="jQuery">An element to bind to.</param>
            /// <param name="options" type="Object">An options collection to extend.</param>
            /// <returns type="Object">The options object with DOM methods attached.</returns> 
            'use strict';
            //#region internal

            var internal = {};
            external = external || {};

            internal.controlPreProcessing = function (opts, controlName) {
                /* 
                This is our last chance to capture context for chaining. 
                Reference useful relationships. 
                TODO: We must fix this to allow Csw style children() as well as parent()
                */
                opts = opts || {};
                opts.controlName = controlName;
                opts.$parent = $element;
                opts.root = external.root;
                if (opts.suffix) {
                    opts.ID = Csw.makeId(external.getId(), opts.suffix);
                } else if (Csw.isNullOrEmpty(opts.ID) && false === Csw.isNullOrEmpty(external.getId())) {
                    opts.ID = Csw.makeId(external.getId(), controlName);
                }
                opts.parent = function () {
                    return external;
                };
                return opts;
            };

            internal.controlPostProcessing = function (componentParent, controlName) {
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

            internal.makeControlForChaining = function (opts, controlName, $jqElement) {
                /* To support faux children(), find(), first() conventions from the jQuery-verse. The return is not a true Csw control object. Beware! */
                var ret = {};
                opts = internal.controlPreProcessing(opts, controlName);
                if (controlName === 'jquery') {
                    ret = Csw.literals.factory($jqElement, opts);
                } else {
                    ret = Csw.literals[controlName](opts);
                }
                internal.controlPostProcessing(ret, controlName);
                return ret;
            };

            if (Csw.isJQuery($element)) {
                external = Csw.dom(external, $element);
                internal.controlPostProcessing(external);
            } else {
                throw new Error('Cannot directly instance a literals factory without a jQuery element.');
            }

            //#endregion internal

            //#region Csw DOM classes

            external.a = function (opts) {
                /// <summary> Creates a Csw.a on this element</summary>
                /// <param name="opts" type="Object">Options to define the a.</param>
                /// <returns type="Csw.literals.a">A Csw.literals.a</returns> 
                return internal.makeControlForChaining(opts, 'a');
            };

            external.b = function (opts) {
                /// <summary> Creates a Csw.b on this element</summary>
                /// <param name="opts" type="Object">Options to define the b.</param>
                /// <returns type="Csw.literals.b">A Csw.literals.b</returns> 
                return internal.makeControlForChaining(opts, 'b');
            };

            external.br = function (opts) {
                /// <summary> Creates a Csw.br on this element</summary>
                /// <param name="opts" type="Object">Options to define the br.</param>
                /// <returns type="Csw.literals.br">A Csw.literals.br</returns> 
                return internal.makeControlForChaining(opts, 'br');
            };

            external.button = function (opts) {
                /// <summary> Creates a Csw.button on this element</summary>
                /// <param name="opts" type="Object">Options to define the button.</param>
                /// <returns type="Csw.literals.button">A Csw.literals.button</returns> 
                return internal.makeControlForChaining(opts, 'button');
            };

            external.div = function (opts) {
                /// <summary> Creates a Csw.div on this element</summary>
                /// <param name="opts" type="Object">Options to define the div.</param>
                /// <returns type="Csw.literals.div">A Csw.literals.div</returns> 
                return internal.makeControlForChaining(opts, 'div');
            };

            external.form = function (opts) {
                /// <summary> Creates a Csw.form on this element</summary>
                /// <param name="opts" type="Object">Options to define the form.</param>
                /// <returns type="Csw.literals.form">A Csw.literals.form</returns> 
                return internal.makeControlForChaining(opts, 'form');
            };

            external.img = function (opts) {
                /// <summary> Creates a Csw.img on this element</summary>
                /// <param name="opts" type="Object">Options to define the img.</param>
                /// <returns type="Csw.literals.img">A Csw.literals.img</returns>
                return internal.makeControlForChaining(opts, 'img');
            };

            external.jquery = function ($jqElement, opts) {
                /// <summary> Extend a jQuery object with Csw methods.</summary>
                /// <param name="$element" type="jQuery">Element to extend.</param>
                /// <returns type="Csw.literals.jquery">A Csw.literals.jquery object</returns>
                return internal.makeControlForChaining(opts, 'jquery', $jqElement);
            };

            external.input = function (opts) {
                /// <summary> Creates a Csw.input on this element</summary>
                /// <param name="opts" type="Object">Options to define the input.</param>
                /// <returns type="Csw.literals.input">A Csw.literals.input</returns> 
                return internal.makeControlForChaining(opts, 'input');
            };

            external.label = function (opts) {
                /// <summary> Creates a Csw.label on this element</summary>
                /// <param name="opts" type="Object">Options to define the label.</param>
                /// <returns type="Csw.literals.label">A Csw.literals.label</returns> 
                return internal.makeControlForChaining(opts, 'label');
            };

            /* Case 25125: This literal is deprecated. Use composite instead. */
            external.moreDiv = function (opts) {
                /// <summary> (Deprecated) Creates a Csw.moreDiv on this element</summary>
                /// <param name="opts" type="Object">Options to define the moreDiv.</param>
                /// <returns type="Csw.literals.moreDiv">A Csw.literals.moreDiv</returns> 
                return internal.makeControlForChaining(opts, 'moreDiv');
            };

            external.ol = function (opts) {
                /// <summary> Creates a Csw.ol on this element</summary>
                /// <param name="opts" type="Object">Options to define the ol.</param>
                /// <returns type="Csw.literals.ol">A Csw.literals.ol</returns> 
                return internal.makeControlForChaining(opts, 'ol');
            };

            external.p = function (opts) {
                /// <summary> Creates a Csw.p on this element</summary>
                /// <param name="opts" type="Object">Options to define the p.</param>
                /// <returns type="Csw.literals.p">A Csw.literals.p</returns>
                return internal.makeControlForChaining(opts, 'p');
            };

            external.select = function (opts) {
                /// <summary> Creates a Csw.select on this element</summary>
                /// <param name="opts" type="Object">Options to define the select.</param>
                /// <returns type="Csw.literals.select">A Csw.literals.select</returns>
                return internal.makeControlForChaining(opts, 'select');
            };

            external.span = function (opts) {
                /// <summary> Creates a Csw.span on this element</summary>
                /// <param name="opts" type="Object">Options to define the span.</param>
                /// <returns type="Csw.literals.span">A Csw.literals.span</returns> 
                return internal.makeControlForChaining(opts, 'span');
            };

            /* Case 25125: This literal is deprecated. Use composite instead. */
            external.table = function (opts) {
                /// <summary> (Deprecated) Creates a Csw.table on this element</summary>
                /// <param name="opts" type="Object">Options to define the table.</param>
                /// <returns type="Csw.literals.table">A Csw.literals.table</returns>
                return internal.makeControlForChaining(opts, 'table');
            };

            external.textArea = function (opts) {
                /// <summary> Creates a Csw.textArea on this element</summary>
                /// <param name="opts" type="Object">Options to define the textArea.</param>
                /// <returns type="Csw.literals.textArea">A Csw.literals.textArea</returns>
                return internal.makeControlForChaining(opts, 'textArea');
            };

            external.ul = function (opts) {
                /// <summary> Creates a Csw.ul on this element</summary>
                /// <param name="opts" type="Object">Options to define the ul.</param>
                /// <returns type="Csw.literals.ul">A Csw.literals.ul</returns> 
                return internal.makeControlForChaining(opts, 'ul');
            };

            //#endregion Csw DOM classes

            return external;
        });
} ());


