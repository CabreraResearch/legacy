/// <reference path="~/app/CswApp-vsdoc.js" />

/// <reference path="~/js/thirdparty/extjs-4.1.0/ext-all-debug.js" />
/// <reference path="~/js/thirdparty/mousetrap.min.js"/>
(function () {

    Csw.controls.buttonExt = Csw.controls.buttonExt ||
        Csw.controls.register('buttonExt', function (cswParent, options) {
            'use strict';
            /// <summary> Create or extend an HTML <button /> and return a Csw.button object
            ///     &#10;1 - button(options)
            ///     &#10;2 - button($jqueryElement)
            ///</summary>
            /// <param name="options" type="Object">
            /// <para>A JSON Object</para>
            /// <para>options.ID: An ID for the button.</para>
            /// <para>options.cssclass: CSS class to asign</para>
            /// <para>options.enabledText: Text to display when enabled</para>
            /// <para>options.disabledText: Text to display when disabled</para>
            /// <para>options.disableOnClick: Disable the button when it's clicked</para>
            /// <para>options.onClick: Event to execute when the button is clicked</para>
            /// <para>options.bindOnEnter: if true, bind 'Enter' key to this button.</para>
            /// </param>
            /// <returns type="button">A button object</returns>
            var cswPrivate = {
                ID: '',
                enabledText: '',
                disabledText: '',
                cssclass: '',
                hasText: true,
                disableOnClick: true,
                path: '/NbtWebApp/Images/newicons/',
                icon: Csw.enums.getName(Csw.enums.iconType, Csw.enums.iconType.none),
                onClick: null,
                isEnabled: true,
                bindOnEnter: false,
                size: 'medium',
                tooltip: {
                    title: '',
                    width: 100,
                    showDelay: 1000
                },
                editMode: ''
            };
            var cswPublic = {};

            window.Ext.require('Ext.button.*');

            cswPublic.show = Csw.method(function () {
                cswPublic.button.show();
                return cswPublic;
            });

            cswPublic.hide = Csw.method(function () {
                cswPublic.button.hide();
                return cswPublic;
            });

            cswPublic.addClass = Csw.method(function (cls) {
                cswPublic.button.addClass(cls);
                return cswPublic;
            });

            cswPublic.enable = Csw.method(function () {
                /// <summary>Enable the button.</summary>
                /// <returns type="button">The button object.</returns>
                cswPrivate.isEnabled = true;
                cswPublic.button.enable();
                cswPublic.button.setText(cswPrivate.enabledText);
                return cswPublic;
            });

            cswPublic.isDisabled = Csw.method(function () {
                return cswPublic.button.isDisabled();
            });

            cswPublic.disable = Csw.method(function () {
                /// <summary>Disable the button.</summary>
                /// <returns type="button">The button object.</returns>
                cswPrivate.isEnabled = false;
                cswPublic.button.disable();
                if (false === Csw.isNullOrEmpty(cswPrivate.disabledText)) {
                    cswPublic.button.setText(cswPrivate.disabledText);
                }
                return cswPublic;
            });

            cswPublic.click = Csw.method(function (func) {
                /// <summary>Trigger or assign a button click event.</summary>
                /// <param name="func" type="Function">(Optional) A function to bind to the control.</param>
                /// <returns type="button">The button object.</returns>
                if (Csw.isFunction(func)) {
                    cswPublic.button.click(func);
                } else {
                    if (false == cswPublic.isDisabled()) {
                        cswPublic.button.fireHandler('click');
                    }
                }
                return cswPublic;
            });

            (function () {
                if (options) {
                    Csw.extend(cswPrivate, options, true);
                }

                switch (Csw.string(cswPrivate.size, 'medium').toLowerCase()) {
                    case 'medium':
                        cswPrivate.size = 'medium';
                        cswPrivate.path += '18/';
                        break;
                    case 'small':
                        cswPrivate.size = 'small';
                        cswPrivate.path += '16/';
                        break;
                }

                var internalOnClick = Csw.makeDelegate(cswPrivate.onClick);

                var icon = '';
                if (false === Csw.isNullOrEmpty(cswPrivate.icon) && 'none' !== cswPrivate.icon) {
                    icon = cswPrivate.path + cswPrivate.icon + '.png';
                }

                function onClick() {
                    var doEnable = function () {
                        cswPublic.enable();
                        cswPublic.button.setText(cswPrivate.enabledText);
                        Csw.unsubscribe(Csw.enums.events.ajax.globalAjaxStop, doEnable);
                    };
                    /* Case 25810 */
                    if (cswPrivate.isEnabled) {
                        if (cswPrivate.disableOnClick && false === Csw.ajax.ajaxInProgress()) {
                            cswPublic.disable();
                            if (false === Csw.isNullOrEmpty(cswPrivate.disabledText)) {
                                cswPublic.button.setText(cswPrivate.disabledText);
                            }
                            Csw.subscribe(Csw.enums.events.ajax.globalAjaxStop, doEnable);
                        }
                        Csw.tryExec(internalOnClick, arguments);
                    }
                }

                cswPrivate.onClick = onClick;

                if (Csw.bool(cswPrivate.bindOnEnter)) {
                    window.Mousetrap.bind('enter', cswPrivate.onClick);
                }
                cswPrivate.initBtn = function () {
                    cswPublic.button = window.Ext.create('Ext.Button', {
                        renderTo: cswParent.getId(),
                        text: Csw.string(cswPrivate.enabledText),
                        handler: cswPrivate.onClick,
                        icon: icon,
                        cls: Csw.string(cswPrivate.cssclass),
                        scale: Csw.string(cswPrivate.size, 'medium'),
                        disabled: (Csw.enums.editMode.PrintReport === cswPrivate.editMode || Csw.enums.editMode.AuditHistoryInPopup === cswPrivate.editMode)
                    });

                    if (false === Csw.isNullOrEmpty(cswPrivate.tooltip.title)) {
                        cswPrivate.tooltip.target = cswPublic.button.getId();
                        window.Ext.create('Ext.tip.ToolTip', cswPrivate.tooltip);
                        window.Ext.QuickTips.init();
                    }
                };
                if (false === Csw.isNullOrEmpty($('#' + cswParent.getId()), true)) {
                    cswPrivate.initBtn();
                } else {
                    cswPublic.button = window.Ext.create('Ext.Button');
                    window.setTimeout(function () {
                        if (false === Csw.isNullOrEmpty($('#' + cswParent.getId()), true)) {
                            cswPrivate.initBtn();
                        }
                    }, 500);
                }
            } ());

            return cswPublic;

        });

} ());

