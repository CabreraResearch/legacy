/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.literals.button = Csw.literals.button ||
        Csw.literals.register('button', function (options) {
            'use strict';
            /// <summary> Create or extend an HTML <button /> and return a Csw.button object
            ///     &#10;1 - button(options)
            ///     &#10;2 - button($jqueryElement)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.$parent: An element to attach to.</para>
            /// <para>options.ID: An ID for the button.</para>
            /// <para>options.cssclass: CSS class to asign</para>
            /// <para>options.enabledText: Text to display when enabled</para>
            /// <para>options.disabledText: Text to display when disabled</para>
            /// <para>options.disableOnClick: Disable the button when it's clicked</para>
            /// <para>options.onClick: Event to execute when the button is clicked</para>
            /// <para>options.bindOnEnter: CswParent object bind 'Enter' key to this button.</para>
            /// </param>
            /// <returns type="button">A button object</returns>
            var cswPrivate = {
                $parent: '',
                ID: '',
                enabledText: '',
                disabledText: '',
                cssclass: '',
                hasText: true,
                disableOnClick: true,
                primaryicon: '',
                secondaryicon: '',
                onClick: null,
                isEnabled: true,
                bindOnEnter: {}
            };
            var cswPublic = {};

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options);
                }
                cswPrivate.type = Csw.enums.inputTypes.button;
                var buttonOpt;
                var internalOnClick = Csw.makeDelegate(cswPrivate.onClick);

                function onClick() {
                    var doEnable = function () {
                        cswPublic.enable();
                        Csw.unsubscribe(Csw.enums.events.ajax.globalAjaxStop, doEnable);
                    };
                    /* Case 25810 */
                    if (cswPrivate.isEnabled) {
                        if (cswPrivate.disableOnClick && false === Csw.ajax.ajaxInProgress()) {
                            cswPublic.disable();
                            Csw.subscribe(Csw.enums.events.ajax.globalAjaxStop, doEnable);
                        }
                        Csw.tryExec(internalOnClick, arguments);
                    }
                }

                cswPrivate.onClick = onClick;

                Csw.extend(cswPublic, Csw.literals.input(cswPrivate));

                if(false === Csw.isNullOrEmpty(cswPrivate.bindOnEnter)) {
                    cswPrivate.bindOnEnter.clickOnEnter(cswPublic);
                }
                
                cswPublic.propNonDom({
                    enabledText: cswPrivate.enabledText,
                    disabledText: cswPrivate.disabledText
                });

                if (false === Csw.isNullOrEmpty(cswPrivate.cssclass)) {
                    cswPublic.addClass(cswPrivate.cssclass);
                }

                buttonOpt = {
                    text: Csw.bool(cswPrivate.hasText),
                    label: cswPrivate.enabledText,
                    disabled: Csw.bool(cswPrivate.ReadOnly) || false === Csw.bool(cswPrivate.isEnabled),
                    icons: {
                        primary: cswPrivate.primaryicon,
                        secondary: cswPrivate.secondaryicon
                    }
                };
                if (buttonOpt.disabled && false === Csw.isNullOrEmpty(cswPrivate.disabledText)) {
                    buttonOpt.label = cswPrivate.disabledText;
                }
                cswPublic.$.button(buttonOpt);

            } ());


            
            cswPublic.enable = function () {
                /// <summary>Enable the button.</summary>
                /// <returns type="button">The button object.</returns>
                cswPrivate.isEnabled = true;
                if (cswPublic.length() > 0) {
                    cswPublic.$.button({
                        label: cswPublic.propNonDom('enabledText'),
                        disabled: false
                    });
                }
                return cswPublic;
            };
            cswPublic.disable = function () {
                /// <summary>Disable the button.</summary>
                /// <returns type="button">The button object.</returns>
                cswPrivate.isEnabled = false;
                if (cswPublic.length() > 0) {
                    cswPublic.$.button({
                        label: cswPublic.propNonDom('disabledText'),
                        disabled: true
                    });
                }
                return cswPublic;
            };

            cswPublic.click = function (func) {
                /// <summary>Trigger or assign a button click event.</summary>
                /// <param name="func" type="Function">(Optional) A function to bind to the control.</param>
                /// <returns type="button">The button object.</returns>
                if (Csw.isFunction(func)) {
                    cswPublic.bind('click', func);
                } else {
                    if (cswPrivate.isEnabled) {
                        cswPublic.trigger('click');
                    }
                }
                return cswPublic;
            };


            return cswPublic;
        });

} ());

