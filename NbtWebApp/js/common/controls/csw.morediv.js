/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.moreDiv = Csw.controls.moreDiv ||
        Csw.controls.register('moreDiv', function (cswParent, options) {
            'use strict';
            var cswPrivateVar = {
                ID: '',
                moretext: 'more',
                lesstext: 'less'
            };
            if (options) {
                $.extend(cswPrivateVar, options);
            }

            var cswPublicRet;

            cswPrivateVar.moreDiv = cswParent.div();
            cswPublicRet = Csw.dom({}, cswPrivateVar.moreDiv);

            cswPublicRet.shownDiv = cswPrivateVar.moreDiv.div({
                ID: Csw.makeId(cswPrivateVar.ID, '', '_shwn')
            });

            cswPublicRet.hiddenDiv = cswPrivateVar.moreDiv.div({
                ID: Csw.makeId(cswPrivateVar.ID, '', '_hddn')
            }).hide();

            cswPublicRet.moreLink = cswPrivateVar.moreDiv.a({
                ID: Csw.makeId(cswPrivateVar.ID, '', '_more'),
                text: cswPrivateVar.moretext,
                cssclass: 'morelink',
                onClick: function () {
                    if (cswPublicRet.moreLink.toggleState === Csw.enums.toggleState.on) {
                        cswPublicRet.moreLink.text(cswPrivateVar.lesstext);
                        cswPublicRet.hiddenDiv.show();
                    } else {
                        cswPublicRet.moreLink.text(cswPrivateVar.moretext);
                        cswPublicRet.hiddenDiv.hide();
                    }
                    return false;
                } // onClick()
            });

            return cswPublicRet;
        });

} ());
