/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.moreDiv = Csw.controls.moreDiv ||
        Csw.controls.register('moreDiv', function (cswParent, options) {
            'use strict';
            var internal = {
                ID: '',
                moretext: 'more',
                lesstext: 'less'
            };
            if (options) {
                $.extend(internal, options);
            }

            var external;

            internal.moreDiv = cswParent.div();
            external = Csw.dom({}, internal.moreDiv);

            external.shownDiv = internal.moreDiv.div({
                ID: Csw.makeId(internal.ID, '', '_shwn')
            });

            external.hiddenDiv = internal.moreDiv.div({
                ID: Csw.makeId(internal.ID, '', '_hddn')
            }).hide();

            external.moreLink = internal.moreDiv.link({
                ID: Csw.makeId(internal.ID, '', '_more'),
                text: internal.moretext,
                cssclass: 'morelink',
                onClick: function () {
                    if (external.moreLink.toggleState === Csw.enums.toggleState.on) {
                        external.moreLink.text(internal.lesstext);
                        external.hiddenDiv.show();
                    } else {
                        external.moreLink.text(internal.moretext);
                        external.hiddenDiv.hide();
                    }
                    return false;
                } // onClick()
            });

            return external;
        });

} ());
