/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.literals.moreDiv = Csw.literals.moreDiv ||
        Csw.literals.register('moreDiv', function (options) {
            'use strict';
            var cswPrivateVar = {
                ID: '',
                $parent: '',
                moretext: 'more',
                lesstext: 'less'
            };
            if (options) $.extend(cswPrivateVar, options);

            var cswPublicRet = {};

            cswPublicRet.shownDiv = Csw.literals.div({
                ID: Csw.makeId(cswPrivateVar.ID, '', '_shwn'),
                $parent: cswPrivateVar.$parent
            });

            cswPublicRet.hiddenDiv = Csw.literals.div({
                ID: Csw.makeId(cswPrivateVar.ID, '', '_hddn'),
                $parent: cswPrivateVar.$parent
            }).hide();

            cswPublicRet.moreLink = Csw.literals.a({
                ID: Csw.makeId(cswPrivateVar.ID, '', '_more'),
                $parent: cswPrivateVar.$parent,
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
