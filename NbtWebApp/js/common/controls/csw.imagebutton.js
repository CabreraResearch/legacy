/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {


    Csw.components.imageButton = Csw.components.imageButton ||
        Csw.components.register('imageButton', function (cswParent, options) {
            ///<summary>Generates an imageButton</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach imageButton to.</param>
            ///<param name="options" type="Object">Object defining paramaters for imageButton construction.</param>
            ///<returns type="Csw.components.imageButton">Object representing an imageButton</returns>
            'use strict';
            var internal = {
                $parent: '',
                ButtonType: Csw.enums.imageButton_ButtonType.None,
                Active: false,
                AlternateText: '',
                ID: '',
                cssclass: '',
                Required: false,
                onClick: function () {
                    return Csw.enums.imageButton_ButtonType.None;
                }
            };
            var external = {};

            internal.setButton = function (newButtonType) {
                var multiplier = -18;
                //Case 24112: IE7 processes url() using https but randles the response as http--prompting the security dialog.
                var port = document.location.port;
                var prefix = document.location.protocol + "//" + document.location.hostname;
                if (false === Csw.isNullOrEmpty(port) && port !== 80) {
                    prefix += ':' + port;
                }
                prefix += '/NbtWebApp';
                if (newButtonType !== undefined && newButtonType !== Csw.enums.imageButton_ButtonType.None) {
                
                var offset = 0;
                if(internal.Active) {
                    offset = -36;
                }
                external.$.get(0).style.background = 'url(\'' + prefix + '/Images/buttons/buttons18.gif\') '+ offset +'px ' + newButtonType * multiplier + 'px no-repeat';
                
                    external.unbind('mouseover');
                    external.unbind('mouseout');
                    external.unbind('mousedown');
                    external.unbind('mouseup');
                    external.bind('mouseover', function () {
                        external.css('background-position', '-18px ' + newButtonType * multiplier + 'px');
                    });
                    external.bind('mouseout', function () {
                    external.css('background-position', offset + 'px ' + newButtonType * multiplier + 'px');
                    });
                    external.bind('mousedown', function () {
                        external.css('background-position', '-36px ' + newButtonType * multiplier + 'px');
                    });
                    external.bind('mouseup', function () {
                        external.css('background-position', '-18px ' + newButtonType * multiplier + 'px');
                    });
                }
                return false;
            };

            external.click = function (newButtonType, func) {
                if (Csw.isFunction(func)) {
                    return external.bind('click', func);
                } else {
                    return internal.setButton(newButtonType);
                }
            };

            (function () {
                if (options) {
                    $.extend(internal, options);
                }

                internal.imageButton = cswParent.div(internal);
                external = Csw.dom(external, internal.imageButton);
                
                //$.extend(external, Csw.literals.div(internal));
                external.addClass('divbutton');
                external.propNonDom('title', internal.AlternateText);
                external.css('display', 'inline-block');

                internal.setButton(internal.ButtonType);
                external.bind('click', function () {
                    return internal.setButton();
                });
            } ());

            return external;
        });


} ());
