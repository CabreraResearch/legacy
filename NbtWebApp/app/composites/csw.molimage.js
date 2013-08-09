
/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    
    var getMolImgFromText = function (nodeId, molTxt, onSuccess) {
        /// <summary>
        /// Takes a MOL string and queries the Mol webservice for a thumbnail
        /// </summary>
        /// 
        /// <param name="nodeId"></param>
        /// <param name="molTxt">A string of mol data to be thumbnailed</param>
        /// <param name="onSuccess">callback function handling result, which must accept one parameter (dictionary with result information)</param>

        Csw.ajaxWcf.post({
            urlMethod: 'Mol/getImg',
            data: {
                molString: molTxt,
                nodeId: nodeId,
                molImgAsBase64String: ''
            },
            success: function (data) {
                onSuccess(data);
            }

        });
    };

    Csw.register('getMolImgFromText', getMolImgFromText);
    Csw.getMolImgFromText = Csw.getMolImgFromText || getMolImgFromText;
    
}());
