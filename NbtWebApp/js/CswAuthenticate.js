
function authenticate(AccessId, UserName, Password, onsuccess) {
    $.ajax({
        type: 'POST',
        url: '/NbtWebApp/wsNBT.asmx/Authenticate',
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: "{AccessId: '" + AccessId + "', UserName: '" + UserName + "', Password: '" + Password + "'}",
        success: function (data, textStatus, XMLHttpRequest) {
            var $xml = $(data.d);
            if ($xml.get(0).nodeName == "ERROR") {
                _handleAjaxError(XMLHttpRequest, $xml.text(), '');
            } else {
                SessionId = $xml.find('SessionId').text();
                if (SessionId != "") {

                    onsuccess(SessionId);

                    //                        updateTimer("Authentication", starttime, new Date());
                    //                        getViewSelect();
                } // if (SessionId != "")
                else {
                    //_handleAuthenticationStatus($xml.find('AuthenticationStatus').text());
                }
            }
        }, // success{}
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            //_handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
        }
    });  // $.ajax({
} // authenticate()
