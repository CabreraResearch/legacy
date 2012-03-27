/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {
    
    Csw.literals.button = Csw.literals.button ||
        Csw.literals.register('button', function(options) {
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
            /// </param>
            /// <returns type="button">A button object</returns>
            var internal = {
                $parent: '',
                ID: '',
                enabledText: '',
                disabledText: '',
                cssclass: '',
                hasText: true,
                disableOnClick: true,
                primaryicon: '',
                secondaryicon: '',
                onClick: null
            };
            var external = { };

            external.enable = function() {
                /// <summary>Enable the button.</summary>
                /// <returns type="button">The button object.</returns>
                if (external.length() > 0) {
                    external.$.button({
                        label: external.propNonDom('enabledText'),
                        disabled: false
                    });
                }
                return external;
            };
            external.disable = function() {
                /// <summary>Disable the button.</summary>
                /// <returns type="button">The button object.</returns>
                if (external.length() > 0) {
                    external.$.button({
                        label: external.propNonDom('disabledText'),
                        disabled: true
                    });
                }
                return external;
            };

            external.click = function(func) {
                /// <summary>Trigger or assign a button click event.</summary>
                /// <param name="func" type="Function">(Optional) A function to bind to the control.</param>
                /// <returns type="button">The button object.</returns>
                if (Csw.isFunction(func)) {
                    external.bind('click', func);
                } else {
                    external.trigger('click');
                }
                return external;
            };

            (function() {
                if (options) {
                    $.extend(internal, options);
                }
                internal.type = Csw.enums.inputTypes.button;
                var buttonOpt;
                var internalOnClick = Csw.makeDelegate(internal.onClick);

                function onClick() {
                    var doEnable = function() {
                        external.enable();
                        Csw.unsubscribe(Csw.enums.events.ajax.globalAjaxStop, doEnable);
                    };
                    if (internal.disableOnClick && false === Csw.ajax.ajaxInProgress()) {
                        external.disable();
                        Csw.subscribe(Csw.enums.events.ajax.globalAjaxStop, doEnable);
                    }
                    Csw.tryExec(internalOnClick, arguments);
                }

                internal.onClick = onClick;

                $.extend(external, Csw.literals.input(internal));


                external.propNonDom({
                    enabledText: internal.enabledText,
                    disabledText: internal.disabledText
                });

                if (false === Csw.isNullOrEmpty(internal.cssclass)) {
                    external.addClass(internal.cssclass);
                }

                buttonOpt = {
                    text: (internal.hasText),
                    label: internal.enabledText,
                    disabled: (internal.ReadOnly),
                    icons: {
                        primary: internal.primaryicon,
                        secondary: internal.secondaryicon
                    }
                };
                if (buttonOpt.disabled) {
                    buttonOpt.label = internal.disabledText;
                }
                external.$.button(buttonOpt);

            }());

            return external;
        });

} ());

