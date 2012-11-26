/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.literals.factory = Csw.literals.factory ||
        Csw.literals.register('factory', function ($element, cswPublic) {
            /// <summary>Extends a Csw Control class with basic DOM methods.</summary>
            /// <param name="$element" type="jQuery">An element to bind to.</param>
            /// <param name="options" type="Object">An options collection to extend.</param>
            /// <returns type="Object">The options object with DOM methods attached.</returns> 
            'use strict';
            //#region cswPrivate

            var cswPrivate = {
                count: 0
            };
            cswPublic = cswPublic || {};

            cswPrivate.domNodeProcessing = function (control) {
                control = Csw.dom(control);
                cswPrivate.count += 1;
                control.$parent = cswPublic.$.parent();
                control.root = control;
                control.ID = control.getId();
                control.parent = function () {
                    return Csw.domNode(control.$parent[0].id);
                };
                $element = control.$;
                
                cswPrivate.controlPostProcessing(control);
                return control;
            };

            cswPrivate.controlPreProcessing = function (opts, controlName) {
                /* 
                This is our last chance to capture context for chaining. 
                Reference useful relationships. 
                TODO: We must fix this to allow Csw style children() as well as parent()
                */
                cswPrivate.count += 1;
                opts = opts || {};
                opts.controlName = controlName;
                opts.$parent = $element;
                opts.root = cswPublic.root;
                //This seemed like a good idea, but it fails far too often.
                //  Since these failures don't correspond to faults in the page, it's not clear that they are meaningful.
                //Csw.debug.assert(false === Csw.isNullOrEmpty(cswPublic.getId()), 'Parent\'s element id was null or empty.');
                opts.ID = cswPublic.getId();
                if (opts.suffix) {
                    opts.ID += opts.suffix;
                }
                opts.ID += controlName + cswPrivate.count;
                //opts.ID = window.Ext.id();
                if (false === Csw.isNullOrEmpty(opts.labelText)) {
                    cswPublic.label({ forAttr: opts.ID, text: opts.labelText, useWide: opts.useWide, isRequired: opts.isRequired });
                }
                opts.parent = function () {
                    return cswPublic;
                };
                return opts;
            };

            cswPrivate.controlPostProcessing = function (componentParent, controlName) {
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

            cswPrivate.makeControlForChaining = function (opts, controlName, $jqElement) {
                /* To support faux children(), find(), first() conventions from the jQuery-verse. The return is not a true Csw control object. Beware! */
                var ret = {};
                opts = cswPrivate.controlPreProcessing(opts, controlName);
                if (controlName === 'jquery') {
                    ret = Csw.literals.factory($jqElement, opts);
                } else {
                    ret = Csw.literals[controlName](opts);
                    ret.controlName = controlName;
                }
                cswPrivate.controlPostProcessing(ret, controlName);
                return ret;
            };

            if (null === $element && cswPublic.$ && cswPublic[0]) {
                cswPrivate.domNodeProcessing(cswPublic);
            }
            else if (Csw.isJQuery($element)) {
                cswPublic = Csw.dom(cswPublic, $element);
                cswPrivate.controlPostProcessing(cswPublic);
            } else {
                Csw.debug.error('Cannot directly instance a literals factory without a DOM element.');
            }

            //#endregion cswPrivate

            //Csw.each is EXPENSIVE. Do !not! do this until each() is fixed.
            //Csw.each(Csw.literals, function(literal, name) {
            //    if (false === Csw.contains(Csw, name) &&
            //        name !== 'factory') {
            //       cswPublic[name] = function(opts) {
            //           return cswPrivate.makeControlForChaining(opts, name, $element);
            //       };
            //   }
            //});

            cswPublic.a = function (opts) {
                /// <summary> Creates a Csw.a on this element</summary>
                /// <param name="opts" type="Object">Options to define the a.</param>
                /// <returns type="Csw.literals.a">A Csw.literals.a</returns> 
                return cswPrivate.makeControlForChaining(opts, 'a');
            };

            cswPublic.applet = function (opts) {
                /// <summary> Creates a Csw.applet on this element</summary>
                /// <param name="opts" type="Object">Options to define the applet.</param>
                /// <returns type="Csw.literals.applet">A Csw.literals.applet</returns> 
                return cswPrivate.makeControlForChaining(opts, 'applet');
            };

            cswPublic.b = function (opts) {
                /// <summary> Creates a Csw.b on this element</summary>
                /// <param name="opts" type="Object">Options to define the b.</param>
                /// <returns type="Csw.literals.b">A Csw.literals.b</returns> 
                return cswPrivate.makeControlForChaining(opts, 'b');
            };

            cswPublic.br = function (opts) {
                /// <summary> Creates a Csw.br on this element</summary>
                /// <param name="opts" type="Object">Options to define the br.</param>
                /// <returns type="Csw.literals.br">A Csw.literals.br</returns> 
                return cswPrivate.makeControlForChaining(opts, 'br');
            };

            cswPublic.button = function (opts) {
                /// <summary> Creates a Csw.button on this element</summary>
                /// <param name="opts" type="Object">Options to define the button.</param>
                /// <returns type="Csw.literals.button">A Csw.literals.button</returns> 
                return cswPrivate.makeControlForChaining(opts, 'button');
            };

            cswPublic.div = function (opts) {
                /// <summary> Creates a Csw.div on this element</summary>
                /// <param name="opts" type="Object">Options to define the div.</param>
                /// <returns type="Csw.literals.div">A Csw.literals.div</returns> 
                return cswPrivate.makeControlForChaining(opts, 'div');
            };

            cswPublic.fieldSet = function (opts) {
                /// <summary> Creates a Csw.fieldSet on this element</summary>
                /// <param name="opts" type="Object">Options to define the fieldSet.</param>
                /// <returns type="Csw.literals.fieldSet">A Csw.literals.fieldSet</returns> 
                return cswPrivate.makeControlForChaining(opts, 'fieldSet');
            };

            cswPublic.form = function (opts) {
                /// <summary> Creates a Csw.form on this element</summary>
                /// <param name="opts" type="Object">Options to define the form.</param>
                /// <returns type="Csw.literals.form">A Csw.literals.form</returns> 
                return cswPrivate.makeControlForChaining(opts, 'form');
            };

            cswPublic.img = function (opts) {
                /// <summary> Creates a Csw.img on this element</summary>
                /// <param name="opts" type="Object">Options to define the img.</param>
                /// <returns type="Csw.literals.img">A Csw.literals.img</returns>
                return cswPrivate.makeControlForChaining(opts, 'img');
            };

            cswPublic.input = function (opts) {
                /// <summary> Creates a Csw.input on this element</summary>
                /// <param name="opts" type="Object">Options to define the input.</param>
                /// <returns type="Csw.literals.input">A Csw.literals.input</returns> 
                return cswPrivate.makeControlForChaining(opts, 'input');
            };
            
            cswPublic.jquery = function ($jqElement, opts) {
                /// <summary> Extend a jQuery object with Csw methods.</summary>
                return cswPrivate.makeControlForChaining(opts, 'jquery', $jqElement);
            };

            cswPublic.label = function (opts) {
                /// <summary> Creates a Csw.label on this element</summary>
                /// <param name="opts" type="Object">Options to define the label.</param>
                /// <returns type="Csw.literals.label">A Csw.literals.label</returns> 
                return cswPrivate.makeControlForChaining(opts, 'label');
            };

            cswPublic.ol = function (opts) {
                /// <summary> Creates a Csw.ol on this element</summary>
                /// <param name="opts" type="Object">Options to define the ol.</param>
                /// <returns type="Csw.literals.ol">A Csw.literals.ol</returns> 
                return cswPrivate.makeControlForChaining(opts, 'ol');
            };

            cswPublic.p = function (opts) {
                /// <summary> Creates a Csw.p on this element</summary>
                /// <param name="opts" type="Object">Options to define the p.</param>
                /// <returns type="Csw.literals.p">A Csw.literals.p</returns>
                return cswPrivate.makeControlForChaining(opts, 'p');
            };

            cswPublic.select = function (opts) {
                /// <summary> Creates a Csw.select on this element</summary>
                /// <param name="opts" type="Object">Options to define the select.</param>
                /// <returns type="Csw.literals.select">A Csw.literals.select</returns>
                return cswPrivate.makeControlForChaining(opts, 'select');
            };

            cswPublic.span = function (opts) {
                /// <summary> Creates a Csw.span on this element</summary>
                /// <param name="opts" type="Object">Options to define the span.</param>
                /// <returns type="Csw.literals.span">A Csw.literals.span</returns> 
                return cswPrivate.makeControlForChaining(opts, 'span');
            };

            /* Case 25125: This literal is deprecated. Use composite instead. */
            cswPublic.table = function (opts) {
                /// <summary> (Deprecated) Creates a Csw.table on this element</summary>
                /// <param name="opts" type="Object">Options to define the table.</param>
                /// <returns type="Csw.literals.table">A Csw.literals.table</returns>
                return cswPrivate.makeControlForChaining(opts, 'table');
            };

            cswPublic.textArea = function (opts) {
                /// <summary> Creates a Csw.textArea on this element</summary>
                /// <param name="opts" type="Object">Options to define the textArea.</param>
                /// <returns type="Csw.literals.textArea">A Csw.literals.textArea</returns>
                return cswPrivate.makeControlForChaining(opts, 'textArea');
            };

            cswPublic.ul = function (opts) {
                /// <summary> Creates a Csw.ul on this element</summary>
                /// <param name="opts" type="Object">Options to define the ul.</param>
                /// <returns type="Csw.literals.ul">A Csw.literals.ul</returns> 
                return cswPrivate.makeControlForChaining(opts, 'ul');
            };



            return cswPublic;
        });
}());


