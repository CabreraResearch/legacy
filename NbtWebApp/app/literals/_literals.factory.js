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

            var cswPrivate = {};
            cswPublic = cswPublic || {};

            cswPrivate.controlPreProcessing = function (opts, controlName) {
                /* 
                This is our last chance to capture context for chaining. 
                Reference useful relationships. 
                TODO: We must fix this to allow Csw style children() as well as parent()
                */
                opts = opts || {};
                opts.controlName = controlName;
                opts.$parent = $element;
                opts.root = cswPublic.root;
                opts.ID = cswPublic.getId() + '_' + controlName;
                if (false === Csw.isNullOrEmpty(opts.labelText)) {
                    cswPublic.label({ forAttr: opts.ID, text: opts.labelText, useWide: opts.useWide });
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

            if (Csw.isJQuery($element)) {
                cswPublic = Csw.dom(cswPublic, $element);
                cswPrivate.controlPostProcessing(cswPublic);
            } else {
                throw new Error('Cannot directly instance a literals factory without a jQuery element.');
            }

            //#endregion cswPrivate

            Csw.each(Csw.literals, function(literal, name) {
                if (false === Csw.contains(Csw, name) &&
                    name !== 'factory') {
                   cswPublic[name] = function(opts) {
                       return cswPrivate.makeControlForChaining(opts, name, $element);
                   };
               }
            });
            
            cswPublic.jquery = function ($jqElement, opts) {
                /// <summary> Extend a jQuery object with Csw methods.</summary>
                /// <param name="$element" type="jQuery">Element to extend.</param>
                /// <returns type="Csw.literals.jquery">A Csw.literals.jquery object</returns>
                return cswPrivate.makeControlForChaining(opts, 'jquery', $jqElement);
            };

            return cswPublic;
        });
} ());


