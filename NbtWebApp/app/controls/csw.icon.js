/// <reference path="~/app/CswApp-vsdoc.js" />


(function _cswIcon() {
    "use strict";

    Csw.controls.icon = Csw.controls.icon ||
        Csw.controls.register('icon', function (cswParent, options) {
            ///<summary>Generates an icon</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach icon to.</param>
            ///<param name="options" type="Object">Object defining paramaters for icon construction.</param>
            ///<returns type="Csw.controls.icon">Object representing an icon</returns>
            'use strict';

            var cswPrivate = {
                name: '',
                hovertext: '',
                iconType: Csw.enums.iconType.none,
                state: Csw.enums.iconState.normal,
                isButton: false,
                onClick: null,
                size: 18, // 16, 18, or 100

                iconFilePrefix: 'Images/newicons/icons',
                iconFileSuffix: '.png'
            };
            var cswPublic = cswParent.div({ text: '&nbsp;' });

            cswPrivate.url = function () {
//                // Case 24112: IE7 processes url() using https but handles the response as http--prompting the security dialog.
//                var port = document.location.port;
//                var prefix = document.location.protocol + "//" + document.location.hostname;
//                if (false === Csw.isNullOrEmpty(port) && port !== 80) {
//                    prefix += ':' + port;
//                }
//                prefix += '/NbtWebApp';

//                return prefix + '/' + cswPrivate.iconFilePrefix + cswPrivate.size + cswPrivate.iconFileSuffix;
                return cswPrivate.iconFilePrefix + cswPrivate.size + cswPrivate.iconFileSuffix;
            }; // url()

            cswPrivate.offsetCss = function (state) {
                var multiplier = -1 * cswPrivate.size;

                return {
                    'background-position': (state * multiplier) + 'px ' + (cswPrivate.iconType * multiplier) + 'px',
                    'background-repeat': 'no-repeat'
                };
            }; // offsetCss()

            cswPublic.setType = function (newType) {
                if (false === Csw.isNullOrEmpty(newType) && newType != Csw.enums.iconType.none) {
                    cswPrivate.iconType = newType;

                    cswPublic.css({
                        background: 'url(\'' + cswPrivate.url() + '\')'
                    });
                    cswPublic.css(cswPrivate.offsetCss(cswPrivate.state));

                    cswPublic.unbind('mouseover');
                    cswPublic.unbind('mouseout');
                    cswPublic.unbind('mousedown');
                    cswPublic.unbind('mouseup');

                    if (cswPrivate.isButton && cswPrivate.state != Csw.enums.iconState.disabled) {
                        cswPublic.bind('mouseover', function () {
                            cswPublic.css(cswPrivate.offsetCss(Csw.enums.iconState.hover));
                        });
                        cswPublic.bind('mouseout', function () {
                            cswPublic.css(cswPrivate.offsetCss(cswPrivate.state));
                        });
                        cswPublic.bind('mousedown', function () {
                            cswPublic.css(cswPrivate.offsetCss(Csw.enums.iconState.selected));
                        });
                        cswPublic.bind('mouseup', function () {
                            cswPublic.css(cswPrivate.offsetCss(Csw.enums.iconState.hover));
                        });
                    }

                } // if( false === Csw.isNullOrEmpty(newType) && newType != Csw.enums.iconType.none )
            }; // setType()

            cswPublic.getType = function () {
                return cswPrivate.iconType;
            }; // getType()

            // Constructor
            (function () {
                if (options) Csw.extend(cswPrivate, options);

                cswPublic.css({
                    display: 'inline-block',
                    width: cswPrivate.size,
                    height: cswPrivate.size
                });
                cswPublic.propNonDom('title', cswPrivate.hovertext);

                // Case 24112: IE7 processes url() using https but handles the response as http--prompting the security dialog.

                cswPublic.bind('click', function () {
                    Csw.tryExec(cswPrivate.onClick);
                });
                cswPublic.setType(cswPrivate.iconType);
            } ());

            return cswPublic;
        });

} ());