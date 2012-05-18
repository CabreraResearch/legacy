/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

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
            var cswPrivateVar = {
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
            var cswPublicRet = {};

            cswPublicRet.enable = function () {
                /// <summary>Enable the button.</summary>
                /// <returns type="button">The button object.</returns>
                cswPrivateVar.isEnabled = true;
                if (cswPublicRet.length() > 0) {
                    cswPublicRet.$.button({
                        label: cswPublicRet.propNonDom('enabledText'),
                        disabled: false
                    });
                }
                return cswPublicRet;
            };
            cswPublicRet.disable = function () {
                /// <summary>Disable the button.</summary>
                /// <returns type="button">The button object.</returns>
                cswPrivateVar.isEnabled = false;
                if (cswPublicRet.length() > 0) {
                    cswPublicRet.$.button({
                        label: cswPublicRet.propNonDom('disabledText'),
                        disabled: true
                    });
                }
                return cswPublicRet;
            };

            cswPublicRet.click = function (func) {
                /// <summary>Trigger or assign a button click event.</summary>
                /// <param name="func" type="Function">(Optional) A function to bind to the control.</param>
                /// <returns type="button">The button object.</returns>
                if (Csw.isFunction(func)) {
                    cswPublicRet.bind('click', func);
                } else {
                    if (cswPrivateVar.isEnabled) {
                        cswPublicRet.trigger('click');
                    }
                }
                return cswPublicRet;
            };

            (function () {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }
                cswPrivateVar.type = Csw.enums.inputTypes.button;
                var buttonOpt;
                var internalOnClick = Csw.makeDelegate(cswPrivateVar.onClick);

                function onClick() {
                    var doEnable = function () {
                        cswPublicRet.enable();
                        Csw.unsubscribe(Csw.enums.events.ajax.globalAjaxStop, doEnable);
                    };
                    /* Case 25810 */
                    if (cswPrivateVar.isEnabled) {
                        if (cswPrivateVar.disableOnClick && false === Csw.ajax.ajaxInProgress()) {
                            cswPublicRet.disable();
                            Csw.subscribe(Csw.enums.events.ajax.globalAjaxStop, doEnable);
                        }
                        Csw.tryExec(internalOnClick, arguments);
                    }
                }

                cswPrivateVar.onClick = onClick;

                $.extend(cswPublicRet, Csw.literals.input(cswPrivateVar));

                if(false === Csw.isNullOrEmpty(cswPrivateVar.bindOnEnter)) {
                    cswPrivateVar.bindOnEnter.clickOnEnter(cswPublicRet);
                }
                
                cswPublicRet.propNonDom({
                    enabledText: cswPrivateVar.enabledText,
                    disabledText: cswPrivateVar.disabledText
                });

                if (false === Csw.isNullOrEmpty(cswPrivateVar.cssclass)) {
                    cswPublicRet.addClass(cswPrivateVar.cssclass);
                }

                buttonOpt = {
                    text: (cswPrivateVar.hasText),
                    label: cswPrivateVar.enabledText,
                    disabled: (cswPrivateVar.ReadOnly),
                    icons: {
                        primary: cswPrivateVar.primaryicon,
                        secondary: cswPrivateVar.secondaryicon
                    }
                };
                if (buttonOpt.disabled) {
                    buttonOpt.label = cswPrivateVar.disabledText;
                }
                cswPublicRet.$.button(buttonOpt);

            } ());

            return cswPublicRet;
        });

} ());

