/// <reference path="~/js/ChemSW-vsdoc.js" />
/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />

(function () {

    Csw.controls.factory = Csw.controls.factory ||
        Csw.controls.register('factory', function ($element, external) {
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
                opts.parent = function () {
                    return external;
                };
                return opts;
            };

            internal.controlPostProcessing = function (opts, controlName) {
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
                    controlName === 'tabDiv'
                 ) {
                    Csw.components.factory(external, opts);
                }
                return opts;
            };

            internal.makeControlForChaining = function (opts, controlName, $jqElement) {
                /* To support faux children(), find(), first() conventions from the jQuery-verse. The return is not a true Csw control object. Beware! */
                var ret = {};
                opts = internal.controlPreProcessing(opts, controlName);
                if (controlName === 'jquery') {
                    ret = Csw.controls.factory($jqElement, opts);
                } else {
                    ret = Csw.controls[controlName](opts);
                }
                internal.controlPostProcessing(ret, controlName);
                return ret;
            };

            if (Csw.isJQuery($element)) {
                external = Csw.dom(external, $element);
                internal.controlPostProcessing(external);
            } else {
                internal.id = '';
                external.$ = {};
            }

            //#endregion internal

            //#region Csw DOM classes

            external.br = function (opts) {
                /// <summary> Creates a Csw.br on this element</summary>
                /// <param name="opts" type="Object">Options to define the br.</param>
                /// <returns type="Csw.controls.br">A Csw.controls.br</returns> 
                return internal.makeControlForChaining(opts, 'br');
            };

            external.button = function (opts) {
                /// <summary> Creates a Csw.button on this element</summary>
                /// <param name="opts" type="Object">Options to define the button.</param>
                /// <returns type="Csw.controls.button">A Csw.controls.button</returns> 
                return internal.makeControlForChaining(opts, 'button');
            };

            external.div = function (opts) {
                /// <summary> Creates a Csw.div on this element</summary>
                /// <param name="opts" type="Object">Options to define the div.</param>
                /// <returns type="Csw.controls.div">A Csw.controls.div</returns> 
                return internal.makeControlForChaining(opts, 'div');
            };

            external.form = function (opts) {
                /// <summary> Creates a Csw.form on this element</summary>
                /// <param name="opts" type="Object">Options to define the form.</param>
                /// <returns type="Csw.controls.form">A Csw.controls.form</returns> 
                return internal.makeControlForChaining(opts, 'form');
            };

            external.img = function (opts) {
                /// <summary> Creates a Csw.img on this element</summary>
                /// <param name="opts" type="Object">Options to define the img.</param>
                /// <returns type="Csw.controls.img">A Csw.controls.img</returns>
                return internal.makeControlForChaining(opts, 'img');
            };

            external.jquery = function ($jqElement, opts) {
                /// <summary> Extend a jQuery object with Csw methods.</summary>
                /// <param name="$element" type="jQuery">Element to extend.</param>
                /// <returns type="Csw.controls.jquery">A Csw.controls.jquery object</returns>
                return internal.makeControlForChaining(opts, 'jquery', $jqElement);
            };

            external.input = function (opts) {
                /// <summary> Creates a Csw.input on this element</summary>
                /// <param name="opts" type="Object">Options to define the input.</param>
                /// <returns type="Csw.controls.input">A Csw.controls.input</returns> 
                return internal.makeControlForChaining(opts, 'input');
            };

            external.link = function (opts) {
                /// <summary> Creates a Csw.link on this element</summary>
                /// <param name="opts" type="Object">Options to define the link.</param>
                /// <returns type="Csw.controls.link">A Csw.controls.link</returns> 
                return internal.makeControlForChaining(opts, 'link');
            };

            external.numberTextBox = function (opts) {
                /// <summary> Creates a Csw.numberTextBox on this element</summary>
                /// <param name="opts" type="Object">Options to define the numberTextBox.</param>
                /// <returns type="Csw.controls.numberTextBox">A Csw.controls.numberTextBox</returns>
                return internal.makeControlForChaining(opts, 'numberTextBox');
            };

            external.ol = function (opts) {
                /// <summary> Creates a Csw.ol on this element</summary>
                /// <param name="opts" type="Object">Options to define the ol.</param>
                /// <returns type="Csw.controls.ol">A Csw.controls.ol</returns> 
                return internal.makeControlForChaining(opts, 'ol');
            };

            external.p = function (opts) {
                /// <summary> Creates a Csw.p on this element</summary>
                /// <param name="opts" type="Object">Options to define the p.</param>
                /// <returns type="Csw.controls.p">A Csw.controls.p</returns>
                return internal.makeControlForChaining(opts, 'p');
            };

            external.select = function (opts) {
                /// <summary> Creates a Csw.select on this element</summary>
                /// <param name="opts" type="Object">Options to define the select.</param>
                /// <returns type="Csw.controls.select">A Csw.controls.select</returns>
                return internal.makeControlForChaining(opts, 'select');
            };

            external.span = function (opts) {
                /// <summary> Creates a Csw.span on this element</summary>
                /// <param name="opts" type="Object">Options to define the span.</param>
                /// <returns type="Csw.controls.span">A Csw.controls.span</returns> 
                return internal.makeControlForChaining(opts, 'span');
            };

            external.tabDiv = function (opts) {
                /// <summary> Creates a Csw.tabDiv on this element</summary>
                /// <param name="opts" type="Object">Options to define the div.</param>
                /// <returns type="Csw.controls.tabDiv">A Csw.controls.tabDiv</returns> 
                return internal.makeControlForChaining(opts, 'tabDiv');
            };

            external.table = function (opts) {
                /// <summary> Creates a Csw.table on this element</summary>
                /// <param name="opts" type="Object">Options to define the table.</param>
                /// <returns type="Csw.controls.table">A Csw.controls.table</returns> 
                return internal.makeControlForChaining(opts, 'table');
            };

            external.textArea = function (opts) {
                /// <summary> Creates a Csw.textArea on this element</summary>
                /// <param name="opts" type="Object">Options to define the textArea.</param>
                /// <returns type="Csw.controls.textArea">A Csw.controls.textArea</returns>
                return internal.makeControlForChaining(opts, 'textArea');
            };

            external.ul = function (opts) {
                /// <summary> Creates a Csw.ul on this element</summary>
                /// <param name="opts" type="Object">Options to define the ul.</param>
                /// <returns type="Csw.controls.ul">A Csw.controls.ul</returns> 
                return internal.makeControlForChaining(opts, 'ul');
            };

            //#endregion Csw DOM classes

            return external;
        });


} ());


