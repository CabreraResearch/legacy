/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {


    Csw.controls.imageButton = Csw.controls.imageButton ||
        Csw.controls.register('imageButton', function(cswParent, options) {
            ///<summary>Generates an imageButton</summary>
            ///<param name="cswParent" type="Csw.literals">Parent element to attach imageButton to.</param>
            ///<param name="options" type="Object">Object defining paramaters for imageButton construction.</param>
            ///<returns type="Csw.controls.imageButton">Object representing an imageButton</returns>
            'use strict';
            var cswPrivateVar = {
                ButtonType: Csw.enums.imageButton_ButtonType.None,
                Active: false,
                AlternateText: '',
                ID: '',
                cssclass: '',
                Required: false,
                onClick: null
            };
            var cswPublicRet = { };

            (function() {
                if (options) {
                    $.extend(cswPrivateVar, options);
                }

                cswPrivateVar.imageButton = cswParent.div(cswPrivateVar);
                cswPublicRet = Csw.dom({ }, cswPrivateVar.imageButton);
            }());

            cswPublicRet.setButtonType = function(newButtonType) {
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
                    if (cswPrivateVar.Active) {
                        offset = -36;
                    }
                    cswPublicRet.$.get(0).style.background = 'url(\'' + prefix + '/Images/buttons/buttons18.gif\') ' + offset + 'px ' + newButtonType * multiplier + 'px no-repeat';

                    cswPublicRet.unbind('mouseover');
                    cswPublicRet.unbind('mouseout');
                    cswPublicRet.unbind('mousedown');
                    cswPublicRet.unbind('mouseup');
                    cswPublicRet.bind('mouseover', function() {
                        cswPublicRet.css('background-position', '-18px ' + newButtonType * multiplier + 'px');
                    });
                    cswPublicRet.bind('mouseout', function() {
                        cswPublicRet.css('background-position', offset + 'px ' + newButtonType * multiplier + 'px');
                    });
                    cswPublicRet.bind('mousedown', function() {
                        cswPublicRet.css('background-position', '-36px ' + newButtonType * multiplier + 'px');
                    });
                    cswPublicRet.bind('mouseup', function() {
                        cswPublicRet.css('background-position', '-18px ' + newButtonType * multiplier + 'px');
                    });
                }
                cswPrivateVar.ButtonType = newButtonType;
                return false;
            };

            cswPublicRet.getButtonType = function() {
                return cswPrivateVar.ButtonType;
            };


            cswPublicRet.click = function(newButtonType, func) {
                if (Csw.isFunction(func)) {
                    return cswPublicRet.bind('click', func);
                } else {
                    return cswPublicRet.setButtonType(newButtonType);
                }
            };

            (function() {
                cswPublicRet.addClass('divbutton');
                cswPublicRet.propNonDom('title', cswPrivateVar.AlternateText);
                cswPublicRet.css('display', 'inline-block');
                cswPublicRet.setButtonType(cswPrivateVar.ButtonType);
            }());
            return cswPublicRet;
        });


} ());
