/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.literals.moreDiv = Csw.literals.moreDiv ||
        Csw.literals.register('moreDiv', function (options) {
            'use strict';
            var cswPrivate = {
                ID: '',
                $parent: '',
                moretext: 'more',
                lesstext: 'less'
            };
            if (options) Csw.extend(cswPrivate, options);

            var cswPublic = {};

            cswPublic.shownDiv = Csw.literals.div({
                ID: Csw.makeId(cswPrivate.ID, '', '_shwn'),
                $parent: cswPrivate.$parent
            });

            cswPublic.hiddenDiv = Csw.literals.div({
                ID: Csw.makeId(cswPrivate.ID, '', '_hddn'),
                $parent: cswPrivate.$parent
            }).hide();

            cswPublic.moreLink = Csw.literals.a({
                ID: Csw.makeId(cswPrivate.ID, '', '_more'),
                $parent: cswPrivate.$parent,
                text: cswPrivate.moretext,
                cssclass: 'morelink',
                onClick: function () {
                    if (cswPublic.moreLink.toggleState === Csw.enums.toggleState.on) {
                        cswPublic.moreLink.text(cswPrivate.lesstext);
                        cswPublic.hiddenDiv.show();
                    } else {
                        cswPublic.moreLink.text(cswPrivate.moretext);
                        cswPublic.hiddenDiv.hide();
                    }
                    return false;
                } // onClick()
            });

            return cswPublic;
        });

} ());
