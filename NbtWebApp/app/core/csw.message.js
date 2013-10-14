(function () {
    'use strict';

    Csw.message.register('showMessage', function (messageJson, friendlyMsg, esotericMsg) {
        'use strict';
        /// <summary>Displays a message that doesn't interfere with success of the method.</summary>
        /// <param name="messageJson" type="Object/String"></param>
        /// <param name="friendlyMsg" type="String"> A friendly message to display.</param>
        /// <param name="esotericMsg" type="String"> A verbose message for developers.</param>
        ///<returns type="Boolean">True</returns>
        var e = {
            'name': '',
            'type': '',
            'message': friendlyMsg,
            'detail': esotericMsg,
            'display': true
        };
        if (Csw.isPlainObject(messageJson)) {
            Csw.extend(e, messageJson);
        } else {
            e.type = messageJson;
        }

        var $messagediv = $('#DialogErrorDiv');
        if ($messagediv.length <= 0) {
            $messagediv = $('#ErrorDiv');
        }

        if ($messagediv.length > 0 && Csw.bool(e.display)) {
            $messagediv.CswErrorMessage(
                {
                    'name': e.name,
                    'type': e.type,
                    'message': e.message,
                    'detail': e.detail
                });
        }
        Csw.debug.error(e.message + '; ' + e.detail);
        return true;
    });

}());
